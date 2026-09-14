namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// The title drawn above a legend's items, written to <c>options.plugins.legend.title</c>.
    /// </summary>
    /// <remarks>
    /// Not the chart's <see cref="Common.Title"/>: the legend title is part of the legend box, has
    /// no <c>align</c> or <c>fullSize</c>, and takes a single line of text, because Chart.js reserves
    /// the height of exactly one line for it.
    /// </remarks>
    public class LegendTitle
    {
        private Align? _position;

        /// <summary>
        /// Gets or sets the colour of the title text.
        /// </summary>
        /// <value>
        /// A CSS colour. Unset, Chart.js uses <see cref="Options.Color"/> when the chart sets one, and
        /// the page-wide default otherwise.
        /// </value>
        [JsonPropertyName("color")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Color { get; set; }

        /// <summary>
        /// Gets or sets whether the title is shown.
        /// </summary>
        /// <value>
        /// <c>true</c> to draw the title. Chart.js's default is <c>false</c>, so a legend title with
        /// <see cref="Text"/> and no <c>Display = true</c> is not drawn.
        /// </value>
        [JsonPropertyName("display")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Display { get; set; }

        /// <summary>
        /// Gets or sets the font of the title text.
        /// </summary>
        /// <value>
        /// The font.
        /// </value>
        [JsonPropertyName("font")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Font? Font { get; set; }

        /// <summary>
        /// Gets or sets the padding above and below the title.
        /// </summary>
        /// <value>
        /// The padding. Chart.js reads only the vertical pair here, as it does for the chart title,
        /// so this is <see cref="TitlePadding"/> rather than the four-sided <see cref="Common.Padding"/>.
        /// </value>
        [JsonPropertyName("padding")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TitlePadding? Padding { get; set; }

        /// <summary>
        /// Gets or sets where the title sits across the width of the legend.
        /// </summary>
        /// <value>
        /// <see cref="Align.Start"/>, <see cref="Align.Center"/> (Chart.js's default) or
        /// <see cref="Align.End"/>. Chart.js names this key <c>position</c>, but it is an alignment, not
        /// a side of the chart: the legend itself is placed with <see cref="Legend.Position"/>.
        /// </value>
        [JsonIgnore]
        public Align? Position
        {
            get => _position;
            set
            {
                _position = value;
                PositionString = value?.Value;
            }
        }

        /// <summary>
        /// Gets or sets the position string.
        /// </summary>
        /// <value>
        /// <c>start</c>, <c>center</c> or <c>end</c>. Set by <see cref="Position"/>.
        /// </value>
        [JsonPropertyName("position")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PositionString { get; set; }

        /// <summary>
        /// Gets or sets the title text.
        /// </summary>
        /// <value>
        /// The text, drawn on one line.
        /// </value>
        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Text { get; set; }
    }
}
