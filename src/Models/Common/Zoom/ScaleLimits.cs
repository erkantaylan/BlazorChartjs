using System.Globalization;
using System.Text.Json;

namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// Class ScaleLimits. The bounds a single axis may be zoomed or panned to.
    /// </summary>
    /// <remarks>
    /// Serialized by <see cref="ScaleLimitsJsonConverter"/>, because a limit is either a
    /// JSON number or the literal string <c>"original"</c> - a shape no plain property can
    /// express without putting an <c>object</c> on the public API.
    /// </remarks>
    [JsonConverter(typeof(ScaleLimitsJsonConverter))]
    public class ScaleLimits
    {
        private double? _min = null;
        private double? _max = null;
        private bool _minOriginal = false;
        private bool _maxOriginal = false;

        /// <summary>
        /// Minimum allowed value for scale.min.
        /// </summary>
        /// <value>
        /// A number, or the literal <c>"original"</c> to clamp to the value the axis
        /// started at. Null (the default) leaves the axis unbounded.
        /// </value>
        public string? Min
        {
            get
            {
                if (_minOriginal)
                    return "original";
                return _min?.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                _min = null;
                _minOriginal = false;

                if (value == null)
                    return;

                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
                    _min = parsed;
                else
                    _minOriginal = true;
            }
        }

        /// <summary>
        /// Maximum allowed value for scale.max.
        /// </summary>
        /// <value>
        /// A number, or the literal <c>"original"</c> to clamp to the value the axis
        /// started at. Null (the default) leaves the axis unbounded.
        /// </value>
        public string? Max
        {
            get
            {
                if (_maxOriginal)
                    return "original";
                return _max?.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                _max = null;
                _maxOriginal = false;

                if (value == null)
                    return;

                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
                    _max = parsed;
                else
                    _maxOriginal = true;
            }
        }

        /// <summary>
        /// Minimum allowed range (max - min). This defines the max zoom level.
        /// </summary>
        /// <value>
        /// The minRange.
        /// </value>
        public double? MinRange { get; set; }

        /// <summary>
        /// The numeric minimum, or null when unset or set to <c>"original"</c>. The plugin
        /// does arithmetic on this value, so a numeric limit must not be written as a string.
        /// </summary>
        internal double? MinNumber => _minOriginal ? null : _min;

        /// <summary>
        /// Whether the minimum is the literal <c>"original"</c>.
        /// </summary>
        internal bool MinIsOriginal => _minOriginal;

        /// <summary>
        /// The numeric maximum, or null when unset or set to <c>"original"</c>.
        /// </summary>
        internal double? MaxNumber => _maxOriginal ? null : _max;

        /// <summary>
        /// Whether the maximum is the literal <c>"original"</c>.
        /// </summary>
        internal bool MaxIsOriginal => _maxOriginal;
    }

    /// <summary>
    /// Writes <see cref="ScaleLimits"/> in the shape chartjs-plugin-zoom expects:
    /// <c>{"min":&lt;number|"original"&gt;,"max":&lt;number|"original"&gt;,"minRange":&lt;number&gt;}</c>,
    /// with every unset member omitted.
    /// </summary>
    public class ScaleLimitsJsonConverter : JsonConverter<ScaleLimits>
    {
        /// <inheritdoc />
        public override ScaleLimits? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected an object for ScaleLimits.");

            var result = new ScaleLimits();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return result;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("Expected a property name in ScaleLimits.");

                var name = reader.GetString();
                reader.Read();

                switch (name)
                {
                    case "min":
                        result.Min = ReadLimit(ref reader);
                        break;
                    case "max":
                        result.Max = ReadLimit(ref reader);
                        break;
                    case "minRange":
                        result.MinRange = reader.TokenType == JsonTokenType.Null ? null : reader.GetDouble();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            throw new JsonException("Unexpected end of JSON while reading ScaleLimits.");
        }

        private static string? ReadLimit(ref Utf8JsonReader reader) => reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.String => reader.GetString(),
            JsonTokenType.Number => reader.GetDouble().ToString(CultureInfo.InvariantCulture),
            _ => throw new JsonException("A scale limit must be a number or a string.")
        };

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, ScaleLimits value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (value.MinIsOriginal)
                writer.WriteString("min", "original");
            else if (value.MinNumber.HasValue)
                writer.WriteNumber("min", value.MinNumber.Value);

            if (value.MaxIsOriginal)
                writer.WriteString("max", "original");
            else if (value.MaxNumber.HasValue)
                writer.WriteNumber("max", value.MaxNumber.Value);

            if (value.MinRange.HasValue)
                writer.WriteNumber("minRange", value.MinRange.Value);

            writer.WriteEndObject();
        }
    }
}
