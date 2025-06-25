using Microsoft.EntityFrameworkCore;

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
            var products = Exercises.GetProductsByCategory(context, "Electronics");
            var expected = 5;
            var actual = products.Count;
            if (actual == expected && products.Exists(p => p.Name == "Laptop") && products.Exists(p => p.Name == "Smartphone"))
                Console.WriteLine($"PASS (count: {actual})");
            else
                Console.WriteLine($"GetProductsByCategory: FAIL (expected: {expected}, actual: {actual})");
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
            var orders = Exercises.GetOrdersByCustomer(context, "Alice Johnson");
            var expectedMin = 5;
            var expectedMax = 15;
            var actual = orders.Count;
            if (actual >= expectedMin && actual <= expectedMax)
                Console.WriteLine($"PASS (orders: {actual})");
            else
                Console.WriteLine($"Test_GetOrdersByCustomer: FAIL (expected: between {expectedMin} and {expectedMax}, actual: {actual})");
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
            var total = Exercises.GetTotalAmountSpentByCustomer(context, "Alice Johnson");
            var expected = context.Orders
                .Join(context.Customers, o => o.CustomerId, c => c.Id, (o, c) => new { o, c })
                .Where(x => x.c.Name == "Alice Johnson")
                .Join(context.Payments, x => x.o.Id, p => p.OrderId, (x, p) => p.Amount)
                .Sum();
            if (Math.Abs(total - expected) < 0.01m)
                Console.WriteLine($"PASS (total: {total:C})");
            else
                Console.WriteLine($"Test_GetTotalAmountSpentByCustomer: FAIL (expected: {expected:C}, actual: {total:C})");
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
                Console.WriteLine($"{customerName.PadRight(maxNameLength)} | {maxAmount,13:C} | {fitsCondition}");
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
            var expectedCount = 3;
            var actualCount = topProducts.Count;
            if (actualCount == expectedCount)
                Console.WriteLine($"PASS (top: {string.Join(", ", topProducts.Select(tp => tp.ProductName + " x" + tp.TotalQuantity))})");
            else
                Console.WriteLine($"Test_GetTop3ProductsByQuantity: FAIL (expected: {expectedCount}, actual: {actualCount})");
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
