using BepInEx;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CrimsonClans.Structs;

public readonly struct Settings
{
    // Operations
    public static ConfigEntry<bool> LockInvite;
    public static ConfigEntry<bool> LockCreate;
    public static ConfigEntry<bool> LockLeave;
    public static ConfigEntry<bool> LockKick;
    public static ConfigEntry<bool> LockEdit;

    public static ConfigEntry<int> PreRaidBuffer;
    public static ConfigEntry<int> PostRaidBuffer;

    public static ConfigEntry<int> LeaveCooldown;

    public static ConfigEntry<int> HeartsPerClan;

    private static readonly List<string> OrderedSections = new()
    {
        "General",
        "Raid Time - Buffers",
        "Raid Time - Ability Locks",
        "Cooldowns",
    };

    public static void InitConfig()
    {
        foreach (string path in directoryPaths)
        {
            CreateDirectories(path);
        }

        HeartsPerClan = InitConfigEntry(OrderedSections[0], "HeartsPerClan", 1,
            "The amount of castle hearts a clan can have.");

        PreRaidBuffer = InitConfigEntry(OrderedSections[1], "PreRaidBufferMins", 30,
            "The number of minutes prior to the raid window to lock clan operation.");

        PostRaidBuffer = InitConfigEntry(OrderedSections[1], "PostRaidBufferMins", 0,
            "The number of minutes past the raid window to lock clan operation.");

        LockInvite = InitConfigEntry(OrderedSections[2], "Invite", true,
            "If this is set to true, clans will be unable to invite players to their clan during raid time.");
        LockCreate = InitConfigEntry(OrderedSections[2], "Create", true,
            "If this is set to true, clans will be unable to be created during raid time.");
        LockEdit = InitConfigEntry(OrderedSections[2], "Edit", true,
            "If this is set to true, clans will not be able to change their details during raid time.");
        LockKick = InitConfigEntry(OrderedSections[2], "Kick", false,
            "If this is set to true, clans will be unable to kick players from clan during raid time.");
        LockLeave = InitConfigEntry(OrderedSections[2], "Leave", false,
            "If this is set to true, players will be unable to leave clans during raid time.");
        
        LeaveCooldown = InitConfigEntry(OrderedSections[3], "JoinCooldown", 0,
            "The number of minutes that a player must wait to join a clan after leaving a prior clan.");

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
