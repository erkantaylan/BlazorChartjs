namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// A chart title, written to <c>options.plugins.title</c> as <see cref="Plugins.Title"/> and to
    /// <c>options.plugins.subtitle</c> as <see cref="Plugins.Subtitle"/>.
    /// </summary>
    /// <remarks>
    /// Chart.js's title and subtitle plugins read the same options, so one class serves both. The
    /// subtitle is drawn directly below the title.
    /// </remarks>
    public class Title
    {
        /// <summary>
        /// Gets or sets the align.
        /// </summary>
        /// <value>
        /// The align.
        /// </value>
        [JsonIgnore]
        public Align? Align
        {
            get => _align;
            set
            {
                _align = value;
                AlignString = value?.Value;
            }
        }
        private Align? _align;

        /// <summary>Gets or sets the align of the title</summary>
        /// <value>
        ///   <para>
        /// The align of the title. It accepts those values:</para>
        ///   <list type="bullet">
        ///     <item>
        ///       start
        ///     </item>
        ///     <item>
        ///       center
        ///     </item>
        ///     <item>
        ///       end
        ///     </item>
        ///   </list>
        /// </value>
        [JsonPropertyName("align")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AlignString { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        [JsonPropertyName("color")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Color { get; set; }

        /// <summary>
        /// Gets or sets if the title is shown.
        /// </summary>
        /// <value>
        /// The display.
        /// </value>
        [JsonPropertyName("display")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Display { get; set; }

        /// <summary>
        /// Gets or sets the full size.
        /// </summary>
        /// <value>
        /// Marks that this box should take the full width/height of the canvas. 
        /// If false, the box is sized and placed above/beside the chart area.
        /// </value>
        [JsonPropertyName("fullSize")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? FullSize { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        [JsonIgnore]
        public Position? Position
        {
            get => _position;
            set
            {
                _position = value;
                PositionString = value?.Value;
            }
        }
        private Position? _position;

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position of the title. It accepts those values:
        ///   <list type="bullet">
        ///     <item>
        ///       top
        ///     </item>
        ///     <item>
        ///       left
        ///     </item>
        ///     <item>
        ///       bottom
        ///     </item>
        ///     <item>
        ///       right
        ///     </item>
        ///   </list>
        /// </value>
        [JsonPropertyName("position")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PositionString { get; set; }

        /// <summary>
        /// Gets or sets the text, on one line.
        /// </summary>
        /// <value>
        /// The title text. Assigning a value clears <see cref="TextLines"/>: the text assigned last
        /// is the one written to <c>text</c>.
        /// </value>
        [JsonIgnore]
        public string? Text
        {
            get => _text;
            set
            {
                _text = value;
                if (value is not null) _textLines = null;
            }
        }
        private string? _text;

        /// <summary>
        /// Gets or sets the text as several lines, the first drawn on top.
        /// </summary>
        /// <value>
        /// One entry per line, written to <c>text</c> as an array. Assigning a list clears
        /// <see cref="Text"/>: the text assigned last is the one written.
        /// </value>
        [JsonIgnore]
        public List<string>? TextLines
        {
            get => _textLines;
            set
            {
                _textLines = value;
                if (value is not null) _text = null;
            }
        }
        private List<string>? _textLines;

        /// <summary>
        /// The one <c>text</c> key, from whichever of <see cref="Text"/> and <see cref="TextLines"/>
        /// is set. At most one ever is.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        private object? TextValue => (object?)_textLines ?? _text;

        /// <summary>
        /// Gets or sets the padding.
        /// </summary>
        /// <value>
        /// The padding above and below the title. Chart.js reads only the vertical pair here,
        /// so this is <see cref="TitlePadding"/> rather than the four-sided <see cref="Padding"/>.
        /// </value>
        [JsonPropertyName("padding")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TitlePadding? Padding { get; set; }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>
        /// The font.
        /// </value>
        [JsonPropertyName("font")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Font? Font { get; set; }

        /// <summary>
        /// Gets or sets title options this class has no property for, written into
        /// <c>options.plugins.title</c> or <c>options.plugins.subtitle</c> next to the typed keys.
        /// </summary>
        /// <value>
        /// Each entry becomes one key, spelled exactly as given: <c>["weight"] = 2000</c>.
        /// A key must not repeat one a property of this class already writes.
        /// </value>
        [JsonExtensionData]
        public Dictionary<string, object?>? ExtraOptions { get; set; }
    }
}
