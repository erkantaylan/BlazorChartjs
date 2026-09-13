# Extra options

The typed models do not cover all of Chart.js — [Feature coverage](feature-coverage.md) lists what they miss. Until a property lands, you can use two escape hatches:

- **`ExtraOptions`**, a bag on each option model. Its entries go into the JSON object that model writes, next to the typed keys.
- **`RegisterPlugins`** on `Options` and `RadarOptions`, which attaches any Chart.js plugin the page has loaded to one chart. See [Any other Chart.js plugin](plugins.md#any-other-chartjs-plugin).

The [Extra options and plugins](https://erkantaylan.github.io/BlazorChartjs/extraOptions) demo page uses both.

## `ExtraOptions`

```csharp
var config = new BarChartConfig
{
    Options = new Options
    {
        // options.datasets.bar: defaults for every bar dataset on the chart
        ExtraOptions = new() { ["datasets"] = new { bar = new { categoryPercentage = 0.6 } } },
        Plugins = new Plugins
        {
            // options.plugins.subtitle
            ExtraOptions = new() { ["subtitle"] = new { display = true, text = "Q3" } },
            Legend = new Legend
            {
                Position = LegendPosition.Bottom,
                // options.plugins.legend.maxHeight
                ExtraOptions = new() { ["maxHeight"] = 60 },
            },
        },
        Scales = new()
        {
            // options.scales.y.grace
            ["y"] = new Axis { BeginAtZero = true, ExtraOptions = new() { ["grace"] = "5%" } },
        },
    },
};

config.Data.Datasets.Add(new BarDataset
{
    Label = "Revenue",
    Data = [10, 20, 30],
    // data.datasets[0].borderRadius
    ExtraOptions = new() { ["borderRadius"] = 6 },
});
```

The `options` object that serializes to, in full — the bag entries sit inside the object of the class that holds them, after that class's own keys:

```json
"options": {
  "hasOnHoverAsync": false,
  "maintainAspectRatio": false,
  "plugins": {
    "legend": { "display": true, "position": "bottom", "reverse": false, "hasLegendClick": false, "maxHeight": 60 },
    "subtitle": { "display": true, "text": "Q3" }
  },
  "responsive": true,
  "registerDataLabels": false,
  "scales": { "y": { "beginAtZero": true, "grace": "5%" } },
  "datasets": { "bar": { "categoryPercentage": 0.6 } }
}
```

(`hasOnHoverAsync`, `hasLegendClick` and `registerDataLabels` are read and removed by the component before Chart.js sees the configuration.)

### Where the bag is

| Class | Its entries are written into |
| --- | --- |
| `Options` (and `PieOptions`, which derives from it) | `options` |
| `RadarOptions` | `options` — a radar chart has no `Plugins` property, so its legend, title and tooltip go here as `["plugins"]` |
| `Plugins` | `options.plugins` |
| `Legend` | `options.plugins.legend` |
| `Title` | `options.plugins.title` |
| `Tooltip` | `options.plugins.tooltip` |
| `Axis` | `options.scales.<id>` |
| `Dataset` — bar, line, pie, doughnut, polar area and radar datasets | that dataset's entry in `data.datasets` |
| `CustomDataset` — scatter and bubble datasets | that dataset's entry in `data.datasets` |

Put an option in the bag of the class whose JSON object holds it. A legend option goes in `Legend.ExtraOptions`, not in `Plugins.ExtraOptions["legend"]`: `Options.Plugins` and `Plugins.Legend` are created by default, so that `legend` key is already written.

Classes without a bag (`LegendLabels`, `Ticks`, `Grid`, `Border`, `AxesTitle`, `Font`, `Layout`, `Padding`, `Colors`, `Interaction`, `Elements`, `Animations`, the zoom and datalabels classes) are reached from the level above. To get to `ticks.padding`, set `Axis.Ticks` to `null` and write the whole `ticks` object through `Axis.ExtraOptions`. That axis then loses the typed `Ticks` properties, including its tick callbacks. The same works for `Title.Text`: leave it unset, and `Title.ExtraOptions["text"] = new[] { "First line", "Second line" }` gives a multi-line title.

### How entries are written

- **The key is written exactly as you give it.** Use the Chart.js name: `maxHeight`, not `MaxHeight`.
- **The value is serialized by its runtime type**, with the same serializer and settings as the rest of the configuration (System.Text.Json with the Web defaults, which is what Blazor's JS interop uses):
  - members of an anonymous object or a class are camel-cased, so `new { AutoPadding = false }` writes `autoPadding`
  - a nested `Dictionary<string, object?>` keeps its keys as given
  - arrays and lists are written as arrays
  - a `null` value is written as `null`, which Chart.js does not treat the same as a missing key
- **Entries come after the model's own keys**, in the order the dictionary enumerates them (for a `Dictionary` nothing has been removed from, the order they were added).
- **A null or empty bag writes nothing.** Every bag starts out `null`.
- **A JavaScript function cannot be written.** The configuration travels as JSON, so the bag cannot reach callbacks, scriptable options or anything else Chart.js expects to be a function.

### Do not repeat a key the model already writes

A bag key must not be one the class already writes. System.Text.Json does not merge duplicate keys, so the JSON gets both:

```csharp
new Tooltip { BorderWidth = 1, ExtraOptions = new() { ["borderWidth"] = 2 } }
```

```json
{"borderWidth":1,"borderWidth":2}
```

Blazor turns that JSON into a JavaScript object with `JSON.parse`, which keeps the last copy of a duplicate key, so today Chart.js receives `2`. **This is not supported.** It relies on the order the serializer writes keys in, and nothing checks it. This repository's tests record the JSON above so that a change to it is noticed, and that is all they do.

To let the bag own a key the model has a property for:

- **The property is nullable** (most are): leave it `null`. `new Tooltip { ExtraOptions = new() { ["borderWidth"] = 2 } }` writes `{"borderWidth":2}`.
- **The property has a non-null default**: set it to `null` first. `Options.Plugins` and `Plugins.Legend` start out as new objects, `Options.Responsive` starts out `true`, `Options.MaintainAspectRatio` `false` and `Legend.Display` `true`, so all five keys are written unless you clear them.
- **The property is not nullable**, so its key is always written: use the property. That is `Responsive` and `MaintainAspectRatio` on `RadarOptions`, `Options.RegisterDataLabels` and `Legend.Reverse`.

When a later release adds a property for a key you set through the bag, move the value to the property. Otherwise it becomes a clash.

Some keys are read and removed by this component before Chart.js sees the configuration: `registerPlugins`, `registerDataLabels`, `crosshair`, `groupXAxis`, `groupYAxis` and the `has…` markers. They are not Chart.js options. Set them through their properties, never through a bag.

### What nobody checks for you

The bag is not validated. A misspelt key, or a real key at the wrong level, reaches Chart.js and is silently ignored. That is the defect this repository's test suite exists to catch in the typed models, and the bag is outside that net. Look up the key and where it nests in the [Chart.js 4.x documentation](https://www.chartjs.org/docs/4.5.1/) (or the plugin's own), and check the chart in a browser.
