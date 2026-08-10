using System.Text.Json;
using Erkan.Blazor.Chartjs.Models.Common;
using Erkan.Blazor.Chartjs.Tests.Infrastructure;

namespace Erkan.Blazor.Chartjs.Tests.Regression;

/// <summary>
/// 1.0.0: <c>Zoom.Mode</c> and <c>Zoom.OverScaleMode</c> serialized one level too high.
/// </summary>
/// <remarks>
/// chartjs-plugin-zoom reads the zoom direction from <c>plugins.zoom.zoom.mode</c>. The wrapper
/// wrote <c>plugins.zoom.mode</c>, so <c>Mode = "x"</c> silently did nothing. This is the
/// nesting half of the key-validation problem in its purest form: the name was right, the
/// parent was wrong, and nothing anywhere complained.
/// <para>
/// The fix pushes the flat value into the nested object from all three setters, so an object
/// initializer gets the same result whichever order it assigns in — which is exactly what these
/// tests pin, because an order-dependent fix is a bug waiting for a reformat.
/// </para>
/// </remarks>
public class ZoomModeRegressionTests
{
    [Fact]
    public void Mode_lands_under_plugins_zoom_zoom_when_set_alone()
    {
        var zoom = new Zoom { Mode = "x" };

        AssertNestedMode(zoom, "x");
        AssertNoFlatMode(zoom);
    }

    [Fact]
    public void Mode_lands_under_plugins_zoom_zoom_when_assigned_before_the_zoom_options()
    {
        var zoom = new Zoom
        {
            Mode = "x",
            ZoomOptions = new ZoomOptions { Wheel = new Wheel { Enabled = true } },
        };

        AssertNestedMode(zoom, "x");
        AssertNoFlatMode(zoom);
    }

    [Fact]
    public void Mode_lands_under_plugins_zoom_zoom_when_assigned_after_the_zoom_options()
    {
        var zoom = new Zoom
        {
            ZoomOptions = new ZoomOptions { Wheel = new Wheel { Enabled = true } },
            Mode = "x",
        };

        AssertNestedMode(zoom, "x");
        AssertNoFlatMode(zoom);
    }

    /// <summary>The two orders must be byte-identical, not merely both correct.</summary>
    [Fact]
    public void Both_initializer_orders_serialize_identically()
    {
        var modeFirst = new Zoom
        {
            Mode = "xy",
            OverScaleMode = "y",
            ZoomOptions = new ZoomOptions { ScaleMode = "x" },
        };
        var optionsFirst = new Zoom
        {
            ZoomOptions = new ZoomOptions { ScaleMode = "x" },
            Mode = "xy",
            OverScaleMode = "y",
        };

        Assert.Equal(ChartJson.Serialize(optionsFirst), ChartJson.Serialize(modeFirst));
    }

    [Fact]
    public void OverScaleMode_lands_under_plugins_zoom_zoom_in_both_orders()
    {
        foreach (var zoom in new[]
                 {
                     new Zoom { OverScaleMode = "y", ZoomOptions = new ZoomOptions() },
                     new Zoom { ZoomOptions = new ZoomOptions(), OverScaleMode = "y" },
                 })
        {
            using var document = ChartJson.SerializeToDocument(zoom);
            var root = document.RootElement;

            Assert.False(root.TryGetProperty("overScaleMode", out _),
                "overScaleMode must not sit next to the plugin options; the plugin reads it from zoom.");
            Assert.Equal("y", root.GetProperty("zoom").GetProperty("overScaleMode").GetString());
        }
    }

    /// <summary>A value set on the nested object directly wins, in either order.</summary>
    [Fact]
    public void An_explicit_nested_mode_is_never_overwritten()
    {
        var modeFirst = new Zoom { Mode = "x", ZoomOptions = new ZoomOptions { Mode = "y" } };
        var optionsFirst = new Zoom { ZoomOptions = new ZoomOptions { Mode = "y" }, Mode = "x" };

        AssertNestedMode(modeFirst, "y");
        AssertNestedMode(optionsFirst, "y");
        Assert.Equal(ChartJson.Serialize(optionsFirst), ChartJson.Serialize(modeFirst));
    }

    /// <summary>
    /// Setting neither must not materialize an empty <c>"zoom": {}</c> — an option object that
    /// exists but says nothing is still a change to the config Chart.js receives.
    /// </summary>
    [Fact]
    public void An_untouched_zoom_writes_no_nested_zoom_object()
    {
        using var document = ChartJson.SerializeToDocument(new Zoom());

        Assert.False(document.RootElement.TryGetProperty("zoom", out _));
        Assert.Equal("{}", ChartJson.Serialize(new Zoom()));
    }

    /// <summary>The getter reports what will actually be serialized, not the value last assigned.</summary>
    [Fact]
    public void The_getter_reports_the_value_that_will_be_serialized()
    {
        var zoom = new Zoom { Mode = "x", ZoomOptions = new ZoomOptions { Mode = "y" } };
        Assert.Equal("y", zoom.Mode);
    }

    /// <summary>End to end, through a whole chart configuration.</summary>
    [Fact]
    public void Mode_reaches_plugins_zoom_zoom_mode_in_a_full_configuration()
    {
        var config = SampleConfigs.Rich(SampleConfigs.Line);
        var paths = JsonPaths.Map(config);

        Assert.True(paths.ContainsKey("options.plugins.zoom.zoom.mode"));
        Assert.False(paths.ContainsKey("options.plugins.zoom.mode"));
        Assert.Equal("xy", paths["options.plugins.zoom.zoom.mode"].GetString());
    }

    private static void AssertNestedMode(Zoom zoom, string expected)
    {
        using var document = ChartJson.SerializeToDocument(zoom);
        Assert.Equal(expected, document.RootElement.GetProperty("zoom").GetProperty("mode").GetString());
    }

    private static void AssertNoFlatMode(Zoom zoom)
    {
        using var document = ChartJson.SerializeToDocument(zoom);
        Assert.False(document.RootElement.TryGetProperty("mode", out _),
            "plugins.zoom.mode is not an option chartjs-plugin-zoom reads; the mode belongs in plugins.zoom.zoom.");
    }
}
