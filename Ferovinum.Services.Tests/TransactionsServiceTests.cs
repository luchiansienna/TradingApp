using AutoMapper;
using Ferovinum.Domain;
using Ferovinum.Services.Exceptions;
using Moq;

namespace Ferovinum.Services.Tests
{
    public class TransactionsServiceTests
    {

        private AutoMapper.Mapper mapper;
        private MockData mockData;

        [SetUp]
        public void SetUp()
        {
            MapperConfiguration mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(TransactionsService).Assembly);
            });
            mapper = new AutoMapper.Mapper(mapperConfig);
            mockData = new MockData();
        }

        TransactionsService SetupService(List<Transaction> transactionsList)
        {
            var clients = new List<Client>();
            var products = new List<Product>();
            for (int i = 1; i <= 100; i++)
            {
                clients.Add(new Client() { Id = "C-" + i, Fee = MockData.ClientFee });
                products.Add(new Product() { Id = "P-" + i, Price = MockData.ProductPrice });
            }

            var dbContextMock = DbContextMock.GetMock(new Mock<TransactionsContext>(), clients, x => x.Clients);
            dbContextMock = DbContextMock.GetMock(dbContextMock, products, x => x.Products);

            dbContextMock = DbContextMock.GetMock(dbContextMock, transactionsList, x => x.Transactions);


            return new TransactionsService(dbContextMock.Object, mapper);
        }


        [Test]
        public void CreateBuyTransaction()
        {

            var transaction = mockData.BuyTransactionDTO;
            var transactionServices = SetupService(new List<Transaction>() { });
            var createdTransaction = transactionServices.Save(transaction);

            var result = transactionServices.GetAllBuyOrders(transaction.ProductId, transaction.ClientId);
            var dbCreatedTransaction = result.FirstOrDefault();
            
            Assert.Multiple(() =>
            {
                Assert.That(createdTransaction, Is.Not.Null);
                Assert.That(result, Is.Not.Null);
                Assert.That(dbCreatedTransaction, Is.Not.Null);
                Assert.That(dbCreatedTransaction.Quantity, Is.EqualTo(transaction.Quantity));
                Assert.That(dbCreatedTransaction.OrderType.ToString(), Is.EqualTo(transaction.OrderType));
                Assert.That(dbCreatedTransaction.Price, Is.EqualTo(MockData.ProductPrice));
            });
        }



        [Test]
        public void CreateSellTransaction()
        {
            var buyTransaction = mockData.BuyTransaction;
            buyTransaction.Id = 13;
            buyTransaction.Timestamp = new DateTime(2021, 01, 01);
            var transactionServices = SetupService(new List<Transaction>() { buyTransaction });

            var sellTransaction = mockData.SellTransaction;
            sellTransaction.Timestamp = new DateTime(2021, 01, 02);
            var createdTransaction = transactionServices.Save(sellTransaction);

            var result = transactionServices.GetAllSellOrders(sellTransaction.ProductId, sellTransaction.ClientId);
            var dbCreatedTransaction = result.FirstOrDefault();

            Assert.Multiple(() =>
            {
                Assert.That(createdTransaction, Is.Not.Null);
                Assert.That(result, Is.Not.Null);
                Assert.That(dbCreatedTransaction, Is.Not.Null);
                Assert.That(dbCreatedTransaction.Quantity, Is.EqualTo(sellTransaction.Quantity));
                Assert.That(dbCreatedTransaction.OrderType.ToString(), Is.EqualTo(sellTransaction.OrderType));
                Assert.That(dbCreatedTransaction.Price, Is.EqualTo((float)Math.Round(MockData.ProductPrice * Math.Pow(1 + MockData.ClientFee / 12, 1), 2)));
                Assert.That(dbCreatedTransaction.ParentBuyTransactionId, Is.EqualTo(buyTransaction.Id));
            });
        }

        [Test]
        public void CreateSellTransactionWithNoStock()
        {
            var transactionServices = SetupService(new List<Transaction>() { });

            var sellTransaction = mockData.SellTransaction;
            sellTransaction.Timestamp = new DateTime(2021, 01, 02);

            Assert.Throws<TransactionStockException>(() => transactionServices.Save(sellTransaction));
        }


        [Test]
        public void CreateSellTransactionWithNoStockLeft()
        {
            var buyTransaction = mockData.BuyTransaction;
            buyTransaction.Id = 13;
            buyTransaction.StockLeft = buyTransaction.Quantity;
            buyTransaction.Timestamp = new DateTime(2021, 01, 01);
            var transactionServices = SetupService(new List<Transaction>() { buyTransaction });

            var sellTransaction = mockData.SellTransaction;
            sellTransaction.Quantity = buyTransaction.Quantity + 1;
            sellTransaction.Timestamp = new DateTime(2021, 01, 02);
            Assert.Throws<TransactionStockException>(() => transactionServices.Save(sellTransaction));
        }
    }
}