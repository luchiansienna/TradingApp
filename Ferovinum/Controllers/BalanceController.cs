using Ferovinum.Services.Contracts;
using Ferovinum.Services.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Ferovinum.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BalanceController : ControllerBase
    {
        private readonly ILogger<BalanceController> _logger;
        private readonly IBalanceService _service;
        public BalanceController(ILogger<BalanceController> logger, IBalanceService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// List all product quantities for given clientId. The optional date parameter specifies a particular date to extract a snapshot past result, otherwise the latest result will be returned.
        /// </summary>
        /// <param name="clientId">The ID of the client</param>
        /// <param name="date">The date of the report</param>
        [HttpGet("/balance/client/{clientId}")]
        [ProducesResponseType(typeof(IEnumerable<BalanceDTO>), StatusCodes.Status200OK)]
        public IActionResult GetByClientId(string clientId, [FromQuery] DateTime? date)
        {
            var transactions = _service.GetBalanceByClientId(clientId, date);
            return Ok(transactions);
        }

        /// <summary>
        /// List all product quantities for given productId. The optional date parameter specifies a particular date to extract a snapshot past result, otherwise the latest result will be returned.
        /// </summary>
        /// <param name="productId">The ID of the product</param>
        /// <param name="date">The date of the report</param>
        [HttpGet("/balance/product/{productId}")]
        [ProducesResponseType(typeof(IEnumerable<BalanceDTO>), StatusCodes.Status200OK)]
        public IActionResult GetByProductId(string productId, [FromQuery] DateTime? date)
        {
            var transactions = _service.GetBalanceByProductId(productId, date);
            return Ok(transactions);
        }
    }
}
