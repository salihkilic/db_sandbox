using System.Collections.Generic;
using System.Linq;

public static class Exercises
{

    // 1. Get all products in a given category name (using join)
    public static List<Product> GetProductsByCategory(AppDbContext context, string categoryName)
    {
        return (from p in context.Products
                join c in context.Categories on p.CategoryId equals c.Id
                where c.Name == categoryName
                select p).ToList();
    }

    // 2. Get all orders for a given customer name (using join)
    public static List<Order> GetOrdersByCustomer(AppDbContext context, string customerName)
    {
        return (from o in context.Orders
                join c in context.Customers on o.CustomerId equals c.Id
                where c.Name == customerName
                select o).ToList();
    }

    // 3. Get the total amount spent by a customer (sum of their payments, using joins)
    public static decimal GetTotalAmountSpentByCustomer(AppDbContext context, string customerName)
    {
        return (from o in context.Orders
                join c in context.Customers on o.CustomerId equals c.Id
                join p in context.Payments on o.Id equals p.OrderId
                where c.Name == customerName
                select p.Amount).Sum();
    }

    // 4. Get names of all customers who have placed at least one order over $1000
    public static List<string> GetCustomersWithLargeOrder(AppDbContext context, decimal minOrderAmount)
    {
        var result = context.Orders
            .Join(context.Payments,
                  o => o.Id,
                  p => p.OrderId,
                  (o, p) => new { o.CustomerId, p.Amount })
            .Where(x => x.Amount > minOrderAmount)
            .Join(context.Customers,
                  x => x.CustomerId,
                  c => c.Id,
                  (x, c) => c.Name)
            .Distinct()
            .ToList();
        return result;
    }

    // 5. Get the top 3 most frequently ordered products (by total quantity)
    public static List<(string ProductName, int TotalQuantity)> GetTop3ProductsByQuantity(AppDbContext context)
    {
        var result = context.OrderItems
            .GroupBy(oi => oi.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalQuantity = g.Sum(oi => oi.Quantity)
            })
            .OrderByDescending(x => x.TotalQuantity)
            .Take(3)
            .Join(context.Products,
                  x => x.ProductId,
                  p => p.Id,
                  (x, p) => new { p.Name, x.TotalQuantity })
            .ToList()
            .Select(x => (x.Name, x.TotalQuantity))
            .ToList();
        return result;
    }
}
