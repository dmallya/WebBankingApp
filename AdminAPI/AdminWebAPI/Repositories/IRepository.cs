using AdminWebAPI.Data;

namespace AdminWebAPI.Repositories
{
    public interface IRepository<T>
    {
        Task<List<T>> Get();
        Task<T> Get(int id);
        void Post(T entity);
        void Update(T entity);
        void Put(int id, T entity);
    }
}
