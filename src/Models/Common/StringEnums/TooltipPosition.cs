namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// The built-in tooltip positioners: where Chart.js puts the tooltip relative to the items it describes.
    /// </summary>
    /// <remarks>
    /// A positioner registered in page script as <c>Chart.Tooltip.positioners.myName</c> is set through
    /// <see cref="Tooltip.PositionString"/> instead.
    /// </remarks>
    public class TooltipPosition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TooltipPosition"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        private TooltipPosition(string value)
        { Value = value; }

        /// <summary>
        /// Average
        /// </summary>
        /// <value>
        /// At the average position of all the items the tooltip shows. The Chart.js default.
        /// </value>
        public static TooltipPosition Average
        { get { return new TooltipPosition("average"); } }

        /// <summary>
        /// Nearest
        /// </summary>
        /// <value>
        /// At the item nearest to the pointer.
        /// </value>
        public static TooltipPosition Nearest
        { get { return new TooltipPosition("nearest"); } }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }
    }
}
