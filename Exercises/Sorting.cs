public static class SortingExercises
{
    // 1. Get all products in a category, sorted by price ascending
    public static List<Product> GetProductsByCategorySortedByPrice(AppDbContext context, string categoryName)
    {
        return (from p in context.Products
                join c in context.Categories on p.CategoryId equals c.Id
                where c.Name == categoryName
                orderby p.Price
                select p).ToList();
    }

    // 2. Get all customers sorted by name descending
    public static List<Customer> GetCustomersSortedByNameDesc(AppDbContext context)
    {
        return context.Customers.OrderByDescending(c => c.Name).ToList();
    }

    // 3. Get all orders for a customer, sorted by date descending, THEN by id ascending
    public static List<Order> GetOrdersByCustomerSorted(AppDbContext context, string customerName)
    {
        return (from o in context.Orders
                join c in context.Customers on o.CustomerId equals c.Id
                where c.Name == customerName
                orderby o.OrderDate descending, o.Id
                select o).ToList();
    }

    // 4. Get all product names in reverse alphabetical order
    public static List<string> GetProductNamesReverseAlphabetical(AppDbContext context)
    {
        return context.Products
            .OrderByDescending(p => p.Name)
            .Select(p => p.Name)
            .ToList();
    }
}

// Sorting: OrderBy, ThenBy, Reverse
