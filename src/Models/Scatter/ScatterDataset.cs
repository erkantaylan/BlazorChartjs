namespace Erkan.Blazor.Chartjs.Models.Scatter
{
    public class ScatterDataset : CustomDataset<ScatterXYValue>
    {
        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>
        /// The color of the background.
        /// </value>
        [JsonPropertyName("backgroundColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        /// <value>
        /// The color of the border.
        /// </value>
        [JsonPropertyName("borderColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string BorderColor { get; set; }

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
        /// Gets or sets the point hit radius.
        /// </summary>
        /// <value>
        /// The point hit radius.
        /// </value>
        [JsonPropertyName("pointHitRadius")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? PointHitRadius { get; set; }

        /// <summary>
        /// Gets or sets the point style.
        /// </summary>
        /// <value>
        /// The point style.
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
        /// Gets or sets the point radius.
        /// </summary>
        /// <value>
        /// The point radius.
        /// </value>
        [JsonPropertyName("pointRadius")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? PointRadius { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show line.
        /// </summary>
        /// <value>
        ///   <c>true</c> to join the points with a line; <c>false</c> to explicitly suppress it,
        ///   overriding a chart-level <c>showLine</c>; <c>null</c> leaves the inherited value
        ///   (Chart.js default <c>false</c> for scatter) in place.
        /// </value>
        [JsonPropertyName("showLine")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ShowLine { get; set; }

        /// <summary>
        /// Gets or sets the tension.
        /// </summary>
        /// <value>
        /// The tension. <c>0</c> draws straight segments, overriding a chart-level
        /// <c>elements.line.tension</c>; <c>null</c> leaves the inherited value in place.
        /// </value>
        [JsonPropertyName("tension")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? Tension { get; set; }

        /// <summary>
        /// Gets or sets the y axis identifier.
        /// </summary>
        /// <value>
        /// The y axis identifier.
        /// </value>
        [JsonPropertyName("yAxisID")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? YAxisId { get; set; }
    }
}