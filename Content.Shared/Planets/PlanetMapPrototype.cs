using Content.Shared.Atmos;
using Content.Shared.EntityEffects;
using Content.Shared.Parallax.Biomes;
using Content.Shared.Parallax.Biomes.Markers;
using Robust.Shared.Prototypes;

namespace Content.Shared.Planets;

/// <summary>
/// Prototype used to define a set of values to apply on a map entity in order to make it a playable station planet map.
///
/// Also, please, set "randomRotation" to false on the map.
/// </summary>
[Prototype]
public sealed partial class PlanetMapPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set;  } = default!;

    /// <summary>
    /// The color of the map's ambient light
    /// </summary>
    [DataField]
    public Color MapLight = Color.Black;

    /// <summary>
    /// The fixed map's atmosphere.
    /// </summary>
    /// <example>
    /// An example of a breathable atmosphere at normal temperature.
    /// <code>
    /// volume: 2500
    /// temperature: 290
    /// moles:
    /// - 21
    /// - 82
    /// </code>
    /// </example>
    [DataField(required: true)]
    public GasMixture Atmosphere;

    /// <summary>
    /// The biome to generate the surface of the planet with.
    /// Is this some kind of planet... map?
    /// </summary>
    [DataField(required: true)]
    public ProtoId<BiomeTemplatePrototype> Biome;

    /// <summary>
    /// Optionally, extra marker layers to apply on the biome of the map.
    /// This is useful for generating ores.
    /// <example>
    /// Some example layers you can enter.
    /// <code>
    /// markerLayers:
    /// - OreIron
    /// - OreQuartz
    /// - OreSalt
    /// </code>
    /// </example>
    /// </summary>
    [DataField]
    public List<ProtoId<BiomeMarkerLayerPrototype>> MarkerLayers = new();

    /// <summary>
    /// Entity effects to run on the map, before the biome has been preloaded.
    /// <example>
    /// Adding a weather status effect.
    /// </example>
    /// </summary>
    [DataField]
    public EntityEffect[]? Effects;

    /// <summary>
    /// The restricted range in which players are not allowed to go past, but also the range in which the biome preloads.
    /// This is useful for having known bounds, and will help performance so players don't go past the restricted range chunks.
    /// </summary>
    [DataField]
    public float Range = 512f;
}
