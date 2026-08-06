using Erkan.Blazor.Chartjs.Models.Bar;
using Erkan.Blazor.Chartjs.Models.Bubble;
using Erkan.Blazor.Chartjs.Models.Doughnut;
using Erkan.Blazor.Chartjs.Models.Line;
using Erkan.Blazor.Chartjs.Models.Pie;
using Erkan.Blazor.Chartjs.Models.Polar;
using Erkan.Blazor.Chartjs.Models.Radar;
using Erkan.Blazor.Chartjs.Models.Scatter;

namespace Erkan.Blazor.Chartjs.Interfaces 
{
    public interface IChartConfig 
    {
        string CanvasId { get; }
        string Type { get; set; }
        IOptions Options { get; }
    }
}
