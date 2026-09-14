using System.Threading.Tasks;

namespace Erkan.Blazor.Chartjs.Models.Common 
{
    /// <summary>
    /// Legend
    /// </summary>
    public class Legend {
        private Align? _align;
        private LegendPosition? _legendPosition;
        private TextDirection? _textDirection;

        /// <summary>]
        /// Gets or sets the align. <seealso cref="Common.Align"/>
        /// </summary>
        /// <value>
        /// The align.
        /// </value>
        [JsonIgnore]
        public Align? Align 
        {
            get => _align;
            set {
                _align = value;
                AlignString = value?.Value;
            }
        }

        /// <summary>
        /// Gets or sets the align string.
        /// </summary>
        /// <value>
        /// The align string.
        /// </value>
        [JsonPropertyName("align")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AlignString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Legend"/> is display.
        /// </summary>
        /// <value>
        ///   <c>true</c> if display; otherwise, <c>false</c>. <c>null</c>, the default, writes no key
        ///   and Chart.js shows the legend.
        /// </value>
        [JsonPropertyName("display")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Display { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether full size.
        /// Marks that this box should take the full width/height of the canvas (moving other boxes).
        /// This is unlikely to need to be changed in day-to-day use.
        /// </summary>
        /// <value><c>null</c> if fullSize contains no value, <c>true</c> is the default; otherwise, <c>false</c>.</value>
        [JsonPropertyName("fullSize")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? FullSize { get; set; }

        ///<summary>
        /// Gets or sets the legend labels configuration
        ///</summary>
        ///<value>The Legend label configuration</value>
        [JsonPropertyName("labels")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LegendLabels? Labels { get; set; }

        /// <summary>
        /// Gets or sets the maximum height of the legend.
        /// </summary>
        /// <value>
        /// The largest height, in pixels, the legend box may take. Unset, it may take all the height
        /// the layout offers it, which is what Chart.js also does with <c>0</c>.
        /// </value>
        [JsonPropertyName("maxHeight")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MaxHeight { get; set; }

        /// <summary>
        /// Gets or sets the maximum width of the legend.
        /// </summary>
        /// <value>
        /// The largest width, in pixels, the legend box may take. Unset, it may take all the width
        /// the layout offers it, which is what Chart.js also does with <c>0</c>.
        /// </value>
        [JsonPropertyName("maxWidth")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MaxWidth { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        [JsonIgnore]
        public LegendPosition? Position 
        {
            get => _legendPosition;
            set {
                _legendPosition = value;
                PositionString = value?.Value;
            }
        }

        /// <summary>
        /// Gets or sets the position. <seealso cref="LegendPosition"/>
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        [JsonPropertyName("position")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PositionString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that the <see cref="Legend"/> will show in reverse order.
        /// </summary>
        /// <value>
        ///   <c>true</c> if reverse; otherwise, <c>false</c>. <c>null</c>, the default, writes no key
        ///   and Chart.js keeps the dataset order.
        /// </value>
        [JsonPropertyName("reverse")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Reverse { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the <see cref="Legend"/> will be rendered from right to left.
        /// </summary>
        /// <value><c>null</c> if [RTL] contains no value, <c>true</c> if [RTL]; otherwise, <c>false</c>.</value>
        [JsonPropertyName("rtl")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? RTL { get; set; }

        /// <summary>
        /// Gets or sets the text direction (ltr or rlt)
        /// </summary>
        /// <value>The text direction.</value>
        [JsonIgnore]
        public TextDirection? TextDirection 
        {
            get => _textDirection;
            set {
                _textDirection = value;
                TextDirectionString = value?.Value;
            }
        }

        /// <summary>
        /// Gets or sets the text direction string (ltr or rtl)
        /// </summary>
        /// <value>The text direction string.</value>
        [JsonPropertyName("textDirection")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TextDirectionString { get; set; }

        /// <summary>
        /// Gets or sets the title drawn above the legend items.
        /// </summary>
        /// <value>
        /// The legend title. Chart.js draws none unless <see cref="LegendTitle.Display"/> is <c>true</c>.
        /// </value>
        [JsonPropertyName("title")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LegendTitle? Title { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has legend click.
        /// </summary>
        /// <value><c>true</c> if this instance has legend click; otherwise, <c>false</c>.</value>
        [JsonInclude]
        [JsonPropertyName("hasLegendClick")]
        public bool HasLegendClick => OnClickAsync != null;

        /// <summary>
        /// Gets or sets the on legend click asynchronous.
        /// </summary>
        /// <value>The on legend click asynchronous.</value>
        [JsonIgnore]
        public Func<LegendClickContext, ValueTask>? OnClickAsync { get; set; }

        /// <summary>
        /// Gets or sets legend options this class has no property for, written into
        /// <c>options.plugins.legend</c> next to the typed keys.
        /// </summary>
        /// <value>
        /// Each entry becomes one key, spelled exactly as given: <c>["weight"] = 500</c>.
        /// A key must not repeat one a property of this class already writes.
        /// </value>
        [JsonExtensionData]
        public Dictionary<string, object?>? ExtraOptions { get; set; }
    }
}
