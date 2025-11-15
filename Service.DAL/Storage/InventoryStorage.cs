using Microsoft.EntityFrameworkCore;
using Service.Domain.ModelsDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Service.DAL.Storage
{
    public class InventoryStorage(ApplicationDbContext db) : IBaseStorage<InventoryDb>
    {
        public readonly ApplicationDbContext _db = db;

        public async Task AddAsync(InventoryDb item)
        {
            await _db.Inventory.AddAsync(item);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(InventoryDb item)
        {
            _db.Inventory.Remove(item);
            await _db.SaveChangesAsync();
        }

        public async Task<InventoryDb?> GetAsync(Guid id)
        {
            return await _db.Inventory
                .Include(i => i.Orders)
                .FirstOrDefaultAsync(x => x.ItemId == id);
        }

        public IQueryable<InventoryDb> GetAll()
        {
            return _db.Inventory.Include(i => i.Orders);
        }

        public async Task<InventoryDb> UpdateAsync(InventoryDb item)
        {
            _db.Inventory.Update(item);
            await _db.SaveChangesAsync();
            return item;
        }

        public async Task<InventoryDb?> GetByCondition(Expression<Func<InventoryDb, bool>> condition)
        {
            return await _db.Inventory
                .Include(i => i.Orders)
                .FirstOrDefaultAsync(condition);
        }

        public async Task<bool> Exists(Guid id)
        {
            return await _db.Inventory.AnyAsync(x => x.ItemId == id);
        }

        public async Task<List<InventoryDb>> GetListByCondition(Expression<Func<InventoryDb, bool>> condition)
        {
            return await _db.Inventory
                .Include(i => i.Orders)
                .Where(condition)
                .ToListAsync();
        }

        // Дополнительные специфические методы для InventoryDb

        public async Task<List<InventoryDb>> GetByCategoryAsync(string category)
        {
            return await GetListByCondition(i => i.ItemCategory == category);
        }

        public async Task<List<InventoryDb>> GetByNameAsync(string name)
        {
            return await GetListByCondition(i => i.ItemName.Contains(name));
        }

        public async Task<List<InventoryDb>> GetByLocationAsync(string location)
        {
            return await GetListByCondition(i => i.Location == location);
        }

        public async Task<List<InventoryDb>> GetLowQuantityAsync(decimal threshold = 10)
        {
            return await GetListByCondition(i => i.Quantity <= threshold);
        }

        public async Task<List<InventoryDb>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await GetListByCondition(i => i.UnitPrice >= minPrice && i.UnitPrice <= maxPrice);
        }

        public async Task<List<InventoryDb>> GetAvailableItemsAsync()
        {
            return await GetListByCondition(i => i.Quantity > 0);
        }

        public async Task<List<string>> GetAllCategoriesAsync()
        {
            return await _db.Inventory
                .Select(i => i.ItemCategory)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<string>> GetAllLocationsAsync()
        {
            return await _db.Inventory
                .Select(i => i.Location)
                .Distinct()
                .ToListAsync();
        }

        public async Task<decimal> GetTotalInventoryValueAsync()
        {
            return await _db.Inventory
                .SumAsync(i => i.Quantity * i.UnitPrice);
        }

        public async Task<Dictionary<string, decimal>> GetInventoryValueByCategoryAsync()
        {
            return await _db.Inventory
                .GroupBy(i => i.ItemCategory)
                .Select(g => new { Category = g.Key, TotalValue = g.Sum(i => i.Quantity * i.UnitPrice) })
                .ToDictionaryAsync(x => x.Category, x => x.TotalValue);
        }

        public async Task<Dictionary<string, int>> GetItemCountByCategoryAsync()
        {
            return await _db.Inventory
                .GroupBy(i => i.ItemCategory)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Category, x => x.Count);
        }

        public async Task UpdateQuantityAsync(Guid itemId, decimal newQuantity)
        {
            var item = await GetAsync(itemId);
            if (item != null)
            {
                item.Quantity = newQuantity;
                await UpdateAsync(item);
            }
        }

        public async Task IncreaseQuantityAsync(Guid itemId, decimal amount)
        {
            var item = await GetAsync(itemId);
            if (item != null)
            {
                item.Quantity += amount;
                await UpdateAsync(item);
            }
        }

        public async Task DecreaseQuantityAsync(Guid itemId, decimal amount)
        {
            var item = await GetAsync(itemId);
            if (item != null && item.Quantity >= amount)
            {
                item.Quantity -= amount;
                await UpdateAsync(item);
            }
        }

        public async Task<List<InventoryDb>> GetRecentlyAddedAsync(int count = 10)
        {
            return await _db.Inventory
                .Include(i => i.Orders)
                .OrderByDescending(i => i.CreatedDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<InventoryDb>> SearchItemsAsync(string searchTerm)
        {
            return await GetListByCondition(i =>
                i.ItemName.Contains(searchTerm) ||
                i.Description.Contains(searchTerm) ||
                i.ItemCategory.Contains(searchTerm));
        }
    }
}