using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Common.Infrastructures.Tracers
{
    public class CorrelationModel
    {
        public string? TraceId { get; set; }
        public string? GetCorretionId()
        {
            return TraceId;
        }
    }
}
