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
    /// <see cref="BooleanStringJsonConverter"/> — the string <c>"false"</c> is truthy, and drew a stepped line.
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
}
