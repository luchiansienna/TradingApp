using Ferovinum.Domain;
using Microsoft.EntityFrameworkCore;


namespace Ferovinum.Services
{
    public class TransactionsContext : DbContext
    {
        public TransactionsContext(DbContextOptions<TransactionsContext> options) : base(options) { }
        public DbSet<Transaction> Transactions { get; set; } = null!;

        public DbSet<Product> Products { get; set; } = null!;

        public DbSet<Client> Clients { get; set; } = null!;
    }
}
