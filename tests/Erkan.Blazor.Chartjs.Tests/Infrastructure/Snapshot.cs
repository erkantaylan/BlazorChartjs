using System.Text;

namespace Erkan.Blazor.Chartjs.Tests.Infrastructure;

/// <summary>
/// Golden-file comparison against <c>tests/Erkan.Blazor.Chartjs.Tests/Snapshots</c>.
/// </summary>
/// <remarks>
/// A snapshot's only value is that a reviewer can see what changed in a pull request, so the
/// files are indented JSON in the source tree, not blobs under bin/. Rewrite them all with:
/// <code>UPDATE_SNAPSHOTS=1 dotnet test</code>
/// and read the resulting diff. Every changed line is a change in what Chart.js receives.
/// </remarks>
public static class Snapshot
{
    private static bool UpdateRequested =>
        Environment.GetEnvironmentVariable("UPDATE_SNAPSHOTS") is "1" or "true";

    /// <summary>Serializes <paramref name="config"/> and asserts it matches <paramref name="name"/>.json.</summary>
    public static void MatchesJson(string name, object config) =>
        Matches(name, ChartJson.SerializeIndented(config));

    public static void Matches(string name, string actual)
    {
        Directory.CreateDirectory(TestPaths.SnapshotDirectory);
        var file = Path.Combine(TestPaths.SnapshotDirectory, $"{name}.json");
        var normalized = actual.ReplaceLineEndings("\n").TrimEnd() + "\n";

        if (UpdateRequested)
        {
            File.WriteAllText(file, normalized, new UTF8Encoding(false));
            return;
        }

        if (!File.Exists(file))
        {
            throw new Xunit.Sdk.XunitException(
                $"""
                 Snapshot '{name}' does not exist yet at
                   {file}

                 Create it, then read it before committing — an unreviewed snapshot asserts nothing:
                   UPDATE_SNAPSHOTS=1 dotnet test

                 It would have contained:
                 {normalized}
                 """);
        }

        var expected = File.ReadAllText(file).ReplaceLineEndings("\n").TrimEnd() + "\n";
        if (string.Equals(expected, normalized, StringComparison.Ordinal))
            return;

        throw new Xunit.Sdk.XunitException(
            $"""
             Snapshot '{name}' does not match the serialized configuration.

             Every line below is a difference in the JSON Chart.js receives. If the change is
             intended, re-record and review the diff:
               UPDATE_SNAPSHOTS=1 dotnet test

             {Diff(expected, normalized)}
             """);
    }

    private static string Diff(string expected, string actual)
    {
        var expectedLines = expected.Split('\n');
        var actualLines = actual.Split('\n');
        var report = new StringBuilder();
        var shown = 0;

        for (var i = 0; i < Math.Max(expectedLines.Length, actualLines.Length) && shown < 40; i++)
        {
            var e = i < expectedLines.Length ? expectedLines[i] : null;
            var a = i < actualLines.Length ? actualLines[i] : null;
            if (e == a) continue;

            if (e is not null) report.AppendLine($"  {i + 1,4} - expected: {e.Trim()}");
            if (a is not null) report.AppendLine($"  {i + 1,4} + actual:   {a.Trim()}");
            shown++;
        }

        if (shown == 0) report.AppendLine("  (files differ only in trailing whitespace)");
        return report.ToString();
    }
}
