using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1
{
    public partial class ProductWindow : Window
    {
        private readonly ProductService _productService;
        private readonly Product? _existingProduct;

        public ProductWindow(Product? product = null)
        {
            InitializeComponent();
            _productService = new ProductService();
            _existingProduct = product;

            if (_existingProduct != null)
            {
                // Режим редактирования
                Title = "Редактирование товара";
                LoadProductData();
            }
            else
            {
                // Режим добавления
                Title = "Новый товар";
            }
        }

        private void LoadProductData()
        {
            if (_existingProduct == null) return;

            NameTextBox.Text = _existingProduct.Name;
            CategoryComboBox.SelectedItem = CategoryComboBox.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == _existingProduct.Category);
            SizeTextBox.Text = _existingProduct.Size;
            ColorTextBox.Text = _existingProduct.Color;
            PriceTextBox.Text = _existingProduct.Price.ToString();
            StockQuantityTextBox.Text = _existingProduct.StockQuantity.ToString();
            DescriptionTextBox.Text = _existingProduct.Description;
            DateAddedPicker.SelectedDate = _existingProduct.DateAdded;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var product = _existingProduct ?? new Product();
                product.Name = NameTextBox.Text;
                product.Category = (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty;
                product.Size = SizeTextBox.Text;
                product.Color = ColorTextBox.Text;
                product.Description = DescriptionTextBox.Text;
                product.DateAdded = DateAddedPicker.SelectedDate ?? DateTime.Now;
                product.Price = decimal.Parse(PriceTextBox.Text);
                product.StockQuantity = int.Parse(StockQuantityTextBox.Text);

                if (_existingProduct != null)
                {
                    _productService.UpdateProduct(product);
                }
                else
                {
                    _productService.AddProduct(product);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении товара: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text)) return false;
            if (CategoryComboBox.SelectedItem == null) return false;
            if (string.IsNullOrWhiteSpace(SizeTextBox.Text)) return false;
            if (string.IsNullOrWhiteSpace(ColorTextBox.Text)) return false;
            if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text)) return false;

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0) return false;
            if (!int.TryParse(StockQuantityTextBox.Text, out int quantity) || quantity < 0) return false;

            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !decimal.TryParse(e.Text, out _);
        }
    }
}
