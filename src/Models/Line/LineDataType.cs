namespace Erkan.Blazor.Chartjs.Models.Line
{
    /// <summary>
    /// Class LineDataType.
    /// </summary>
    public class LineDataType
    {
        /// <summary>
        /// Gets or sets the x value.
        /// </summary>
        /// <value>
        /// x value
        /// </value>
        [JsonPropertyName("x")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? X { get; set; }

        /// <summary>
        /// Gets or sets the x value.
        /// </summary>
        /// <value>
        /// x value
        /// </value>
        [JsonPropertyName("y")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Y { get; set; }
    }
}
