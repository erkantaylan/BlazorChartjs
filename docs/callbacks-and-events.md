# Callbacks and events

## Callbacks

The component has a few callbacks (more in development) to customize your chart. The callbacks are ready to use are:

- Tooltip
  * Labels
  * Titles
- Axis ticks — `Ticks.Callback` and `Ticks.CallbackAsync`
- Legend entries — `LegendLabels.Filter`

The demo page at `/ticksfilter` ([`ChartjsDemo/Pages/TicksFilterPage.razor`](../ChartjsDemo/Pages/TicksFilterPage.razor)) exercises all of them on one chart, together with live data updates, and shows how often each one is actually asked.

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
}
```

`Label` and `Title` are `Func<CallbackGenericContext, string[]>`. `CallbackGenericContext` carries `DatasetIndex` and `DataIndex` as `int` and `Value` as `decimal` — `Value` keeps its fractional part now; upstream cast it to `int`, so `12.5` arrived as `12`.

The upstream author, Enrico Rossini, writes about the background to these callbacks on [PureSourceCode.com](https://www.puresourcecode.com/dotnet/blazor/custom-javascript-function-in-blazor/).

> **Blazor Server and SSR:** these callbacks used to be invoked through synchronous JS→.NET interop, which is only supported on WebAssembly and threw everywhere else. They are async now. As a consequence the tooltip paints Chart.js's own default text on the very first frame and swaps in your value once .NET replies — a frame later, and cached from then on.

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

The function `hoverAsync` receives as parameter a `HoverContext` that contains the 2 values: `DataX` and `DataY` as decimal. They are read from the scales named `x` and `y`; a chart that has no such scale — pie, doughnut, polar area and radar, or a cartesian chart whose axes are named something else — reports `0` for the missing one instead of throwing on every mouse move.

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

## Component event callbacks

`OnClickAsync` and `OnHoverAsync` above are set on the chart *configuration*. The same three events are also available as normal Blazor parameters on the component itself, which is usually more convenient because the handler can be a method on the page and Blazor re-renders for you:

```razor
<Chart Config="_config1" @ref="_chart1"
       OnChartClick="OnClick"
       OnChartOver="OnOver"
       OnLegendClick="OnLegend" />

@code {
    private void OnClick(CallbackGenericContext ctx) { /* DatasetIndex, DataIndex, Value */ }
    private void OnOver(HoverContext ctx)            { /* DataX, DataY */ }
    private void OnLegend(LegendClickContext ctx)    { /* LegendIndex, LegendText */ }
}
```

Both styles can be used at once: the component callback runs first, then the one on `Options`.

`LegendClickContext.LegendIndex` is the index the clicked entry stands for: the **dataset** index on charts whose legend has one entry per dataset (bar, line, scatter, bubble …), and the **data** index on pie, doughnut and polar-area charts, whose legend has one entry per slice. `OnChartOver` carries the same `HoverContext` as `OnHoverAsync`, so the same caveat applies — on a chart with no `x`/`y` scale (pie, doughnut, polar area, radar) it reports `0` for both axis values rather than throwing.

> These three parameters existed upstream but were never wired to anything, so nothing ever invoked them. They fire now.
>
> `OnLegendClick` no longer suppresses Chart.js's own legend behaviour either — clicking a legend entry still toggles its dataset (upstream [#89](https://github.com/erossini/BlazorChartjs/issues/89)). The override is only installed when a handler is actually registered.
