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
