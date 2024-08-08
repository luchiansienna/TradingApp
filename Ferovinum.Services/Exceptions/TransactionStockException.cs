namespace Ferovinum.Services.Exceptions
{
    public class TransactionStockException : Exception
    {
        public TransactionStockException()
        {
        }

        public TransactionStockException(string message) : base(message)
        {
        }
    }
}
