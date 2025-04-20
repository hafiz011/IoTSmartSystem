using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Npgsql;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class MqttSubscriberService : BackgroundService
{
    private readonly ILogger<MqttSubscriberService> _logger;
    private readonly IMqttClient _mqttClient;
    private readonly string _connectionString;

    public MqttSubscriberService(ILogger<MqttSubscriberService> logger)
    {
        _logger = logger;
        _mqttClient = new MqttFactory().CreateMqttClient();
        _connectionString = "Host=localhost;Database=iot_data;Username=postgres;Password=yourpassword";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost") // Replace with your MQTT broker address
            .WithClientId("dotnet_backend_subscriber")
            .Build();

        _mqttClient.UseConnectedHandler(async e =>
        {
            _logger.LogInformation("Connected to MQTT broker");

            // Subscribe to topics
            await _mqttClient.SubscribeAsync("sensor/+/temp");
            await _mqttClient.SubscribeAsync("sensor/+/humidity");
            await _mqttClient.SubscribeAsync("output/+/+/status");

            _logger.LogInformation("Subscribed to topics");
        });

        _mqttClient.UseDisconnectedHandler(e =>
        {
            _logger.LogWarning("Disconnected from MQTT broker");
        });

        _mqttClient.UseApplicationMessageReceivedHandler(async e =>
        {
            try
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                _logger.LogInformation($"Received message on topic: {topic}");
                _logger.LogInformation($"Payload: {payload}");

                await ProcessMessage(topic, payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing MQTT message");
            }
        });

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!_mqttClient.IsConnected)
                {
                    await _mqttClient.ConnectAsync(options, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MQTT connection error");
                await Task.Delay(5000, stoppingToken);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task ProcessMessage(string topic, string payload)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        if (topic.Contains("sensor/") && topic.Contains("/temp"))
        {
            var data = JsonSerializer.Deserialize<TemperatureData>(payload);
            await StoreTemperatureData(connection, data, topic);
        }
        else if (topic.Contains("sensor/") && topic.Contains("/humidity"))
        {
            var data = JsonSerializer.Deserialize<HumidityData>(payload);
            await StoreHumidityData(connection, data, topic);
        }
        else if (topic.Contains("output/") && topic.Contains("/status"))
        {
            var data = JsonSerializer.Deserialize<DeviceStatus>(payload);
            await StoreDeviceStatus(connection, data, topic);
        }
    }

    private async Task StoreTemperatureData(NpgsqlConnection connection, TemperatureData data, string topic)
    {
        var location = GetLocationFromTopic(topic);

        var cmd = new NpgsqlCommand(@"
            INSERT INTO sensor_data (timestamp, sensor_type, location, mac_address, value, unit)
            VALUES (@timestamp, 'temperature', @location, @mac, @value, @unit)",
            connection);

        cmd.Parameters.AddWithValue("timestamp", DateTime.Parse(data.Time));
        cmd.Parameters.AddWithValue("location", location);
        cmd.Parameters.AddWithValue("mac", data.Mac);
        cmd.Parameters.AddWithValue("value", data.Value);
        cmd.Parameters.AddWithValue("unit", data.Unit);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task StoreHumidityData(NpgsqlConnection connection, HumidityData data, string topic)
    {
        var location = GetLocationFromTopic(topic);

        var cmd = new NpgsqlCommand(@"
            INSERT INTO sensor_data (timestamp, sensor_type, location, mac_address, value, unit)
            VALUES (@timestamp, 'humidity', @location, @mac, @value, @unit)",
            connection);

        cmd.Parameters.AddWithValue("timestamp", DateTime.Parse(data.Time));
        cmd.Parameters.AddWithValue("location", location);
        cmd.Parameters.AddWithValue("mac", data.Mac);
        cmd.Parameters.AddWithValue("value", data.Value);
        cmd.Parameters.AddWithValue("unit", data.Unit);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task StoreDeviceStatus(NpgsqlConnection connection, DeviceStatus data, string topic)
    {
        var parts = topic.Split('/');
        var location = parts[1];
        var device = parts[2];

        var cmd = new NpgsqlCommand(@"
            INSERT INTO device_status (timestamp, device_name, location, mac_address, status)
            VALUES (@timestamp, @device, @location, @mac, @status)",
            connection);

        cmd.Parameters.AddWithValue("timestamp", DateTime.Parse(data.Time));
        cmd.Parameters.AddWithValue("device", device);
        cmd.Parameters.AddWithValue("location", location);
        cmd.Parameters.AddWithValue("mac", data.Mac);
        cmd.Parameters.AddWithValue("status", data.Status);

        await cmd.ExecuteNonQueryAsync();
    }

    private string GetLocationFromTopic(string topic)
    {
        var parts = topic.Split('/');
        return parts.Length > 1 ? parts[1] : "unknown";
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _mqttClient.DisconnectAsync();
        _logger.LogInformation("MQTT subscriber service stopped");
        await base.StopAsync(cancellationToken);
    }
}

// Data models
public class TemperatureData
{
    public double Value { get; set; }
    public string Unit { get; set; }
    public string Mac { get; set; }
    public string Time { get; set; }
}

public class HumidityData
{
    public int Value { get; set; }
    public string Unit { get; set; }
    public string Mac { get; set; }
    public string Time { get; set; }
}

public class DeviceStatus
{
    public string Status { get; set; }
    public string Mac { get; set; }
    public string Time { get; set; }
}