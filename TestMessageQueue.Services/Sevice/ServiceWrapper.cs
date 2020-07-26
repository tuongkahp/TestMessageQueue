using Microsoft.Extensions.Options;
using System;
using TestMessageQueue.Models.Options;
using TestMessageQueue.Services.Interface;

namespace TestMessageQueue.Services.Sevice
{
    public class ServiceWrapper : IServiceWrapper
    {
        private IRabbitMQService _rabbitMQService;
        private IOptions<RabbitMqConfiguration> _rabbitMqOptions;
        private object _obj;

        public ServiceWrapper(IOptions<RabbitMqConfiguration> rabbitMqOptions)
        {  
            _rabbitMqOptions = rabbitMqOptions;
            _obj = new object();
        }

        public IRabbitMQService RabbitMQService
        {
            get
            {
                if (_rabbitMQService == null)
                {
                    lock (_obj)
                    {
                        if (_rabbitMQService == null)
                        {
                            _rabbitMQService = new RabbitMQService(_rabbitMqOptions);
                        }
                    }
                }

                return _rabbitMQService;
            }
        }
    }
}
