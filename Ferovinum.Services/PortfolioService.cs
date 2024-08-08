

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
        public PortfolioService(TransactionsContext context, IMapper mapper) : base(context, mapper) {
            _mapper = mapper;
        }



        public PortfolioDTO GetPortfolioByClientId(string clientId, DateTime? date)
        {

            //  You should be calculating metrics:
            //● lifeToDateFeeNotional: Notional amount of all fees ever charged for client
            //● lifeToDateProductNotional: Notional amount of all products ever transacted with client - is this including buy and sells? or just sell?
            //● outstandingFeeNotional: Notional amount of outstanding fees with client
            //● outstandingProductNotional: Notional amount of outstanding products for client
            //● weightedAverageRealisedAnnualisedYield: Annualised yield of fees weighted on notional amount of sold stock
            //● weightedAverageRealisedDuration: Average holding duration(days) weighted on notional amount of sold stock

            var productList = _context.Products.ToList();

            var currentDate = date ?? DateTime.Now;

            var client = _context.Clients.FirstOrDefault(c => c.Id == clientId);
            var sellOrders = GetAllSellOrders(null, clientId);
            var lifeToDateFeeNotional = sellOrders.ApplyDateOptionally(date).Join(_context.Products, tr => tr.ProductId, p => p.Id, (tr, p) => new { tr, p })
                .Sum(x => (x.tr.Price - x.p.Price) * x.tr.Quantity);
            float lifeToDateProductNotional = 0;

            float outstandingFeeNotional = 0, outstandingProductNotional = 0;

            GetAllBuyOrders(null, clientId).ApplyDateOptionally(date).ForEachAsync(transaction =>
            {
                var product = productList.Find(x => x.Id == transaction.ProductId);
                var startDate = transaction.Timestamp;
                var monthsPassed = startDate.DifferenceInMonths(currentDate);
                var totalSellPriceNow = (float)Math.Round(product.Price * Math.Pow(1 + client.Fee / 12, monthsPassed + 1), 2);

                lifeToDateProductNotional += (float)transaction.Quantity * product.Price;

                outstandingFeeNotional += (float)transaction.StockLeft * (totalSellPriceNow - product.Price);
                outstandingProductNotional += (float)transaction.StockLeft * product.Price;

            }).Wait();

            float weightedAverageRealisedDurationSum = 0, weightedAverageRealisedAnnualisedYieldSum = 0;
            GetAllBuyOrders(null, clientId).ApplyDateOptionally(date).ForEachAsync(transaction =>
            {
                var allSellOrders = sellOrders.Where(x => x.ProductId == transaction.ProductId && x.ParentBuyTransactionId == transaction.Id).ToList();
                weightedAverageRealisedDurationSum += allSellOrders.Sum(x => x.Quantity * x.Price * (x.Timestamp - transaction.Timestamp).Days);

                weightedAverageRealisedAnnualisedYieldSum += allSellOrders.Sum(x => x.Quantity * x.Price * (float)Math.Pow(1 + client.Fee / 12, 12));

            }).Wait();


            var weightedAverageRealisedDuration = weightedAverageRealisedDurationSum / lifeToDateProductNotional;
            var weightedAverageRealisedAnnualisedYield = weightedAverageRealisedAnnualisedYieldSum / lifeToDateProductNotional;
            return new PortfolioDTO()
            {
                LifeToDateFeeNotional = (float)Math.Round(lifeToDateFeeNotional, 2),
                LifeToDateProductNotional = (float)Math.Round(lifeToDateProductNotional, 2),
                OutstandingFeeNotional = (float)Math.Round(outstandingFeeNotional, 2),
                OutstandingProductNotional = (float)Math.Round(outstandingProductNotional, 2),
                WeightedAverageRealisedAnnualisedYield = (float)Math.Round(weightedAverageRealisedAnnualisedYield, 2),
                WeightedAverageRealisedDuration = (float)Math.Round(weightedAverageRealisedDuration, 2)
            };

        }
    }
}
