using Erkan.Blazor.Chartjs.Models.Bubble;
using Erkan.Blazor.Chartjs.Models.Scatter;

namespace Erkan.Blazor.Chartjs.Models.Common 
{
    [JsonDerivedType(typeof(CustomDataset), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(BubbleDataset), typeDiscriminator: "bubbleData")]
    [JsonDerivedType(typeof(ScatterDataset), typeDiscriminator: "scatterData")]
    public class CustomDataset 
    {
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        [JsonPropertyName("label")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets dataset options the scatter and bubble dataset classes have no property
        /// for, written into this entry of <c>data.datasets</c> next to the typed keys.
        /// </summary>
        /// <value>
        /// Each entry becomes one key, spelled exactly as given: <c>["hidden"] = true</c>.
        /// A key must not repeat one a property of the dataset class already writes.
        /// </value>
        [JsonExtensionData]
        public Dictionary<string, object?>? ExtraOptions { get; set; }
    }
    /// <summary>
    /// Datatset for charts
    /// </summary>
    public class CustomDataset<T> : CustomDataset where T : class 
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual List<T> Data { get; set; }
    }
}
