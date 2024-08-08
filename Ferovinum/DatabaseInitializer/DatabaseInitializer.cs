using CsvHelper;
using Ferovinum.Domain;
using Ferovinum.Services;
using System.Globalization;

namespace Ferovinum.DatabaseInitializer
{
    public class DatabaseInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<TransactionsContext>();

                if (!context.Products.Any())
                {

                    using (var reader = new StreamReader("DatabaseInitializer\\Datasets\\ProductPrices.csv"))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        csv.Context.RegisterClassMap<ProductMap>();
                        var productsToAdd = csv.GetRecords<Product>();
                        context.Products.AddRange(productsToAdd);
                        context.SaveChanges();
                    }

                }

                if (!context.Clients.Any())
                {
                    using (var reader = new StreamReader("DatabaseInitializer\\Datasets\\ClientFees.csv"))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        csv.Context.RegisterClassMap<ClientMap>();
                        var clientsToAdd = csv.GetRecords<Client>();
                        context.Clients.AddRange(clientsToAdd);
                        context.SaveChanges();
                    }

                }
            }
        }
    }
}
