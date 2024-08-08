using Ferovinum.Domain;
using Ferovinum.Services.DTO;

namespace Ferovinum.Services.Contracts
{
    public interface ITransactionsService : IBaseRepository<Transaction>
    {
        public TransactionWithIdDTO Get(int id);
        public TransactionWithIdDTO Save(TransactionDTO dtoModel);
        public IEnumerable<TransactionDTO> GetByClientId(string clientId, DateTime? from, DateTime? to);

        public IEnumerable<TransactionDTO> GetByProductId(string productId, DateTime? from, DateTime? to);

    }
}