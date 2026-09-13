# Erkan.Blazor.Chartjs

[![NuGet](https://img.shields.io/nuget/v/Erkan.Blazor.Chartjs.svg)](https://www.nuget.org/packages/Erkan.Blazor.Chartjs/)
[![NuGet downloads](https://img.shields.io/nuget/dt/Erkan.Blazor.Chartjs.svg)](https://www.nuget.org/packages/Erkan.Blazor.Chartjs/)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/erkantaylan/BlazorChartjs/blob/main/LICENSE)

A [Chart.js](https://www.chartjs.org/) wrapper for [Blazor WebAssembly](https://learn.microsoft.com/aspnet/core/blazor/hosting-models) and Blazor Server, targeting **.NET 10** and **Chart.js 4.5.1**.

### ▶ [Live demo](https://erkantaylan.github.io/BlazorChartjs/)

Every chart type, plus zoom, annotations, time axes and callbacks, running in the browser. Source in [`ChartjsDemo/`](https://github.com/erkantaylan/BlazorChartjs/tree/main/ChartjsDemo).

This is a fork of [erossini/BlazorChartjs](https://github.com/erossini/BlazorChartjs) (`PSC.Blazor.Components.Chartjs`) by Enrico Rossini, published independently as `Erkan.Blazor.Chartjs`. It is MIT licensed, same as upstream.

## Installation

```
dotnet add package Erkan.Blazor.Chartjs
```

Then open your `index.html` or `_Host` and add at the end of the page:

```html
<!-- required -->
<script src="_content/Erkan.Blazor.Chartjs/lib/Chart.js/chart.umd.js"></script>

<!-- optional, add only what you use -->
<script src="_content/Erkan.Blazor.Chartjs/lib/moment/moment-with-locales.min.js"></script>
<script src="_content/Erkan.Blazor.Chartjs/lib/chartjs-adapter-moment/chartjs-adapter-moment.min.js"></script>
<script src="_content/Erkan.Blazor.Chartjs/lib/hammer.js/hammer.js"></script>
<script src="_content/Erkan.Blazor.Chartjs/lib/chartjs-plugin-zoom/chartjs-plugin-zoom.js"></script>
<script src="_content/Erkan.Blazor.Chartjs/lib/chartjs-plugin-datalabels/chartjs-plugin-datalabels.js"></script>
<script src="_content/Erkan.Blazor.Chartjs/lib/chartjs-plugin-annotation/chartjs-plugin-annotation.min.js"></script>
```

`moment` must come before the moment adapter, and `chart.umd.js` before every plugin. The interop module (`_content/Erkan.Blazor.Chartjs/Chart.js`) is imported on demand — do **not** add a `<script>` tag for it.

Then, open your `_Imports.razor` and add the following:

```
@using Erkan.Blazor.Chartjs
@using Erkan.Blazor.Chartjs.Enums
@using Erkan.Blazor.Chartjs.Models
@using Erkan.Blazor.Chartjs.Models.Common
@using Erkan.Blazor.Chartjs.Models.Bar
@using Erkan.Blazor.Chartjs.Models.Bubble
@using Erkan.Blazor.Chartjs.Models.Doughnut
@using Erkan.Blazor.Chartjs.Models.Line
@using Erkan.Blazor.Chartjs.Models.Pie
@using Erkan.Blazor.Chartjs.Models.Polar
@using Erkan.Blazor.Chartjs.Models.Radar
@using Erkan.Blazor.Chartjs.Models.Scatter
```

There is a namespace for each chart plus the common namespaces (Enum, Models and the base).

## Quick start
On your page you can create a new chart by adding this code

```
<Chart Config="_config1" @ref="_chart1"></Chart>
```

In the code section you have to define the variables:

```csharp
private BarChartConfig _config1;
private Chart _chart1;
```

Then, you can pass the configuration for the chart into `_config1` (in the example code above). For a bar chart, the configuration is

```csharp
_config1 = new BarChartConfig()
{
    Options = new Options()
    {
        Plugins = new Plugins()
        {
            Legend = new Legend()
            {
                Align = Align.Center,
                Display = false,
                Position = LegendPosition.Right
            }
        },
        Scales = new Dictionary<string, Axis>()
        {
            {
                Scales.XAxisId, new Axis()
                {
                    Stacked = true,
                    Ticks = new Ticks()
                    {
                        MaxRotation = 0,
                        MinRotation = 0
                    }
                }
            },
            {
                Scales.YAxisId, new Axis()
                {
                    Stacked = true
                }
            }
        }
    }
};
```

Then, you have to define the `Labels` and the `Datasets` like that

```csharp
_config1.Data.Labels = new List<string>
    { "January", "February", "March", "April", "May", "June" };

_config1.Data.Datasets.Add(new BarDataset()
{
    Label = "Value",
    Data = new List<decimal?> { 65, 59, 80, 81, 56, 55 },
    BackgroundColor = new List<string>
    {
        "rgba(255, 99, 132, 0.2)", "rgba(255, 159, 64, 0.2)", "rgba(255, 205, 86, 0.2)",
        "rgba(75, 192, 192, 0.2)", "rgba(54, 162, 235, 0.2)", "rgba(153, 102, 255, 0.2)"
    },
    BorderColor = new List<string>
    {
        "rgb(255, 99, 132)", "rgb(255, 159, 64)", "rgb(255, 205, 86)",
        "rgb(75, 192, 192)", "rgb(54, 162, 235)", "rgb(153, 102, 255)"
    },
    BorderWidth = 1
});
```

The dataset type has to match the config: `BarChartConfig.Data.Datasets` is a `List<BarDataset>`, `LineChartConfig`'s is a `List<LineDataset>`, and so on. The base `Dataset` type carries only `Label`, `Data`, `DataLabels`, `Order` and `Type` — colours and widths live on the per-chart subclasses.

The result of the code above is this chart

![image](https://user-images.githubusercontent.com/9497415/196763122-306142fa-e810-47fc-af06-12d4889ab21f.png)

## Implemented charts

For what each type's model actually exposes, and where it stops, see [Feature coverage](https://github.com/erkantaylan/BlazorChartjs/blob/main/docs/feature-coverage.md).

- [x] Bar chart
- [x] Line chart
- [x] Area
- [x] Other charts
  - [x] Scatter
  - [x] Scatter - Multi axis
  - [x] Doughnut
  - [x] Pie
  - [x] Multi Series Pie
  - [x] Polar area
  - [x] Radar
  - [x] Radar skip points
  - [x] Combo bar/line
  - [x] Stacked bar/line

## Documentation

- [Feature coverage](https://github.com/erkantaylan/BlazorChartjs/blob/main/docs/feature-coverage.md) — what Chart.js 4.5.1 offers against what the C# models expose, and the escape hatches where they stop
- [Upgrading](https://github.com/erkantaylan/BlazorChartjs/blob/main/docs/upgrading.md) — from `Erkan.Blazor.Chartjs` 1.0.0, and from upstream `PSC.Blazor.Components.Chartjs`
- [Updating data](https://github.com/erkantaylan/BlazorChartjs/blob/main/docs/updating-data.md) — `AddData`, `AddDataset<T>` and `ClearData` on a chart that has already rendered
- [Callbacks and events](https://github.com/erkantaylan/BlazorChartjs/blob/main/docs/callbacks-and-events.md) — tooltip, tick and legend callbacks, `OnClickAsync`, `OnHoverAsync` and the component's event parameters
- [Styling](https://github.com/erkantaylan/BlazorChartjs/blob/main/docs/styling.md) — legend label styling and the axis border
- [Plugins](https://github.com/erkantaylan/BlazorChartjs/blob/main/docs/plugins.md) — data labels, zoom and pan, and attaching any other Chart.js plugin with `RegisterPlugins`
- [Extra options](https://github.com/erkantaylan/BlazorChartjs/blob/main/docs/extra-options.md) — `ExtraOptions`, for the Chart.js options the models have no property for
- [Changelog](https://github.com/erkantaylan/BlazorChartjs/blob/main/CHANGELOG.md)

## Links
* [Live demo](https://erkantaylan.github.io/BlazorChartjs/) for this fork
* Source code on [GitHub](https://github.com/erkantaylan/BlazorChartjs)
* [NuGet](https://www.nuget.org/packages/Erkan.Blazor.Chartjs/) package
* Upstream project by Enrico Rossini: [erossini/BlazorChartjs](https://github.com/erossini/BlazorChartjs) · [upstream demo site](https://chartjs.puresourcecode.com/) · [upstream docs](https://www.puresourcecode.com/dotnet/blazor/blazor-component-for-chartjs/)

## Contribution

Contributors to the upstream project, whose work this fork inherits:

- [macias](https://github.com/macias) for adding the crosshair line to the components
- [Heitor Eleutério de Rezende](https://github.com/heitoreleuterio) for the migration to NET7 and adding:
    - Legend Labels Filtering
    - Support to Ticks' AutoSkip and Font properties
    - Tooltip Callback Label problem fixed.
    - Ticks callback


## Credits

Original project by [Enrico Rossini](https://github.com/erossini) — [erossini/BlazorChartjs](https://github.com/erossini/BlazorChartjs), documented on [PureSourceCode.com](https://www.puresourcecode.com/dotnet/blazor/blazor-component-for-chartjs/). Nearly all of the component's design and the bulk of its code are his.

This fork is maintained by [erkantaylan](https://github.com/erkantaylan) and released under the same MIT license. It is not affiliated with or endorsed by PureSourceCode; please raise issues with this fork at [erkantaylan/BlazorChartjs](https://github.com/erkantaylan/BlazorChartjs/issues) rather than upstream.

## License

MIT. See [LICENSE](https://github.com/erkantaylan/BlazorChartjs/blob/main/LICENSE).
