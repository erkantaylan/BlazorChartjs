using System.Text.Json;

namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// The rounding of a bar's corners, in pixels.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Chart.js reads <c>borderRadius</c> as <c>number | { topLeft, topRight, bottomLeft, bottomRight }</c>.
    /// <see cref="BorderRadiusJsonConverter"/> writes <see cref="Radius"/> alone as the number, and
    /// anything with a corner set as an object of the corners that have a value; a corner left out
    /// of the object is square.
    /// </para>
    /// <para>
    /// The two forms are not equivalent, which is why four equal corners are still sent as an
    /// object. On a stacked scale Chart.js applies a number only to the outermost bar on each side
    /// of zero, and an object to every bar in the stack — unless <c>borderSkipped</c> is
    /// <c>'middle'</c>, which rounds them all. Either way a corner on an edge <c>borderSkipped</c>
    /// skips stays square, and by default that is the edge the bar grows from, so
    /// <c>BorderRadius = 8</c> alone rounds the two corners away from it.
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
        /// Initializes a new instance of the <see cref="BorderRadius"/> class with one radius for
        /// every corner, written as a bare number.
        /// </summary>
        /// <param name="radius">The radius, in pixels.</param>
        public BorderRadius(int radius)
        {
            Radius = radius;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BorderRadius"/> class corner by corner,
        /// written as an object even when the four agree.
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
        /// One radius for every corner, written as a bare number: <c>BorderRadius = 8</c>.
        /// </summary>
        /// <param name="radius">The radius, in pixels.</param>
        public static implicit operator BorderRadius(int radius) => new BorderRadius(radius);

        /// <summary>
        /// Gets or sets the one radius for every corner.
        /// </summary>
        /// <value>
        /// The radius, in pixels, sent as a bare number while no corner is set. A corner set as well
        /// overrides it for that corner and makes the value an object: <c>new BorderRadius(8) { BottomLeft = 0 }</c>
        /// sends all four corners.
        /// </value>
        /// <remarks>
        /// <see cref="BorderRadiusJsonConverter"/> writes it. The <c>[JsonIgnore]</c> is for the model
        /// walk the key-validation tests make: the object form has no <c>radius</c> key, so this
        /// property must not look like one.
        /// </remarks>
        [JsonIgnore]
        public int? Radius { get; set; }

        /// <summary>
        /// Gets or sets the top left radius.
        /// </summary>
        /// <value>The radius, in pixels; unset, <see cref="Radius"/>.</value>
        [JsonPropertyName("topLeft")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TopLeft { get => _topLeft ?? Radius; set => _topLeft = value; }
        private int? _topLeft;

        /// <summary>
        /// Gets or sets the top right radius.
        /// </summary>
        /// <value>The radius, in pixels; unset, <see cref="Radius"/>.</value>
        [JsonPropertyName("topRight")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TopRight { get => _topRight ?? Radius; set => _topRight = value; }
        private int? _topRight;

        /// <summary>
        /// Gets or sets the bottom left radius.
        /// </summary>
        /// <value>The radius, in pixels; unset, <see cref="Radius"/>.</value>
        [JsonPropertyName("bottomLeft")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BottomLeft { get => _bottomLeft ?? Radius; set => _bottomLeft = value; }
        private int? _bottomLeft;

        /// <summary>
        /// Gets or sets the bottom right radius.
        /// </summary>
        /// <value>The radius, in pixels; unset, <see cref="Radius"/>.</value>
        [JsonPropertyName("bottomRight")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BottomRight { get => _bottomRight ?? Radius; set => _bottomRight = value; }
        private int? _bottomRight;

        /// <summary>
        /// Whether any corner has been set on its own, which makes the value an object.
        /// </summary>
        internal bool HasCorners =>
            _topLeft.HasValue || _topRight.HasValue || _bottomLeft.HasValue || _bottomRight.HasValue;
    }

    /// <summary>
    /// Writes <see cref="BorderRadius"/> in the shape Chart.js reads: <see cref="BorderRadius.Radius"/>
    /// alone as a number, otherwise <c>{"topLeft":…,"topRight":…,"bottomLeft":…,"bottomRight":…}</c>
    /// with every corner that has no value omitted.
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
            if (!value.HasCorners && value.Radius is int radius)
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
