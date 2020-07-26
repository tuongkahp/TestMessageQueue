using System.Collections.Generic;
using System.Threading.Tasks;
using static TestMessageQueue.Services.Sevice.RabbitMQService;

namespace TestMessageQueue.Services.Interface
{
    public interface IRabbitMQService
    {
        Task<bool> CreateMessage(string queueName, string message);
        Task<bool> SendMessage(int totalMessage);
        void GetMessage(string queueName, DelegateFunc delegateFunc);
        Task Close();
    }
}