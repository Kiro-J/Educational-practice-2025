using Microsoft.EntityFrameworkCore;
using Service.Domain.ModelsDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Service.DAL.Storage
{
    public class UserStorage(ApplicationDbContext db) : IBaseStorage<UserDb>
    {
        public readonly ApplicationDbContext _db = db;

        public async Task AddAsync(UserDb item)
        {
            await _db.Users.AddAsync(item);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(UserDb item)
        {
            _db.Users.Remove(item);
            await _db.SaveChangesAsync();
        }

        public async Task<UserDb?> GetAsync(Guid id)
        {
            return await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public IQueryable<UserDb> GetAll()
        {
            return _db.Users;
        }

        public async Task<UserDb> UpdateAsync(UserDb item)
        {
            _db.Users.Update(item);
            await _db.SaveChangesAsync();
            return item;
        }

        public async Task<UserDb?> GetByCondition(Expression<Func<UserDb, bool>> condition)
        {
            return await _db.Users.FirstOrDefaultAsync(condition);
        }

        public async Task<bool> Exists(Guid id)
        {
            return await _db.Users.AnyAsync(x => x.Id == id);
        }

        public async Task<List<UserDb>> GetListByCondition(Expression<Func<UserDb, bool>> condition)
        {
            return await _db.Users.Where(condition).ToListAsync();
        }
    }
}