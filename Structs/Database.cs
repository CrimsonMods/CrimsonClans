using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CrimsonClans.Structs;

public class Database
{
    public static string JoinCooldownPath = Path.Combine(Plugin.ConfigFiles, "joincooldowns.json");

    public List<Cooldown> Cooldowns { get; set; }

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
            Cooldowns = JsonSerializer.Deserialize<List<Cooldown>>(json);
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

    public static void SaveFiles()
    {
        var json = JsonSerializer.Serialize(Core.DB.Cooldowns);
        File.WriteAllText(JoinCooldownPath, json);
    }
}

[Serializable]
public struct Cooldown
{
    public ulong PlayerId { get; set; }
    public DateTime Time { get; set; }

    public Cooldown(ulong playerId, DateTime time)
    {
        PlayerId = playerId;
        Time = time;
    }
}