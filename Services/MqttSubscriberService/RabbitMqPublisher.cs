using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttSubscriberService
{
    public class RabbitMqPublisher
    {
        private readonly string _host;
        private readonly string _queueName;

        public RabbitMqPublisher(string host, string queueName)
        {
            _host = host;
            _queueName = queueName;
        }

        public void Publish(string message)
        {
            var factory = new ConnectionFactory() { HostName = _host };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
        }
    }
}
