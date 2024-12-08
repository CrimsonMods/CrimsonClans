using System;
using System.Linq;
using CrimsonClans.Structs;
using CrimsonClans.Utilities;
using HarmonyLib;
using ProjectM;
using ProjectM.Gameplay.Clan;
using ProjectM.Network;
using ProjectM.Shared;
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
            (__instance._EditClanEventQuery, "Edit"),
        };

        foreach (var (query, type) in queries)
        {
            var entities = query.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                if (HandleRaidWindow(entity)) continue;
                if (HandleLeaveCooldown(entity, type)) continue;
                if (HandleJoinResponseWithCooldown(entity, type)) continue;
            }
            entities.Dispose();
        }
    }

    private static bool HandleRaidWindow(Entity entity)
    {
        if (!Settings.LockMembers.Value) return false;
        if (RaidTime.IsRaidTimeNow())
        {
            Cancel(entity, "Clan operations are disabled during the raid window.");
            return true;
        }

        return false;
    }

    private static bool HandleLeaveCooldown(Entity entity, string type)
    {
        if (type != "Leave") return false;
        if (Settings.LeaveCooldown.Value == 0) return false;

        var fromCharacter = Core.EntityManager.GetComponentData<FromCharacter>(entity);
        var user = Core.EntityManager.GetComponentData<User>(fromCharacter.User);

        Core.DB.Cooldowns.Add(new(user.PlatformId, DateTime.Now.AddMinutes(Settings.LeaveCooldown.Value)));
        return true;
    }

    private static bool HandleJoinResponseWithCooldown(Entity entity, string type)
    {
        if (type != "Response") return false;
        if (Settings.LeaveCooldown.Value == 0) return false;

        var fromCharacter = Core.EntityManager.GetComponentData<FromCharacter>(entity);
        var user = Core.EntityManager.GetComponentData<User>(fromCharacter.User);

        bool isCooldown = Core.DB.Cooldowns.Any(x => x.Item1 == user.PlatformId && x.Item2 > DateTime.Now);

        if (isCooldown)
        {
            Cancel(entity, "You are on cooldown from joining a clan.");
            return true;
        }

        return false;
    }

    private static void Cancel(Entity entity, string cancelReason)
    {
        var fromCharacter = Core.EntityManager.GetComponentData<FromCharacter>(entity);
        var user = Core.EntityManager.GetComponentData<User>(fromCharacter.User);

        ServerChatUtils.SendSystemMessageToClient(Core.EntityManager, user, cancelReason);

        Core.EntityManager.AddComponent<Disabled>(entity);
        DestroyUtility.CreateDestroyEvent(Core.EntityManager, entity, DestroyReason.Default, DestroyDebugReason.ByScript);
        DestroyUtility.Destroy(Core.EntityManager, entity);
    }
}
