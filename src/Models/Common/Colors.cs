namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// Options for Chart.js's built-in <c>colors</c> plugin, which gives datasets that set no colours
    /// a default palette.
    /// </summary>
    public class Colors
    {

        /// <summary>
        /// Enable or disable the default palette.
        /// </summary>
        /// <value>
        /// <c>true</c> by default in Chart.js; <c>false</c> leaves uncoloured datasets in the grey
        /// <c>rgba(0,0,0,0.1)</c> default.
        /// </value>
        [JsonPropertyName("enabled")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Enabled { get; set; }

        /// <summary>
        /// Apply the palette even when colours are already set.
        /// </summary>
        /// <value>
        /// <c>false</c> by default in Chart.js, and the plugin then colours nothing at all as soon as any
        /// dataset, <see cref="Options.Elements"/>, <see cref="Options.BackgroundColor"/> or
        /// <see cref="Options.BorderColor"/> sets a colour. <c>true</c> replaces every dataset's
        /// colours with the palette.
        /// </value>
        [JsonPropertyName("forceOverride")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ForceOverride { get; set; }
    }
}
