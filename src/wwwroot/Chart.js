const DATA_TYPES = {
    "bar": "barData",
    "line": "lineData",
    "pie": "pieData",
    "doughnut": "doughnutData",
    "radar": "radarData",
    "polarArea": "polarData",
    "bubble": "bubbleData",
    "scatter": "scatterData"
};

// Per-canvas state. Keeping the live chart, its DOM listeners and any queued
// animation frame in one place lets destroyChart tear everything down instead
// of leaking a chart + listeners per navigation.
const chartStates = new Map();

function createState(id) {
    return {
        id: id,
        chart: null,
        destroyed: false,
        redrawHandle: 0,
        crosshairHandle: 0,
        crosshairEvent: null,
        crosshairListener: null,
        // Every callback cache created for this canvas, so a data change can drop
        // all of them at once.
        caches: []
    };
}

function releaseState(id) {
    let state = chartStates.get(id);
    if (state) {
        state.destroyed = true;

        if (state.redrawHandle) {
            cancelAnimationFrame(state.redrawHandle);
            state.redrawHandle = 0;
        }
        if (state.crosshairHandle) {
            cancelAnimationFrame(state.crosshairHandle);
            state.crosshairHandle = 0;
        }
        if (state.chart) {
            if (state.crosshairListener && state.chart.canvas)
                state.chart.canvas.removeEventListener("mousemove", state.crosshairListener);
            try { state.chart.destroy(); } catch (e) { /* canvas already detached */ }
            state.chart = null;
        }

        chartStates.delete(id);
    }

    // A chart may still be registered against this canvas if it was created
    // outside of chartSetup, or if a previous setup did not finish.
    let orphan = Chart.getChart(id);
    if (orphan) {
        try { orphan.destroy(); } catch (e) { /* canvas already detached */ }
    }
}

// Redraws are coalesced into a single animation frame: several async callbacks
// resolving in the same tick must not trigger several full chart updates.
function scheduleRedraw(state) {
    if (state.destroyed || state.redrawHandle)
        return;

    state.redrawHandle = requestAnimationFrame(() => {
        state.redrawHandle = 0;
        if (state.destroyed || !state.chart) return;
        state.chart.update('none');
    });
}

function newCallbackCache(state) {
    let cache = { values: new Map(), pending: new Set(), generation: 0 };
    if (state) state.caches.push(cache);
    return cache;
}

// Drops every cached callback result for a canvas. Called from the data-mutating
// entry points only: the cached values were computed from data that no longer
// exists, and a caller's callback may also close over state that changed with it.
// Deliberately NOT called from the render path - clearing on every update would
// turn resolve -> redraw -> resolve into an endless loop.
function invalidateCallbackCaches(id) {
    let state = chartStates.get(id);
    if (!state) return;

    for (const cache of state.caches) {
        // Results still in flight belong to the previous generation and are
        // discarded when they arrive, so they cannot overwrite a fresh value.
        cache.generation++;
        cache.values.clear();
        cache.pending.clear();
    }
}

// Chart.js callbacks are synchronous, but JS -> .NET calls have to be async
// (DotNet.invokeMethod is unsupported on Blazor Server and SSR). So: return a
// sensible value straight away, ask .NET for the real one in the background,
// cache it by key and redraw once. Every key is requested at most once per
// result, so the chart settles instead of looping.
function resolveAsync(state, cache, key, invoke, fallback) {
    if (cache.values.has(key))
        return cache.values.get(key);

    if (!cache.pending.has(key)) {
        cache.pending.add(key);
        let generation = cache.generation;
        invoke()
            .then(result => {
                // The data changed while .NET was answering: this result describes
                // data that is gone. The cache was already emptied, so drop it.
                if (generation !== cache.generation) return;

                cache.pending.delete(key);
                // Zoom/pan can produce an unbounded number of distinct keys over
                // the life of a chart; drop the cache rather than grow forever.
                if (cache.values.size > 1000) cache.values.clear();
                cache.values.set(key, result);
                scheduleRedraw(state);
            })
            .catch(() => {
                if (generation === cache.generation) cache.pending.delete(key);
            });
    }

    return fallback;
}

// The .NET callback contexts carry a single decimal value. Scatter and bubble
// data points are objects, which cannot be deserialized into a decimal, so
// reduce them to their most meaningful scalar instead of throwing.
function scalarDatum(value) {
    if (typeof value === 'number')
        return Number.isFinite(value) ? value : 0;

    if (typeof value === 'string') {
        let parsed = Number(value);
        return Number.isFinite(parsed) ? parsed : 0;
    }

    if (value && typeof value === 'object') {
        let candidate = typeof value.y === 'number' ? value.y
            : typeof value.r === 'number' ? value.r
                : value.x;
        return typeof candidate === 'number' && Number.isFinite(candidate) ? candidate : 0;
    }

    return 0;
}

function tickValues(ticks) {
    return ticks.map(tick => Number.isFinite(tick.value) ? tick.value : 0);
}

// Chart.js' own default tooltip label, used while .NET is being asked for the
// real one.
function defaultTooltipLabel(ctx) {
    let label = (ctx.dataset && ctx.dataset.label) || '';
    if (label) label += ': ';
    if (ctx.formattedValue !== null && ctx.formattedValue !== undefined)
        label += ctx.formattedValue;
    return label;
}

function crosshairLine(chart, evt, plugin) {
    const { canvas, ctx, chartArea: { left, right, top, bottom } } = chart;

    chart.update("none");

    if (plugin.cursor) {
        if (evt.offsetX >= left && evt.offsetX <= right && evt.offsetY <= bottom && evt.offsetY >= top) {
            canvas.style.cursor = plugin.cursor;
        } else
            canvas.style.cursor = "default";
    }

    if (plugin.vertical && evt.offsetX >= left && evt.offsetX <= right) {
        let line = plugin.vertical;

        ctx.save();
        ctx.beginPath();
        ctx.moveTo(evt.offsetX, top);
        ctx.lineTo(evt.offsetX, bottom);
        ctx.lineWidth = line.width;
        if (line.color)
            ctx.strokeStyle = line.color;
        if (line.dash)
            ctx.setLineDash(line.dash);
        ctx.stroke();
        ctx.restore();
    }

    if (plugin.horizontal && evt.offsetY <= bottom && evt.offsetY >= top) {
        let line = plugin.horizontal;

        ctx.save();
        ctx.beginPath();
        ctx.moveTo(left, evt.offsetY);
        ctx.lineTo(right, evt.offsetY);
        ctx.lineWidth = line.width;
        if (line.color)
            ctx.strokeStyle = line.color;
        if (line.dash)
            ctx.setLineDash(line.dash);
        ctx.stroke();
        ctx.restore();
    }
}

export function chartSetup(id, dotnetConfig, jsonConfig, hooks) {
    let chartElement = document.getElementById("chartcontainer" + id);
    if (!chartElement) return;

    hooks = hooks || {};

    // Tear down anything still attached to this canvas before the element is
    // replaced below, otherwise the old chart and its listeners are orphaned.
    releaseState(id);

    let state = createState(id);
    chartStates.set(id, state);

    chartElement.style.display = 'none';
    chartElement.innerHTML = '&nbsp;';
    chartElement.innerHTML = '<canvas id="' + id + '"></canvas>';
    chartElement.style.display = '';

    var context2d = document.getElementById(id).getContext('2d');
    let config = eval(jsonConfig);

    if (config?.options?.plugins?.legend?.labels?.hasFilter) {
        delete config.options.plugins.legend.labels.hasFilter;
        let filterCache = newCallbackCache(state);
        config.options.plugins.legend.labels.filter = function (item, data) {
            let key = item.datasetIndex + '|' + item.index + '|' + item.text;
            return resolveAsync(state, filterCache, key, () => {
                let json = JSON.stringify(data);
                let jsonArray = [...json];

                let dataType = DATA_TYPES[config.type] || "base";
                jsonArray.splice(1, 0, `"$type":"${dataType}",`);
                json = jsonArray.join("");

                return DotNet.invokeMethodAsync('Erkan.Blazor.Chartjs', 'LegendLabelsFilter',
                    dotnetConfig, item, JSON.parse(json))
                    // Only an explicit false hides the entry; a null means "no opinion".
                    .then(keep => keep !== false);
            }, true);
        };
    }

    if (config?.options?.plugins?.tooltip?.callbacks?.hasLabel) {
        delete config.options.plugins.tooltip.callbacks.hasLabel;
        let labelCache = newCallbackCache(state);
        config.options.plugins.tooltip.callbacks.label = function (ctx) {
            var dsIndex = -1;
            var dIndex = -1;
            var vl = 0;
            if (ctx.datasetIndex >= 0 && ctx.dataIndex >= 0) {
                dsIndex = ctx.datasetIndex;
                dIndex = ctx.dataIndex;
                vl = scalarDatum(ctx.raw !== undefined
                    ? ctx.raw
                    : ctx.chart.data.datasets[dsIndex].data[dIndex]);
            }

            // The datum is part of the key: those three values are the whole input
            // .NET sees, so a new value has to produce a new lookup instead of
            // reusing the label computed for whatever used to sit at that index.
            return resolveAsync(state, labelCache, dsIndex + '|' + dIndex + '|' + vl,
                () => DotNet.invokeMethodAsync('Erkan.Blazor.Chartjs', 'TooltipCallbacksLabel',
                    dotnetConfig, [dsIndex, dIndex, vl]),
                defaultTooltipLabel(ctx));
        };
    }

    if (config?.options?.plugins?.tooltip?.callbacks?.hasCustomTitle) {
        delete config.options.plugins.tooltip.callbacks.hasCustomTitle;
        let titleCache = newCallbackCache(state);
        config.options.plugins.tooltip.callbacks.title = function (ctx) {
            var first = ctx && ctx.length ? ctx[0] : null;
            var dsIndex = -1;
            var dIndex = -1;
            var vl = 0;
            if (first && first.datasetIndex >= 0 && first.dataIndex >= 0) {
                dsIndex = first.datasetIndex;
                dIndex = first.dataIndex;
                vl = scalarDatum(first.raw !== undefined
                    ? first.raw
                    : first.chart.data.datasets[dsIndex].data[dIndex]);
            }

            return resolveAsync(state, titleCache, dsIndex + '|' + dIndex + '|' + vl,
                () => DotNet.invokeMethodAsync('Erkan.Blazor.Chartjs', 'TitleCallbacks',
                    dotnetConfig, [dsIndex, dIndex, vl]),
                first ? first.label : '');
        };
    }

    let crosshair_plugin = config?.options?.plugins?.crosshair;
    if (config?.options?.plugins?.crosshair) {
        config.options.plugins.crosshair = undefined;
    }

    let hasHover = !!config?.options?.hasOnHoverAsync || !!hooks.hasHover;
    if (config?.options?.hasOnHoverAsync !== undefined)
        delete config.options.hasOnHoverAsync;

    let hasLegendClick = !!hooks.hasLegendClick || !!config?.options?.plugins?.legend?.hasLegendClick;
    if (config?.options?.plugins?.legend?.hasLegendClick !== undefined)
        delete config.options.plugins.legend.hasLegendClick;

    if (hasHover && config?.options) {
        config.options.onHover = function (evt, activeElements, ch) {
            const canvasPosition = Chart.helpers.getRelativePosition(evt, ch);

            // Pie, doughnut, polarArea and radar have no cartesian x/y scales, and
            // a cartesian chart can name its axes anything. HoverContext carries
            // two non-nullable decimals, so an absent scale reports 0 rather than
            // throwing on every mousemove.
            const xScale = ch.scales ? ch.scales.x : null;
            const yScale = ch.scales ? ch.scales.y : null;
            const dataX = xScale ? xScale.getValueForPixel(canvasPosition.x) : 0;
            const dataY = yScale ? yScale.getValueForPixel(canvasPosition.y) : 0;

            var rtn = {
                DataX: dataX,
                DataY: dataY
            };

            return DotNet.invokeMethodAsync('Erkan.Blazor.Chartjs', 'OnHoverAsync',
                dotnetConfig, rtn);
        };
    }

    if (config?.options?.groupXAxis) {
        config.options.groupXAxis = undefined;

        config.options.scales.x.ticks.callback = function (label) {
            let realLabel = this.getLabelForValue(label)
            var lbl = realLabel.split(";")[0];
            return lbl;
        }

        config.options.scales.xAxis2.type = 'category';
        config.options.scales.xAxis2.grid.drawOnChartArea = false;
        config.options.scales.xAxis2.ticks.callback = function (label) {
            let realLabel = this.getLabelForValue(label)

            var lbl = realLabel.split(";")[1];
            var position = realLabel.split(";")[2];
            if (position !== undefined && position !== '') {
                return lbl;
            } else {
                return "";
            }
        }
    }

    if (config?.options?.groupYAxis) {
        config.options.groupYAxis = undefined;

        config.options.scales.y.ticks.callback = function (label) {
            let realLabel = this.getLabelForValue(label)
            var lbl = realLabel.split(";")[0];
            return lbl;
        }

        config.options.scales.yAxis2.type = 'category';
        config.options.scales.yAxis2.grid.drawOnChartArea = false;
        config.options.scales.yAxis2.ticks.callback = function (label) {
            let realLabel = this.getLabelForValue(label)

            var lbl = realLabel.split(";")[1];
            var position = realLabel.split(";")[2];
            if (position !== undefined && position !== '') {
                return lbl;
            } else {
                return "";
            }
        }
    }

    if (config?.options?.locale && config?.options?.scales != null) {
        var scaleKeys = Object.keys(config.options.scales);
        for (let key of scaleKeys) {
            var scl = config.options.scales[key];
            if (scl?.type === 'time' || scl?.time) {
                if (!scl.adapters) scl.adapters = {};
                if (!scl.adapters.date) scl.adapters.date = {};
                if (!scl.adapters.date.locale) scl.adapters.date.locale = config.options.locale;
            }
        }
    }

    if (config?.options?.scales != null) {
        var scales = Object.keys(config.options.scales);
        for (let scale of scales) {
            if (config.options.scales[scale]?.ticks?.hasCallback) {
                delete config.options.scales[scale].ticks.hasCallback;
                installTicksCallback(state, config, scale, dotnetConfig, 'TicksCallback');
            }
            if (config.options.scales[scale]?.ticks?.hasAsyncCallback) {
                delete config.options.scales[scale].ticks.hasAsyncCallback;
                installTicksCallback(state, config, scale, dotnetConfig, 'TicksCallbackAsync');
            }
        }
    }

    // The DataLabels plugin is attached to this chart only. Chart.register would
    // be process-wide, so on a page with two charts the last one to render would
    // decide for both.
    if (config?.options?.registerDataLabels) {
        if (typeof ChartDataLabels !== 'undefined') {
            if (!config.plugins) config.plugins = [];
            config.plugins.push(ChartDataLabels);
        } else {
            console.warn('[BlazorChartjs] RegisterDataLabels is set but chartjs-plugin-datalabels is not loaded.');
        }
    }
    if (config?.options?.registerDataLabels !== undefined)
        delete config.options.registerDataLabels;

    // chartjs-plugin-annotation is only registered when the chart actually declares
    // annotations, so charts without them keep the plugin out of the draw loop.
    if (config?.options?.plugins?.annotation) {
        const annotationPlugin = window['chartjs-plugin-annotation'];
        if (annotationPlugin)
            Chart.register(annotationPlugin);
        else
            console.warn('[BlazorChartjs] plugins.annotation is set but chartjs-plugin-annotation.min.js is not loaded.');
    }

    // Clean up floating-point noise on tick values (e.g. after zoom plugin
    // recalculates axis bounds, 0 can become 1.42e-14, 100 becomes 100.00000000001).
    // The tolerance is relative, so 0.30000000000000004 is cleaned up while data
    // that legitimately lives around 1e-11 survives untouched.
    if (!config.plugins) config.plugins = [];
    config.plugins.push({
        id: 'floatCleanup',
        afterBuildTicks: function (chart, args) {
            var scale = args.scale;
            if (!scale || !scale.ticks)
                return;
            if (scale.type !== 'linear' && scale.type !== 'logarithmic' && scale.type !== 'radialLinear')
                return;

            // Anything smaller than a ten-billionth of the visible range is noise
            // around zero rather than a value the axis could ever render apart.
            var span = Math.abs(scale.max - scale.min);
            var epsilon = Number.isFinite(span) && span > 0 ? span * 1e-10 : 0;
            var snapToZero = scale.type !== 'logarithmic' && epsilon > 0;

            // The rounding below may never move a tick by more than this. It is
            // derived from the axis, not from a fixed digit count: on an axis
            // spanning 10 units around 1.2e12 (epoch milliseconds, byte counts,
            // ids) a tick has to keep 24 significant digits' worth of resolution,
            // and rounding it to 12 would collapse whole groups of ticks onto the
            // same wrong label.
            var resolution = Number.isFinite(span) && span > 0 ? span * 1e-12 : 0;
            var resolutionExponent = resolution > 0 ? Math.floor(Math.log10(resolution)) : 0;

            for (var i = 0; i < scale.ticks.length; i++) {
                var value = scale.ticks[i].value;
                if (!Number.isFinite(value))
                    continue;

                if (snapToZero && Math.abs(value) < epsilon) {
                    scale.ticks[i].value = 0;
                    continue;
                }

                // An exact integer carries no binary-representation noise to strip -
                // rounding it could only turn a correct value into a wrong one. This is
                // what keeps epoch milliseconds, byte counts, ids and monetary minor
                // units intact however wide the axis is.
                if (resolution <= 0 || Number.isInteger(value))
                    continue;

                // Keep every digit down to the axis resolution, and never fewer
                // than 12 significant digits (that is what strips the
                // 0.30000000000000004 style noise this plugin exists for, and it
                // is far more precision than any small value needs to survive).
                // 17 digits round-trip any double exactly, so anything at or above
                // that is a no-op - which is exactly what a value carrying more
                // significant digits than the axis can resolve should get.
                var digits = Math.floor(Math.log10(Math.abs(value))) - resolutionExponent + 1;
                if (digits < 12) digits = 12;
                if (digits > 21) digits = 21;

                var cleaned = Number(value.toPrecision(digits));
                // Belt and braces: a rounding that would actually be visible on
                // this axis is not noise removal, so it is refused.
                if (Math.abs(cleaned - value) <= resolution)
                    scale.ticks[i].value = cleaned;
            }
        }
    });

    var chart = new Chart(context2d, config);
    state.chart = chart;

    if (crosshair_plugin) {
        // One redraw per animation frame instead of one per mouse pixel.
        state.crosshairListener = function (evt) {
            state.crosshairEvent = { offsetX: evt.offsetX, offsetY: evt.offsetY };
            if (state.crosshairHandle) return;

            state.crosshairHandle = requestAnimationFrame(() => {
                state.crosshairHandle = 0;
                if (state.destroyed || !state.chart || !state.crosshairEvent) return;
                crosshairLine(state.chart, state.crosshairEvent, crosshair_plugin);
            });
        };
        chart.canvas.addEventListener("mousemove", state.crosshairListener);
    }

    chart.options.onClick = function (evt, activeElements, chart) {
        if (activeElements.length > 0) {
            var dsIndex = activeElements[0].datasetIndex;
            var dIndex = activeElements[0].index;
            var vl = 0;

            if (dsIndex >= 0 && dIndex >= 0) {
                vl = scalarDatum(chart.data.datasets[dsIndex].data[dIndex]);
            }

            var rtn = {
                DatasetIndex: dsIndex,
                DataIndex: dIndex,
                Value: vl
            };

            DotNet.invokeMethodAsync('Erkan.Blazor.Chartjs', 'OnClickAsync',
                dotnetConfig, rtn);
        }
    };

    // Only take over the legend click when a handler is actually registered, and
    // always run Chart.js' own handler first so the built-in show/hide toggle
    // keeps working.
    // Radar/polar/pie charts and charts configured with Plugins = null may have no
    // resolved legend options, so guard before overriding onClick.
    if (hasLegendClick && chart.options.plugins?.legend) {
        let defaultLegendClick = chart.options.plugins.legend.onClick;

        chart.options.plugins.legend.onClick = function (e, legendItem, legend) {
            if (typeof defaultLegendClick === 'function')
                defaultLegendClick.call(this, e, legendItem, legend);

            var rtn = {
                // Dataset legends (bar/line/scatter/...) carry datasetIndex and no
                // index; pie/doughnut/polarArea legends are per data point and
                // carry index instead.
                LegendIndex: legendItem.datasetIndex ?? legendItem.index,
                LegendText: legendItem.text
            };

            DotNet.invokeMethodAsync('Erkan.Blazor.Chartjs', 'OnLegendClickAsync',
                dotnetConfig, rtn);
        };
    }
}

function installTicksCallback(state, config, scale, dotnetConfig, methodName) {
    let cache = newCallbackCache(state);

    config.options.scales[scale].ticks.callback = function (value, index, ticks) {
        // Keyed by the tick that was requested, by its value so that a zoom or pan
        // (which reuses the same indexes for different values) re-resolves, and by
        // the underlying label: on a category axis the value IS the index, so the
        // label is the only part of the key that moves when the data changes.
        // The label is a pure function of the chart data, never of what this
        // callback returned, so the key cannot change from one render to the next
        // on its own - the resolve/redraw loop still settles.
        let label = this.getLabelForValue(value);
        let key = index + ':' + value + ':' + label;
        let values = tickValues(ticks);

        return resolveAsync(state, cache, key,
            () => DotNet.invokeMethodAsync('Erkan.Blazor.Chartjs', methodName,
                dotnetConfig, scale, value, index, values),
            label);
    };
}

export function destroyChart(id) {
    releaseState(id);
}

export function addData(id, labels, dataset, data) {
    var chart = Chart.getChart(id);
    if (!chart) return;

    if (labels) {
        if (!chart.data.labels) chart.data.labels = [];
        for (const label of labels)
            chart.data.labels.push(label);
    }

    var target = chart.data.datasets ? chart.data.datasets[dataset] : undefined;
    if (target && data) {
        if (!target.data) target.data = [];
        for (const value of data)
            target.data.push(value);
    }

    // Tooltip, title, tick and legend labels resolved from the previous data are
    // no longer valid.
    invalidateCallbackCaches(id);

    // One update for the whole batch, not one per point.
    chart.update();
}

export function addNewDataset(id, dataset) {
    var chart = Chart.getChart(id);
    if (!chart) return;

    chart.data.datasets.push(dataset);

    invalidateCallbackCaches(id);
    chart.update();
}

export function clearData(id) {
    var chart = Chart.getChart(id);
    if (!chart) return;

    chart.data.labels = [];

    chart.data.datasets.forEach((dataset) => {
        dataset.data = [];
    });

    invalidateCallbackCaches(id);
    chart.update();
}
