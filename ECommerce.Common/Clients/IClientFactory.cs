using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Common.Clients
{
    public interface IClientFactory
    {
        IMessageQueueClient GetQueueClient(string connectionStringKey, string queueName);
    }
}
