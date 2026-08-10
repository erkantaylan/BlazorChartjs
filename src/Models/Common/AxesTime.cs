namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// Class AxesTime. This class cannot be inherited.
    /// </summary>
    public sealed class AxesTime
    {
        /// <summary>
        /// Get or set the display formats.
        /// </summary>
        /// <value>
        /// The display formats.
        /// </value>
        [JsonPropertyName("displayFormats")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AxesTimeFormats? DisplayFormats { get; set; }

        /// <summary>
        /// The day the week starts on when the axis is rounded to weeks.
        /// </summary>
        /// <value>
        /// A day index: 0 = Sunday, 1 = Monday ... 6 = Saturday. Leave it null to keep
        /// the Chart.js default (no week rounding).
        /// </value>
        [JsonPropertyName("isoWeekday")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? IsoWeekday { get; set; }

        /// <summary>
        /// Get or set the round.
        /// </summary>
        /// <value>
        /// The round.
        /// </value>
        [JsonPropertyName("round")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Round { get; set; }

        /// <summary>
        /// The format string to use for the tooltip.
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        [JsonPropertyName("tooltipFormat")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TooltipFormat { get; set; }

        /// <summary>
        /// Force the unit to be a certain type.
        /// </summary>
        /// <value>
        /// The unit.
        /// </value>
        [JsonPropertyName("unit")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Unit { get; set; }

        /// <summary>
        /// The minimum display format to be used for a time unit.
        /// </summary>
        /// <value>
        /// The unit.
        /// </value>
        [JsonPropertyName("minUnit")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? MinUnit { get; set; }
    }
}
