namespace Erkan.Blazor.Chartjs.Models.Radar
{
    /// <summary>
    /// Radar Options Elements Line
    /// </summary>
    public class RadarOptionsElementsLine
    {
        /// <summary>
        /// Gets or sets the width of the border.
        /// </summary>
        /// <value>
        /// The width of the border, in pixels. Default: 3.
        /// <c>0</c> hides the radar outline; <c>null</c> omits the key and leaves the
        /// Chart.js default (3) in place.
        /// </value>
        [JsonPropertyName("borderWidth")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BorderWidth { get; set; } = 3;
    }
}
