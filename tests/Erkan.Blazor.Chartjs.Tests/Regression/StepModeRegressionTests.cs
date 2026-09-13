using System.Text.Json;
using Erkan.Blazor.Chartjs.Models.Common.StringEnums;
using Erkan.Blazor.Chartjs.Models.Line;
using Erkan.Blazor.Chartjs.Tests.Infrastructure;

namespace Erkan.Blazor.Chartjs.Tests.Regression;

/// <summary>
/// Up to 2.0.0: <c>StepMode.False</c> drew a stepped line.
/// </summary>
/// <remarks>
/// Chart.js reads <c>stepped</c> as <c>boolean | 'before' | 'after' | 'middle'</c> and branches
/// on <c>if (options.stepped)</c>. The wrapper wrote every value as a string, so
/// <c>StepMode.False</c> sent <c>"stepped": "false"</c> — a non-empty string, truthy — and
/// <c>_steppedLineTo</c> treated the unrecognised mode as <c>'before'</c>. <c>StepMode.True</c>
/// only worked because <c>"true"</c> happens to be truthy too. The value that should have turned
/// stepping off, and the value that overrides a chart-level <c>elements.line.stepped</c>, did the
/// opposite of what it said.
/// <para>
/// The fix keeps the five values and gives the serialized twin a converter that writes
/// <c>"true"</c> and <c>"false"</c> as JSON booleans. These tests pin the JSON token kind, not
/// just the text, because <c>false</c> and <c>"false"</c> differ only in the token.
/// </para>
/// </remarks>
public class StepModeRegressionTests
{
    private static JsonElement Stepped(LineDataset dataset)
    {
        using var document = ChartJson.SerializeToDocument(dataset);
        Assert.True(document.RootElement.TryGetProperty("stepped", out var value),
            "the dataset dropped 'stepped' entirely.");
        return value.Clone();
    }

    [Fact]
    public void StepMode_False_writes_the_json_boolean_false()
    {
        var stepped = Stepped(new LineDataset { StepMode = StepMode.False });

        Assert.True(stepped.ValueKind == JsonValueKind.False,
            $"stepped was written as {stepped.GetRawText()}. Chart.js tests it for truthiness, "
            + "so anything but the boolean false draws a stepped line.");
    }

    [Fact]
    public void StepMode_True_writes_the_json_boolean_true()
    {
        Assert.Equal(JsonValueKind.True, Stepped(new LineDataset { StepMode = StepMode.True }).ValueKind);
    }

    [Theory]
    [InlineData("before")]
    [InlineData("after")]
    [InlineData("middle")]
    public void The_named_modes_stay_strings(string mode)
    {
        var value = mode switch
        {
            "before" => StepMode.Before,
            "after" => StepMode.After,
            _ => StepMode.Middle,
        };

        var stepped = Stepped(new LineDataset { StepMode = value });

        Assert.Equal(JsonValueKind.String, stepped.ValueKind);
        Assert.Equal(mode, stepped.GetString());
    }

    /// <summary>A raw value set on the twin gets the same treatment as the typed property.</summary>
    [Fact]
    public void A_raw_true_or_false_on_the_string_twin_is_written_as_a_boolean()
    {
        Assert.Equal(JsonValueKind.False, Stepped(new LineDataset { StepModeString = "false" }).ValueKind);
        Assert.Equal(JsonValueKind.True, Stepped(new LineDataset { StepModeString = "true" }).ValueKind);
    }

    /// <summary>
    /// The converter must not turn an unset or cleared value into <c>null</c> or <c>false</c>:
    /// omitted is the only shape that leaves a chart-level <c>stepped</c> in charge.
    /// </summary>
    [Fact]
    public void An_unset_or_cleared_step_mode_writes_no_key()
    {
        var cleared = new LineDataset { StepMode = StepMode.False };
        cleared.StepMode = null;

        Assert.DoesNotContain("\"stepped\"", ChartJson.Serialize(new LineDataset()), StringComparison.Ordinal);
        Assert.DoesNotContain("\"stepped\"", ChartJson.Serialize(cleared), StringComparison.Ordinal);
    }

    /// <summary>What Chart.js sends back reads into the same string the typed property writes.</summary>
    [Fact]
    public void Both_json_shapes_read_back_into_the_string_twin()
    {
        Assert.Equal("false", JsonSerializer.Deserialize<LineDataset>("""{"stepped":false}""", ChartJson.Web)!.StepModeString);
        Assert.Equal("true", JsonSerializer.Deserialize<LineDataset>("""{"stepped":true}""", ChartJson.Web)!.StepModeString);
        Assert.Equal("middle", JsonSerializer.Deserialize<LineDataset>("""{"stepped":"middle"}""", ChartJson.Web)!.StepModeString);
    }

    /// <summary>End to end, through a whole chart configuration.</summary>
    [Fact]
    public void The_rich_line_configuration_sends_a_boolean_false()
    {
        var paths = JsonPaths.Map(SampleConfigs.Rich(SampleConfigs.Line));

        Assert.Equal(JsonValueKind.False, paths["data.datasets.stepped"].ValueKind);
    }
}
