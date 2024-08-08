﻿namespace Ferovinum.Services.DTO
{

    public class TransactionDTO
    {
        public int? Id {  get; set; }
        public string ClientId { get;set; }

        public string ProductId { get; set; }

        public int Quantity { get; set; }

        public float? Price { get; set; }

        public string OrderType { get; set; }

        public DateTime? Timestamp { get; set; }

        //public int? StockLeft { get; set; }
        //public int? ParentBuyTransactionId { get; set; }
    }
}
