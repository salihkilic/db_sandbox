namespace db_sandbox.Exercises;

public static class Grouping
{
    // 1. Group products by category
    // Return (Category.Name, Product[])
    public static List<IGrouping<string, Product>> GroupProductsByCategory(AppDbContext context)
    {
        var query =
            from p in context.Products
            join c in context.Categories on p.CategoryId equals c.Id
            select new { CategoryName = c.Name, Product = p };
        
        return query
            .AsEnumerable()
            .GroupBy(x => x.CategoryName, x => x.Product)
            .ToList();
    }

    // 2. GroupJoin: For each category, get all products
    // Return (string CategoryName, List<Product> Products)
    public static List<(string CategoryName, List<Product> Products)> GroupJoinCategoryProducts(AppDbContext context)
    {
        return context.Categories
            .Select(c => new { c.Name, Products = context.Products.Where(
                p => p.CategoryId == c.Id).ToList() })
            
            .AsEnumerable()
            .Select(x => (x.Name, x.Products))
            .ToList();
    }

    // 3. Aggregate in group: For each category, get total number of products
    public static List<(string CategoryName, int ProductCount)> ProductCountPerCategory(AppDbContext context)
    {
        return context.Products
            .Join(context.Categories, p => p.CategoryId, c => c.Id, (p, c) => new { Product = p, CategoryName = c.Name })
            .AsEnumerable()
            .GroupBy(x => x.CategoryName)
            .Select(g => (g.Key, g.Count()))
            .ToList();
    }

    // 4. Filtering in a group: For each category, get products with price > minPrice
    public static List<(string CategoryName, List<Product> ExpensiveProducts)> ExpensiveProductsPerCategory(AppDbContext context, decimal minPrice)
    {
        return context.Categories
            .Select(c => new { c.Name, ExpensiveProducts = context.Products.Where(p => p.CategoryId == c.Id && p.Price > minPrice).ToList() })
            .AsEnumerable()
            .Select(x => (x.Name, x.ExpensiveProducts))
            .ToList();
    }

    // 5. Projecting custom group results: For each category, return category name and a list of product names
    public static List<(string CategoryName, List<string> ProductNames)> ProductNamesPerCategory(AppDbContext context)
    {
        return context.Categories
            .Select(c => new { c.Name, ProductNames = context.Products.Where(p => p.CategoryId == c.Id).Select(p => p.Name).ToList() })
            .AsEnumerable()
            .Select(x => (x.Name, x.ProductNames))
            .ToList();
    }

    // 6. Top N in group: For each product, get top 5 customers by order quantity
    public static List<(string ProductName, List<(string CustomerName, int TotalQuantity)> TopCustomers)> TopCustomersPerProduct(AppDbContext context, int topN = 5)
    {
        var query = from p in context.Products
                    join oi in context.OrderItems on p.Id equals oi.ProductId
                    join o in context.Orders on oi.OrderId equals o.Id
                    join c in context.Customers on o.CustomerId equals c.Id
                    select new { ProductName = p.Name, CustomerName = c.Name, oi.Quantity };
        var grouped = query
            .AsEnumerable()
            .GroupBy(x => x.ProductName)
            .Select(g => (g.Key, g.GroupBy(x => x.CustomerName)
                                    .Select(cg => (cg.Key, cg.Sum(x => x.Quantity)))
                                    .OrderByDescending(x => x.Item2)
                                    .Take(topN)
                                    .ToList()))
            .ToList();
        return grouped;
    }
}