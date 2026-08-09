using Microsoft.JSInterop;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Erkan.Blazor.Chartjs
{
    /// <summary>
    /// Class ChartJsInterop.
    /// Implements the <see cref="System.IAsyncDisposable" />
    /// </summary>
    /// <seealso cref="System.IAsyncDisposable" />
    public class ChartJsInterop : IAsyncDisposable
    {
        /// <summary>
        /// The module task
        /// </summary>
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartJsInterop"/> class.
        /// </summary>
        /// <param name="jsRuntime">The js runtime.</param>
        public ChartJsInterop(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import",
                "./_content/Erkan.Blazor.Chartjs/Chart.js").AsTask());
        }

        /// <summary>
        /// Setups the specified dot net object reference.
        /// </summary>
        /// <param name="dotNetObjectRef">The dot net object reference.</param>
        /// <param name="Config">The configuration.</param>
        /// <param name="hasHover">Whether the component exposes a hover handler of its own,
        /// in addition to <c>Options.OnHoverAsync</c>.</param>
        /// <param name="hasLegendClick">Whether a legend click handler is registered. When
        /// no handler is registered the built-in Chart.js legend toggle is left alone.</param>
        /// <returns>ValueTask.</returns>
        public async ValueTask Setup(DotNetObjectReference<IChartConfig> dotNetObjectRef, IChartConfig Config,
            bool hasHover = false, bool hasLegendClick = false)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("chartSetup", Config.CanvasId, dotNetObjectRef, Config,
                new { hasHover, hasLegendClick });
        }

        /// <summary>
        /// Adds the data. Labels and values are sent as one batch so the chart is
        /// updated once instead of once per point.
        /// </summary>
        /// <param name="CanvasId">The canvas identifier.</param>
        /// <param name="labels">The labels, or <c>null</c> to leave the labels untouched.</param>
        /// <param name="datasetIndex">Index of the dataset.</param>
        /// <param name="data">The data.</param>
        /// <returns>ValueTask.</returns>
        public async ValueTask AddData(string CanvasId, List<string?>? labels, int datasetIndex, List<decimal?>? data)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("addData", CanvasId, labels, datasetIndex, data);
        }

        /// <summary>
        /// Adds the new dataset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CanvasId">The canvas identifier.</param>
        /// <param name="dataset">The dataset.</param>
        /// <returns>ValueTask.</returns>
        public async ValueTask AddNewDataset<T>(string CanvasId, T dataset) where T : class
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("addNewDataset", CanvasId, dataset);
        }

        /// <summary>
        /// Clears the data.
        /// </summary>
        /// <param name="CanvasId">The canvas identifier.</param>
        /// <returns>ValueTask.</returns>
        public async ValueTask ClearData(string CanvasId)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("clearData", CanvasId);
        }

        /// <summary>
        /// Destroys the Chart.js instance bound to the canvas, together with the event
        /// listeners and queued redraws it owns.
        /// </summary>
        /// <param name="CanvasId">The canvas identifier.</param>
        /// <returns>ValueTask.</returns>
        public async ValueTask DestroyChart(string CanvasId)
        {
            if (!moduleTask.IsValueCreated)
                return;

            try
            {
                var module = await moduleTask.Value;
                await module.InvokeVoidAsync("destroyChart", CanvasId);
            }
            catch (JSDisconnectedException)
            {
                // The circuit is already gone; the browser released the chart with it.
            }
            catch (ObjectDisposedException) { }
        }

        /// <summary>
        /// Releases the imported JavaScript module.
        /// </summary>
        /// <returns>ValueTask.</returns>
        public async ValueTask DisposeAsync()
        {
            if (!moduleTask.IsValueCreated)
                return;

            try
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Nothing to release: the circuit that owned the module is gone.
            }
            catch (ObjectDisposedException) { }
        }
    }
}
