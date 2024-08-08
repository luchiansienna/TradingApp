using Ferovinum.Services.Contracts;
using Ferovinum.Services.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Ferovinum.Controllers
{
    [ApiController]
    [Route("portfolio/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly ILogger<PortfolioController> _logger;
        private readonly IPortfolioService _service;
        public PortfolioController(ILogger<PortfolioController> logger, IPortfolioService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// Calculate portfolio metrics for given clientId. The optional date parameter specifies a particular date to extract a snapshot past result, otherwise the latest result will be returned.
        /// </summary>
        /// <param name="clientId">The client id of which portfolio is requested</param>
        /// <param name="date">The date to extract tue snapshot past result.</param>
        /// <returns></returns>
        [HttpGet("/portfolio/client/{clientId}")]
        [ProducesResponseType(typeof(IEnumerable<PortfolioDTO>), StatusCodes.Status200OK)]
        public IActionResult Get(string clientId, [FromQuery] DateTime? date)
        {
            var portfolioMetrics = _service.GetPortfolioByClientId(clientId, date);
            return Ok(portfolioMetrics);
        }

    }
}
