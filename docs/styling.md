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

Every one of these is optional: leave a property `null` and it is not serialized at all, so Chart.js keeps its own default. For `Color` and `Font` that default is the library-wide `#666` at 12px — `options.color` and `options.font` have no property in this package either, so a themed legend has to be set here. `PointStyle` mirrors into `PointStyleString` for a raw value the enumeration does not carry, the same pattern as `Legend.Align` / `AlignString`.

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
