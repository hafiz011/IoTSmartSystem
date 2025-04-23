using Grpc.Core;
using MQTTnet;
using MQTTnet.Client;
using MqttSubscriberService;
using MqttSubscriberService.Models;
using System.Text;
using System.Text.Json;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private IMqttClient _mqttClient;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
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

            // Expecting topic: iot/message/{deviceId}
            var deviceId = topic.Split("/").Last();
            //var deviceId = "d2876c4b-79ec-451e-a218-d578b67f951a";
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                _logger.LogWarning("Device ID is missing in topic.");
                return;
            }

            try
            {
                var sensorData = JsonSerializer.Deserialize<SensorDataDto>(payload);
                if (sensorData == null)
                {
                    _logger.LogWarning("SensorData deserialization failed.");
                    return;
                }

                sensorData.DeviceId = deviceId;
                sensorData.ReceivedAt = DateTime.UtcNow;

                var clientService = new DeviceClientService();
                var deviceResponse = await clientService.GetDeviceInfoAsync(sensorData.DeviceId);

                if (deviceResponse == null)
                {
                    _logger.LogWarning($"Unauthorized device: {sensorData.DeviceId}");
                    return;
                }

                _logger.LogInformation($"Received:\n Topic: {topic}\n Payload: {payload}");

                await ForwardToSensorDataLoggingAsync(sensorData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing message: {ex.Message}");
            }
        };

        _mqttClient.ConnectedAsync += async e =>
        {
            _logger.LogInformation("Connected to MQTT broker.");
            await _mqttClient.SubscribeAsync("iot/message/+"); // `+` for wildcard device ID
            _logger.LogInformation("Subscribed to topic: iot/message/+");
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


    private async Task ForwardToSensorDataLoggingAsync(SensorDataDto data)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            //var httpClient = new HttpClient();
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = _configuration["SensorDataApiUrl"]; // put in appsettings.json
            var response = await httpClient.PostAsync($"{url}/api/log", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully forwarded sensor data.");
            }
            else
            {
                _logger.LogWarning($"Forward failed: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error forwarding sensor data: {ex.Message}");
        }
    }
}
