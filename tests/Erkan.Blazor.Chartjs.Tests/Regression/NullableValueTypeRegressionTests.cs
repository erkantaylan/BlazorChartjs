using System.Text.Json;
using Erkan.Blazor.Chartjs.Models.Common;
using Erkan.Blazor.Chartjs.Models.Line;
using Erkan.Blazor.Chartjs.Models.Radar;
using Erkan.Blazor.Chartjs.Models.Scatter;
using Erkan.Blazor.Chartjs.Tests.Infrastructure;

namespace Erkan.Blazor.Chartjs.Tests.Regression;

/// <summary>
/// 2.0.0: eighteen properties that silently swallowed <c>0</c> and <c>false</c>.
/// </summary>
/// <remarks>
/// Each was a non-nullable value type serialized with
/// <c>JsonIgnoreCondition.WhenWritingDefault</c>, so assigning the type's own default wrote no
/// key at all and Chart.js applied its own default instead — there was no value you could give
/// any of them to mean "zero" or "off". <c>RadarOptionsElementsLine.BorderWidth = 0</c> could
/// not hide a radar outline; <c>Wheel.Speed = 0</c> could not freeze wheel zoom.
/// <para>
/// One test per property, asserting the key is present and carries the falsy value. The
/// structural guard in <see cref="ModelStructureGuardTests"/> makes the whole class of bug
/// unreachable; these eighteen pin the specific properties that shipped broken.
/// </para>
/// </remarks>
public class NullableValueTypeRegressionTests
{
    private static JsonElement Property(object owner, string key)
    {
        using var document = ChartJson.SerializeToDocument(owner);
        Assert.True(document.RootElement.TryGetProperty(key, out var value),
            $"{owner.GetType().Name} dropped '{key}' entirely. A falsy value must still reach Chart.js — "
            + "this is the WhenWritingDefault bug fixed in 2.0.0.");
        return value.Clone();
    }

    private static void AssertWritesZero(object owner, string key) =>
        Assert.Equal(0, Property(owner, key).GetDecimal());

    private static void AssertWritesFalse(object owner, string key) =>
        Assert.Equal(JsonValueKind.False, Property(owner, key).ValueKind);

    // ------------------------------------------------------ DataLabels: six int?

    [Fact] public void DataLabels_BorderRadius_zero_is_written() => AssertWritesZero(new DataLabels { BorderRadius = 0 }, "borderRadius");

    [Fact] public void DataLabels_BorderWidth_zero_is_written() => AssertWritesZero(new DataLabels { BorderWidth = 0 }, "borderWidth");

    [Fact] public void DataLabels_Offset_zero_is_written() => AssertWritesZero(new DataLabels { Offset = 0 }, "offset");

    [Fact] public void DataLabels_Rotation_zero_is_written() => AssertWritesZero(new DataLabels { Rotation = 0 }, "rotation");

    [Fact] public void DataLabels_TextStrokeWidth_zero_is_written() => AssertWritesZero(new DataLabels { TextStrokeWidth = 0 }, "textStrokeWidth");

    [Fact] public void DataLabels_textShadowBlur_zero_is_written() => AssertWritesZero(new DataLabels { textShadowBlur = 0 }, "textShadowBlur");

    // ----------------------------------------------------- DataLabels: two bool?

    [Fact] public void DataLabels_Clamp_false_is_written() => AssertWritesFalse(new DataLabels { Clamp = false }, "clamp");

    [Fact] public void DataLabels_Clip_false_is_written() => AssertWritesFalse(new DataLabels { Clip = false }, "clip");

    // -------------------------------------------------- DataLabels: one decimal?

    [Fact] public void DataLabels_Opacity_zero_is_written() => AssertWritesZero(new DataLabels { Opacity = 0 }, "opacity");

    // ------------------------------------------------------------------ datasets

    [Fact] public void LineDataset_Fill_false_is_written() => AssertWritesFalse(new LineDataset { Fill = false }, "fill");

    [Fact] public void LineDataset_Tension_zero_is_written() => AssertWritesZero(new LineDataset { Tension = 0 }, "tension");

    [Fact] public void ScatterDataset_Tension_zero_is_written() => AssertWritesZero(new ScatterDataset { Tension = 0 }, "tension");

    [Fact] public void ScatterDataset_ShowLine_false_is_written() => AssertWritesFalse(new ScatterDataset { ShowLine = false }, "showLine");

    [Fact] public void RadarDataset_Fill_false_is_written() => AssertWritesFalse(new RadarDataset { Fill = false }, "fill");

    /// <summary>
    /// The headline case: no value could hide a radar outline before, because <c>0</c> was
    /// dropped and Chart.js reapplied its default width of 3.
    /// </summary>
    [Fact]
    public void RadarOptionsElementsLine_BorderWidth_zero_is_written() =>
        AssertWritesZero(new RadarOptionsElementsLine { BorderWidth = 0 }, "borderWidth");

    // ---------------------------------------------------------------------- zoom

    [Fact] public void Zoom_Drag_Threshold_zero_is_written() => AssertWritesZero(new Drag { Threshold = 0 }, "threshold");

    [Fact] public void Zoom_Pan_Threshold_zero_is_written() => AssertWritesZero(new Pan { Threshold = 0 }, "threshold");

    [Fact] public void Zoom_Wheel_Speed_zero_is_written() => AssertWritesZero(new Wheel { Speed = 0 }, "speed");

    // -------------------------------------------------------------------- counts

    /// <summary>
    /// All eighteen are nullable, so an unset one is omitted rather than written as a type
    /// default — which is what kept the 2.0.0 change from altering any existing chart.
    /// </summary>
    [Fact]
    public void Every_one_of_the_eighteen_is_nullable_and_omitted_when_unset()
    {
        (Type Owner, string Property)[] eighteen =
        [
            (typeof(DataLabels), nameof(DataLabels.BorderRadius)),
            (typeof(DataLabels), nameof(DataLabels.BorderWidth)),
            (typeof(DataLabels), nameof(DataLabels.Offset)),
            (typeof(DataLabels), nameof(DataLabels.Rotation)),
            (typeof(DataLabels), nameof(DataLabels.TextStrokeWidth)),
            (typeof(DataLabels), nameof(DataLabels.textShadowBlur)),
            (typeof(DataLabels), nameof(DataLabels.Clamp)),
            (typeof(DataLabels), nameof(DataLabels.Clip)),
            (typeof(DataLabels), nameof(DataLabels.Opacity)),
            (typeof(LineDataset), nameof(LineDataset.Fill)),
            (typeof(LineDataset), nameof(LineDataset.Tension)),
            (typeof(ScatterDataset), nameof(ScatterDataset.Tension)),
            (typeof(ScatterDataset), nameof(ScatterDataset.ShowLine)),
            (typeof(RadarDataset), nameof(RadarDataset.Fill)),
            (typeof(RadarOptionsElementsLine), nameof(RadarOptionsElementsLine.BorderWidth)),
            (typeof(Drag), nameof(Drag.Threshold)),
            (typeof(Pan), nameof(Pan.Threshold)),
            (typeof(Wheel), nameof(Wheel.Speed)),
        ];

        Assert.Equal(18, eighteen.Length);

        foreach (var (owner, name) in eighteen)
        {
            var property = owner.GetProperty(name);
            Assert.NotNull(property);
            Assert.True(Nullable.GetUnderlyingType(property.PropertyType) is not null,
                $"{owner.Name}.{name} is {property.PropertyType.Name}, not a nullable value type. "
                + "It cannot distinguish 'unset' from 'zero', which is the 2.0.0 bug.");
        }
    }

    /// <summary>
    /// Properties that shipped a deliberate non-default initializer keep it, so a chart that
    /// sets none of the eighteen serializes exactly as it did in 1.0.0.
    /// </summary>
    [Fact]
    public void Deliberate_non_default_initializers_are_preserved()
    {
        Assert.Equal(10, new Pan().Threshold);
        Assert.Equal(0.1M, new Wheel().Speed);
        Assert.Equal(3, new RadarOptionsElementsLine().BorderWidth);

        // and an untouched instance of each still writes them
        Assert.Equal(10, Property(new Pan(), "threshold").GetInt32());
        Assert.Equal(0.1M, Property(new Wheel(), "speed").GetDecimal());
        Assert.Equal(3, Property(new RadarOptionsElementsLine(), "borderWidth").GetInt32());
    }
}
