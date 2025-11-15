using Microsoft.EntityFrameworkCore;
using Service.Domain.ModelsDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Service.DAL.Storage
{
    public class OrderStorage(ApplicationDbContext db) : IBaseStorage<OrderDb>
    {
        public readonly ApplicationDbContext _db = db;

        public async Task AddAsync(OrderDb item)
        {
            await _db.Orders.AddAsync(item);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(OrderDb item)
        {
            _db.Orders.Remove(item);
            await _db.SaveChangesAsync();
        }

        public async Task<OrderDb?> GetAsync(Guid id)
        {
            return await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Inventory)
                .Include(o => o.Delivery)
                .FirstOrDefaultAsync(x => x.OrderId == id);
        }

        public IQueryable<OrderDb> GetAll()
        {
            return _db.Orders
                .Include(o => o.User)
                .Include(o => o.Inventory)
                .Include(o => o.Delivery);
        }

        public async Task<OrderDb> UpdateAsync(OrderDb item)
        {
            _db.Orders.Update(item);
            await _db.SaveChangesAsync();
            return item;
        }

        public async Task<OrderDb?> GetByCondition(Expression<Func<OrderDb, bool>> condition)
        {
            return await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Inventory)
                .Include(o => o.Delivery)
                .FirstOrDefaultAsync(condition);
        }

        public async Task<bool> Exists(Guid id)
        {
            return await _db.Orders.AnyAsync(x => x.OrderId == id);
        }

        public async Task<List<OrderDb>> GetListByCondition(Expression<Func<OrderDb, bool>> condition)
        {
            return await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Inventory)
                .Include(o => o.Delivery)
                .Where(condition)
                .ToListAsync();
        }

        // Дополнительные специфические методы для OrderDb

        public async Task<List<OrderDb>> GetByUserIdAsync(Guid userId)
        {
            return await GetListByCondition(o => o.UserId == userId);
        }

        public async Task<List<OrderDb>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await GetListByCondition(o => o.OrderDate >= startDate && o.OrderDate <= endDate);
        }

        public async Task<List<OrderDb>> GetByPaymentStatusAsync(string paymentStatus)
        {
            return await GetListByCondition(o => o.PaymentStatus == paymentStatus);
        }

        public async Task<List<OrderDb>> GetByPaymentMethodAsync(string paymentMethod)
        {
            return await GetListByCondition(o => o.PaymentMethod == paymentMethod);
        }

        public async Task<List<OrderDb>> GetByItemIdAsync(Guid itemId)
        {
            return await GetListByCondition(o => o.ItemId == itemId);
        }

        public async Task<decimal> GetTotalAmountByUserAsync(Guid userId)
        {
            return await _db.Orders
                .Where(o => o.UserId == userId)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<decimal> GetTotalRevenueByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _db.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<int> GetCountByUserAsync(Guid userId)
        {
            return await _db.Orders
                .Where(o => o.UserId == userId)
                .CountAsync();
        }

        public async Task<List<OrderDb>> GetOrdersWithDetailsAsync()
        {
            return await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Inventory)
                .Include(o => o.Delivery)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<List<OrderDb>> GetOrdersWithDeliveryAsync()
        {
            return await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Inventory)
                .Include(o => o.Delivery)
                .Where(o => o.Delivery != null)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetOrderCountByPaymentMethodAsync()
        {
            return await _db.Orders
                .GroupBy(o => o.PaymentMethod)
                .Select(g => new { PaymentMethod = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.PaymentMethod, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetOrderCountByPaymentStatusAsync()
        {
            return await _db.Orders
                .GroupBy(o => o.PaymentStatus)
                .Select(g => new { PaymentStatus = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.PaymentStatus, x => x.Count);
        }
    }
}