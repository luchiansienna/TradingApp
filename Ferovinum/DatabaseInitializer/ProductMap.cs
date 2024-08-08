using CsvHelper.Configuration;
using Ferovinum.Domain;

namespace Ferovinum.DatabaseInitializer
{
    public sealed class ProductMap : ClassMap<Product>
    {
        public ProductMap()
        {
            Map(m => m.Id).Name("productId", "ProductId");
            Map(m => m.Price).Name("price", "Price");
        }
    }
}
