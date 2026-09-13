using Microsoft.AspNetCore.Components.Web;
using System.Linq;
using System.Threading.Tasks;

namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// Options
    /// </summary>
    public class Options : IOptions
    {
        #region Events

        /// <summary>
        /// Gets or sets the on chart click.
        /// </summary>
        /// <value>The on chart click.</value>
        [JsonIgnore]
        public Func<CallbackGenericContext, ValueTask>? OnClickAsync { get; set; }

        /// <summary>
        /// Gets or sets the on hover asynchronous.
        /// </summary>
        /// <value>
        /// The on hover asynchronous.
        /// </value>
        [JsonIgnore]
        public Func<HoverContext, ValueTask>? OnHoverAsync { get; set; }

        /// <summary>
        /// Gets or sets the on mouse out asynchronous.
        /// </summary>
        /// <value>
        /// The on mouse out asynchronous.
        /// </value>
        [JsonIgnore]
        public Func<MouseEventArgs, ValueTask>? OnMouseOutAsync { get; set; }

        #endregion Events

        /// <summary>
        /// Gets or sets the animations.
        /// </summary>
        /// <value>
        /// Enables/disables all animations
        /// </value>
        [JsonPropertyName("animation")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Animation { get; set; }

        /// <summary>
        /// Gets or sets the animations.
        /// </summary>
        /// <value>
        /// The animations.
        /// </value>
        [JsonPropertyName("animations")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Animations? Animations { get; set; }

        /// <summary>
        /// Gets or sets the canvas aspect ratio (<c>width / height</c>).
        /// </summary>
        /// <value>
        /// The ratio Chart.js keeps the canvas at. Only read when <see cref="MaintainAspectRatio"/> is
        /// <c>true</c>; Chart.js defaults it to <c>2</c>, and to <c>1</c> for radial charts.
        /// </value>
        [JsonPropertyName("aspectRatio")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? AspectRatio { get; set; }

        /// <summary>
        /// Gets or sets the chart-wide default background colour.
        /// </summary>
        /// <value>
        /// The fill colour every bar, point, line area and arc falls back to when neither its dataset
        /// nor <see cref="Elements"/> sets one. Grid lines do not use it. Setting it also switches off
        /// the built-in colors plugin unless <see cref="Colors.ForceOverride"/> is <c>true</c>.
        /// </value>
        [JsonPropertyName("backgroundColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the chart-wide default border colour.
        /// </summary>
        /// <value>
        /// The border colour every bar, point, line and arc falls back to when neither its dataset nor
        /// <see cref="Elements"/> sets one. Grid lines do not use it. Setting it also switches off the
        /// built-in colors plugin unless <see cref="Colors.ForceOverride"/> is <c>true</c>.
        /// </value>
        [JsonPropertyName("borderColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BorderColor { get; set; }

        /// <summary>
        /// Gets or sets the chart's default text colour.
        /// </summary>
        /// <value>
        /// In Chart.js 4.5.1 this per-chart value is the fallback for legend label text only. The
        /// title, tick labels, axis titles and datalabels fall back to the page-wide
        /// <c>Chart.defaults.color</c> (<c>#666</c>) instead, so set <c>Title.Color</c>,
        /// <c>Ticks.Color</c>, <c>AxesTitle.Color</c> and <c>DataLabels.Color</c> on each.
        /// </value>
        [JsonPropertyName("color")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Color { get; set; }

        /// <summary>
        /// Gets or sets the device pixel ratio the canvas is rendered at.
        /// </summary>
        /// <value>
        /// Overrides <c>window.devicePixelRatio</c>, e.g. to render a sharper canvas for printing.
        /// Unset uses the browser's own ratio.
        /// </value>
        [JsonPropertyName("devicePixelRatio")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? DevicePixelRatio { get; set; }

        /// <summary>
        /// Gets or sets the elements.
        /// </summary>
        /// <value>
        /// The elements.
        /// </value>
        [JsonPropertyName("elements")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Elements? Elements { get; set; }

        /// <summary>
        /// Gets or sets the DOM events the chart listens to.
        /// </summary>
        /// <value>
        /// Event names such as <c>"mousemove"</c> or <c>"click"</c>. Chart.js defaults to
        /// <c>mousemove</c>, <c>mouseout</c>, <c>click</c>, <c>touchstart</c> and <c>touchmove</c>; an
        /// empty list is written as <c>[]</c> and stops the chart reacting to any of them. Hover
        /// callbacks only follow the pointer while <c>mousemove</c> is listed, and click callbacks
        /// need <c>click</c>.
        /// </value>
        [JsonPropertyName("events")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Events { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether there are groups for axes.
        /// </summary>
        /// <value><c>true</c> if there are groups for axes (the label should have a semicolumn (;) to divide the label to the name of the gorup); otherwise, <c>false</c>.</value>
        [JsonPropertyName("groupXAxis")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? GroupXAxis
        {
            get => _groupXAxis;
            set
            {
                _groupXAxis = value;

                if (Scales == null)
                    Scales = new Dictionary<string, Axis>();

                if (Scales.Keys.Where(k => k == "x").Count() == 0)
                    Scales.Add("x", new Axis() { Ticks = new Ticks() });
                if (Scales.Keys.Where(k => k == "xAxis2").Count() == 0)
                    Scales.Add("xAxis2", new Axis()
                    {
                        Type = "category",
                        Grid = new Grid()
                        {
                            DrawOnChartArea = false
                        },
                        Ticks = new Ticks()
                    });
            }
        }
        private bool? _groupXAxis;

        /// <summary>
        /// Gets or sets a value indicating whether [group y axis].
        /// </summary>
        /// <value><c>null</c> if [group y axis] contains no value, <c>true</c> if [group y axis]; otherwise, <c>false</c>.</value>
        [JsonPropertyName("groupYAxis")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? GroupYAxis
        {
            get => _groupYAxis;
            set
            {
                _groupYAxis = value;

                if (Scales == null)
                    Scales = new Dictionary<string, Axis>();

                if (Scales.Keys.Where(k => k == "y").Count() == 0)
                    Scales.Add("y", new Axis() { Ticks = new Ticks() });
                if (Scales.Keys.Where(k => k == "yAxis2").Count() == 0)
                    Scales.Add("yAxis2", new Axis()
                    {
                        Type = "category",
                        Grid = new Grid()
                        {
                            DrawOnChartArea = false
                        },
                        Ticks = new Ticks()
                    });
            }
        }
        private bool? _groupYAxis;

        /// <summary>
        /// Gets a value indicating whether this instance has on hover asynchronous.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has on hover asynchronous; otherwise, <c>false</c>.
        /// </value>
        [JsonInclude]
        [JsonPropertyName("hasOnHoverAsync")]
        public bool HasOnHoverAsync => OnHoverAsync != null;

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        [JsonIgnore]
        public string Height { get; set; }

        /// <summary>
        /// Gets or sets the hover options.
        /// </summary>
        /// <value>
        /// The same four settings as <see cref="Interaction"/>, applied to hover only. Any member left
        /// unset falls back to <see cref="Interaction"/>.
        /// </value>
        [JsonPropertyName("hover")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Interaction? Hover { get; set; }

        /// <summary>
        /// Gets or sets the index axis. <seealso cref="Axes"/>
        /// </summary>
        /// <value>
        /// X or Y
        /// </value>
        [JsonPropertyName("indexAxis")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string IndexAxis { get; set; } = Axes.Default;

        /// <summary>
        /// Gets or sets the interaction.
        /// </summary>
        /// <value>The interaction.</value>
        [JsonPropertyName("interaction")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Interaction? Interaction { get; set; }

        /// <summary>
        /// Gets or sets the layout options.
        /// </summary>
        /// <value>
        /// The padding around the chart area.
        /// </value>
        [JsonPropertyName("layout")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Layout? Layout { get; set; }

        /// <summary>
        /// Gets or sets the locale for the chart. This is passed to Chart.js as the
        /// <c>locale</c> option and propagated to date adapters for time-based axes.
        /// Use a BCP 47 language tag (e.g. "tr-TR" for Turkish, "de-DE" for German).
        /// </summary>
        /// <value>A BCP 47 locale string, or null to use the default locale.</value>
        [JsonPropertyName("locale")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Locale { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the canvas keeps <see cref="AspectRatio"/> when it
        /// resizes.
        /// </summary>
        /// <value>
        ///   <c>false</c> by default — unlike Chart.js, whose default is <c>true</c> — so the chart
        ///   fills the height given to the <c>&lt;Chart&gt;</c> component's <c>Height</c> parameter.
        ///   <c>true</c> sizes the height from the width instead. <c>null</c> writes nothing and hands
        ///   the decision back to Chart.js.
        /// </value>
        [JsonPropertyName("maintainAspectRatio")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? MaintainAspectRatio { get; set; } = false;

        /// <summary>
        /// Gets or sets the plugins.
        /// </summary>
        /// <value>
        /// The plugins.
        /// </value>
        [JsonPropertyName("plugins")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Plugins Plugins { get; set; } = new Plugins();

        /// <summary>
        /// Gets or sets the delay, in milliseconds, before a resize is applied.
        /// </summary>
        /// <value>
        /// Debounces resize updates. Chart.js defaults it to <c>0</c>, which resizes immediately.
        /// </value>
        [JsonPropertyName("resizeDelay")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ResizeDelay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Options"/> is responsive.
        /// </summary>
        /// <value>
        ///   <c>true</c> by default, so the canvas resizes with its container; <c>false</c> fixes its
        ///   size. <c>null</c> writes nothing and hands the decision back to Chart.js.
        /// </value>
        [JsonPropertyName("responsive")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Responsive { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating if you want to register the DataLabels plugin.
        /// If the Chart doesn't find the DataLabels script, this plugin won't be added to the chart
        /// </summary>
        /// <value><c>true</c> if you want to register the DataLabels plugin for ChartJs; otherwise, <c>false</c> (by default).</value>
        [JsonPropertyName("registerDataLabels")]
        public bool RegisterDataLabels { get; set; } = false;

        /// <summary>
        /// Gets or sets the scales.
        /// </summary>
        /// <value>
        /// The scales.
        /// </value>
        [JsonPropertyName("scales")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, Axis> Scales { get; set; }
    }
}