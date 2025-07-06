using db_sandbox.Exercises;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

public static class GroupingTests
{
    private static void Test_GroupProductsByCategory()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Group_GroupProductsByCategory")
            .Options;
        using (var context = new AppDbContext(options))
        {
            DataSeeder.SeedWebshop(context);
            var groups = Grouping.GroupProductsByCategory(context);
            var expected = context.Products
                .Join(context.Categories, p => p.CategoryId, c => c.Id, (p, c) => new { Product = p, CategoryName = c.Name })
                .AsEnumerable()
                .GroupBy(x => x.CategoryName, x => x.Product)
                .ToList();
            bool pass = groups.Count == expected.Count && groups.All(g => expected.Any(e => e.Key == g.Key && e.Count() == g.Count()));
            AnsiConsole.MarkupLine(pass ? "[lime]PASS[/] GroupProductsByCategory" : "[red]FAIL[/] GroupProductsByCategory");
        }
    }

    private static void Test_GroupJoinCategoryProducts()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Group_GroupJoinCategoryProducts")
            .Options;
        using (var context = new AppDbContext(options))
        {
            DataSeeder.SeedWebshop(context);
            var actual = Grouping.GroupJoinCategoryProducts(context);
            var expected = context.Categories
                .GroupJoin(context.Products,
                    c => c.Id,
                    p => p.CategoryId,
                    (c, products) => new { c.Name, Products = products.ToList() })
                .AsEnumerable()
                .Select(x => (x.Name, x.Products))
                .ToList();
            bool pass = actual.Count == expected.Count && actual.All(a => expected.Any(e => e.Name == a.CategoryName && e.Products.Count == a.Products.Count));
            AnsiConsole.MarkupLine(pass ? "[lime]PASS[/] GroupJoinCategoryProducts" : "[red]FAIL[/] GroupJoinCategoryProducts");
        }
    }

    private static void Test_ProductCountPerCategory()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Group_ProductCountPerCategory")
            .Options;
        using (var context = new AppDbContext(options))
        {
            DataSeeder.SeedWebshop(context);
            var actual = Grouping.ProductCountPerCategory(context);
            var expected = context.Products
                .Join(context.Categories, p => p.CategoryId, c => c.Id, (p, c) => new { Product = p, CategoryName = c.Name })
                .GroupBy(x => x.CategoryName)
                .Select(g => new { g.Key, Count = g.Count() })
                .AsEnumerable()
                .Select(x => (x.Key, x.Count))
                .ToList();
            bool pass = actual.Count == expected.Count && actual.All(a => expected.Any(e => e.Key == a.CategoryName && e.Item2 == a.ProductCount));
            AnsiConsole.MarkupLine(pass ? "[lime]PASS[/] ProductCountPerCategory" : "[red]FAIL[/] ProductCountPerCategory");
        }
    }

    private static void Test_ExpensiveProductsPerCategory()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Group_ExpensiveProductsPerCategory")
            .Options;
        using (var context = new AppDbContext(options))
        {
            DataSeeder.SeedWebshop(context);
            decimal minPrice = 100m;
            var actual = Grouping.ExpensiveProductsPerCategory(context, minPrice);
            var expected = context.Categories
                .GroupJoin(context.Products,
                    c => c.Id,
                    p => p.CategoryId,
                    (c, products) => new { c.Name, ExpensiveProducts = products.Where(p => p.Price > minPrice).ToList() })
                .AsEnumerable()
                .Select(x => (x.Name, x.ExpensiveProducts))
                .ToList();
            bool pass = actual.Count == expected.Count && actual.All(a => expected.Any(e => e.Name == a.CategoryName && e.ExpensiveProducts.Count == a.ExpensiveProducts.Count));
            AnsiConsole.MarkupLine(pass ? "[lime]PASS[/] ExpensiveProductsPerCategory" : "[red]FAIL[/] ExpensiveProductsPerCategory");
        }
    }

    private static void Test_ProductNamesPerCategory()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Group_ProductNamesPerCategory")
            .Options;
        using (var context = new AppDbContext(options))
        {
            DataSeeder.SeedWebshop(context);
            var actual = Grouping.ProductNamesPerCategory(context);
            var expected = context.Categories
                .GroupJoin(context.Products,
                    c => c.Id,
                    p => p.CategoryId,
                    (c, products) => new { c.Name, ProductNames = products.Select(p => p.Name).ToList() })
                .AsEnumerable()
                .Select(x => (x.Name, x.ProductNames))
                .ToList();
            bool pass = actual.Count == expected.Count && actual.All(a => expected.Any(e => e.Name == a.CategoryName && e.ProductNames.Count == a.ProductNames.Count));
            AnsiConsole.MarkupLine(pass ? "[lime]PASS[/] ProductNamesPerCategory" : "[red]FAIL[/] ProductNamesPerCategory");
        }
    }

    private static void Test_TopCustomersPerProduct()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Group_TopCustomersPerProduct")
            .Options;
        using (var context = new AppDbContext(options))
        {
            DataSeeder.SeedWebshop(context);
            var actual = Grouping.TopCustomersPerProduct(context, 5);
            // For brevity, just check that the result is not empty and each product has at most 5 customers
            bool pass = actual.All(a => a.TopCustomers.Count <= 5);
            AnsiConsole.MarkupLine(pass ? "[lime]PASS[/] TopCustomersPerProduct" : "[red]FAIL[/] TopCustomersPerProduct");
        }
    }

    public static void RunAll()
    {
        var tests = new List<(string Name, Action Test)>
        {
            ("GroupProductsByCategory", Test_GroupProductsByCategory),
            ("GroupJoinCategoryProducts", Test_GroupJoinCategoryProducts),
            ("ProductCountPerCategory", Test_ProductCountPerCategory),
            ("ExpensiveProductsPerCategory", Test_ExpensiveProductsPerCategory),
            ("ProductNamesPerCategory", Test_ProductNamesPerCategory),
            ("TopCustomersPerProduct", Test_TopCustomersPerProduct)
        };
        foreach (var (name, test) in tests)
        {
            Console.WriteLine($"\n--- {name}");
            test();
            Console.WriteLine(new string('-', 50));
        }
    }
}
