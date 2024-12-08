using CrimsonClans.Services;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using Unity.Collections;

namespace CrimsonClans.Patches;

[HarmonyPatch(typeof(PlaceTileModelSystem), nameof(PlaceTileModelSystem.OnUpdate))]
internal class PlaceTileModelSystemPatch
{
    public static void Prefix(PlaceTileModelSystem __instance)
    {
        var buildEvents = __instance._BuildTileQuery.ToEntityArray(Allocator.Temp);

        foreach (var build in buildEvents)
        {
            var model = build.Read<BuildTileModelEvent>();

            if (model.PrefabGuid == new Stunlock.Core.PrefabGUID(-485210554)) // Heart - (-485210554) Rebuilding? - (-600018251)
            {
                var fromCharacter = build.Read<FromCharacter>();
                var user = fromCharacter.User.Read<User>();

                if (!CastleHeartService.CanPlaceHeart(fromCharacter.Character))
                {
                    ServerChatUtils.SendSystemMessageToClient(Core.EntityManager, user, "You or your clan have reached the maximum number of castle hearts.");
                    build.DestroyWithReason();
                }
            }
        }

        buildEvents.Dispose();
    }
}