using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;

namespace MqttListenerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IManagedMqttClient _mqttClient;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var options = new MqttClientOptionsBuilder()
                .WithClientId("MqttListenerClient")
                .WithTcpServer("k2cd0116.ala.eu-central-1.emqxsl.com", 8883)
                .WithCredentials("your-username", "your-password") // optional
                .WithTls()
                .Build();

            var managedOptions = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(options)
                .Build();

            _mqttClient = new MqttFactory().CreateManagedMqttClient();

            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var payload = e.ApplicationMessage?.Payload == null
                    ? null
                    : System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                _logger.LogInformation($"MQTT Message Received: Topic = {e.ApplicationMessage.Topic}, Payload = {payload}");

                // TODO: Parse payload, save to MongoDB, etc.
                await Task.CompletedTask;
            };

            await _mqttClient.StartAsync(managedOptions);
            await _mqttClient.SubscribeAsync("iot/device/#");

            _logger.LogInformation("MQTT Client Started & Subscribed.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000); // Just keeping the background task alive
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _mqttClient.StopAsync();
            _logger.LogInformation("MQTT Client Stopped.");
        }
    }
}
