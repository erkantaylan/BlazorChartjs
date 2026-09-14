using System.Reflection;
using System.Text.Json;
using Erkan.Blazor.Chartjs.Models.Common;
using Erkan.Blazor.Chartjs.Tests.Infrastructure;

namespace Erkan.Blazor.Chartjs.Tests.Regression;

/// <summary>
/// Up to 2.0.0: a legend position that placed nothing, and two legend keys written into every chart.
/// </summary>
/// <remarks>
/// <para>
/// <b><c>LegendPosition.Bar</c></b> sent <c>"position": "bar"</c>, which is not a layout position
/// at all: Chart.js's layout places boxes on the four sides and in the chart area, and a legend at
/// <c>"bar"</c> is in none of those lists. Meanwhile <c>"top"</c>, Chart.js's own default, had no value.
/// </para>
/// <para>
/// <b><c>Legend.Display</c></b> started out <c>true</c> and <b><c>Legend.Reverse</c></b> was a plain
/// <c>bool</c>, and <c>Plugins.Legend</c> starts out as a new <c>Legend</c>, so every chart built
/// on <c>Options</c> sent <c>display: true</c> and <c>reverse: false</c> whether or not anyone set
/// them. Both happen to be Chart.js's defaults, so nothing looked wrong; but the pair overrode
/// whatever a page set on <c>Chart.defaults.plugins.legend</c>, and a key the caller never
/// assigned is the thing the models are not supposed to write.
/// </para>
/// </remarks>
public class LegendRegressionTests
{
    private static IEnumerable<LegendPosition> OfferedPositions() =>
        typeof(LegendPosition).GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p => p.PropertyType == typeof(LegendPosition))
            .Select(p => (LegendPosition)p.GetValue(null)!);

    // ---------------------------------------------------------------- position

    [Fact]
    public void LegendPosition_Bar_is_gone()
    {
        Assert.True(typeof(LegendPosition).GetProperty("Bar") is null,
            "LegendPosition.Bar is back. \"bar\" is not a position Chart.js's layout places a legend at.");
    }

    [Fact]
    public void LegendPosition_Top_writes_top()
    {
        var paths = JsonPaths.Map(new Legend { Position = LegendPosition.Top });

        Assert.Equal("\"top\"", paths["position"].GetRawText());
    }

    /// <summary>
    /// Exactly the positions Chart.js's layout places a legend at. <c>center</c> is a layout position
    /// too, but only for a box that belongs to an axis, which the legend is not, so it is not offered.
    /// </summary>
    [Fact]
    public void LegendPosition_offers_exactly_the_positions_a_legend_is_placed_at()
    {
        Assert.Equal(
            ["bottom", "chartArea", "left", "right", "top"],
            OfferedPositions().Select(p => p.Value).Order(StringComparer.Ordinal));
    }

    // --------------------------------------------------------- unrequested keys

    [Fact]
    public void An_untouched_Legend_writes_neither_display_nor_reverse()
    {
        Assert.Equal("""{"hasLegendClick":false}""", ChartJson.Serialize(new Legend()));
    }

    /// <summary>
    /// The shape a consumer hits: a chart that sets no legend option at all. A radar chart passes
    /// trivially, because <c>RadarOptions</c> has no <c>Plugins</c> and writes no legend.
    /// </summary>
    [Theory]
    [MemberData(nameof(SampleConfigs.AllKinds), MemberType = typeof(SampleConfigs))]
    public void A_minimal_configuration_writes_no_legend_display_or_reverse(string kind)
    {
        var paths = JsonPaths.Map(SampleConfigs.Minimal(kind));

        Assert.False(paths.ContainsKey("options.plugins.legend.display"), $"a minimal {kind} configuration writes legend.display.");
        Assert.False(paths.ContainsKey("options.plugins.legend.reverse"), $"a minimal {kind} configuration writes legend.reverse.");
    }

    [Fact]
    public void Display_and_Reverse_are_nullable_and_start_out_null()
    {
        Assert.Equal(typeof(bool?), typeof(Legend).GetProperty(nameof(Legend.Display))!.PropertyType);
        Assert.Equal(typeof(bool?), typeof(Legend).GetProperty(nameof(Legend.Reverse))!.PropertyType);

        var legend = new Legend();
        Assert.Null(legend.Display);
        Assert.Null(legend.Reverse);
    }

    /// <summary>Unset writes nothing, but an assigned <c>false</c> still reaches Chart.js.</summary>
    [Fact]
    public void Display_false_and_Reverse_false_are_written()
    {
        using var document = ChartJson.SerializeToDocument(new Legend { Display = false, Reverse = false });

        Assert.Equal(JsonValueKind.False, document.RootElement.GetProperty("display").ValueKind);
        Assert.Equal(JsonValueKind.False, document.RootElement.GetProperty("reverse").ValueKind);
    }

    [Fact]
    public void Display_true_and_Reverse_true_are_written()
    {
        using var document = ChartJson.SerializeToDocument(new Legend { Display = true, Reverse = true });

        Assert.Equal(JsonValueKind.True, document.RootElement.GetProperty("display").ValueKind);
        Assert.Equal(JsonValueKind.True, document.RootElement.GetProperty("reverse").ValueKind);
    }
}
