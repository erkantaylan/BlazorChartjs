using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erkan.Blazor.Chartjs.Models.Radar
{
    /// <summary>
    /// Radar Options Scales
    /// </summary>
    public class RadarOptionsScales
    {
        /// <summary>
        /// Gets or sets the scale radius options
        /// </summary>
        /// <value>
        /// The radius options
        /// </value>
        [JsonPropertyName("r")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public RadarOptionsScalesRadius? R { get; set; }
    }
}
