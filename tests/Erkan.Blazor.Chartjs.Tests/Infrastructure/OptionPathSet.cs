namespace Erkan.Blazor.Chartjs.Tests.Infrastructure;

/// <summary>
/// A set of dotted option paths in which a <c>*</c> segment matches any single segment,
/// so <c>options.scales.*.min</c> accepts <c>options.scales.y2.min</c>.
/// </summary>
/// <remarks>
/// A trie rather than a flat set because the whole point of the key check is that nesting
/// matters: <c>mode</c> is a real option under <c>plugins.zoom.zoom</c> and a no-op under
/// <c>plugins.zoom</c>, and only a path-aware structure can tell those apart.
/// </remarks>
public sealed class OptionPathSet
{
    private sealed class Node
    {
        public readonly Dictionary<string, Node> Children = new(StringComparer.Ordinal);
        public bool Terminal;
    }

    private readonly Node _root = new();

    public OptionPathSet(IEnumerable<string> paths)
    {
        foreach (var path in paths)
        {
            var node = _root;
            foreach (var segment in path.Split('.'))
            {
                if (!node.Children.TryGetValue(segment, out var next))
                    node.Children[segment] = next = new Node();
                node = next;
            }
            node.Terminal = true;
        }
    }

    public bool Contains(string path) => Match(_root, path.Split('.'), 0);

    private static bool Match(Node node, string[] segments, int index)
    {
        if (index == segments.Length)
            return node.Terminal;

        var segment = segments[index];
        if (node.Children.TryGetValue(segment, out var exact) && Match(exact, segments, index + 1))
            return true;
        return segment != "*"
               && node.Children.TryGetValue("*", out var wildcard)
               && Match(wildcard, segments, index + 1);
    }

    /// <summary>
    /// The keys accepted directly beneath <paramref name="path"/>, for failure messages —
    /// "you wrote plugins.zoom.mode; plugins.zoom accepts pan, zoom, limits" is a far more
    /// useful failure than "unknown key".
    /// </summary>
    public IReadOnlyList<string> ChildrenOf(string path)
    {
        var nodes = new List<Node> { _root };
        foreach (var segment in path.Split('.', StringSplitOptions.RemoveEmptyEntries))
        {
            var next = new List<Node>();
            foreach (var node in nodes)
            {
                if (node.Children.TryGetValue(segment, out var exact)) next.Add(exact);
                if (segment != "*" && node.Children.TryGetValue("*", out var wildcard)) next.Add(wildcard);
            }
            if (next.Count == 0) return [];
            nodes = next;
        }
        return nodes.SelectMany(n => n.Children.Keys).Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToList();
    }
}
