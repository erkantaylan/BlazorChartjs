using System.Reflection;
using System.Text.Json.Serialization;
using Erkan.Blazor.Chartjs.Models.Common;
using Erkan.Blazor.Chartjs.Models.Line;
using Erkan.Blazor.Chartjs.Models.Scatter;
using Erkan.Blazor.Chartjs.Tests.Infrastructure;

namespace Erkan.Blazor.Chartjs.Tests.Regression;

/// <summary>
/// 2.0.0: assigning <c>null</c> to a string-enum property threw a <c>NullReferenceException</c>.
/// </summary>
/// <remarks>
/// Nine properties dereferenced the incoming value in the setter to mirror it into the backing
/// <c>*String</c> property, so <c>legend.Position = null</c> — the obvious way to clear one back
/// to the Chart.js default — crashed instead of clearing it.
/// <para>
/// The theory below is driven by reflection over every string-enum property rather than by a
/// list of nine, so a tenth added later is covered without anyone remembering to add it.
/// </para>
/// </remarks>
public class NullAssignmentRegressionTests
{
    /// <summary>
    /// Every property whose type is one of the string-enum wrapper classes: a class with a
    /// <c>Value</c> string, used as the typed face of a <c>*String</c> JSON property.
    /// </summary>
    public static TheoryData<string, string> StringEnumProperties()
    {
        var data = new TheoryData<string, string>();
        foreach (var (owner, property) in Discover())
            data.Add(owner.FullName!, property.Name);
        return data;
    }

    [Theory]
    [MemberData(nameof(StringEnumProperties))]
    public void Assigning_null_clears_the_property_instead_of_throwing(string ownerName, string propertyName)
    {
        var owner = ModelGraph.LibraryAssembly.GetType(ownerName)!;
        var property = owner.GetProperty(propertyName)!;
        var instance = Activator.CreateInstance(owner)!;

        // set it to something first, so clearing has work to do
        var value = property.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Static)
            .First(p => p.PropertyType == property.PropertyType)
            .GetValue(null);
        property.SetValue(instance, value);
        Assert.NotNull(property.GetValue(instance));

        var exception = Record.Exception(() => property.SetValue(instance, null));
        Assert.True(exception is null,
            $"{owner.Name}.{propertyName} = null threw {exception?.InnerException?.GetType().Name}. "
            + "Clearing a string enum back to the Chart.js default must be allowed.");

        Assert.Null(property.GetValue(instance));
    }

    [Theory]
    [MemberData(nameof(StringEnumProperties))]
    public void Clearing_the_property_removes_the_serialized_key(string ownerName, string propertyName)
    {
        var owner = ModelGraph.LibraryAssembly.GetType(ownerName)!;
        var property = owner.GetProperty(propertyName)!;

        // the mirrored *String property is the one that carries the JSON key
        var mirror = owner.GetProperty(propertyName + "String")
                     ?? owner.GetProperties().FirstOrDefault(p =>
                         p.PropertyType == typeof(string)
                         && p.GetCustomAttribute<JsonPropertyNameAttribute>() is not null
                         && p.Name.StartsWith(propertyName, StringComparison.Ordinal));
        if (mirror is null) return;

        var instance = Activator.CreateInstance(owner)!;
        var value = property.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Static)
            .First(p => p.PropertyType == property.PropertyType)
            .GetValue(null);

        property.SetValue(instance, value);
        Assert.NotNull(mirror.GetValue(instance));

        property.SetValue(instance, null);
        Assert.Null(mirror.GetValue(instance));

        var key = mirror.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name;
        Assert.DoesNotContain($"\"{key}\"", ChartJson.Serialize(instance), StringComparison.Ordinal);
    }

    /// <summary>
    /// The nine the 2.0.0 notes name, asserted to be among those discovered — so the reflection
    /// above cannot quietly stop finding them.
    /// </summary>
    [Fact]
    public void The_nine_reported_properties_are_covered()
    {
        var discovered = Discover().Select(d => $"{d.Owner.Name}.{d.Property.Name}").ToHashSet(StringComparer.Ordinal);

        string[] nine =
        [
            $"{nameof(Legend)}.{nameof(Legend.Position)}",
            $"{nameof(Legend)}.{nameof(Legend.TextDirection)}",
            $"{nameof(Title)}.{nameof(Title.Position)}",
            $"{nameof(Axis)}.{nameof(Axis.Position)}",
            $"{nameof(AxesTitle)}.{nameof(AxesTitle.Align)}",
            $"{nameof(LineDataset)}.{nameof(LineDataset.CubicInterpolationMode)}",
            $"{nameof(LineDataset)}.{nameof(LineDataset.PointStyle)}",
            $"{nameof(LineDataset)}.{nameof(LineDataset.StepMode)}",
            $"{nameof(ScatterDataset)}.{nameof(ScatterDataset.PointStyle)}",
        ];

        Assert.Equal(9, nine.Length);
        foreach (var name in nine) Assert.Contains(name, discovered);
    }

    /// <summary>
    /// The five properties whose declared type was non-nullable over a backing field that stays
    /// null until assigned — the declaration was lying, and now says so.
    /// </summary>
    [Fact]
    public void The_five_mis_annotated_properties_are_declared_nullable()
    {
        var nullability = new NullabilityInfoContext();

        (Type Owner, string Property)[] five =
        [
            (typeof(Legend), nameof(Legend.Position)),
            (typeof(Legend), nameof(Legend.TextDirection)),
            (typeof(Title), nameof(Title.Position)),
            (typeof(Axis), nameof(Axis.Position)),
            (typeof(AxesTitle), nameof(AxesTitle.Align)),
        ];

        foreach (var (owner, name) in five)
        {
            var property = owner.GetProperty(name)!;
            Assert.Equal(NullabilityState.Nullable, nullability.Create(property).ReadState);
        }

        // Legend.Labels was null at runtime unless assigned; the declaration now agrees
        Assert.Equal(NullabilityState.Nullable,
            nullability.Create(typeof(Legend).GetProperty(nameof(Legend.Labels))!).ReadState);
        Assert.Null(new Legend().Labels);
    }

    /// <summary>
    /// 1.0.0: <c>LegendLabelsFilter</c> and friends threw <c>NotSupportedException</c> when the
    /// property they read was null. The models must at least survive being serialized in that
    /// state, which is the shape those helpers see.
    /// </summary>
    [Fact]
    public void A_legend_with_no_labels_object_still_serializes()
    {
        var legend = new Legend { Labels = null };
        var exception = Record.Exception(() => ChartJson.Serialize(legend));

        Assert.Null(exception);
        Assert.DoesNotContain("\"labels\"", ChartJson.Serialize(legend), StringComparison.Ordinal);
    }

    private static List<(Type Owner, PropertyInfo Property)> Discover()
    {
        var stringEnums = ModelGraph.LibraryAssembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && t.GetProperty("Value")?.PropertyType == typeof(string)
                        && t.GetProperties(BindingFlags.Public | BindingFlags.Static)
                            .Any(p => p.PropertyType == t))
            .ToHashSet();

        Assert.NotEmpty(stringEnums);

        return ModelGraph.LibraryAssembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, ContainsGenericParameters: false })
            .Where(t => t.GetConstructor(Type.EmptyTypes) is not null)
            .SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.SetMethod is { IsPublic: true } && stringEnums.Contains(p.PropertyType))
                .Select(p => (Owner: t, Property: p)))
            .OrderBy(x => x.Owner.Name, StringComparer.Ordinal)
            .ThenBy(x => x.Property.Name, StringComparer.Ordinal)
            .ToList();
    }
}
