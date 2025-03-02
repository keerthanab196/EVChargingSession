using EVChargingSessionWebAPI.Controllers;

using Microsoft.OpenApi.Writers;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System.Security.Principal;
using System.Text;
using System.Threading;


namespace EVChargingSessionWebAPI.Services
{
    public class MqttService: IHostedService
    {
        private IMqttClient _mqttClient;
        private EVChargingControlService _controlService;
        private ILogger<MqttService> _logger;
       
        public MqttService(EVChargingControlService controlService, ILogger<MqttService> logger)
        {
            var mqttFactory = new MqttFactory();
            _mqttClient = mqttFactory.CreateMqttClient();
            _controlService = controlService;
            _logger = logger;

           
          
        }

       
        public async Task StartAsync(CancellationToken cToken)
        {
           

            var options = new MqttClientOptionsBuilder()
                .WithClientId("MyClient")
                //.WithTcpServer("localhost",1883)
                //.WithWebSocketServer("ws://localhost:9001/mqtt")
                //.WithWebSocketServer(webSocketOptions =>
                //{
                //    webSocketOptions.WithUri("ws://localhost:9001/mqtt");
                //})
                .WithWebSocketServer(s=>s.WithUri("ws://mqtt-broker:9001/mqtt")
                .WithRequestHeaders(new Dictionary<string,string>
                {
                    { "Host","mqtt-broker"}
                }))
                .WithCleanSession()
                .Build();

            var result= await _mqttClient.ConnectAsync(options);

            if (result.ResultCode == MqttClientConnectResultCode.Success)
            {
                _logger.LogInformation("Connected successfully");
            }
            else
            {
                _logger.LogInformation("MQTT Connection failed: {resultcode}",result.ResultCode);
            }

            await PublishMessages("charging/acknowledge", "Hi connected");
            await _mqttClient.SubscribeAsync("charging/start");
            await _mqttClient.SubscribeAsync("charging/stop");
            
            _mqttClient.ApplicationMessageReceivedAsync +=async e =>
            {
                try
                {
                    var topic = e.ApplicationMessage.Topic;

                    var message = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                    _logger.LogInformation($"Received: {message} (Topic: {e.ApplicationMessage.Topic})");

                    if (topic == "charging/start")
                    {
                        var chargingResult = await _controlService.StartCharging();
                        await PublishMessages("charging/start_response", chargingResult);
                    }
                    else if (topic == "charging/stop")
                    {
                        var stopChargingResult = await _controlService.StopCharging(message);
                        await PublishMessages("charging/stop_response", stopChargingResult);

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.Message,"Error in getting MQTT messages");
                }
                

            };
           
        }

        public async Task StopAsync(CancellationToken cToken)
        {
            if (_mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync();
                
            }
        }
        public async Task PublishMessages(string topic, string message)
        {
            try
            {
                if (!_mqttClient.IsConnected)
                {
                    await StartAsync(CancellationToken.None);
                }
                var mqttMsg = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(message)
                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build();
                await _mqttClient.PublishAsync(mqttMsg);
                _logger.LogInformation("Published the message to topic:{topic}",topic);
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex, "Error publishing to topic:{topic}", topic);
            }
        }
        public async Task SubscribeToMessages(string topic)
        {
            if (!_mqttClient.IsConnected)
            {
                await StartAsync(CancellationToken.None);
            }
            await _mqttClient.SubscribeAsync(topic);
            Console.WriteLine($"[MQTT] Subscribed to topic: {topic}");
        }
    }

}
