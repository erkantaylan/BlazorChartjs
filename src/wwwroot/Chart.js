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

export function chartSetup(id, dotnetConfig, jsonConfig) {
    let chartElement = document.getElementById("chartcontainer" + id);
    if (!chartElement) return;

    chartElement.style.display = 'none';
    chartElement.innerHTML = '&nbsp;';
    chartElement.innerHTML = '<canvas id="' + id + '"></canvas>';
    chartElement.style.display = '';

    var context2d = document.getElementById(id).getContext('2d');
    let config = eval(jsonConfig);

    if (config?.options?.plugins?.legend?.labels?.hasFilter) {
        config.options.plugins.legend.labels.hasFilter = undefined;
        config.options.plugins.legend.labels.filter = function (item, data) {
            let json = JSON.stringify(data);
            let jsonArray = [...json];

            let dataType = DATA_TYPES[jsonConfig.type];
            jsonArray.splice(1, 0, `"$type":"${dataType}",`);
            json = jsonArray.join("");

            return DotNet.invokeMethod('Erkan.Blazor.Chartjs', 'LegendLabelsFilter',
                dotnetConfig, item, JSON.parse(json))
        };
    }

    if (config?.options?.plugins?.tooltip?.callbacks?.hasLabel) {
        config.options.plugins.tooltip.callbacks.hasLabel = undefined;
        config.options.plugins.tooltip.callbacks.label = function (ctx) {
            var dsIndex = -1;
            var dIndex = -1;
            var vl = 0;
            if (ctx.datasetIndex >= 0 && ctx.dataIndex >= 0) {
                dsIndex = ctx.datasetIndex;
                dIndex = ctx.dataIndex;
                vl = chart.data.datasets[dsIndex].data[dIndex];
            }
            return DotNet.invokeMethod('Erkan.Blazor.Chartjs', 'TooltipCallbacksLabel',
                dotnetConfig, [dsIndex, dIndex, vl]);
        };
    }

    if (config?.options?.plugins?.tooltip?.callbacks?.hasCustomTitle) {
        config.options.plugins.tooltip.callbacks.hasCustomTitle = undefined;
        config.options.plugins.tooltip.callbacks.title = function (ctx) {
            var dsIndex = -1;
            var dIndex = -1;
            var vl = 0;
            if (ctx[0].datasetIndex >= 0 && ctx[0].dataIndex >= 0) {
                dsIndex = ctx[0].datasetIndex;
                dIndex = ctx[0].dataIndex;
                vl = chart.data.datasets[dsIndex].data[dIndex];
            }
            return DotNet.invokeMethod('Erkan.Blazor.Chartjs', 'TitleCallbacks',
                dotnetConfig, [dsIndex, dIndex, vl]);
        };
    }

    let crosshair_plugin = config?.options?.plugins?.crosshair;
    if (config?.options?.plugins?.crosshair) {
        config.options.plugins.crosshair = undefined;
    }

    if (config?.options?.hasOnHoverAsync) {
        config.options.hasOnHoverAsync = undefined;
        config.options.onHover = function (evt, activeElements, ch) {
            const canvasPosition = Chart.helpers.getRelativePosition(evt, ch);

            // Substitute the appropriate scale IDs
            const dataX = ch.scales.x.getValueForPixel(canvasPosition.x);
            const dataY = ch.scales.y.getValueForPixel(canvasPosition.y);

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
                config.options.scales[scale].ticks.callback = function (value, index, ticks) {
                    return DotNet.invokeMethod('Erkan.Blazor.Chartjs', 'TicksCallback',
                        dotnetConfig, scale, value, index, ticks.map(tick => tick.value));
                };
            }
            if (config.options.scales[scale]?.ticks?.hasAsyncCallback) {
                delete config.options.scales[scale].ticks.hasAsyncCallback;
                // Chart.js tick callbacks are synchronous - a Promise return value would be
                // rendered as "[object Promise]". Resolve the labels up front instead: render
                // the raw values on the first pass, then re-label once .NET replies.
                let asyncLabels = {};
                let asyncPending = false;
                config.options.scales[scale].ticks.callback = function (value, index, ticks) {
                    if (Object.prototype.hasOwnProperty.call(asyncLabels, index))
                        return asyncLabels[index];

                    if (!asyncPending) {
                        asyncPending = true;
                        let chartRef = this.chart;
                        DotNet.invokeMethodAsync('Erkan.Blazor.Chartjs', 'TicksCallbackAsync',
                            dotnetConfig, scale, value, index, ticks.map(tick => tick.value))
                            .then(labels => {
                                asyncPending = false;
                                if (!labels) return;
                                labels.forEach((lbl, i) => asyncLabels[i] = lbl);
                                chartRef.update('none');
                            })
                            .catch(() => { asyncPending = false; });
                    }

                    return this.getLabelForValue(value);
                };
            }
        }
    }

    if (typeof ChartDataLabels !== 'undefined') {
        if (config?.options?.registerDataLabels)
            Chart.register(ChartDataLabels);
        else
            Chart.unregister(ChartDataLabels);
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
    // Rounding to 10 decimal places removes this noise while preserving meaningful precision.
    if (!config.plugins) config.plugins = [];
    config.plugins.push({
        id: 'floatCleanup',
        afterBuildTicks: function (chart, args) {
            var scale = args.scale;
            if (scale && scale.ticks &&
                (scale.type === 'linear' || scale.type === 'logarithmic' || scale.type === 'radialLinear')) {
                for (var i = 0; i < scale.ticks.length; i++) {
                    scale.ticks[i].value = Math.round(scale.ticks[i].value * 1e10) / 1e10;
                }
            }
        }
    });

    var chart = new Chart(context2d, config);
    if (crosshair_plugin) {
        chart.canvas.addEventListener("mousemove", (evt) => {
            crosshairLine(chart, evt, crosshair_plugin);
        });
    }

    chart.options.onClick = function (evt, activeElements, chart) {
        if (activeElements.length > 0) {
            var dsIndex = activeElements[0].datasetIndex;
            var dIndex = activeElements[0].index;
            var vl = 0;

            if (dsIndex >= 0 && dIndex >= 0) {
                vl = chart.data.datasets[dsIndex].data[dIndex];
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

    // Radar/polar/pie charts and charts configured with Plugins = null may have no
    // resolved legend options, so guard before overriding onClick.
    if (chart.options.plugins?.legend) {
        chart.options.plugins.legend.onClick = function (e, legendItem) {
            var rtn = {
                LegendIndex: legendItem.index,
                LegendText: legendItem.text
            };

            DotNet.invokeMethodAsync('Erkan.Blazor.Chartjs', 'OnLegendClickAsync',
                dotnetConfig, rtn);
        };
    }
}

export function addData(id, label, dataset, data) {
    var chart = Chart.getChart(id);

    if (label !== null)
        chart.data.labels.push(label);
    if (dataset < chart.data.datasets.length)
        chart.data.datasets[dataset].data.push(data);

    chart.update();
}

export function addNewDataset(id, dataset) {
    var chart = Chart.getChart(id);
    chart.data.datasets.push(dataset);
    chart.update();
}

export function clearData(id) {
    var chart = Chart.getChart(id);

    chart.data.labels = [];

    chart.data.datasets.forEach((dataset) => {
        dataset.data = [];
    });

    chart.update();
}