using System;
using System.Windows;
using System.Windows.Controls;

namespace ChudoObuv;

public partial class CatalogPage : Page
{
    public CatalogPage()
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