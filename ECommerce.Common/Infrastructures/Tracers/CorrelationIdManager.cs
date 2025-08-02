using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;

namespace ECommerce.Common.Infrastructures.Tracers
{
    /// <summary>
    /// <inheritdoc cref="ICorrelationIdManager"/>
    /// </summary>
    public class CorrelationIdManager : ICorrelationIdManager
    {
        private CorrelationModel? _correlation; // Marked as nullable to address CS8601
        private readonly ICorrelationIdGenerator _correlationIdGenerator;
        private readonly ConcurrentQueue<CorrelationModel> _correlationQueue;

        public CorrelationIdManager(ICorrelationIdGenerator correlationIdGenerator)
        {
            _correlationIdGenerator = correlationIdGenerator ?? throw new ArgumentNullException(nameof(correlationIdGenerator));
            _correlationQueue = new ConcurrentQueue<CorrelationModel>();
        }

        public CorrelationModel GetNestedCorrelationDependOnHttpRequest(IHeaderDictionary httpRequestHeaders)
        {
            if (_correlation is not null)
            {
                return _correlation;
            }

            var correlationModel = _correlationIdGenerator.GetNestedCorrelation(httpRequestHeaders);

            _correlationQueue.Enqueue(correlationModel);
            _correlationQueue.TryPeek(out _correlation);

            return _correlation!;
        }

        public CorrelationModel GetCorrelation()
        {
            if (_correlation is not null)
            {
                return _correlation;
            }

            var correlationModel = _correlationIdGenerator.GenerateNewCorrelation();
            _correlationQueue.Enqueue(correlationModel);
            _correlationQueue.TryPeek(out _correlation);
            return _correlation!;
        }

        public CorrelationModel GetNestedCorrelation(CorrelationModel parentCorrelation)
        {
            if (_correlation is not null)
            {
                return _correlation;
            }

            var correlationModel = parentCorrelation is null
                ? _correlationIdGenerator.GenerateNewCorrelation()
                : _correlationIdGenerator.GetNestedCorrelation(parentCorrelation);

            _correlationQueue.Enqueue(correlationModel);
            _correlationQueue.TryPeek(out _correlation);
            return _correlation!;
        }
    }
}
