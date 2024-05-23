using System.Linq.Expressions;
using docker_dotnet_three_js.DataAccess.Implementations.Entities;

namespace docker_dotnet_three_js.DataAccess.Contracts
{
    public interface IGenericRepository<T> where T:class
    {
        T GetById(int id);
        List<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void SaveChanges();
        void Update(T entity);

    }
}