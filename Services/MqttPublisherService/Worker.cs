using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System.Text;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private IMqttClient _mqttClient;
    private readonly IConfiguration _configuration;
    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();
        var mqttConfig = _configuration.GetSection("MqttConfig");

        var _options = new MqttClientOptionsBuilder()
             .WithClientId(mqttConfig["ClientId"])
             .WithTcpServer(mqttConfig["BrokerAddress"], int.Parse(mqttConfig["BrokerPort"]))
             .WithCredentials(mqttConfig["UserName"], mqttConfig["Password"])
             .WithCleanSession()
             .WithTls()
             .Build();


        _mqttClient.ConnectedAsync += async e =>
        {
            _logger.LogInformation("Connected to MQTT broker.");
            await Task.CompletedTask;
        };

        _mqttClient.DisconnectedAsync += async e =>
        {
            _logger.LogWarning("Disconnected from MQTT broker.");
            await Task.Delay(TimeSpan.FromSeconds(3));
            try
            {
                await _mqttClient.ConnectAsync(_options, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Reconnect failed: {ex.Message}");
            }
        };

        try
        {
            await _mqttClient.ConnectAsync(_options, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Initial connection failed: {ex.Message}");
            return;
        }

        var random = new Random();

        while (!stoppingToken.IsCancellationRequested)
        {
            string topic = "iot/message/d2876c4b-79ec-451e-a218-d578b67f951a";
            var temp = 20 + random.NextDouble() * 10; // 20.0 - 30.0
            var payload = $"{{ \"temperature\": {temp:F2}, \"timestamp\": \"{DateTime.UtcNow:o}\" }}";

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await _mqttClient.PublishAsync(message, stoppingToken);
            _logger.LogInformation($"Published: {payload}");

            await Task.Delay(5000, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_mqttClient.IsConnected)
        {
            await _mqttClient.DisconnectAsync();
        }
        await base.StopAsync(cancellationToken);
    }
}
