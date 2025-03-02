
using EVChargingSessionWebAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EVChargingSessionWebAPI.Services
{
    public class EVChargingControlService
    {
        private  EVChargingDBService _eVChargingDBService;
        private ILogger<EVChargingControlService> _logger;
        
        public EVChargingControlService(EVChargingDBService eVChargingDBService,ILogger<EVChargingControlService> logger)
        {
            _eVChargingDBService = eVChargingDBService;
            _logger = logger;
            
        }

        public async Task<ChargingSession> GetLatestSession()
        {
            try
            {
                ChargingSession session = await _eVChargingDBService.GetLatestChargingSessionAsync();
                if (session == null)
                {
                    _logger.LogInformation("Session does not exist");
                }
                return session;
            }
            catch(Exception e)
            {
                _logger.LogInformation(e, "Error retrieving latest session");
                return null;
                
            }

            
            
            
        }

        
        public async Task<String> StartCharging()
        {
            try
            {
                var session = new ChargingSession
                {
                    SessionId = Guid.NewGuid().ToString(),
                    StartTime = DateTime.Now,
                    Status = "Charging",
                    EnergyConsumed = 0
                };
                await _eVChargingDBService.StartChargingSessionAsync(session);

                string mqttMessage = $"{{ \"sessionId\": \"{session.SessionId}\", \"status\": \"Charging\", \"startTime\": \"{session.StartTime}\" }}";
             
                return mqttMessage;
                
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message, "An error occcurred");
                string mqttMessage = $"{{ \"sessionId\": \"Not Available\", \"status\": \"Unable to start charging\"}}";
                return mqttMessage;
            }


        }

        
        public async Task<String> StopCharging(string sessionId)
        {
            string mqttMessage = "";
            try
            {
                
                var session = await _eVChargingDBService.GetChargingSessionAsync(sessionId);
                if (session == null)
                {
                    mqttMessage = $"{{ \"sessionId\": \"{sessionId}\",\"status\": \"Session does not exist\"}}";
                    return mqttMessage;
                }
                if(session.Status== "Charging")
                {
                    session.EndTime = DateTime.Now;
                    session.Status = "Stopped";
                    await _eVChargingDBService.UpdateChargingSession(sessionId, session);
                    var newSession = await _eVChargingDBService.GetChargingSessionAsync(sessionId);
                    mqttMessage = $"{{ \"sessionId\": \"{newSession.SessionId}\", \"startTime\": \"{newSession.StartTime}\", \"endTime\": \"{newSession.EndTime}\", \"status\": \"Stopped\", \"energyConsumed\": {newSession.EnergyConsumed} }}";
                    return mqttMessage;
                }
                else
                {
                    mqttMessage = $"{{ \"sessionId\": \"{sessionId}\",\"status\": \"Charging session was not active\"}}";
                    return mqttMessage;
                }

               
            }
            catch (Exception e)
            {
                _logger.LogInformation( e.Message, "An error occurred");
                mqttMessage = $"{{ \"sessionId\": \"{sessionId}\",\"status\": \"Charging session was not active\"}}";
                return mqttMessage;
            }
        }

        public async Task UpdateEnergyConsumption(string sessionId,ChargingSession session)
        {
            await _eVChargingDBService.UpdateChargingSession(sessionId, session);
        }

    }
}
