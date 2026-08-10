# Erkan.Blazor.Chartjs

[![NuGet](https://img.shields.io/nuget/v/Erkan.Blazor.Chartjs.svg)](https://www.nuget.org/packages/Erkan.Blazor.Chartjs/)
[![NuGet downloads](https://img.shields.io/nuget/dt/Erkan.Blazor.Chartjs.svg)](https://www.nuget.org/packages/Erkan.Blazor.Chartjs/)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

A [Chart.js](https://www.chartjs.org/) wrapper for [Blazor WebAssembly](https://learn.microsoft.com/aspnet/core/blazor/hosting-models) and Blazor Server, targeting **.NET 10** and **Chart.js 4.5.1**.

### ▶ [Live demo](https://erkantaylan.github.io/BlazorChartjs/)

Every chart type, plus zoom, annotations, time axes and callbacks, running in the browser. Source in [`ChartjsDemo/`](ChartjsDemo).

This is a fork of [erossini/BlazorChartjs](https://github.com/erossini/BlazorChartjs) (`PSC.Blazor.Components.Chartjs`) by Enrico Rossini, published independently as `Erkan.Blazor.Chartjs`. It is MIT licensed, same as upstream.

## Feature coverage

What Chart.js **4.5.1** can do, against what the C# models in **2.0.0** actually let you set. Assessed by reading the vendored `chart.js` 4.5.1 bundle and the bundled plugins alongside `src/Models/**`, and by checking that each property serializes to the key Chart.js reads — a property that exists but lands where Chart.js never looks is listed as missing, not as supported.

| Status | Meaning |
| --- | --- |
| **Full** | Everything Chart.js offers for that option is reachable from C# |
| **Partial** | Reachable, with the gaps named in the note |
| **None** | No C# property. Not reachable through the models |

Coverage is thin in places, and the table says so. A **None** row means there is no property today, not that one is planned. If you land on one, see [Escape hatches](#escape-hatches).

### Chart types

| Type | Status | Notes |
| --- | --- | --- |
| Bar (vertical and horizontal) | **Partial** | `BarDataset` has `BackgroundColor`, `BorderColor`, `BorderWidth`, `Fill`, `HoverBackgroundColor`, `Stack`. Horizontal bars via `Options.IndexAxis = Axes.Y`. No `barThickness`, `barPercentage`, `categoryPercentage`, `borderRadius`, `borderSkipped`, `minBarLength`, and no `xAxisID`/`yAxisID`, so a bar dataset cannot be pinned to a named axis. |
| Line | **Partial** | `BackgroundColor`, `BorderColor`, `BorderWidth`, `Tension`, `Stepped`, `CubicInterpolationMode`, `PointRadius`, `PointStyle`, `Fill`, `YAxisId` — pin a series to a second axis with `YAxisId = "y2"`. `Fill` is `bool?` and `Tension` is `decimal?`, so `Fill = false` and `Tension = 0` are serialized rather than dropped — both agree with Chart.js's own line defaults, so they only override something if the chart options come from a custom options class ([Escape hatches](#escape-hatches)); `Options.Elements.Line` here is `BorderColor` and `BorderWidth` only. No `spanGaps`, `borderDash`, `segment`, `hidden`, `pointHoverRadius`, `pointBackgroundColor`/`pointBorderColor`. |
| Area (filled line) | **Partial** | `LineDataset.Fill` and `RadarDataset.Fill` are `bool?`: `true` fills to the origin, `false` turns the fill off, unset leaves the Chart.js default for the type (off for line, on for radar). The `'origin'`, `'start'`, `'end'`, `'+1'`, `'-1'` and `{ target, above, below }` fill targets cannot be expressed, so filling to another dataset or to a value is out of reach. |
| Pie | **Partial** | `PieOptions` adds `Rotation` and `Circumference`. Dataset: `BackgroundColor`, `BorderWidth`, `HoverOffset`. No `borderColor`, `offset`, `spacing`, `weight`. |
| Doughnut | **Partial** | Same dataset surface as pie, but `DoughnutChartConfig.Options` is the plain `Options`, so `cutout`, `rotation` and `circumference` have no property at all — the ring is always Chart.js's default 50% cutout. |
| Polar area | **Partial** | Dataset: `BackgroundColor` and `BorderWidth` only. The `r` scale is reachable as `Options.Scales["r"]`, which has no `angleLines`, `pointLabels` or `startAngle`. |
| Radar | **Partial** | The thinnest type. `RadarChartConfig.Options` is `RadarOptions`, which does **not** derive from `Options`: only `Responsive`, `MaintainAspectRatio`, `Elements.Line.BorderWidth` and `Scales.R` (`BeginAtZero`, `Min`, `Max`). No `Plugins` block, so no legend, title, tooltip, datalabels, zoom or annotation options, and no `Options.OnClickAsync`/`OnHoverAsync` or tooltip/tick callbacks. The `OnChartClick`, `OnChartOver` and `OnLegendClick` component parameters still fire. Both of the properties it does have are nullable now, and both defaults are non-zero, so both are worth setting: `RadarDataset.Fill` is `bool?`, so `Fill = false` turns off the fill Chart.js puts there by default, and `Elements.Line.BorderWidth` is `int?`, so `BorderWidth = 0` hides the outline instead of falling back to Chart.js's `3`. |
| Scatter | **Partial** | `ScatterDataset` derives from `CustomDataset<ScatterXYValue>`, not `Dataset`, so it has no `Order`, `Type` or per-dataset `DataLabels`. Covers `BackgroundColor`, `BorderColor`, `BorderWidth`, `PointRadius`, `PointStyle`, `PointHitRadius`, `ShowLine`, `Tension` and `YAxisId` — a second axis is bound with `YAxisId = "y2"`. `ShowLine` is `bool?` and `Tension` is `decimal?`, so `ShowLine = false` and `Tension = 0` are serialized rather than dropped; like line, both agree with Chart.js's own scatter defaults. |
| Bubble | **Partial** | `BubbleDataset` has `Label`, `Data` and `BackgroundColor` and nothing else. `BubbleCoords.X`, `.Y` and `.R` are `int`, so fractional coordinates and radii cannot be expressed. No `borderColor`, `borderWidth`, `hoverRadius`. |
| Mixed / combo | **Partial** | Set `Dataset.Type` per dataset, e.g. `Type = "line"` on a dataset inside a `BarChartConfig`. Every dataset must still be the config's own dataset type, so that "line" is a `BarDataset` and cannot use `Tension`, `PointStyle`, `PointRadius`, `Stepped` or `YAxisId`. `ScatterDataset` and `BubbleDataset` have no `Type`, so they cannot join a mixed chart. |

### Scales

| Feature | Status | Notes |
| --- | --- | --- |
| `linear` | **Partial** | `Axis` covers `Min`, `Max`, `SuggestedMin`, `SuggestedMax`, `BeginAtZero`, `Display`, `Position`, `Type`, `Grid`, `Border`, `Ticks`, `Title`. No `grace`, `offset`, `reverse`, `alignToPixels`, `bounds`, `weight`, `clip`, `stack`/`stackWeight`. |
| `logarithmic` | **Partial** | Selected with `Axis.Type = "logarithmic"`; same `Axis` surface as linear. Tick number formatting is not exposed, so log labels keep Chart.js defaults. |
| `category` | **Partial** | `Axis.Type = "category"`. The scale's own `labels` array is not exposed, and `Axis.Min`/`Max` are `double?`, so bounding a category axis by label string is not possible. |
| `time` | **Partial** | `Axis.Time` gives `Unit`, `MinUnit`, `Round`, `IsoWeekday`, `TooltipFormat` and `DisplayFormats`, and tick generation is `Axis.Ticks.Source` (`"auto"`, `"data"`, `"labels"`), the key Chart.js actually reads. `time.parser` is missing, so a custom input format cannot be declared — timestamps have to arrive in a form the moment adapter parses unaided (ISO 8601 strings, epoch numbers). Needs the moment and moment-adapter script tags. |
| `timeseries` | **Partial** | `Axis.Type = "timeseries"`; identical surface and identical caveats to `time`. |
| `radialLinear` | **Partial** | Polar area reaches it through `Options.Scales["r"]` (a full `Axis`); radar only through `RadarOptions.Scales.R` (`BeginAtZero`, `Min`, `Max`). `angleLines`, `pointLabels`, `startAngle` and `ticks.backdropColor` are missing on both. |
| Stacking | **Partial** | `Axis.Stacked` (bool) plus `BarDataset.Stack` for stack groups. `stacked: 'single'` is not expressible, and only bar datasets have a `Stack`. |
| Multiple axes | **Partial** | Any number of entries in `Options.Scales`. Only `LineDataset` and `ScatterDataset` can bind to one, via `YAxisId`. There is no `xAxisID` anywhere, and no axis id on bar, pie, doughnut, polar or radar datasets. |
| Axis position | **Partial** | `Axis.Position` offers top, left, bottom, right; `Axis.PositionString` takes any string, so `"center"` also works. The object form (`position: { y: 0 }`, an axis pinned to a value on another scale) cannot be expressed. |
| Grid lines | **Partial** | `Grid` has `Color`, `Display`, `DrawOnChartArea`, `DrawTicks`. No `lineWidth`, `offset`, `tickColor`, `tickLength`, `tickWidth`, `tickBorderDash`, `z`, `circular`. |
| Axis border | **Full** | `Axis.Border` covers the whole Chart.js 4 `scales[].border` object: `Display`, `Width`, `Color`, `Dash`, `DashOffset`, `Z`. |
| Ticks | **Partial** | `Color`, `Font`, `AutoSkip`, `MaxTicksLimit`, `StepSize`, `MinRotation`, `MaxRotation`, `CrossAlign`, `Source` (time scales only), `Callback`/`CallbackAsync`. No `display`, `align`, `padding`, `precision`, `count`, `includeBounds`, `labelOffset`, `mirror`, `sampleSize`, `autoSkipPadding`, `backdropColor`, `textStrokeColor`/`textStrokeWidth`, `z`, or the `Intl.NumberFormat` `format` object. |
| Axis title | **Partial** | `AxesTitle` has `Text`, `Display`, `Align`, `Color`, `Font`. `padding` is missing. |

### Built-in plugins

| Plugin | Status | Notes |
| --- | --- | --- |
| `legend` | **Partial** | `Display`, `Position`, `Align`, `Reverse`, `RTL`, `TextDirection`, `FullSize` and a click handler. `Labels` covers `Color`, `Font`, `BoxWidth`, `BoxHeight`, `Padding`, `UsePointStyle`, `PointStyle`/`PointStyleWidth`, `TextAlign`, `UseBorderRadius`, `BorderRadius` and `Filter` — the whole `legend.labels` object bar `generateLabels` and `sort`. See [Legend label styling](#legend-label-styling). On the legend itself, `maxWidth`/`maxHeight`, `title`, `onHover` and `onLeave` are still missing. |
| `title` | **Partial** | `Display`, `Text`, `Position`, `Align`, `Color`, `Font`, `Padding`, `FullSize`. `Padding` is a `TitlePadding` carrying `Top` and `Bottom`, which is the whole of what Chart.js reads here — the type omits `Left`/`Right` because the title box discards them. `Text` is a single `string`, so a multi-line title (Chart.js accepts `string[]`) is not possible. |
| `subtitle` | **None** | No property for `plugins.subtitle`. A second `Title` cannot stand in for it — there is only one `title` slot. |
| `tooltip` | **Partial** | Colours and fonts: `BackgroundColor`, `TitleColor`/`TitleFont`, `BodyColor`/`BodyFont`, `FooterColor`/`FooterFont`, `BorderColor`, `BorderWidth`, `MultiKeyBackground`, plus `Callbacks.Label` and `Callbacks.Title`. Layout and behaviour are still unreachable: `enabled`, `mode`, `intersect`, `position`, `padding`/`caretPadding`/`caretSize`/`boxPadding`, `cornerRadius`, `displayColors`, `boxWidth`/`boxHeight`, `usePointStyle`, the `*Align` and `*Spacing` options, `external`, `filter`, `itemSort`, `rtl`/`textDirection`, and the other dozen callbacks. |
| `filler` | **Partial** | Reachable only as the `bool?` `Fill` on a line or radar dataset — on, explicitly off, or left to the type's default. `plugins.filler` itself (`propagate`, `drawTime`) has no property. |
| `decimation` | **None** | No property for `plugins.decimation`, so LTTB and min/max decimation of large series are unavailable. |
| `colors` | **None** | No property for `plugins.colors`. The plugin ships enabled in Chart.js 4 and will colour any dataset that defines none, and it can neither be configured nor switched off from C#. Set `BackgroundColor`/`BorderColor` on every dataset to keep it out of the way. |

### Bundled third-party plugins

| Plugin | Status | Notes |
| --- | --- | --- |
| zoom — wheel | **Full** | `Wheel.Enabled`, `Speed`, `ModifierKey` is the plugin's whole `zoom.wheel` surface. `Speed` is `decimal?` and still defaults to the plugin's `0.1`; `Speed = 0` is expressible now and makes a wheel event zoom by a factor of exactly 1, i.e. not at all. |
| zoom — pinch | **Full** | `Pinch.Enabled` is the whole `zoom.pinch` surface. Needs `hammer.js` on the page. |
| zoom — drag | **Partial** | `Enabled`, `BackgroundColor`, `BorderColor`, `BorderWidth`, `ModifierKey`, `Threshold` (`int?`, unset means the plugin's own `0`, so every drag zooms however short). Missing `drawTime` and `maintainAspectRatio`. |
| zoom — direction | **Full** | `ZoomOptions.Mode`, `ScaleMode` and `OverScaleMode`, plus `Zoom.Mode`/`Zoom.OverScaleMode` which are pushed into the nested object the plugin reads. |
| zoom — pan | **Partial** | `Enabled`, `Mode`, `ModifierKey`, `OverScaleMode`, `Threshold` — `Threshold` is `int?` and still defaults to the plugin's `10`, and `Threshold = 0` (start panning on the first pointer move) is reachable now. Missing `scaleMode` and the `onPan`, `onPanStart`, `onPanComplete`, `onPanRejected` callbacks. |
| zoom — limits | **Partial** | `Limits.X` and `Limits.Y` with `Min`, `Max` and `MinRange`, numbers serialized as numbers and `"original"` as the literal. Limits keyed by a custom scale id (anything other than `x`/`y`) cannot be expressed. |
| zoom — events and API | **None** | No `onZoom`, `onZoomStart`, `onZoomComplete`, `onZoomRejected`, no `zoom.animation`, and the component exposes no `ResetZoom()`, `Zoom()`, `Pan()` or `GetZoomLevel()`. Resetting the view means recreating the chart. |
| datalabels | **Partial** | `Plugins.DataLabels` chart-wide and `Dataset.DataLabels` per dataset: alignment, anchor, colours, border, font, padding, rotation, text align, text stroke and shadow. Missing `formatter` (so the label is always the raw value), `display`, `labels` (several labels per point) and `listeners` — everything else in the plugin's option set has a property. Every numeric and boolean one of them is nullable (`BorderRadius`, `BorderWidth`, `Clamp`, `Clip`, `Offset`, `Opacity`, `Rotation`, `TextStrokeWidth`, `textShadowBlur`), so a zero or a `false` reaches the plugin instead of being dropped: `Offset = 0` sits the label on its anchor rather than 4px off it, `Opacity = 0` hides a label, and `Clamp = false` / `Clip = false` override a chart-wide `Plugins.DataLabels` that set them. Requires `Options.RegisterDataLabels = true` and the script tag. |
| annotation | **Partial (raw JSON)** | `Plugins.Annotation.Annotations` is a `Dictionary<string, object>`, so every annotation type the plugin supports — line, box, ellipse, label, point, polygon — works, but there are no typed models: you build each annotation as an anonymous object using the plugin's own key names, with no compile-time checking. Registered automatically when the property is set. |
| autocolors | **None** | The plugin is vendored, but `Plugins` has no `autocolors` property, nothing registers it, and its script is not in the install list. The `Autocolors` class under `Models/Common` is not referenced by anything. |
| crosshair | n/a | `Plugins.Crosshair` (`Cursor`, `Vertical`, `Horizontal`) is this package's own canvas drawing, not chartjs-plugin-crosshair. It is not a Chart.js feature and has no upstream equivalent. |
| date adapter | **Partial** | moment and `chartjs-adapter-moment` are bundled, and `Options.Locale` is copied into `scales[].adapters.date.locale` for time scales. `scales[].adapters` is not otherwise reachable, and no other adapter can be selected. |

### Options families

| Family | Status | Notes |
| --- | --- | --- |
| `animation` | **Partial** | `Options.Animation` is a `bool?`: animation can be switched off, and that is all. `duration`, `easing`, `delay`, `loop`, `onProgress` and `onComplete` have no property, so there is no way to be told an animation finished. |
| `animations` (per property) | **Partial** | `Animations` exposes `Colors` and `X` as `bool` and `Tension` as an object (`From`, `To`, `Duration`, `Delay`, `Easing`, `Loop`). Chart.js ignores a non-object here, so on `Colors`/`X` only `false` does anything. `numbers`, `radius`, `y` and custom `properties`/`type`/`fn` configs are not exposed. |
| `transitions` | **None** | No property for `options.transitions`, so the `active`, `resize`, `show` and `hide` transition modes keep their defaults. |
| `interaction` | **Full** | `Options.Interaction` covers `Mode`, `Intersect`, `Axis` and `IncludeInvisible` — the entire `options.interaction` object. |
| `hover` | **None** | No property for `options.hover`, so hover-specific overrides of the interaction settings are unavailable. Use `Options.Interaction` and `OnHoverAsync`. |
| `events` | **None** | No property for `options.events`, so which DOM events the chart listens to cannot be narrowed. |
| `layout` | **None** | No property for `options.layout.padding` or `autoPadding`. The `Height`, `Width` and `Style` parameters on `<Chart>` size the container but cannot pad the chart area. |
| Responsive and aspect ratio | **Partial** | `Responsive` and `MaintainAspectRatio` only — note this package defaults `MaintainAspectRatio` to `false`, where Chart.js defaults it to `true`. `aspectRatio`, `resizeDelay`, `devicePixelRatio` and `onResize` have no property. |
| `elements` | **Partial** | `Options.Elements` has `Line.BorderColor` and `Line.BorderWidth` and nothing else. `elements.point`, `elements.bar`, `elements.arc` and the rest of `elements.line` — `tension`, `fill`, `borderDash`, `stepped`, `spanGaps`, `capBezierPoints`, `cubicInterpolationMode`, the border cap and join styles — are not exposed. `RadarOptions.Elements.Line` has only `BorderWidth` (`int?`, default `3`; `0` hides the radar outline). |
| Per-dataset defaults | **None** | No property for `options.datasets.<type>`, so defaults shared by every dataset of a type must be repeated on each dataset. |
| `parsing` | **None** | Neither `options.parsing` nor `dataset.parsing` has a property, so data has to arrive in the shape the controller expects. (`Models/Common/Parsing.cs` exists but nothing references it.) |
| `locale` | **Full** | `Options.Locale` sets `options.locale` and is propagated to the moment adapter for time scales. |
| `indexAxis` | **Full** | `Options.IndexAxis` with the `Axes.X` / `Axes.Y` / `Axes.Default` constants. |
| Grouped axis labels | extra | `Options.GroupXAxis` / `GroupYAxis` are a feature of this package, not of Chart.js: they add a second category axis and split labels on `;`. |

### Callbacks and events

| Callback | Status | Notes |
| --- | --- | --- |
| Tooltip label and title | **Partial** | `Tooltip.Callbacks.Label` and `.Title`, both `Func<CallbackGenericContext, string[]>`. The context carries only `DatasetIndex`, `DataIndex` and `Value` — not the dataset, the label or the formatted value. Resolved asynchronously, so Chart.js's own text is painted on the first frame and replaced when .NET answers. No other tooltip callback is exposed. |
| Tick callback | **Partial** | `Ticks.Callback` and `Ticks.CallbackAsync` per scale; the context is `Value`, `Index` and the tick values. Returns `string[]`. |
| Legend filter | **Partial** | `LegendLabels.Filter`; returning `null` means "no opinion" and keeps the entry, only `false` hides it. `generateLabels` and `sort` are not exposed. |
| Legend click | **Partial** | `Plugins.Legend.OnClickAsync` or the `OnLegendClick` parameter, with `LegendIndex` and `LegendText`. Chart.js's own show/hide toggle still runs. `onHover` and `onLeave` are not exposed. |
| Chart click | **Partial** | `Options.OnClickAsync` or the `OnChartClick` parameter. Fires only when an element is hit, and the context is `DatasetIndex`, `DataIndex`, `Value` — the raw event and the active-element list are not passed. `options.onClick` is always installed by this package, so a handler of your own cannot replace it. |
| Chart hover | **Partial** | `Options.OnHoverAsync` or the `OnChartOver` parameter. The context is the value under the cursor on the scales literally named `x` and `y`, and reports `0` for a chart that has no such scale (pie, doughnut, polar area, radar) or whose axes are named otherwise. |
| Mouse out | extra | `Options.OnMouseOutAsync` is a Blazor `@onmouseout` on the container, not a Chart.js option. |
| Scriptable options | **None** | Chart.js lets nearly any option be a function of the data point. Nothing in the models does, so per-point colours, radii and styles have to be supplied as arrays instead. |
| Chart.js instance API | **None** | The component exposes `AddData`, `AddDataset<T>` and `ClearData`. `update`, `reset`, `resize`, `stop`, `toBase64Image`, `getElementsAtEventForMode` and the zoom plugin's chart methods are not surfaced. |

### Escape hatches

There is no general one. The models have no `[JsonExtensionData]` bag anywhere, so arbitrary keys cannot be added to `options`, `scales`, `plugins` or a dataset. What does exist:

- **`Plugins.Annotation.Annotations`** is a `Dictionary<string, object>`, and the objects in it are serialized verbatim. This is the one place raw Chart.js JSON is accepted by design, and it is the whole reason every annotation type works.
- **`Chart.AddDataset<T>(T dataset)`** takes any `class` and serializes it as-is onto `chart.data.datasets`, so a dataset with options the typed classes do not model can be pushed at runtime — after the first render, and without the config object knowing about it. This is the practical way out of a dataset-level gap.
- **`Chart.Config` is `IChartConfig` and `IOptions` is an empty marker interface**, so you can supply your own config and options classes and emit whatever `options` JSON you like. The cost is that the C# callback plumbing checks `Options is Options`: with a custom options type, `Options.OnClickAsync`, `OnHoverAsync`, the tooltip and tick callbacks and the legend filter all stop firing. (This is exactly why `RadarOptions` has none of them.) The `OnChartClick`, `OnChartOver` and `OnLegendClick` component parameters keep working.

What is *not* an escape hatch: **subclassing a typed dataset**. `Data<T>.Datasets` is a `List<T>` of the concrete type, and `System.Text.Json` serializes those elements by their declared type, so properties added on a derived class are silently dropped. `CustomDataset<T>` is likewise not a general-purpose container — it is the base for `ScatterDataset` and `BubbleDataset` and carries only `label` and `data`.

## Upgrading from 1.0.0

Already on `Erkan.Blazor.Chartjs` 1.0.0? This is the only section you need — [the next one](#migrating-from-pscblazorcomponentschartjs) is for people coming off upstream `PSC.Blazor.Components.Chartjs`.

### Six properties are gone

2.0.0 deletes six properties. Each of them existed, serialized, and wrote a key Chart.js 4.5.1 never reads, so no chart was ever affected by the value you gave it. The compile error is the whole of the breakage, and every one has a replacement that does work.

| Removed in 2.0.0 | It wrote | Use instead |
| --- | --- | --- |
| `AxesTime.Source` | `scales[].time.source` | `Ticks.Source`. Chart.js reads tick generation from `ticks.source`, so the value moves from `Axis.Time` to `Axis.Ticks`: `Time = new AxesTime { Source = "data" }` becomes `Ticks = new Ticks { Source = "data" }`. Same `auto` / `data` / `labels` values. |
| `Axis.Color` | `scales[].color` | One of four, depending on what you meant to colour — there is no single equivalent. `Ticks.Color` for the tick labels, `Grid.Color` for the grid lines, `Border.Color` for the axis line itself, `AxesTitle.Color` for the axis title. Chart.js 4 has no scale-level `color`: a scale declares none, nothing lets it inherit one, and every colour a scale draws with comes from those four objects. |
| `LineDataset.Y2AxisId`, `ScatterDataset.Y2AxisId` | `y2AxisID` | `YAxisId = "y2"`. There is no `y2AxisID` option in Chart.js — a dataset names its scale with `yAxisID`, whatever that scale is keyed as in `Options.Scales`. |
| `LineDataset.FillColor` | `fillColor` | `BackgroundColor`. `fillColor` is a Chart.js **1.x** name and has not been read since 2.0. |
| `LineDataset.StrokeColor` | `strokeColor` | `BorderColor`. Also a Chart.js 1.x name. |
| `OnAnimationComplete` on `BarChartConfig`, `BubbleChartConfig`, `DoughnutChartConfig`, `PieChartConfig`, `PolarChartConfig`, `RadarChartConfig` and `ScatterChartConfig` | `onAnimationComplete` at the root of the config object | Nothing yet — delete the assignment. Chart.js 4 has `options.animation.onComplete`, which this package does not expose ([`animation` row](#options-families)). |

`Axis.Color` is the one with a choice to make. Because it never coloured anything, the value you had set cannot tell you which of the four you meant:

```diff
- new Axis { Color = "#52606d" }
+ new Axis { Ticks = new Ticks { Color = "#52606d" } }   // tick labels
+ // also: Grid.Color, Border.Color, AxesTitle.Color
```

### Title padding is `TitlePadding`

`Title.Padding` is a `TitlePadding?` rather than the four-sided `Padding`, and carries `Top` and `Bottom` only:

```diff
- Padding = new Padding { Top = 8, Right = 8, Bottom = 16, Left = 8 }
+ Padding = new TitlePadding { Top = 8, Bottom = 16 }
```

Chart.js types `plugins.title.padding` as `number | { top, bottom }`, and the title box reads only `padding.height` (top plus bottom) and `padding.top`. A horizontal title is laid out across the full chart width and a vertical one across the full height, so there is no horizontal extent for a left or right padding to occupy — the two values were accepted, serialized, and discarded. Dropping them from the type is the whole change; `new TitlePadding(6)` gives equal padding above and below, which is what a bare `number` means to Chart.js too.

This affects the seven chart types whose options class exposes `Plugins`. Radar is unaffected because `RadarOptions` has no `Plugins` block at all. The shared four-sided `Padding` class is untouched and stays correct where Chart.js does read all four sides, which is `DataLabels.Padding`.

Like `Axis.Color`, migrating causes no visual change. Both properties compiled and serialized perfectly well in 1.0.0 and changed nothing in the browser, so the code you are about to edit was already a no-op — the compile error is the entire cost of the upgrade.

### Eighteen properties are nullable now

These were declared as non-nullable value types and skipped by the serializer whenever they held the type's own default, so assigning `0` or `false` wrote no key at all and Chart.js applied its own default instead. There was no value you could give any of them to mean "zero" or "off". All eighteen are nullable now, and the value you assign is the value Chart.js receives:

| Owner | Properties | Type now |
| --- | --- | --- |
| `DataLabels` | `BorderRadius`, `BorderWidth`, `Offset`, `Rotation`, `TextStrokeWidth`, `textShadowBlur` | `int?` |
| `DataLabels` | `Clamp`, `Clip` | `bool?` |
| `DataLabels` | `Opacity` | `decimal?` |
| `LineDataset` | `Fill` | `bool?` |
| `LineDataset`, `ScatterDataset` | `Tension` | `decimal?` |
| `RadarDataset` | `Fill` | `bool?` |
| `RadarOptionsElementsLine` | `BorderWidth` | `int?` |
| `ScatterDataset` | `ShowLine` | `bool?` |
| `Drag`, `Pan` (zoom) | `Threshold` | `int?` |
| `Wheel` (zoom) | `Speed` | `decimal?` |

The one most likely to matter: `RadarOptionsElementsLine.BorderWidth = 0` hides the radar outline, which was previously impossible — `0` was dropped and the outline came back at Chart.js's default width of `3`. Alongside it, `RadarDataset.Fill = false` turns off the fill radar puts there by default; `DataLabels.Offset = 0` sits a label on its anchor rather than the plugin's default `4` away from it, `DataLabels.Opacity = 0` hides one, and `Clamp = false` / `Clip = false` on a dataset override a chart-wide `Plugins.DataLabels` that set them; `Pan.Threshold = 0` starts a pan on the first pointer move rather than after 10 pixels, and `Wheel.Speed = 0` freezes wheel zoom. `Fill`, `Tension` and `ShowLine` on a line or scatter dataset agree with Chart.js's own defaults either way, so those only override something when the chart's options come from a custom options class ([Escape hatches](#escape-hatches)).

**Assigning them is unchanged.** `Tension = 0`, `Fill = false` and `Offset = 0` compile exactly as they did in 1.0.0 — the difference is that they now reach Chart.js. Only code that **reads** one breaks, because the value is nullable:

```diff
- decimal tension = dataset.Tension;
+ decimal tension = dataset.Tension ?? 0;

- if (dataset.Fill) { … }
+ if (dataset.Fill == true) { … }
```

**Nothing renders differently.** A chart that never set one of the eighteen serializes byte-for-byte the same JSON as it did under 1.0.0 — this was checked by diffing the serialized configuration before and after. Where a property carried a non-default initializer it was kept deliberately, so `Pan.Threshold` still emits `10`, `Wheel.Speed` still `0.1` and `RadarOptionsElementsLine.BorderWidth` still `3`; where the initializer merely repeated the type default it was dropped, so no new key appears either. Upgrading cannot change a chart you did not touch.

One more property changed shape: `Legend.Labels` is `LegendLabels?`. It has always been null unless you assigned it; the annotation says so now, so `#nullable enable` code that dereferenced it gets a warning it should have had.

### Five string-enum properties tell the truth about null

They were declared non-nullable over a backing field that stays null until you assign one, so the getter already returned null and the declaration was lying. The annotation is honest now: `Legend.Position` is `LegendPosition?`, `Legend.TextDirection` is `TextDirection?`, `Title.Position` and `Axis.Position` are `Position?`, and `AxesTitle.Align` is `Align?` — with its `AxesTitle.AlignString` now `string?`.

These are reference types, so unlike [the eighteen above](#eighteen-properties-are-nullable-now) nothing changes shape at runtime and no read is a hard compile error. Under `#nullable enable` a read into a non-nullable local raises `CS8600`, the warning it should always have raised; a fallback settles it:

```diff
- LegendPosition position = legend.Position;
+ LegendPosition position = legend.Position ?? LegendPosition.Bottom;
```

And the reason it matters: **assigning `null` used to throw.** Each of these facades mirrors its value into a `*String` twin that is what actually serializes, and the setter read `value.Value` without checking — so `Position = null`, the obvious way to clear one back to the Chart.js default, raised a `NullReferenceException`. Nine properties had it, the five above plus `LineDataset.CubicInterpolationMode`, `LineDataset.PointStyle`, `LineDataset.Stepped` and `ScatterDataset.PointStyle` (those four were already nullable and needed only the setter fix). All nine accept `null` now and clear the serialized key with it.

### Dataset colour lists start out null

`BarDataset.BackgroundColor`, `BarDataset.BorderColor`, `PieDataset.BackgroundColor`, `DoughnutDataset.BackgroundColor` and `PolarDataset.BackgroundColor` are `List<string>?` with no initializer. They used to be handed an empty list, which every untouched chart then shipped to Chart.js as `"backgroundColor": []` — and on bar, `"borderColor": []` as well.

Assigning a whole list is unchanged, which is how the examples in this README and the demo do it. What breaks is calling `.Add()` on the property without assigning one first:

```diff
- dataset.BackgroundColor.Add("#f00");
+ dataset.BackgroundColor = new List<string> { "#f00" };
```

### Keys that no longer reach Chart.js

None of these need a source change.

- **The wrapper's internal markers.** `hasFilter`, `hasLabel`, `hasCustomTitle`, `hasCallback` and `hasAsyncCallback` were each deleted only on the branch that handled a registered callback, so a `LegendLabels` without a `Filter`, a `Tooltip` without callbacks, and any scale without a tick callback shipped their marker through as a live `false` — `plugins.legend.labels.hasFilter`, `plugins.tooltip.callbacks.hasLabel` and `hasCustomTitle`, and `scales[].ticks.hasCallback` and `hasAsyncCallback`. All five are stripped unconditionally now.
- **`crosshair`, `groupXAxis` and `groupYAxis`.** `crosshair` was blanked with `undefined` rather than deleted, which leaves the key in place on a live object, and the two group markers were only cleared inside the branch that acts on them — so `GroupXAxis = false` was handed to Chart.js as an option of its own. All three are deleted before the chart is constructed.
- **Bare `null` values.** `RadarOptions.Scales`, `RadarOptionsScales.R`, `RadarOptionsScalesRadius.Min` and `.Max`, and `LineDataType.X` and `.Y` were serialized as `null` rather than omitted, so an unconfigured radar shipped `"scales": null` and every `LineDataType` point carried `"x": null, "y": null`. All six are omitted when unset.

### The legend filter no longer breaks on an empty data object

`LegendLabels.Filter` injected a `$type` discriminator by splicing into the serialized JSON text, which turns an empty data object `{}` into the invalid `{"$type":"base",}`. The exception left the filter permanently pinned to its fallback, so no legend entry could be hidden for the life of the chart. The discriminator is set on the object directly now, and the chart configuration reaches the JS layer as an object instead of being rebuilt with `eval`.

## Migrating from `PSC.Blazor.Components.Chartjs`

### Package, assembly and namespace

The package ID, assembly, and root namespace all changed. Replace `PSC.Blazor.Components.Chartjs` with `Erkan.Blazor.Chartjs` in your `_Imports.razor` and in the `_content/...` script paths in `index.html` / `_Host.cshtml`. Renaming is not enough for the Chart.js `<script>` tag itself — its **filename** changes too, from `chart.js` to `chart.umd.js`, as shown in [Script tags](#script-tags) right below. A plain find-and-replace leaves you pointing at `_content/Erkan.Blazor.Chartjs/lib/Chart.js/chart.js`, which is the ES module build and fails to load from a classic `<script>` tag.

### Script tags

Chart.js moved from 3.9.1 to 4.5.1. Load the **UMD** build:

```diff
- <script src="_content/PSC.Blazor.Components.Chartjs/lib/Chart.js/chart.js"></script>
+ <script src="_content/Erkan.Blazor.Chartjs/lib/Chart.js/chart.umd.js"></script>
```

`chart.js` in the Chart.js 4.x distribution is an **ES module** and throws `Unexpected token 'export'` under a classic `<script>` tag. `chart.umd.js` is the build to use from a `<script>` tag. See [Installation](#installation) for the full list.

### Breaking API changes

| Upstream | Now | Migration |
| --- | --- | --- |
| `Chart.AddData`, `Chart.AddDataset<T>`, `Chart.ClearData` return `void` (`async void`) | return `Task` | `await _chart1.AddData(...)`. The old `async void` swallowed exceptions and could not be sequenced; awaiting is now the supported way to know the chart has updated. `AddData`'s `labels` is explicitly nullable — pass `null` to append values without adding labels. |
| `Chart : IDisposable` | `Chart : IAsyncDisposable` | `Dispose()` is gone, `DisposeAsync()` replaces it. Blazor calls it for you; only code that disposed a chart by hand needs changing. This fixes a leak where every chart re-creation leaked a `DotNetObjectReference`, a JS module handle, and a live Chart.js instance. |
| `BarDataset.Stack` is `List<string>` | `string?` | `Stack = new List<string> { "One" }` becomes `Stack = "One"`. Chart.js compares stack identifiers by value, so a list never matched and grouped-stacked bars rendered misaligned (upstream [#48](https://github.com/erossini/BlazorChartjs/issues/48)). |
| `Axis.Text` | removed | It serialized as `"Text"`, which is not a Chart.js option and was silently ignored. Use `Axis.Title` (`AxesTitle`), whose `Text` property is the real axis title. |
| `Grid.DrawBorder` | removed | Chart.js 4 moved the axis border out of `grid` into a `border` object. Use the new `Axis.Border` (`Display`, `Width`, `Color`, `Dash`, `DashOffset`, `Z`). |
| `AxesTime.IsoWeekday` is `bool?` | `int?` | A day index: `0` = Sunday, `1` = Monday … `6` = Saturday. `IsoWeekday = true` becomes `IsoWeekday = 1`. |
| `Zoom.Enabled` | removed | chartjs-plugin-zoom 2.x has no master switch, so there was nothing left for the property to switch. `Enabled = true` is now a compile error: delete it and turn on the gestures you want instead — `ZoomOptions.Wheel`, `ZoomOptions.Pinch` and `ZoomOptions.Drag` for zooming, `Zoom.Pan` for panning. |
| `Zoom.Mode`, `Zoom.OverScaleMode` serialized to `plugins.zoom.mode` | serialized to `plugins.zoom.zoom.mode` | No source change needed — but these previously landed where the plugin never looked, so `Mode = "x"` did nothing. If you worked around that, remove the workaround. `ZoomOptions` also gained `Mode`, `OverScaleMode` and `ScaleMode` if you prefer to set them directly; a value set on `ZoomOptions` wins over the one on `Zoom`. |
| `Limits` / `ScaleLimits` | implemented | `Limits` was an empty class, so zoom limits could not be expressed at all. It now has `X` and `Y` (`ScaleLimits`). Numeric limits serialize as JSON numbers rather than strings, and an unset limit is omitted instead of defaulting to `"original"`. |

### Behaviour fixes (no source change needed)

- **Tooltip, title and legend-filter callbacks now work on Blazor Server and SSR.** They used synchronous JS→.NET interop, which throws on any render mode other than WebAssembly — including the default .NET 10 Blazor Web App template. They are async now and work on all render modes: the chart renders Chart.js's own default label first, then swaps in your value once .NET replies.
- **Legend clicks no longer break the built-in toggle.** Registering a legend handler used to replace Chart.js's own `onClick`, killing the show/hide-dataset behaviour (upstream [#89](https://github.com/erossini/BlazorChartjs/issues/89)). The default handler now runs first, and the override is only installed when a handler is actually registered.
- **`RegisterDataLabels` is scoped per chart.** It called the process-wide `Chart.register`/`Chart.unregister`, so on a page with several charts the last one to render decided for all of them (upstream [#83](https://github.com/erossini/BlazorChartjs/issues/83)).
- **Tooltip callback values keep their precision.** `CallbackGenericContext.Value` was cast to `int`, so `12.5` arrived as `12`.
- **`Ticks.CallbackAsync` no longer spins at 100% CPU.** Each resolved label triggered a redraw, which re-ran the callback, which requested the label again.
- **Tick float-noise cleanup no longer zeroes small values, or blurs large ones.** The cleanup rounded to 10 decimal places, which flattened any legitimate value below `1e-10` to zero. The tolerance is relative to the axis range now, and exact integers are left alone entirely.
- **`OnChartClick`, `OnChartOver` and `OnLegendClick` fire.** These `Chart` parameters were declared but never wired to anything.
- **`AddData` does one redraw per call**, not one full chart re-render per point.
- **Canvas `Height` and `Width` both apply.** A missing CSS semicolon meant setting both silently dropped both.
- **Callbacks no longer throw `NotSupportedException`** when the property they read is null. `LegendLabelsFilter`, `TicksCallback`, `TitleCallbacks` and `TooltipCallbacksLabel` now return an empty result instead. A `LegendLabels.Filter` that returns `null` means "no opinion" and keeps the entry; only an explicit `false` hides it.

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
- `Axis.Border` — the Chart.js 4 axis border (`display`, `width`, `color`, `dash`, `dashOffset`, `z`), replacing the removed `Grid.DrawBorder`
- `Plugins.Annotation` — [chartjs-plugin-annotation](https://www.chartjs.org/chartjs-plugin-annotation/latest/guide/) support for lines, boxes, points, and labels. The plugin is registered automatically when a chart declares annotations.
- `Options.Locale` — BCP 47 tag (e.g. `tr-TR`) that propagates to Chart.js and to the moment date adapter, so time-axis labels format in the given locale
- Zoom `Limits` — `Limits.X` / `Limits.Y` are implemented, so pan and zoom can be bounded per axis
- `ZoomOptions.Mode`, `ZoomOptions.OverScaleMode`, `ZoomOptions.ScaleMode`
- `Legend.Labels` — the Chart.js `legend.labels` object beyond `Filter`: colour, font, box size, padding, point style, text align and border radius, so a canvas-drawn legend can follow an app's theme ([Legend label styling](#legend-label-styling))
- `Tooltip` colours and fonts — `BackgroundColor`, `TitleColor`/`TitleFont`, `BodyColor`/`BodyFont`, `FooterColor`/`FooterFont`, `BorderColor`, `BorderWidth`, `MultiKeyBackground`
- `Ticks.Source` — `auto` / `data` / `labels` tick generation on time scales, which is where Chart.js reads it from

### Fixes
- `0` and `false` reach Chart.js. Eighteen properties were non-nullable value types the serializer skipped whenever they held the type default, so `Fill = false`, `Tension = 0`, `Offset = 0`, `Threshold = 0` and the rest wrote no key at all and Chart.js used its own default instead. All eighteen are nullable now — see [Eighteen properties are nullable now](#eighteen-properties-are-nullable-now) for the list and for what does *not* change.
- `null` is handled throughout. Nine string-enum properties threw a `NullReferenceException` when set to `null`, five of them while declared non-nullable over a backing field that was null anyway; six properties wrote a bare `null` into the config instead of omitting the key; and five dataset colour lists shipped an empty `[]` on every untouched chart. See [Five string-enum properties tell the truth about null](#five-string-enum-properties-tell-the-truth-about-null) and the two sections after it.
- The wrapper's internal bookkeeping stays out of the chart config. The `hasFilter`, `hasLabel`, `hasCustomTitle`, `hasCallback` and `hasAsyncCallback` markers, and the `crosshair`, `groupXAxis` and `groupYAxis` keys, all reached Chart.js as live options whenever the feature they gate was off — `GroupXAxis = false` included. The legend filter also threw on a chart with an empty data object, which pinned it to its fallback for the life of the chart.
- Chart teardown: `Chart` is `IAsyncDisposable` and destroys the Chart.js instance, its DOM listeners and any queued animation frame. Previously each chart re-creation leaked a `DotNetObjectReference`, a JS module handle and a live chart.
- Tooltip, title, legend-filter and tick callbacks use async interop, so they work on Blazor Server and SSR instead of throwing
- Tick float noise is cleaned with a tolerance relative to the axis range, not a fixed digit count: at least 12 significant digits are kept, and more when the axis is zoomed in far enough to need them. Exact integers are never rounded at all, and any rounding that would be visible at the axis's own resolution is refused — so epoch milliseconds, byte counts and ids survive intact however wide the axis is. On top of that, a value below one ten-billionth of the visible span snaps to zero. The earlier fixed rounding to 10 decimal places flattened legitimate values below `1e-10` to zero.
- The async ticks callback (`Ticks.CallbackAsync`) renders, and no longer loops at 100% CPU. Chart.js tick callbacks are synchronous, so returning a Promise previously rendered `[object Promise]`; labels are now resolved out-of-band, cached per tick, and applied on a single coalesced `update('none')`.
- `registerDataLabels` was compared instead of deleted (`==` where `delete` was meant), leaving the internal flag in the serialized options
- The DataLabels plugin is attached per chart rather than registered globally, so one chart can no longer turn labels off for every other chart on the page
- `legend.onClick` is guarded and chains Chart.js's own handler, so the built-in show/hide toggle survives — and charts configured with `Plugins = null` no longer throw a `TypeError` during setup
- Crosshair redraws and `AddData` are batched into one update instead of one per mouse pixel / per point
- The demo host page loaded `Chart.js` (an ES module) as a classic script, throwing `Unexpected token 'export'` on every page load. The module is imported by the interop layer; the stray tag is gone.
- moment and `chartjs-adapter-moment` are now actually shipped and loaded, so `Options.Locale` and time axes work at runtime

> **Note on `chartjs-adapter-moment`:** the bundled copy is hand-patched to apply a per-instance locale in `format()`, so it is deliberately excluded from `libman.json`. Do not overwrite it with a LibMan restore without re-applying that patch.

### Tests

There is a test suite — 284 tests, upstream had none — and CI runs it on every push and pull request. The publish workflow runs it again and will not pack a release that fails.

```bash
dotnet test tests/Erkan.Blazor.Chartjs.Tests/Erkan.Blazor.Chartjs.Tests.csproj
```

It exists because this package's characteristic bug is invisible: a property that serializes to a key Chart.js never reads, or a value that vanishes on the way out, throws nothing and logs nothing — the chart just quietly ignores you. Three things guard against that.

- **Golden-JSON snapshots** of the serialized configuration for all eight chart types, in an empty, a minimal and a fully populated variant. Any change to what goes on the wire shows up as a diff rather than as a rendering surprise. Re-record with `UPDATE_SNAPSHOTS=1 dotnet test`.
- **Key validation.** Every `[JsonPropertyName]` in the model tree is checked against the option paths generated from Chart.js's own TypeScript definitions and those of the bundled plugins, so a property that writes a key nothing reads fails the build. The generated allowlist is checked in, and the suite fails if it is stale relative to the vendored bundle versions — regenerate with `cd tests/tools/chartjs-keys && npm install && npm run generate`.
- **A regression test per defect** fixed in 1.0.0 and 2.0.0, including one per removed property and one per nullability change.

This is how the last two defects in 2.0.0 — `Axis.Color` and the title padding — were caught before release rather than after.

## Links
* [Live demo](https://erkantaylan.github.io/BlazorChartjs/) for this fork
* Source code on [GitHub](https://github.com/erkantaylan/BlazorChartjs)
* [NuGet](https://www.nuget.org/packages/Erkan.Blazor.Chartjs/) package
* Upstream project by Enrico Rossini: [erossini/BlazorChartjs](https://github.com/erossini/BlazorChartjs) · [upstream demo site](https://chartjs.puresourcecode.com/) · [upstream docs](https://www.puresourcecode.com/dotnet/blazor/blazor-component-for-chartjs/)

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

For what each type's model actually exposes, and where it stops, see [Feature coverage](#feature-coverage).

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
Task AddData(List<string?>? labels, int datasetIndex, List<decimal?>? data)
```

For example, using __chart1_, the following code adds a new label `Test1` to the list of labels, and for the dataset _0_ adds a random number.

```csharp
await _chart1.AddData(new List<string?>() { "Test1" }, 0, new List<decimal?>() { rd.Next(0, 200) });
```

`AddData` returns a `Task` — await it. The whole batch is appended in one round trip and the chart is redrawn once, so passing several labels and values at a time is cheaper than calling it in a loop. Pass `null` for `labels` to append values without touching the labels. The call is a no-op if the chart has not rendered yet.

The result is visible in the following screenshot.

![chart-addnewdata](https://user-images.githubusercontent.com/9497415/229902251-8a2adf61-b37c-4fdc-a869-ca8eb1a7cd81.gif)

### Add a new dataset

It is also possible to add a completely new dataset to the graph. For that, there is the function `AddDataset`. This function requires a new dataset of the same format as the others already existing in the chart.

For example, this code adds a new dataset using `LineDataset` using some of the properties this dataset has.

```csharp
private async Task AddNewDataset()
{
    Random rd = new Random();
    List<decimal?> addDS = new List<decimal?>();
    for (int i = 0; i < 8; i++)
    {
        addDS.Add(rd.Next(i, 200));
    }

    var color = String.Format("#{0:X6}", rd.Next(0x1000000));

    await _chart1.AddDataset<LineDataset>(new LineDataset()
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

### Remove every value

`ClearData` empties the labels and the data of every dataset, keeping the datasets and the chart configuration in place.

```csharp
await _chart1.ClearData();
```

> `AddData`, `AddDataset<T>` and `ClearData` all return `Task`. In upstream they were `async void`, which meant an exception inside them could not be caught and the caller had no way to know when the chart had finished updating.

## Callbacks

The component has a few callbacks (more in development) to customize your chart. The callbacks are ready to use are:

- Tooltip
  * Labels
  * Titles
- Axis ticks — `Ticks.Callback` and `Ticks.CallbackAsync`
- Legend entries — `LegendLabels.Filter`

The demo page at `/ticksfilter` ([`ChartjsDemo/Pages/TicksFilterPage.razor`](ChartjsDemo/Pages/TicksFilterPage.razor)) exercises all of them on one chart, together with live data updates, and shows how often each one is actually asked.

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

MIT. See [LICENSE](LICENSE).
