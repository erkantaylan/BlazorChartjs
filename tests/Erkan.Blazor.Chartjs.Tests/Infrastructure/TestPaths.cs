using System.Reflection;

namespace Erkan.Blazor.Chartjs.Tests.Infrastructure;

/// <summary>
/// Source-tree locations, injected at build time by the csproj so the tests read and write
/// the same files a reviewer sees in the diff rather than copies under bin/.
/// </summary>
public static class TestPaths
{
    private static string Metadata(string key) =>
        typeof(TestPaths).Assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == key)?.Value
        ?? throw new InvalidOperationException(
            $"Assembly metadata '{key}' is missing. It is set in Erkan.Blazor.Chartjs.Tests.csproj.");

    public static string ProjectDirectory => Metadata("ProjectDirectory");

    public static string RepositoryRoot => Metadata("RepositoryRoot");

    public static string SnapshotDirectory => Path.Combine(ProjectDirectory, "Snapshots");

    public static string ChartJsKeyFile =>
        Path.Combine(ProjectDirectory, "ChartJs", "chartjs-option-paths.json");

    /// <summary>The wrapper's own interop module, which post-processes the config in the browser.</summary>
    public static string InteropModule =>
        Path.Combine(RepositoryRoot, "src", "wwwroot", "Chart.js");

    public static string SourceModelsDirectory =>
        Path.Combine(RepositoryRoot, "src", "Models");
}
