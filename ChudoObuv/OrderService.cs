using System;
using System.Collections.Generic;
using System.Linq;
using ChudoObuv.Models;

namespace ChudoObuv;

public class OrderListItem
{
    public int OrderId { get; init; }
    public DateOnly OrderDate { get; init; }
    public string ClientName { get; init; } = "";
    public decimal TotalAmount { get; init; }
}

public class OrderLineInfo
{
    public int OrderItemId { get; init; }
    public string ProductTitle { get; init; } = "";
    public decimal SizeValue { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Total => Quantity * UnitPrice;
}

public class ClientOption
{
    public int UserId { get; init; }
    public string FullName { get; init; } = "";
}

public static class OrderService
{
    public static List<OrderListItem> GetOrders()
    {
        using var db = new AppDbContext();
        return db.Orders
            .OrderByDescending(o => o.OrderDate)
            .ThenByDescending(o => o.OrderId)
            .Select(o => new OrderListItem
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                ClientName = o.User.Surname + " " + o.User.Name + " " + o.User.Patronymic,
                TotalAmount = o.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)
            }).ToList();
    }

    public static OrderListItem GetOrder(int orderId)
    {
        using var db = new AppDbContext();
        return db.Orders.Where(o => o.OrderId == orderId)
            .Select(o => new OrderListItem
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                ClientName = o.User.Surname + " " + o.User.Name + " " + o.User.Patronymic,
                TotalAmount = o.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)
            }).First();
    }

    public static List<OrderLineInfo> GetOrderLines(int orderId)
    {
        using var db = new AppDbContext();
        return db.OrderItems
            .Where(oi => oi.OrderId == orderId)
            .OrderBy(oi => oi.OrderItemId)
            .Select(oi => new OrderLineInfo
            {
                OrderItemId = oi.OrderItemId,
                ProductTitle = oi.StockItem.Product.Manufacturer.ManufacturerName + " | " +
                               oi.StockItem.Product.ProductName,
                SizeValue = oi.StockItem.Size.SizeValue,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList();
    }

    public static List<ClientOption> GetClients()
    {
        using var db = new AppDbContext();
        return db.Users
            .Where(u => u.Role.RoleName == RoleNames.Client)
            .OrderBy(u => u.Surname).ThenBy(u => u.Name)
            .Select(u => new ClientOption
            {
                UserId = u.UserId,
                FullName = u.Surname + " " + u.Name + " " + u.Patronymic
            }).ToList();
    }

    public static void UpdateOrderDate(int orderId, DateOnly date)
    {
        using var db = new AppDbContext();
        var order = db.Orders.First(o => o.OrderId == orderId);
        order.OrderDate = date;
        db.SaveChanges();
    }

    public static void DeleteOrderLine(int orderItemId)
    {
        using var db = new AppDbContext();
        var item = db.OrderItems.First(oi => oi.OrderItemId == orderItemId);
        ReturnToStock(db, item);
        db.OrderItems.Remove(item);
        db.SaveChanges();
    }

    public static void DeleteOrder(int orderId)
    {
        using var db = new AppDbContext();
        var items = db.OrderItems.Where(oi => oi.OrderId == orderId).ToList();
        foreach (var item in items)
        {
            ReturnToStock(db, item);
        }

        db.OrderItems.RemoveRange(items);
        db.Orders.Remove(db.Orders.First(o => o.OrderId == orderId));
        db.SaveChanges();
    }

    // Пары удалённой позиции снова доступны для заказа
    private static void ReturnToStock(AppDbContext db, OrderItem item)
    {
        var stock = db.StockItems.First(s => s.ProductId == item.ProductId && s.SizeId == item.SizeId);
        stock.Quantity += item.Quantity;
    }
}   