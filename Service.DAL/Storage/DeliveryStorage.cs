using Microsoft.EntityFrameworkCore;
using Service.Domain.ModelsDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Service.DAL.Storage
{
    public class DeliveryStorage(ApplicationDbContext db) : IBaseStorage<DeliveryDb>
    {
        public readonly ApplicationDbContext _db = db;

        public async Task AddAsync(DeliveryDb item)
        {
            await _db.Deliveries.AddAsync(item);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(DeliveryDb item)
        {
            _db.Deliveries.Remove(item);
            await _db.SaveChangesAsync();
        }

        public async Task<DeliveryDb?> GetAsync(Guid id)
        {
            return await _db.Deliveries
                .Include(d => d.Order)
                .ThenInclude(o => o!.User)
                .Include(d => d.Order)
                .ThenInclude(o => o!.Inventory)
                .FirstOrDefaultAsync(x => x.DeliveryId == id);
        }

        public IQueryable<DeliveryDb> GetAll()
        {
            return _db.Deliveries
                .Include(d => d.Order)
                .ThenInclude(o => o!.User)
                .Include(d => d.Order)
                .ThenInclude(o => o!.Inventory);
        }

        public async Task<DeliveryDb> UpdateAsync(DeliveryDb item)
        {
            _db.Deliveries.Update(item);
            await _db.SaveChangesAsync();
            return item;
        }

        public async Task<DeliveryDb?> GetByCondition(Expression<Func<DeliveryDb, bool>> condition)
        {
            return await _db.Deliveries
                .Include(d => d.Order)
                .ThenInclude(o => o!.User)
                .Include(d => d.Order)
                .ThenInclude(o => o!.Inventory)
                .FirstOrDefaultAsync(condition);
        }

        public async Task<bool> Exists(Guid id)
        {
            return await _db.Deliveries.AnyAsync(x => x.DeliveryId == id);
        }

        public async Task<List<DeliveryDb>> GetListByCondition(Expression<Func<DeliveryDb, bool>> condition)
        {
            return await _db.Deliveries
                .Include(d => d.Order)
                .ThenInclude(o => o!.User)
                .Include(d => d.Order)
                .ThenInclude(o => o!.Inventory)
                .Where(condition)
                .ToListAsync();
        }

        // Дополнительные специфические методы для DeliveryDb

        public async Task<List<DeliveryDb>> GetByOrderIdAsync(Guid orderId)
        {
            return await GetListByCondition(d => d.OrderId == orderId);
        }

        public async Task<List<DeliveryDb>> GetByDeliveryDateAsync(DateOnly deliveryDate)
        {
            return await GetListByCondition(d => d.DeliveryDate == deliveryDate);
        }

        public async Task<List<DeliveryDb>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
        {
            return await GetListByCondition(d => d.DeliveryDate >= startDate && d.DeliveryDate <= endDate);
        }

        public async Task<List<DeliveryDb>> GetByDeliveryTypeAsync(string deliveryType)
        {
            return await GetListByCondition(d => d.DeliveryType == deliveryType);
        }

        public async Task<List<DeliveryDb>> GetByAddressAsync(string address)
        {
            return await GetListByCondition(d => d.DeliveryAddress.Contains(address));
        }

        public async Task<DeliveryDb?> GetByOrderIdSingleAsync(Guid orderId)
        {
            return await GetByCondition(d => d.OrderId == orderId);
        }

        public async Task<List<DeliveryDb>> GetUpcomingDeliveriesAsync(int days = 7)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var endDate = today.AddDays(days);
            return await GetListByCondition(d => d.DeliveryDate >= today && d.DeliveryDate <= endDate);
        }

        public async Task<List<DeliveryDb>> GetPastDeliveriesAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            return await GetListByCondition(d => d.DeliveryDate < today);
        }

        public async Task<List<DeliveryDb>> GetTodayDeliveriesAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            return await GetListByCondition(d => d.DeliveryDate == today);
        }

        public async Task<List<string>> GetAllDeliveryTypesAsync()
        {
            return await _db.Deliveries
                .Select(d => d.DeliveryType)
                .Distinct()
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetDeliveryCountByTypeAsync()
        {
            return await _db.Deliveries
                .GroupBy(d => d.DeliveryType)
                .Select(g => new { DeliveryType = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.DeliveryType, x => x.Count);
        }

        public async Task<Dictionary<DateOnly, int>> GetDeliveryCountByDateAsync(DateOnly startDate, DateOnly endDate)
        {
            return await _db.Deliveries
                .Where(d => d.DeliveryDate >= startDate && d.DeliveryDate <= endDate)
                .GroupBy(d => d.DeliveryDate)
                .Select(g => new { DeliveryDate = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.DeliveryDate, x => x.Count);
        }

        public async Task<bool> ExistsForOrderAsync(Guid orderId)
        {
            return await _db.Deliveries.AnyAsync(d => d.OrderId == orderId);
        }

        public async Task UpdateDeliveryDateAsync(Guid deliveryId, DateOnly newDeliveryDate)
        {
            var delivery = await GetAsync(deliveryId);
            if (delivery != null)
            {
                delivery.DeliveryDate = newDeliveryDate;
                await UpdateAsync(delivery);
            }
        }

        public async Task UpdateDeliveryAddressAsync(Guid deliveryId, string newAddress)
        {
            var delivery = await GetAsync(deliveryId);
            if (delivery != null)
            {
                delivery.DeliveryAddress = newAddress;
                await UpdateAsync(delivery);
            }
        }

        public async Task<List<DeliveryDb>> GetDeliveriesWithOrdersAsync()
        {
            return await _db.Deliveries
                .Include(d => d.Order)
                .ThenInclude(o => o!.User)
                .Include(d => d.Order)
                .ThenInclude(o => o!.Inventory)
                .OrderBy(d => d.DeliveryDate)
                .ThenBy(d => d.CreatedAt)
                .ToListAsync();
        }
    }
}