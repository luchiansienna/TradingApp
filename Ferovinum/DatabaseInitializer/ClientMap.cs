using CsvHelper.Configuration;
using Ferovinum.Domain;

namespace Ferovinum.DatabaseInitializer
{
    public sealed class ClientMap : ClassMap<Client>
    {
        public ClientMap()
        {
            Map(m => m.Id).Name("clientId", "ClientId");
            Map(m => m.Fee).Name("fee", "Fee").TypeConverter<FeeTypeConverter>();
        }
    }
}
