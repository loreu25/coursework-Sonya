using System;
using System.ComponentModel.DataAnnotations;

namespace WpfApp1.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int StockQuantity { get; set; }
        public string ImagePath { get; set; }
    }
}