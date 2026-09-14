namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// Class BorderSkipped. The edge of a bar that is drawn without a border.
    /// </summary>
    /// <remarks>
    /// Chart.js reads <c>borderSkipped</c> as
    /// <c>'start' | 'end' | 'middle' | 'bottom' | 'left' | 'top' | 'right' | boolean</c> and tests it
    /// for truthiness, so <see cref="False"/> and <see cref="True"/> are written as JSON booleans by
    /// <see cref="Erkan.Blazor.Chartjs.Models.Common.StringEnums.BooleanStringJsonConverter"/>, as
    /// <c>stepped</c> is. A corner on a skipped edge is never rounded.
    /// </remarks>
    public class BorderSkipped
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BorderSkipped"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        private BorderSkipped(string value) { Value = value; }

        /// <summary>
        /// The edge the bar grows from (the Chart.js default): the bottom of an upward vertical bar,
        /// the left of a horizontal bar that grows to the right.
        /// </summary>
        /// <value>start.</value>
        public static BorderSkipped Start { get { return new BorderSkipped("start"); } }

        /// <summary>
        /// The edge at the end of the bar, opposite <see cref="Start"/>.
        /// </summary>
        /// <value>end.</value>
        public static BorderSkipped End { get { return new BorderSkipped("end"); } }

        /// <summary>
        /// In a stack, the edges where bars meet, so the stack is outlined as one, and every bar in
        /// it can be rounded; the two outer ends keep their border. Outside a stack, no edge.
        /// Chart.js 4.5.1 finds the bottom of a stack of positive values by dataset index 0, so in
        /// a stack that does not start with the first dataset the base edge is skipped as well.
        /// </summary>
        /// <value>middle.</value>
        public static BorderSkipped Middle { get { return new BorderSkipped("middle"); } }

        /// <summary>
        /// The bottom edge, mirrored to the top on a vertical bar that grows downwards.
        /// </summary>
        /// <value>bottom.</value>
        public static BorderSkipped Bottom { get { return new BorderSkipped("bottom"); } }

        /// <summary>
        /// The left edge, mirrored to the right on a horizontal bar that grows leftwards.
        /// </summary>
        /// <value>left.</value>
        public static BorderSkipped Left { get { return new BorderSkipped("left"); } }

        /// <summary>
        /// The top edge, mirrored to the bottom on a vertical bar that grows downwards.
        /// </summary>
        /// <value>top.</value>
        public static BorderSkipped Top { get { return new BorderSkipped("top"); } }

        /// <summary>
        /// The right edge, mirrored to the left on a horizontal bar that grows leftwards.
        /// </summary>
        /// <value>right.</value>
        public static BorderSkipped Right { get { return new BorderSkipped("right"); } }

        /// <summary>
        /// No edge: the border is drawn all the way round and every corner can be rounded.
        /// Written as JSON <c>false</c>.
        /// </summary>
        /// <value>false.</value>
        public static BorderSkipped False { get { return new BorderSkipped("false"); } }

        /// <summary>
        /// Every edge: no border, and no rounded corners. Written as JSON <c>true</c>.
        /// </summary>
        /// <value>true.</value>
        public static BorderSkipped True { get { return new BorderSkipped("true"); } }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }
    }
}
