namespace NoCrosshair;

using HarmonyLib;using BepInEx;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.SoftDependency)]
public class Main: BaseUnityPlugin
{
    private void Awake()
    {
        Harmony.CreateAndPatchAll(typeof(Patches.Patches), MyPluginInfo.PLUGIN_GUID);
    }
}