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
        /// Gets or sets the width of the border.
        /// </summary>
        /// <value>
        /// The width of the border.
        /// </value>
        [JsonPropertyName("borderWidth")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BorderWidth { get; set; }

        /// <summary>
        /// Gets or sets the fill.
        /// </summary>
        /// <value>
        /// The fill.
        /// </value>
        [JsonPropertyName("fill")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Fill { get; set; }

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
    }
}