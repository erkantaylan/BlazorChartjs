# Styling

## Legend label styling

Legend text is painted on the canvas, so no stylesheet can reach it — an app whose colours come from design tokens has to hand those colours to Chart.js. `Legend.Labels` covers the whole `legend.labels` object for that ([#1](https://github.com/erkantaylan/BlazorChartjs/issues/1), upstream [#90](https://github.com/erossini/BlazorChartjs/issues/90) and [#55](https://github.com/erossini/BlazorChartjs/issues/55)):

```csharp
Plugins = new Plugins()
{
    Legend = new Legend()
    {
        Position = LegendPosition.Bottom,
        Labels = new LegendLabels()
        {
            Color = theme.TextColor,   // "#111827" in light, "#e5e7eb" in dark
            Font = new Font()
            {
                Family = "Inter, system-ui, sans-serif",
                Size = 13,
                Weight = "500"
            },
            UsePointStyle = true,
            PointStyle = PointStyle.Circle,
            BoxHeight = 8,
            Padding = 16
        }
    }
}
```

Every one of these is optional: leave a property `null` and it is not serialized at all, so Chart.js keeps its own default. For `Color` that default is `Options.Color` if the chart sets one (see [Chart colours](#chart-colours)) and the page-wide `#666` otherwise; for `Font` it is always the page-wide 12px Helvetica, because there is no per-chart font that reaches the legend. `PointStyle` mirrors into `PointStyleString` for a raw value the enumeration does not carry, the same pattern as `Legend.Align` / `AlignString`.

`Tooltip` takes the same treatment for the same reason: `BackgroundColor`, `TitleColor`/`TitleFont`, `BodyColor`/`BodyFont`, `FooterColor`/`FooterFont`, `BorderColor` and `BorderWidth`.

Chart.js reads these when the chart is built, and the `<Chart>` component compares `Config` **by reference** — so mutating `Labels.Color` on the config it is already holding changes nothing on screen. Switching theme at runtime means handing it a new config object:

```razor
<Chart Config="_config" />

@code {
    private IChartConfig _config = default!;

    protected override void OnInitialized() => _config = BuildConfig(Theme.Current);

    // Call this when the app switches light/dark. BuildConfig returns a *new*
    // config object carrying the legend block above, built from the new tokens.
    private void ThemeChanged() => _config = BuildConfig(Theme.Current);
}
```

The component destroys the previous Chart.js instance and creates the new one for you.

## Chart colours

`Options` carries the three colours Chart.js lets a chart set for itself:

```csharp
Options = new Options()
{
    Color = "#1e3a8a",                          // legend label text
    BackgroundColor = "rgba(71, 85, 105, 0.25)", // bars, line areas, points, arcs
    BorderColor = "#475569"                      // their borders, and lines
}
```

They reach less than their names suggest, and the reason is Chart.js, not this package:

- **`BackgroundColor` and `BorderColor`** are what every bar, line, point and arc falls back to when neither its dataset nor `Options.Elements` sets a colour. Grid lines and the axis border do not use them — those have `Grid.Color` and `Border.Color`. Setting either one also switches the [default colour palette](#default-colour-palette) off.
- **`Color`** is the fallback for **legend label text only**. The title, tick labels, axis titles and datalabels fall back to the page-wide `Chart.defaults.color` (`#666`) instead, and the tooltip keeps its own white text, so they need their own `Title.Color`, `Ticks.Color`, `AxesTitle.Color`, `DataLabels.Color` and `Tooltip` colours.

There is **no chart-wide font**. Chart.js 4.5.1 reads a per-chart `options.font` only for radial-scale point labels, which no chart built on `Options` can show, so the package has no `Options.Font` rather than one that changes nothing. Every text element has a `Font` of its own — `LegendLabels.Font`, `Title.Font`, `Ticks.Font`, `AxesTitle.Font`, `Tooltip.TitleFont`/`BodyFont`/`FooterFont`, `DataLabels.Font` — and a theme sets each of them. The **Chart options** page of the demo does exactly that.

## Default colour palette

Chart.js 4 ships a `colors` plugin, enabled by default, that paints datasets from a seven-colour palette. `Plugins.Colors` configures it:

```csharp
Plugins = new Plugins()
{
    Colors = new Colors()
    {
        Enabled = true,        // false: uncoloured datasets stay grey, rgba(0,0,0,0.1)
        ForceOverride = false  // true: the palette replaces every colour a dataset sets
    }
}
```

The plugin is **all or nothing**. Unless `ForceOverride` is `true`, it colours no dataset at all as soon as *any* dataset sets a `BackgroundColor` or `BorderColor`, or `Options.Elements`, `Options.BackgroundColor` or `Options.BorderColor` sets one. It never fills in just the datasets you left uncoloured — colour all of them, or none.

This replaces the `Autocolors` class, which modelled `chartjs-plugin-autocolors`. That plugin is still vendored under `lib/`, but this package never registered it, and the built-in plugin covers what it did.

## Line datasets

Series in similar colours cannot be told apart without dashes, a series with missing values breaks wherever a value is `null`, and a point that matters has to stand out from the ones around it. `LineDataset` covers the line and point styling Chart.js reads on a line dataset ([#8](https://github.com/erkantaylan/BlazorChartjs/issues/8), upstream [#95](https://github.com/erossini/BlazorChartjs/issues/95)):

```csharp
using Erkan.Blazor.Chartjs.Models.Common.StringEnums; // StepMode lives here, unlike the other string enums

_config.Data.Datasets.Add(new LineDataset()
{
    Label = "Forecast",
    Data = forecast,
    BorderColor = "#9fb3c8",
    BorderWidth = 3,
    BorderDash = new List<decimal> { 6, 4 },    // 6px dash, 4px gap
    BorderDashOffset = 0,
    BorderCapStyle = BorderCapStyle.Round,
    BorderJoinStyle = BorderJoinStyle.Round,
    SpanGaps = true,                            // draw across null values
    StepMode = StepMode.Middle
});

_config.Data.Datasets.Add(new LineDataset()
{
    Label = "Actual",
    Data = actual,
    BorderColor = "#334e68",
    // one colour per point
    PointBackgroundColor = actual.Select(v => v < 60 ? "#e12d39" : "#199473").ToList(),
    // one entry colours every point
    PointBorderColor = new List<string> { "#ffffff" },
    PointRadius = 6,
    PointHitRadius = 10,
    PointHoverRadius = 9,
    PointHoverBorderWidth = 3,
    XAxisId = Scales.XAxisId
});
```

- **Dashes.** `BorderDash` holds dash and gap lengths in pixels, alternating, the way the canvas `setLineDash` reads them. It is `List<decimal>`, so fractions work. A one-pixel dash with `BorderCapStyle.Round` draws a dotted line. `BorderCapStyle` is `Butt` (the default), `Round` or `Square`, and `BorderJoinStyle` is `Miter` (the default), `Bevel` or `Round`. Each one mirrors into a `*String` twin, such as `BorderCapStyleString`, which takes a raw value the class does not carry.
- **Point colours are lists.** `PointBackgroundColor`, `PointBorderColor`, `PointHoverBackgroundColor` and `PointHoverBorderColor` are `List<string>?`. Chart.js picks the entry at `index % count`, so a list as long as the data colours every point separately, and a single entry colours them all. The numeric point options — `PointRadius`, `PointRotation`, `PointBorderWidth`, `PointHitRadius`, `PointHoverRadius` and `PointHoverBorderWidth` — take one `int?` for the whole dataset. `HoverBackgroundColor`, `HoverBorderColor` and `HoverBorderWidth` are what Chart.js falls back to when the matching `PointHover*` option is unset.
- **Gaps.** `SpanGaps = true` joins the points on either side of a `null`. `SpanGaps = false` breaks the line there, even when the chart as a whole spans gaps. Chart.js also accepts a number, the largest gap to span, but that form is not exposed. `ShowLine = false` draws the points without the line.
- **Stepped lines.** `StepMode.False` and `StepMode.True` go out as the JSON booleans `false` and `true`, and `Before`, `After` and `Middle` as strings. Up to 2.0.0, `StepMode.False` was sent as the string `"false"`. Chart.js only checks whether `stepped` is truthy, and a non-empty string is, so the line was drawn stepped. Any chart that set it to turn stepping off has been showing steps.
- **Placement.** `XAxisId` binds the dataset to a named x axis in `Options.Scales`, the way `YAxisId` binds it to a y axis. Datasets with the same `Stack` are stacked together when the value axis has `Stacked = true`. `Clip` is a number of pixels on every side that the dataset may draw beyond the chart area, so points on the edge are not cut in half. Chart.js's `false` and per-side object forms of `clip` are not exposed. `DrawActiveElementsOnTop = false` keeps a hovered point in data order instead of raising it above the others.

As everywhere else, an unset property is not serialized and Chart.js keeps its own default. The *Line Styling* and *Step Line* pages of the [live demo](https://erkantaylan.github.io/BlazorChartjs/) show dashes, gaps, per-point colours and the four step modes.

## Bar datasets

Rounded corners, thin bars, a bar drawn over another rather than beside it, and a dataset pinned to a named axis. `BarDataset` covers the options Chart.js reads on a bar dataset ([#7](https://github.com/erkantaylan/BlazorChartjs/issues/7)):

```csharp
_config.Options.Scales = new Dictionary<string, Axis>()
{
    { "months", new Axis() { Position = Position.Bottom } },
    { "revenue", new Axis() { Position = Position.Left, BeginAtZero = true } }
};

_config.Data.Datasets.Add(new BarDataset()
{
    Label = "Actual",
    Data = actual,
    BarThickness = 14,                          // pixels; or BarThickness.Flex
    BorderRadius = 7,                           // one radius for every corner
    BorderSkipped = BorderSkipped.False,        // no skipped edge, so all four corners round
    Grouped = false,                            // drawn over the other datasets, not beside them
    XAxisId = "months",
    YAxisId = "revenue",
    Order = 1
});

_config.Data.Datasets.Add(new BarDataset()
{
    Label = "Target",
    Data = target,
    BorderRadius = new BorderRadius { TopLeft = 6, TopRight = 6 },
    Grouped = false,
    CategoryPercentage = 0.6m,
    BarPercentage = 1,
    XAxisId = "months",
    YAxisId = "revenue",
    Order = 2
});
```

- **Width.** By default Chart.js makes each category `CategoryPercentage` (0.8) of the room it has, and each bar `BarPercentage` (0.9) of its slot in that. `BarThickness = 14` fixes the width in pixels and ignores both percentages, `MaxBarThickness` caps the width however much room there is, and `BarThickness.Flex` sizes each category from the distance to its neighbours, which suits an unevenly spaced axis. Chart.js 4.5.1 draws nothing for `Flex` on a dataset with `Grouped = false`. `MinBarLength` keeps a bar for a value close to zero at least that many pixels long.
- **Rounded corners.** `BorderRadius = 8` is one radius, sent as a number. `new BorderRadius { TopLeft = 8, TopRight = 8 }` rounds only the corners it names and is sent as an object; a corner it leaves out stays square. `new BorderRadius(8) { BottomLeft = 0 }` starts from one radius and overrides a corner. A corner on the edge `BorderSkipped` skips is never rounded, and that is the edge the bar grows from unless you say otherwise. So `BorderRadius = 8` on its own rounds the two corners away from the axis, and `BorderSkipped = BorderSkipped.False` rounds all four. The two forms differ on a stacked axis: Chart.js applies a single radius only to the outermost bar on each side of zero, and corners to every bar. That is why `new BorderRadius(8, 8, 8, 8)` is still sent as an object. `HoverBorderRadius` takes the same forms.
- **Skipped edges.** `BorderSkipped` is `Start` (the default), `End`, `Middle`, `Bottom`, `Left`, `Top`, `Right`, `False` or `True`. `Middle` drops the border wherever stacked bars meet, so a stack is outlined as one, and lets a single radius round both ends of it. Chart.js 4.5.1 recognises the bottom of a stack of positive values only when it is the first dataset, so a stack group that starts with a later dataset loses its base border too. `False` and `True` are sent as the JSON booleans Chart.js tests for, the same way `StepMode` is: `False` borders every edge, and `True` borders none and rounds no corners. `BorderSkippedString` takes a raw value.
- **Grouping.** `Grouped = false` takes the dataset out of the group. Its bars are centred on the category, at the width one bar would have alone there, and overlap the other datasets' bars. `Order` decides which is on top: a lower `Order` is drawn later, so give the bar that belongs in front the lower one. `SkipNull = true` leaves no gap for a `null` value, so the other datasets' bars in that category share the room. Each dataset reads its own `SkipNull` when it places its bars, so set it on every dataset in the group; on the dataset with the `null`s alone it does nothing.
- **Placement.** `XAxisId` and `YAxisId` bind the dataset to named axes in `Options.Scales`. `IndexAxis = Axes.Y` lays this one dataset along the y axis, whatever `Options.IndexAxis` says. Chart.js builds the default `x` and `y` scales from the first dataset that uses them, so beside vertical datasets a horizontal one needs axes of its own, named with `XAxisId` and `YAxisId`. `Base` starts every bar at a value other than zero, so `Base = 50` draws a bar from 50 up or down to its value. `InflateAmount` is how many pixels each bar is drawn beyond its edges. The default, `InflateAmount.Auto`, is 0.33 pixels when `BarThickness` is set (a width or `Flex`) or both percentages are `1`, which hides the hairline gap between bars that touch, and nothing otherwise. `InflateAmount = 0` draws every bar at its exact size; a fraction is a `decimal`, `0.5m`.

`BarDataset.Fill` is gone. `fill` is a line and radar option, and a bar has nothing to fill. The *Bar Styling* page of the [live demo](https://erkantaylan.github.io/BlazorChartjs/) shows thin, rounded and ungrouped bars on named axes, a stack outlined as one, and `SkipNull`.

## Axis border

Chart.js 4 moved the axis border out of `grid` into a scale option of its own, so `Grid.DrawBorder` is gone. Use `Axis.Border`:

```csharp
Scales = new Dictionary<string, Axis>()
{
    {
        Scales.YAxisId, new Axis()
        {
            Border = new Border()
            {
                Display = true,
                Color = "#888",
                Width = 2,
                Dash = new List<int> { 4, 4 },
                DashOffset = 0,
                Z = 1
            }
        }
    }
}
```
