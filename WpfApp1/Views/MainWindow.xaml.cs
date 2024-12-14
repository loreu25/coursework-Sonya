using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private readonly ProductService _productService;
        private User? _currentUser;
        private readonly List<CartItem> _cartItems;

        public MainWindow(User? currentUser = null)
        {
            InitializeComponent();
            _productService = new ProductService();
            _currentUser = currentUser;
            _cartItems = new List<CartItem>();

            LoadProducts();
            UpdateCartItemCount();
            UpdateAdminButtonVisibility();
        }

        private void LoadProducts()
        {
            var products = _productService.GetAllProducts().ToList();
            
            // Фильтруем товары для обычных пользователей
            if (_currentUser?.IsAdmin != true)
            {
                products = products.Where(p => p.StockQuantity > 0).ToList();
            }
            
            ProductsListView.ItemsSource = null;
            ProductsListView.ItemsSource = products;
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var grid = button.Parent as StackPanel;
                if (grid == null) return;

                var quantityTextBox = grid.Children.OfType<TextBox>().FirstOrDefault();
                var product = (button.TemplatedParent as ContentPresenter)?.Content as Product;

                if (quantityTextBox != null && product != null && int.TryParse(quantityTextBox.Text, out int quantity))
                {
                    if (quantity <= 0)
                    {
                        MessageBox.Show("Пожалуйста, введите положительное количество товара.", 
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Проверяем общее количество с учетом уже добавленного в корзину
                    var existingItem = _cartItems.FirstOrDefault(i => i.Product?.Id == product.Id);
                    int totalQuantity = quantity + (existingItem?.Quantity ?? 0);

                    if (totalQuantity > product.StockQuantity)
                    {
                        int remainingQuantity = product.StockQuantity - (existingItem?.Quantity ?? 0);
                        MessageBox.Show(
                            remainingQuantity > 0 
                                ? $"Недостаточно товара на складе. Можно добавить еще: {remainingQuantity} шт." 
                                : "Весь доступный товар уже в корзине", 
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (existingItem != null)
                    {
                        existingItem.Quantity = totalQuantity;
                    }
                    else
                    {
                        _cartItems.Add(new CartItem { Product = product, Quantity = quantity });
                    }

                    UpdateCartItemCount();
                    quantityTextBox.Text = "1";
                    MessageBox.Show($"Товар {product.Name} добавлен в корзину", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void UpdateCartItemCount()
        {
            CartItemCountTextBlock.Text = _cartItems.Sum(item => item.Quantity).ToString();
        }

        private void UpdateAdminButtonVisibility()
        {
            AdminPanelButton.Visibility = _currentUser?.IsAdmin == true ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ViewCartButton_Click(object sender, RoutedEventArgs e)
        {
            var cartWindow = new CartWindow();
            cartWindow.SetCartItems(_cartItems);
            if (cartWindow.ShowDialog() == true)
            {
                _cartItems.Clear();
                UpdateCartItemCount();
                LoadProducts();
            }
        }

        private void AdminPanelButton_Click(object sender, RoutedEventArgs e)
        {
            var adminPanel = new AdminPanelWindow();
            adminPanel.ShowDialog();
            LoadProducts();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }
    }
}