using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ChudoObuv;

public partial class OrderDetailsPage : Page
{
    private readonly int orderId;
    private readonly bool isLoaded;

    public OrderDetailsPage(int orderId)
    {
        InitializeComponent();
        this.orderId = orderId;

        try
        {
            var order = OrderService.GetOrder(orderId);
            OrderTitleText.Text = $"Заказ №{order.OrderId}";
            ClientText.Text = $"Клиент: {order.ClientName}";
            DateText.Text = order.OrderDate.ToString("dd.MM.yyyy");
            OrderDatePicker.DisplayDateEnd = DateTime.Today;
            OrderDatePicker.SelectedDate = order.OrderDate.ToDateTime(TimeOnly.MinValue);
            ReloadLines();
            isLoaded = true;
        }
        catch (Exception ex)
        {
            Show($"Не удалось загрузить заказ: {ex.Message}", "Ошибка", MessageBoxImage.Error);
            Loaded += (_, _) => NavigationService?.GoBack();
            return;
        }

        if (AppSession.IsAdministrator)
        {
            DateText.Visibility = Visibility.Collapsed;
            OrderDatePicker.Visibility = Visibility.Visible;
            SaveDateButton.Visibility = Visibility.Visible;
            DeleteLineButton.Visibility = Visibility.Visible;
        }
    }

    private void ReloadLines()
    {
        List<OrderLineInfo> lines = OrderService.GetOrderLines(orderId);
        LinesGrid.ItemsSource = lines;
        TotalText.Text = $"Итого по заказу: {lines.Sum(l => l.Total):N0} ₽";
    }

    private void SaveDateButton_Click(object sender, RoutedEventArgs e)
    {
        if (!isLoaded)
        {
            return;
        }

        if (OrderDatePicker.SelectedDate is not DateTime date)
        {
            Show("Выберите корректную дату заказа в формате дд.мм.гггг.", "Изменение даты", MessageBoxImage.Warning);
            return;
        }

        if (date.Date > DateTime.Today)
        {
            Show("Дата заказа не может быть в будущем.", "Изменение даты", MessageBoxImage.Warning);
            return;
        }

        try
        {
            OrderService.UpdateOrderDate(orderId, DateOnly.FromDateTime(date));
            Show("Дата заказа изменена.", "Изменение даты", MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            Show($"Не удалось сохранить дату: {ex.Message}", "Ошибка", MessageBoxImage.Error);
        }
    }

    private void DeleteLineButton_Click(object sender, RoutedEventArgs e)
    {
        if (LinesGrid.SelectedItem is not OrderLineInfo line)
        {
            Show("Выберите позицию, которую нужно удалить.", "Удаление позиции", MessageBoxImage.Warning);
            return;
        }

        if (LinesGrid.Items.Count == 1)
        {
            Show("Это единственная позиция заказа. Чтобы убрать её, удалите заказ целиком на странице списка заказов.",
                "Удаление позиции", MessageBoxImage.Warning);
            return;
        }

        var answer = MessageBox.Show(
            $"Удалить позицию «{line.ProductTitle}», размер {line.SizeValue:0.#}? Пары вернутся на склад.",
            "Удаление позиции", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (answer != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            OrderService.DeleteOrderLine(line.OrderItemId);
            ReloadLines();
        }
        catch (Exception ex)
        {
            Show($"Не удалось удалить позицию: {ex.Message}", "Ошибка", MessageBoxImage.Error);
        }
    }

    private static void Show(string message, string title, MessageBoxImage icon)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, icon);
    }
}