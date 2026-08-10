using System.Text.Json;
using Erkan.Blazor.Chartjs.Models.Bar;
using Erkan.Blazor.Chartjs.Models.Common;
using Erkan.Blazor.Chartjs.Models.Line;
using Erkan.Blazor.Chartjs.Models.Scatter;
using Erkan.Blazor.Chartjs.Tests.Infrastructure;

namespace Erkan.Blazor.Chartjs.Tests.Regression;

/// <summary>
/// 1.0.0 and 2.0.0: properties removed because each serialized to a key Chart.js 4 never read.
/// </summary>
/// <remarks>
/// These cannot be tested by assigning them — they do not compile any more, which is the whole
/// point. They are asserted by reflection instead, so that re-adding one fails the build here
/// rather than shipping a property that does nothing. The Chart.js key check would also catch a
/// re-added property; this file names each one and says what replaced it.
/// </remarks>
public class RemovedPropertyRegressionTests
{
    [Theory]
    // 1.0.0 — serialized as "Text", which Chart.js never read. Replaced by Axis.Title.
    [InlineData(typeof(Axis), "Text", "use Axis.Title")]
    // 1.0.0 — Chart.js 4 moved the axis border out of grid. Replaced by Axis.Border.
    [InlineData(typeof(Grid), "DrawBorder", "use Axis.Border")]
    // 1.0.0 — chartjs-plugin-zoom 2.x has no master switch; the property controlled nothing.
    [InlineData(typeof(Zoom), "Enabled", "turn zoom on per gesture via ZoomOptions.Wheel/Pinch/Drag")]
    // 2.0.0 — wrote time.source; Chart.js reads ticks.source. Replaced by Ticks.Source.
    [InlineData(typeof(AxesTime), "Source", "use Ticks.Source")]
    // 2.0.0 — wrote y2AxisID, which is not a Chart.js option. A dataset names its scale with yAxisID.
    [InlineData(typeof(LineDataset), "Y2AxisId", "use YAxisId = \"y2\"")]
    [InlineData(typeof(ScatterDataset), "Y2AxisId", "use YAxisId = \"y2\"")]
    // 2.0.0 — fillColor and strokeColor are Chart.js 1.x names, unread since 2.0.
    [InlineData(typeof(LineDataset), "FillColor", "use BackgroundColor")]
    [InlineData(typeof(LineDataset), "StrokeColor", "use BorderColor")]
    // 2.0.0 — wrote scales[].color. Chart.js has no colour at the scale level: defaults.scale
    // declares none, describe('scale', {_fallback: false}) blocks inheriting one, and every
    // colour the scale draws with is read from grid, ticks, border or title.
    [InlineData(typeof(Axis), "Color",
        "use Ticks.Color, Grid.Color, Border.Color or AxesTitle.Color")]
    public void Removed_property_is_still_removed(Type owner, string property, string replacement)
    {
        Assert.True(owner.GetProperty(property) is null,
            $"{owner.Name}.{property} is back. It serializes to a key Chart.js 4.5.1 does not read — "
            + $"{replacement}.");
    }

    /// <summary>
    /// <c>OnAnimationComplete</c> wrote <c>onAnimationComplete</c> at the root of the config
    /// object, where Chart.js does not look. Removed from all seven configurations that had it.
    /// </summary>
    [Fact]
    public void OnAnimationComplete_is_gone_from_every_chart_configuration()
    {
        var offenders = ModelGraph.ConfigRoots
            .Where(root => root.GetProperty("OnAnimationComplete") is not null)
            .Select(root => root.Name)
            .Order(StringComparer.Ordinal)
            .ToList();

        Assert.True(offenders.Count == 0,
            $"{string.Join(", ", offenders)} declare OnAnimationComplete again. It wrote "
            + "onAnimationComplete at the config root, which Chart.js does not read.");
    }

    /// <summary>
    /// 1.0.0: grouped-stacked bars rendered misaligned because <c>Stack</c> was a
    /// <c>List&lt;string&gt;</c> and serialized as an array. Chart.js wants a string.
    /// </summary>
    [Fact]
    public void BarDataset_Stack_serializes_as_a_string()
    {
        Assert.Equal(typeof(string), typeof(BarDataset).GetProperty(nameof(BarDataset.Stack))!.PropertyType);

        using var document = ChartJson.SerializeToDocument(new BarDataset { Stack = "One" });
        var stack = document.RootElement.GetProperty("stack");

        Assert.Equal(JsonValueKind.String, stack.ValueKind);
        Assert.Equal("One", stack.GetString());
    }

    [Fact]
    public void BarDataset_Stack_is_omitted_when_unset()
    {
        using var document = ChartJson.SerializeToDocument(new BarDataset());
        Assert.False(document.RootElement.TryGetProperty("stack", out _));
    }

    /// <summary>
    /// 1.0.0: <c>Axis.Border</c> replaced <c>Grid.DrawBorder</c>, which Chart.js 4 stopped
    /// reading. The replacement must match Chart.js 4's <c>border</c> object exactly — a
    /// replacement with the wrong shape would be the same bug in a new place.
    /// </summary>
    [Fact]
    public void Axis_Border_matches_the_ChartJs_4_border_object()
    {
        var border = new Border
        {
            Display = true,
            Width = 2,
            Color = "#333333",
            Dash = [4, 4],
            DashOffset = 1,
            Z = 3,
        };

        using var document = ChartJson.SerializeToDocument(border);
        var written = document.RootElement.EnumerateObject().Select(p => p.Name).Order(StringComparer.Ordinal).ToList();

        Assert.Equal(["color", "dash", "dashOffset", "display", "width", "z"], written);

        // and every one of them is a key Chart.js reads at scales[].border
        var keys = ChartJsKeyList.Current.Paths;
        foreach (var name in written)
            Assert.True(keys.Contains($"options.scales.*.border.{name}"),
                $"scales[].border.{name} is not a Chart.js 4.5.1 option.");
    }

    [Fact]
    public void Grid_no_longer_writes_drawBorder()
    {
        var grid = new Grid { Display = true, Color = "#eee", DrawOnChartArea = true, DrawTicks = true };
        var json = ChartJson.Serialize(grid);

        Assert.DoesNotContain("drawBorder", json, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 1.0.0: <c>AxesTime.IsoWeekday</c> became <c>int?</c> (0 = Sunday … 6 = Saturday). As a
    /// <c>bool?</c> it could not express a weekday at all.
    /// </summary>
    [Fact]
    public void AxesTime_IsoWeekday_is_a_weekday_number()
    {
        Assert.Equal(typeof(int?), typeof(AxesTime).GetProperty(nameof(AxesTime.IsoWeekday))!.PropertyType);

        using var document = ChartJson.SerializeToDocument(new AxesTime { IsoWeekday = 0 });
        Assert.Equal(0, document.RootElement.GetProperty("isoWeekday").GetInt32());
    }

    /// <summary>
    /// The replacement for the removed <c>AxesTime.Source</c> writes to the path Chart.js reads.
    /// </summary>
    [Fact]
    public void Ticks_Source_writes_to_the_path_ChartJs_reads()
    {
        var axis = new Axis { Type = "time", Ticks = new Ticks { Source = "labels" } };
        var paths = JsonPaths.Map(axis);

        Assert.Equal("labels", paths["ticks.source"].GetString());
        Assert.False(paths.ContainsKey("time.source"));

        Assert.True(ChartJsKeyList.Current.Paths.Contains("options.scales.*.ticks.source"));
        Assert.False(ChartJsKeyList.Current.Paths.Contains("options.scales.*.time.source"));
    }

    /// <summary>
    /// The replacement for the removed <c>Y2AxisId</c>: a dataset names its scale with
    /// <c>yAxisID</c>, and the id is just a scale key.
    /// </summary>
    [Fact]
    public void A_dataset_names_its_second_axis_with_yAxisID()
    {
        using var document = ChartJson.SerializeToDocument(new LineDataset { YAxisId = Scales.Y2AxisId });

        Assert.Equal("y2", document.RootElement.GetProperty("yAxisID").GetString());
        Assert.False(document.RootElement.TryGetProperty("y2AxisID", out _));
    }

    /// <summary>
    /// The four replacements the removal of <c>Axis.Color</c> points a caller at must each
    /// actually reach Chart.js — a removal message naming a second dead property would be worse
    /// than the property it replaced.
    /// </summary>
    [Fact]
    public void The_replacements_for_Axis_Color_all_write_colours_ChartJs_reads()
    {
        var axis = new Axis
        {
            Ticks = new Ticks { Color = "#111111" },
            Grid = new Grid { Color = "#222222" },
            Border = new Border { Color = "#333333" },
            Title = new AxesTitle { Color = "#444444" },
        };

        var paths = JsonPaths.Map(axis);
        var keys = ChartJsKeyList.Current.Paths;

        foreach (var (path, expected) in new[]
                 {
                     ("ticks.color", "#111111"),
                     ("grid.color", "#222222"),
                     ("border.color", "#333333"),
                     ("title.color", "#444444"),
                 })
        {
            Assert.Equal(expected, paths[path].GetString());
            Assert.True(keys.Contains($"options.scales.*.{path}"),
                $"scales[].{path} is not a Chart.js 4.5.1 option.");
        }

        // and the axis itself no longer writes a colour of its own
        Assert.False(paths.ContainsKey("color"));
        Assert.False(keys.Contains("options.scales.*.color"));
    }

    /// <summary>
    /// 2.0.0: <c>plugins.title.padding</c> is <c>number | { top, bottom }</c> in Chart.js, and
    /// the title box reads only <c>padding.height</c> and <c>padding.top</c>. The shared
    /// four-sided <see cref="Padding"/> let a caller express a left and right padding the title
    /// discards, so the title takes a <see cref="TitlePadding"/> instead.
    /// </summary>
    [Fact]
    public void Title_padding_cannot_express_a_horizontal_padding()
    {
        Assert.Equal(typeof(TitlePadding), typeof(Title).GetProperty(nameof(Title.Padding))!.PropertyType);

        Assert.Null(typeof(TitlePadding).GetProperty("Left"));
        Assert.Null(typeof(TitlePadding).GetProperty("Right"));

        var paths = JsonPaths.Map(new Title { Padding = new TitlePadding(6) });
        Assert.Equal(6, paths["padding.top"].GetInt32());
        Assert.Equal(6, paths["padding.bottom"].GetInt32());
        Assert.False(paths.ContainsKey("padding.left"));
        Assert.False(paths.ContainsKey("padding.right"));

        var keys = ChartJsKeyList.Current.Paths;
        Assert.True(keys.Contains("options.plugins.title.padding.top"));
        Assert.True(keys.Contains("options.plugins.title.padding.bottom"));
        Assert.False(keys.Contains("options.plugins.title.padding.left"));
        Assert.False(keys.Contains("options.plugins.title.padding.right"));
    }

    /// <summary>
    /// The other half of the same fix: <see cref="Padding"/> stays four-sided, because under
    /// <c>plugins.datalabels.padding</c> Chart.js reads all four. Narrowing it there would have
    /// removed working options.
    /// </summary>
    [Fact]
    public void Datalabels_padding_is_still_four_sided()
    {
        Assert.Equal(typeof(Padding), typeof(DataLabels).GetProperty(nameof(DataLabels.Padding))!.PropertyType);

        var paths = JsonPaths.Map(new DataLabels { Padding = new Padding(2, 4, 2, 4) });
        var keys = ChartJsKeyList.Current.Paths;

        foreach (var side in new[] { "top", "right", "bottom", "left" })
        {
            Assert.True(paths.ContainsKey($"padding.{side}"), $"DataLabels padding lost {side}.");
            Assert.True(keys.Contains($"options.plugins.datalabels.padding.{side}"),
                $"plugins.datalabels.padding.{side} is not a Chart.js 4.5.1 option.");
        }
    }
}
