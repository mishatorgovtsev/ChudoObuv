using System;
using System.Windows;

namespace ChudoObuv;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        LoadCatalog();
    }

    private void LoadCatalog()
    {
        try
        {
            ProductsList.ItemsSource = CatalogService.GetCatalog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось загрузить каталог: {ex.Message}",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}