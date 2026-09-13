# Upgrading

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
| `OnAnimationComplete` on `BarChartConfig`, `BubbleChartConfig`, `DoughnutChartConfig`, `PieChartConfig`, `PolarChartConfig`, `RadarChartConfig` and `ScatterChartConfig` | `onAnimationComplete` at the root of the config object | Nothing yet — delete the assignment. Chart.js 4 has `options.animation.onComplete`, which this package does not expose ([`animation` row](feature-coverage.md#options-families)). |

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

The one most likely to matter: `RadarOptionsElementsLine.BorderWidth = 0` hides the radar outline, which was previously impossible — `0` was dropped and the outline came back at Chart.js's default width of `3`. Alongside it, `RadarDataset.Fill = false` turns off the fill radar puts there by default; `DataLabels.Offset = 0` sits a label on its anchor rather than the plugin's default `4` away from it, `DataLabels.Opacity = 0` hides one, and `Clamp = false` / `Clip = false` on a dataset override a chart-wide `Plugins.DataLabels` that set them; `Pan.Threshold = 0` starts a pan on the first pointer move rather than after 10 pixels, and `Wheel.Speed = 0` freezes wheel zoom. `Fill`, `Tension` and `ShowLine` on a line or scatter dataset agree with Chart.js's own defaults either way, so those only override something when the chart's options come from a custom options class ([Escape hatches](feature-coverage.md#escape-hatches)).

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

Assigning a whole list is unchanged, which is how the examples in the [README](../README.md#quick-start) and the demo do it. What breaks is calling `.Add()` on the property without assigning one first:

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

`chart.js` in the Chart.js 4.x distribution is an **ES module** and throws `Unexpected token 'export'` under a classic `<script>` tag. `chart.umd.js` is the build to use from a `<script>` tag. See [Installation](../README.md#installation) for the full list.

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
