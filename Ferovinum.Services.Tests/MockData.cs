
using Ferovinum.Domain;
using Ferovinum.Services.DTO;

namespace Ferovinum.Services.Tests
{
    public class MockData
    {
        public static float ClientFee { get { return 0.39F; } }
        public static float ProductPrice { get { return 10; } }

        public TransactionDTO BuyTransactionDTO = new()
        {
            ClientId = "C-1",
            ProductId = "P-1",
            OrderType = OrderType.buy.ToString(),
            Quantity = 15
        };

        public Transaction BuyTransaction = new()
        {
            ClientId = "C-1",
            ProductId = "P-1",
            OrderType = OrderType.buy,
            Quantity = 15,
            Price = ProductPrice,
            Timestamp = DateTime.Now
        };

        public TransactionDTO SellTransaction = new()
        {
            ClientId = "C-1",
            ProductId = "P-1",
            OrderType = OrderType.sell.ToString(),
            Quantity = 15
        };
    }
}
