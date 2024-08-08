

using AutoMapper;
using Ferovinum.Domain;
using Ferovinum.Services.DTO;
using Ferovinum.Services.Contracts;
using Ferovinum.Services.Utils;

namespace Ferovinum.Services
{
    public class BalanceService : BaseRepository<Transaction>, IBalanceService
    {

        protected readonly IMapper _mapper;
        public BalanceService(TransactionsContext context, IMapper mapper) : base(context) {
            _mapper = mapper;
        }
        public IEnumerable<BalanceDTO> GetBalanceByClientId(string clientId, DateTime? date)
             => _dbSet
                .Where(tr => tr.ClientId == clientId).ApplyDateOptionally(date).GroupBy(tr => tr.ProductId)
                  .Select(g => new BalanceDTO
                  {
                      ClientId = clientId,
                      ProductId = g.Key,
                      Quantity = g.Sum(s => s.OrderType == OrderType.buy ? s.Quantity : -s.Quantity)
                  });

        public IEnumerable<BalanceDTO> GetBalanceByProductId(string productId, DateTime? date)
            => _dbSet
               .Where(tr => tr.ProductId == productId).ApplyDateOptionally(date).GroupBy(tr => tr.ClientId)
                 .Select(g => new BalanceDTO
                 {
                     ClientId = g.Key,
                     ProductId = productId,
                     Quantity = g.Sum(s => s.OrderType == OrderType.buy ? s.Quantity : -s.Quantity)
                 });
    }
}
