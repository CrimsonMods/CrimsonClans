using CrimsonClans.Structs;
using ProjectM;
using ProjectM.CastleBuilding;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;

namespace CrimsonClans.Services;

internal class CastleHeartService
{
    public static EntityQuery CastleHeartQuery;
    public static EntityQuery ClanQuery;

    public CastleHeartService()
    {
        CastleHeartQuery = Core.EntityManager.CreateEntityQuery(new EntityQueryDesc()
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
        if (!Core.EntityManager.HasComponent<Team>(entity))
        {
            return 0;
        }

        var team = entity.Read<Team>();
        int i = 0;

        var heartEntities = CastleHeartQuery.ToEntityArray(Allocator.Temp);
        try
        {
            foreach (var heartEntity in heartEntities)
            {
                if (!Core.EntityManager.HasComponent<Team>(heartEntity))
                    continue;

                var heartTeam = heartEntity.Read<Team>();
                if (team.Value.Equals(heartTeam.Value))
                {
                    i++;
                }
            }
            return i;
        }
        finally
        {
            heartEntities.Dispose();
        }
    }

    public static bool TryGetClanByID(NetworkId clandId, out Entity clan)
    {
        var clanEntities = ClanQuery.ToEntityArray(Allocator.Temp);
        foreach (var clanEntity in clanEntities)
        {
            var networkId = clanEntity.Read<NetworkId>();
            if (networkId.Equals(clandId))
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

    public static bool TryGetCastleHeartByID(NetworkId castleId, out Entity castle)
    {
        var castleEntities = CastleHeartQuery.ToEntityArray(Allocator.Temp);
        foreach (var castleEntity in castleEntities)
        {
            var networkId = castleEntity.Read<NetworkId>();
            if (networkId.Equals(castleId))
            {
                castle = castleEntity;
                castleEntities.Dispose();
                return true;
            }
        }
        castleEntities.Dispose();
        castle = Entity.Null;
        return false;
    }
}