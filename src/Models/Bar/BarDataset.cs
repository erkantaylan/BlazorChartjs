using Erkan.Blazor.Chartjs.Models.Common.StringEnums;

namespace Erkan.Blazor.Chartjs.Models.Bar
{
    /// <summary>
    /// Bar Dataset
    /// </summary>
    /// <seealso cref="Erkan.Blazor.Chartjs.Models.Common.Dataset" />
    public class BarDataset : Dataset
    {
        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>
        /// One colour per bar, cycled when there are more bars than colours.
        /// <c>null</c> - the default - writes no <c>backgroundColor</c> at all and leaves
        /// the Chart.js default in place.
        /// </value>
        [JsonPropertyName("backgroundColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets how much of its slot in the category each bar fills.
        /// </summary>
        /// <value>
        /// A fraction: Chart.js's default is <c>0.9</c>, and <c>1</c> makes the bars of a group touch.
        /// Ignored when <see cref="BarThickness"/> is a fixed width.
        /// </value>
        [JsonPropertyName("barPercentage")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? BarPercentage { get; set; }

        /// <summary>
        /// Gets or sets the width of each bar.
        /// </summary>
        /// <value>
        /// A fixed width in pixels (<c>BarThickness = 12</c>), which ignores <see cref="BarPercentage"/>
        /// and <see cref="CategoryPercentage"/>, or <see cref="Common.BarThickness.Flex"/>. <c>null</c>
        /// sizes bars from the smallest gap between categories.
        /// </value>
        [JsonPropertyName("barThickness")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public BarThickness? BarThickness { get; set; }

        /// <summary>
        /// Gets or sets the value the bars are drawn from.
        /// </summary>
        /// <value>
        /// A value on the value axis, in data units: with <c>Base = 50</c> a bar runs from 50 to its
        /// value, downwards when the value is lower. <c>null</c> draws from zero, or from the top of
        /// the bar below in a stack. Floating bars ignore it.
        /// </value>
        [JsonPropertyName("base")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? Base { get; set; }

        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        /// <value>
        /// One colour per bar, cycled when there are more bars than colours.
        /// <c>null</c> - the default - writes no <c>borderColor</c> at all and leaves
        /// the Chart.js default in place. An empty list used to be sent instead, which
        /// put <c>"borderColor": []</c> on every untouched bar chart.
        /// </value>
        [JsonPropertyName("borderColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? BorderColor { get; set; }

        /// <summary>
        /// Gets or sets the rounding of the bar corners.
        /// </summary>
        /// <value>
        /// <c>BorderRadius = 8</c> rounds every corner, <c>new BorderRadius { TopLeft = 8, TopRight = 8 }</c>
        /// only the ones named. Corners on the edge <see cref="BorderSkipped"/> skips stay square, and
        /// by default that is the edge the bar grows from. <c>0</c> is written, not dropped.
        /// </value>
        [JsonPropertyName("borderRadius")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public BorderRadius? BorderRadius { get; set; }

        /// <summary>
        /// Gets or sets the edge of each bar drawn without a border.
        /// </summary>
        /// <value>The skipped edge; <c>null</c> leaves the Chart.js default (<c>start</c>) in place.</value>
        [JsonIgnore]
        public BorderSkipped? BorderSkipped
        {
            get => _borderSkipped;
            set
            {
                _borderSkipped = value;
                BorderSkippedString = value?.Value;
            }
        }
        private BorderSkipped? _borderSkipped;

        /// <summary>
        /// Gets or sets the border skipped string.
        /// </summary>
        /// <value>
        /// The border skipped string. <c>"true"</c> and <c>"false"</c> are written as JSON booleans,
        /// because Chart.js tests <c>borderSkipped</c> for truthiness; anything else as a string.
        /// </value>
        [JsonPropertyName("borderSkipped")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonConverter(typeof(BooleanStringJsonConverter))]
        public string? BorderSkippedString { get; set; }

        /// <summary>
        /// Gets or sets the width of the border.
        /// </summary>
        /// <value>
        /// The width of the border.
        /// </value>
        [JsonPropertyName("borderWidth")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BorderWidth { get; set; }

        /// <summary>
        /// Gets or sets how much of each category the group of bars fills.
        /// </summary>
        /// <value>
        /// A fraction: Chart.js's default is <c>0.8</c>, and <c>1</c> leaves no gap between
        /// categories. Ignored when <see cref="BarThickness"/> is a fixed width.
        /// </value>
        [JsonPropertyName("categoryPercentage")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? CategoryPercentage { get; set; }

        /// <summary>
        /// Gets or sets how far outside the chart area the dataset may draw.
        /// </summary>
        /// <value>
        /// Pixels on every side: a positive value lets a bar that runs past the axis bounds draw
        /// beyond the chart area, a negative one clips inside it. Chart.js's <c>false</c> and
        /// per-side object forms are not exposed.
        /// </value>
        [JsonPropertyName("clip")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Clip { get; set; }

        /// <summary>
        /// Gets or sets whether the bars take a slot beside the other datasets' bars in each category.
        /// </summary>
        /// <value>
        /// <c>false</c> centres this dataset's bars on the category, over the other datasets', at
        /// the width a whole group would have; <c>null</c> leaves the Chart.js default (<c>true</c>)
        /// in place.
        /// </value>
        [JsonPropertyName("grouped")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Grouped { get; set; }

        /// <summary>
        /// Gets or sets the background color hover.
        /// </summary>
        /// <value>
        /// The background color hover.
        /// </value>
        [JsonPropertyName("hoverBackgroundColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? HoverBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the border color when hovered.
        /// </summary>
        /// <value>
        /// One colour per bar, cycled when there are more bars than colours.
        /// </value>
        [JsonPropertyName("hoverBorderColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? HoverBorderColor { get; set; }

        /// <summary>
        /// Gets or sets the rounding of the bar corners when hovered.
        /// </summary>
        /// <value>
        /// The same shapes as <see cref="BorderRadius"/>. <c>null</c> keeps <see cref="BorderRadius"/>
        /// on hover.
        /// </value>
        [JsonPropertyName("hoverBorderRadius")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public BorderRadius? HoverBorderRadius { get; set; }

        /// <summary>
        /// Gets or sets the border width when hovered.
        /// </summary>
        /// <value>
        /// The hover border width, in pixels. <c>0</c> is written, not dropped.
        /// </value>
        [JsonPropertyName("hoverBorderWidth")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? HoverBorderWidth { get; set; }

        /// <summary>
        /// Gets or sets the index axis of this dataset.
        /// </summary>
        /// <value>
        /// <see cref="Enums.Axes.Y"/> draws this dataset's bars horizontally even when the chart's
        /// <c>Options.IndexAxis</c> does not; <c>null</c> follows the chart.
        /// </value>
        [JsonPropertyName("indexAxis")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? IndexAxis { get; set; }

        /// <summary>
        /// Gets or sets how far each bar is drawn beyond its own edges.
        /// </summary>
        /// <value>
        /// Pixels (<c>InflateAmount = 0</c> draws bars at their exact size), or
        /// <see cref="Common.InflateAmount.Auto"/>, the Chart.js default.
        /// </value>
        [JsonPropertyName("inflateAmount")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public InflateAmount? InflateAmount { get; set; }

        /// <summary>
        /// Gets or sets the widest a bar may be drawn.
        /// </summary>
        /// <value>
        /// The maximum width, in pixels, however much room the category has.
        /// </value>
        [JsonPropertyName("maxBarThickness")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MaxBarThickness { get; set; }

        /// <summary>
        /// Gets or sets the shortest a bar may be drawn.
        /// </summary>
        /// <value>
        /// The minimum length, in pixels, so that a value close to the base still shows.
        /// </value>
        [JsonPropertyName("minBarLength")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MinBarLength { get; set; }

        /// <summary>
        /// Gets or sets the point style.
        /// </summary>
        /// <value>
        /// The shape of this dataset's legend entry when <c>Legend.Labels.UsePointStyle</c> is set.
        /// </value>
        [JsonIgnore]
        public PointStyle? PointStyle
        {
            get => _pointStyle;
            set
            {
                _pointStyle = value;
                PointStyleString = value?.Value;
            }
        }
        private PointStyle? _pointStyle;

        /// <summary>
        /// Gets or sets the point style string.
        /// </summary>
        /// <value>
        /// The point style string.
        /// </value>
        [JsonPropertyName("pointStyle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PointStyleString { get; set; }

        /// <summary>
        /// Gets or sets whether a <c>null</c> value keeps its slot in the group.
        /// </summary>
        /// <value>
        /// <c>true</c> closes the gap: in a category where this dataset has no value, the other
        /// datasets' bars share the room. <c>null</c> leaves the Chart.js default (<c>false</c>) in
        /// place.
        /// </value>
        [JsonPropertyName("skipNull")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? SkipNull { get; set; }

        /// <summary>
        /// Gets or sets the stack.
        /// </summary>
        /// <value>
        /// The identifier of the group this dataset belongs to. Datasets sharing the same
        /// value are stacked together; Chart.js compares stack identifiers by value, so
        /// this has to be a plain string rather than a collection.
        /// </value>
        [JsonPropertyName("stack")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Stack { get; set; }

        /// <summary>
        /// Gets or sets the x axis identifier.
        /// </summary>
        /// <value>
        /// The key of the x axis in <c>Options.Scales</c> this dataset is drawn against.
        /// </value>
        [JsonPropertyName("xAxisID")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? XAxisId { get; set; }

        /// <summary>
        /// Gets or sets the y axis identifier.
        /// </summary>
        /// <value>
        /// The key of the y axis in <c>Options.Scales</c> this dataset is drawn against.
        /// </value>
        [JsonPropertyName("yAxisID")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? YAxisId { get; set; }
    }
}
