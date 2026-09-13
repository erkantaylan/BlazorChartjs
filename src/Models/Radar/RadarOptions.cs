namespace Erkan.Blazor.Chartjs.Models.Radar
{
    /// <summary>
    /// Radar Options
    /// </summary>
    public class RadarOptions : IOptions
    {
        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        [JsonPropertyName("elements")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public RadarOptionsElements? Elements { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [maintain aspect ratio].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [maintain aspect ratio]; otherwise, <c>false</c>.
        /// </value>
        [JsonPropertyName("maintainAspectRatio")]
        public bool MaintainAspectRatio { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Options"/> is responsive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if responsive; otherwise, <c>false</c>.
        /// </value>
        [JsonPropertyName("responsive")]
        public bool Responsive { get; set; } = true;

        /// <summary>
        /// Gets or sets the the scales
        /// </summary>
        /// <value>
        /// The scales options
        /// </value>
        [JsonPropertyName("scales")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public RadarOptionsScales? Scales { get; set; }

        /// <summary>
        /// Gets or sets the Chart.js plugins to attach to this chart, by the global name their
        /// script defines on <c>window</c>. Attached as <see cref="Options.RegisterPlugins"/>
        /// attaches them; a radar chart has no <c>Plugins</c>, so the plugin's options go in
        /// <see cref="ExtraOptions"/> under <c>["plugins"]</c>.
        /// </summary>
        /// <value>The global names, or <c>null</c> to attach none.</value>
        [JsonPropertyName("registerPlugins")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? RegisterPlugins { get; set; }

        /// <summary>
        /// Gets or sets Chart.js options this class has no property for, written into
        /// <c>options</c> next to the typed keys.
        /// </summary>
        /// <value>
        /// Each entry becomes one key, spelled exactly as given. <see cref="RadarOptions"/> has no
        /// <c>Plugins</c> property, so this is where a radar chart's legend, title and tooltip
        /// go: <c>["plugins"] = new { legend = new { position = "right" } }</c>.
        /// A key must not repeat one a property of this class already writes.
        /// </value>
        [JsonExtensionData]
        public Dictionary<string, object?>? ExtraOptions { get; set; }
    }
}
