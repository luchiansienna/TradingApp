using Ferovinum.Domain;
using Microsoft.EntityFrameworkCore;


namespace Ferovinum.Services
{
    public class TransactionsContext : DbContext
    {
        public TransactionsContext() : base() { }

        public TransactionsContext(DbContextOptions<TransactionsContext> options) : base(options) { }
        public virtual DbSet<Transaction> Transactions { get; set; } = null!;

        public virtual DbSet<Product> Products { get; set; } = null!;

        public virtual DbSet<Client> Clients { get; set; } = null!;
    }
}
