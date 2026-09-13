namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// Class BorderJoinStyle. How two segments of a line meet, as the canvas <c>lineJoin</c>.
    /// </summary>
    public class BorderJoinStyle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BorderJoinStyle"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        private BorderJoinStyle(string value) { Value = value; }

        /// <summary>
        /// A flat corner.
        /// </summary>
        /// <value>bevel.</value>
        public static BorderJoinStyle Bevel { get { return new BorderJoinStyle("bevel"); } }

        /// <summary>
        /// A sharp corner (the Chart.js default).
        /// </summary>
        /// <value>miter.</value>
        public static BorderJoinStyle Miter { get { return new BorderJoinStyle("miter"); } }

        /// <summary>
        /// A rounded corner.
        /// </summary>
        /// <value>round.</value>
        public static BorderJoinStyle Round { get { return new BorderJoinStyle("round"); } }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }
    }
}
