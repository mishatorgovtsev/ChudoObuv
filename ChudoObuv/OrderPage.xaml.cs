using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ChudoObuv.Models;

namespace ChudoObuv;

public partial class OrderPage : Page
{
    public OrderPage()
    {
        InitializeComponent();
        OrderDateText.Text = $"Дата заказа: {DateTime.Today:dd.MM.yyyy}";
        RefreshLines();
    }

    private void RefreshLines()
    {
        LinesGrid.ItemsSource = OrderCart.Lines.ToList();
        TotalText.Text = $"Итого: {OrderCart.Lines.Sum(l => l.Total):N0} ₽";
        EmptyText.Visibility = OrderCart.Lines.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void RemoveButton_Click(object sender, RoutedEventArgs e)
    {
        if (LinesGrid.SelectedItem is not OrderLine line)
        {
            Show("Выберите позицию в таблице, которую нужно убрать.", "Удаление позиции", MessageBoxImage.Warning);
            return;
        }

        var answer = MessageBox.Show("Убрать выбранную позицию из заказа?", "Удаление позиции",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (answer == MessageBoxResult.Yes)
        {
            OrderCart.Remove(line);
            RefreshLines();
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        var answer = MessageBox.Show("Отказаться от заказа? Все добавленные позиции будут удалены.",
            "Отказ от заказа", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (answer == MessageBoxResult.Yes)
        {
            OrderCart.Clear();
            NavigationService.GoBack();
        }
    }

    private void ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        if (!AppSession.IsAuthorized)
        {
            Show("Заказ может оформить только авторизованный пользователь. Войдите в систему.",
                "Оформление заказа", MessageBoxImage.Warning);
            return;
        }

        if (OrderCart.Lines.Count == 0)
        {
            Show("В заказе нет ни одной позиции. Добавьте товары из каталога.",
                "Оформление заказа", MessageBoxImage.Warning);
            return;
        }

        var answer = MessageBox.Show($"Подтвердить заказ на сумму {OrderCart.Lines.Sum(l => l.Total):N0} ₽?",
            "Подтверждение заказа", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (answer != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            if (!TrySaveOrder(out var orderId, out var error))
            {
                Show(error, "Заказ не оформлен", MessageBoxImage.Warning);
                return;
            }

            OrderCart.Clear();
            Show($"Заказ №{orderId} оформлен.", "Заказ оформлен", MessageBoxImage.Information);
            NavigationService.GoBack();
        }
        catch (Exception ex)
        {
            Show($"Не удалось сохранить заказ: {ex.Message}", "Ошибка", MessageBoxImage.Error);
        }
    }

    private static bool TrySaveOrder(out int orderId, out string error)
    {
        orderId = 0;
        error = "";

        using var db = new AppDbContext();
        var pairs = new List<(OrderLine Line, StockItem Stock)>();

        // Остатки перечитываются из базы: за время оформления они могли измениться
        foreach (var line in OrderCart.Lines)
        {
            var stock = db.StockItems.FirstOrDefault(s => s.ProductId == line.ProductId && s.SizeId == line.SizeId);
            if (stock == null || stock.Quantity < line.Quantity)
            {
                error = $"«{line.ProductTitle}», размер {line.SizeValue:0.#}: на складе осталось " +
                        $"{stock?.Quantity ?? 0} пар, в заказе {line.Quantity}. " +
                        "Измените количество или уберите позицию.";
                return false;
            }

            pairs.Add((line, stock));
        }

        // В таблицах нет автонумерации, поэтому новый номер считается как максимальный плюс один
        orderId = (db.Orders.Max(o => (int?)o.OrderId) ?? 0) + 1;
        var nextItemId = (db.OrderItems.Max(oi => (int?)oi.OrderItemId) ?? 0) + 1;

        db.Orders.Add(new Order
        {
            OrderId = orderId,
            OrderDate = DateOnly.FromDateTime(DateTime.Today),
            UserId = AppSession.CurrentUser!.UserId
        });

        foreach (var (line, stock) in pairs)
        {
            db.OrderItems.Add(new OrderItem
            {
                OrderItemId = nextItemId++,
                OrderId = orderId,
                ProductId = line.ProductId,
                SizeId = line.SizeId,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice
            });
            stock.Quantity -= line.Quantity;
        }

        db.SaveChanges();
        return true;
    }

    private static void Show(string message, string title, MessageBoxImage icon)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, icon);
    }
}