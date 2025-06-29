// Joins: Join, Multiple Joins, GroupJoin
// Add your join-related LINQ exercises and tests here.

namespace db_sandbox.Exercises;

public static class Joins
{
    // Left Inner Join: Get all products with their category name (only if category exists)
    // (Name, CategoryName)
    public static List<(string ProductName, string CategoryName)> LeftInnerJoinProductsCategories(AppDbContext context)
    {
        return (from product in context.Products
                join category in context.Categories on product.CategoryId equals category.Id
                // I made product.name and category.name clash on purpose
                // We need to set a custom property name in the anonymous object
                select new {product.Name, CategoryName = category.Name} 
                )
                
            .AsEnumerable()
            .Select(x => (x.Name, x.CategoryName))
            .ToList();
    }

    // Left Outer Join: Get all products and their category name (null default if no category)
    public static List<(string ProductName, string? CategoryName)> LeftOuterJoinProductsCategories(AppDbContext context)
    {
        return (from product in context.Products
                join category in context.Categories on product.CategoryId equals category.Id into pc
                // This category variable is not the same as line above!
                // Try renaming it to see why
                from category in pc.DefaultIfEmpty() 
                select new { product.Name, CategoryName = category != null ? category.Name : null }
                )
            .AsEnumerable()
            .Select(x => (x.Name, x.CategoryName))
            .ToList();
    }

    // Full Outer Join: Get all products and categories, even if no match
    public static List<(string? ProductName, string? CategoryName)> FullOuterJoinProductsCategories(AppDbContext context)
    {
        // Left side: Products with their categories
        var left = (from p in context.Products
                join c in context.Categories on p.CategoryId equals c.Id into pc
                from c in pc.DefaultIfEmpty()
                select new { ProductName = p.Name, CategoryName = c != null ? c.Name : null })
            .AsEnumerable()
            .Select(x => (x.ProductName, x.CategoryName));
        
        // Right side: Categories without products
        var right = (from c in context.Categories
                join p in context.Products on c.Id equals p.CategoryId into cp
                from p in cp.DefaultIfEmpty()
                where p == null
                select new { ProductName = (string?)null, CategoryName = c.Name })
            .AsEnumerable()
            .Select(x => (x.ProductName, x.CategoryName));
        return left.Union(right).ToList();
    }

    // Multiple Joins: Get all order items with product and customer name
    public static List<(string CustomerName, string ProductName, int Quantity)> MultipleJoinsOrderItems(AppDbContext context)
    {
        return (from oi in context.OrderItems
                join o in context.Orders on oi.OrderId equals o.Id
                join c in context.Customers on o.CustomerId equals c.Id
                join p in context.Products on oi.ProductId equals p.Id
                select new { CustomerName = c.Name, ProductName = p.Name, oi.Quantity })
            .AsEnumerable()
            .Select(x => (x.CustomerName, x.ProductName, x.Quantity))
            .ToList();
    }

    // GroupJoin: Get each customer and a list of their order IDs
    public static List<(string CustomerName, List<int> OrderIds)> GroupJoinCustomerOrders(AppDbContext context)
    {
        return (from c in context.Customers
                join o in context.Orders on c.Id equals o.CustomerId into orders
                select new { c.Name, OrderIds = orders.Select(x => x.Id).ToList() })
            .AsEnumerable()
            .Select(x => (x.Name, x.OrderIds))
            .ToList();
    }
}