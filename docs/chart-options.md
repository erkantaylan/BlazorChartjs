# Chart options

The options that belong to a chart as a whole rather than to a dataset, a scale or a plugin: how big it is, how much room it leaves around itself, and what it does when the pointer moves over it. The chart's own colours and the default palette are in [Styling](styling.md#chart-colours). The **Chart options** page of the demo shows all of it.

## Size and aspect ratio

```csharp
Options = new Options()
{
    Responsive = true,          // follow the container's size (the default here and in Chart.js)
    MaintainAspectRatio = true, // size the height from the width...
    AspectRatio = 3,            // ...at three times as wide as tall
    ResizeDelay = 100,          // wait 100ms after a resize before redrawing
    DevicePixelRatio = 2        // render at 2x whatever the screen is
}
```

**This package starts `MaintainAspectRatio` at `false`**, where Chart.js starts it at `true`. That is what makes the `Height` parameter on `<Chart>` work: the canvas fills the height the container is given instead of computing one from its width. To size a chart by ratio instead, set `MaintainAspectRatio = true` and leave `Height` off the component.

`Responsive` starts at `true`, like Chart.js; `false` stops the canvas following its container. Both properties are `bool?`, and `null` writes no key at all, which leaves the choice to Chart.js's own defaults (`responsive: true`, `maintainAspectRatio: true`).

`AspectRatio` is only read while `MaintainAspectRatio` is `true`; Chart.js defaults it to `2`, and to `1` for pie, doughnut, polar area and radar. `ResizeDelay` debounces resizes, in milliseconds (Chart.js default `0`). `DevicePixelRatio` overrides `window.devicePixelRatio`, for example to print a sharper chart. The `onResize` callback has no property.

## Layout padding

```csharp
Options = new Options()
{
    Layout = new Layout()
    {
        Padding = new Padding(24),  // every side; or new Padding(top, right, bottom, left)
        AutoPadding = false
    }
}
```

`Padding` is the space between the edge of the canvas and everything drawn on it, legend and title included. `AutoPadding` is Chart.js's own extra padding, on by default, which keeps elements drawn on the edge of the chart area — a point on the last tick, say — from being clipped; `false` leaves only the padding you set.

## Hover and events

`Options.Hover` takes the same `Interaction` as `Options.Interaction`, and applies it to **hover** only: which elements get their hover style. Anything `Hover` leaves unset falls back to `Interaction`. The tooltip does not read `Hover` — it follows `Interaction`. So this highlights every bar of a month while the tooltip still describes the single bar under the pointer:

```csharp
Options = new Options()
{
    Interaction = new Interaction() { Mode = InteractionMode.Nearest, Intersect = true },
    Hover = new Interaction() { Mode = InteractionMode.Index, Intersect = false }
}
```

`Options.Events` is the list of DOM events Chart.js listens to. Leave it unset for Chart.js's `mousemove`, `mouseout`, `click`, `touchstart` and `touchmove`, or narrow it:

```csharp
Options = new Options()
{
    Events = ["click"]   // no hover effects; the tooltip appears on click
}
```

An empty list is written as `[]` and is not the same as unset: the chart then reacts to nothing.

The package's own chart callbacks ride on Chart.js's event handling, so the list decides when they fire:

- `OnHoverAsync` and `OnChartOver` are called for **every** event the chart listens to inside the chart area — without `mousemove` they stop following the pointer, and fire only on the events that are left.
- `OnClickAsync`, `OnChartClick`, `Legend.OnClickAsync` and `OnLegendClick` need `click` (or `mouseup`) in the list.
- The crosshair and `OnMouseOutAsync` are ordinary DOM listeners of this package, not Chart.js events, and `Events` does not affect them.
