using AutoMapper;
using Ferovinum.Domain;
using Ferovinum.Services.DTO;
using Microsoft.EntityFrameworkCore;

namespace Ferovinum.Services.Contracts
{
    public abstract class BaseRepository<DbModel> : IBaseRepository<DbModel> where DbModel : BaseModel
    {
        protected TransactionsContext _context;
        protected DbSet<DbModel> _dbSet;
        protected BaseRepository(TransactionsContext context)
        {
            _context = context;
            _dbSet = context.Set<DbModel>();
        }

        public DbModel? Get(int id) => _dbSet.FirstOrDefault(x => x.Id == id);

        public virtual DbModel Save(DbModel model)
        {
            _context.Attach(model);
            _context.SaveChanges();
            return model;
        }
    }
}
