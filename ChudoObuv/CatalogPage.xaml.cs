using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChudoObuv;

public partial class CatalogPage : Page
{
    private const string AllCategories = "Все категории";
    private const int SortAscendingIndex = 1;
    private const int SortDescendingIndex = 2;

    private List<CatalogItem> allItems = new();
    private bool isReady;

    public CatalogPage()
    {
        InitializeComponent();
        FilterPanel.Visibility = AppSession.IsAuthorized ? Visibility.Visible : Visibility.Collapsed;
        OrdersButton.Visibility = AppSession.CanManageOrders ? Visibility.Visible : Visibility.Collapsed;
        LoadCategories();
        isReady = true;
        Loaded += (_, _) => RefreshCatalog();
    }

    private void LoadCategories()
    {
        try
        {
            var categories = new List<string> { AllCategories };
            categories.AddRange(CatalogService.GetCategoryNames());
            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            ShowLoadError(ex);
        }
    }

    private void RefreshCatalog()
    {
        try
        {
            allItems = CatalogService.GetCatalog();
        }
        catch (Exception ex)
        {
            ShowLoadError(ex);
        }

        ApplyFilters();
        OrderButton.Content = $"Заказ ({OrderCart.Lines.Sum(l => l.Quantity)})";
    }

    private void ApplyFilters()
    {
        IEnumerable<CatalogItem> items = allItems;

        var search = SearchTextBox.Text.Trim();
        if (search.Length > 0)
        {
            items = items.Where(i =>
                i.ProductName.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                i.Description.Contains(search, StringComparison.CurrentCultureIgnoreCase));
        }

        if (CategoryComboBox.SelectedIndex > 0)
        {
            var category = (string)CategoryComboBox.SelectedItem;
            items = items.Where(i => i.Category == category);
        }

        items = SortComboBox.SelectedIndex switch
        {
            SortAscendingIndex => items.OrderBy(i => i.FinalPrice),
            SortDescendingIndex => items.OrderByDescending(i => i.FinalPrice),
            _ => items
        };

        var result = items.ToList();
        ProductsList.ItemsSource = result;
        EmptyText.Visibility = result.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private static void ShowLoadError(Exception ex)
    {
        MessageBox.Show($"Не удалось загрузить каталог: {ex.Message}",
            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (isReady) ApplyFilters();
    }

    private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (isReady) ApplyFilters();
    }

    private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (isReady) ApplyFilters();
    }

    private void ProductItem_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is ListBoxItem { DataContext: CatalogItem item })
        {
            NavigationService.Navigate(new ProductPage(item.ProductId));
        }
    }

    private void OrderButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new OrderPage());
    }
    private void OrdersButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new OrdersPage());
    }
}