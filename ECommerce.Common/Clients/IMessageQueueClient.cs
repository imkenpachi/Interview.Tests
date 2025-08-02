using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Common.Clients
{
    public interface IMessageQueueClient : IDisposable
    {
        /// <summary>
        /// Send a message to the queue
        /// </summary>
        /// <typeparam name="T">The type of the message to be sent</typeparam>
        /// <param name="message">The message</param>
        /// <returns></returns>
        Task SendMessageAsync<T>(T message);

        /// <summary>
        /// Registers a message handler to process incoming messages of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the message to be handled.</typeparam>
        /// <param name="messageHandler">A function that processes a message of type <typeparamref name="T"/>.  The function should return a <see
        /// cref="Task"/> that completes when the message has been processed.</param>
        /// <param name="exceptionHandler">A function that handles exceptions occurring during message processing.  The function should return a <see
        /// cref="Task"/> that completes when the exception has been handled.</param>
        /// <param name="exceptionWithMessageHandler">A function that handles exceptions occurring during message processing and provides access to the message 
        /// that caused the exception. The function should return a <see cref="Task"/> that completes when the exception
        /// and associated message have been handled.</param>
        void RegisterMessageHandler<T>(Func<T, Task> messageHandler,
            Func<Exception, Task>? exceptionHandler,
            Func<Exception, T, Task>? exceptionWithMessageHandler);
    }
}
