# Feature coverage

What Chart.js **4.5.1** can do, against what the C# models in **2.0.0** actually let you set. Assessed by reading the vendored `chart.js` 4.5.1 bundle and the bundled plugins alongside `src/Models/**`, and by checking that each property serializes to the key Chart.js reads — a property that exists but lands where Chart.js never looks is listed as missing, not as supported.

| Status | Meaning |
| --- | --- |
| **Full** | Everything Chart.js offers for that option is reachable from C# |
| **Partial** | Reachable, with the gaps named in the note |
| **None** | No C# property. Not reachable through the models |

Coverage is thin in places, and the table says so. A **None** row means there is no property today, not that one is planned. If you land on one, see [Escape hatches](#escape-hatches).

## Chart types

| Type | Status | Notes |
| --- | --- | --- |
| Bar (vertical and horizontal) | **Partial** | `BarDataset` has `BackgroundColor`, `BorderColor`, `BorderWidth`, `Fill`, `HoverBackgroundColor`, `Stack`. Horizontal bars via `Options.IndexAxis = Axes.Y`. No `barThickness`, `barPercentage`, `categoryPercentage`, `borderRadius`, `borderSkipped`, `minBarLength`, and no `xAxisID`/`yAxisID`, so a bar dataset cannot be pinned to a named axis. |
| Line | **Partial** | The line: `BackgroundColor`, `BorderColor`, `BorderWidth`, `BorderDash`, `BorderDashOffset`, `BorderCapStyle`, `BorderJoinStyle`, `Tension`, `CubicInterpolationMode`, `StepMode`, `Fill`, `ShowLine`, `SpanGaps`, and on hover `HoverBackgroundColor`, `HoverBorderColor`, `HoverBorderWidth`. The points: `PointRadius`, `PointStyle`, `PointRotation`, `PointBackgroundColor`, `PointBorderColor`, `PointBorderWidth`, `PointHitRadius`, `PointHoverRadius`, `PointHoverBackgroundColor`, `PointHoverBorderColor`, `PointHoverBorderWidth`. Placement: `XAxisId`, `YAxisId` (pin a series to a second axis with `YAxisId = "y2"`), `Stack`, `Clip`, `DrawActiveElementsOnTop`. See [Line datasets](styling.md#line-datasets). The four point colours are `List<string>?`, one per point; the numeric point options take one value for the whole dataset. `StepMode.False` and `StepMode.True` are sent as JSON booleans — up to 2.0.0 `StepMode.False` went out as the string `"false"`, which Chart.js treats as truthy, and drew a stepped line. `Fill` is `bool?` and `Tension` is `decimal?`, so `Fill = false` and `Tension = 0` are serialized rather than dropped — both agree with Chart.js's own line defaults, so they only override something if the chart options come from a custom options class ([Escape hatches](#escape-hatches)); `Options.Elements.Line` here is `BorderColor` and `BorderWidth` only. Still missing: `segment`, `hidden`, `indexAxis`, `hoverBorderDash`, `hoverBorderDashOffset`, `hoverBorderCapStyle`, `hoverBorderJoinStyle`, `pointStyle: false`, the numeric form of `spanGaps` (the largest gap to span), and the `false` and per-side object forms of `clip` — `Clip` is a pixel count for every side. |
| Area (filled line) | **Partial** | `LineDataset.Fill` and `RadarDataset.Fill` are `bool?`: `true` fills to the origin, `false` turns the fill off, unset leaves the Chart.js default for the type (off for line, on for radar). The `'origin'`, `'start'`, `'end'`, `'+1'`, `'-1'` and `{ target, above, below }` fill targets cannot be expressed, so filling to another dataset or to a value is out of reach. |
| Pie | **Partial** | `PieOptions` adds `Rotation` and `Circumference`. Dataset: `BackgroundColor`, `BorderWidth`, `HoverOffset`. No `borderColor`, `offset`, `spacing`, `weight`. |
| Doughnut | **Partial** | Same dataset surface as pie, but `DoughnutChartConfig.Options` is the plain `Options`, so `cutout`, `rotation` and `circumference` have no property at all — the ring is always Chart.js's default 50% cutout. |
| Polar area | **Partial** | Dataset: `BackgroundColor` and `BorderWidth` only. The `r` scale is reachable as `Options.Scales["r"]`, which has no `angleLines`, `pointLabels` or `startAngle`. |
| Radar | **Partial** | The thinnest type. `RadarChartConfig.Options` is `RadarOptions`, which does **not** derive from `Options`: only `Responsive`, `MaintainAspectRatio`, `Elements.Line.BorderWidth` and `Scales.R` (`BeginAtZero`, `Min`, `Max`). No `Plugins` block, so no legend, title, tooltip, datalabels, zoom or annotation options, and no `Options.OnClickAsync`/`OnHoverAsync` or tooltip/tick callbacks. The `OnChartClick`, `OnChartOver` and `OnLegendClick` component parameters still fire. Both of the properties it does have are nullable now, and both defaults are non-zero, so both are worth setting: `RadarDataset.Fill` is `bool?`, so `Fill = false` turns off the fill Chart.js puts there by default, and `Elements.Line.BorderWidth` is `int?`, so `BorderWidth = 0` hides the outline instead of falling back to Chart.js's `3`. |
| Scatter | **Partial** | `ScatterDataset` derives from `CustomDataset<ScatterXYValue>`, not `Dataset`, so it has no `Order`, `Type` or per-dataset `DataLabels`. Covers `BackgroundColor`, `BorderColor`, `BorderWidth`, `PointRadius`, `PointStyle`, `PointHitRadius`, `ShowLine`, `Tension` and `YAxisId` — a second axis is bound with `YAxisId = "y2"`. `ShowLine` is `bool?` and `Tension` is `decimal?`, so `ShowLine = false` and `Tension = 0` are serialized rather than dropped; like line, both agree with Chart.js's own scatter defaults. |
| Bubble | **Partial** | `BubbleDataset` has `Label`, `Data` and `BackgroundColor` and nothing else. `BubbleCoords.X`, `.Y` and `.R` are `int`, so fractional coordinates and radii cannot be expressed. No `borderColor`, `borderWidth`, `hoverRadius`. |
| Mixed / combo | **Partial** | Set `Dataset.Type` per dataset, e.g. `Type = "line"` on a dataset inside a `BarChartConfig`. Every dataset must still be the config's own dataset type, so that "line" is a `BarDataset` and cannot use `Tension`, `PointStyle`, `PointRadius`, `StepMode`, `YAxisId` or any other line-only option. `ScatterDataset` and `BubbleDataset` have no `Type`, so they cannot join a mixed chart. |

## Scales

| Feature | Status | Notes |
| --- | --- | --- |
| `linear` | **Partial** | `Axis` covers `Min`, `Max`, `SuggestedMin`, `SuggestedMax`, `BeginAtZero`, `Display`, `Position`, `Type`, `Grid`, `Border`, `Ticks`, `Title`. No `grace`, `offset`, `reverse`, `alignToPixels`, `bounds`, `weight`, `clip`, `stack`/`stackWeight`. |
| `logarithmic` | **Partial** | Selected with `Axis.Type = "logarithmic"`; same `Axis` surface as linear. Tick number formatting is not exposed, so log labels keep Chart.js defaults. |
| `category` | **Partial** | `Axis.Type = "category"`. The scale's own `labels` array is not exposed, and `Axis.Min`/`Max` are `double?`, so bounding a category axis by label string is not possible. |
| `time` | **Partial** | `Axis.Time` gives `Unit`, `MinUnit`, `Round`, `IsoWeekday`, `TooltipFormat` and `DisplayFormats`, and tick generation is `Axis.Ticks.Source` (`"auto"`, `"data"`, `"labels"`), the key Chart.js actually reads. `time.parser` is missing, so a custom input format cannot be declared — timestamps have to arrive in a form the moment adapter parses unaided (ISO 8601 strings, epoch numbers). Needs the moment and moment-adapter script tags. |
| `timeseries` | **Partial** | `Axis.Type = "timeseries"`; identical surface and identical caveats to `time`. |
| `radialLinear` | **Partial** | Polar area reaches it through `Options.Scales["r"]` (a full `Axis`); radar only through `RadarOptions.Scales.R` (`BeginAtZero`, `Min`, `Max`). `angleLines`, `pointLabels`, `startAngle` and `ticks.backdropColor` are missing on both. |
| Stacking | **Partial** | `Axis.Stacked` (bool) plus `BarDataset.Stack` and `LineDataset.Stack` for stack groups. `stacked: 'single'` is not expressible, and no other dataset has a `Stack`. |
| Multiple axes | **Partial** | Any number of entries in `Options.Scales`. Only `LineDataset` and `ScatterDataset` can bind to one: `LineDataset` via `XAxisId` and `YAxisId`, `ScatterDataset` via `YAxisId` only. No axis id on bar, pie, doughnut, polar or radar datasets. |
| Axis position | **Partial** | `Axis.Position` offers top, left, bottom, right; `Axis.PositionString` takes any string, so `"center"` also works. The object form (`position: { y: 0 }`, an axis pinned to a value on another scale) cannot be expressed. |
| Grid lines | **Partial** | `Grid` has `Color`, `Display`, `DrawOnChartArea`, `DrawTicks`. No `lineWidth`, `offset`, `tickColor`, `tickLength`, `tickWidth`, `tickBorderDash`, `z`, `circular`. |
| Axis border | **Full** | `Axis.Border` covers the whole Chart.js 4 `scales[].border` object: `Display`, `Width`, `Color`, `Dash`, `DashOffset`, `Z`. |
| Ticks | **Partial** | `Color`, `Font`, `AutoSkip`, `MaxTicksLimit`, `StepSize`, `MinRotation`, `MaxRotation`, `CrossAlign`, `Source` (time scales only), `Callback`/`CallbackAsync`. No `display`, `align`, `padding`, `precision`, `count`, `includeBounds`, `labelOffset`, `mirror`, `sampleSize`, `autoSkipPadding`, `backdropColor`, `textStrokeColor`/`textStrokeWidth`, `z`, or the `Intl.NumberFormat` `format` object. |
| Axis title | **Partial** | `AxesTitle` has `Text`, `Display`, `Align`, `Color`, `Font`. `padding` is missing. |

## Built-in plugins

| Plugin | Status | Notes |
| --- | --- | --- |
| `legend` | **Partial** | `Display`, `Position`, `Align`, `Reverse`, `RTL`, `TextDirection`, `FullSize` and a click handler. `Labels` covers `Color`, `Font`, `BoxWidth`, `BoxHeight`, `Padding`, `UsePointStyle`, `PointStyle`/`PointStyleWidth`, `TextAlign`, `UseBorderRadius`, `BorderRadius` and `Filter` — the whole `legend.labels` object bar `generateLabels` and `sort`. See [Legend label styling](styling.md#legend-label-styling). On the legend itself, `maxWidth`/`maxHeight`, `title`, `onHover` and `onLeave` are still missing. |
| `title` | **Partial** | `Display`, `Text`, `Position`, `Align`, `Color`, `Font`, `Padding`, `FullSize`. `Padding` is a `TitlePadding` carrying `Top` and `Bottom`, which is the whole of what Chart.js reads here — the type omits `Left`/`Right` because the title box discards them. `Text` is a single `string`, so a multi-line title (Chart.js accepts `string[]`) is not possible. |
| `subtitle` | **None** | No property for `plugins.subtitle`. A second `Title` cannot stand in for it — there is only one `title` slot. |
| `tooltip` | **Partial** | Colours and fonts: `BackgroundColor`, `TitleColor`/`TitleFont`, `BodyColor`/`BodyFont`, `FooterColor`/`FooterFont`, `BorderColor`, `BorderWidth`, `MultiKeyBackground`, plus `Callbacks.Label` and `Callbacks.Title`. Layout and behaviour are still unreachable: `enabled`, `mode`, `intersect`, `position`, `padding`/`caretPadding`/`caretSize`/`boxPadding`, `cornerRadius`, `displayColors`, `boxWidth`/`boxHeight`, `usePointStyle`, the `*Align` and `*Spacing` options, `external`, `filter`, `itemSort`, `rtl`/`textDirection`, and the other dozen callbacks. |
| `filler` | **Partial** | Reachable only as the `bool?` `Fill` on a line or radar dataset — on, explicitly off, or left to the type's default. `plugins.filler` itself (`propagate`, `drawTime`) has no property. |
| `decimation` | **None** | No property for `plugins.decimation`, so LTTB and min/max decimation of large series are unavailable. |
| `colors` | **None** | No property for `plugins.colors`. The plugin ships enabled in Chart.js 4 and will colour any dataset that defines none, and it can neither be configured nor switched off from C#. Set `BackgroundColor`/`BorderColor` on every dataset to keep it out of the way. |

## Bundled third-party plugins

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

## Options families

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
| `elements` | **Partial** | `Options.Elements` has `Line.BorderColor` and `Line.BorderWidth` and nothing else. `elements.point`, `elements.bar`, `elements.arc` and the rest of `elements.line` — `tension`, `fill`, `borderDash`, `stepped`, `spanGaps`, `capBezierPoints`, `cubicInterpolationMode`, the border cap and join styles — are not exposed chart-wide, though `LineDataset` has every one of them except `capBezierPoints` per dataset. `RadarOptions.Elements.Line` has only `BorderWidth` (`int?`, default `3`; `0` hides the radar outline). |
| Per-dataset defaults | **None** | No property for `options.datasets.<type>`, so defaults shared by every dataset of a type must be repeated on each dataset. |
| `parsing` | **None** | Neither `options.parsing` nor `dataset.parsing` has a property, so data has to arrive in the shape the controller expects. (`Models/Common/Parsing.cs` exists but nothing references it.) |
| `locale` | **Full** | `Options.Locale` sets `options.locale` and is propagated to the moment adapter for time scales. |
| `indexAxis` | **Full** | `Options.IndexAxis` with the `Axes.X` / `Axes.Y` / `Axes.Default` constants. |
| Grouped axis labels | extra | `Options.GroupXAxis` / `GroupYAxis` are a feature of this package, not of Chart.js: they add a second category axis and split labels on `;`. |

## Callbacks and events

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

## Escape hatches

There is no general one. The models have no `[JsonExtensionData]` bag anywhere, so arbitrary keys cannot be added to `options`, `scales`, `plugins` or a dataset. What does exist:

- **`Plugins.Annotation.Annotations`** is a `Dictionary<string, object>`, and the objects in it are serialized verbatim. This is the one place raw Chart.js JSON is accepted by design, and it is the whole reason every annotation type works.
- **`Chart.AddDataset<T>(T dataset)`** takes any `class` and serializes it as-is onto `chart.data.datasets`, so a dataset with options the typed classes do not model can be pushed at runtime — after the first render, and without the config object knowing about it. This is the practical way out of a dataset-level gap.
- **`Chart.Config` is `IChartConfig` and `IOptions` is an empty marker interface**, so you can supply your own config and options classes and emit whatever `options` JSON you like. The cost is that the C# callback plumbing checks `Options is Options`: with a custom options type, `Options.OnClickAsync`, `OnHoverAsync`, the tooltip and tick callbacks and the legend filter all stop firing. (This is exactly why `RadarOptions` has none of them.) The `OnChartClick`, `OnChartOver` and `OnLegendClick` component parameters keep working.

What is *not* an escape hatch: **subclassing a typed dataset**. `Data<T>.Datasets` is a `List<T>` of the concrete type, and `System.Text.Json` serializes those elements by their declared type, so properties added on a derived class are silently dropped. `CustomDataset<T>` is likewise not a general-purpose container — it is the base for `ScatterDataset` and `BubbleDataset` and carries only `label` and `data`.
