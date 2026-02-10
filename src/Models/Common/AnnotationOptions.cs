namespace PSC.Blazor.Components.Chartjs.Models.Common
{
    /// <summary>
    /// Options for the chartjs-plugin-annotation plugin.
    /// </summary>
    public sealed class AnnotationOptions
    {
        /// <summary>
        /// Gets or sets the annotations dictionary.
        /// </summary>
        /// <value>
        /// The annotations.
        /// </value>
        [JsonPropertyName("annotations")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, object>? Annotations { get; set; }
    }
}
