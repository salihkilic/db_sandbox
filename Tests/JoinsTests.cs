using db_sandbox.Exercises;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace db_sandbox.Tests;

// Helper for comparing (string, List<int>) tuples by value
class CustomerOrdersTupleComparer : IEqualityComparer<(string, List<int>)>
{
    public bool Equals((string, List<int>) x, (string, List<int>) y)
    {
        if (!string.Equals(x.Item1, y.Item1))
            return false;
        if (x.Item2 == null && y.Item2 == null)
            return true;
        if (x.Item2 == null || y.Item2 == null)
            return false;
        // Order-insensitive comparison
        return x.Item2.Count == y.Item2.Count && !x.Item2.Except(y.Item2).Any() && !y.Item2.Except(x.Item2).Any();
    }
    public int GetHashCode((string, List<int>) obj)
    {
        int hash = obj.Item1?.GetHashCode() ?? 0;
        if (obj.Item2 != null)
        {
            // Order-insensitive hash: XOR all hashes
            hash = obj.Item2.Aggregate(hash, (current, id) => current ^ id.GetHashCode());
        }
        return hash;
    }
}

public static class JoinsTests
{
    private static void Test_LeftInnerJoin()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Join_LeftInner")
            .Options;
        using (var context = new AppDbContext(options))
        {
            DataSeeder.SeedWebshop(context);
            var actual = Joins.LeftInnerJoinProductsCategories(context);
            var expected = (from p in context.Products
                            join c in context.Categories on p.CategoryId equals c.Id
                            select new { p.Name, CategoryName = c.Name })
                            .AsEnumerable()
                            .Select(x => (x.Name, x.CategoryName))
                            .ToList();
            bool pass = actual.OrderBy(x => x.Item1 + x.Item2)
                              .SequenceEqual(expected.OrderBy(x => x.Item1 + x.Item2));
            AnsiConsole.MarkupLine(pass ? "[lime]PASS[/] LeftInnerJoin" : $"[red]FAIL[/] LeftInnerJoin\nExpected: {string.Join(", ", expected)}\nActual: {string.Join(", ", actual)}");
        }
    }

    private static void Test_LeftOuterJoin()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Join_LeftOuter")
            .Options;
        using (var context = new AppDbContext(options))
        {
            DataSeeder.SeedWebshop(context);
            var actual = Joins.LeftOuterJoinProductsCategories(context);
            var expected = (from p in context.Products
                            join c in context.Categories on p.CategoryId equals c.Id into pc
                            from c in pc.DefaultIfEmpty()
                            select new { p.Name, CategoryName = c != null ? c.Name : null })
                            .AsEnumerable()
                            .Select(x => (x.Name, x.CategoryName))
                            .ToList();
            bool pass = actual.OrderBy(x => x.Item1 + x.Item2)
                              .SequenceEqual(expected.OrderBy(x => x.Item1 + x.Item2));
            AnsiConsole.MarkupLine(pass ? "[lime]PASS[/] LeftOuterJoin" : $"[red]FAIL[/] LeftOuterJoin\nExpected: {string.Join(", ", expected)}\nActual: {string.Join(", ", actual)}");
        }
    }

    private static void Test_FullOuterJoin()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Join_FullOuter")
            .Options;
        using (var context = new AppDbContext(options))
        {
            DataSeeder.SeedWebshop(context);
            var actual = Joins.FullOuterJoinProductsCategories(context);
            var left = (from p in context.Products
                        join c in context.Categories on p.CategoryId equals c.Id into pc
                        from c in pc.DefaultIfEmpty()
                        select new { ProductName = p.Name, CategoryName = c != null ? c.Name : null })
                        .AsEnumerable()
                        .Select(x => (x.ProductName, x.CategoryName));
            var right = (from c in context.Categories
                         join p in context.Products on c.Id equals p.CategoryId into cp
                         from p in cp.DefaultIfEmpty()
                         where p == null
                         select new { ProductName = (string)null, CategoryName = c.Name })
                         .AsEnumerable()
                         .Select(x => (x.ProductName, x.CategoryName));
            var expected = left.Concat(right).ToList();
            bool pass = actual.Count == expected.Count &&
                        !expected.Except(actual).Any() &&
                        !actual.Except(expected).Any();
            AnsiConsole.MarkupLine(pass ? "[lime]PASS[/] FullOuterJoin" : $"[red]FAIL[/] FullOuterJoin\nExpected: {string.Join(", ", expected)}\nActual: {string.Join(", ", actual)}");
        }
    }

    private static void Test_MultipleJoins()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Join_Multiple")
            .Options;
        using (var context = new AppDbContext(options))
        {
            DataSeeder.SeedWebshop(context);
            var actual = Joins.MultipleJoinsOrderItems(context);
            var expected = (from oi in context.OrderItems
                            join o in context.Orders on oi.OrderId equals o.Id
                            join c in context.Customers on o.CustomerId equals c.Id
                            join p in context.Products on oi.ProductId equals p.Id
                            select new { CategoryName = c.Name, p.Name, oi.Quantity })
                            .AsEnumerable()
                            .Select(x => (x.CategoryName, x.Name, x.Quantity))
                            .ToList();
            bool pass = actual.Count == expected.Count &&
                        !expected.Except(actual).Any() &&
                        !actual.Except(expected).Any();
            AnsiConsole.MarkupLine(pass ? "[lime]PASS[/] MultipleJoins" : $"[red]FAIL[/] MultipleJoins\nExpected: {string.Join(", ", expected)}\nActual: {string.Join(", ", actual)}");
        }
    }

    private static void Test_GroupJoin()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Join_GroupJoin")
            .Options;
        
        using (var context = new AppDbContext(options))
        {
            DataSeeder.SeedWebshop(context);
            var actual = Joins.GroupJoinCustomerOrders(context);
            var expected = (from c in context.Customers
                            join o in context.Orders on c.Id equals o.CustomerId into orders
                            select new { c.Name, OrderIds = orders.Select(x => x.Id).ToList() })
                            .AsEnumerable()
                            .Select(x => (x.Name, x.OrderIds))
                            .ToList();
            
            // Use the custom comparer to compare the result tuples
            var comparer = new CustomerOrdersTupleComparer();
            bool pass = actual.Count == expected.Count &&
                        !expected.Except(actual, comparer).Any() &&
                        !actual.Except(expected, comparer).Any();
            
            // Test output
            if (pass)
            {
                AnsiConsole.MarkupLine("[lime]PASS[/] GroupJoin");
            }
            else
            {
                // Check for missing and extra items
                var missing = expected.Except(actual, comparer).ToList();
                var extra = actual.Except(expected, comparer).ToList();
                
                // Format the output for missing and extra items
                string missingStr = string.Join("\n ", missing.Select(e => $"({e.Item1}, [{string.Join(",", e.Item2)}])"));
                string extraStr = string.Join("\n ", extra.Select(a => $"({a.Item1}, [{string.Join(",", a.Item2)}])"));
                
                // Print the results
                AnsiConsole.MarkupLine($"[red]FAIL[/] GroupJoin");
                if (missing.Any())
                    AnsiConsole.MarkupLine($"[yellow]Missing:[/]\n{missingStr}");
                if (extra.Any())
                    AnsiConsole.MarkupLine($"[yellow]Extra:[/]\n{extraStr}");
            }
        }
    }

    public static void RunAll()
    {
        var tests = new List<(string Name, Action Test)>
        {
            ("LeftInnerJoin", Test_LeftInnerJoin),
            ("LeftOuterJoin", Test_LeftOuterJoin),
            ("FullOuterJoin", Test_FullOuterJoin),
            ("MultipleJoins", Test_MultipleJoins),
            ("GroupJoin", Test_GroupJoin)
        };
        foreach (var (name, test) in tests)
        {
            Console.WriteLine($"\n--- {name}");
            test();
            Console.WriteLine(new string('-', 50));
        }
    }
}