namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// Zoom options. Serialized as <c>plugins.zoom.zoom</c>, which is where
    /// chartjs-plugin-zoom reads the zoom direction and the wheel/drag/pinch switches.
    /// </summary>
    public class ZoomOptions
    {
        /// <summary>
        /// Gets or sets the drag.
        /// </summary>
        /// <value>
        /// The drag.
        /// </value>
        [JsonPropertyName("drag")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Drag? Drag { get; set; }

        /// <summary>
        /// Gets or sets the zoom direction.
        /// </summary>
        /// <value>
        /// The mode. Values: x, y, xy. Defaults to xy when not set.
        /// </value>
        [JsonPropertyName("mode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Mode { get; set; }

        /// <summary>
        /// Gets or sets the over scale mode.
        /// </summary>
        /// <value>
        /// The over scale mode. Values: x, y, xy. Deprecated by the plugin in favour of
        /// <see cref="ScaleMode"/>.
        /// </value>
        [JsonPropertyName("overScaleMode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? OverScaleMode { get; set; }

        /// <summary>
        /// Gets or sets the pinch.
        /// </summary>
        /// <value>
        /// The pinch.
        /// </value>
        [JsonPropertyName("pinch")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Pinch? Pinch { get; set; }

        /// <summary>
        /// Gets or sets the scale mode.
        /// </summary>
        /// <value>
        /// Which axes are zoomed when the pointer is over a scale rather than over the
        /// chart area. Values: x, y, xy.
        /// </value>
        [JsonPropertyName("scaleMode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ScaleMode { get; set; }

        /// <summary>
        /// Gets or sets the wheel.
        /// </summary>
        /// <value>
        /// The wheel.
        /// </value>
        [JsonPropertyName("wheel")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Wheel? Wheel { get; set; }
    }
}
