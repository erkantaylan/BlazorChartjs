namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// Class BorderCapStyle. How the ends of a line are drawn, as the canvas <c>lineCap</c>.
    /// </summary>
    public class BorderCapStyle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BorderCapStyle"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        private BorderCapStyle(string value) { Value = value; }

        /// <summary>
        /// Flat ends that stop at the end point (the Chart.js default).
        /// </summary>
        /// <value>butt.</value>
        public static BorderCapStyle Butt { get { return new BorderCapStyle("butt"); } }

        /// <summary>
        /// Rounded ends that extend past the end point by half the line width.
        /// </summary>
        /// <value>round.</value>
        public static BorderCapStyle Round { get { return new BorderCapStyle("round"); } }

        /// <summary>
        /// Square ends that extend past the end point by half the line width.
        /// </summary>
        /// <value>square.</value>
        public static BorderCapStyle Square { get { return new BorderCapStyle("square"); } }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }
    }
}
