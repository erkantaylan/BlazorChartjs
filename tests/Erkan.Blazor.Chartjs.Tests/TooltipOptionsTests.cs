using System.Text.Json;
using Erkan.Blazor.Chartjs.Models.Common;
using Erkan.Blazor.Chartjs.Tests.Infrastructure;

namespace Erkan.Blazor.Chartjs.Tests;

/// <summary>
/// The tooltip's behaviour and layout options on <see cref="Tooltip"/>: whether it is drawn, which
/// elements it describes, where it is placed, how its text is aligned and spaced, its padding and
/// caret, and the colour box beside each item.
/// </summary>
/// <remarks>
/// The snapshots record what the rich configs emit, and the key validation proves every key exists
/// somewhere in Chart.js. Neither says which property wrote which key, whether a falsy value
/// survives, or whether an unset one stays out of the config — so those are asserted here, one
/// property at a time.
/// </remarks>
public class TooltipOptionsTests
{
    private static readonly OptionPathSet ChartJsPaths = ChartJsKeyList.Current.Paths;

    /// <summary>Every behaviour and layout option set at once, each to a value that is not its default.</summary>
    private static Tooltip EverythingSet() => new()
    {
        Enabled = false,
        Mode = InteractionMode.Index,
        Intersect = false,
        Axis = AxisInteractions.X,
        IncludeInvisible = true,
        Position = TooltipPosition.Nearest,
        XAlign = TooltipXAlign.Right,
        YAlign = TooltipYAlign.Bottom,
        TitleAlign = TextAlign.Center,
        TitleSpacing = 5,
        TitleMarginBottom = 9,
        BodyAlign = TextAlign.Right,
        BodySpacing = 3,
        FooterAlign = TextAlign.Center,
        FooterSpacing = 7,
        FooterMarginTop = 11,
        Padding = new Padding(1, 2, 3, 4),
        CaretPadding = 8,
        CaretSize = 12,
        CornerRadius = 0,
        DisplayColors = false,
        BoxWidth = 14,
        BoxHeight = 6,
        BoxPadding = 3,
        UsePointStyle = true,
        RTL = true,
        TextDirection = TextDirection.RTL,
    };

    public static TheoryData<string, string> ExpectedKeys => new()
    {
        { "enabled", "false" },
        { "mode", "\"index\"" },
        { "intersect", "false" },
        { "axis", "\"x\"" },
        { "includeInvisible", "true" },
        { "position", "\"nearest\"" },
        { "xAlign", "\"right\"" },
        { "yAlign", "\"bottom\"" },
        { "titleAlign", "\"center\"" },
        { "titleSpacing", "5" },
        { "titleMarginBottom", "9" },
        { "bodyAlign", "\"right\"" },
        { "bodySpacing", "3" },
        { "footerAlign", "\"center\"" },
        { "footerSpacing", "7" },
        { "footerMarginTop", "11" },
        { "padding.top", "1" },
        { "padding.right", "2" },
        { "padding.bottom", "3" },
        { "padding.left", "4" },
        { "caretPadding", "8" },
        { "caretSize", "12" },
        { "cornerRadius", "0" },
        { "displayColors", "false" },
        { "boxWidth", "14" },
        { "boxHeight", "6" },
        { "boxPadding", "3" },
        { "usePointStyle", "true" },
        { "rtl", "true" },
        { "textDirection", "\"rtl\"" },
    };

    [Theory]
    [MemberData(nameof(ExpectedKeys))]
    public void Each_option_writes_its_value_at_the_path_ChartJs_reads(string path, string json)
    {
        var paths = JsonPaths.Map(new Options { Plugins = new Plugins { Tooltip = EverythingSet() } });
        var key = $"plugins.tooltip.{path}";

        Assert.True(paths.TryGetValue(key, out var written), $"options.{key} was not written.");
        Assert.Equal(json, written.GetRawText());
        Assert.True(ChartJsPaths.Contains($"options.{key}"), $"options.{key} is not a Chart.js 4.5.1 option.");
    }

    /// <summary>The theory above covers every new property: one that is added later without a row fails here.</summary>
    [Fact]
    public void Every_key_the_fully_set_tooltip_writes_has_a_row()
    {
        var expected = ExpectedKeys.Select(row => (string)row[0]).ToHashSet(StringComparer.Ordinal);
        var written = JsonPaths.Map(EverythingSet()).Keys
            .Where(k => k != "padding")
            .ToHashSet(StringComparer.Ordinal);

        Assert.Equal(expected.Order(StringComparer.Ordinal), written.Order(StringComparer.Ordinal));
    }

    // ------------------------------------------------------------- falsy

    /// <summary>
    /// <c>Enabled = false</c> is how a tooltip is switched off and <c>DisplayColors = false</c> is how
    /// its colour boxes are hidden; neither may be dropped as a default. Zero is a real caret size,
    /// corner radius and spacing too.
    /// </summary>
    [Fact]
    public void Zero_and_false_values_are_written()
    {
        var tooltip = new Tooltip
        {
            Enabled = false,
            Intersect = false,
            IncludeInvisible = false,
            DisplayColors = false,
            UsePointStyle = false,
            RTL = false,
            TitleSpacing = 0,
            TitleMarginBottom = 0,
            BodySpacing = 0,
            FooterSpacing = 0,
            FooterMarginTop = 0,
            Padding = new Padding(0),
            CaretPadding = 0,
            CaretSize = 0,
            CornerRadius = 0,
            BoxWidth = 0,
            BoxHeight = 0,
            BoxPadding = 0,
        };

        var paths = JsonPaths.Map(tooltip);

        foreach (var key in new[] { "enabled", "intersect", "includeInvisible", "displayColors", "usePointStyle", "rtl" })
            Assert.Equal(JsonValueKind.False, paths[key].ValueKind);

        foreach (var key in new[] { "titleSpacing", "titleMarginBottom", "bodySpacing", "footerSpacing", "footerMarginTop",
                     "padding.top", "padding.right", "padding.bottom", "padding.left",
                     "caretPadding", "caretSize", "cornerRadius", "boxWidth", "boxHeight", "boxPadding" })
            Assert.Equal(0, paths[key].GetDecimal());
    }

    // --------------------------------------------------------------------- unset

    [Fact]
    public void An_untouched_Tooltip_writes_an_empty_object()
    {
        Assert.Equal("{}", ChartJson.Serialize(new Tooltip()));
    }

    /// <summary>
    /// <c>Position = null</c> is how a position is cleared back to Chart.js's <c>"average"</c>: it must
    /// not throw, and must take the <c>position</c> key with it.
    /// </summary>
    [Fact]
    public void Clearing_Position_does_not_throw_and_writes_no_key()
    {
        var tooltip = new Tooltip { Position = TooltipPosition.Average };
        Assert.Equal("average", tooltip.PositionString);

        var exception = Record.Exception(() => tooltip.Position = null);

        Assert.Null(exception);
        Assert.Null(tooltip.Position);
        Assert.Null(tooltip.PositionString);
        Assert.Equal("{}", ChartJson.Serialize(tooltip));
    }

    // ------------------------------------------------------------ string enums

    /// <summary>
    /// A positioner registered in page script under its own name has no enum value; the raw string
    /// carries it to Chart.js untouched.
    /// </summary>
    [Fact]
    public void A_custom_positioner_name_is_written_through_PositionString()
    {
        var tooltip = new Tooltip { PositionString = "cursor" };

        Assert.Null(tooltip.Position);
        Assert.Equal("""{"position":"cursor"}""", ChartJson.Serialize(tooltip));
    }

    [Fact]
    public void The_string_enums_offer_the_values_the_tooltip_reads()
    {
        Assert.Equal(["average", "nearest"], new[] { TooltipPosition.Average.Value, TooltipPosition.Nearest.Value });
        Assert.Equal(["left", "center", "right"], new[] { TooltipXAlign.Left.Value, TooltipXAlign.Center.Value, TooltipXAlign.Right.Value });
        Assert.Equal(["top", "center", "bottom"], new[] { TooltipYAlign.Top.Value, TooltipYAlign.Center.Value, TooltipYAlign.Bottom.Value });
        Assert.Equal(["left", "center", "right"], new[] { TextAlign.Left.Value, TextAlign.Center.Value, TextAlign.Right.Value });
    }

    // ------------------------------------------------------------------ reuse

    /// <summary>
    /// The tooltip reads the same four interaction keys as <c>options.interaction</c>, and falls back
    /// to it for each one it leaves unset, so it takes the same types and writes the same keys.
    /// </summary>
    [Fact]
    public void The_interaction_settings_reuse_Interaction_types_and_write_its_keys()
    {
        Assert.Equal(typeof(InteractionMode), typeof(Tooltip).GetProperty(nameof(Tooltip.Mode))!.PropertyType);
        Assert.Equal(typeof(AxisInteractions), typeof(Tooltip).GetProperty(nameof(Tooltip.Axis))!.PropertyType);

        var tooltip = new Tooltip
        {
            Mode = InteractionMode.Nearest,
            Axis = AxisInteractions.XY,
            Intersect = true,
            IncludeInvisible = false,
        };
        var interaction = new Interaction
        {
            Mode = InteractionMode.Nearest,
            Axis = AxisInteractions.XY,
            Intersect = true,
            IncludeInvisible = false,
        };

        static IEnumerable<string> Pairs(object value) =>
            JsonPaths.Map(value).Select(p => $"{p.Key}={p.Value.GetRawText()}").Order(StringComparer.Ordinal);

        Assert.Equal(["axis=\"xy\"", "includeInvisible=false", "intersect=true", "mode=\"nearest\""], Pairs(tooltip));
        Assert.Equal(Pairs(interaction), Pairs(tooltip));
    }

    [Fact]
    public void Padding_is_the_four_sided_Padding_and_a_single_size_pads_every_side()
    {
        Assert.Equal(typeof(Padding), typeof(Tooltip).GetProperty(nameof(Tooltip.Padding))!.PropertyType);

        var paths = JsonPaths.Map(new Tooltip { Padding = new Padding(10) });
        foreach (var side in new[] { "top", "right", "bottom", "left" })
        {
            Assert.Equal(10, paths[$"padding.{side}"].GetInt32());
            Assert.True(ChartJsPaths.Contains($"options.plugins.tooltip.padding.{side}"));
        }
    }

    [Fact]
    public void TextDirection_is_the_same_type_the_legend_takes()
    {
        Assert.Equal(typeof(Legend).GetProperty(nameof(Legend.TextDirection))!.PropertyType,
            typeof(Tooltip).GetProperty(nameof(Tooltip.TextDirection))!.PropertyType);
    }
}
