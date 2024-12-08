using BepInEx;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.IO;

namespace CrimsonClans.Structs;

public readonly struct Settings
{
    public static ConfigEntry<bool> LockMembers;
    public static ConfigEntry<int> PreRaidBuffer;
    public static ConfigEntry<int> PostRaidBuffer;

    public static ConfigEntry<int> LeaveCooldown;

    public static void InitConfig()
    {
        foreach (string path in directoryPaths)
        {
            CreateDirectories(path);
        }

        LockMembers = InitConfigEntry("Raid Time Lock", "LockMembers", true,
            "If this is set to true, members will be unable to invite/kick/join clans during raid time.");

        PreRaidBuffer = InitConfigEntry("Raid Time Lock", "PreRaidBufferMins", 30,
            "The number of minutes before raid time that members will be unable to invite/kick/join clans.");

        PostRaidBuffer = InitConfigEntry("RaidTimeLock", "PostRaidBufferMins", 0,
            "The number of minutes after raid time that members will be unable to invite/kick/join clans.");

        LeaveCooldown = InitConfigEntry("Cooldowns", "JoinCooldown", 0,
            "Length of time in minutes that a player must wait before they can join a clan after leaving their previous.");
    }

    static ConfigEntry<T> InitConfigEntry<T>(string section, string key, T defaultValue, string description)
    {
        // Bind the configuration entry and get its value
        var entry = Plugin.Instance.Config.Bind(section, key, defaultValue, description);

        // Check if the key exists in the configuration file and retrieve its current value
        var newFile = Path.Combine(Paths.ConfigPath, $"{MyPluginInfo.PLUGIN_GUID}.cfg");

        if (File.Exists(newFile))
        {
            var config = new ConfigFile(newFile, true);
            if (config.TryGetEntry(section, key, out ConfigEntry<T> existingEntry))
            {
                // If the entry exists, update the value to the existing value
                entry.Value = existingEntry.Value;
            }
        }
        return entry;
    }

    static void CreateDirectories(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    static readonly List<string> directoryPaths =
        [
            Plugin.ConfigFiles
        ];
}
