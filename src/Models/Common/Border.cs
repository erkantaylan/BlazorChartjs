namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// Axis border. In Chart.js 3 these options lived under <c>grid</c>
    /// (<c>grid.drawBorder</c>, <c>grid.borderWidth</c>, ...); since Chart.js 4 they are
    /// a scale option of their own.
    /// </summary>
    public class Border
    {
        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        /// <value>
        /// The color of the border.
        /// </value>
        [JsonPropertyName("color")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Color { get; set; }

        /// <summary>
        /// Gets or sets the line dash pattern of the border.
        /// </summary>
        /// <value>
        /// The lengths of alternating dashes and gaps, in pixels.
        /// </value>
        [JsonPropertyName("dash")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<int>? Dash { get; set; }

        /// <summary>
        /// Gets or sets the offset of the line dash pattern.
        /// </summary>
        /// <value>
        /// The offset, in pixels.
        /// </value>
        [JsonPropertyName("dashOffset")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? DashOffset { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the border is drawn.
        /// This replaces the Chart.js 3 option <c>grid.drawBorder</c>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the border is drawn; otherwise, <c>false</c>.
        /// </value>
        [JsonPropertyName("display")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Display { get; set; }

        /// <summary>
        /// Gets or sets the width of the border.
        /// </summary>
        /// <value>
        /// The width, in pixels.
        /// </value>
        [JsonPropertyName("width")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Width { get; set; }

        /// <summary>
        /// Gets or sets the z index of the border.
        /// </summary>
        /// <value>
        /// A value under 0 draws the border beneath the datasets, a value over 0 above them.
        /// </value>
        [JsonPropertyName("z")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Z { get; set; }
    }
}
