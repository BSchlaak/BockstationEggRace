using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Bockstation.EggRace.Core.Mqtt
{
    public class Connector
    {
        private const string Topic = "/home/data";

        private readonly IMqttClient _client;
        private readonly IMqttClientOptions _options;
        private readonly IManagedMqttClient _managedClient;

        public EventHandler<Tuple<long, long, long, long>> MessageReceived;

        public Connector(string server, int? port = null)
        {
            var factory = new MQTTnet.MqttFactory();

            _client = factory.CreateMqttClient();

            _managedClient = factory.CreateManagedMqttClient();
            _managedClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(ConnectedHandler);
            _managedClient.ApplicationMessageProcessedHandler =
                new ApplicationMessageProcessedHandlerDelegate(ApplicationMessageProcessedHandler);
            _managedClient.ApplicationMessageSkippedHandler =
                new ApplicationMessageSkippedHandlerDelegate(ApplicationMessageSkippedHandler);

            _options = new MqttClientOptionsBuilder()
                .WithClientId(nameof(Connector))
                .WithTcpServer(server, port)
                .Build();
        }

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"Connecting to: {_options.ClientId}");
            var authenticateResult = await _client.ConnectAsync(_options, cancellationToken);
            Console.WriteLine($"Authenticate Result: {authenticateResult.ResultCode} | Reason: {authenticateResult.ReasonString}");

            Console.WriteLine($"Subscribing to topic: {Topic}");
            var subscribeResult = await _client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(Topic).Build());
            Console.WriteLine($"Subscribe Result: {string.Join(" | ", subscribeResult.Items.Select(i => i.ResultCode.ToString()))}");

            _client.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine($"Received message: {e.ApplicationMessage.Topic}");

                if (cancellationToken.CanBeCanceled && cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine($"Connection should be cancelled - disconnecting from: {_options.ClientId}");
                    _client.DisconnectAsync(cancellationToken);
                }
                else
                {
                    var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                    Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                    Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                    Console.WriteLine($"+ Payload = {payload}");
                    Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                    Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                    Console.WriteLine();

                    var timestamps = JsonSerializer.Deserialize<long[]>(payload);
                    MessageReceived?.Invoke(this, new Tuple<long, long, long, long>(timestamps[0], timestamps[1], timestamps[2], timestamps[3]));
                }
            });
        }

        public async Task ConnectManagedAsync()
        {
            Console.WriteLine($"Connecting to: {_options.ClientId}");
            await _managedClient.StartAsync(new ManagedMqttClientOptions
            {
                ClientOptions = _options
            });

            Console.WriteLine($"Subscribing to topic: {Topic}");
            await _managedClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(Topic).Build());
        }

        private void ConnectedHandler(MqttClientConnectedEventArgs obj)
        {
            Console.WriteLine($"CONNECTED: Result={obj.AuthenticateResult.ResultCode} | Reason={obj.AuthenticateResult.ReasonString}");
        }

        private void ApplicationMessageProcessedHandler(ApplicationMessageProcessedEventArgs obj)
        {
            Console.WriteLine($"PROCESSED: {obj.ApplicationMessage.ApplicationMessage.Topic} | {Encoding.UTF8.GetString(obj.ApplicationMessage.ApplicationMessage.Payload)}");
        }

        private void ApplicationMessageSkippedHandler(ApplicationMessageSkippedEventArgs obj)
        {
            Console.WriteLine($"SKIPPED: {obj.ApplicationMessage.ApplicationMessage.Topic} | {Encoding.UTF8.GetString(obj.ApplicationMessage.ApplicationMessage.Payload)}");
        }
    }
}
