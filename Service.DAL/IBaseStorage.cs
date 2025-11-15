using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Service.DAL.Storage
{
    public interface IBaseStorage<T>
    {
        Task AddAsync(T item);
        Task DeleteAsync(T item);
        Task<T?> GetAsync(Guid id);
        IQueryable<T> GetAll();
        Task<T> UpdateAsync(T item);
        Task<T?> GetByCondition(Expression<Func<T, bool>> condition);
        Task<bool> Exists(Guid id);
        Task<List<T>> GetListByCondition(Expression<Func<T, bool>> condition);
    }
}