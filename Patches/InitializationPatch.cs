using HarmonyLib;
using Unity.Scenes;

namespace CrimsonClans.Patches;

[HarmonyPatch]
internal static class InitializationPatch
{
    [HarmonyPatch(typeof(SceneSystem), nameof(SceneSystem.ShutdownStreamingSupport))]
    [HarmonyPostfix]
    static void ShutdownStreamingSupportPostfix()
    {
        Core.InitializeAfterLoaded();
        if (Core._hasInitialized)
        {
            Plugin.Haromy.Unpatch(typeof(SceneSystem).GetMethod("ShutdownStreamingSupport"), typeof(InitializationPatch).GetMethod("OneShot_AfterLoad_InitializationPatch"));
        }
    }
}