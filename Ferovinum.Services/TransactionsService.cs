

using AutoMapper;
using Ferovinum.Domain;
using Ferovinum.Services.DTO;
using Ferovinum.Services.Contracts;
using Ferovinum.Services.Utils;
using Ferovinum.Services.Exceptions;

namespace Ferovinum.Services
{
    public class TransactionsService : BaseRepository<Transaction>, ITransactionsService
    {

        protected readonly IMapper _mapper;
        public TransactionsService(TransactionsContext context, IMapper mapper) : base(context) {
            _mapper = mapper;
        }

        public IQueryable<Transaction> GetAllSellOrders(string? productId, string? clientId)
        {
            var transactions = _context.Transactions.Where(tr => tr.OrderType == OrderType.sell);
            if (productId != null)
            {
                transactions = transactions.Where(tr => tr.ProductId == productId);
            }
            if (clientId != null)
            {
                transactions = transactions.Where(tr => tr.ClientId == clientId);
            }
            return transactions;
        }

        public IQueryable<Transaction> GetAllBuyOrders(string? productId, string? clientId)
        {
            var transactions = _context.Transactions.Where(tr => tr.OrderType == OrderType.buy);
            if (productId != null)
            {
                transactions = transactions.Where(tr => tr.ProductId == productId);
            }
            if (clientId != null)
            {
                transactions = transactions.Where(tr => tr.ClientId == clientId);
            }
            return transactions;
        }

        public int GetSumSoldQuantity(string? productId, string? clientId) =>
           GetAllSellOrders(productId, clientId).Sum(x => x.Quantity);

        public TransactionWithIdDTO Get(int id)
        {
            return _mapper.Map<TransactionWithIdDTO>(base.Get(id));
        }
        public TransactionWithIdDTO Save(TransactionDTO dtoModel)
        {
            var model = _mapper.Map<Transaction>(dtoModel);
            var product = _context.Products.FirstOrDefault(p => p.Id == model.ProductId);

            if (product == null)
            {
                throw new DbEntityNotFoundException($"Product with id {model.ProductId} not found in the database.");
            }

            var client = _context.Clients.FirstOrDefault(c => c.Id == model.ClientId);

            if (client == null)
            {
                throw new DbEntityNotFoundException($"Client with id {model.ClientId} not found in the database.");
            }
            // don't forget to uncomment this
            //model.Timestamp = DateTime.Now;
            if (model.OrderType == OrderType.buy)
            {
                model.Price = product.Price;
                model.StockLeft = model.Quantity;
            }
            else
            {
                var sumQuantitySold = GetSumSoldQuantity(model.ProductId, model.ClientId);
                var transactions = GetAllBuyOrders(model.ProductId, model.ClientId).OrderBy(tr => tr.Timestamp);
                Transaction theBuyTransactionWhereWeExtractFrom = transactions.FirstOrDefault(x => x.StockLeft != 0);
                // don t forget to uncomment this
                //model.Timestamp = Datetime.Now

                if (theBuyTransactionWhereWeExtractFrom == null)
                {
                    throw new TransactionStockException($"Product with id {model.ProductId} has its stock depleted or never ordered. A buy transaction has to be made in order to sell this product.");
                }
                else if (model.Quantity > theBuyTransactionWhereWeExtractFrom.StockLeft)
                {
                    throw new TransactionStockException($"Quantity of '{model.Quantity}' cannot be ordered. Product with id {model.ProductId} has {theBuyTransactionWhereWeExtractFrom.StockLeft} units left from the transaction from" +
                        $"'{theBuyTransactionWhereWeExtractFrom?.Timestamp}'. Make an sell order of {theBuyTransactionWhereWeExtractFrom.StockLeft} to finish the current batch or order less to consume from the batch.");
                }

                var startDate = theBuyTransactionWhereWeExtractFrom.Timestamp;
                // There is a limitation here, the stock that is taken from is the first buy,
                // but if the Ferovinum buys multiple times the same product from the same client,
                // the sell orders should be separated, cause the price will be different as the products will be sold from 2 or more different buys
                // ( with different time purchase ) so the sell price will be different 
                var monthsPassed = startDate.DifferenceInMonths(model.Timestamp);


                model.Price = (float)Math.Round(product.Price * Math.Pow(1 + client.Fee / 12, monthsPassed + 1), 2);
                model.ParentBuyTransactionId = theBuyTransactionWhereWeExtractFrom.Id;
                theBuyTransactionWhereWeExtractFrom.StockLeft -= model.Quantity;
                base.Save(theBuyTransactionWhereWeExtractFrom);
            }
            return _mapper.Map<TransactionWithIdDTO>(base.Save(model));
        }

        public IEnumerable<TransactionDTO> GetByClientId(string clientId, DateTime? from, DateTime? to)
            => _mapper.Map<IEnumerable<Transaction>, IEnumerable<TransactionDTO>>(_dbSet
                .Where(tr => tr.ClientId == clientId).ApplyDatesOptionally(from, to));


        public IEnumerable<TransactionDTO> GetByProductId(string productId, DateTime? from, DateTime? to)
            => _mapper.Map<IEnumerable<Transaction>, IEnumerable<TransactionDTO>>(_dbSet
                .Where(tr => tr.ProductId == productId).ApplyDatesOptionally(from, to));




    }
}
