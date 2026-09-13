using System.Text.Json;
using Erkan.Blazor.Chartjs.Enums;
using Erkan.Blazor.Chartjs.Models.Bar;
using Erkan.Blazor.Chartjs.Models.Common;
using Erkan.Blazor.Chartjs.Tests.Infrastructure;

namespace Erkan.Blazor.Chartjs.Tests;

/// <summary>
/// The bar dataset options added for thickness, percentages, rounding, skipped edges, grouping and
/// axis binding, asserted on the JSON shape Chart.js reads.
/// </summary>
/// <remarks>
/// Four of them are unions in Chart.js — <c>barThickness</c> is <c>number | 'flex'</c>,
/// <c>inflateAmount</c> is <c>number | 'auto'</c>, <c>borderRadius</c> is a number or an object
/// of corners, and <c>borderSkipped</c> is an edge name or a boolean — and the key check cannot
/// see which shape went out. A <c>"false"</c> where Chart.js wants <c>false</c> passes every
/// key check and draws the opposite of what was asked, so the token kind is pinned here.
/// </remarks>
public class BarDatasetOptionTests
{
    private static JsonElement Property(BarDataset dataset, string key)
    {
        using var document = ChartJson.SerializeToDocument(dataset);
        Assert.True(document.RootElement.TryGetProperty(key, out var value),
            $"BarDataset dropped '{key}' entirely.");
        return value.Clone();
    }

    private static BarDataset Read(string json) => JsonSerializer.Deserialize<BarDataset>(json, ChartJson.Web)!;

    // ------------------------------------------------ falsy values reach Chart.js

    [Fact] public void BorderRadius_zero_is_written_as_the_number_zero() => Assert.Equal("""{"borderRadius":0}""", ChartJson.Serialize(new BarDataset { BorderRadius = 0 }));

    [Fact] public void Grouped_false_is_written() => Assert.Equal(JsonValueKind.False, Property(new BarDataset { Grouped = false }, "grouped").ValueKind);

    [Fact] public void SkipNull_false_is_written() => Assert.Equal(JsonValueKind.False, Property(new BarDataset { SkipNull = false }, "skipNull").ValueKind);

    [Fact] public void MinBarLength_zero_is_written() => Assert.Equal(0, Property(new BarDataset { MinBarLength = 0 }, "minBarLength").GetInt32());

    [Fact] public void Base_zero_is_written() => Assert.Equal(0, Property(new BarDataset { Base = 0 }, "base").GetDecimal());

    [Fact] public void InflateAmount_zero_is_written_as_the_number_zero() => Assert.Equal("""{"inflateAmount":0}""", ChartJson.Serialize(new BarDataset { InflateAmount = 0 }));

    [Fact] public void HoverBorderWidth_zero_is_written() => Assert.Equal(0, Property(new BarDataset { HoverBorderWidth = 0 }, "hoverBorderWidth").GetInt32());

    [Fact] public void Clip_zero_is_written() => Assert.Equal(0, Property(new BarDataset { Clip = 0 }, "clip").GetInt32());

    // ------------------------------------------------------------ borderSkipped

    /// <summary>
    /// Chart.js tests <c>borderSkipped</c> for truthiness, so <c>"false"</c> — a non-empty string —
    /// would skip an edge instead of none. Both booleans are pinned by token kind.
    /// </summary>
    [Fact]
    public void BorderSkipped_False_and_True_write_json_booleans()
    {
        Assert.Equal(JsonValueKind.False, Property(new BarDataset { BorderSkipped = BorderSkipped.False }, "borderSkipped").ValueKind);
        Assert.Equal(JsonValueKind.True, Property(new BarDataset { BorderSkipped = BorderSkipped.True }, "borderSkipped").ValueKind);
    }

    [Fact]
    public void BorderSkipped_edges_stay_strings()
    {
        foreach (var (edge, expected) in new[]
                 {
                     (BorderSkipped.Start, "start"), (BorderSkipped.End, "end"), (BorderSkipped.Middle, "middle"),
                     (BorderSkipped.Bottom, "bottom"), (BorderSkipped.Left, "left"), (BorderSkipped.Top, "top"),
                     (BorderSkipped.Right, "right"),
                 })
        {
            var value = Property(new BarDataset { BorderSkipped = edge }, "borderSkipped");
            Assert.Equal(JsonValueKind.String, value.ValueKind);
            Assert.Equal(expected, value.GetString());
        }
    }

    /// <summary>A raw value set on the twin gets the same treatment as the typed property.</summary>
    [Fact]
    public void A_raw_false_on_the_string_twin_is_written_as_a_boolean() =>
        Assert.Equal("""{"borderSkipped":false}""", ChartJson.Serialize(new BarDataset { BorderSkippedString = "false" }));

    // ------------------------------------------------------------- borderRadius

    [Fact]
    public void BorderRadius_with_four_equal_corners_is_written_as_a_number()
    {
        Assert.Equal("""{"borderRadius":8}""", ChartJson.Serialize(new BarDataset { BorderRadius = 8 }));
        Assert.Equal("""{"borderRadius":8}""", ChartJson.Serialize(new BarDataset { BorderRadius = new BorderRadius(8, 8, 8, 8) }));
    }

    /// <summary>Only the corners that are set go out: a missing corner is square to Chart.js.</summary>
    [Fact]
    public void BorderRadius_with_named_corners_is_written_as_an_object_of_those_corners()
    {
        Assert.Equal("""{"borderRadius":{"topLeft":8,"topRight":8}}""",
            ChartJson.Serialize(new BarDataset { BorderRadius = new BorderRadius { TopLeft = 8, TopRight = 8 } }));

        Assert.Equal("""{"borderRadius":{"topLeft":1,"topRight":2,"bottomLeft":3,"bottomRight":4}}""",
            ChartJson.Serialize(new BarDataset { BorderRadius = new BorderRadius(1, 2, 3, 4) }));
    }

    [Fact]
    public void HoverBorderRadius_takes_the_same_shapes()
    {
        Assert.Equal("""{"hoverBorderRadius":0}""", ChartJson.Serialize(new BarDataset { HoverBorderRadius = 0 }));
        Assert.Equal("""{"hoverBorderRadius":{"bottomLeft":6}}""",
            ChartJson.Serialize(new BarDataset { HoverBorderRadius = new BorderRadius { BottomLeft = 6 } }));
    }

    [Fact]
    public void Both_borderRadius_shapes_read_back()
    {
        Assert.Equal(12, Read("""{"borderRadius":12}""").BorderRadius!.BottomRight);

        var corners = Read("""{"borderRadius":{"topLeft":4,"bottomRight":2,"outerStart":9}}""").BorderRadius!;
        Assert.Equal(4, corners.TopLeft);
        Assert.Null(corners.TopRight);
        Assert.Null(corners.BottomLeft);
        Assert.Equal(2, corners.BottomRight);
    }

    // --------------------------------------------------- barThickness, inflateAmount

    [Fact]
    public void BarThickness_writes_a_number_or_flex()
    {
        Assert.Equal("""{"barThickness":12}""", ChartJson.Serialize(new BarDataset { BarThickness = 12 }));
        Assert.Equal("""{"barThickness":"flex"}""", ChartJson.Serialize(new BarDataset { BarThickness = BarThickness.Flex }));
    }

    [Fact]
    public void InflateAmount_writes_a_number_or_auto()
    {
        Assert.Equal("""{"inflateAmount":1.5}""", ChartJson.Serialize(new BarDataset { InflateAmount = 1.5m }));
        Assert.Equal("""{"inflateAmount":"auto"}""", ChartJson.Serialize(new BarDataset { InflateAmount = InflateAmount.Auto }));
    }

    [Fact]
    public void Number_or_keyword_shapes_read_back()
    {
        Assert.Equal(12, Read("""{"barThickness":12}""").BarThickness!.Value.Pixels);
        Assert.True(Read("""{"barThickness":"flex"}""").BarThickness!.Value.IsFlex);
        Assert.Equal(0.5m, Read("""{"inflateAmount":0.5}""").InflateAmount!.Value.Pixels);
        Assert.True(Read("""{"inflateAmount":"auto"}""").InflateAmount!.Value.IsAuto);
        Assert.Equal("false", Read("""{"borderSkipped":false}""").BorderSkippedString);
    }

    // ------------------------------------------------ what the key check walks

    /// <summary>
    /// The key check walks the CLR shape of each type, not the converter. <c>BorderRadius</c>'s
    /// properties are exactly its object form, so they may only produce the four corner paths; the
    /// two structs are leaves and may produce nothing beneath their own key.
    /// </summary>
    [Fact]
    public void The_union_types_add_only_real_paths_to_the_model_graph()
    {
        List<string> Beneath(string key) => ModelGraph.AllKeys
            .Select(k => k.Path)
            .Where(path => path.StartsWith($"data.datasets.{key}.", StringComparison.Ordinal))
            .Order(StringComparer.Ordinal)
            .ToList();

        string[] corners = ["bottomLeft", "bottomRight", "topLeft", "topRight"];
        Assert.Equal(corners.Select(c => $"data.datasets.borderRadius.{c}"), Beneath("borderRadius"));
        Assert.Equal(corners.Select(c => $"data.datasets.hoverBorderRadius.{c}"), Beneath("hoverBorderRadius"));
        Assert.Empty(Beneath("barThickness"));
        Assert.Empty(Beneath("inflateAmount"));

        foreach (var corner in corners)
            Assert.True(ChartJsKeyList.Current.Paths.Contains($"data.datasets.borderRadius.{corner}"));
    }

    // ---------------------------------------------------------------- the rest

    /// <summary>
    /// <c>xAxisID</c> and <c>yAxisID</c>, capital <c>ID</c>. The naming policy would write
    /// <c>xAxisId</c>, which Chart.js ignores and binds the dataset to the first axis instead.
    /// </summary>
    [Fact]
    public void Axis_ids_and_index_axis_land_on_their_own_keys() =>
        Assert.Equal("""{"indexAxis":"y","xAxisID":"revenue","yAxisID":"months"}""",
            ChartJson.Serialize(new BarDataset { IndexAxis = Axes.Y, XAxisId = "revenue", YAxisId = "months" }));

    [Fact]
    public void Sizes_percentages_and_hover_colours_land_on_their_own_keys()
    {
        var dataset = new BarDataset
        {
            BarPercentage = 0.5m,
            CategoryPercentage = 1,
            MaxBarThickness = 40,
            HoverBorderColor = ["#102a43", "#243b53"],
            PointStyle = PointStyle.Triangle,
        };

        Assert.Equal(0.5m, Property(dataset, "barPercentage").GetDecimal());
        Assert.Equal(1, Property(dataset, "categoryPercentage").GetDecimal());
        Assert.Equal(40, Property(dataset, "maxBarThickness").GetInt32());
        Assert.Equal(["#102a43", "#243b53"], Property(dataset, "hoverBorderColor").EnumerateArray().Select(e => e.GetString()));
        Assert.Equal("triangle", Property(dataset, "pointStyle").GetString());
    }

    /// <summary>End to end, through a whole chart configuration.</summary>
    [Fact]
    public void The_rich_bar_configuration_sends_the_falsy_values_as_values()
    {
        var paths = JsonPaths.Map(SampleConfigs.Rich(SampleConfigs.Bar));

        Assert.Equal(JsonValueKind.Number, paths["data.datasets.borderRadius"].ValueKind);
        Assert.Equal(0, paths["data.datasets.borderRadius"].GetInt32());
        Assert.Equal(JsonValueKind.False, paths["data.datasets.grouped"].ValueKind);
        Assert.Equal(JsonValueKind.False, paths["data.datasets.borderSkipped"].ValueKind);
    }

    /// <summary>None of the new options may appear unless set: each would override a Chart.js default.</summary>
    [Fact]
    public void An_untouched_bar_dataset_writes_nothing() =>
        Assert.Equal("{}", ChartJson.Serialize(new BarDataset()));
}
