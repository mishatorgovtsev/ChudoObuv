using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChudoObuv.Models;
using Microsoft.EntityFrameworkCore;

namespace ChudoObuv;

public partial class LoginPage : Page
{
    public LoginPage()
    {
        InitializeComponent();
        LoginTextBox.Focus();
    }

    private void LoginTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !e.Text.All(char.IsAsciiLetterOrDigit);
    }

    private void SignInButton_Click(object sender, RoutedEventArgs e)
    {
        var login = LoginTextBox.Text.Trim();

        if (login.Length == 0)
        {
            ShowWarning("Введите логин. Пароль вводить не нужно.");
            return;
        }

        if (!login.All(char.IsAsciiLetterOrDigit))
        {
            ShowWarning("Логин может содержать только латинские буквы и цифры. Проверьте раскладку клавиатуры.");
            return;
        }

        try
        {
            using var db = new AppDbContext();
            var user = db.Users.Include(u => u.Role).FirstOrDefault(u => u.Login == login);

            if (user == null)
            {
                MessageBox.Show("Пользователь с таким логином не найден. Проверьте логин или продолжите как гость.",
                    "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            AppSession.SignIn(user);
            NavigationService.Navigate(new CatalogPage());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось подключиться к базе данных: {ex.Message}",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void GuestButton_Click(object sender, RoutedEventArgs e)
    {
        AppSession.SignOut();
        NavigationService.Navigate(new CatalogPage());
    }

    private static void ShowWarning(string message)
    {
        MessageBox.Show(message, "Вход в систему", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}