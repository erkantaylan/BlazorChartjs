using System.Text.Json;
using Erkan.Blazor.Chartjs.Tests.Infrastructure;

namespace Erkan.Blazor.Chartjs.Tests;

/// <summary>
/// Golden-JSON snapshots of what each chart type sends to Chart.js.
/// </summary>
/// <remarks>
/// <para>
/// Sixteen files under <c>Snapshots/</c>: a minimal and a richly-configured configuration for
/// each of the eight chart types. Their value is entirely in review — a pull request that
/// changes a model changes these files, and the diff is the exact change in what the browser
/// receives, in a form a human can read without running anything.
/// </para>
/// <para>
/// The minimal snapshots are the ones that catch leakage. Nothing in them asks for an empty
/// array, a bare <c>null</c> or a wrapper-internal marker, so any of those appearing in the
/// file is something the models emitted on their own — which is precisely how the five
/// <c>[]</c> colour lists, the six bare <c>null</c>s and the five <c>has*</c> markers fixed in
/// 2.0.0 would have surfaced.
/// </para>
/// <para>Re-record with <c>UPDATE_SNAPSHOTS=1 dotnet test</c>, then read the diff.</para>
/// </remarks>
public class SnapshotTests
{
    /// <summary>
    /// <c>new XChartConfig()</c> and nothing else. Whatever appears here, the models emit
    /// entirely on their own — so this file is the shortest possible statement of what the
    /// wrapper adds to a chart the caller has not configured.
    /// </summary>
    [Theory]
    [MemberData(nameof(SampleConfigs.AllKinds), MemberType = typeof(SampleConfigs))]
    public void Untouched_configuration_matches_snapshot(string kind) =>
        Snapshot.MatchesJson($"{kind}.empty", SampleConfigs.Empty(kind));

    [Theory]
    [MemberData(nameof(SampleConfigs.AllKinds), MemberType = typeof(SampleConfigs))]
    public void Minimal_configuration_matches_snapshot(string kind) =>
        Snapshot.MatchesJson($"{kind}.minimal", SampleConfigs.Minimal(kind));

    [Theory]
    [MemberData(nameof(SampleConfigs.AllKinds), MemberType = typeof(SampleConfigs))]
    public void Rich_configuration_matches_snapshot(string kind) =>
        Snapshot.MatchesJson($"{kind}.rich", SampleConfigs.Rich(kind));

    /// <summary>
    /// The snapshots are written indented so they can be read; production JSON is compact.
    /// This is the assertion that lets the first stand in for the second.
    /// </summary>
    [Theory]
    [MemberData(nameof(SampleConfigs.AllKinds), MemberType = typeof(SampleConfigs))]
    public void Indenting_changes_only_whitespace(string kind)
    {
        foreach (var config in new[] { SampleConfigs.Minimal(kind), SampleConfigs.Rich(kind) })
        {
            var compact = ChartJson.Serialize(config);
            using var reparsed = JsonDocument.Parse(ChartJson.SerializeIndented(config));
            Assert.Equal(compact, JsonSerializer.Serialize(reparsed.RootElement, ChartJson.Web));
        }
    }

    /// <summary>
    /// Serialization must not depend on anything that varies between runs, or the snapshots
    /// would be noise. <c>CanvasId</c> is a fresh <c>Guid</c> per configuration and is the one
    /// obvious candidate — it is <c>[JsonIgnore]</c>, and this proves it.
    /// </summary>
    [Theory]
    [MemberData(nameof(SampleConfigs.AllKinds), MemberType = typeof(SampleConfigs))]
    public void Serialization_is_deterministic_across_instances(string kind)
    {
        Assert.Equal(
            ChartJson.Serialize(SampleConfigs.Rich(kind)),
            ChartJson.Serialize(SampleConfigs.Rich(kind)));

        Assert.Equal(
            ChartJson.Serialize(SampleConfigs.Minimal(kind)),
            ChartJson.Serialize(SampleConfigs.Minimal(kind)));
    }

    /// <summary>
    /// Every snapshot on disk belongs to a live test. A renamed chart type or a deleted fixture
    /// would otherwise leave a stale file that still looks like coverage.
    /// </summary>
    [Fact]
    public void No_snapshot_file_is_orphaned()
    {
        var expected = SampleConfigs.Kinds
            .SelectMany(kind => new[] { $"{kind}.empty.json", $"{kind}.minimal.json", $"{kind}.rich.json" })
            .ToHashSet(StringComparer.Ordinal);

        var actual = Directory.GetFiles(TestPaths.SnapshotDirectory, "*.json")
            .Select(Path.GetFileName)
            .ToHashSet(StringComparer.Ordinal)!;

        Assert.Equal(expected.Order(StringComparer.Ordinal), actual.Order(StringComparer.Ordinal)!);
    }
}
