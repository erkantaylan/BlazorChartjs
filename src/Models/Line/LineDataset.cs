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
        /// Gets or sets the color of the border.
        /// </summary>
        /// <value>
        /// The color of the background.
        /// </value>
        [JsonPropertyName("borderColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string BorderColor { get; set; }

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
        /// Gets or sets the point radius.
        /// </summary>
        /// <value>
        /// The point radius.
        /// </value>
        [JsonPropertyName("pointRadius")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PointRadius { get; set; }

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
                SteppedString = value?.Value;
            }
        }
        private StepMode? _stepMode;

        /// <summary>
        /// Gets or sets the stepped string.
        /// </summary>
        /// <value>The stepped string.</value>
        [JsonPropertyName("stepped")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SteppedString { get; set; }

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