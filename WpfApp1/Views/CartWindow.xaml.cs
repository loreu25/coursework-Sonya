using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.Views
{
    public partial class CartWindow : Window
    {
        private List<CartItem> _cartItems = new();
        private readonly ProductService _productService;
        private decimal _totalAmount;

        public CartWindow()
        {
            InitializeComponent();
            _productService = new ProductService();
        }

        public void SetCartItems(List<CartItem> cartItems)
        {
            if (cartItems == null)
                return;

            _cartItems = cartItems;
            CartItemsListView.ItemsSource = _cartItems;
            UpdateTotalAmount();
        }

        private void UpdateTotalAmount()
        {
            _totalAmount = _cartItems.Sum(item => item.Product?.Price * item.Quantity ?? 0);
            TotalAmountTextBlock.Text = $"{_totalAmount:N0} ₽";
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = CartItemsListView.SelectedItem as CartItem;
            if (selectedItem != null)
            {
                _cartItems.Remove(selectedItem);
                CartItemsListView.ItemsSource = null;
                CartItemsListView.ItemsSource = _cartItems;
                UpdateTotalAmount();
            }
        }

        private void RemoveFromCartButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            var cartItem = button?.DataContext as CartItem;
            if (cartItem != null)
            {
                _cartItems.Remove(cartItem);
                CartItemsListView.ItemsSource = null;
                CartItemsListView.ItemsSource = _cartItems;
                UpdateTotalAmount();
            }
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем наличие товаров
                foreach (var item in _cartItems)
                {
                    if (item.Product == null) continue;
                    
                    var currentProduct = _productService.GetProductById(item.Product.Id);
                    if (currentProduct == null || currentProduct.StockQuantity < item.Quantity)
                    {
                        MessageBox.Show($"Товар '{item.Product.Name}' недоступен в запрошенном количестве.", 
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                // Обновляем количество товаров на складе
                foreach (var item in _cartItems)
                {
                    if (item.Product == null) continue;
                    _productService.UpdateStock(item.Product.Id, item.Quantity);
                }

                MessageBox.Show($"Заказ оформлен на сумму {_totalAmount:N0} ₽", 
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оформлении заказа: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class CartItem
    {
        public int Id { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
    }
}