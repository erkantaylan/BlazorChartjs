namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// The horizontal alignment of a block of text inside its box.
    /// </summary>
    public class TextAlign
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextAlign"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        private TextAlign(string value)
        { Value = value; }

        /// <summary>
        /// Left
        /// </summary>
        /// <value>
        /// Left
        /// </value>
        public static TextAlign Left
        { get { return new TextAlign("left"); } }

        /// <summary>
        /// Center
        /// </summary>
        /// <value>
        /// Center
        /// </value>
        public static TextAlign Center
        { get { return new TextAlign("center"); } }

        /// <summary>
        /// Right
        /// </summary>
        /// <value>
        /// Right
        /// </value>
        public static TextAlign Right
        { get { return new TextAlign("right"); } }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }
    }
}
