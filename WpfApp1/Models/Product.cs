using System;
using System.ComponentModel.DataAnnotations;

namespace WpfApp1.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public ProductStatus Status 
        { 
            get => StockQuantity > 0 ? ProductStatus.InStock : ProductStatus.OutOfStock;
            set { } // Для EF Core
        }
        public string? ImagePath { get; set; }
    }

    public enum ProductStatus
    {
        InStock,
        OutOfStock
    }
}