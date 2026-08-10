namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// Legend Label Configuration
    /// </summary>
    public class LegendLabels
    {
        //https://www.chartjs.org/docs/latest/configuration/legend.html#legend-label-configuration

        /// <summary>
        /// Gets a value indicating whether this instance has filter.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has filter; otherwise, <c>false</c>.
        /// </value>
        [JsonInclude]
        [JsonPropertyName("hasFilter")]
        public bool HasFilter => Filter != null;

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        [JsonIgnore]
        public Func<LegendFilterContext, bool?>? Filter { get; set; }

        /// <summary>
        /// Gets or sets the color of the legend label text.
        /// </summary>
        /// <value>
        ///   <para>Any CSS color string, for example <c>"#000"</c>, <c>"rgb(0, 0, 0)"</c> or <c>"black"</c>.</para>
        ///   <para>Leave it <c>null</c> to inherit the chart-wide <c>options.color</c>.</para>
        ///   <para>Maps to <c>options.plugins.legend.labels.color</c>.</para>
        /// </value>
        [JsonPropertyName("color")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Color { get; set; }

        /// <summary>
        /// Gets or sets the font of the legend label text.
        /// </summary>
        /// <value>
        /// The font. Leave it <c>null</c> to inherit the chart-wide <c>options.font</c>.
        /// </value>
        [JsonPropertyName("font")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Font? Font { get; set; }

        /// <summary>
        /// Gets or sets the width of the coloured box next to each legend label.
        /// </summary>
        /// <value>
        ///   <para>The box width in pixels. The Chart.js default is <c>40</c>.</para>
        ///   <para>If <see cref="UsePointStyle"/> is <c>true</c> this is ignored unless
        ///   <see cref="PointStyleWidth"/> is also set.</para>
        /// </value>
        [JsonPropertyName("boxWidth")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BoxWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the coloured box next to each legend label.
        /// </summary>
        /// <value>
        /// The box height in pixels. Chart.js falls back to the label font size when unset.
        /// </value>
        [JsonPropertyName("boxHeight")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BoxHeight { get; set; }

        /// <summary>
        /// Gets or sets the padding between each legend item.
        /// </summary>
        /// <value>
        /// The padding in pixels. The Chart.js default is <c>10</c>.
        /// </value>
        [JsonPropertyName("padding")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Padding { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the legend uses the dataset point style
        /// instead of a coloured box.
        /// </summary>
        /// <value>
        ///   <c>true</c> to draw the dataset point style; <c>false</c> to draw a box;
        ///   <c>null</c> to keep the Chart.js default (<c>false</c>).
        /// </value>
        [JsonPropertyName("usePointStyle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? UsePointStyle { get; set; }

        /// <summary>
        /// Gets or sets the width reserved for the point style when
        /// <see cref="UsePointStyle"/> is <c>true</c>.
        /// </summary>
        /// <value>
        /// The point style width in pixels. Falls back to <see cref="BoxWidth"/> when unset.
        /// </value>
        [JsonPropertyName("pointStyleWidth")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PointStyleWidth { get; set; }

        /// <summary>
        /// Gets or sets the point style drawn for each legend item. <seealso cref="Common.PointStyle"/>
        /// </summary>
        /// <value>
        /// The point style. Only used when <see cref="UsePointStyle"/> is <c>true</c>.
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
        /// Gets or sets the point style.
        /// </summary>
        /// <value>
        ///   <para>The point style as a raw string, so a value the <see cref="Common.PointStyle"/>
        ///   enumeration does not cover can still be passed through.</para>
        ///   <para>It accepts those values: <c>circle</c>, <c>cross</c>, <c>crossRot</c>, <c>dash</c>,
        ///   <c>line</c>, <c>rect</c>, <c>rectRounded</c>, <c>rectRot</c>, <c>star</c>, <c>triangle</c>,
        ///   and <c>false</c>.</para>
        /// </value>
        [JsonPropertyName("pointStyle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PointStyleString { get; set; }

        /// <summary>
        /// Gets or sets the horizontal alignment of the label text.
        /// </summary>
        /// <value>
        ///   <para>The text align. It accepts those values:</para>
        ///   <list type="bullet">
        ///     <item>left</item>
        ///     <item>center</item>
        ///     <item>right</item>
        ///   </list>
        /// </value>
        [JsonPropertyName("textAlign")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TextAlign { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the legend box uses the dataset
        /// <c>borderRadius</c> (or <see cref="BorderRadius"/>) for its corners.
        /// </summary>
        /// <value>
        ///   <c>true</c> to round the legend box corners; <c>null</c> to keep the
        ///   Chart.js default (<c>false</c>).
        /// </value>
        [JsonPropertyName("useBorderRadius")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? UseBorderRadius { get; set; }

        /// <summary>
        /// Gets or sets the corner radius of the legend box.
        /// </summary>
        /// <value>
        /// The border radius in pixels. Only applied when <see cref="UseBorderRadius"/> is <c>true</c>.
        /// </value>
        [JsonPropertyName("borderRadius")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BorderRadius { get; set; }
    }
}
