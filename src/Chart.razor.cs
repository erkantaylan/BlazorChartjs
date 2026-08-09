using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Erkan.Blazor.Chartjs.Models.Bar;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace Erkan.Blazor.Chartjs
{
    /// <summary>
    /// Class Chart.
    /// Implements the <see cref="ComponentBase" />
    /// Implements the <see cref="System.IAsyncDisposable" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    /// <seealso cref="System.IAsyncDisposable" />
    public partial class Chart : IAsyncDisposable
    {
        #region .NET Object references

        /// <summary>
        /// Maps a configuration back to the component that rendered it, so the static
        /// [JSInvokable] entry points can raise the component's own event callbacks.
        /// The table holds no strong reference to either side.
        /// </summary>
        private static readonly ConditionalWeakTable<IChartConfig, Chart> Instances = new();

        /// <summary>
        /// The .NET object reference handed to JavaScript. Kept in a field so it can be
        /// released instead of being pinned for the lifetime of the circuit.
        /// </summary>
        private DotNetObjectReference<IChartConfig>? _dotNetObjectRef;

        /// <summary>
        /// Set once the component has been disposed, so late renders and JS callbacks
        /// become no-ops instead of touching a torn-down chart.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Set while a chart is being created in JavaScript. Blazor does not wait for one
        /// <see cref="OnAfterRenderAsync" /> to finish before starting the next render, and
        /// the first setup awaits a dynamic <c>import()</c> over the network, so renders
        /// triggered in that window would otherwise start a second setup and dispose the
        /// <see cref="DotNetObjectReference{TValue}" /> the first one is still marshalling.
        /// </summary>
        private bool _settingUp;

        /// <summary>
        /// Gets or sets the js module.
        /// </summary>
        /// <value>The js module.</value>
        protected ChartJsInterop? JSModule { get; set; }

        #endregion .NET Object references

        #region Parameters

        /// <summary>
        /// Gets or sets the class.
        /// </summary>
        /// <value>
        /// The class.
        /// </value>
        [Parameter]
        public string? Class { get; set; }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        [Parameter]
        public IChartConfig Config { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        [Parameter]
        public string? Height { get; set; }

        /// <summary>
        /// Gets or sets the old configuration.
        /// </summary>
        /// <value>
        /// The old configuration.
        /// </value>
        public IChartConfig OldConfig { get; set; }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>
        /// The style.
        /// </value>
        [Parameter]
        public string? Style { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        [Parameter]
        public string? Width { get; set; }

        #region Events

        /// <summary>
        /// Gets or sets the on chart click.
        /// </summary>
        /// <value>
        /// The on chart click.
        /// </value>
        [Parameter]
        public EventCallback<CallbackGenericContext> OnChartClick { get; set; }

        /// <summary>
        /// Gets or sets the on chart over.
        /// </summary>
        /// <value>
        /// The on chart over.
        /// </value>
        [Parameter]
        public EventCallback<HoverContext> OnChartOver { get; set; }

        /// <summary>
        /// Gets or sets the on legend click.
        /// </summary>
        /// <value>The on legend click.</value>
        [Parameter]
        public EventCallback<LegendClickContext> OnLegendClick { get; set; }

        #endregion

        #region Public functions

        /// <summary>
        /// Adds the data. The whole batch is appended in a single round trip and the
        /// chart is redrawn once.
        /// </summary>
        /// <param name="labels">The labels, or <c>null</c> to leave the labels untouched.</param>
        /// <param name="datasetIndex">Index of the dataset.</param>
        /// <param name="data">The data.</param>
        /// <returns>A task that completes once the chart has been updated. The call is a
        /// no-op when the chart has not been rendered yet.</returns>
        public async Task AddData(List<string?>? labels, int datasetIndex, List<decimal?>? data)
        {
            if (_disposed || JSModule == null || Config == null)
                return;

            try
            {
                await JSModule.AddData(Config.CanvasId, labels, datasetIndex, data);
            }
            catch (JSDisconnectedException) { }
            catch (ObjectDisposedException) { }
        }

        /// <summary>
        /// Adds the dataset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataset">The dataset.</param>
        /// <returns>A task that completes once the chart has been updated. The call is a
        /// no-op when the chart has not been rendered yet.</returns>
        public async Task AddDataset<T>(T dataset) where T : class
        {
            if (_disposed || JSModule == null || Config == null)
                return;

            try
            {
                await JSModule.AddNewDataset(Config.CanvasId, dataset);
            }
            catch (JSDisconnectedException) { }
            catch (ObjectDisposedException) { }
        }

        #endregion

        #endregion Parameters

        /// <summary>
        /// Destroys the Chart.js instance, releases the JavaScript module and the
        /// .NET object reference handed to it.
        /// </summary>
        /// <returns>System.Threading.Tasks.ValueTask.</returns>
        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (Config != null)
                Instances.Remove(Config);

            var module = JSModule;
            JSModule = null;

            if (module != null)
            {
                if (Config != null)
                    await module.DestroyChart(Config.CanvasId);

                await module.DisposeAsync();
            }

            _dotNetObjectRef?.Dispose();
            _dotNetObjectRef = null;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_disposed || Config == null || ReferenceEquals(Config, OldConfig))
                return;

            // A setup is already running. It re-reads Config when it finishes, so a
            // configuration assigned in the meantime is still picked up - without a second
            // setup racing the first one over the same canvas and object reference.
            if (_settingUp)
                return;

            _settingUp = true;

            try
            {
                while (!_disposed && Config != null && !ReferenceEquals(Config, OldConfig))
                {
                    var config = Config;
                    var previousConfig = OldConfig;
                    var previousRef = _dotNetObjectRef;

                    var objectRef = DotNetObjectReference.Create(config);
                    _dotNetObjectRef = objectRef;
                    Instances.AddOrUpdate(config, this);

                    // Claimed before the await, so a render that arrives while the chart is
                    // being created does not see this configuration as still pending.
                    OldConfig = config;

                    if (previousConfig != null && !ReferenceEquals(previousConfig, config))
                        Instances.Remove(previousConfig);

                    JSModule ??= new ChartJsInterop(JSRuntime);

                    try
                    {
                        await JSModule.Setup(objectRef, config,
                            hasHover: OnChartOver.HasDelegate,
                            hasLegendClick: OnLegendClick.HasDelegate);
                    }
                    catch (JSDisconnectedException)
                    {
                        // The circuit went away while the chart was being created.
                        return;
                    }
                    catch (ObjectDisposedException)
                    {
                        return;
                    }
                    finally
                    {
                        // Released only once the chart that used it has been replaced, so the
                        // reference is never disposed while JavaScript is still marshalling it.
                        // Doing it here rather than not at all is what keeps a chart whose
                        // Config is swapped repeatedly from leaking one reference per swap.
                        if (!ReferenceEquals(previousRef, objectRef))
                            previousRef?.Dispose();
                    }
                }
            }
            finally
            {
                _settingUp = false;
            }
        }

        /// <summary>
        /// Handles the <see cref="E:MouseOutAsync" /> event.
        /// </summary>
        /// <param name="mouseEventArgs">The <see cref="Microsoft.AspNetCore.Components.Web.MouseEventArgs" /> instance containing the event data.</param>
        /// <returns>System.Threading.Tasks.ValueTask.</returns>
        private ValueTask OnMouseOutAsync(MouseEventArgs mouseEventArgs)
        {
            if (Config.Options is Options { OnMouseOutAsync: { } } options)
                return options.OnMouseOutAsync(mouseEventArgs);
            else
                return ValueTask.CompletedTask;
        }

        #region JavaScript invokable functions

        /// <summary>
        /// Clears the data.
        /// </summary>
        /// <returns>A task that completes once the chart has been updated. The call is a
        /// no-op when the chart has not been rendered yet.</returns>
        public async Task ClearData()
        {
            if (_disposed || JSModule == null || Config == null)
                return;

            try
            {
                await JSModule.ClearData(Config.CanvasId);
            }
            catch (JSDisconnectedException) { }
            catch (ObjectDisposedException) { }
        }

        /// <summary>
        /// Legends the labels filter.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="item">The item.</param>
        /// <param name="data">The data.</param>
        /// <returns>System.Nullable&lt;System.Boolean&gt;.</returns>
        [JSInvokable]
        public static bool? LegendLabelsFilter(DotNetObjectReference<IChartConfig> config, LegendItem item, Data data)
        {
            var ctx = new LegendFilterContext(item, data);
            if (config.Value.Options is Options options && options.Plugins?.Legend?.Labels?.Filter != null)
                return options.Plugins.Legend.Labels.Filter(ctx);
            else
                return null;
        }

        /// <summary>
        /// Called when [click asynchronous].
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="ctx">The CTX.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        [JSInvokable]
        public static async Task OnClickAsync(DotNetObjectReference<IChartConfig> config, CallbackGenericContext ctx)
        {
            if (Instances.TryGetValue(config.Value, out var chart) && chart.OnChartClick.HasDelegate)
                await chart.OnChartClick.InvokeAsync(ctx);

            if (config.Value.Options is Options options && options.OnClickAsync != null)
                await options.OnClickAsync(ctx);
        }

        /// <summary>
        /// Called when [hover asynchronous].
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="ctx">The CTX.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        [JSInvokable]
        public static async Task OnHoverAsync(DotNetObjectReference<IChartConfig> config, HoverContext ctx)
        {
            if (Instances.TryGetValue(config.Value, out var chart) && chart.OnChartOver.HasDelegate)
                await chart.OnChartOver.InvokeAsync(ctx);

            if (config.Value.Options is Options options && options.OnHoverAsync != null)
                await options.OnHoverAsync(ctx);
        }

        /// <summary>
        /// Called when [legend click asynchronous].
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="ctx">The CTX.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        [JSInvokable]
        public static async Task OnLegendClickAsync(DotNetObjectReference<IChartConfig> config,
            LegendClickContext ctx)
        {
            if (Instances.TryGetValue(config.Value, out var chart) && chart.OnLegendClick.HasDelegate)
                await chart.OnLegendClick.InvokeAsync(ctx);

            if (config.Value.Options is Options options && options?.Plugins?.Legend?.OnClickAsync != null)
                await options.Plugins.Legend.OnClickAsync(ctx);
        }

        /// <summary>
        /// Tickses the callback.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="scaleName">Name of the scale.</param>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
        /// <param name="ticksValues">The ticks values.</param>
        /// <returns>System.String[].</returns>
        [JSInvokable]
        public static string[] TicksCallback(DotNetObjectReference<IChartConfig> config,
            string scaleName, decimal value, int index, decimal[] ticksValues)
        {
            var ctx = new TicksCallbackContext(value, index, ticksValues);
            if (config.Value.Options is Options options
                && options.Scales != null
                && options.Scales.TryGetValue(scaleName, out var axis)
                && axis.Ticks?.Callback != null)
                return axis.Ticks.Callback(ctx);
            else
                return Array.Empty<string>();
        }

        /// <summary>
        /// Tickses the callback asynchronous.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="scaleName">Name of the scale.</param>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
        /// <param name="ticksValues">The ticks values.</param>
        /// <returns>System.Threading.Tasks.Task&lt;System.String[]&gt;.</returns>
        [JSInvokable]
        public static async Task<string[]> TicksCallbackAsync(DotNetObjectReference<IChartConfig> config,
            string scaleName, decimal value, int index, decimal[] ticksValues)
        {
            var ctx = new TicksCallbackContext(value, index, ticksValues);
            if (config.Value.Options is Options options
                && options.Scales != null
                && options.Scales.TryGetValue(scaleName, out var axis)
                && axis.Ticks?.CallbackAsync != null)
                return await axis.Ticks.CallbackAsync(ctx);
            else
                return Array.Empty<string>();
        }

        /// <summary>
        /// Titles the callbacks.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String[].</returns>
        [JSInvokable]
        public static string[] TitleCallbacks(DotNetObjectReference<IChartConfig> config, decimal[] parameters)
        {
            if (parameters == null || parameters.Length < 3)
                return Array.Empty<string>();

            var ctx = new CallbackGenericContext((int)parameters[0], (int)parameters[1], parameters[2]);
            if (config.Value.Options is Options options && options.Plugins?.Tooltip?.Callbacks?.Title != null)
                return options.Plugins.Tooltip.Callbacks.Title(ctx);
            else
                return Array.Empty<string>();
        }

        /// <summary>
        /// Tooltips the callbacks label.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String[].</returns>
        [JSInvokable]
        public static string[] TooltipCallbacksLabel(DotNetObjectReference<IChartConfig> config, decimal[] parameters)
        {
            if (parameters == null || parameters.Length < 3)
                return Array.Empty<string>();

            var ctx = new CallbackGenericContext((int)parameters[0], (int)parameters[1], parameters[2]);
            if (config.Value.Options is Options options && options.Plugins?.Tooltip?.Callbacks?.Label != null)
                return options.Plugins.Tooltip.Callbacks.Label(ctx);
            else
                return Array.Empty<string>();
        }

        #endregion JavaScript invokable functions
    }
}
