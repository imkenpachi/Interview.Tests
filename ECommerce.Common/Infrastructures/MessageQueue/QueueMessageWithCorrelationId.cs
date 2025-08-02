using ECommerce.Common.Infrastructures.Tracers;
using System.Text.Json.Serialization;

namespace ECommerce.Common.Infrastructures.MessageQueue
{
    public class QueueMessageWithCorrelationId<T>
    {
        public T? Data { get; set; }
        public string? CorrelationId { get; set; }

        [JsonIgnore]
        public CorrelationModel? CorrelationModel { get; set; }
    }
}
