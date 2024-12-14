using System.Windows;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.Views
{
    public partial class EditProductWindow : Window
    {
        private ProductService _productService;
        private Product _product;

        public EditProductWindow(Product product)
        {
            InitializeComponent();
            _productService = new ProductService();
            _product = product;
            LoadProductData();
        }

        private void LoadProductData()
        {
            txtName.Text = _product.Name;
            txtCategory.Text = _product.Category;
            txtPrice.Text = _product.Price.ToString();
            txtSize.Text = _product.Size;
            txtColor.Text = _product.Color;
            txtStockQuantity.Text = _product.StockQuantity.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _product.Name = txtName.Text;
            _product.Category = txtCategory.Text;
            _product.Price = decimal.Parse(txtPrice.Text);
            _product.Size = txtSize.Text;
            _product.Color = txtColor.Text;
            _product.StockQuantity = int.Parse(txtStockQuantity.Text);

            _productService.UpdateProduct(_product);
            this.Close();
        }
    }
}