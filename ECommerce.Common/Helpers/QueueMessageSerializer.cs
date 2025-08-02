using ECommerce.Common.Infrastructures.MessageQueue;
using ECommerce.Common.Infrastructures.Tracers;
using Newtonsoft.Json;
using Serilog;

namespace ECommerce.Common.Helpers
{
    internal static class QueueMessageSerializer
    {
        internal static string SerializeObject<T>(T value, CorrelationModel correlationModel)
        {
            var messageWithCorrelationId = new QueueMessageWithCorrelationId<T>
            {
                Data = value,
                CorrelationId = correlationModel.GetCorretionId(),
                CorrelationModel = correlationModel,
            };

            return JsonConvert.SerializeObject(messageWithCorrelationId);
        }

        internal static QueueMessageWithCorrelationId<T>? DeserializeObject<T>(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return new QueueMessageWithCorrelationId<T>();
            }

            var result = JsonConvert.DeserializeObject<QueueMessageWithCorrelationId<T>>(data);

            if (string.IsNullOrEmpty(result?.CorrelationId))
            {
                Log.Warning("CorrelationId value is empty in the received message of type {Type}", typeof(T));
                return result;
            }

            result.CorrelationModel = new CorrelationModel
            {
                TraceId = result.CorrelationId,
            };

            return result;
        }
    }
}
