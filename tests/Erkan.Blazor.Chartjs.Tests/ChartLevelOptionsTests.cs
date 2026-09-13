using System.Text.Json;
using Erkan.Blazor.Chartjs.Models.Common;
using Erkan.Blazor.Chartjs.Tests.Infrastructure;

namespace Erkan.Blazor.Chartjs.Tests;

/// <summary>
/// The chart-level options on <see cref="Options"/>: layout, sizing, the chart's own colours,
/// hover, events, and the built-in colors plugin under <see cref="Plugins.Colors"/>.
/// </summary>
/// <remarks>
/// The snapshots record what the rich configs emit, and the key validation proves every key exists
/// somewhere in Chart.js. Neither says which property wrote which key, whether a falsy value
/// survives, or whether an unset one stays out of the config — so those are asserted here, one
/// property at a time.
/// </remarks>
public class ChartLevelOptionsTests
{
    private static readonly OptionPathSet ChartJsPaths = ChartJsKeyList.Current.Paths;

    /// <summary>Every chart-level option set at once, each to a value that is not its default.</summary>
    private static Options EverythingSet() => new()
    {
        AspectRatio = 1.5,
        ResizeDelay = 200,
        DevicePixelRatio = 2,
        Color = "#102a43",
        BackgroundColor = "#e0e8f9",
        BorderColor = "#334e68",
        Layout = new Layout { AutoPadding = false, Padding = new Padding(1, 2, 3, 4) },
        Hover = new Interaction
        {
            Mode = InteractionMode.Index,
            Axis = AxisInteractions.X,
            Intersect = false,
            IncludeInvisible = true,
        },
        Events = ["mousemove", "click"],
        MaintainAspectRatio = true,
        Responsive = false,
        Plugins = new Plugins { Colors = new Colors { Enabled = false, ForceOverride = true } },
    };

    public static TheoryData<string, string> ExpectedKeys => new()
    {
        { "aspectRatio", "1.5" },
        { "resizeDelay", "200" },
        { "devicePixelRatio", "2" },
        { "color", "\"#102a43\"" },
        { "backgroundColor", "\"#e0e8f9\"" },
        { "borderColor", "\"#334e68\"" },
        { "layout.autoPadding", "false" },
        { "layout.padding.top", "1" },
        { "layout.padding.right", "2" },
        { "layout.padding.bottom", "3" },
        { "layout.padding.left", "4" },
        { "hover.mode", "\"index\"" },
        { "hover.axis", "\"x\"" },
        { "hover.intersect", "false" },
        { "hover.includeInvisible", "true" },
        { "events", """["mousemove","click"]""" },
        { "maintainAspectRatio", "true" },
        { "responsive", "false" },
        { "plugins.colors.enabled", "false" },
        { "plugins.colors.forceOverride", "true" },
    };

    [Theory]
    [MemberData(nameof(ExpectedKeys))]
    public void Each_option_writes_its_value_at_the_path_ChartJs_reads(string path, string json)
    {
        var paths = JsonPaths.Map(EverythingSet());

        Assert.True(paths.TryGetValue(path, out var written), $"options.{path} was not written.");
        Assert.Equal(json, written.GetRawText());
        Assert.True(ChartJsPaths.Contains($"options.{path}"), $"options.{path} is not a Chart.js 4.5.1 option.");
    }

    // ------------------------------------------------------------- falsy and empty

    /// <summary>
    /// Every value that means "zero", "off" or "none" reaches Chart.js. <c>Events = []</c> in
    /// particular is an instruction — listen to nothing — not an absence.
    /// </summary>
    [Fact]
    public void Zero_false_and_empty_values_are_written()
    {
        var options = new Options
        {
            AspectRatio = 0,
            ResizeDelay = 0,
            DevicePixelRatio = 0,
            Color = "",
            BackgroundColor = "",
            BorderColor = "",
            Layout = new Layout { AutoPadding = false, Padding = new Padding(0) },
            Hover = new Interaction { Intersect = false, IncludeInvisible = false },
            Events = [],
            MaintainAspectRatio = false,
            Responsive = false,
            Plugins = new Plugins { Colors = new Colors { Enabled = false, ForceOverride = false } },
        };

        var paths = JsonPaths.Map(options);

        foreach (var key in new[] { "aspectRatio", "resizeDelay", "devicePixelRatio",
                     "layout.padding.top", "layout.padding.right", "layout.padding.bottom", "layout.padding.left" })
            Assert.Equal(0, paths[key].GetDecimal());

        foreach (var key in new[] { "layout.autoPadding", "hover.intersect", "hover.includeInvisible",
                     "maintainAspectRatio", "responsive", "plugins.colors.enabled", "plugins.colors.forceOverride" })
            Assert.Equal(JsonValueKind.False, paths[key].ValueKind);

        foreach (var key in new[] { "color", "backgroundColor", "borderColor" })
            Assert.Equal("", paths[key].GetString());

        Assert.Equal(JsonValueKind.Array, paths["events"].ValueKind);
        Assert.Equal(0, paths["events"].GetArrayLength());
    }

    // --------------------------------------------------------------------- unset

    [Fact]
    public void An_untouched_Options_writes_none_of_the_chart_level_keys()
    {
        using var document = ChartJson.SerializeToDocument(new Options());
        var written = document.RootElement.EnumerateObject().Select(p => p.Name).ToHashSet(StringComparer.Ordinal);

        foreach (var key in new[] { "aspectRatio", "resizeDelay", "devicePixelRatio", "color", "backgroundColor",
                     "borderColor", "layout", "hover", "events" })
            Assert.DoesNotContain(key, written);

        Assert.False(document.RootElement.GetProperty("plugins").TryGetProperty("colors", out _),
            "an untouched Plugins writes a colors object nobody asked for.");
    }

    [Fact]
    public void Unset_members_of_Layout_and_Colors_are_omitted()
    {
        Assert.Equal("{}", ChartJson.Serialize(new Layout()));
        Assert.Equal("{}", ChartJson.Serialize(new Colors()));
        Assert.Equal("""{"padding":{"top":24}}""", ChartJson.Serialize(new Layout { Padding = new Padding { Top = 24 } }));
    }

    // ------------------------------------------------ maintainAspectRatio and responsive

    /// <summary>
    /// Both became <c>bool?</c> but kept the values they always wrote, so an existing chart sends
    /// exactly what it sent before: the <c>&lt;Chart&gt;</c> component's <c>Height</c> parameter
    /// only works because <c>maintainAspectRatio</c> is <c>false</c>.
    /// </summary>
    [Fact]
    public void MaintainAspectRatio_and_Responsive_keep_the_values_they_always_wrote()
    {
        Assert.Equal(typeof(bool?), typeof(Options).GetProperty(nameof(Options.MaintainAspectRatio))!.PropertyType);
        Assert.Equal(typeof(bool?), typeof(Options).GetProperty(nameof(Options.Responsive))!.PropertyType);

        var options = new Options();
        Assert.False(options.MaintainAspectRatio);
        Assert.True(options.Responsive);

        var paths = JsonPaths.Map(options);
        Assert.Equal(JsonValueKind.False, paths["maintainAspectRatio"].ValueKind);
        Assert.Equal(JsonValueKind.True, paths["responsive"].ValueKind);
    }

    /// <summary><c>null</c> hands the decision back to Chart.js: no key at all.</summary>
    [Fact]
    public void Null_MaintainAspectRatio_and_Responsive_write_nothing()
    {
        var paths = JsonPaths.Map(new Options { MaintainAspectRatio = null, Responsive = null });

        Assert.False(paths.ContainsKey("maintainAspectRatio"));
        Assert.False(paths.ContainsKey("responsive"));
    }

    // ------------------------------------------------------------------ reuse

    /// <summary>
    /// <c>options.hover</c> is the same four-key object as <c>options.interaction</c>, so it is the
    /// same type, and writes the same keys.
    /// </summary>
    [Fact]
    public void Hover_is_an_Interaction_and_writes_exactly_the_interaction_keys()
    {
        Assert.Equal(typeof(Interaction), typeof(Options).GetProperty(nameof(Options.Hover))!.PropertyType);

        var interaction = new Interaction
        {
            Mode = InteractionMode.Nearest,
            Axis = AxisInteractions.XY,
            Intersect = true,
            IncludeInvisible = false,
        };
        var paths = JsonPaths.Map(new Options { Hover = interaction, Interaction = interaction });

        var hover = paths.Keys.Where(k => k.StartsWith("hover.", StringComparison.Ordinal)).Select(k => k["hover.".Length..]).Order(StringComparer.Ordinal);
        var interactionKeys = paths.Keys.Where(k => k.StartsWith("interaction.", StringComparison.Ordinal)).Select(k => k["interaction.".Length..]).Order(StringComparer.Ordinal);

        Assert.Equal(["axis", "includeInvisible", "intersect", "mode"], hover);
        Assert.Equal(interactionKeys, hover);
    }

    [Fact]
    public void Layout_padding_is_the_four_sided_Padding()
    {
        Assert.Equal(typeof(Padding), typeof(Layout).GetProperty(nameof(Layout.Padding))!.PropertyType);

        var paths = JsonPaths.Map(new Layout { Padding = new Padding(10) });
        foreach (var side in new[] { "top", "right", "bottom", "left" })
        {
            Assert.Equal(10, paths[$"padding.{side}"].GetInt32());
            Assert.True(ChartJsPaths.Contains($"options.layout.padding.{side}"));
        }
    }

    // ------------------------------------------------------------- not added

    /// <summary>
    /// Chart.js declares a per-chart <c>options.font</c>, and the key list therefore accepts it, but
    /// 4.5.1 reads it only for radial-scale point labels. Legend, title, ticks, axis titles, tooltip
    /// and datalabels fall back to the page-wide <c>Chart.defaults.font</c> instead. No
    /// <see cref="Options"/>-based chart can show a point label today, so the property would change
    /// nothing on screen. See issue #4.
    /// </summary>
    [Fact]
    public void Options_has_no_chart_wide_Font()
    {
        Assert.True(typeof(Options).GetProperty("Font") is null,
            "Options.Font is back. Chart.js 4.5.1 reads per-chart options.font only for radial point labels, "
            + "so on every chart Options can build it does nothing. If it now does something on purpose — the "
            + "wrapper cascades it, or radar uses Options and shows point labels — delete this test and say so "
            + "in CHANGELOG.md.");
    }
}
