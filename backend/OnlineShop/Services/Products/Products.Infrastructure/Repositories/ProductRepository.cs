using Microsoft.EntityFrameworkCore;
using Products.Core.Entities;
using Products.Core.Repositories;
using Products.Infrastructure.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) => _context = context;

        public async Task<Product> GetByIdAsync(int id) =>
            await _context.Products.FindAsync(id)
            ?? throw new KeyNotFoundException($"Product {id} not found");

        public async Task<IEnumerable<Product>> GetAllAsync() =>
            await _context.Products.ToListAsync();

        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await GetByIdAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
