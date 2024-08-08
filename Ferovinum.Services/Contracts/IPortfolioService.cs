using Ferovinum.Domain;
using Ferovinum.Services.DTO;

namespace Ferovinum.Services.Contracts
{
    public interface IPortfolioService : IBaseRepository<Transaction>
    {
        public PortfolioDTO GetPortfolioByClientId(string clientId, DateTime? date);
    }
}