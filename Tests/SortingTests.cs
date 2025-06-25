using Microsoft.EntityFrameworkCore;
using Spectre.Console;

public static class SortingTests
{
    private static void Test_GetProductsByCategorySortedByPrice()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Sorting_ProductsByCategory")
            .Options;
        using (var context = new AppDbContext(options))
        {
            DataSeeder.SeedWebshop(context);
            var category = context.Categories.First(c => c.Name == "Electronics");
            var expected = context.Products
                .Where(p => p.CategoryId == category.Id)
                .OrderBy(p => p.Price)
                .Select(p => p.Name)
                .ToList();
            var actual = SortingExercises.GetProductsByCategorySortedByPrice(context, "Electronics")
                .Select(p => p.Name)
                .ToList();
            if (expected.SequenceEqual(actual))
                AnsiConsole.MarkupLine($"[lime]PASS[/] Products by price in 'Electronics' (count: {actual.Count})");
            else
                AnsiConsole.MarkupLine($"[red]FAIL[/] Products by price in 'Electronics'\nExpected: {string.Join(", ", expected)}\nActual:   {string.Join(", ", actual)}");
        }
    }

    private static void Test_GetCustomersSortedByNameDesc()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Sorting_CustomersByNameDesc")
            .Options;
        using (var context = new AppDbContext(options))
        {
            DataSeeder.SeedWebshop(context);
            var expected = context.Customers.OrderByDescending(c => c.Name).Select(c => c.Name).ToList();
            var actual = SortingExercises.GetCustomersSortedByNameDesc(context).Select(c => c.Name).ToList();
            if (expected.SequenceEqual(actual))
                AnsiConsole.MarkupLine($"[lime]PASS[/] Customers by name descending (count: {actual.Count})");
            else
                AnsiConsole.MarkupLine($"[red]FAIL[/] Customers by name descending\nExpected: {string.Join(", ", expected)}\nActual:   {string.Join(", ", actual)}");
        }
    }

    private static void Test_GetOrdersByCustomerSorted()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Sorting_OrdersByCustomer")
            .Options;
        using (var context = new AppDbContext(options))
        {
            DataSeeder.SeedWebshop(context);
            var customer = context.Customers.First(c => c.Name == "Alice Johnson");
            var expected = context.Orders
                .Where(o => o.CustomerId == customer.Id)
                .OrderByDescending(o => o.OrderDate)
                .ThenBy(o => o.Id)
                .Select(o => o.Id)
                .ToList();
            var actual = SortingExercises.GetOrdersByCustomerSorted(context, "Alice Johnson").Select(o => o.Id).ToList();
            if (expected.SequenceEqual(actual))
                AnsiConsole.MarkupLine($"[lime]PASS[/] Orders for 'Alice Johnson' by date desc, id asc (count: {actual.Count})");
            else
                AnsiConsole.MarkupLine($"[red]FAIL[/] Orders for 'Alice Johnson' by date desc, id asc\nExpected: {string.Join(", ", expected)}\nActual:   {string.Join(", ", actual)}");
        }
    }

    private static void Test_GetProductNamesReverseAlphabetical()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Sorting_ProductNamesReverse")
            .Options;
        using (var context = new AppDbContext(options))
        {
            DataSeeder.SeedWebshop(context);
            var expected = context.Products.OrderBy(p => p.Name).Select(p => p.Name).ToList();
            expected.Reverse();
            var actual = SortingExercises.GetProductNamesReverseAlphabetical(context);
            if (expected.SequenceEqual(actual))
                AnsiConsole.MarkupLine($"[lime]PASS[/] Product names in reverse alphabetical order (count: {actual.Count})");
            else
                AnsiConsole.MarkupLine($"[red]FAIL[/] Product names in reverse alphabetical order\nExpected: {string.Join(", ", expected)}\nActual:   {string.Join(", ", actual)}");
        }
    }

    public static void RunAll()
    {
        var tests = new List<(string Name, Action Test)>
        {
            ("GetProductsByCategorySortedByPrice", Test_GetProductsByCategorySortedByPrice),
            ("GetCustomersSortedByNameDesc", Test_GetCustomersSortedByNameDesc),
            ("GetOrdersByCustomerSorted", Test_GetOrdersByCustomerSorted),
            ("GetProductNamesReverseAlphabetical", Test_GetProductNamesReverseAlphabetical)
        };
        foreach (var (name, test) in tests)
        {
            Console.WriteLine($"\n--- {name}");
            test();
            Console.WriteLine(new string('-', 50));
        }
    }
}

