# Erkan.Blazor.Chartjs

A [Chart.js](https://www.chartjs.org/) wrapper for [Blazor WebAssembly](https://learn.microsoft.com/aspnet/core/blazor/hosting-models) and Blazor Server, targeting **.NET 10** and **Chart.js 4.5.1**.

This is a fork of [erossini/BlazorChartjs](https://github.com/erossini/BlazorChartjs) (`PSC.Blazor.Components.Chartjs`) by Enrico Rossini, published independently as `Erkan.Blazor.Chartjs`. It is MIT licensed, same as upstream.

> **Migrating from `PSC.Blazor.Components.Chartjs`:** the package ID, assembly, and root namespace all changed. Replace `PSC.Blazor.Components.Chartjs` with `Erkan.Blazor.Chartjs` in your `_Imports.razor` and in the `_content/...` script paths in `index.html` / `_Host.cshtml`.

## Fork changes

### .NET 10 + Chart.js 4.x
- `TargetFramework` moved from `net8.0` to `net10.0`
- All Microsoft NuGet packages on 10.0.x
- Chart.js **3.9.1 → 4.5.1** (UMD build: `chart.umd.js`)
- chartjs-plugin-zoom **1.2.1 → 2.2.0**, chartjs-plugin-autocolors **0.2.2 → 0.3.1**
- Bundled chartjs-plugin-annotation **3.1.0** and moment **2.30.1** (needed by the time-axis locale support)
- Removed the stale Chart.js 3.x module chunks and type definitions

### New features
- `Axis.Time` — time-axis configuration (`unit`, `displayFormats`, `tooltipFormat`, `round`, `minUnit`)
- `Plugins.Annotation` — [chartjs-plugin-annotation](https://www.chartjs.org/chartjs-plugin-annotation/latest/guide/) support for lines, boxes, points, and labels. The plugin is registered automatically when a chart declares annotations.
- `Options.Locale` — BCP 47 tag (e.g. `tr-TR`) that propagates to Chart.js and to the moment date adapter, so time-axis labels format in the given locale

### Fixes
- Tick values are rounded to 10 decimal places after `afterBuildTicks`, so the zoom plugin no longer produces axis labels like `1.42e-14` or `100.00000000001`
- The async ticks callback (`Ticks.CallbackAsync`) now actually renders. Chart.js tick callbacks are synchronous, so returning a Promise previously rendered `[object Promise]`; labels are now resolved out-of-band and applied on a follow-up `update('none')`
- `registerDataLabels` was compared instead of deleted (`==` where `delete` was meant), leaving the internal flag in the serialized options
- `legend.onClick` is guarded — charts configured with `Plugins = null` no longer throw a `TypeError` during setup
- The demo host page loaded `Chart.js` (an ES module) as a classic script, throwing `Unexpected token 'export'` on every page load. The module is imported by the interop layer; the stray tag is gone.
- moment and `chartjs-adapter-moment` are now actually shipped and loaded, so `Options.Locale` and time axes work at runtime

> **Note on `chartjs-adapter-moment`:** the bundled copy is hand-patched to apply a per-instance locale in `format()`, so it is deliberately excluded from `libman.json`. Do not overwrite it with a LibMan restore without re-applying that patch.

## Links
* Source code on [GitHub](https://github.com/erkantaylan/BlazorChartjs)
* [NuGet](https://www.nuget.org/packages/Erkan.Blazor.Chartjs/) package
* Upstream project: [erossini/BlazorChartjs](https://github.com/erossini/BlazorChartjs) · [demo site](https://chartjs.puresourcecode.com/) · [docs](https://www.puresourcecode.com/dotnet/blazor/blazor-component-for-chartjs/)

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

## Add a new chart
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
                Align = LegendAlign.Center,
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
_config1.Data.Labels = BarDataExamples.SimpleBarText;
_config1.Data.Datasets.Add(new Dataset()
{
    Label = "Value",
    Data = BarDataExamples.SimpleBar.Select(l => l.Value).ToList(),
    BackgroundColor = Colors.Palette1,
    BorderColor = Colors.PaletteBorder1,
    BorderWidth = 1
});
```

The result of the code above is this chart

![image](https://user-images.githubusercontent.com/9497415/196763122-306142fa-e810-47fc-af06-12d4889ab21f.png)

## Implemented charts
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

## Add new values

When a graph is created, it means that the configuration is already defined and the datasets are passed to the chart engine. Without recreating the graph, it is possible to add a new value to a specific dataset and/or add a completely new dataset to the graph.

On your page, create a new chart by adding this code

```
<Chart Config="_config1" @ref="_chart1"></Chart>
```

In the code section you have to define the variables:

```csharp
private LineChartConfig _config1;
private Chart _chart1;
```

_chart1_ is the reference to the `Chart` component and from it, you can access all the functions and properties the component has to offer.

### Add a new value

In an existing graph, it is possible to add a single new value to a specific dataset calling `AddData` function that is available on the chart.

Now, the function `AddData` allows to add a new value in a specific existing dataset. The definition of `AddData` is the following

```csharp
AddData(List<string> labels, int datasetIndex, List<decimal?> data)
```

For example, using __chart1_, the following code adds a new label `Test1` to the list of labels, and for the dataset _0_ adds a random number.

```csharp
_chart1.AddData(new List<string?>() { "Test1" }, 0, new List<decimal?>() { rd.Next(0, 200) });
```

The result is visible in the following screenshot.

![chart-addnewdata](https://user-images.githubusercontent.com/9497415/229902251-8a2adf61-b37c-4fdc-a869-ca8eb1a7cd81.gif)

### Add a new dataset

It is also possible to add a completely new dataset to the graph. For that, there is the function `AddDataset`. This function requires a new dataset of the same format as the others already existing in the chart.

For example, this code adds a new dataset using `LineDataset` using some of the properties this dataset has.

```csharp
private void AddNewDataset()
{
    Random rd = new Random();
    List<decimal?> addDS = new List<decimal?>();
    for (int i = 0; i < 8; i++)
    {
        addDS.Add(rd.Next(i, 200));
    }

    var color = String.Format("#{0:X6}", rd.Next(0x1000000));

    _chart1.AddDataset<LineDataset>(new LineDataset()
        {
            Label = $"Dataset {DateTime.Now}",
            Data = addDS,
            BorderColor = color,
            Fill = false
        });
}
```

The result of this code is the following screenshot.

![chart-addnewdataset](https://user-images.githubusercontent.com/9497415/229904537-22805b25-747f-4020-9eed-51533183324c.gif)

## Callbacks

The component has a few callbacks (more in development) to customize your chart. The callbacks are ready to use are:

- Tooltip
  * Labels
  * Titles

### How to use it

In the configuration of the chart in your Blazor page, you can add your custom code for each callback. 
For an example, see the following code.

```csharp
protected override async Task OnInitializedAsync()
{
    _config1 = new BarChartConfig()
        {
            Options = new Options()
            {
                Responsive = true,
                MaintainAspectRatio = false,
                Plugins = new Plugins()
                {
                    Legend = new Legend()
                    {
                        Align = Align.Center,
                        Display = true,
                        Position = LegendPosition.Right
                    },
                    Tooltip = new Tooltip()
                    {
                        Callbacks = new Callbacks()
                        {
                            Label = (ctx) =>
                            {
                                return new[] { 
                                    $"DataIndex: {ctx.DataIndex}\nDatasetIndex: {ctx.DatasetIndex}" };
                            },
                            Title = (ctx) =>
                            {
                                return new[] { $"This is the value {ctx.Value}" };
                            }
                        }
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

For more info, please see my posts on [PureSourceCode.com](https://www.puresourcecode.com/dotnet/blazor/custom-javascript-function-in-blazor/).

## Add labels to the chart

I added the `chartjs-plugin-datalabels` plugin in the component. This plugin shows the labels for each point in each graph. For more details about this plugin, visit its [website](https://chartjs-plugin-datalabels.netlify.app/).

![image](https://user-images.githubusercontent.com/9497415/224721251-da6959de-2b20-4d42-926b-b036de6695ee.png)

First, in the _index.html_, we have to add after the `chart.js` script, another script for this component. It is important to add the script for `chartjs-plugin-datalabels` after `chart.js`. If the order is different, the plugin could not work. For example

```
<script src="_content/Erkan.Blazor.Chartjs/lib/Chart.js/chart.js"></script>
<script src="_content/Erkan.Blazor.Chartjs/lib/hammer.js/hammer.js"></script>
<script src="_content/Erkan.Blazor.Chartjs/lib/chartjs-plugin-zoom/chartjs-plugin-zoom.js"></script>
<script src="_content/Erkan.Blazor.Chartjs/lib/chartjs-plugin-datalabels/chartjs-plugin-datalabels.js"></script>
```

In the code, you have to change the property `RegisterDataLabels` under `Options` to `true`. That asks to the component to register the library if the library is added to the page and there is data to show. For example, if I define a `LineChartConfig` the code is

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

With this code, the component will register the library in `chart.js`. It is possible to define a `DataLabels` for the entire chart. Also, each dataset can have its own `DataLabels` that rewrites the common settings.

## OnClickAsync

When a user click on a point on the chart with a value, it is possible to add in the chart configuration a specific function to receive the data for that point ad in particular the index of the dataset, the index of the value in the dataset and the value.

```
<Chart Config="_config1" @ref="_chart1" Height="400px"></Chart>
```

In the configuration, under `Options`, there is `OnClickAsync`. Here, specified the function that has to receive the values (in this case `clickAsync`).

```csharp
_config1 = new LineChartConfig()
    {
        Options = new Options()
        {
            OnClickAsync = clickAsync,
            ...
        }
    }
```

The function `clickAsync` receives as a parameter a `CallbackGenericContext` that contains the 3 values: `DatasetIndex` and `DataIndex` as int and the `Value` as decimal.

In the following example, the function changes the string `ClickString` using `values`.

```csharp
public ValueTask clickAsync(CallbackGenericContext value)
{
    ClickString = $"Dataset index: {value.DatasetIndex} - " +
                    $"Value index: {value.DataIndex} - " + 
                    $"Value: {value.Value}";
    StateHasChanged();

    return ValueTask.CompletedTask;
}
```

With this code, if the user clicks on a point, the function writes the values on the page.

![image](https://user-images.githubusercontent.com/9497415/225041631-805cf3c6-4b3f-4475-b57e-2a1962472c35.png)

## OnHoverAsync

This function returns the position of the cursor on the chart. Define a new chart as usual.

```
<Chart Config="_config1" @ref="_chart1" Height="400px"></Chart>
```

In the configuration, under `Options`, there is `OnHoverAsync`. This provides the position of the cursor on the chart.

```csharp
_config1 = new LineChartConfig()
    {
        Options = new Options()
        {
            OnHoverAsync = hoverAsync,
            ...
        }
    }
```

The function `hoverAsync` receives as parameter a `HoverContext` that contains the 2 values: `DataX` and `DataY` as decimal.

In the following example, the function changes the string `HoverString` using `values`.

```csharp
private ValueTask hoverAsync(HoverContext ctx)
{
    HoverString = $"X: {ctx.DataX} - Y: {ctx.DataY}";
    StateHasChanged();

    return ValueTask.CompletedTask;
}
```

With this code, if the user moves the mouse on the chart, the function writes the values in the page.

![chart-hover](https://user-images.githubusercontent.com/9497415/229874627-e720d5dc-bae2-4cfa-8dcc-55ddc58ef4f9.gif)

## Contribution

- [macias](https://github.com/macias) for adding the crosshair line to the components
- [Heitor Eleutério de Rezende](https://github.com/heitoreleuterio) for the migration to NET7 and adding:
    - Legend Labels Filtering
    - Support to Ticks' AutoSkip and Font properties
    - Tooltip Callback Label problem fixed.
    - Ticks callback


## Credits

Original project by [Enrico Rossini](https://github.com/erossini) — [PureSourceCode.com](https://www.puresourcecode.com/dotnet/blazor/blazor-component-for-chartjs/).
This fork is maintained by [erkantaylan](https://github.com/erkantaylan) and released under the same MIT license.

## License

MIT. See [LICENSE](LICENSE).
