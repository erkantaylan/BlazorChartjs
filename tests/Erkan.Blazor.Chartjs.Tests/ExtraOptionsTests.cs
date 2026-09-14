using System.Reflection;
using System.Text.Json;
using Erkan.Blazor.Chartjs.Interfaces;
using Erkan.Blazor.Chartjs.Models.Bar;
using Erkan.Blazor.Chartjs.Models.Common;
using Erkan.Blazor.Chartjs.Models.Radar;
using Erkan.Blazor.Chartjs.Models.Scatter;
using Erkan.Blazor.Chartjs.Tests.Infrastructure;

namespace Erkan.Blazor.Chartjs.Tests;

/// <summary>
/// The <c>ExtraOptions</c> escape hatch: a <c>[JsonExtensionData]</c> bag that writes Chart.js
/// options the models have no property for, inline beside the typed keys.
/// </summary>
/// <remarks>
/// The key check validates the entries the sample configurations put in a bag, because it reads
/// the emitted JSON — but it cannot say an entry landed in the object its owner writes, rather
/// than one level up or wrapped in an <c>extraOptions</c> key that happens to be skipped. These
/// tests pin that, what an unused bag writes, and the JSON a key clash produces, which
/// <c>docs/extra-options.md</c> describes to users.
/// </remarks>
public class ExtraOptionsTests
{
    /// <summary>The classes that declare the bag. Adding or removing one anywhere fails here first.</summary>
    private static readonly string[] Owners =
    [
        nameof(Axis), nameof(CustomDataset), nameof(Dataset), nameof(Legend), nameof(Options),
        nameof(Plugins), nameof(RadarOptions), nameof(Title), nameof(Tooltip),
    ];

    /// <summary>
    /// One name and one type everywhere, so a user who has met the bag on one class has met it
    /// on all of them.
    /// </summary>
    [Fact]
    public void Every_bag_is_an_ExtraOptions_dictionary_on_the_expected_classes()
    {
        var bags = ModelGraph.LibraryAssembly.GetTypes()
            .SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .Where(ModelGraph.IsExtensionData)
            .ToList();

        Assert.Equal(
            Owners.Order(StringComparer.Ordinal),
            bags.Select(p => p.DeclaringType!.Name).Order(StringComparer.Ordinal));

        foreach (var bag in bags)
        {
            Assert.Equal("ExtraOptions", bag.Name);
            Assert.Equal(typeof(Dictionary<string, object>), bag.PropertyType);
            Assert.True(bag.SetMethod is { IsPublic: true }, $"{bag.DeclaringType!.Name}.ExtraOptions has no public setter");
        }
    }

    public static TheoryData<string> Placements()
    {
        var data = new TheoryData<string>();
        foreach (var owner in Owners) data.Add(owner);
        return data;
    }

    /// <summary>
    /// An entry lands in the very object its owner's typed keys are written to, under its own
    /// key, and nothing in the configuration is called <c>extraOptions</c>.
    /// </summary>
    [Theory]
    [MemberData(nameof(Placements))]
    public void An_entry_is_written_inline_in_the_object_its_owner_writes(string owner)
    {
        var (config, parent, typedKey, extraKey, expected) = Placement(owner);

        using var document = ChartJson.SerializeToDocument(config);
        var target = At(document.RootElement, parent);

        Assert.True(target.TryGetProperty(typedKey, out _),
            $"'{parent}' does not hold the typed key '{typedKey}', so this is not the object {owner} writes.");
        Assert.True(target.TryGetProperty(extraKey, out var value),
            $"{owner}.ExtraOptions[\"{extraKey}\"] is not a direct member of '{parent}'. Serialized: {ChartJson.Serialize(config)}");
        Assert.Equal(expected, value.GetRawText());

        var wrapped = JsonPaths.Enumerate(config)
            .Where(node => node.Path.Split('.').Any(s => s.Equals("extraOptions", StringComparison.OrdinalIgnoreCase)))
            .Select(node => node.Pointer)
            .ToList();
        Assert.True(wrapped.Count == 0, "the bag was written under a key of its own: " + string.Join(", ", wrapped));

        // every placement here uses a key Chart.js really reads, so the documented examples are valid
        Assert.True(ChartJsKeyList.Current.Paths.Contains($"{parent.Replace("[0]", "")}.{extraKey}"),
            $"'{parent}.{extraKey}' is not a Chart.js option path; pick a real key for this example.");
    }

    private static (IChartConfig Config, string Parent, string TypedKey, string ExtraKey, string Expected) Placement(string owner) => owner switch
    {
        nameof(Options) => (
            new BarChartConfig { Options = new Options { ExtraOptions = new() { ["datasets"] = new { bar = new { categoryPercentage = 0.5 } } } } },
            "options", "responsive", "datasets", """{"bar":{"categoryPercentage":0.5}}"""),

        nameof(RadarOptions) => (
            new RadarChartConfig
            {
                Options = new RadarOptions { ExtraOptions = new() { ["plugins"] = new { legend = new { position = "right" } } } },
            },
            "options", "responsive", "plugins", """{"legend":{"position":"right"}}"""),

        nameof(Plugins) => (
            new BarChartConfig
            {
                Options = new Options { Plugins = new Plugins { ExtraOptions = new() { ["filler"] = new { propagate = false } } } },
            },
            "options.plugins", "legend", "filler", """{"propagate":false}"""),

        nameof(Legend) => (
            new BarChartConfig
            {
                Options = new Options { Plugins = new Plugins { Legend = new Legend { ExtraOptions = new() { ["weight"] = 500 } } } },
            },
            "options.plugins.legend", "hasLegendClick", "weight", "500"),

        nameof(Tooltip) => (
            new BarChartConfig
            {
                Options = new Options
                {
                    Plugins = new Plugins { Tooltip = new Tooltip { BorderWidth = 1, ExtraOptions = new() { ["animation"] = new { duration = 0 } } } },
                },
            },
            "options.plugins.tooltip", "borderWidth", "animation", """{"duration":0}"""),

        nameof(Title) => (
            new BarChartConfig
            {
                Options = new Options
                {
                    Plugins = new Plugins
                    {
                        Title = new Title { Display = true, ExtraOptions = new() { ["weight"] = 2000 } },
                    },
                },
            },
            "options.plugins.title", "display", "weight", "2000"),

        nameof(Axis) => (
            new BarChartConfig
            {
                Options = new Options
                {
                    Scales = new() { ["y"] = new Axis { BeginAtZero = true, ExtraOptions = new() { ["grace"] = "5%" } } },
                },
            },
            "options.scales.y", "beginAtZero", "grace", "\"5%\""),

        nameof(Dataset) => (
            new BarChartConfig
            {
                Data = new BarData { Datasets = [new BarDataset { Label = "a", ExtraOptions = new() { ["borderRadius"] = 6 } }] },
            },
            "data.datasets[0]", "label", "borderRadius", "6"),

        nameof(CustomDataset) => (
            new ScatterChartConfig
            {
                Data = new ScatterData { Datasets = [new ScatterDataset { Label = "a", ExtraOptions = new() { ["hidden"] = true } }] },
            },
            "data.datasets[0]", "label", "hidden", "true"),

        _ => throw new ArgumentOutOfRangeException(nameof(owner), owner, "no placement case for this owner"),
    };

    public static TheoryData<string> TypesCarryingTheBag()
    {
        var data = new TheoryData<string>();
        foreach (var type in ModelGraph.LibraryAssembly.GetTypes()
                     .Where(t => t is { IsClass: true, IsAbstract: false, ContainsGenericParameters: false })
                     .Where(t => t.GetProperties().Any(ModelGraph.IsExtensionData))
                     .OrderBy(t => t.FullName, StringComparer.Ordinal))
            data.Add(type.FullName!);
        return data;
    }

    /// <summary>
    /// A bag that was never touched, set to <c>null</c>, or left empty writes nothing — which is
    /// what keeps the Empty and Minimal snapshots byte-identical to the ones before the bag
    /// existed. Run over every concrete type that inherits a bag, not only the declaring ones.
    /// </summary>
    [Theory]
    [MemberData(nameof(TypesCarryingTheBag))]
    public void A_null_or_empty_bag_writes_nothing(string typeName)
    {
        var type = ModelGraph.LibraryAssembly.GetType(typeName)!;
        var bag = type.GetProperties().Single(ModelGraph.IsExtensionData);

        var untouched = Activator.CreateInstance(type)!;
        Assert.Null(bag.GetValue(untouched));

        var nulled = Activator.CreateInstance(type)!;
        bag.SetValue(nulled, null);

        var emptied = Activator.CreateInstance(type)!;
        bag.SetValue(emptied, new Dictionary<string, object?>());

        var expected = ChartJson.Serialize(untouched);
        Assert.Equal(expected, ChartJson.Serialize(nulled));
        Assert.Equal(expected, ChartJson.Serialize(emptied));
    }

    /// <summary>
    /// The decided behaviour on a clash, pinned byte for byte because the docs quote it.
    /// <c>System.Text.Json</c> does not deduplicate: the typed key is written where it always
    /// is, and the bag's copy follows it. <c>JSON.parse</c> keeps the last of two equal keys, so
    /// the bag's value is what Chart.js receives — but that is documented as unsupported, and
    /// the supported way to hand a key to the bag is to leave the typed property null.
    /// </summary>
    [Fact]
    public void A_bag_key_that_repeats_a_typed_key_is_written_twice_typed_first()
    {
        var clash = new Tooltip { BorderWidth = 1, ExtraOptions = new() { ["borderWidth"] = 2 } };
        Assert.Equal("""{"borderWidth":1,"borderWidth":2}""", ChartJson.Serialize(clash));

        var handedOver = new Tooltip { BorderWidth = null, ExtraOptions = new() { ["borderWidth"] = 2 } };
        Assert.Equal("""{"borderWidth":2}""", ChartJson.Serialize(handedOver));
    }

    /// <summary>
    /// A bag key is written exactly as given; the Web naming policy only reaches the members of
    /// an object used as a value, and a nested dictionary keeps its keys. A <c>null</c> value is
    /// written, because Chart.js treats an explicit <c>null</c> differently from an absent key.
    /// </summary>
    [Fact]
    public void Bag_keys_are_written_as_given_and_only_object_members_are_camel_cased()
    {
        var tooltip = new Tooltip
        {
            ExtraOptions = new()
            {
                ["CaretSize"] = 1,
                ["padding"] = new { Top = 2, Bottom = 2 },
                ["animation"] = new Dictionary<string, object?> { ["Duration"] = null },
            },
        };

        Assert.Equal(
            """{"CaretSize":1,"padding":{"top":2,"bottom":2},"animation":{"Duration":null}}""",
            ChartJson.Serialize(tooltip));
    }

    /// <summary>
    /// No sample configuration writes one key twice in the same object. A bag entry and a typed
    /// property for the same key produce exactly that, and nothing else notices: the snapshot
    /// records it, the key check passes it (both copies are valid keys), and <c>JSON.parse</c>
    /// quietly keeps one. Typed properties keep landing for keys the fixtures reach through a
    /// bag, so this is the check that says a bag entry has to move when they do.
    /// </summary>
    [Theory]
    [MemberData(nameof(SampleConfigs.AllKinds), MemberType = typeof(SampleConfigs))]
    public void No_sample_configuration_writes_a_key_twice_in_one_object(string kind)
    {
        foreach (var (shape, config) in new[]
                 {
                     ("empty", SampleConfigs.Empty(kind)),
                     ("minimal", SampleConfigs.Minimal(kind)),
                     ("rich", SampleConfigs.Rich(kind)),
                 })
        {
            using var document = ChartJson.SerializeToDocument(config);
            var duplicates = new List<string>();
            CollectDuplicateKeys(document.RootElement, "$", duplicates);

            Assert.True(duplicates.Count == 0,
                $"{kind}.{shape} writes a key twice — a bag entry now repeats a typed property, so move "
                + "the entry to the property or to a key that is still untyped:" + Environment.NewLine + "  "
                + string.Join($"{Environment.NewLine}  ", duplicates));
        }
    }

    private static void CollectDuplicateKeys(JsonElement element, string pointer, List<string> duplicates)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var seen = new HashSet<string>(StringComparer.Ordinal);
                foreach (var property in element.EnumerateObject())
                {
                    if (!seen.Add(property.Name))
                        duplicates.Add($"{pointer}.{property.Name}");
                    CollectDuplicateKeys(property.Value, $"{pointer}.{property.Name}", duplicates);
                }
                break;

            case JsonValueKind.Array:
                var index = 0;
                foreach (var item in element.EnumerateArray())
                    CollectDuplicateKeys(item, $"{pointer}[{index++}]", duplicates);
                break;
        }
    }

    /// <summary>Walks a dotted path through objects; a <c>[0]</c> suffix steps into an array.</summary>
    private static JsonElement At(JsonElement root, string path)
    {
        var current = root;
        foreach (var segment in path.Split('.'))
        {
            var indexed = segment.EndsWith("[0]", StringComparison.Ordinal);
            var name = indexed ? segment[..^3] : segment;
            Assert.True(current.TryGetProperty(name, out current), $"no '{name}' on the way to '{path}'");
            if (indexed) current = current[0];
        }
        return current;
    }
}
