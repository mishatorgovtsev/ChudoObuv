using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ChudoObuv;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainFrame.Navigate(new LoginPage());
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (MainFrame.CanGoBack)
        {
            MainFrame.GoBack();
        }
    }

    private void MainFrame_Navigated(object sender, NavigationEventArgs e)
    {
        // Возврат на страницу входа означает выход из учётной записи
        if (e.Content is LoginPage)
        {
            AppSession.SignOut();
        }

        BackButton.Visibility = MainFrame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
        UserNameText.Text = AppSession.FullName;

        if (e.Content is Page page)
        {
            PageTitleText.Text = page.Title;
            Title = $"Чудо Обувь — {page.Title}";
        }
    }
}