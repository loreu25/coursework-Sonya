using System.Linq;
using System.Windows;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.Views
{
    public partial class AdminPanelWindow : Window
    {
        private readonly ProductService _productService;

        public AdminPanelWindow()
        {
            InitializeComponent();
            _productService = new ProductService();
            LoadProducts();
        }

        private void LoadProducts()
        {
            ProductsListView.ItemsSource = _productService.GetAllProducts().ToList();
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            var productWindow = new ProductWindow();
            if (productWindow.ShowDialog() == true)
            {
                LoadProducts();
            }
        }

        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Product product)
            {
                var productWindow = new ProductWindow(product);
                if (productWindow.ShowDialog() == true)
                {
                    LoadProducts();
                }
            }
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Product product)
            {
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить товар '{product.Name}'?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _productService.DeleteProduct(product.Id);
                    LoadProducts();
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
