using System;
using System.Linq;
using CrimsonClans.Services;
using CrimsonClans.Structs;
using CrimsonClans.Utilities;
using HarmonyLib;
using ProjectM;
using ProjectM.Gameplay.Clan;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;

namespace CrimsonClans.Patches;

[HarmonyPatch(typeof(ClanSystem_Server), nameof(ClanSystem_Server.OnUpdate))]
internal class ClanSystemServerPatch
{
    public static void Prefix(ClanSystem_Server __instance)
    {
        var queries = new[]
        {
            (__instance._InvitePlayerToClanQuery, "Invite"),
            (__instance._ClanInviteResponseQuery, "Response"),
            (__instance._KickRequestQuery, "Kick"),
            (__instance._CreateClanEventQuery, "Create"),
            (__instance._LeaveClanEventQuery, "Leave"),
            (__instance._EditClanEventQuery, "Edit")
        };

        foreach (var (query, type) in queries)
        {
            var entities = query.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                if (HandleRaidWindow(entity, type)) continue;
                if (HandleLeaveCooldown(entity, type)) continue;
                if (HandleJoinResponseWithCooldown(entity, type)) continue;
            }
            entities.Dispose();
        }
    }

    private static bool HandleRaidWindow(Entity entity, string type)
    {
        if (type == "Invite" && !Settings.LockInvite.Value) return false;
        if (type == "Response" && !Settings.LockInvite.Value) return false;
        if (type == "Kick" && !Settings.LockKick.Value) return false;
        if (type == "Create" && !Settings.LockCreate.Value) return false;
        if (type == "Leave" && !Settings.LockLeave.Value) return false;
        if (type == "Edit" && !Settings.LockEdit.Value) return false;

        if (RaidTime.IsRaidTimeNow())
        {
            Cancel(entity, $"The Clan {type} ability is disabled during the raid window.");
            return true;
        }

        return false;
    }

    private static bool HandleLeaveCooldown(Entity entity, string type)
    {
        if (type != "Leave") return false;
        if (Settings.LeaveCooldown.Value == 0) return false;

        var fromCharacter = entity.Read<FromCharacter>();
        var user = fromCharacter.User.Read<User>();

        Core.DB.Cooldowns.Add(new(user.PlatformId, DateTime.Now.AddMinutes(Settings.LeaveCooldown.Value)));
        Database.SaveFiles();
        return true;
    }

    private static bool HandleJoinResponseWithCooldown(Entity entity, string type)
    {
        if (type != "Response") return false;
        var inviteResponse = entity.Read<ClanEvents_Client.ClanInviteResponse>();
        if (!inviteResponse.Response.Equals(InviteRequestResponse.Accept)) return false;

        var fromCharacter = entity.Read<FromCharacter>();
        var user = fromCharacter.User.Read<User>();

        if (Settings.LeaveCooldown.Value != 0 && Core.DB.Cooldowns.Exists(x => x.PlayerId == user.PlatformId && x.Time > DateTime.Now))
        {
            Cooldown cooldown = Core.DB.Cooldowns.First(x => x.PlayerId == user.PlatformId && x.Time > DateTime.Now);
            Cancel(entity, $"You are on cooldown from joining a clan for {FormatRemainder(cooldown.Time - DateTime.Now)}.");
            return true;
        }

        if (!CastleHeartService.TryGetClanByID(inviteResponse.ClanId, out var clan)) return false;

        if (!CastleHeartService.CanJoinClan(fromCharacter.Character, clan))
        {
            Cancel(entity, "Joining the clan would result in exceeding the maximum number of castle hearts.");
            return true;
        }

        return false;
    }

    private static void Cancel(Entity entity, string cancelReason)
    {
        var fromCharacter = Core.EntityManager.GetComponentData<FromCharacter>(entity);
        var user = Core.EntityManager.GetComponentData<User>(fromCharacter.User);

        ServerChatUtils.SendSystemMessageToClient(Core.EntityManager, user, cancelReason);

        entity.DestroyWithReason();
    }

    private static string FormatRemainder(TimeSpan remainder)
    {
        string formattedRemainder = string.Empty;
        if (remainder.Days > 0)
            formattedRemainder += $"{remainder.Days} days, ";
        if (remainder.Hours > 0)
            formattedRemainder += $"{remainder.Hours} hours, ";
        if (remainder.Minutes > 0)
            formattedRemainder += $"{remainder.Minutes} minutes";

        if (formattedRemainder.EndsWith(", "))
            formattedRemainder = formattedRemainder.Substring(0, formattedRemainder.Length - 2);

        return formattedRemainder;
    }
}
