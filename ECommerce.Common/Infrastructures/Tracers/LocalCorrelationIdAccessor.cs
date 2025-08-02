using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Common.Infrastructures.Tracers
{
    /// <summary>
    /// <inheritdoc cref="ILocalCorrelationIdAccessor"/>
    /// </summary>
    public class LocalCorrelationIdAccessor : ILocalCorrelationIdAccessor
    {
        private static readonly AsyncLocal<CorrelationModel> _correlation = new();

        public CorrelationModel Correlation
        {
            get => _correlation.Value ?? new CorrelationModel();
            set => _correlation.Value = value;
        }
    }
}
