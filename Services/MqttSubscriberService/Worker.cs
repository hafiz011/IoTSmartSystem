using MQTTnet;
using MQTTnet.Client;
using System.Text;
using System.Text.Json;

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

        _mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

            _logger.LogInformation($"Received message:\n  Topic: {topic}\n  Payload: {payload}");

            // Optional: Forward to sensor-data service
            await ForwardToSensorDataLoggingAsync(payload);
        };

        _mqttClient.ConnectedAsync += async e =>
        {
            _logger.LogInformation("Connected to MQTT broker.");
            await _mqttClient.SubscribeAsync("iot/message");
            _logger.LogInformation("Subscribed to topic: iot/message");
        };

        _mqttClient.DisconnectedAsync += async e =>
        {
            _logger.LogWarning("Disconnected from MQTT broker.");
            await Task.Delay(5000);
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
            _logger.LogError($"Connection failed: {ex.Message}");
        }
    }

    private async Task ForwardToSensorDataLoggingAsync(string jsonPayload)
    {
        try
        {
            // Replace this URL with your actual endpoint
            var httpClient = new HttpClient();
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("http://sensordatamicroservice/api/log", content);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Forwarded to Sensor Data Logging Service.");
            }
            else
            {
                _logger.LogWarning($"Failed to forward. Status: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error forwarding to Sensor Data Logging Service: {ex.Message}");
        }
    }
}
