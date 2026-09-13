namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// Where the caret sits along the tooltip's vertical edge, which decides whether the tooltip is
    /// drawn above, below or level with the point.
    /// </summary>
    public class TooltipYAlign
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TooltipYAlign"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        private TooltipYAlign(string value)
        { Value = value; }

        /// <summary>
        /// Top
        /// </summary>
        /// <value>
        /// The caret is on top, so the tooltip is drawn below the point.
        /// </value>
        public static TooltipYAlign Top
        { get { return new TooltipYAlign("top"); } }

        /// <summary>
        /// Center
        /// </summary>
        /// <value>
        /// The tooltip is centred vertically on the point, with the caret on its left or right side.
        /// </value>
        public static TooltipYAlign Center
        { get { return new TooltipYAlign("center"); } }

        /// <summary>
        /// Bottom
        /// </summary>
        /// <value>
        /// The caret is at the bottom, so the tooltip is drawn above the point.
        /// </value>
        public static TooltipYAlign Bottom
        { get { return new TooltipYAlign("bottom"); } }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }
    }
}
