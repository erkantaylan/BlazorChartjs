namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// Where the legend is placed: one of the four sides of the chart, or inside the chart area.
    /// </summary>
    /// <remarks>
    /// Chart.js types the legend's <c>position</c> as a layout position, which also admits
    /// <c>'center'</c> and a <c>{ scaleId: value }</c> object. Neither places a legend: Chart.js's
    /// layout positions a box at those only if the box belongs to an axis, which scales do and the
    /// legend does not, so the layout never gives such a legend a place. They are not offered here.
    /// </remarks>
    public class LegendPosition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LegendPosition"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        private LegendPosition(string value)
        { Value = value; }

        /// <summary>
        /// Top
        /// </summary>
        /// <value>
        /// Above the chart. This is Chart.js's default.
        /// </value>
        public static LegendPosition Top
        { get { return new LegendPosition("top"); } }

        /// <summary>
        /// Bottom
        /// </summary>
        /// <value>
        /// Bottom
        /// </value>
        public static LegendPosition Bottom
        { get { return new LegendPosition("bottom"); } }

        /// <summary>
        /// Chart area
        /// </summary>
        /// <value>
        /// Inside the chart area. Chart.js lays the legend out vertically there and does not let
        /// its place inside the area be configured.
        /// </value>
        public static LegendPosition ChartArea
        { get { return new LegendPosition("chartArea"); } }

        /// <summary>
        /// Left
        /// </summary>
        /// <value>
        /// Left
        /// </value>
        public static LegendPosition Left
        { get { return new LegendPosition("left"); } }

        /// <summary>
        /// Right.
        /// </summary>
        /// <value>
        /// Right
        /// </value>
        public static LegendPosition Right
        { get { return new LegendPosition("right"); } }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }
    }
}
