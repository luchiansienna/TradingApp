using Ferovinum.Domain;

namespace Ferovinum.Services.Contracts
{
    public interface IBaseRepository<DbModel> where DbModel : BaseModel
    {
        DbModel? Get(int id);

        DbModel Save(DbModel model);
    }
}
