using System.Text.Json;

namespace Erkan.Blazor.Chartjs.Models.Common.StringEnums
{
    /// <summary>
    /// Writes the string twin of a string enum whose Chart.js option is <c>boolean | string</c>:
    /// <c>"true"</c> and <c>"false"</c> as the JSON booleans <c>true</c> and <c>false</c>, and every
    /// other value as a string.
    /// </summary>
    /// <remarks>
    /// Chart.js tests such an option for truthiness, and the string <c>"false"</c> is truthy, so a
    /// <c>False</c> written as a string does the opposite of what it says. Used by
    /// <c>LineDataset.StepModeString</c> (<c>stepped</c>) and <c>BarDataset.BorderSkippedString</c>
    /// (<c>borderSkipped</c>). Applied to the string twin rather than to the string-enum class, so
    /// the property stays a plain string on the public API and a raw value set on the twin gets the
    /// same treatment.
    /// </remarks>
    public class BooleanStringJsonConverter : JsonConverter<string>
    {
        /// <inheritdoc />
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            reader.TokenType switch
            {
                JsonTokenType.True => "true",
                JsonTokenType.False => "false",
                JsonTokenType.String => reader.GetString(),
                JsonTokenType.Null => null,
                _ => throw new JsonException("Expected a boolean or a string.")
            };

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case "true":
                    writer.WriteBooleanValue(true);
                    break;
                case "false":
                    writer.WriteBooleanValue(false);
                    break;
                default:
                    writer.WriteStringValue(value);
                    break;
            }
        }
    }
}
