using System.Text.Json;
using Erkan.Blazor.Chartjs.Models.Common;
using Erkan.Blazor.Chartjs.Tests.Infrastructure;

namespace Erkan.Blazor.Chartjs.Tests.Regression;

/// <summary>
/// 2.0.0: wrapper-internal markers and dead option keys reaching the live chart.
/// </summary>
/// <remarks>
/// The wrapper serializes <c>hasFilter</c>, <c>hasLabel</c>, <c>hasCustomTitle</c>,
/// <c>hasCallback</c> and <c>hasAsyncCallback</c> so its JavaScript knows whether a .NET
/// callback is registered. Each was deleted only on the branch that handled a registered
/// callback, so a <c>LegendLabels</c> without a <c>Filter</c>, a <c>Tooltip</c> without
/// callbacks and any scale without a tick callback shipped their marker through as a live
/// <c>false</c>. <c>crosshair</c> was blanked with <c>undefined</c> rather than deleted, which
/// leaves the key on a live object, and the group markers were cleared only inside the branch
/// that acted on them.
/// <para>
/// The markers are legitimately in the serialized JSON — that is how they carry information to
/// the interop layer. What matters is that the interop layer removes every one of them
/// unconditionally, so these tests read <c>src/wwwroot/Chart.js</c> rather than asserting the
/// keys are absent from the JSON.
/// </para>
/// </remarks>
public class InternalMarkerLeakTests
{
    private static readonly string Interop = File.ReadAllText(TestPaths.InteropModule);

    /// <summary>
    /// The five markers, each deleted unconditionally — not inside the branch that consumes it.
    /// </summary>
    [Theory]
    [InlineData("hasFilter")]
    [InlineData("hasLabel")]
    [InlineData("hasCustomTitle")]
    [InlineData("hasCallback")]
    [InlineData("hasAsyncCallback")]
    [InlineData("hasOnHoverAsync")]
    [InlineData("hasLegendClick")]
    [InlineData("registerDataLabels")]
    public void Marker_is_deleted_by_the_interop_layer(string marker)
    {
        Assert.Contains($"delete", Interop);
        Assert.True(DeletesKey(marker),
            $"src/wwwroot/Chart.js no longer contains a 'delete ....{marker};' statement. "
            + "The marker would reach Chart.js as a live option.");
    }

    /// <summary>
    /// <c>crosshair</c>, <c>groupXAxis</c> and <c>groupYAxis</c> are option keys the wrapper
    /// invents; Chart.js has no such options. All three must be deleted, not set to
    /// <c>undefined</c> — assigning <c>undefined</c> leaves the key present on a live object.
    /// </summary>
    [Theory]
    [InlineData("crosshair")]
    [InlineData("groupXAxis")]
    [InlineData("groupYAxis")]
    public void Wrapper_invented_option_key_is_deleted_not_blanked(string key)
    {
        Assert.True(DeletesKey(key),
            $"src/wwwroot/Chart.js no longer deletes '{key}'. Chart.js would receive it as an option.");

        Assert.DoesNotContain($".{key} = undefined", Interop, StringComparison.Ordinal);
        Assert.DoesNotContain($".{key}=undefined", Interop, StringComparison.Ordinal);
    }

    /// <summary>
    /// Every marker the models can emit is one the interop layer removes. This is the general
    /// form: a marker added later without a matching delete fails here, and fails the Chart.js
    /// key check too, because the exemption list is derived from these same delete statements.
    /// </summary>
    [Fact]
    public void Every_marker_property_in_the_models_has_a_matching_delete()
    {
        var markers = ModelGraph.AllKeys
            .Where(key => key.Key.StartsWith("has", StringComparison.Ordinal)
                          && key.Key.Length > 3
                          && char.IsUpper(key.Key[3]))
            .ToList();

        Assert.NotEmpty(markers);

        var leaked = markers.Where(m => !DeletesKey(m.Key)).ToList();
        Assert.True(leaked.Count == 0,
            "these marker properties are serialized but never deleted by src/wwwroot/Chart.js, "
            + "so Chart.js receives them as live options:" + Environment.NewLine + "  "
            + string.Join($"{Environment.NewLine}  ", leaked.Select(m => $"{m.Path} (from {m.Owner})")));
    }

    /// <summary>
    /// A <c>LegendLabels</c> without a filter still writes <c>hasFilter: false</c> — the
    /// marker is informational, and the interop layer strips it. Recorded so a change to that
    /// contract is deliberate.
    /// </summary>
    [Fact]
    public void Markers_are_written_as_false_when_no_callback_is_registered()
    {
        using var labels = ChartJson.SerializeToDocument(new LegendLabels());
        Assert.Equal(JsonValueKind.False, labels.RootElement.GetProperty("hasFilter").ValueKind);

        using var callbacks = ChartJson.SerializeToDocument(new Callbacks());
        Assert.Equal(JsonValueKind.False, callbacks.RootElement.GetProperty("hasLabel").ValueKind);
        Assert.Equal(JsonValueKind.False, callbacks.RootElement.GetProperty("hasCustomTitle").ValueKind);

        using var ticks = ChartJson.SerializeToDocument(new Ticks());
        Assert.Equal(JsonValueKind.False, ticks.RootElement.GetProperty("hasCallback").ValueKind);
        Assert.Equal(JsonValueKind.False, ticks.RootElement.GetProperty("hasAsyncCallback").ValueKind);
    }

    private static bool DeletesKey(string key) =>
        System.Text.RegularExpressions.Regex.IsMatch(
            Interop, $@"\bdelete\s+[A-Za-z_$][\w$?.]*\.{System.Text.RegularExpressions.Regex.Escape(key)}\s*;");
}
