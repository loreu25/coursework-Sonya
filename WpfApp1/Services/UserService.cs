using System;
using System.Linq;
using WpfApp1.Data;
using WpfApp1.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService()
        {
            try
            {
                _context = new ApplicationDbContext();
                // Убедимся, что база данных создана
                _context.Database.EnsureCreated();

                // Создаем администратора, если его нет
                if (!_context.Users.Any(u => u.Username == "admin"))
                {
                    var adminUser = new User
                    {
                        Username = "admin",
                        PasswordHash = HashPassword("123"),
                        IsAdmin = true
                    };
                    _context.Users.Add(adminUser);
                    _context.SaveChanges();
                }

                // Создаем обычного пользователя, если его нет
                if (!_context.Users.Any(u => u.Username == "user"))
                {
                    var defaultUser = new User
                    {
                        Username = "user",
                        PasswordHash = HashPassword("123"),
                        IsAdmin = false
                    };
                    _context.Users.Add(defaultUser);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка инициализации базы данных: {ex.Message}");
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }

        public bool Authenticate(string username, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    return false;

                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user == null)
                    return false;

                return VerifyPassword(password, user.PasswordHash);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка аутентификации: {ex.Message}");
            }
        }

        public User? AuthenticateAndGetUser(string username, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    return null;

                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user == null)
                    return null;

                return VerifyPassword(password, user.PasswordHash) ? user : null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка аутентификации: {ex.Message}");
            }
        }

        public void RegisterUser(string username, string password, bool isAdmin = false)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    throw new Exception("Имя пользователя и пароль не могут быть пустыми");

                // Проверка, существует ли уже пользователь
                if (_context.Users.Any(u => u.Username == username))
                {
                    throw new Exception("Пользователь с таким именем уже существует");
                }

                // Создание нового пользователя
                var user = new User
                {
                    Username = username,
                    PasswordHash = HashPassword(password),
                    IsAdmin = isAdmin
                };

                // Добавление в базу данных
                _context.Users.Add(user);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка регистрации: {ex.Message}");
            }
        }
    }
}