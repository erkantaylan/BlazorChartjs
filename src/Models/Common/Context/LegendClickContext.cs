using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// Legend Click Context.
    /// </summary>
    public readonly record struct LegendClickContext(int LegendIndex, string LegendText)
    {
    }
}
