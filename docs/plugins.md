# Plugins

## Add labels to the chart

The component bundles the `chartjs-plugin-datalabels` plugin. This plugin shows the labels for each point in each graph. For more details about this plugin, visit its [website](https://chartjs-plugin-datalabels.netlify.app/).

![image](https://user-images.githubusercontent.com/9497415/224721251-da6959de-2b20-4d42-926b-b036de6695ee.png)

First, in the _index.html_, we have to add after the Chart.js script, another script for this component. It is important to add the script for `chartjs-plugin-datalabels` after Chart.js. If the order is different, the plugin could not work. For example

```html
<script src="_content/Erkan.Blazor.Chartjs/lib/Chart.js/chart.umd.js"></script>
<script src="_content/Erkan.Blazor.Chartjs/lib/hammer.js/hammer.js"></script>
<script src="_content/Erkan.Blazor.Chartjs/lib/chartjs-plugin-zoom/chartjs-plugin-zoom.js"></script>
<script src="_content/Erkan.Blazor.Chartjs/lib/chartjs-plugin-datalabels/chartjs-plugin-datalabels.js"></script>
```

> Use `chart.umd.js`, not `chart.js`. Both files ship in the package, but `chart.js` is the **ES module** build and throws `Unexpected token 'export'` when loaded from a classic `<script>` tag.

In the code, you have to change the property `RegisterDataLabels` under `Options` to `true`. That asks the component to register the library if the library is added to the page and there is data to show. For example, for a `LineChartConfig` the code is

```csharp
_config1 = new LineChartConfig()
{
    Options = new Options()
    {
        RegisterDataLabels = true,
        Plugins = new Plugins()
        {
            DataLabels = new DataLabels()
            {
                Align = DatalabelsAlign.Start,
                Anchor = DatalabelsAnchor.Start,
            }
        }
    }
};
```

With this code, the component will register the library in Chart.js. It is possible to define a `DataLabels` for the entire chart. Also, each dataset can have its own `DataLabels` that rewrites the common settings.

The plugin is attached to this chart alone. Upstream called the process-wide `Chart.register` / `Chart.unregister`, so on a page with several charts the last one to render decided whether labels were shown on all of them (upstream [#83](https://github.com/erossini/BlazorChartjs/issues/83)). If `RegisterDataLabels` is `true` but the plugin script is missing from the page, the component logs a warning to the browser console instead of failing silently.

## Zoom and pan

Zoom is provided by [chartjs-plugin-zoom](https://www.chartjs.org/chartjs-plugin-zoom/latest/) 2.x, which needs `hammer.js` loaded before it for pinch and pan gestures.

```csharp
Plugins = new Plugins()
{
    Zoom = new Zoom()
    {
        Mode = "x",
        ZoomOptions = new ZoomOptions()
        {
            Wheel = new Wheel() { Enabled = true },
            Pinch = new Pinch() { Enabled = true }
        },
        Pan = new Pan() { Enabled = true, Mode = "x" },
        Limits = new Limits()
        {
            X = new ScaleLimits() { Min = "0", Max = "100", MinRange = 10 }
        }
    }
}
```

Notes for anyone coming from upstream:

- `Zoom.Enabled` no longer exists — `Enabled = true` will not compile. Plugin 2.x has no master switch, so there is nothing for the property to turn on. Enable each gesture instead: `ZoomOptions.Wheel`, `ZoomOptions.Pinch` and `ZoomOptions.Drag` for zooming, and `Zoom.Pan` for panning (as in the example above).
- `Zoom.Mode` and `Zoom.OverScaleMode` used to serialize next to the plugin options instead of inside them, so the plugin never read them and `Mode = "x"` silently did nothing. They are written to the right place now. You can also set `Mode`, `OverScaleMode` and `ScaleMode` on `ZoomOptions` directly; a value set there wins, whichever order the two are assigned in — and the `Zoom.Mode` / `Zoom.OverScaleMode` getters report the value that will actually be serialized, not the one you handed them.
- `Limits` was an empty class upstream and could not express anything. It now has `X` and `Y`. `ScaleLimits.Min`/`Max` take a numeric string or the literal `"original"`; numbers serialize as JSON numbers, because the plugin does arithmetic on them. Leaving one unset omits it rather than defaulting it to `"original"`.
