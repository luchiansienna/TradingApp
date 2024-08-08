using Ferovinum.Services.Contracts;
using Ferovinum.Services.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Ferovinum.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ILogger<TransactionsController> _logger;
        private readonly ITransactionsService _service;
        public TransactionsController(ILogger<TransactionsController> logger, ITransactionsService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// List all transactions for given clientId. The optional fromDate, toDate parameters specifies a particular date range to filter results, otherwise all transactions will be returned.
        /// </summary>
        /// <param name="clientId">The ID of the client</param>
        /// <param name="fromDate">The start date of the search</param>
        /// <param name="toDate">The end date of the search</param>
        [ProducesResponseType(typeof(IEnumerable<TransactionDTO>), StatusCodes.Status200OK)]
        [HttpGet("/transactions/client/{clientId}")]
        public IActionResult GetByClientId(string clientId, [FromQuery]DateTime? fromDate, [FromQuery]DateTime? toDate)
        {
            var transactions = _service.GetByClientId(clientId, fromDate, toDate);
            return Ok(transactions);
        }

        /// <summary>
        /// List all transactions for given productId. The optional fromDate, toDate parameters specifies a particular date range to filter results, otherwise all transactions will be returned.
        /// </summary>
        /// <param name="productId">The ID of the product</param>
        /// <param name="fromDate">The start date of the search</param>
        /// <param name="toDate">The end date of the search</param>
        [ProducesResponseType(typeof(IEnumerable<TransactionDTO>), StatusCodes.Status200OK)]
        [HttpGet("/transactions/product/{productId}")]
        public IActionResult GetByProductId(string productId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var transactions = _service.GetByProductId(productId, fromDate, toDate);
            return Ok(transactions);
        }
    }
}
