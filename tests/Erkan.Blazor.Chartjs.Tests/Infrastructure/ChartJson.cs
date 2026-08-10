using System.Text.Json;

namespace Erkan.Blazor.Chartjs.Tests.Infrastructure;

/// <summary>
/// The serializer the wrapper's configuration actually goes through.
/// </summary>
/// <remarks>
/// A chart configuration is handed to <c>IJSObjectReference.InvokeVoidAsync</c> as a plain
/// argument, so Blazor's JSRuntime — not the wrapper — serializes it. Every assertion in
/// this suite is made against <see cref="Web"/>, and
/// <see cref="SerializerParityTests"/> proves that <see cref="Web"/> and the options a real
/// <c>JSRuntime</c> carries produce byte-identical JSON for these models. Without that
/// proof the whole suite would be testing a serializer nobody runs.
/// </remarks>
public static class ChartJson
{
    /// <summary>What <c>JSRuntime</c> uses. See <see cref="SerializerParityTests"/>.</summary>
    public static readonly JsonSerializerOptions Web = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// The same options, indented. Indentation is whitespace only — it changes no key, no
    /// value and no ordering — so a snapshot written with this is a faithful record of what
    /// <see cref="Web"/> produces, and is something a reviewer can actually diff.
    /// <see cref="SnapshotTests.Indenting_changes_only_whitespace"/> holds that claim up.
    /// </summary>
    public static readonly JsonSerializerOptions WebIndented =
        new(JsonSerializerDefaults.Web) { WriteIndented = true };

    /// <summary>
    /// Serializes through the object's runtime type. Serializing through a static
    /// <c>object</c> or interface type would silently emit <c>{}</c>.
    /// </summary>
    public static string Serialize(object value) =>
        JsonSerializer.Serialize(value, value.GetType(), Web);

    /// <summary>Indented counterpart of <see cref="Serialize"/>.</summary>
    public static string SerializeIndented(object value) =>
        JsonSerializer.Serialize(value, value.GetType(), WebIndented);

    /// <summary>Serializes and reparses, for structural assertions.</summary>
    public static JsonDocument SerializeToDocument(object value) =>
        JsonDocument.Parse(Serialize(value));
}
