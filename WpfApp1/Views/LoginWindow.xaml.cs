using System;
using System.Windows;
using WpfApp1.Services;
using WpfApp1.Models;

namespace WpfApp1.Views
{
    public partial class LoginWindow : Window
    {
        private readonly UserService _userService;

        public LoginWindow()
        {
            InitializeComponent();
            try
            {
                _userService = new UserService();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = txtUsername.Text;
                string password = txtPassword.Password;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    txtError.Text = "Введите имя пользователя и пароль";
                    return;
                }

                var user = _userService.AuthenticateAndGetUser(username, password);
                if (user != null)
                {
                    var mainWindow = new MainWindow(user);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    txtError.Text = "Неверный логин или пароль";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка входа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = txtUsername.Text;
                string password = txtPassword.Password;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    txtError.Text = "Введите имя пользователя и пароль";
                    return;
                }

                _userService.RegisterUser(username, password);
                MessageBox.Show("Пользователь успешно зарегистрирован!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                txtError.Text = string.Empty;
            }
            catch (Exception ex)
            {
                txtError.Text = ex.Message;
            }
        }
    }
}