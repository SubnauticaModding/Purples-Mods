﻿namespace ExtraOptions
{
    using Configuration;
    using HarmonyLib;
    using QModManager.API.ModLoading;
    using SMLHelper.V2.Handlers;
    using System.IO;
    using System.Reflection;
    using UnityEngine;

    [QModCore]
    public static class Main
    {
        private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
        internal static readonly string modPath = Path.GetDirectoryName(assembly.Location);
        internal static Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        [QModPatch]
        public static void Load()
        {
            var harmony = Harmony.CreateAndPatchAll(assembly, "com.m22spencer.extraOptions");

            // When a preset is selected, the texture quality is also set, reload settings here to override this
            harmony.Patch(AccessTools.Method(typeof(uGUI_OptionsPanel), nameof(uGUI_OptionsPanel.SyncQualityPresetSelection)), postfix: new HarmonyMethod(typeof(Main).GetMethod(nameof(ApplyOptions))));
            harmony.Patch(AccessTools.Method(typeof(Player), nameof(Player.Start)), postfix: new HarmonyMethod(typeof(Main).GetMethod(nameof(ApplyOptions))));
            harmony.Patch(AccessTools.Method(typeof(MainMenuController), nameof(MainMenuController.Start)), postfix: new HarmonyMethod(typeof(Main).GetMethod(nameof(ApplyOptions))));
        }

        public static void ApplyOptions()
        {
            try
            {

                QualitySettings.masterTextureLimit = 4 - Config.TextureQuality;

                switch(Config.ShadowCascades)
                {
                    case 0:
                        QualitySettings.shadowCascades = 1;
                        break;
                    case 1:
                        QualitySettings.shadowCascades = 2;
                        break;
                    case 2:
                        QualitySettings.shadowCascades = 4;
                        break;
                }

                Shader.globalMaximumLOD = Config.ShaderLOD;
                QualitySettings.lodBias = Config.LODGroupBias;

                foreach(var s in Object.FindObjectsOfType<WaterSunShaftsOnCamera>() ?? new WaterSunShaftsOnCamera[0])
                    s.enabled = Config.LightShafts;

                foreach(var p in Object.FindObjectsOfType<AmbientParticles>() ?? new AmbientParticles[0])
                    p.enabled = Config.AmbientParticles;

                if(!Config.VariablePhysicsStep)
                {
                    Time.fixedDeltaTime = 0.02f;
                    Time.maximumDeltaTime = 0.33333f;
                    Time.maximumParticleDeltaTime = 0.03f;
                }

                foreach(var w in Object.FindObjectsOfType<WaterBiomeManager>() ?? new WaterBiomeManager[0])
                    w.Rebuild();

                Config.Save();
            }
            catch
            {
                // ignored
            }
        }

    }
}