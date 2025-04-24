
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SensorDataLoggingService.Models; // Your DTO location

namespace SensorDataLoggingService
{
    public class RabbitMqConsumer : BackgroundService
    {
        private readonly ILogger<RabbitMqConsumer> _logger;
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMqConsumer(ILogger<RabbitMqConsumer> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            InitializeRabbitMq();
        }

        private void InitializeRabbitMq()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:Host"]
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _configuration["RabbitMQ:Queue"],
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var sensorData = JsonSerializer.Deserialize<SensorDataDto>(message);
                    if (sensorData != null)
                    {
                        // Process sensor data (e.g., save to database)
                        _logger.LogInformation($"[✓] Received from queue: {JsonSerializer.Serialize(sensorData)}");

                        // await _yourService.SaveSensorDataAsync(sensorData);
                    }
                    else
                    {
                        _logger.LogWarning("[!] SensorData deserialization returned null.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[!] Error processing message: {ex.Message}");
                }
            };

            _channel.BasicConsume(queue: _configuration["RabbitMQ:Queue"],
                                  autoAck: true,
                                  consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
