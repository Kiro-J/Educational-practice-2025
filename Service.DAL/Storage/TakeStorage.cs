using Microsoft.EntityFrameworkCore;
using Service.Domain.ModelsDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Service.DAL.Storage
{
    public class TakeStorage : IBaseStorage<TakeDb>
    {
        public readonly ApplicationDbContext _db;

        public TakeStorage(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(TakeDb item)
        {
            await _db.Takes.AddAsync(item);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(TakeDb item)
        {
            _db.Takes.Remove(item);
            await _db.SaveChangesAsync();
        }

        public async Task<TakeDb?> GetAsync(Guid id)
        {
            return await _db.Takes
                .Include(t => t.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public IQueryable<TakeDb> GetAll()
        {
            return _db.Takes.Include(t => t.User);
        }

        public async Task<TakeDb> UpdateAsync(TakeDb item)
        {
            _db.Takes.Update(item);
            await _db.SaveChangesAsync();
            return item;
        }

        public async Task<TakeDb?> GetByCondition(Expression<Func<TakeDb, bool>> condition)
        {
            return await _db.Takes
                .Include(t => t.User)
                .FirstOrDefaultAsync(condition);
        }

        public async Task<bool> Exists(Guid id)
        {
            return await _db.Takes.AnyAsync(x => x.Id == id);
        }

        public async Task<List<TakeDb>> GetListByCondition(Expression<Func<TakeDb, bool>> condition)
        {
            return await _db.Takes
                .Include(t => t.User)
                .Where(condition)
                .ToListAsync();
        }

        // Дополнительные специфические методы для TakeDb

        public async Task<List<TakeDb>> GetByUserIdAsync(Guid userId)
        {
            return await GetListByCondition(t => t.UserId == userId);
        }

        public async Task<List<TakeDb>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
        {
            return await GetListByCondition(t => t.ReceptionDate >= startDate && t.ReceptionDate <= endDate);
        }

        public async Task<List<TakeDb>> GetByPaymentStatusAsync(string paymentStatus)
        {
            return await GetListByCondition(t => t.PaymentStatusText == paymentStatus);
        }

        public async Task<decimal> GetTotalAmountByUserAsync(Guid userId)
        {
            return await _db.Takes
                .Where(t => t.UserId == userId)
                .SumAsync(t => t.TotalAmount);
        }

        public async Task<int> GetCountByUserAsync(Guid userId)
        {
            return await _db.Takes
                .Where(t => t.UserId == userId)
                .CountAsync();
        }

        public async Task<List<TakeDb>> GetWithUsersAsync()
        {
            return await _db.Takes
                .Include(t => t.User)
                .OrderByDescending(t => t.ReceptionDate)
                .ToListAsync();
        }
    }
}