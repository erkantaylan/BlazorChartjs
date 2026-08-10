using System.Text.Json;

namespace Erkan.Blazor.Chartjs.Tests.Infrastructure;

/// <summary>A node in serialized JSON, addressed the same way <see cref="ModelGraph"/> addresses it.</summary>
/// <param name="Path">Array indices collapsed, so <c>data.datasets[0].label</c> is <c>data.datasets.label</c>.</param>
/// <param name="Pointer">The exact location, indices intact, for failure messages.</param>
public readonly record struct JsonNode(string Path, string Pointer, JsonElement Element)
{
    public JsonValueKind Kind => Element.ValueKind;
}

/// <summary>Flattens serialized JSON into addressable nodes.</summary>
public static class JsonPaths
{
    /// <summary>
    /// Every node below the root, arrays collapsed onto the path of the array itself so the
    /// result lines up with both <see cref="ModelGraph"/> and the generated Chart.js key list.
    /// </summary>
    public static IReadOnlyList<JsonNode> Enumerate(JsonElement root)
    {
        var nodes = new List<JsonNode>();
        Visit(root, "", "$", nodes);
        return nodes;
    }

    public static IReadOnlyList<JsonNode> Enumerate(object config)
    {
        using var document = ChartJson.SerializeToDocument(config);
        return Enumerate(document.RootElement.Clone());
    }

    /// <summary>
    /// Path to first occurrence. Paths repeat legitimately — two datasets both carry
    /// <c>data.datasets.borderColor</c> — so a plain dictionary build would throw.
    /// </summary>
    public static IReadOnlyDictionary<string, JsonElement> Map(object config)
    {
        var map = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
        foreach (var node in Enumerate(config)) map.TryAdd(node.Path, node.Element);
        return map;
    }

    private static void Visit(JsonElement element, string path, string pointer, List<JsonNode> nodes)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    var childPath = path.Length == 0 ? property.Name : $"{path}.{property.Name}";
                    var childPointer = $"{pointer}.{property.Name}";
                    nodes.Add(new JsonNode(childPath, childPointer, property.Value));
                    Visit(property.Value, childPath, childPointer, nodes);
                }
                break;

            case JsonValueKind.Array:
                var index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    // the array element shares its parent's path; only the pointer records the index
                    Visit(item, path, $"{pointer}[{index}]", nodes);
                    index++;
                }
                break;
        }
    }
}
