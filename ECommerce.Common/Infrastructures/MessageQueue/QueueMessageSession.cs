using ECommerce.Common.Helpers;
using ECommerce.Common.Infrastructures.Tracers;
using System.Text;

namespace ECommerce.Common.Infrastructures.MessageQueue
{
    public class QueueMessageSession<T>
    {
        public CorrelationModel? CorrelationModel { get; set; }
        public QueueMessageWithCorrelationId<T>? QueueMessage
        { 
            get 
            {
                if (MessageData == null)
                {
                    return null;
                }
                return QueueMessageSerializer.DeserializeObject<T>(Encoding.UTF8.GetString(MessageData));
            } 
        }

        public long MessageSize { get; set; }
        public byte[]? MessageData { get; set; }
        public ICorrelationIdManager? CorrelationIdManager { get; set; }

        public QueueMessageSession(CorrelationModel correlationModel)
        {
            CorrelationModel = correlationModel;
        }
    }
}
