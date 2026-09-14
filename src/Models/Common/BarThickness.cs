using System.Globalization;
using System.Text.Json;

namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// The width of each bar: a number of pixels, or <see cref="Flex"/>.
    /// </summary>
    /// <remarks>
    /// Chart.js reads <c>barThickness</c> as <c>number | 'flex'</c>, and
    /// <see cref="BarThicknessJsonConverter"/> writes it that way. Assign a number directly:
    /// <c>BarThickness = 12</c>. <c>default(BarThickness)</c> is <see cref="Flex"/>; a property left
    /// unset is a <c>null</c> <c>BarThickness?</c> and writes nothing.
    /// </remarks>
    [JsonConverter(typeof(BarThicknessJsonConverter))]
    public readonly struct BarThickness
    {
        private BarThickness(int? pixels) { Pixels = pixels; }

        /// <summary>
        /// Sizes each category from the distance to its neighbours instead of the smallest gap on
        /// the axis, so bars on an unevenly spaced axis fill the room they have.
        /// <c>categoryPercentage</c> and <c>barPercentage</c> still apply. Chart.js 4.5.1 computes no
        /// width for it on a dataset with <c>grouped: false</c>, and draws nothing.
        /// </summary>
        public static BarThickness Flex => new BarThickness(null);

        /// <summary>
        /// A fixed width, in pixels. <c>barPercentage</c> and <c>categoryPercentage</c> are ignored.
        /// </summary>
        /// <param name="pixels">The width of each bar, in pixels.</param>
        public static implicit operator BarThickness(int pixels) => new BarThickness(pixels);

        /// <summary>
        /// Gets the fixed width.
        /// </summary>
        /// <value>The width in pixels, or <c>null</c> for <see cref="Flex"/>.</value>
        public int? Pixels { get; }

        /// <summary>
        /// Gets whether this is <see cref="Flex"/>.
        /// </summary>
        public bool IsFlex => Pixels is null;

        /// <inheritdoc />
        public override string ToString() => Pixels?.ToString(CultureInfo.InvariantCulture) ?? "flex";
    }

    /// <summary>
    /// Writes <see cref="BarThickness"/> as a JSON number, or as the string <c>"flex"</c>.
    /// </summary>
    public class BarThicknessJsonConverter : JsonConverter<BarThickness>
    {
        /// <inheritdoc />
        public override BarThickness Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            reader.TokenType switch
            {
                JsonTokenType.Number => reader.GetInt32(),
                JsonTokenType.String when reader.GetString() == "flex" => BarThickness.Flex,
                _ => throw new JsonException("barThickness must be a number or \"flex\".")
            };

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, BarThickness value, JsonSerializerOptions options)
        {
            if (value.Pixels is int pixels)
                writer.WriteNumberValue(pixels);
            else
                writer.WriteStringValue("flex");
        }
    }
}
