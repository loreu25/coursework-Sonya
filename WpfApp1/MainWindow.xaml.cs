using System.Windows;
using WpfApp1.Services;
using WpfApp1.Models;
using System.Linq;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private ProductService _productService;

        public MainWindow()
        {
            InitializeComponent();
            _productService = new ProductService();
            LoadProducts();
        }

        private void LoadProducts()
        {
            ProductsListView.ItemsSource = _productService.GetAllProducts().ToList();
        }

        private void OpenCartButton_Click(object sender, RoutedEventArgs e)
        {
            CartWindow cartWindow = new CartWindow();
            cartWindow.Show();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            var product = (sender as FrameworkElement)?.DataContext as Product;
            if (product != null)
            {
                // Логика добавления в корзину
                MessageBox.Show($"Товар {product.Name} добавлен в корзину");
            }
        }
    }
}