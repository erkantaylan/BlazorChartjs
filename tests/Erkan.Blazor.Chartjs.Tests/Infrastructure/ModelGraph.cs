using System.Collections;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Erkan.Blazor.Chartjs.Interfaces;

namespace Erkan.Blazor.Chartjs.Tests.Infrastructure;

/// <summary>One serialized property, and where in a chart configuration it lands.</summary>
/// <param name="Path">Dotted path from the config root, e.g. <c>options.plugins.zoom.zoom.mode</c>.</param>
/// <param name="Key">The JSON key itself, e.g. <c>mode</c>.</param>
/// <param name="Property">The CLR property that produces it.</param>
public readonly record struct ModelKey(string Path, string Key, PropertyInfo Property)
{
    public string Owner => $"{Property.DeclaringType!.Name}.{Property.Name}";

    public override string ToString() => $"{Path}  ({Owner})";
}

/// <summary>
/// Walks the model type graph the way <c>System.Text.Json</c> walks it, producing the full
/// set of JSON paths a chart configuration can emit.
/// </summary>
/// <remarks>
/// Static, not reflective-of-an-instance: it finds every key the models are <em>capable</em>
/// of writing, including the ones no sample config happens to set. That is what makes the
/// key check exhaustive rather than a check of whatever the fixtures touch.
/// </remarks>
public static class ModelGraph
{
    public static Assembly LibraryAssembly => typeof(IChartConfig).Assembly;

    /// <summary>The eight chart configurations, which are the only roots the wrapper serializes.</summary>
    public static IReadOnlyList<Type> ConfigRoots { get; } =
        LibraryAssembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IChartConfig).IsAssignableFrom(t))
            .OrderBy(t => t.Name, StringComparer.Ordinal)
            .ToList();

    /// <summary>Every model type reachable by serializing one of <see cref="ConfigRoots"/>.</summary>
    public static IReadOnlyCollection<Type> ReachableTypes { get; } = ComputeReachableTypes();

    /// <summary>Every JSON path any chart configuration can emit.</summary>
    public static IReadOnlyList<ModelKey> AllKeys { get; } = ComputeAllKeys();

    private static IReadOnlyList<ModelKey> ComputeAllKeys()
    {
        var keys = new Dictionary<string, ModelKey>(StringComparer.Ordinal);
        foreach (var root in ConfigRoots)
            Walk(root, prefix: "", keys, new HashSet<string>(StringComparer.Ordinal), depth: 0);
        return keys.Values.OrderBy(k => k.Path, StringComparer.Ordinal).ToList();
    }

    private static IReadOnlyCollection<Type> ComputeReachableTypes()
    {
        var types = new HashSet<Type>();
        foreach (var root in ConfigRoots)
            CollectTypes(root, types, 0);
        return types;
    }

    private static void CollectTypes(Type type, HashSet<Type> seen, int depth)
    {
        if (depth > 24 || !IsModelType(type) || !seen.Add(type)) return;

        // a base type's properties are serialized through every derived type, so it is
        // covered as soon as one of its descendants is
        for (var baseType = type.BaseType; baseType is not null && IsModelType(baseType); baseType = baseType.BaseType)
            seen.Add(baseType);

        foreach (var property in SerializedProperties(type))
        {
            foreach (var next in ContentTypes(property.PropertyType))
            foreach (var candidate in WithAssignableTypes(next))
                CollectTypes(candidate, seen, depth + 1);
        }
    }

    /// <summary>
    /// A declared property type together with every concrete model type assignable to it.
    /// <c>BarChartConfig.Data</c> is declared as <c>Data&lt;BarDataset&gt;</c> but a consumer
    /// assigns a <c>BarData</c>, and <c>Options</c> holds a <c>PieOptions</c> on a pie chart —
    /// so the keys those subclasses add are part of the serialized surface too.
    /// </summary>
    private static IEnumerable<Type> WithAssignableTypes(Type type)
    {
        yield return type;
        if (!IsModelType(type)) yield break;

        foreach (var candidate in LibraryAssembly.GetTypes())
        {
            if (candidate != type
                && candidate is { IsClass: true, IsAbstract: false, ContainsGenericParameters: false }
                && type.IsAssignableFrom(candidate))
            {
                yield return candidate;
            }
        }
    }

    private static void Walk(Type type, string prefix, Dictionary<string, ModelKey> keys, HashSet<string> visiting, int depth)
    {
        // A model tree deeper than this is a cycle we failed to notice, not a real option path.
        if (depth > 24) return;

        var frame = $"{type.FullName}@{prefix}";
        if (!visiting.Add(frame)) return;

        foreach (var property in SerializedProperties(type))
        {
            var key = JsonKeyOf(property);
            var path = prefix.Length == 0 ? key : $"{prefix}.{key}";
            keys.TryAdd(path, new ModelKey(path, key, property));

            foreach (var (childType, childPath) in Descend(property.PropertyType, path))
            foreach (var candidate in WithAssignableTypes(childType))
            {
                if (IsModelType(candidate))
                    Walk(candidate, childPath, keys, visiting, depth + 1);
            }
        }

        visiting.Remove(frame);
    }

    /// <summary>
    /// Where a property's value lands relative to the property's own path. A list occupies
    /// the same path as the list itself (<c>data.datasets[0].label</c> is
    /// <c>data.datasets.label</c> here); a dictionary contributes a <c>*</c> segment, because
    /// its keys are chosen by the caller — scale ids, annotation ids.
    /// </summary>
    private static IEnumerable<(Type Type, string Path)> Descend(Type type, string path)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        var dictionary = ClosedInterface(type, typeof(IDictionary<,>));
        if (dictionary is not null)
        {
            yield return (dictionary.GetGenericArguments()[1], $"{path}.*");
            yield break;
        }

        if (type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type))
        {
            var enumerable = ClosedInterface(type, typeof(IEnumerable<>));
            if (enumerable is not null)
                yield return (enumerable.GetGenericArguments()[0], path);
            yield break;
        }

        yield return (type, path);
    }

    private static IEnumerable<Type> ContentTypes(Type type) => Descend(type, "x").Select(t => t.Type);

    private static Type? ClosedInterface(Type type, Type openGeneric) =>
        type.GetInterfaces()
            .Concat(type.IsInterface ? [type] : Array.Empty<Type>())
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGeneric);

    /// <summary>A type whose own properties become JSON keys, as opposed to a leaf value.</summary>
    public static bool IsModelType(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        return type.Assembly == LibraryAssembly && type is { IsClass: true, IsEnum: false };
    }

    /// <summary>
    /// The properties <c>System.Text.Json</c> would write: public, with a public getter, and
    /// not unconditionally ignored.
    /// </summary>
    public static IEnumerable<PropertyInfo> SerializedProperties(Type type) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetIndexParameters().Length == 0)
            .Where(p => p.GetMethod is { IsPublic: true })
            .Where(p => !IsUnconditionallyIgnored(p))
            .OrderBy(p => p.Name, StringComparer.Ordinal);

    private static bool IsUnconditionallyIgnored(PropertyInfo property)
    {
        var ignore = property.GetCustomAttribute<JsonIgnoreAttribute>();
        return ignore is not null && ignore.Condition == JsonIgnoreCondition.Always;
    }

    /// <summary>
    /// The JSON key a property serializes to: its <c>[JsonPropertyName]</c> when it has one,
    /// otherwise whatever the Web naming policy makes of the CLR name.
    /// </summary>
    public static string JsonKeyOf(PropertyInfo property) =>
        property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
        ?? JsonNamingPolicy.CamelCase.ConvertName(property.Name);

    /// <summary>Model types that carry a <c>[JsonPropertyName]</c> somewhere.</summary>
    public static IEnumerable<Type> TypesWithJsonPropertyNames() =>
        LibraryAssembly.GetTypes()
            .Where(t => t.IsClass && t.Namespace?.StartsWith("Erkan.Blazor.Chartjs.Models", StringComparison.Ordinal) == true)
            .Where(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>() is not null));
}
