using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Net;
using MarketAssetApi.Services;
using System.Net.WebSockets;
using System.Text.Json;
using MarketAssetApi.Models.DTOs;
using System.Threading;
using MarketAssetApi.Models.MarketAssetDatabase;
using Swashbuckle.AspNetCore.Annotations;

namespace MarketAssetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("OpenCORSPolicy")]
    public class MarketAssetController : ControllerBase
    {
        private readonly IMarketAssetService _marketAssetService;
        public MarketAssetController(IMarketAssetService marketAssetService)
        {
            _marketAssetService = marketAssetService;
        }

        [HttpGet("market-assets")]
        public async Task<IActionResult> MarketAssets()
        {
            var response = await _marketAssetService.GetMarketAssets();
            switch (response.Status)
            {
                case HttpStatusCode.OK:
                    return Ok(response.Data);
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
        [HttpGet("historical-prices")]
        public async Task<IActionResult> HistoricalPrices([FromQuery]List<string> symbols, [FromQuery] int interval, [FromQuery] string periodicity, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var response = await _marketAssetService.GetHistoricalPrices(symbols, interval, periodicity, startDate, endDate);
            switch (response.Status)
            {
                case HttpStatusCode.OK:
                    return Ok(response.Data);
                case HttpStatusCode.BadRequest:
                    return BadRequest(response.Error);
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
        [Route("real-time-prices")]
        //[HttpGet("real-time-prices")]
        //[ExcludeFromDescription]
        [SwaggerIgnore]
        public async Task RealTimePrices([FromQuery] List<string> symbols)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                await _marketAssetService.GetRealTimePrices(webSocket, symbols);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            return;
        }
        [HttpGet("seed")]
        public async Task<IActionResult> Seed()
        {
            var response = await _marketAssetService.Seed();
            switch (response.Status)
            {
                case HttpStatusCode.OK:
                    return Ok();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

    }
}
