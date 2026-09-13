namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// Where the caret sits along the tooltip's horizontal edge, which decides which side of the point
    /// the tooltip is drawn on.
    /// </summary>
    public class TooltipXAlign
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TooltipXAlign"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        private TooltipXAlign(string value)
        { Value = value; }

        /// <summary>
        /// Left
        /// </summary>
        /// <value>
        /// The caret is on the left, so the tooltip is drawn to the right of the point.
        /// </value>
        public static TooltipXAlign Left
        { get { return new TooltipXAlign("left"); } }

        /// <summary>
        /// Center
        /// </summary>
        /// <value>
        /// The tooltip is centred horizontally on the point.
        /// </value>
        public static TooltipXAlign Center
        { get { return new TooltipXAlign("center"); } }

        /// <summary>
        /// Right
        /// </summary>
        /// <value>
        /// The caret is on the right, so the tooltip is drawn to the left of the point.
        /// </value>
        public static TooltipXAlign Right
        { get { return new TooltipXAlign("right"); } }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }
    }
}
