using System.Text.Json;
using Erkan.Blazor.Chartjs.Models.Bar;
using Erkan.Blazor.Chartjs.Models.Common;
using Erkan.Blazor.Chartjs.Models.Doughnut;
using Erkan.Blazor.Chartjs.Models.Line;
using Erkan.Blazor.Chartjs.Models.Pie;
using Erkan.Blazor.Chartjs.Models.Polar;
using Erkan.Blazor.Chartjs.Models.Radar;
using Erkan.Blazor.Chartjs.Tests.Infrastructure;

namespace Erkan.Blazor.Chartjs.Tests.Regression;

/// <summary>
/// 2.0.0: a default configuration must contain nothing the caller did not ask for.
/// </summary>
/// <remarks>
/// Six properties wrote a bare <c>null</c> — an unconfigured radar shipped <c>"scales": null</c>
/// and every <c>LineDataType</c> point carried <c>"x": null, "y": null</c> — and five dataset
/// colour lists defaulted to an empty list, so an untouched bar, pie, doughnut or polar chart
/// shipped <c>"backgroundColor": []</c> into Chart.js. An empty array is not "no value" to
/// Chart.js; it is an instruction to use no colours.
/// </remarks>
public class DefaultConfigHygieneTests
{
    /// <summary>
    /// Only the two collections a caller genuinely left empty may be empty, and only on a
    /// configuration that has no data at all.
    /// </summary>
    private static readonly HashSet<string> LegitimatelyEmptyOnAnEmptyConfig =
        ["data.labels", "data.datasets"];

    [Theory]
    [MemberData(nameof(SampleConfigs.AllKinds), MemberType = typeof(SampleConfigs))]
    public void An_untouched_configuration_writes_no_bare_null(string kind)
    {
        var offenders = JsonPaths.Enumerate(SampleConfigs.Empty(kind))
            .Where(node => node.Kind == JsonValueKind.Null)
            .Select(node => node.Pointer)
            .ToList();

        Assert.True(offenders.Count == 0,
            $"an untouched {kind} configuration writes {offenders.Count} bare null(s): "
            + string.Join(", ", offenders) + ". An unset property must omit its key.");
    }

    [Theory]
    [MemberData(nameof(SampleConfigs.AllKinds), MemberType = typeof(SampleConfigs))]
    public void A_minimal_configuration_writes_no_bare_null(string kind)
    {
        var offenders = JsonPaths.Enumerate(SampleConfigs.Minimal(kind))
            .Where(node => node.Kind == JsonValueKind.Null)
            .Select(node => node.Pointer)
            .ToList();

        Assert.True(offenders.Count == 0,
            $"a minimal {kind} configuration writes {offenders.Count} bare null(s): "
            + string.Join(", ", offenders));
    }

    [Theory]
    [MemberData(nameof(SampleConfigs.AllKinds), MemberType = typeof(SampleConfigs))]
    public void An_untouched_configuration_writes_no_stray_empty_array(string kind)
    {
        var offenders = JsonPaths.Enumerate(SampleConfigs.Empty(kind))
            .Where(node => node.Kind == JsonValueKind.Array && node.Element.GetArrayLength() == 0)
            .Where(node => !LegitimatelyEmptyOnAnEmptyConfig.Contains(node.Path))
            .Select(node => node.Pointer)
            .ToList();

        Assert.True(offenders.Count == 0,
            $"an untouched {kind} configuration ships {offenders.Count} empty array(s): "
            + string.Join(", ", offenders)
            + ". Chart.js reads [] as 'no colours', not as 'unset'.");
    }

    [Theory]
    [MemberData(nameof(SampleConfigs.AllKinds), MemberType = typeof(SampleConfigs))]
    public void A_minimal_configuration_writes_no_empty_array_at_all(string kind)
    {
        var offenders = JsonPaths.Enumerate(SampleConfigs.Minimal(kind))
            .Where(node => node.Kind == JsonValueKind.Array && node.Element.GetArrayLength() == 0)
            .Select(node => node.Pointer)
            .ToList();

        Assert.True(offenders.Count == 0,
            $"a minimal {kind} configuration ships {offenders.Count} empty array(s): "
            + string.Join(", ", offenders));
    }

    /// <summary>The five colour lists, each named, so a regression says which one came back.</summary>
    [Fact]
    public void The_five_dataset_colour_lists_start_out_absent()
    {
        AssertKeyAbsent(new BarDataset(), "backgroundColor");
        AssertKeyAbsent(new BarDataset(), "borderColor");
        AssertKeyAbsent(new PieDataset(), "backgroundColor");
        AssertKeyAbsent(new DoughnutDataset(), "backgroundColor");
        AssertKeyAbsent(new PolarDataset(), "backgroundColor");

        Assert.Null(new BarDataset().BackgroundColor);
        Assert.Null(new BarDataset().BorderColor);
        Assert.Null(new PieDataset().BackgroundColor);
        Assert.Null(new DoughnutDataset().BackgroundColor);
        Assert.Null(new PolarDataset().BackgroundColor);
    }

    /// <summary>The six properties that wrote a bare null, each named.</summary>
    [Fact]
    public void The_six_bare_null_properties_omit_their_keys_when_unset()
    {
        AssertKeyAbsent(new RadarOptions(), "scales");
        AssertKeyAbsent(new RadarOptionsScales(), "r");
        AssertKeyAbsent(new RadarOptionsScalesRadius(), "min");
        AssertKeyAbsent(new RadarOptionsScalesRadius(), "max");
        AssertKeyAbsent(new LineDataType(), "x");
        AssertKeyAbsent(new LineDataType(), "y");
    }

    /// <summary>
    /// An unconfigured radar shipped <c>"scales": null</c>, which is the shape a consumer would
    /// hit first. Asserted end to end on a whole configuration rather than on the options object.
    /// </summary>
    [Fact]
    public void An_unconfigured_radar_does_not_ship_a_null_scales_object()
    {
        var json = ChartJson.Serialize(new RadarChartConfig { Options = new RadarOptions() });

        Assert.DoesNotContain("\"scales\":null", json, StringComparison.Ordinal);
        Assert.DoesNotContain("\"scales\"", json, StringComparison.Ordinal);
    }

    /// <summary>A <c>LineDataType</c> with no coordinates writes an empty object, not two nulls.</summary>
    [Fact]
    public void A_LineDataType_point_carries_no_null_coordinates()
    {
        Assert.Equal("{}", ChartJson.Serialize(new LineDataType()));
        Assert.Equal("""{"x":"1"}""", ChartJson.Serialize(new LineDataType { X = "1" }));
    }

    private static void AssertKeyAbsent(object value, string key)
    {
        using var document = ChartJson.SerializeToDocument(value);
        Assert.False(document.RootElement.TryGetProperty(key, out _),
            $"{value.GetType().Name} writes '{key}' when it was never set. "
            + $"Serialized: {ChartJson.Serialize(value)}");
    }
}
