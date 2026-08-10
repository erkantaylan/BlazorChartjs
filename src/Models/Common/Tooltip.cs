namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// Tooltip
    /// </summary>
    public sealed class Tooltip
    {
        //https://www.chartjs.org/docs/latest/configuration/tooltip.html

        /// <summary>
        /// Gets or sets the callbacks.
        /// </summary>
        /// <value>
        /// The callbacks.
        /// </value>
        [JsonPropertyName("callbacks")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Callbacks? Callbacks { get; set; }

        /// <summary>
        /// Gets or sets the background color of the tooltip.
        /// </summary>
        /// <value>
        ///   <para>Any CSS color string. The Chart.js default is <c>"rgba(0, 0, 0, 0.8)"</c>.</para>
        ///   <para>Maps to <c>options.plugins.tooltip.backgroundColor</c>.</para>
        /// </value>
        [JsonPropertyName("backgroundColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the tooltip title text.
        /// </summary>
        /// <value>
        /// Any CSS color string. The Chart.js default is <c>"#fff"</c>.
        /// </value>
        [JsonPropertyName("titleColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TitleColor { get; set; }

        /// <summary>
        /// Gets or sets the font of the tooltip title text.
        /// </summary>
        /// <value>
        /// The font. Chart.js defaults to the chart-wide font with <c>weight: "bold"</c>.
        /// </value>
        [JsonPropertyName("titleFont")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Font? TitleFont { get; set; }

        /// <summary>
        /// Gets or sets the color of the tooltip body text.
        /// </summary>
        /// <value>
        /// Any CSS color string. The Chart.js default is <c>"#fff"</c>.
        /// </value>
        [JsonPropertyName("bodyColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BodyColor { get; set; }

        /// <summary>
        /// Gets or sets the font of the tooltip body text.
        /// </summary>
        /// <value>
        /// The font. Chart.js defaults to the chart-wide font.
        /// </value>
        [JsonPropertyName("bodyFont")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Font? BodyFont { get; set; }

        /// <summary>
        /// Gets or sets the color of the tooltip footer text.
        /// </summary>
        /// <value>
        /// Any CSS color string. The Chart.js default is <c>"#fff"</c>.
        /// </value>
        [JsonPropertyName("footerColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? FooterColor { get; set; }

        /// <summary>
        /// Gets or sets the font of the tooltip footer text.
        /// </summary>
        /// <value>
        /// The font. Chart.js defaults to the chart-wide font with <c>weight: "bold"</c>.
        /// </value>
        [JsonPropertyName("footerFont")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Font? FooterFont { get; set; }

        /// <summary>
        /// Gets or sets the color drawn behind the colored swatch of each tooltip item.
        /// </summary>
        /// <value>
        ///   <para>Any CSS color string. The Chart.js default is <c>"#fff"</c>.</para>
        ///   <para>It is only visible when <c>displayColors</c> is left on and the swatch
        ///   color is translucent.</para>
        /// </value>
        [JsonPropertyName("multiKeyBackground")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? MultiKeyBackground { get; set; }

        /// <summary>
        /// Gets or sets the color of the tooltip border.
        /// </summary>
        /// <value>
        ///   <para>Any CSS color string. The Chart.js default is <c>"rgba(0, 0, 0, 0)"</c> (transparent).</para>
        ///   <para>It is only visible when <see cref="BorderWidth"/> is greater than <c>0</c>.</para>
        /// </value>
        [JsonPropertyName("borderColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BorderColor { get; set; }

        /// <summary>
        /// Gets or sets the width of the tooltip border.
        /// </summary>
        /// <value>
        /// The border width in pixels. The Chart.js default is <c>0</c>.
        /// </value>
        [JsonPropertyName("borderWidth")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BorderWidth { get; set; }
    }
}
