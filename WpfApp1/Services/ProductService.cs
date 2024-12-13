using System.Linq;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService()
        {
            _context = new ApplicationDbContext();
            // Добавляем тестовые товары, если их нет
            if (!_context.Products.Any())
            {
                AddProduct(new Product { Name = "Футболка", Price = 999.99m, Category = "Одежда", Size = "M", Color = "Белый", StockQuantity = 10 });
                AddProduct(new Product { Name = "Джинсы", Price = 2999.99m, Category = "Одежда", Size = "L", Color = "Синий", StockQuantity = 5 });
            }
        }

        public IQueryable<Product> GetAllProducts()
        {
            _context.ChangeTracker.Clear(); // Очищаем отслеживание изменений
            return _context.Products.AsNoTracking(); // Получаем данные без отслеживания
        }

        public Product? GetProductById(int id)
        {
            return _context.Products.FirstOrDefault(p => p.Id == id);
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void UpdateProduct(Product product)
        {
            var existingProduct = _context.Products.Find(product.Id);
            if (existingProduct != null)
            {
                _context.Entry(existingProduct).CurrentValues.SetValues(product);
                _context.SaveChanges();
            }
        }

        public void DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }

        public void UpdateStock(int productId, int quantity)
        {
            var product = _context.Products.Find(productId);
            if (product != null)
            {
                if (product.StockQuantity < quantity)
                {
                    throw new InvalidOperationException($"Недостаточно товара на складе. В наличии: {product.StockQuantity}");
                }
                product.StockQuantity -= quantity;
                _context.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("Товар не найден");
            }
        }
    }
}