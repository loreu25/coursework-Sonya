using System;
using System.Windows;
using System.Windows.Input;
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
            CategoryTextBox.Text = _existingProduct.Category;
            SizeTextBox.Text = _existingProduct.Size;
            ColorTextBox.Text = _existingProduct.Color;
            PriceTextBox.Text = _existingProduct.Price.ToString();
            StockQuantityTextBox.Text = _existingProduct.StockQuantity.ToString();
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
                if (_existingProduct != null)
                {
                    // Обновление существующего товара
                    _existingProduct.Name = NameTextBox.Text;
                    _existingProduct.Category = CategoryTextBox.Text;
                    _existingProduct.Size = SizeTextBox.Text;
                    _existingProduct.Color = ColorTextBox.Text;
                    _existingProduct.Price = decimal.Parse(PriceTextBox.Text);
                    _existingProduct.StockQuantity = int.Parse(StockQuantityTextBox.Text);

                    _productService.UpdateProduct(_existingProduct);
                }
                else
                {
                    // Создание нового товара
                    var newProduct = new Product
                    {
                        Name = NameTextBox.Text,
                        Category = CategoryTextBox.Text,
                        Size = SizeTextBox.Text,
                        Color = ColorTextBox.Text,
                        Price = decimal.Parse(PriceTextBox.Text),
                        StockQuantity = int.Parse(StockQuantityTextBox.Text)
                    };

                    _productService.AddProduct(newProduct);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text)) return false;
            if (string.IsNullOrWhiteSpace(CategoryTextBox.Text)) return false;
            if (string.IsNullOrWhiteSpace(SizeTextBox.Text)) return false;
            if (string.IsNullOrWhiteSpace(ColorTextBox.Text)) return false;

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
