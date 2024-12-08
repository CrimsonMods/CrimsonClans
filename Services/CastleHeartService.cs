using System.Collections.Generic;
using CrimsonClans.Structs;
using ProjectM;
using ProjectM.CastleBuilding;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;

namespace CrimsonClans.Services;

internal class CastleHeartService
{
    public static EntityQuery CaslteHeartQuery;
    public static EntityQuery ClanQuery;

    public CastleHeartService()
    {
        CaslteHeartQuery = Core.EntityManager.CreateEntityQuery(new EntityQueryDesc()
        {
            All = new ComponentType[] {
                ComponentType.ReadOnly<CastleHeart>(),
                ComponentType.ReadOnly<Team>(),
            },
        });

        ClanQuery = Core.EntityManager.CreateEntityQuery(new EntityQueryDesc()
        {
            All = new ComponentType[] {
                ComponentType.ReadOnly<ClanTeam>()
            },
        });
    }

    public static bool CanPlaceHeart(Entity character)
    {
        return CountTeamHearts(character) < Settings.HeartsPerClan.Value;
    }

    public static bool CanJoinClan(Entity character, Entity clan)
    {
        int hearts = CountTeamHearts(character);
        hearts += CountTeamHearts(clan);

        return hearts < Settings.HeartsPerClan.Value;
    }

    private static int CountTeamHearts(Entity entity)
    {
        var team = entity.Read<Team>();
        int i = 0;

        var heartEntities = CaslteHeartQuery.ToEntityArray(Allocator.Temp);
        foreach (var heartEntity in heartEntities)
        {
            var heartTeam = heartEntity.Read<Team>();
            if (team.Value.Equals(heartTeam.Value))
            {
                i++;
            }
        }

        heartEntities.Dispose();
        return i;
    }

    public static bool TryGetClanByID(NetworkId clandId, out Entity clan)
    {
        var clanEntities = CaslteHeartQuery.ToEntityArray(Allocator.Temp);
        foreach(var clanEntity in clanEntities)
        {
            var networkId = clanEntity.Read<NetworkId>();
            if(networkId.Equals(clandId))
            {
                clan = clanEntity;
                clanEntities.Dispose();
                return true;
            }
        }
        clanEntities.Dispose();
        clan = Entity.Null;
        return false;
    }
}