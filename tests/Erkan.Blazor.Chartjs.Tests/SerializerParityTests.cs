using System.Text.Json;
using System.Text.Json.Serialization;
using Erkan.Blazor.Chartjs.Tests.Infrastructure;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;

namespace Erkan.Blazor.Chartjs.Tests;

/// <summary>
/// Establishes that the serializer every other test in this suite uses is the serializer
/// that actually runs in a Blazor app.
/// </summary>
/// <remarks>
/// A chart configuration is never serialized by the wrapper. It is passed as a plain argument
/// to <c>IJSObjectReference.InvokeVoidAsync</c>, and Blazor's <c>JSRuntime</c> serializes it
/// with options the wrapper does not control. Asserting snapshots and key paths against
/// <c>JsonSerializerDefaults.Web</c> is therefore only meaningful if those options and
/// <c>JSRuntime</c>'s agree — so this file does not assume it, it checks it, against the real
/// <c>JSRuntime</c> type from Microsoft.JSInterop rather than a description of it.
/// </remarks>
public class SerializerParityTests
{
    /// <summary>
    /// Exposes the options a real <c>JSRuntime</c> serializes arguments with.
    /// <c>JSRuntime.JsonSerializerOptions</c> is <c>protected internal</c>, so reaching it
    /// means deriving from the real class — which is the point: this is the production type,
    /// not a stand-in.
    /// </summary>
    private sealed class ProbeJSRuntime : JSRuntime
    {
        public JsonSerializerOptions Options => JsonSerializerOptions;

        protected override void BeginInvokeJS(long taskId, string identifier, string? argsJson,
            JSCallResultType resultType, long targetInstanceId) =>
            throw new NotSupportedException("The probe never dispatches a call.");

        protected override void EndInvokeDotNet(DotNetInvocationInfo invocationInfo,
            in DotNetInvocationResult invocationResult) =>
            throw new NotSupportedException("The probe never dispatches a call.");
    }

    private static JsonSerializerOptions JSRuntimeOptions => new ProbeJSRuntime().Options;

    /// <summary>
    /// Every setting that shapes written JSON is identical between the two.
    /// </summary>
    [Fact]
    public void JSRuntime_matches_the_web_defaults_on_every_write_side_setting()
    {
        var runtime = JSRuntimeOptions;
        var web = ChartJson.Web;

        // the naming policy is the one that decides the key for a property with no
        // [JsonPropertyName], which is most of ScaleLimits and every marker property
        Assert.Equal(JsonNamingPolicy.CamelCase, runtime.PropertyNamingPolicy);
        Assert.Equal(web.PropertyNamingPolicy, runtime.PropertyNamingPolicy);

        Assert.Equal(web.DefaultIgnoreCondition, runtime.DefaultIgnoreCondition);
        Assert.Equal(web.IgnoreReadOnlyProperties, runtime.IgnoreReadOnlyProperties);
        Assert.Equal(web.IgnoreReadOnlyFields, runtime.IgnoreReadOnlyFields);
        Assert.Equal(web.IncludeFields, runtime.IncludeFields);
        Assert.Equal(web.WriteIndented, runtime.WriteIndented);
        Assert.Equal(web.DictionaryKeyPolicy, runtime.DictionaryKeyPolicy);
        Assert.Equal(web.Encoder, runtime.Encoder);
        Assert.Equal(web.ReferenceHandler, runtime.ReferenceHandler);
        Assert.Equal(web.UnknownTypeHandling, runtime.UnknownTypeHandling);
        Assert.Equal(web.PreferredObjectCreationHandling, runtime.PreferredObjectCreationHandling);
        // TypeInfoResolver is deliberately not compared: reading it materializes the default
        // reflection resolver, so the comparison would depend on which side was touched first
    }

    /// <summary>
    /// The three settings on which they differ, pinned deliberately.
    /// </summary>
    /// <remarks>
    /// "JSRuntime uses <c>JsonSerializerDefaults.Web</c>" is very nearly true and worth not
    /// taking on trust: in .NET 10 <c>JSRuntime</c> starts from the Web defaults and then
    /// makes three changes. None of them can alter the JSON a chart configuration writes,
    /// which is why <see cref="ChartJson.Web"/> is a sound stand-in — but if a future runtime
    /// changes one of them into something that does matter, this test says so instead of the
    /// whole suite quietly drifting away from production.
    /// <list type="number">
    ///   <item>Five interop converters, for <c>DotNetObjectReference</c>, <c>IJSObjectReference</c>,
    ///   <c>IJSStreamReference</c>, <c>DotNetStreamReference</c> and <c>byte[]</c>. No chart
    ///   configuration contains any of those types — the config is handed to JS as one
    ///   argument and the <c>DotNetObjectReference</c> travels as a separate one.</item>
    ///   <item><c>NumberHandling</c> back to <c>Strict</c>. The Web default's
    ///   <c>AllowReadingFromString</c> is a deserialization allowance; it changes nothing on
    ///   the write path.</item>
    ///   <item><c>MaxDepth</c> capped at 32. Enforced against every fixture below.</item>
    /// </list>
    /// </remarks>
    [Fact]
    public void JSRuntime_differs_from_the_web_defaults_in_exactly_three_known_ways()
    {
        var runtime = JSRuntimeOptions;

        Assert.Equal(JsonNumberHandling.AllowReadingFromString, ChartJson.Web.NumberHandling);
        Assert.Equal(JsonNumberHandling.Strict, runtime.NumberHandling);

        Assert.Equal(0, ChartJson.Web.MaxDepth); // 0 means the 64-level default
        Assert.Equal(32, runtime.MaxDepth);

        Assert.Empty(ChartJson.Web.Converters);
        Assert.Equal(
            [
                "DotNetObjectReferenceJsonConverterFactory",
                "JSObjectReferenceJsonConverter",
                "JSStreamReferenceJsonConverter",
                "DotNetStreamReferenceJsonConverter",
                "ByteArrayJsonConverter",
            ],
            runtime.Converters.Select(c => c.GetType().Name));
    }

    /// <summary>
    /// No configuration nests deeply enough for <c>JSRuntime</c>'s <c>MaxDepth</c> of 32 to
    /// truncate it — the one difference between the two option sets that could bite.
    /// </summary>
    [Theory]
    [MemberData(nameof(EveryConfiguration))]
    public void Every_configuration_stays_within_the_JSRuntime_depth_limit(string name, IChartConfigBox box)
    {
        var config = box.Value;
        // serializing with the real options is itself the check: MaxDepth overflow throws
        var json = JsonSerializer.Serialize(config, config.GetType(), JSRuntimeOptions);

        Assert.NotNull(json);
        Assert.False(string.IsNullOrWhiteSpace(name));
    }

    /// <summary>
    /// The settings comparison above could still miss a converter that changes an output.
    /// This is the claim that actually matters: for every configuration in the suite, both
    /// option sets produce the same bytes.
    /// </summary>
    [Theory]
    [MemberData(nameof(EveryConfiguration))]
    public void Every_configuration_serializes_identically_under_both_option_sets(string name, IChartConfigBox box)
    {
        var config = box.Value;
        var throughWebDefaults = JsonSerializer.Serialize(config, config.GetType(), ChartJson.Web);
        var throughJSRuntime = JsonSerializer.Serialize(config, config.GetType(), JSRuntimeOptions);

        Assert.Equal(throughJSRuntime, throughWebDefaults);
        Assert.False(string.IsNullOrWhiteSpace(name));
    }

    /// <summary>
    /// Indentation is the only difference between the options snapshots are written with and
    /// the options a browser receives, so a snapshot is a faithful record of production JSON.
    /// </summary>
    [Theory]
    [MemberData(nameof(EveryConfiguration))]
    public void Indented_and_compact_output_carry_the_same_content(string name, IChartConfigBox box)
    {
        var config = box.Value;
        var compact = ChartJson.Serialize(config);
        var indented = ChartJson.SerializeIndented(config);

        // reparse and re-emit compactly: identical bytes mean identical keys, values and order
        using var document = JsonDocument.Parse(indented);
        Assert.Equal(compact, JsonSerializer.Serialize(document.RootElement, ChartJson.Web));
        Assert.False(string.IsNullOrWhiteSpace(name));
    }

    /// <summary>xUnit needs the payload to be serializable for display; a box keeps it opaque.</summary>
    public sealed class IChartConfigBox(object value) : Xunit.Abstractions.IXunitSerializable
    {
        public object Value { get; private set; } = value;

        public IChartConfigBox() : this(new object()) { }

        public void Deserialize(Xunit.Abstractions.IXunitSerializationInfo info) { }

        public void Serialize(Xunit.Abstractions.IXunitSerializationInfo info) { }

        public override string ToString() => Value.GetType().Name;
    }

    public static TheoryData<string, IChartConfigBox> EveryConfiguration()
    {
        var data = new TheoryData<string, IChartConfigBox>();
        foreach (var kind in SampleConfigs.Kinds)
        {
            data.Add($"{kind}.empty", new IChartConfigBox(SampleConfigs.Empty(kind)));
            data.Add($"{kind}.minimal", new IChartConfigBox(SampleConfigs.Minimal(kind)));
            data.Add($"{kind}.rich", new IChartConfigBox(SampleConfigs.Rich(kind)));
        }
        return data;
    }
}
