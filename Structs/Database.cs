using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CrimsonClans.Structs;

public class Database
{
    public static string JoinCooldownPath = Path.Combine(Plugin.ConfigFiles, "joincooldowns.json");

    public List<(ulong, DateTime)> Cooldowns { get; set; }

    public Database()
    {
        CreateFiles();
        LoadFiles();
    }

    private void LoadFiles()
    {
        string json = "";
        if(Settings.LeaveCooldown.Value > 0)
        {
            json = File.ReadAllText(JoinCooldownPath);
            Cooldowns = JsonSerializer.Deserialize<List<(ulong, DateTime)>>(json);
        }
    }

    private static void CreateFiles()
    {
        if(!File.Exists(JoinCooldownPath) && Settings.LeaveCooldown.Value > 0)
        {
            List<(ulong, DateTime)> template = new List<(ulong, DateTime)>();

            var json = JsonSerializer.Serialize(template);

            File.WriteAllText(JoinCooldownPath, json);
        }
    }
}