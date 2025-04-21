using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var factory = new MqttFactory();
        var mqttClient = factory.CreateMqttClient();

        var mqttOptions = new MqttClientOptionsBuilder()
            .WithClientId("6e349eca997b44bd8337567cf15a9b3b")
            .WithTcpServer("6e349eca997b44bd8337567cf15a9b3b.s1.eu.hivemq.cloud", 8883)
            .WithCredentials("hrhafij8", "Hjgjh435345@")
            .WithCleanSession()
            .WithTls() // Enable SSL/TLS encryption
            .Build();

        string topic = "iot/message";

        mqttClient.ConnectedAsync += async e =>
        {
            Console.WriteLine("Connected successfully.");

            // Subscribe to topic
            await mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(f => { f.WithTopic(topic).WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce); })
                .Build());

            Console.WriteLine($"Subscribed to topic: {topic}");
        };

        mqttClient.DisconnectedAsync += async e =>
        {
            Console.WriteLine("Disconnected from server.");

            // Optional: auto-reconnect
            await Task.Delay(TimeSpan.FromSeconds(5));
            try
            {
                await mqttClient.ConnectAsync(mqttOptions);
                Console.WriteLine("Reconnected.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Reconnection failed: {ex.Message}");
            }
        };

        mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            Console.WriteLine($"\n Message received:");
            Console.WriteLine($"Topic: {e.ApplicationMessage.Topic}");
            Console.WriteLine($"Payload: {payload}");
            return Task.CompletedTask;
        };

        try
        {
            await mqttClient.ConnectAsync(mqttOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($" Connection failed: {ex.Message}");
            return;
        }

        // Publish message
        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload("Hello from MQTTnet 4.2 with TLS!")
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .WithRetainFlag(false)
            .Build();

        await mqttClient.PublishAsync(message);
        Console.WriteLine($" Published message to topic '{topic}'");

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();

        await mqttClient.DisconnectAsync();
    }
}
