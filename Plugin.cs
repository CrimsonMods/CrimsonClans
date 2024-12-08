using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using CrimsonClans.Structs;
using HarmonyLib;
using System.IO;
using System.Reflection;

namespace CrimsonClans;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    Harmony _harmony;
    internal static Plugin Instance { get; private set; }
    public static Settings Settings { get; private set; }
    public static Harmony Haromy => Instance._harmony;
    public static ManualLogSource LogInstance => Instance.Log;

    public static readonly string ConfigFiles = Path.Combine(Paths.ConfigPath, MyPluginInfo.PLUGIN_NAME);

    public override void Load()
    {
        Instance = this;
        Settings = new Settings();

        _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        Settings.InitConfig();
    }

    public override bool Unload()
    {
        _harmony?.UnpatchSelf();
        return true;
    }
}
