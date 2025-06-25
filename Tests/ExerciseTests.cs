using db_sandbox.Exercises;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

public static class ExerciseTests
{
    private static void Test_GetProductsByCategory()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_ProductsByCategory")
            .Options;
        using (var context = new AppDbContext(options))
        {
            DataSeeder.SeedWebshop(context);
            int maxNameLength = context.Categories.Any() ? context.Categories.Max(c => c.Name.Length) : 10;
            string nameHeader = "Category".PadRight(maxNameLength);
            string countHeader = "Product Count".PadRight(13);
            Console.WriteLine($"{nameHeader} | {countHeader}");
            Console.WriteLine(new string('-', maxNameLength + 3 + 13));
            foreach (var category in context.Categories)
            {
                var products = Exercises.GetProductsByCategory(context, category.Name);
                var expectedProducts = context.Products
                    .Where(p => p.CategoryId == category.Id)
                    .Select(p => p.Name)
                    .OrderBy(x => x)
                    .ToList();
                var actualProducts = products.Select(p => p.Name).OrderBy(x => x).ToList();
                if (expectedProducts.SequenceEqual(actualProducts))
                    AnsiConsole.MarkupLine($"[lime]PASS[/] {category.Name.PadRight(maxNameLength)} (products: {actualProducts.Count})");
                else
                    AnsiConsole.MarkupLine($"[red]FAIL[/] {category.Name.PadRight(maxNameLength)}\nExpected ({expectedProducts.Count}): {string.Join(", ", expectedProducts)}\nActual ({actualProducts.Count}): {string.Join(", ", actualProducts)}");
            }

        }
    }

    private static void Test_GetOrdersByCustomer()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_OrdersByCustomer")
            .Options;
        using (var context = new AppDbContext(options))
        {
            DataSeeder.SeedWebshop(context);
            int maxCustomerNameLength = context.Customers.Any() ? context.Customers.Max(c => c.Name.Length) : 10;
            string custHeader = "Customer".PadRight(maxCustomerNameLength);
            string orderCountHeader = "Order Count".PadRight(11);
            Console.WriteLine($"{custHeader} | {orderCountHeader}");
            Console.WriteLine(new string('-', maxCustomerNameLength + 3 + 11));
            foreach (var customer in context.Customers)
            {
                var orders = Exercises.GetOrdersByCustomer(context, customer.Name);
                var expectedOrders = context.Orders
                    .Where(o => o.CustomerId == customer.Id)
                    .OrderBy(o => o.Id)
                    .Select(o => o.Id)
                    .ToList();
                var actualOrders = orders.OrderBy(o => o.Id).Select(o => o.Id).ToList();
                if (expectedOrders.SequenceEqual(actualOrders))
                    AnsiConsole.MarkupLine($"[lime]PASS[/] {customer.Name.PadRight(maxCustomerNameLength)} (orders: {actualOrders.Count})");
                else
                    AnsiConsole.MarkupLine($"[red]FAIL[/] {customer.Name.PadRight(maxCustomerNameLength)}\nExpected ({expectedOrders.Count}): {string.Join(", ", expectedOrders)}\nActual ({actualOrders.Count}): {string.Join(", ", actualOrders)}");
            }
        }
    }

    private static void Test_GetTotalAmountSpentByCustomer()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_TotalAmountSpent")
            .Options;
        using (var context = new AppDbContext(options))
        {
            DataSeeder.SeedWebshop(context);
            int maxCustomerNameLength = context.Customers.Any() ? context.Customers.Max(c => c.Name.Length) : 10;
            string custHeader = "Customer".PadRight(maxCustomerNameLength);
            string totalHeader = "Total Spent".PadRight(15);
            Console.WriteLine($"{custHeader} | {totalHeader}");
            Console.WriteLine(new string('-', maxCustomerNameLength + 3 + 15));
            foreach (var customer in context.Customers)
            {
                var total = Exercises.GetTotalAmountSpentByCustomer(context, customer.Name);
                var expected = context.Orders
                    .Join(context.Customers, o => o.CustomerId, c => c.Id, (o, c) => new { o, c })
                    .Where(x => x.c.Name == customer.Name)
                    .Join(context.Payments, x => x.o.Id, p => p.OrderId, (x, p) => p.Amount)
                    .Sum();
                string result = Math.Abs(total - expected) < 0.01m
                    ? $"[lime]PASS[/] {customer.Name.PadRight(maxCustomerNameLength)} (total: {total,12:C})"
                    : $"[red]FAIL[/] {customer.Name.PadRight(maxCustomerNameLength)}\nExpected: {expected,12:C}\nActual:   {total,12:C}";
                AnsiConsole.MarkupLine(result);
            }
        }
    }

    private static void Test_GetCustomersWithLargeOrder()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_CustomersWithLargeOrder")
            .Options;
        using (var context = new AppDbContext(options))
        {
            DataSeeder.SeedWebshop(context);
            var actual = Exercises.GetCustomersWithLargeOrder(context, 1000m).OrderBy(x => x).ToList();

            int maxNameLength = actual.Any() ? actual.Max(n => n.Length) : 10;
            string nameHeader = "Customer".PadRight(maxNameLength);
            string orderHeader = "Highest Order".PadRight(15);
            string fitsHeader = "Fits Condition";
            Console.WriteLine($"{nameHeader} | {orderHeader} | {fitsHeader}");
            Console.WriteLine(new string('-', maxNameLength + 3 + 15 + 3 + fitsHeader.Length));
            foreach (var customerName in actual)
            {
                var customer = context.Customers.First(c => c.Name == customerName);
                var orderIds = context.Orders.Where(o => o.CustomerId == customer.Id).Select(o => o.Id).ToList();
                var maxOrder = context.Payments
                    .Where(p => orderIds.Contains(p.OrderId))
                    .OrderByDescending(p => p.Amount)
                    .FirstOrDefault();
                var maxAmount = maxOrder?.Amount ?? 0m;
                var fitsCondition = maxAmount > 1000m ? "YES" : "NO";
                if (fitsCondition == "YES")
                    AnsiConsole.MarkupLine($"[lime]PASS[/] {customerName.PadRight(maxNameLength)} | {maxAmount,13:C} | {fitsCondition}");
                else
                    AnsiConsole.MarkupLine($"[red]FAIL[/] {customerName.PadRight(maxNameLength)} | {maxAmount,13:C} | {fitsCondition}");
            }
        }
    }

    private static void Test_GetTop3ProductsByQuantity()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Top3ProductsByQuantity")
            .Options;
        using (var context = new AppDbContext(options))
        {
            DataSeeder.SeedWebshop(context);
            var topProducts = Exercises.GetTop3ProductsByQuantity(context);
            var expectedTopProducts = context.OrderItems
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
                .ToList();
            var actualTopProducts = topProducts
                .Select(tp => new { Name = tp.ProductName, tp.TotalQuantity })
                .ToList();
            bool pass = expectedTopProducts.Count == actualTopProducts.Count &&
                        !expectedTopProducts.Where((t, i) => t.Name != actualTopProducts[i].Name || t.TotalQuantity != actualTopProducts[i].TotalQuantity).Any();
            if (pass)
                AnsiConsole.MarkupLine($"[lime]PASS[/] (top: {string.Join(", ", actualTopProducts.Select(tp => tp.Name + " x" + tp.TotalQuantity))})");
            else
                AnsiConsole.MarkupLine($"[red]FAIL[/]\nExpected: {string.Join(", ", expectedTopProducts.Select(tp => tp.Name + " x" + tp.TotalQuantity))}\nActual:   {string.Join(", ", actualTopProducts.Select(tp => tp.Name + " x" + tp.TotalQuantity))}");
        }
    }

    public static void RunAll()
    {
        var tests = new List<(string Name, Action Test)>
        {
            ("GetProductsByCategory", Test_GetProductsByCategory),
            ("GetOrdersByCustomer", Test_GetOrdersByCustomer),
            ("GetTotalAmountSpentByCustomer", Test_GetTotalAmountSpentByCustomer),
            ("GetCustomersWithLargeOrder", Test_GetCustomersWithLargeOrder),
            ("GetTop3ProductsByQuantity", Test_GetTop3ProductsByQuantity)
        };
        foreach (var (name, test) in tests)
        {
            Console.WriteLine($"\n--- {name}");
            test();
            Console.WriteLine(new string('-', 50));
        }
    }
}
