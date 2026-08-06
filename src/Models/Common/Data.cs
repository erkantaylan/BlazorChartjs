using Erkan.Blazor.Chartjs.Models.Bar;
using Erkan.Blazor.Chartjs.Models.Bubble;
using Erkan.Blazor.Chartjs.Models.Doughnut;
using Erkan.Blazor.Chartjs.Models.Line;
using Erkan.Blazor.Chartjs.Models.Pie;
using Erkan.Blazor.Chartjs.Models.Polar;
using Erkan.Blazor.Chartjs.Models.Radar;
using Erkan.Blazor.Chartjs.Models.Scatter;

namespace Erkan.Blazor.Chartjs.Models.Common 
{
    /// <summary>
    /// Data for Charts
    /// </summary>
    [JsonDerivedType(typeof(Data), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(BarData), typeDiscriminator: "barData")]
    [JsonDerivedType(typeof(DoughnutData), typeDiscriminator: "doughnutData")]
    [JsonDerivedType(typeof(LineData), typeDiscriminator: "lineData")]
    [JsonDerivedType(typeof(PieData), typeDiscriminator: "pieData")]
    [JsonDerivedType(typeof(PolarData), typeDiscriminator: "polarData")]
    [JsonDerivedType(typeof(RadarData), typeDiscriminator: "radarData")]
    [JsonDerivedType(typeof(BubbleData), typeDiscriminator: "bubbleData")]
    [JsonDerivedType(typeof(ScatterData), typeDiscriminator: "scatterData")]
    public class Data 
    {
        /// <summary>
        /// Gets or sets the labels.
        /// </summary>
        /// <value>
        /// The labels.
        /// </value>
        [JsonPropertyName("labels")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string> Labels { get; set; } = new List<string>();
    }

    /// <summary>
    /// Data for Charts
    /// </summary>
    public class Data<T> : Data where T : class 
    {
        /// <summary>
        /// Gets or sets the datasets.
        /// </summary>
        /// <value>
        /// The datasets.
        /// </value>
        [JsonPropertyName("datasets")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<T> Datasets { get; set; } = new List<T>();
    }
}
