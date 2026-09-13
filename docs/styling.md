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

`Tooltip` takes the same treatment for the same reason: `BackgroundColor`, `TitleColor`/`TitleFont`, `BodyColor`/`BodyFont`, `FooterColor`/`FooterFont`, `BorderColor`, `BorderWidth` and `MultiKeyBackground` — and its placement and layout, below in [Tooltip](#tooltip).

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

## Tooltip

Beyond its colours and fonts, `Tooltip` sets what the tooltip describes, where it is drawn, and how its box is laid out ([#6](https://github.com/erkantaylan/BlazorChartjs/issues/6), and the position half of upstream [#46](https://github.com/erossini/BlazorChartjs/issues/46)):

```csharp
Plugins = new Plugins()
{
    Tooltip = new Tooltip()
    {
        // what it describes
        Mode = InteractionMode.Index,     // every series at the hovered index
        Intersect = false,                // wherever the pointer is, not only over a point

        // where it goes
        Position = TooltipPosition.Nearest,
        XAlign = TooltipXAlign.Center,
        YAlign = TooltipYAlign.Bottom,    // caret at the bottom: the tooltip sits above the point

        // the box
        Padding = new Padding(12),
        CornerRadius = 12,
        CaretSize = 10,
        CaretPadding = 8,

        // the text
        TitleAlign = TextAlign.Center,
        TitleMarginBottom = 10,
        BodySpacing = 6,

        // the colour box beside each item
        UsePointStyle = true,
        BoxWidth = 10,
        BoxHeight = 10,
        BoxPadding = 6
    }
}
```

As with the legend, every property is optional and a `null` one is not serialized, so Chart.js keeps its default; each property's XML documentation names that default.

- **`Enabled = false`** draws no tooltip at all, and **`DisplayColors = false`** leaves out the colour boxes. Both are written as `false`, not dropped.
- **`Mode`, `Intersect`, `Axis` and `IncludeInvisible`** are the four [`Interaction`](chart-options.md#hover-and-events) settings, applied to the tooltip alone. Each one the tooltip leaves unset falls back to `Options.Interaction`, never to `Options.Hover`, so a tooltip can list a whole month while hovering still highlights only the point under the pointer.
- **`Position`** is `TooltipPosition.Average` (Chart.js's default: the middle of every item shown) or `TooltipPosition.Nearest` (the item nearest the pointer). `PositionString` carries the name of a positioner of your own, which has to be registered in page script, after `chart.umd.js` and before the chart is created — Chart.js looks the name up on the first hover and throws if nothing is registered under it:

  ```html
  <script>
      Chart.Tooltip.positioners.cursor = function (items, eventPosition) {
          return { x: eventPosition.x, y: eventPosition.y };
      };
  </script>
  ```

  ```csharp
  Tooltip = new Tooltip() { PositionString = "cursor" }
  ```

- **`XAlign` and `YAlign` name the side the caret is on, not the side of the point the tooltip is on.** `YAlign = TooltipYAlign.Top` puts the caret on top, so the tooltip is drawn *below* the point; `XAlign = TooltipXAlign.Left` draws it to the right. Left unset, Chart.js picks the sides that keep the tooltip on the canvas; set, they are used even at the edge, where Chart.js pushes the box back inside the canvas.
- **`Padding`** is the same four-sided `Padding` as `Layout.Padding`: `new Padding(12)` pads every side, `new Padding(top, right, bottom, left)` each one. **`CornerRadius`** is one radius for all four corners; for Chart.js's per-corner form, leave it `null` and write `ExtraOptions["cornerRadius"] = new { topLeft = 0, topRight = 12, bottomRight = 12, bottomLeft = 12 }` (see [Extra options](extra-options.md)).
- **`TitleAlign`, `BodyAlign` and `FooterAlign`** align each block of text inside the box, with `TextAlign.Left` (the default), `Center` or `Right`. `TitleSpacing`, `BodySpacing` and `FooterSpacing` add space above and below every line of that block, and `TitleMarginBottom` and `FooterMarginTop` separate the blocks.
- **`BoxWidth` and `BoxHeight`** default to the body font size. **`UsePointStyle`** swaps the square for each dataset's own point style, sized to the smaller of the two.
- **`RTL`** and **`TextDirection`** work as they do on `Legend`.

Every `TooltipPosition`, `TooltipXAlign`, `TooltipYAlign` and `TextAlign` property mirrors into a `*String` twin for a raw value, and assigning `null` clears both.

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
