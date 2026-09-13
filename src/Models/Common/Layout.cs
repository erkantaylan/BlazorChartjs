namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// Layout
    /// </summary>
    public class Layout
    {
        /// <summary>
        /// Gets or sets a value indicating whether Chart.js adds padding of its own.
        /// </summary>
        /// <value>
        ///   <c>true</c> by default in Chart.js, which pads the chart area so that points on its edge
        ///   are not clipped; <c>false</c> uses <see cref="Padding"/> alone.
        /// </value>
        [JsonPropertyName("autoPadding")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? AutoPadding { get; set; }

        /// <summary>
        /// Gets or sets the padding around the chart, in pixels.
        /// </summary>
        /// <value>
        /// The space between the edge of the canvas and everything drawn on it. <c>new Padding(20)</c>
        /// pads all four sides.
        /// </value>
        [JsonPropertyName("padding")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Padding? Padding { get; set; }
    }
}
