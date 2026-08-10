namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// Padding around a chart title.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Deliberately not the four-sided <see cref="Padding"/>. Chart.js types
    /// <c>plugins.title.padding</c> as <c>number | { top, bottom }</c>, and the title box reads
    /// only <c>padding.height</c> (top + bottom) and <c>padding.top</c> — a horizontal title is
    /// laid out across the full chart width and a vertical one across the full height, so there
    /// is no horizontal extent for a left or right padding to occupy. Passing <c>left</c> or
    /// <c>right</c> is silently discarded, which is why they are not offered here.
    /// </para>
    /// <para>
    /// Chart.js also accepts a single number for all four sides. Since the title discards the
    /// horizontal two, that form is exactly equivalent to the same value on top and bottom:
    /// use <see cref="TitlePadding(int)"/>.
    /// </para>
    /// </remarks>
    public class TitlePadding
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TitlePadding"/> class.
        /// </summary>
        public TitlePadding() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TitlePadding"/> class with the same
        /// padding above and below the title.
        /// </summary>
        /// <param name="size">The padding, in pixels, applied to both top and bottom.</param>
        public TitlePadding(int size)
        {
            Top = size;
            Bottom = size;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TitlePadding"/> class.
        /// </summary>
        /// <param name="top">The top.</param>
        /// <param name="bottom">The bottom.</param>
        public TitlePadding(int? top, int? bottom)
        {
            Top = top;
            Bottom = bottom;
        }

        /// <summary>
        /// Gets or sets the padding above the title.
        /// </summary>
        /// <value>
        /// The top padding, in pixels.
        /// </value>
        [JsonPropertyName("top")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Top { get; set; }

        /// <summary>
        /// Gets or sets the padding below the title.
        /// </summary>
        /// <value>
        /// The bottom padding, in pixels.
        /// </value>
        [JsonPropertyName("bottom")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Bottom { get; set; }
    }
}
