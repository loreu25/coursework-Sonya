using System.Linq;
using System.Windows;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1
{
    public partial class AdminPanel : Window
    {
        private ProductService _productService;

        public AdminPanel()
        {
            InitializeComponent();
            _productService = new ProductService();
            LoadProducts();
        }

        private void LoadProducts()
        {
            ProductsDataGrid.ItemsSource = _productService.GetAllProducts().ToList();
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            // Открытие окна добавления нового товара
            var addProductWindow = new AddProductWindow();
            addProductWindow.ShowDialog();
            LoadProducts();
        }

        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            var product = (sender as FrameworkElement)?.DataContext as Product;
            if (product != null)
            {
                var editProductWindow = new EditProductWindow(product);
                editProductWindow.ShowDialog();
                LoadProducts();
            }
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            var product = (sender as FrameworkElement)?.DataContext as Product;
            if (product != null)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить товар {product.Name}?", 
                    "Подтверждение", MessageBoxButton.YesNo);
                
                if (result == MessageBoxResult.Yes)
                {
                    _productService.DeleteProduct(product.Id);
                    LoadProducts();
                }
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}