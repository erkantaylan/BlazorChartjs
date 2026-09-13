namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// Tooltip
    /// </summary>
    public sealed class Tooltip
    {
        //https://www.chartjs.org/docs/latest/configuration/tooltip.html

        private InteractionMode? _mode;
        private AxisInteractions? _axis;
        private TooltipPosition? _position;
        private TooltipXAlign? _xAlign;
        private TooltipYAlign? _yAlign;
        private TextAlign? _titleAlign;
        private TextAlign? _bodyAlign;
        private TextAlign? _footerAlign;
        private TextDirection? _textDirection;

        /// <summary>
        /// Gets or sets a value indicating whether the canvas tooltip is drawn.
        /// </summary>
        /// <value>
        ///   <c>true</c> by default in Chart.js; <c>false</c> draws no tooltip at all.
        /// </value>
        [JsonPropertyName("enabled")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Enabled { get; set; }

        /// <summary>
        /// Gets or sets which elements the tooltip describes. <seealso cref="InteractionMode"/>
        /// </summary>
        /// <value>
        /// The mode. When <c>null</c> the tooltip uses <see cref="Options.Interaction"/>, and the chart
        /// type's default if that sets none either — <c>"nearest"</c>, or <c>"point"</c> for bubble and scatter.
        /// </value>
        [JsonIgnore]
        public InteractionMode? Mode
        {
            get => _mode;
            set {
                _mode = value;
                ModeString = value?.Value;
            }
        }

        /// <summary>
        /// Gets or sets the mode string.
        /// </summary>
        /// <value>The mode string.</value>
        [JsonPropertyName("mode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ModeString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tooltip only appears when the pointer is over an element.
        /// </summary>
        /// <value>
        ///   <c>null</c> falls back to <see cref="Options.Interaction"/>; <c>false</c> shows the tooltip
        ///   for the elements <see cref="Mode"/> finds wherever the pointer is.
        /// </value>
        [JsonPropertyName("intersect")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Intersect { get; set; }

        /// <summary>
        /// Gets or sets which directions count when <see cref="Mode"/> measures distance. <seealso cref="AxisInteractions"/>
        /// </summary>
        /// <value>
        /// The axis. When <c>null</c> the tooltip uses <see cref="Options.Interaction"/>.
        /// </value>
        [JsonIgnore]
        public AxisInteractions? Axis
        {
            get => _axis;
            set {
                _axis = value;
                AxisString = value?.Value;
            }
        }

        /// <summary>
        /// Gets or sets the axis string.
        /// </summary>
        /// <value>The axis string.</value>
        [JsonPropertyName("axis")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AxisString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether elements outside the chart area can be described.
        /// </summary>
        /// <value>
        ///   <c>null</c> falls back to <see cref="Options.Interaction"/>, and to Chart.js's <c>false</c>.
        /// </value>
        [JsonPropertyName("includeInvisible")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IncludeInvisible { get; set; }

        /// <summary>
        /// Gets or sets where the tooltip is placed. <seealso cref="TooltipPosition"/>
        /// </summary>
        /// <value>
        /// The position. The Chart.js default is <see cref="TooltipPosition.Average"/>.
        /// </value>
        [JsonIgnore]
        public TooltipPosition? Position
        {
            get => _position;
            set {
                _position = value;
                PositionString = value?.Value;
            }
        }

        /// <summary>
        /// Gets or sets the position string. <seealso cref="TooltipPosition"/>
        /// </summary>
        /// <value>
        ///   <para><c>"average"</c>, <c>"nearest"</c>, or the name of a positioner registered in page
        ///   script as <c>Chart.Tooltip.positioners.myName = function (items, eventPosition) { ... }</c>.</para>
        ///   <para>Register it before the chart is created: Chart.js calls the positioner by name on the
        ///   first hover and throws if there is none.</para>
        /// </value>
        [JsonPropertyName("position")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PositionString { get; set; }

        /// <summary>
        /// Gets or sets the horizontal side of the tooltip the caret is on. <seealso cref="TooltipXAlign"/>
        /// </summary>
        /// <value>
        /// The alignment. When <c>null</c> Chart.js picks one that keeps the tooltip on the canvas.
        /// </value>
        [JsonIgnore]
        public TooltipXAlign? XAlign
        {
            get => _xAlign;
            set {
                _xAlign = value;
                XAlignString = value?.Value;
            }
        }

        /// <summary>
        /// Gets or sets the x align string.
        /// </summary>
        /// <value>The x align string.</value>
        [JsonPropertyName("xAlign")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? XAlignString { get; set; }

        /// <summary>
        /// Gets or sets the vertical side of the tooltip the caret is on. <seealso cref="TooltipYAlign"/>
        /// </summary>
        /// <value>
        /// The alignment. When <c>null</c> Chart.js picks one that keeps the tooltip on the canvas.
        /// </value>
        [JsonIgnore]
        public TooltipYAlign? YAlign
        {
            get => _yAlign;
            set {
                _yAlign = value;
                YAlignString = value?.Value;
            }
        }

        /// <summary>
        /// Gets or sets the y align string.
        /// </summary>
        /// <value>The y align string.</value>
        [JsonPropertyName("yAlign")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? YAlignString { get; set; }

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
        /// Gets or sets the horizontal alignment of the title text. <seealso cref="Common.TextAlign"/>
        /// </summary>
        /// <value>
        /// The alignment. The Chart.js default is <see cref="Common.TextAlign.Left"/>.
        /// </value>
        [JsonIgnore]
        public TextAlign? TitleAlign
        {
            get => _titleAlign;
            set {
                _titleAlign = value;
                TitleAlignString = value?.Value;
            }
        }

        /// <summary>
        /// Gets or sets the title align string.
        /// </summary>
        /// <value>The title align string.</value>
        [JsonPropertyName("titleAlign")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TitleAlignString { get; set; }

        /// <summary>
        /// Gets or sets the space added above and below each title line.
        /// </summary>
        /// <value>
        /// The spacing in pixels. The Chart.js default is <c>2</c>.
        /// </value>
        [JsonPropertyName("titleSpacing")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TitleSpacing { get; set; }

        /// <summary>
        /// Gets or sets the space between the title and the body.
        /// </summary>
        /// <value>
        /// The margin in pixels. The Chart.js default is <c>6</c>.
        /// </value>
        [JsonPropertyName("titleMarginBottom")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TitleMarginBottom { get; set; }

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
        /// Gets or sets the horizontal alignment of the body text. <seealso cref="Common.TextAlign"/>
        /// </summary>
        /// <value>
        /// The alignment. The Chart.js default is <see cref="Common.TextAlign.Left"/>.
        /// </value>
        [JsonIgnore]
        public TextAlign? BodyAlign
        {
            get => _bodyAlign;
            set {
                _bodyAlign = value;
                BodyAlignString = value?.Value;
            }
        }

        /// <summary>
        /// Gets or sets the body align string.
        /// </summary>
        /// <value>The body align string.</value>
        [JsonPropertyName("bodyAlign")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BodyAlignString { get; set; }

        /// <summary>
        /// Gets or sets the space added above and below each tooltip item.
        /// </summary>
        /// <value>
        /// The spacing in pixels. The Chart.js default is <c>2</c>.
        /// </value>
        [JsonPropertyName("bodySpacing")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BodySpacing { get; set; }

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
        /// Gets or sets the horizontal alignment of the footer text. <seealso cref="Common.TextAlign"/>
        /// </summary>
        /// <value>
        /// The alignment. The Chart.js default is <see cref="Common.TextAlign.Left"/>.
        /// </value>
        [JsonIgnore]
        public TextAlign? FooterAlign
        {
            get => _footerAlign;
            set {
                _footerAlign = value;
                FooterAlignString = value?.Value;
            }
        }

        /// <summary>
        /// Gets or sets the footer align string.
        /// </summary>
        /// <value>The footer align string.</value>
        [JsonPropertyName("footerAlign")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? FooterAlignString { get; set; }

        /// <summary>
        /// Gets or sets the space added above and below each footer line.
        /// </summary>
        /// <value>
        /// The spacing in pixels. The Chart.js default is <c>2</c>.
        /// </value>
        [JsonPropertyName("footerSpacing")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? FooterSpacing { get; set; }

        /// <summary>
        /// Gets or sets the space between the body and the footer.
        /// </summary>
        /// <value>
        /// The margin in pixels. The Chart.js default is <c>6</c>.
        /// </value>
        [JsonPropertyName("footerMarginTop")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? FooterMarginTop { get; set; }

        /// <summary>
        /// Gets or sets the padding between the tooltip's edge and its text.
        /// </summary>
        /// <value>
        ///   <para>The padding in pixels. The Chart.js default is <c>6</c> on every side.</para>
        ///   <para><c>new Padding(10)</c> is Chart.js's single-number form: the same padding on all four sides.</para>
        /// </value>
        [JsonPropertyName("padding")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Padding? Padding { get; set; }

        /// <summary>
        /// Gets or sets the gap between the tip of the caret and the point it indicates.
        /// </summary>
        /// <value>
        /// The distance in pixels. The Chart.js default is <c>2</c>.
        /// </value>
        [JsonPropertyName("caretPadding")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? CaretPadding { get; set; }

        /// <summary>
        /// Gets or sets the size of the caret, the arrow pointing from the tooltip to the point.
        /// </summary>
        /// <value>
        /// The size in pixels. The Chart.js default is <c>5</c>; <c>0</c> draws no caret.
        /// </value>
        [JsonPropertyName("caretSize")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? CaretSize { get; set; }

        /// <summary>
        /// Gets or sets the radius of the tooltip's corners.
        /// </summary>
        /// <value>
        ///   <para>The radius in pixels, applied to all four corners. The Chart.js default is <c>6</c>.</para>
        ///   <para>Chart.js also accepts a different radius per corner, which this property cannot express.</para>
        /// </value>
        [JsonPropertyName("cornerRadius")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? CornerRadius { get; set; }

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
        /// Gets or sets a value indicating whether each tooltip item shows its dataset's colour box.
        /// </summary>
        /// <value>
        ///   <c>true</c> by default in Chart.js; <c>false</c> shows the text alone.
        /// </value>
        [JsonPropertyName("displayColors")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? DisplayColors { get; set; }

        /// <summary>
        /// Gets or sets the width of the colour box.
        /// </summary>
        /// <value>
        /// The width in pixels. Chart.js defaults to the body font size.
        /// </value>
        [JsonPropertyName("boxWidth")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BoxWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the colour box.
        /// </summary>
        /// <value>
        /// The height in pixels. Chart.js defaults to the body font size.
        /// </value>
        [JsonPropertyName("boxHeight")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BoxHeight { get; set; }

        /// <summary>
        /// Gets or sets the space between the colour box and the item's text.
        /// </summary>
        /// <value>
        /// The padding in pixels. The Chart.js default is <c>0</c>.
        /// </value>
        [JsonPropertyName("boxPadding")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BoxPadding { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the colour box takes the dataset's point style.
        /// </summary>
        /// <value>
        ///   <c>true</c> draws each item's point style — a circle, a triangle — sized to the smaller of
        ///   <see cref="BoxWidth"/> and <see cref="BoxHeight"/>, instead of a square; Chart.js's default is <c>false</c>.
        /// </value>
        [JsonPropertyName("usePointStyle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? UsePointStyle { get; set; }

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

        /// <summary>
        /// Gets or sets a value indicating whether the tooltip is laid out from right to left.
        /// </summary>
        /// <value>
        ///   <c>true</c> puts each colour box to the right of its text; Chart.js's default is <c>false</c>.
        /// </value>
        [JsonPropertyName("rtl")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? RTL { get; set; }

        /// <summary>
        /// Gets or sets the text direction the tooltip is drawn in, whatever the canvas's CSS says. <seealso cref="Common.TextDirection"/>
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
    }
}
