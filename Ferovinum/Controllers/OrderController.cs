using AutoMapper;
using Ferovinum.Domain;
using Ferovinum.Services.DTO;
using Ferovinum.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ferovinum.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly ITransactionsService _service;
        public OrderController(ILogger<OrderController> logger, ITransactionsService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// Get a transaction by its ID
        /// </summary>
        /// <param name="id">The ID of the transaction</param>
        [HttpGet("/{transactionId}", Name = nameof(GetTransactionById))]
        [ProducesResponseType(typeof(TransactionWithIdDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult GetTransactionById(int transactionId)
        {
            var transaction = _service.Get(transactionId);
            return Ok(transaction);
        }

        /// <summary>
        /// Create a new order - transaction
        /// </summary>
        /// <param name="transaction">The transaction to be created</param>
        [HttpPost("/order")]
        [ProducesResponseType(typeof(TransactionDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Transaction>> Post([FromBody] TransactionDTO? transaction)
        {
            if (transaction == null)
            {
                return BadRequest();
            }
            var createdTransaction = _service.Save(transaction);

            return CreatedAtAction(nameof(GetTransactionById), new { transactionId = createdTransaction.Id }, createdTransaction);
        }
    }
}
