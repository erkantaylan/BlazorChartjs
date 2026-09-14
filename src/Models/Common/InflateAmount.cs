using System.Globalization;
using System.Text.Json;

namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// How far each bar is drawn beyond its own edges: a number of pixels, or <see cref="Auto"/>.
    /// </summary>
    /// <remarks>
    /// Chart.js reads <c>inflateAmount</c> as <c>number | 'auto'</c>, and
    /// <see cref="InflateAmountJsonConverter"/> writes it that way. Assign an <c>int</c> or a
    /// <c>decimal</c> directly: <c>InflateAmount = 0</c>, <c>InflateAmount = 0.5m</c>. <c>default(InflateAmount)</c> is <see cref="Auto"/>, which is also
    /// the Chart.js default; a property left unset is a <c>null</c> <c>InflateAmount?</c> and
    /// writes nothing.
    /// </remarks>
    [JsonConverter(typeof(InflateAmountJsonConverter))]
    public readonly struct InflateAmount
    {
        private InflateAmount(decimal? pixels) { Pixels = pixels; }

        /// <summary>
        /// Inflates by 0.33 pixels when <c>barThickness</c> is set — a width or <c>'flex'</c> — or when
        /// <c>barPercentage</c> and <c>categoryPercentage</c> are both <c>1</c>, which hides the
        /// hairline anti-aliasing leaves between bars that touch; by nothing otherwise.
        /// </summary>
        public static InflateAmount Auto => new InflateAmount(null);

        /// <summary>
        /// A fixed amount, in pixels.
        /// </summary>
        /// <param name="pixels">The amount, in pixels. <c>0</c> draws each bar at its exact size.</param>
        public static implicit operator InflateAmount(decimal pixels) => new InflateAmount(pixels);

        /// <summary>
        /// Gets the fixed amount.
        /// </summary>
        /// <value>The amount in pixels, or <c>null</c> for <see cref="Auto"/>.</value>
        public decimal? Pixels { get; }

        /// <summary>
        /// Gets whether this is <see cref="Auto"/>.
        /// </summary>
        public bool IsAuto => Pixels is null;

        /// <inheritdoc />
        public override string ToString() => Pixels?.ToString(CultureInfo.InvariantCulture) ?? "auto";
    }

    /// <summary>
    /// Writes <see cref="InflateAmount"/> as a JSON number, or as the string <c>"auto"</c>.
    /// </summary>
    public class InflateAmountJsonConverter : JsonConverter<InflateAmount>
    {
        /// <inheritdoc />
        public override InflateAmount Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            reader.TokenType switch
            {
                JsonTokenType.Number => reader.GetDecimal(),
                JsonTokenType.String when reader.GetString() == "auto" => InflateAmount.Auto,
                _ => throw new JsonException("inflateAmount must be a number or \"auto\".")
            };

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, InflateAmount value, JsonSerializerOptions options)
        {
            if (value.Pixels is decimal pixels)
                writer.WriteNumberValue(pixels);
            else
                writer.WriteStringValue("auto");
        }
    }
}
