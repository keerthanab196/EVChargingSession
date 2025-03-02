using EVChargingSessionWebAPI.Models;
using EVChargingSessionWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EVChargingSessionWebAPI.Controllers
{
    [Route("charging/")]
    [ApiController]
    public class ChargingSessionController : ControllerBase
    {

        public readonly EVChargingControlService _eVChargingControlService;

        public ChargingSessionController(EVChargingControlService eVChargingControlService)
        {
            _eVChargingControlService= eVChargingControlService;
        }



        // POST charging/status
        [HttpGet]
        [Route("status")]
        public async Task<IActionResult> GetLatestSession()
        {
            try
            {
                ChargingSession session=await _eVChargingControlService.GetLatestSession();
                if (session == null)
                {
                    return NotFound();
                }
                
                return Ok(new {Message= "Session details", session });
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred");
            }
        }

        // POST charging/start
        [HttpPost]
        [Route("start")]
        public async Task<IActionResult> StartCharging()
        {
            try
            {
                
                var res=await _eVChargingControlService.StartCharging();
                ChargingSession session = await _eVChargingControlService.GetLatestSession();
                return Ok(new { message="Charging started",session});
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred");
            }
           
            
        }

        // POST charging/stop
        [HttpPost]
        [Route("Stop/{sessionId}")]
        public async Task<IActionResult> StopCharging(string sessionId)
        {
            try
            {
                var res=await _eVChargingControlService.StopCharging(sessionId);

                ChargingSession session = await _eVChargingControlService.GetLatestSession();

                if(session==null)
                {
                    return NotFound();
                }


                return Ok(new { message = "Charging stopped",session});
            }
            catch(Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to stop charging");
            }
        }

       
    }
}
