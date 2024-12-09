using CrimsonClans.Services;
using HarmonyLib;
using ProjectM;
using ProjectM.Gameplay.Systems;
using ProjectM.Network;
using Unity.Collections;

namespace CrimsonClans.Patches;

[HarmonyPatch(typeof(CastleHeartEventSystem), nameof(CastleHeartEventSystem.OnUpdate))]
internal class CastleHeartEventSystemPatch
{
    public static void Prefix(CastleHeartEventSystem __instance)
    {
        var entities = __instance._CastleHeartInteractEventQuery.ToEntityArray(Allocator.Temp);
        foreach(var entity in entities)
        {
            var heartEvent = entity.Read<CastleHeartInteractEvent>();
            if(heartEvent.EventType == CastleHeartInteractEventType.Claim)
            {
                var fromCharacter = entity.Read<FromCharacter>();

                if(CastleHeartService.TryGetCastleHeartByID(heartEvent.CastleHeart, out var castle))
                {
                    if(castle.Has<UserOwner>())
                    {
                        if(castle.Read<UserOwner>().Owner._Entity.Read<User>() == fromCharacter.User.Read<User>())
                        {
                            continue;
                        }
                    }
                }

                if(!CastleHeartService.CanPlaceHeart(fromCharacter.Character))
                {
                    ServerChatUtils.SendSystemMessageToClient(Core.EntityManager, fromCharacter.User.Read<User>(), "You or your clan have reached the maximum number of castle hearts.");
                    entity.DestroyWithReason();
                }
            }
        }

        entities.Dispose();
    }
}