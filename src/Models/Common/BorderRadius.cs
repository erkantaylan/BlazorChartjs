using System.Text.Json;

namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// The rounding of a bar's corners, in pixels.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Chart.js reads <c>borderRadius</c> as <c>number | { topLeft, topRight, bottomLeft, bottomRight }</c>.
    /// <see cref="BorderRadiusJsonConverter"/> writes the number when all four corners are set to the
    /// same value, and otherwise an object of the corners that are set; a corner left out of the
    /// object is square.
    /// </para>
    /// <para>
    /// The two forms are not quite equivalent. In a stacked bar a number rounds only the bars at
    /// the two ends of the stack, while an object rounds every bar in it. Either way a corner on
    /// an edge <c>borderSkipped</c> skips stays square — and <c>borderSkipped</c> skips the base
    /// edge by default, so <c>BorderRadius = 8</c> alone rounds the two corners away from it.
    /// </para>
    /// </remarks>
    [JsonConverter(typeof(BorderRadiusJsonConverter))]
    public class BorderRadius
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BorderRadius"/> class with every corner unset.
        /// </summary>
        public BorderRadius() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BorderRadius"/> class with the same radius
        /// on all four corners, written as a bare number.
        /// </summary>
        /// <param name="radius">The radius, in pixels.</param>
        public BorderRadius(int radius)
        {
            TopLeft = radius;
            TopRight = radius;
            BottomLeft = radius;
            BottomRight = radius;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BorderRadius"/> class corner by corner.
        /// </summary>
        /// <param name="topLeft">The top left radius.</param>
        /// <param name="topRight">The top right radius.</param>
        /// <param name="bottomLeft">The bottom left radius.</param>
        /// <param name="bottomRight">The bottom right radius.</param>
        public BorderRadius(int? topLeft, int? topRight, int? bottomLeft, int? bottomRight)
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
        }

        /// <summary>
        /// The same radius on all four corners: <c>BorderRadius = 8</c>.
        /// </summary>
        /// <param name="radius">The radius, in pixels.</param>
        public static implicit operator BorderRadius(int radius) => new BorderRadius(radius);

        /// <summary>
        /// Gets or sets the top left radius.
        /// </summary>
        /// <value>The radius, in pixels.</value>
        [JsonPropertyName("topLeft")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TopLeft { get; set; }

        /// <summary>
        /// Gets or sets the top right radius.
        /// </summary>
        /// <value>The radius, in pixels.</value>
        [JsonPropertyName("topRight")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TopRight { get; set; }

        /// <summary>
        /// Gets or sets the bottom left radius.
        /// </summary>
        /// <value>The radius, in pixels.</value>
        [JsonPropertyName("bottomLeft")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BottomLeft { get; set; }

        /// <summary>
        /// Gets or sets the bottom right radius.
        /// </summary>
        /// <value>The radius, in pixels.</value>
        [JsonPropertyName("bottomRight")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BottomRight { get; set; }

        /// <summary>
        /// The radius shared by all four corners, or <c>null</c> when any corner is unset or they differ.
        /// </summary>
        internal int? Uniform =>
            TopLeft.HasValue && TopLeft == TopRight && TopLeft == BottomLeft && TopLeft == BottomRight
                ? TopLeft
                : null;
    }

    /// <summary>
    /// Writes <see cref="BorderRadius"/> in the shape Chart.js reads: a number when the four corners
    /// agree, otherwise <c>{"topLeft":…,"topRight":…,"bottomLeft":…,"bottomRight":…}</c> with every
    /// unset corner omitted.
    /// </summary>
    /// <remarks>
    /// The object form's keys are the names of <see cref="BorderRadius"/>'s own properties, so the
    /// model graph the key-validation tests walk sees exactly the paths this converter can write.
    /// </remarks>
    public class BorderRadiusJsonConverter : JsonConverter<BorderRadius>
    {
        /// <inheritdoc />
        public override BorderRadius? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.Number:
                    return new BorderRadius(reader.GetInt32());
                case JsonTokenType.StartObject:
                    break;
                default:
                    throw new JsonException("borderRadius must be a number or an object of corners.");
            }

            var result = new BorderRadius();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return result;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("Expected a corner name in borderRadius.");

                var name = reader.GetString();
                reader.Read();

                switch (name)
                {
                    case "topLeft":
                        result.TopLeft = ReadCorner(ref reader);
                        break;
                    case "topRight":
                        result.TopRight = ReadCorner(ref reader);
                        break;
                    case "bottomLeft":
                        result.BottomLeft = ReadCorner(ref reader);
                        break;
                    case "bottomRight":
                        result.BottomRight = ReadCorner(ref reader);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            throw new JsonException("Unexpected end of JSON while reading borderRadius.");
        }

        private static int? ReadCorner(ref Utf8JsonReader reader) =>
            reader.TokenType == JsonTokenType.Null ? null : reader.GetInt32();

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, BorderRadius value, JsonSerializerOptions options)
        {
            if (value.Uniform is int radius)
            {
                writer.WriteNumberValue(radius);
                return;
            }

            writer.WriteStartObject();

            if (value.TopLeft.HasValue)
                writer.WriteNumber("topLeft", value.TopLeft.Value);
            if (value.TopRight.HasValue)
                writer.WriteNumber("topRight", value.TopRight.Value);
            if (value.BottomLeft.HasValue)
                writer.WriteNumber("bottomLeft", value.BottomLeft.Value);
            if (value.BottomRight.HasValue)
                writer.WriteNumber("bottomRight", value.BottomRight.Value);

            writer.WriteEndObject();
        }
    }
}
