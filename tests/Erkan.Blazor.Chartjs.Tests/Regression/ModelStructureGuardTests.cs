using System.Reflection;
using System.Text.Json.Serialization;
using Erkan.Blazor.Chartjs.Tests.Infrastructure;

namespace Erkan.Blazor.Chartjs.Tests.Regression;

/// <summary>
/// Structural guards that make whole classes of defect unreachable, rather than testing the
/// individual instances of them that happened to ship.
/// </summary>
public class ModelStructureGuardTests
{
    /// <summary>
    /// The guard that retires the eighteen-property bug for good.
    /// </summary>
    /// <remarks>
    /// <c>WhenWritingDefault</c> on a non-nullable value type means the type's own default is
    /// unwritable: <c>0</c> and <c>false</c> produce no key, and Chart.js substitutes its own
    /// default. There is no correct use of that combination in a wrapper whose whole job is to
    /// transmit the value the caller assigned, so no new property can be written that way —
    /// with or without anyone remembering the eighteen.
    /// </remarks>
    [Fact]
    public void No_property_combines_WhenWritingDefault_with_a_non_nullable_value_type()
    {
        var offenders = new List<string>();

        foreach (var type in ModelTypes())
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var ignore = property.GetCustomAttribute<JsonIgnoreAttribute>();
            if (ignore?.Condition != JsonIgnoreCondition.WhenWritingDefault) continue;

            var propertyType = property.PropertyType;
            if (propertyType.IsValueType && Nullable.GetUnderlyingType(propertyType) is null)
                offenders.Add($"{type.Name}.{property.Name} ({propertyType.Name})");
        }

        Assert.True(offenders.Count == 0,
            $"""
             {offenders.Count} propert(ies) are non-nullable value types serialized with
             JsonIgnoreCondition.WhenWritingDefault. Each one silently drops the value 0 or false,
             so a caller has no way to send it and Chart.js applies its own default instead.

             Make the property nullable and use WhenWritingNull.

               {string.Join($"{Environment.NewLine}  ", offenders)}
             """);
    }

    /// <summary>
    /// The stronger claim the 2.0.0 notes make: after normalizing the four remaining harmless
    /// uses, <c>WhenWritingDefault</c> appears nowhere under <c>src/</c>, so grepping for it is
    /// a reliable check. This keeps that true.
    /// </summary>
    [Fact]
    public void WhenWritingDefault_appears_nowhere_in_the_library_source()
    {
        var sources = Directory.GetFiles(Path.Combine(TestPaths.RepositoryRoot, "src"), "*.cs",
            SearchOption.AllDirectories);

        var offenders = sources
            .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                           && !file.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
            .Where(file => File.ReadAllText(file).Contains("WhenWritingDefault", StringComparison.Ordinal))
            .Select(file => Path.GetRelativePath(TestPaths.RepositoryRoot, file))
            .Order(StringComparer.Ordinal)
            .ToList();

        Assert.True(offenders.Count == 0,
            "WhenWritingDefault is back under src/. Use WhenWritingNull on a nullable property:"
            + Environment.NewLine + "  " + string.Join($"{Environment.NewLine}  ", offenders));
    }

    /// <summary>
    /// Every nullable model property is omitted when unset rather than written as a bare
    /// <c>null</c>. Six properties shipped without this, so an unconfigured radar sent
    /// <c>"scales": null</c> and every <c>LineDataType</c> point carried <c>"x": null, "y": null</c>.
    /// </summary>
    [Fact]
    public void Every_nullable_property_is_omitted_rather_than_written_as_null()
    {
        var offenders = new List<string>();
        var nullability = new NullabilityInfoContext();

        foreach (var type in ModelTypes())
        {
            // a type with its own converter decides its own output; ScaleLimits omits unset
            // members by hand, and its properties carry no [JsonPropertyName] to go by
            if (type.GetCustomAttribute<JsonConverterAttribute>() is not null) continue;

            foreach (var property in ModelGraph.SerializedProperties(type))
            {
                // only properties that actually become JSON keys in the option tree
                if (property.GetCustomAttribute<JsonPropertyNameAttribute>() is null) continue;

                // a get-only computed marker cannot be null
                if (property.SetMethod is null) continue;

                // the declaration is the contract: T? can arrive null, T says it never does
                if (nullability.Create(property).ReadState != NullabilityState.Nullable) continue;

                var condition = property.GetCustomAttribute<JsonIgnoreAttribute>()?.Condition;
                if (condition is not (JsonIgnoreCondition.WhenWritingNull or JsonIgnoreCondition.WhenWritingDefault))
                    offenders.Add($"{type.Name}.{property.Name} ({property.PropertyType.Name})");
            }
        }

        Assert.True(offenders.Count == 0,
            $"""
             {offenders.Count} nullable propert(ies) are serialized without
             JsonIgnoreCondition.WhenWritingNull, so an unset one writes a bare null into the chart
             configuration instead of being omitted:

               {string.Join($"{Environment.NewLine}  ", offenders)}
             """);
    }

    private static IEnumerable<Type> ModelTypes() =>
        ModelGraph.LibraryAssembly.GetTypes()
            .Where(t => t.IsClass
                        && t.Namespace?.StartsWith("Erkan.Blazor.Chartjs.Models", StringComparison.Ordinal) == true)
            .OrderBy(t => t.Name, StringComparer.Ordinal);
}
