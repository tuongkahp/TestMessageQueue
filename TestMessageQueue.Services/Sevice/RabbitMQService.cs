using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TestMessageQueue.Services.Interface;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TestMessageQueue.Models.Options;
using Newtonsoft.Json;
using TestMessageQueue.Models;
using System.Threading;

namespace TestMessageQueue.Services.Sevice
{
    public class RabbitMQService : IRabbitMQService
    {
        private ConnectionFactory _factory;

        private IConnection _connection;
        private IModel _channel;
        private readonly string _testQueueName;
        public delegate Task<bool> DelegateFunc(string message);

        public RabbitMQService(IOptions<RabbitMqConfiguration> rabbitMqOptions)
        {
            _testQueueName = rabbitMqOptions.Value.QueueName;
            _factory = new ConnectionFactory()
            {
                UserName = rabbitMqOptions.Value.UserName,
                Password = rabbitMqOptions.Value.Password,
                VirtualHost = "/",
                HostName = rabbitMqOptions.Value.Hostname,
                Port = AmqpTcpEndpoint.UseDefaultPort
            };
        }

        public async Task<bool> CreateMessage(string queueName, string message)
        {
            try
            {
                _connection = _factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(queue: queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;

                _channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: properties,
                                     body: body);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<bool> SendMessage(int totalMessage)
        {
            try
            {
                using (var connection = _factory.CreateConnection())
                using (var channel = _connection.CreateModel())
                {
                    channel.QueueDeclare(queue: _testQueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    //prallel is not working.

                    for (int i = 1; i <= totalMessage; i++)
                    {
                        var message = JsonConvert.SerializeObject(new TestMessageModel()
                        {
                            ClientId = i,
                            TimeStamps = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                        });

                        channel.BasicPublish(exchange: "",
                                             routingKey: _testQueueName,
                                             basicProperties: properties,
                                             body: Encoding.UTF8.GetBytes(message));
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void GetMessage(string queueName, DelegateFunc delegateFunc)
        {
            var actualResult = true;

            try
            {
                var connection = _factory.CreateConnection();
                var channel = connection.CreateModel();

                channel.QueueDeclare(queue: queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    actualResult = await delegateFunc(message);

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: !actualResult);
                };

                channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            }
            catch
            {
                var thread = new Thread(() =>
                {
                    Thread.Sleep(30000);
                    GetMessage(queueName, delegateFunc);
                });

                thread.Start();
            }
        }

        public async Task Close()
        {
            try
            {
                _connection.Close();
                _channel.Close();
            }
            catch
            {
            }
        }
    }
}
