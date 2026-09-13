using Erkan.Blazor.Chartjs.Models.Common.StringEnums;

namespace Erkan.Blazor.Chartjs.Models.Line
{
    /// <summary>
    /// Line Dataset
    /// </summary>
    /// <seealso cref="Erkan.Blazor.Chartjs.Models.Common.Dataset" />
    public class LineDataset : Dataset
    {
        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>
        /// The color of the background.
        /// </value>
        [JsonPropertyName("backgroundColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets how the ends of the line are drawn.
        /// </summary>
        /// <value>The cap style; <c>null</c> leaves the Chart.js default (<c>butt</c>) in place.</value>
        [JsonIgnore]
        public BorderCapStyle? BorderCapStyle
        {
            get => _borderCapStyle;
            set
            {
                _borderCapStyle = value;
                BorderCapStyleString = value?.Value;
            }
        }
        private BorderCapStyle? _borderCapStyle;

        /// <summary>
        /// Gets or sets the border cap style string.
        /// </summary>
        /// <value>The border cap style string.</value>
        [JsonPropertyName("borderCapStyle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BorderCapStyleString { get; set; }

        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        /// <value>
        /// The color of the background.
        /// </value>
        [JsonPropertyName("borderColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string BorderColor { get; set; }

        /// <summary>
        /// Gets or sets the dash pattern of the line.
        /// </summary>
        /// <value>
        /// Alternating lengths of drawn and blank segments, in pixels, e.g. <c>[5, 5]</c>, as for
        /// the canvas <c>setLineDash</c>. <c>null</c> draws a solid line.
        /// </value>
        [JsonPropertyName("borderDash")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<decimal>? BorderDash { get; set; }

        /// <summary>
        /// Gets or sets the offset of the dash pattern.
        /// </summary>
        /// <value>
        /// How far into the pattern the line starts, in pixels. <c>0</c> is written, not dropped.
        /// </value>
        [JsonPropertyName("borderDashOffset")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? BorderDashOffset { get; set; }

        /// <summary>
        /// Gets or sets how two segments of the line meet.
        /// </summary>
        /// <value>The join style; <c>null</c> leaves the Chart.js default (<c>miter</c>) in place.</value>
        [JsonIgnore]
        public BorderJoinStyle? BorderJoinStyle
        {
            get => _borderJoinStyle;
            set
            {
                _borderJoinStyle = value;
                BorderJoinStyleString = value?.Value;
            }
        }
        private BorderJoinStyle? _borderJoinStyle;

        /// <summary>
        /// Gets or sets the border join style string.
        /// </summary>
        /// <value>The border join style string.</value>
        [JsonPropertyName("borderJoinStyle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BorderJoinStyleString { get; set; }

        /// <summary>
        /// Gets or sets the width of the border.
        /// </summary>
        /// <value>
        /// The width of the border.
        /// </value>
        [JsonPropertyName("borderWidth")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BorderWidth { get; set; }

        /// <summary>
        /// Gets or sets how far outside the chart area the dataset may draw.
        /// </summary>
        /// <value>
        /// Pixels on every side: a positive value lets points on the edge of the chart area draw
        /// whole, a negative one clips inside it. Chart.js's <c>false</c> and per-side object forms
        /// are not exposed.
        /// </value>
        [JsonPropertyName("clip")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Clip { get; set; }

        /// <summary>
        /// Gets or sets the cubic interpolation mode.
        /// </summary>
        /// <value>The cubic interpolation mode.</value>
        [JsonIgnore]
        public CubicInterpolationMode? CubicInterpolationMode
        {
            get => _interpolation;
            set
            {
                _interpolation = value;
                CubicInterpolationModeString = value?.Value;
            }
        }
        private CubicInterpolationMode? _interpolation;

        /// <summary>
        /// Gets or sets the point style string.
        /// </summary>
        /// <value>
        /// The point style string.
        /// </value>
        [JsonPropertyName("cubicInterpolationMode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CubicInterpolationModeString { get; set; }

        /// <summary>
        /// Gets or sets whether hovered points are drawn over the dataset's other points.
        /// </summary>
        /// <value>
        /// <c>false</c> keeps them in data order; <c>null</c> leaves the Chart.js default
        /// (<c>true</c>) in place.
        /// </value>
        [JsonPropertyName("drawActiveElementsOnTop")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? DrawActiveElementsOnTop { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="LineDataset"/> is fill.
        /// </summary>
        /// <value>
        ///   <c>true</c> if fill; <c>false</c> to explicitly turn the fill off;
        ///   <c>null</c> to leave the Chart.js default (<c>false</c>) in place.
        /// </value>
        [JsonPropertyName("fill")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Fill { get; set; }

        /// <summary>
        /// Gets or sets the background color when hovered.
        /// </summary>
        /// <value>
        /// The hover background color. Also the fallback for <see cref="PointHoverBackgroundColor"/>.
        /// </value>
        [JsonPropertyName("hoverBackgroundColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? HoverBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the border color when hovered.
        /// </summary>
        /// <value>
        /// The hover border color. Also the fallback for <see cref="PointHoverBorderColor"/>.
        /// </value>
        [JsonPropertyName("hoverBorderColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? HoverBorderColor { get; set; }

        /// <summary>
        /// Gets or sets the border width when hovered.
        /// </summary>
        /// <value>
        /// The hover border width. Also the fallback for <see cref="PointHoverBorderWidth"/>.
        /// </value>
        [JsonPropertyName("hoverBorderWidth")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? HoverBorderWidth { get; set; }

        /// <summary>
        /// Gets or sets the fill color of the points.
        /// </summary>
        /// <value>
        /// One color per point. Chart.js wraps around a shorter list, so a single entry colors
        /// every point.
        /// </value>
        [JsonPropertyName("pointBackgroundColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? PointBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the border color of the points.
        /// </summary>
        /// <value>
        /// One color per point. Chart.js wraps around a shorter list, so a single entry colors
        /// every point.
        /// </value>
        [JsonPropertyName("pointBorderColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? PointBorderColor { get; set; }

        /// <summary>
        /// Gets or sets the border width of the points.
        /// </summary>
        /// <value>
        /// The point border width, in pixels.
        /// </value>
        [JsonPropertyName("pointBorderWidth")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PointBorderWidth { get; set; }

        /// <summary>
        /// Gets or sets the radius, around each point, that reacts to the pointer.
        /// </summary>
        /// <value>
        /// The hit radius, in pixels, added to the point radius. Larger values make small points
        /// easier to hover.
        /// </value>
        [JsonPropertyName("pointHitRadius")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PointHitRadius { get; set; }

        /// <summary>
        /// Gets or sets the fill color of a hovered point.
        /// </summary>
        /// <value>
        /// One color per point. Chart.js wraps around a shorter list, so a single entry colors
        /// every point.
        /// </value>
        [JsonPropertyName("pointHoverBackgroundColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? PointHoverBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the border color of a hovered point.
        /// </summary>
        /// <value>
        /// One color per point. Chart.js wraps around a shorter list, so a single entry colors
        /// every point.
        /// </value>
        [JsonPropertyName("pointHoverBorderColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? PointHoverBorderColor { get; set; }

        /// <summary>
        /// Gets or sets the border width of a hovered point.
        /// </summary>
        /// <value>
        /// The hovered point border width, in pixels.
        /// </value>
        [JsonPropertyName("pointHoverBorderWidth")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PointHoverBorderWidth { get; set; }

        /// <summary>
        /// Gets or sets the radius of a hovered point.
        /// </summary>
        /// <value>
        /// The hovered point radius, in pixels.
        /// </value>
        [JsonPropertyName("pointHoverRadius")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PointHoverRadius { get; set; }

        /// <summary>
        /// Gets or sets the point radius.
        /// </summary>
        /// <value>
        /// The point radius.
        /// </value>
        [JsonPropertyName("pointRadius")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PointRadius { get; set; }

        /// <summary>
        /// Gets or sets the rotation of the point shape.
        /// </summary>
        /// <value>
        /// The rotation, in degrees.
        /// </value>
        [JsonPropertyName("pointRotation")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PointRotation { get; set; }

        /// <summary>
        /// Gets or sets the point style.
        /// </summary>
        /// <value>
        /// The point style.
        /// </value>
        [JsonIgnore]
        public PointStyle? PointStyle
        {
            get => _pointStyle;
            set
            {
                _pointStyle = value;
                PointStyleString = value?.Value;
            }
        }
        private PointStyle? _pointStyle;

        /// <summary>
        /// Gets or sets the point style string.
        /// </summary>
        /// <value>
        /// The point style string.
        /// </value>
        [JsonPropertyName("pointStyle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PointStyleString { get; set; }

        /// <summary>
        /// Gets or sets whether the line is drawn.
        /// </summary>
        /// <value>
        /// <c>false</c> draws the points only; <c>null</c> leaves the Chart.js default (<c>true</c>)
        /// in place.
        /// </value>
        [JsonPropertyName("showLine")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ShowLine { get; set; }

        /// <summary>
        /// Gets or sets whether the line is drawn across <c>null</c> data points.
        /// </summary>
        /// <value>
        /// <c>true</c> joins the points on either side of a gap; <c>false</c> breaks the line
        /// there, overriding a chart-level <c>spanGaps</c>. Chart.js's numeric form, the largest
        /// gap to span, is not exposed.
        /// </value>
        [JsonPropertyName("spanGaps")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? SpanGaps { get; set; }

        /// <summary>
        /// Gets or sets the stack group.
        /// </summary>
        /// <value>
        /// Datasets with the same stack id are stacked together when the value axis has
        /// <c>Stacked = true</c>.
        /// </value>
        [JsonPropertyName("stack")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Stack { get; set; }

        /// <summary>
        /// Gets or sets the step mode.
        /// </summary>
        /// <value>
        /// The step mode.
        /// </value>
        [JsonIgnore]
        public StepMode? StepMode
        {
            get => _stepMode;
            set
            {
                _stepMode = value;
                StepModeString = value?.Value;
            }
        }
        private StepMode? _stepMode;

        /// <summary>
        /// Gets or sets the step mode string.
        /// </summary>
        /// <value>
        /// The step mode string. <c>"true"</c> and <c>"false"</c> are written as JSON booleans,
        /// because Chart.js tests <c>stepped</c> for truthiness; anything else as a string.
        /// </value>
        [JsonPropertyName("stepped")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonConverter(typeof(StepModeJsonConverter))]
        public string? StepModeString { get; set; }

        /// <summary>
        /// Gets or sets the tension.
        /// </summary>
        /// <value>
        /// The tension. <c>0</c> draws straight segments, overriding a chart-level
        /// <c>elements.line.tension</c>; <c>null</c> leaves the inherited value in place.
        /// </value>
        [JsonPropertyName("tension")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? Tension { get; set; }

        /// <summary>
        /// Gets or sets the x axis identifier.
        /// </summary>
        /// <value>
        /// The key of the x axis in <c>Options.Scales</c> this dataset is drawn against.
        /// </value>
        [JsonPropertyName("xAxisID")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? XAxisId { get; set; }

        /// <summary>
        /// Gets or sets the y axis identifier.
        /// </summary>
        /// <value>
        /// The y axis identifier.
        /// </value>
        [JsonPropertyName("yAxisID")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? YAxisId { get; set; }
    }
}
