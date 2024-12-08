using BepInEx;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CrimsonClans.Structs;

public readonly struct Settings
{

    public static ConfigEntry<bool> LockMembers;
    public static ConfigEntry<int> PreRaidBuffer;
    public static ConfigEntry<int> PostRaidBuffer;

    public static ConfigEntry<int> LeaveCooldown;

    public static ConfigEntry<int> HeartsPerClan;

    private static readonly List<string> OrderedSections = new()
    {
        "Raid Time Lock",
        "Cooldowns",
        "Limits",
    };

    public static void InitConfig()
    {
        foreach (string path in directoryPaths)
        {
            CreateDirectories(path);
        }

        LockMembers = InitConfigEntry(OrderedSections[0], "LockMembers", true,
            "If this is set to true, members will be unable to invite/kick/join clans during raid time.");

        PreRaidBuffer = InitConfigEntry(OrderedSections[0], "PreRaidBufferMins", 30,
            "The number of minutes before raid time that members will be unable to invite/kick/join clans.");

        PostRaidBuffer = InitConfigEntry(OrderedSections[0], "PostRaidBufferMins", 0,
            "The number of minutes after raid time that members will be unable to invite/kick/join clans.");

        LeaveCooldown = InitConfigEntry(OrderedSections[1], "JoinCooldown", 0,
            "Length of time in minutes that a player must wait before they can join a clan after leaving their previous.");

        HeartsPerClan = InitConfigEntry(OrderedSections[2], "HeartsPerClan", 1, 
            "The amount of castle hearts a clan can have.");
    
        ReorderConfigSections();
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

    private static void ReorderConfigSections()
    {
        var configPath = Path.Combine(Paths.ConfigPath, $"{MyPluginInfo.PLUGIN_GUID}.cfg");
        if (!File.Exists(configPath)) return;

        var lines = File.ReadAllLines(configPath).ToList();
        var sectionsContent = new Dictionary<string, List<string>>();
        string currentSection = "";

        // Parse existing file
        foreach (var line in lines)
        {
            if (line.StartsWith("["))
            {
                currentSection = line.Trim('[', ']');
                sectionsContent[currentSection] = new List<string> { line };
            }
            else if (!string.IsNullOrWhiteSpace(currentSection))
            {
                sectionsContent[currentSection].Add(line);
            }
        }

        // Rewrite file in ordered sections
        using var writer = new StreamWriter(configPath, false);
        foreach (var section in OrderedSections)
        {
            if (sectionsContent.ContainsKey(section))
            {
                foreach (var line in sectionsContent[section])
                {
                    writer.WriteLine(line);
                }
                writer.WriteLine(); // Add spacing between sections
            }
        }
    }

    static readonly List<string> directoryPaths =
        [
            Plugin.ConfigFiles
        ];
}
