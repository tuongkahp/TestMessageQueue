using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TestMessageQueue.Models;
using TestMessageQueue.Models.Options;
using TestMessageQueue.Services.Interface;

namespace TestMessageQueue.Api.Consumer
{
    public class TestMessageConsumer : BackgroundService
    {
        private IServiceWrapper _serviceWrapper;
        private string _queueName;
        private string _logPath;

        public IServiceProvider _services;

        public TestMessageConsumer(IServiceProvider services)
        {
            _services = services;
            DoWork();
        }

        public void DoWork()
        {
            using (var scope = _services.CreateScope())
            {
                _serviceWrapper = scope.ServiceProvider.GetRequiredService<IServiceWrapper>();
                var rabbitmqConfig = scope.ServiceProvider.GetRequiredService<IOptions<RabbitMqConfiguration>>();
                _queueName = rabbitmqConfig.Value.QueueName;
                _logPath = rabbitmqConfig.Value.LogPath;
            }
        }

        private async Task<bool> HandleMessage(string message)
        {
            try
            {
                if (!File.Exists(_logPath))
                {
                    using (var file = new StreamWriter(_logPath, true))
                    {
                        file.WriteLine("Sent Timestamp,Client ID,Received Timestamp");
                    }
                }

                var testMessageModel = JsonConvert.DeserializeObject<TestMessageModel>(message);

                using (var file = new StreamWriter(_logPath, true))
                {
                    file.WriteLine($"{testMessageModel.TimeStamps},{testMessageModel.ClientId},{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}");
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _serviceWrapper.RabbitMQService.GetMessage(_queueName, HandleMessage);
        }
    }
}