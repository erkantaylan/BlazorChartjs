using System.Text.Json;

namespace Erkan.Blazor.Chartjs.Tests.Infrastructure;

/// <summary>
/// The checked-in answer to "what keys does Chart.js actually read", produced by
/// <c>tests/tools/chartjs-keys/generate.mjs</c> from the published TypeScript declarations
/// of Chart.js 4.5.1 and the bundled plugins, cross-checked against the live <c>defaults</c>
/// of the bundles vendored under <c>src/wwwroot/lib</c>.
/// </summary>
public sealed class ChartJsKeyList
{
    private static readonly Lazy<ChartJsKeyList> Instance = new(Load);

    public static ChartJsKeyList Current => Instance.Value;

    /// <summary>Every accepted path, relative to the root of a chart configuration.</summary>
    public required OptionPathSet Paths { get; init; }

    public required IReadOnlyList<string> RawPaths { get; init; }

    /// <summary>Accepted members of a Chart.js <c>LegendItem</c>.</summary>
    public required IReadOnlyList<string> LegendItemPaths { get; init; }

    /// <summary>
    /// Keys the wrapper's own <c>src/wwwroot/Chart.js</c> deletes from the config before
    /// handing it to Chart.js, scraped from its <c>delete</c> statements. These are markers
    /// the wrapper serializes on purpose so its JS knows a .NET callback is registered; they
    /// are legitimately absent from Chart.js's option tree.
    /// </summary>
    public required IReadOnlyList<string> StrippedByInterop { get; init; }

    public required IReadOnlyDictionary<string, string> Versions { get; init; }

    public required IReadOnlyList<string> RegisteredPlugins { get; init; }

    private static ChartJsKeyList Load()
    {
        var file = TestPaths.ChartJsKeyFile;
        if (!File.Exists(file))
        {
            throw new FileNotFoundException(
                $"The Chart.js key list is missing at {file}. Regenerate it with: " +
                "cd tests/tools/chartjs-keys && npm install && npm run generate");
        }

        using var document = JsonDocument.Parse(File.ReadAllText(file));
        var root = document.RootElement;

        static IReadOnlyList<string> Strings(JsonElement element, string name) =>
            element.GetProperty(name).EnumerateArray().Select(e => e.GetString()!).ToList();

        var paths = Strings(root, "paths");
        return new ChartJsKeyList
        {
            RawPaths = paths,
            Paths = new OptionPathSet(paths),
            LegendItemPaths = Strings(root, "legendItemPaths"),
            StrippedByInterop = Strings(root, "strippedByInterop"),
            RegisteredPlugins = Strings(root, "registeredPlugins"),
            Versions = root.GetProperty("versions").EnumerateObject()
                .ToDictionary(p => p.Name, p => p.Value.GetString()!),
        };
    }

    /// <summary>
    /// Whether a path lies inside a subtree the interop layer deletes. Matching on any
    /// segment, not just the last, is what exempts <c>plugins.crosshair.horizontal.color</c>
    /// along with <c>plugins.crosshair</c> itself.
    /// </summary>
    public bool IsStripped(string path) =>
        path.Split('.').Any(segment => StrippedByInterop.Contains(segment, StringComparer.Ordinal));
}
