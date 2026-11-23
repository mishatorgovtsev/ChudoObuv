using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChudoObuv;

public partial class OrdersPage : Page
{
    public OrdersPage()
    {
        InitializeComponent();
        Loaded += (_, _) => RefreshOrders();
    }

    private void RefreshOrders()
    {
        try
        {
            OrdersGrid.ItemsSource = OrderService.GetOrders();
        }
        catch (Exception ex)
        {
            Show($"Не удалось загрузить список заказов: {ex.Message}", "Ошибка", MessageBoxImage.Error);
        }
    }

    private void NewOrderButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new CatalogPage());
    }

    private void OpenButton_Click(object sender, RoutedEventArgs e)
    {
        OpenSelectedOrder();
    }

    private void OrdersGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (OrdersGrid.SelectedItem != null)
        {
            OpenSelectedOrder();
        }
    }

    private void OpenSelectedOrder()
    {
        if (OrdersGrid.SelectedItem is not OrderListItem order)
        {
            Show("Выберите заказ в списке.", "Состав заказа", MessageBoxImage.Warning);
            return;
        }

        NavigationService.Navigate(new OrderDetailsPage(order.OrderId));
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (OrdersGrid.SelectedItem is not OrderListItem order)
        {
            Show("Выберите заказ, который нужно удалить.", "Удаление заказа", MessageBoxImage.Warning);
            return;
        }

        var answer = MessageBox.Show(
            $"Удалить заказ №{order.OrderId} ({order.ClientName})? Пары из заказа вернутся на склад. Действие нельзя отменить.",
            "Удаление заказа", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (answer != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            OrderService.DeleteOrder(order.OrderId);
            RefreshOrders();
        }
        catch (Exception ex)
        {
            Show($"Не удалось удалить заказ: {ex.Message}", "Ошибка", MessageBoxImage.Error);
        }
    }

    private static void Show(string message, string title, MessageBoxImage icon)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, icon);
    }
}