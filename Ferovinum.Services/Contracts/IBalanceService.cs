using Ferovinum.Domain;
using Ferovinum.Services.DTO;

namespace Ferovinum.Services.Contracts
{
    public interface IBalanceService : IBaseRepository<Transaction>
    {
        public IEnumerable<BalanceDTO> GetBalanceByClientId(string clientId, DateTime? date);

        public IEnumerable<BalanceDTO> GetBalanceByProductId(string productId, DateTime? date);

    }
}