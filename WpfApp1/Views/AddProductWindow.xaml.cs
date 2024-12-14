using System.Windows;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.Views
{
    public partial class AddProductWindow : Window
    {
        private ProductService _productService;

        public AddProductWindow()
        {
            InitializeComponent();
            _productService = new ProductService();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var product = new Product
            {
                Name = txtName.Text,
                Category = txtCategory.Text,
                Price = decimal.Parse(txtPrice.Text),
                Size = txtSize.Text,
                Color = txtColor.Text,
                StockQuantity = int.Parse(txtStockQuantity.Text)
            };

            _productService.AddProduct(product);
            this.Close();
        }
    }
}