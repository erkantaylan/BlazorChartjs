using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Erkan.Blazor.Chartjs.Models.Common.StringEnums
{
    /// <summary>
    /// Class StepMode.
    /// </summary>
    /// <remarks>
    /// Chart.js reads <c>stepped</c> as <c>boolean | 'before' | 'after' | 'middle'</c> and tests it
    /// for truthiness, so <see cref="False"/> and <see cref="True"/> are written as JSON booleans by
    /// <see cref="StepModeJsonConverter"/> — the string <c>"false"</c> is truthy, and drew a stepped line.
    /// </remarks>
    public class StepMode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StepMode"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        private StepMode(string value) { Value = value; }

        /// <summary>
        /// Straight lines between points (the Chart.js default). Written as JSON <c>false</c>.
        /// </summary>
        /// <value>
        /// false.
        /// </value>
        public static StepMode False { get { return new StepMode("false"); } }

        /// <summary>
        /// Stepped line in default mode (Before). Written as JSON <c>true</c>.
        /// </summary>
        /// <value>
        /// true.
        /// </value>
        public static StepMode True { get { return new StepMode("true"); } }

        /// <summary>
        /// Line rises before the point.
        /// </summary>
        /// <value>
        /// before.
        /// </value>
        public static StepMode Before { get { return new StepMode("before"); } }

        /// <summary>
        /// Line rises after point.
        /// </summary>
        /// <value>
        /// after.
        /// </value>
        public static StepMode After { get { return new StepMode("after"); } }

        /// <summary>
        /// Line rises before and falls after the point.
        /// </summary>
        /// <value>
        /// middle.
        /// </value>
        public static StepMode Middle { get { return new StepMode("middle"); } }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }
    }

    /// <summary>
    /// Writes a <c>stepped</c> value in the shape Chart.js expects: <c>"true"</c> and <c>"false"</c>
    /// as the JSON booleans <c>true</c> and <c>false</c>, and every other value as a string.
    /// </summary>
    /// <remarks>
    /// Applied to the string twin rather than to <see cref="StepMode"/>, so the property stays a
    /// plain string on the public API and a raw value set on the twin gets the same treatment.
    /// </remarks>
    public class StepModeJsonConverter : JsonConverter<string>
    {
        /// <inheritdoc />
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            reader.TokenType switch
            {
                JsonTokenType.True => "true",
                JsonTokenType.False => "false",
                JsonTokenType.String => reader.GetString(),
                JsonTokenType.Null => null,
                _ => throw new JsonException("stepped must be a boolean or a string.")
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
