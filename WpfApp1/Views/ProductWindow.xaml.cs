using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using WpfApp1.Models;
using WpfApp1.Services;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;

namespace WpfApp1.Views
{
    public partial class ProductWindow : Window
    {
        private readonly ProductService _productService;
        private readonly Product? _existingProduct;
        private string? _selectedImagePath;

        public ProductWindow(Product? product = null)
        {
            InitializeComponent();
            _productService = new ProductService();
            _existingProduct = product;

            if (_existingProduct != null)
            {
                Title = "Редактирование товара";
                LoadProductData();
            }
            else
            {
                Title = "Новый товар";
                AddDatePicker.SelectedDate = DateTime.Now;
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
            QuantityTextBox.Text = _existingProduct.StockQuantity.ToString();
            DescriptionTextBox.Text = _existingProduct.Description;
            AddDatePicker.SelectedDate = _existingProduct.DateAdded;

            if (!string.IsNullOrEmpty(_existingProduct.ImagePath))
            {
                _selectedImagePath = _existingProduct.ImagePath;
                LoadImagePreview(_selectedImagePath);
            }
        }

        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png",
                Title = "Выберите изображение товара"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string sourceFile = openFileDialog.FileName;
                string fileName = Path.GetFileName(sourceFile);
                string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                string targetPath = Path.Combine(targetDirectory, fileName);

                Directory.CreateDirectory(targetDirectory);
                File.Copy(sourceFile, targetPath, true);

                _selectedImagePath = targetPath;
                LoadImagePreview(targetPath);
            }
        }

        private void LoadImagePreview(string imagePath)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(imagePath);
                bitmap.EndInit();
                PreviewImage.Source = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                product.DateAdded = AddDatePicker.SelectedDate ?? DateTime.Now;
                product.Price = decimal.Parse(PriceTextBox.Text);
                product.StockQuantity = int.Parse(QuantityTextBox.Text);
                product.ImagePath = _selectedImagePath;

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
            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity < 0) return false;

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
