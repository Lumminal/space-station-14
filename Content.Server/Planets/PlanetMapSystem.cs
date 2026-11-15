using System.Numerics;
using Content.Server.Atmos.EntitySystems;
using Content.Server.GameTicking;
using Content.Server.Parallax;
using Content.Server.Shuttles.Systems;
using Content.Server.Station.Systems;
using Content.Shared.EntityEffects;
using Content.Shared.Parallax.Biomes;
using Content.Shared.Salvage;

namespace Content.Server.Planets;

/// <summary>
/// System that handles initializing a planet map.
/// The planet is initialized when <see cref="PostGameMapLoad"/> event is raised, so there's enough time to do the setup.
///
/// You should prefer doing planet maps with this, instead of <see cref="StationBiomeComponent"/>, due to the fact that it misses
/// a ton of configuration and setup required.
/// </summary>
public sealed partial class PlanetMapSystem : EntitySystem
{
    [Dependency] private BiomeSystem _biome = default!;
    [Dependency] private SharedMapSystem _map = default!;
    [Dependency] private SharedEntityEffectsSystem _effects = default!;
    [Dependency] private AtmosphereSystem _atmos = default!;

    [Dependency] private EntityQuery<BiomeComponent> _biomeQuery = default!;

    [SubscribeLocalEvent(after: [typeof(ShuttleSystem), typeof(ServerStationSystem)])]
    private void OnPostMapLoad(PostGameMapLoad args)
    {
        if (args.GameMap.Planet is not { } planet
            || !_map.TryGetMap(args.Map, out var mapUid)
            || mapUid is not { } map)
            return;

        if (!ProtoMan.Resolve(planet, out var planetMapPrototype) ||
            !ProtoMan.Resolve(planetMapPrototype.Biome, out var biomePrototype))
            return;

        var mapLight = planetMapPrototype.MapLight;
        var atmos = planetMapPrototype.Atmosphere;
        var range = planetMapPrototype.Range;
        var effects = planetMapPrototype.Effects;

        // Set up the planet with the sane defaults first.
        // Seed is passed as null because it's generated inside this method
        _biome.EnsurePlanet(mapUid: map, biomeTemplate: biomePrototype, seed: null, metadata: null, mapLight: mapLight);

        // Do not let players get past this range, for performance reasons
        var restrictedRange = EnsureComp<RestrictedRangeComponent>(map);
        restrictedRange.Range = range;

        // From here on, set up the biome related stuff
        if (!_biomeQuery.TryComp(map, out var biome))
            return;

        // Say, you want to add ores and stuff.
        foreach (var marker in planetMapPrototype.MarkerLayers)
        {
            _biome.AddMarkerLayer(map, biome, marker);
        }

        _atmos.SetMapAtmosphere(uid: map, space: false, mixture: atmos); // it's a planet, so space is set to false

        // In case you want to add components on the map, or status effects.
        if (effects is { } entityEffects)
            _effects.ApplyEffects(map, entityEffects);

        // This box defines an area where the biome will be preloaded, using a bit more space than the restricted range to be safe.
        var boxRange = range * 1.5f;
        var rangeBox = Box2.CentredAroundZero(new Vector2(boxRange , boxRange));

        // Self-explanatory, preloads the rangeBox to save on the map's performance.
        _biome.Preload(map, biome, rangeBox);

        Log.Info($"Planet map {map} has been successfully loaded with all necessary configuration.");
    }
}
