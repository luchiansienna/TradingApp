using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Ferovinum.Domain
{
    [Index(nameof(ClientId))]
    [Index(nameof(ProductId))]
    [Index(nameof(ClientId), nameof(ProductId))]

    public class Transaction : BaseModel
    {
        [MaxLength(10)]
        public required string ClientId { get;set; }

        [MaxLength(10)]
        public required string ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public required int Quantity { get; set; }

        public required float Price { get; set; }

        public required OrderType OrderType { get; set; }

        public required DateTime Timestamp { get; set; }

        public int? StockLeft { get; set; }

        public int? ParentBuyTransactionId { get; set; }
    }
}
