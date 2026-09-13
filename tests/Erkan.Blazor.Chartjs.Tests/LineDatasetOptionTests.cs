using System.Text.Json;
using Erkan.Blazor.Chartjs.Models.Common;
using Erkan.Blazor.Chartjs.Models.Line;
using Erkan.Blazor.Chartjs.Tests.Infrastructure;

namespace Erkan.Blazor.Chartjs.Tests;

/// <summary>
/// The line dataset options added for dashes, gaps, point colours and hover styling, asserted
/// on the JSON shape Chart.js reads.
/// </summary>
/// <remarks>
/// The key check proves each key exists at <c>data.datasets</c>, and the <c>line.rich</c>
/// snapshot records every one of them. Neither says what a value looks like on the wire, which
/// is where these can go wrong without failing anything: a dash pattern that is not a number
/// array, a falsy value dropped instead of written, a list that should have stayed a list.
/// </remarks>
public class LineDatasetOptionTests
{
    private static JsonElement Property(LineDataset dataset, string key)
    {
        using var document = ChartJson.SerializeToDocument(dataset);
        Assert.True(document.RootElement.TryGetProperty(key, out var value),
            $"LineDataset dropped '{key}' entirely.");
        return value.Clone();
    }

    [Fact]
    public void BorderDash_writes_a_number_array() =>
        Assert.Equal("""{"borderDash":[5,5]}""", ChartJson.Serialize(new LineDataset { BorderDash = [5, 5] }));

    /// <summary>The canvas takes fractional dash lengths, so the list is <c>decimal</c>, not <c>int</c>.</summary>
    [Fact]
    public void BorderDash_keeps_fractional_lengths() =>
        Assert.Equal("""{"borderDash":[2.5,1.25]}""", ChartJson.Serialize(new LineDataset { BorderDash = [2.5m, 1.25m] }));

    // ------------------------------------------------ falsy values reach Chart.js

    [Fact] public void BorderDashOffset_zero_is_written() => Assert.Equal(0, Property(new LineDataset { BorderDashOffset = 0 }, "borderDashOffset").GetDecimal());

    [Fact] public void Clip_zero_is_written() => Assert.Equal(0, Property(new LineDataset { Clip = 0 }, "clip").GetInt32());

    [Fact] public void PointRotation_zero_is_written() => Assert.Equal(0, Property(new LineDataset { PointRotation = 0 }, "pointRotation").GetInt32());

    [Fact] public void SpanGaps_false_is_written() => Assert.Equal(JsonValueKind.False, Property(new LineDataset { SpanGaps = false }, "spanGaps").ValueKind);

    [Fact] public void SpanGaps_true_is_written() => Assert.Equal(JsonValueKind.True, Property(new LineDataset { SpanGaps = true }, "spanGaps").ValueKind);

    [Fact] public void ShowLine_false_is_written() => Assert.Equal(JsonValueKind.False, Property(new LineDataset { ShowLine = false }, "showLine").ValueKind);

    [Fact] public void DrawActiveElementsOnTop_false_is_written() => Assert.Equal(JsonValueKind.False, Property(new LineDataset { DrawActiveElementsOnTop = false }, "drawActiveElementsOnTop").ValueKind);

    // ---------------------------------------------------------------- the rest

    /// <summary>Point colours are indexable in Chart.js: one entry per point, wrapping around.</summary>
    [Fact]
    public void Point_colours_are_written_as_arrays()
    {
        var dataset = new LineDataset
        {
            PointBackgroundColor = ["#199473", "#e12d39"],
            PointBorderColor = ["#ffffff"],
            PointHoverBackgroundColor = ["#0b1f33"],
            PointHoverBorderColor = ["#000000", "#ffffff"],
        };

        foreach (var key in new[] { "pointBackgroundColor", "pointBorderColor", "pointHoverBackgroundColor", "pointHoverBorderColor" })
            Assert.Equal(JsonValueKind.Array, Property(dataset, key).ValueKind);

        Assert.Equal(["#199473", "#e12d39"],
            Property(dataset, "pointBackgroundColor").EnumerateArray().Select(e => e.GetString()));
    }

    [Fact]
    public void Cap_and_join_styles_write_the_canvas_names()
    {
        foreach (var (style, expected) in new[] { (BorderCapStyle.Butt, "butt"), (BorderCapStyle.Round, "round"), (BorderCapStyle.Square, "square") })
            Assert.Equal(expected, Property(new LineDataset { BorderCapStyle = style }, "borderCapStyle").GetString());

        foreach (var (style, expected) in new[] { (BorderJoinStyle.Bevel, "bevel"), (BorderJoinStyle.Miter, "miter"), (BorderJoinStyle.Round, "round") })
            Assert.Equal(expected, Property(new LineDataset { BorderJoinStyle = style }, "borderJoinStyle").GetString());
    }

    /// <summary>
    /// <c>xAxisID</c>, capital <c>ID</c>. The naming policy would write <c>xAxisId</c>, which
    /// Chart.js ignores and binds the dataset to the first x axis instead.
    /// </summary>
    [Fact]
    public void XAxisId_writes_xAxisID() =>
        Assert.Equal("""{"xAxisID":"x2"}""", ChartJson.Serialize(new LineDataset { XAxisId = "x2" }));

    [Fact]
    public void Hover_and_point_numbers_land_on_their_own_keys()
    {
        var dataset = new LineDataset
        {
            HoverBackgroundColor = "#aaaaaa",
            HoverBorderColor = "#bbbbbb",
            HoverBorderWidth = 3,
            PointBorderWidth = 1,
            PointHitRadius = 6,
            PointHoverRadius = 5,
            PointHoverBorderWidth = 2,
            Stack = "s1",
        };

        Assert.Equal("#aaaaaa", Property(dataset, "hoverBackgroundColor").GetString());
        Assert.Equal("#bbbbbb", Property(dataset, "hoverBorderColor").GetString());
        Assert.Equal(3, Property(dataset, "hoverBorderWidth").GetInt32());
        Assert.Equal(1, Property(dataset, "pointBorderWidth").GetInt32());
        Assert.Equal(6, Property(dataset, "pointHitRadius").GetInt32());
        Assert.Equal(5, Property(dataset, "pointHoverRadius").GetInt32());
        Assert.Equal(2, Property(dataset, "pointHoverBorderWidth").GetInt32());
        Assert.Equal("s1", Property(dataset, "stack").GetString());
    }

    /// <summary>None of the new options may appear unless set: each would override a Chart.js default.</summary>
    [Fact]
    public void An_untouched_line_dataset_writes_nothing() =>
        Assert.Equal("{}", ChartJson.Serialize(new LineDataset()));
}
