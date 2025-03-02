using EVChargingSessionWebAPI.Models;

namespace EVChargingSessionWebAPI.Services
{
    public class EnergySimulationService : BackgroundService
    {
        private EVChargingControlService _eVChargingControlService;
        private MqttService _mqttService;
        private ILogger<EnergySimulationService> _logger;
        public EnergySimulationService(EVChargingControlService eVChargingControlService, MqttService mqttService,ILogger<EnergySimulationService> logger)
        {
            _eVChargingControlService = eVChargingControlService;
            _mqttService = mqttService;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken cToken)
        {
            while (!cToken.IsCancellationRequested)
            {
                try
                {
                    var session = await _eVChargingControlService.GetLatestSession();
                    if (session != null && session.Status.Equals("Charging"))
                    {
                        session.EnergyConsumed += 0.5;

                        await _eVChargingControlService.UpdateEnergyConsumption(session.SessionId, session);

                        string mqttMessage = $"{{ \"sessionId\": \"{session.SessionId}\", \"status\": \"Charging\", \"energyConsumed\": {session.EnergyConsumed} }}";

                        await _mqttService.PublishMessages("charging/energySimulation", mqttMessage);

                    }
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during energy simulation iteration.");
                }

                await Task.Delay(1000, cToken);
            }
        }
    }
}
