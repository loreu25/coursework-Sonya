using System.Linq;
using System.Windows;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1
{
    public partial class CartWindow : Window
    {
        private ProductService _productService;
        private UserService _userService;

        public CartWindow()
        {
            InitializeComponent();
            _productService = new ProductService();
            _userService = new UserService();
            LoadCartItems();
        }

        private void LoadCartItems()
        {
            // Здесь будет логика загрузки товаров в корзину
            // Временно используем пустой список
            CartItemsListView.ItemsSource = Enumerable.Empty<CartItem>();
            UpdateTotalPrice();
        }

        private void UpdateTotalPrice()
        {
            // Здесь будет логика подсчета общей стоимости
            TotalPriceTextBlock.Text = "Итого: 0 руб.";
        }

        private void RemoveFromCartButton_Click(object sender, RoutedEventArgs e)
        {
            var cartItem = (sender as FrameworkElement)?.DataContext as CartItem;
            if (cartItem != null)
            {
                // Логика удаления товара из корзины
                MessageBox.Show($"Товар {cartItem.Product.Name} удален из корзины");
            }
        }

        private void PurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика оформления заказа
            MessageBox.Show("Заказ оформлен!");
            this.Close();
        }
    }
}