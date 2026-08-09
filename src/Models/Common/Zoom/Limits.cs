namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// Limits. Serialized as <c>plugins.zoom.limits</c>: the bounds zooming and panning
    /// are not allowed to move past, per axis.
    /// </summary>
    public class Limits
    {
        /// <summary>
        /// Gets or sets the limits for the x axis.
        /// </summary>
        /// <value>
        /// The limits applied to every scale on the x axis.
        /// </value>
        [JsonPropertyName("x")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ScaleLimits? X { get; set; }

        /// <summary>
        /// Gets or sets the limits for the y axis.
        /// </summary>
        /// <value>
        /// The limits applied to every scale on the y axis.
        /// </value>
        [JsonPropertyName("y")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ScaleLimits? Y { get; set; }
    }
}
