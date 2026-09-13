using System.Text.Json;
using Erkan.Blazor.Chartjs.Interfaces;
using Erkan.Blazor.Chartjs.Models.Bar;
using Erkan.Blazor.Chartjs.Models.Common;
using Erkan.Blazor.Chartjs.Models.Radar;
using Erkan.Blazor.Chartjs.Tests.Infrastructure;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;

namespace Erkan.Blazor.Chartjs.Tests;

/// <summary>
/// <c>Options.RegisterPlugins</c>: any Chart.js plugin the page has loaded, attached to one chart
/// by the global name its script defines.
/// </summary>
/// <remarks>
/// Nothing in this suite can run <c>src/wwwroot/Chart.js</c>, so the contract is covered in three
/// halves: the list serializes where the interop module reads it, the argument
/// <c>ChartJsInterop.Setup</c> hands to JavaScript really carries it, and the module's text
/// resolves it the way the docs say. That the plugin then draws is something only a person
/// viewing the demo can confirm. The marker's deletion is in <c>InternalMarkerLeakTests</c>.
/// </remarks>
public class PluginRegistrationTests
{
    private static readonly string Interop = File.ReadAllText(TestPaths.InteropModule);

    [Fact]
    public void An_unset_list_writes_no_key()
    {
        using var options = ChartJson.SerializeToDocument(new Options());
        Assert.False(options.RootElement.TryGetProperty("registerPlugins", out _));

        using var radar = ChartJson.SerializeToDocument(new RadarOptions());
        Assert.False(radar.RootElement.TryGetProperty("registerPlugins", out _));
    }

    [Fact]
    public void The_names_are_written_to_options_registerPlugins_as_given()
    {
        var names = new List<string> { "ChartDataLabels", "chartjs-plugin-autocolors" };

        using var options = ChartJson.SerializeToDocument(new Options { RegisterPlugins = names });
        Assert.Equal("""["ChartDataLabels","chartjs-plugin-autocolors"]""",
            options.RootElement.GetProperty("registerPlugins").GetRawText());

        using var radar = ChartJson.SerializeToDocument(new RadarOptions { RegisterPlugins = names });
        Assert.Equal("""["ChartDataLabels","chartjs-plugin-autocolors"]""",
            radar.RootElement.GetProperty("registerPlugins").GetRawText());
    }

    /// <summary>
    /// The call itself, through a recording <see cref="IJSRuntime"/>: the configuration argument
    /// of <c>chartSetup</c>, serialized with the options a real <c>JSRuntime</c> uses, carries
    /// the plugin names and the <c>ExtraOptions</c> entries inline. <c>JSRuntime</c> serializes
    /// its arguments as <c>object</c>, so this is also the proof that nothing on the way to the
    /// browser narrows the configuration to a type that has no bag.
    /// </summary>
    [Fact]
    public async Task Setup_hands_chartSetup_the_plugin_names_and_the_bag_entries()
    {
        var config = new BarChartConfig
        {
            Options = new Options
            {
                RegisterPlugins = ["chartjs-plugin-autocolors"],
                ExtraOptions = new() { ["datasets"] = new { bar = new { categoryPercentage = 0.5 } } },
                Plugins = new Plugins { ExtraOptions = new() { ["autocolors"] = new { mode = "data" } } },
            },
        };

        var runtime = new RecordingJSRuntime();
        await using (var interop = new ChartJsInterop(runtime))
        {
            using var reference = DotNetObjectReference.Create<IChartConfig>(config);
            await interop.Setup(reference, config);

            var call = Assert.Single(runtime.Module.Calls);
            Assert.Equal("chartSetup", call.Identifier);
            Assert.Same(config, call.Args[2]);

            using var payload = JsonDocument.Parse(JsonSerializer.Serialize(call.Args, new ProbeJSRuntime().Options));
            var options = payload.RootElement[2].GetProperty("options");

            Assert.Equal("""["chartjs-plugin-autocolors"]""", options.GetProperty("registerPlugins").GetRawText());
            Assert.Equal("""{"bar":{"categoryPercentage":0.5}}""", options.GetProperty("datasets").GetRawText());
            Assert.Equal("""{"mode":"data"}""", options.GetProperty("plugins").GetProperty("autocolors").GetRawText());
            Assert.False(options.TryGetProperty("extraOptions", out _));
        }
    }

    /// <summary>
    /// Resolved on <c>window</c>, pushed onto this chart's own <c>plugins</c> array without a
    /// duplicate, never registered process-wide, and a missing global is a console warning
    /// rather than a silent no-op — the same contract <c>RegisterDataLabels</c> has.
    /// </summary>
    [Fact]
    public void The_interop_module_attaches_each_named_global_to_this_chart_only()
    {
        var start = Interop.IndexOf("let registerPlugins", StringComparison.Ordinal);
        Assert.True(start >= 0, "src/wwwroot/Chart.js no longer reads options.registerPlugins.");

        var end = Interop.IndexOf("new Chart(context2d", start, StringComparison.Ordinal);
        Assert.True(end > start, "options.registerPlugins is no longer read before the chart is created.");
        var block = Interop[start..end];

        Assert.Contains("window[name]", block, StringComparison.Ordinal);
        Assert.Contains("!config.plugins.includes(plugin)", block, StringComparison.Ordinal);
        Assert.Contains("config.plugins.push(plugin)", block, StringComparison.Ordinal);
        Assert.Contains("console.warn('[BlazorChartjs] RegisterPlugins", block, StringComparison.Ordinal);
        Assert.DoesNotContain("Chart.register(plugin", block, StringComparison.Ordinal);
    }

    // ----------------------------------------------------------------------------------

    /// <summary>The real <c>JSRuntime</c>'s serializer options; see <see cref="SerializerParityTests"/>.</summary>
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

    /// <summary>Answers the module <c>import</c> with a module that records every call made on it.</summary>
    private sealed class RecordingJSRuntime : IJSRuntime
    {
        public RecordingModule Module { get; } = new();

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) =>
            identifier == "import"
                ? ValueTask.FromResult((TValue)(object)Module)
                : throw new NotSupportedException($"unexpected runtime call '{identifier}'");

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) =>
            InvokeAsync<TValue>(identifier, args);
    }

    private sealed class RecordingModule : IJSObjectReference
    {
        public List<(string Identifier, object?[] Args)> Calls { get; } = [];

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            Calls.Add((identifier, args ?? []));
            return ValueTask.FromResult(default(TValue)!);
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) =>
            InvokeAsync<TValue>(identifier, args);

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
