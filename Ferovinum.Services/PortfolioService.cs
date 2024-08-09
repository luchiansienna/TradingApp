

using AutoMapper;
using Ferovinum.Services.DTO;
using Ferovinum.Services.Contracts;
using Ferovinum.Services.Utils;
using Microsoft.EntityFrameworkCore;

namespace Ferovinum.Services
{
    public class PortfolioService : TransactionsService, IPortfolioService
    {
        protected readonly IMapper _mapper;
        public PortfolioService(TransactionsContext context, IMapper mapper) : base(context, mapper)
        {
            _mapper = mapper;
        }

        public PortfolioDTO GetPortfolioByClientId(string clientId, DateTime? date)
        {
            var productList = _context.Products.ToList();

            var currentDate = date ?? DateTime.Now;

            var client = _context.Clients.FirstOrDefault(c => c.Id == clientId);
            var sellOrders = GetAllSellOrders(null, clientId).ApplyDateOptionally(date);
            
            var lifeToDateFeeNotional = sellOrders.Join(_context.Products, tr => tr.ProductId, p => p.Id, (tr, p) => new { tr, p })
                .Sum(x => (x.tr.Price - x.p.Price) * x.tr.Quantity);

            float lifeToDateProductNotional = 0;
            float outstandingFeeNotional = 0, outstandingProductNotional = 0;
            float weightedAverageRealisedDurationSum = 0, weightedAverageRealisedAnnualisedYieldSum = 0;
            float soldStock = 0;

            GetAllBuyOrders(null, clientId).ApplyDateOptionally(date).ForEachAsync(transaction =>
            {
                var product = productList.Find(x => x.Id == transaction.ProductId);
                var startDate = transaction.Timestamp;
                var monthsPassed = startDate.DifferenceInMonths(currentDate);
                var totalSellPriceNow = (float)Math.Round(product.Price * Math.Pow(1 + client.Fee / 12, monthsPassed + 1), 2);

                lifeToDateProductNotional += transaction.Quantity * product.Price;

                outstandingFeeNotional += (float)transaction.StockLeft * (totalSellPriceNow - product.Price);
                outstandingProductNotional += (float)transaction.StockLeft * product.Price;

                var sellOrdersForProduct = sellOrders.Where(x => x.ProductId == transaction.ProductId && x.ParentBuyTransactionId == transaction.Id).ToList();

                weightedAverageRealisedAnnualisedYieldSum += sellOrdersForProduct.Sum(x => x.Quantity * product.Price * x.Quantity * product.Price * (float)(Math.Pow(1 + client.Fee / 12, 12) - 1));
                weightedAverageRealisedDurationSum += sellOrdersForProduct.Sum(x => x.Quantity * product.Price * ((x.Timestamp - transaction.Timestamp).Days + 1));
                soldStock += sellOrdersForProduct.Sum(x => x.Quantity * product.Price);
                
            }).Wait();

            return new PortfolioDTO()
            {
                LifeToDateFeeNotional = (float)Math.Round(lifeToDateFeeNotional, 2),
                LifeToDateProductNotional = (float)Math.Round(lifeToDateProductNotional, 2),
                OutstandingFeeNotional = (float)Math.Round(outstandingFeeNotional, 2),
                OutstandingProductNotional = (float)Math.Round(outstandingProductNotional, 2),
                WeightedAverageRealisedAnnualisedYield = (float)Math.Round(weightedAverageRealisedAnnualisedYieldSum / soldStock, 2),
                WeightedAverageRealisedDuration = (float)Math.Round(weightedAverageRealisedDurationSum / soldStock, 2)
            };

        }
    }
}
