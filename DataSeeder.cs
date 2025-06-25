public static class DataSeeder
{
    public static void SeedWebshop(AppDbContext context)
    {
        SeedCategories(context);
        SeedProducts(context);
        SeedCustomers(context);
        SeedAddresses(context);
        SeedOrders(context);
        SeedOrderItems(context);
        SeedPayments(context);
    }

    private static void SeedCategories(AppDbContext context)
    {
        var categories = new[]
        {
            new Category { Name = "Electronics" },
            new Category { Name = "Books" },
            new Category { Name = "Clothing" },
            new Category { Name = "Home & Kitchen" },
            new Category { Name = "Sports & Outdoors" },
            new Category { Name = "Toys & Games" },
            new Category { Name = "Beauty & Health" },
            new Category { Name = "Automotive" },
            new Category { Name = "Garden & Tools" },
            new Category { Name = "Office Supplies" }
        };
        context.Categories.AddRange(categories);
        context.SaveChanges();
    }

    private static void SeedProducts(AppDbContext context)
    {
        var categories = context.Categories.ToList();
        var products = new List<Product>
        {
            // Electronics
            new Product { Name = "Smartphone", Price = 699.99m, CategoryId = categories.First(c => c.Name == "Electronics").Id },
            new Product { Name = "Laptop", Price = 1299.99m, CategoryId = categories.First(c => c.Name == "Electronics").Id },
            new Product { Name = "Bluetooth Headphones", Price = 149.99m, CategoryId = categories.First(c => c.Name == "Electronics").Id },
            new Product { Name = "Smartwatch", Price = 199.99m, CategoryId = categories.First(c => c.Name == "Electronics").Id },
            new Product { Name = "Tablet", Price = 399.99m, CategoryId = categories.First(c => c.Name == "Electronics").Id },
            // Books
            new Product { Name = "Science Fiction Novel", Price = 14.99m, CategoryId = categories.First(c => c.Name == "Books").Id },
            new Product { Name = "Cookbook", Price = 24.99m, CategoryId = categories.First(c => c.Name == "Books").Id },
            new Product { Name = "Children's Storybook", Price = 9.99m, CategoryId = categories.First(c => c.Name == "Books").Id },
            new Product { Name = "Mystery Thriller", Price = 12.99m, CategoryId = categories.First(c => c.Name == "Books").Id },
            new Product { Name = "Programming Guide", Price = 39.99m, CategoryId = categories.First(c => c.Name == "Books").Id },
            // Clothing
            new Product { Name = "Men's T-Shirt", Price = 19.99m, CategoryId = categories.First(c => c.Name == "Clothing").Id },
            new Product { Name = "Women's Jeans", Price = 49.99m, CategoryId = categories.First(c => c.Name == "Clothing").Id },
            new Product { Name = "Sneakers", Price = 59.99m, CategoryId = categories.First(c => c.Name == "Clothing").Id },
            new Product { Name = "Jacket", Price = 89.99m, CategoryId = categories.First(c => c.Name == "Clothing").Id },
            new Product { Name = "Socks (5-pack)", Price = 12.99m, CategoryId = categories.First(c => c.Name == "Clothing").Id },
            // Home & Kitchen
            new Product { Name = "Blender", Price = 79.99m, CategoryId = categories.First(c => c.Name == "Home & Kitchen").Id },
            new Product { Name = "Coffee Maker", Price = 99.99m, CategoryId = categories.First(c => c.Name == "Home & Kitchen").Id },
            new Product { Name = "Nonstick Frying Pan", Price = 29.99m, CategoryId = categories.First(c => c.Name == "Home & Kitchen").Id },
            new Product { Name = "Vacuum Cleaner", Price = 149.99m, CategoryId = categories.First(c => c.Name == "Home & Kitchen").Id },
            new Product { Name = "Desk Lamp", Price = 34.99m, CategoryId = categories.First(c => c.Name == "Home & Kitchen").Id },
            // Sports & Outdoors
            new Product { Name = "Mountain Bike", Price = 499.99m, CategoryId = categories.First(c => c.Name == "Sports & Outdoors").Id },
            new Product { Name = "Football", Price = 24.99m, CategoryId = categories.First(c => c.Name == "Sports & Outdoors").Id },
            new Product { Name = "Yoga Mat", Price = 19.99m, CategoryId = categories.First(c => c.Name == "Sports & Outdoors").Id },
            new Product { Name = "Tennis Racket", Price = 89.99m, CategoryId = categories.First(c => c.Name == "Sports & Outdoors").Id },
            new Product { Name = "Camping Tent", Price = 129.99m, CategoryId = categories.First(c => c.Name == "Sports & Outdoors").Id },
            // Toys & Games
            new Product { Name = "Board Game", Price = 29.99m, CategoryId = categories.First(c => c.Name == "Toys & Games").Id },
            new Product { Name = "Puzzle (1000 pieces)", Price = 19.99m, CategoryId = categories.First(c => c.Name == "Toys & Games").Id },
            new Product { Name = "Remote Control Car", Price = 49.99m, CategoryId = categories.First(c => c.Name == "Toys & Games").Id },
            new Product { Name = "Dollhouse", Price = 59.99m, CategoryId = categories.First(c => c.Name == "Toys & Games").Id },
            new Product { Name = "Action Figure", Price = 14.99m, CategoryId = categories.First(c => c.Name == "Toys & Games").Id },
            // Beauty & Health
            new Product { Name = "Electric Toothbrush", Price = 39.99m, CategoryId = categories.First(c => c.Name == "Beauty & Health").Id },
            new Product { Name = "Hair Dryer", Price = 29.99m, CategoryId = categories.First(c => c.Name == "Beauty & Health").Id },
            new Product { Name = "Shampoo (500ml)", Price = 8.99m, CategoryId = categories.First(c => c.Name == "Beauty & Health").Id },
            new Product { Name = "Face Cream", Price = 24.99m, CategoryId = categories.First(c => c.Name == "Beauty & Health").Id },
            new Product { Name = "Razor Set", Price = 19.99m, CategoryId = categories.First(c => c.Name == "Beauty & Health").Id },
            // Automotive
            new Product { Name = "Car Vacuum", Price = 34.99m, CategoryId = categories.First(c => c.Name == "Automotive").Id },
            new Product { Name = "Windshield Sun Shade", Price = 15.99m, CategoryId = categories.First(c => c.Name == "Automotive").Id },
            new Product { Name = "Tire Inflator", Price = 44.99m, CategoryId = categories.First(c => c.Name == "Automotive").Id },
            new Product { Name = "Car Phone Mount", Price = 12.99m, CategoryId = categories.First(c => c.Name == "Automotive").Id },
            new Product { Name = "Jump Starter", Price = 69.99m, CategoryId = categories.First(c => c.Name == "Automotive").Id },
            // Garden & Tools
            new Product { Name = "Garden Hose", Price = 24.99m, CategoryId = categories.First(c => c.Name == "Garden & Tools").Id },
            new Product { Name = "Cordless Drill", Price = 89.99m, CategoryId = categories.First(c => c.Name == "Garden & Tools").Id },
            new Product { Name = "Lawn Mower", Price = 249.99m, CategoryId = categories.First(c => c.Name == "Garden & Tools").Id },
            new Product { Name = "Pruning Shears", Price = 14.99m, CategoryId = categories.First(c => c.Name == "Garden & Tools").Id },
            new Product { Name = "Tool Set", Price = 59.99m, CategoryId = categories.First(c => c.Name == "Garden & Tools").Id },
            // Office Supplies
            new Product { Name = "Notebook (A4)", Price = 4.99m, CategoryId = categories.First(c => c.Name == "Office Supplies").Id },
            new Product { Name = "Ballpoint Pens (10-pack)", Price = 6.99m, CategoryId = categories.First(c => c.Name == "Office Supplies").Id },
            new Product { Name = "Desk Organizer", Price = 19.99m, CategoryId = categories.First(c => c.Name == "Office Supplies").Id },
            new Product { Name = "Stapler", Price = 9.99m, CategoryId = categories.First(c => c.Name == "Office Supplies").Id },
            new Product { Name = "Office Chair", Price = 129.99m, CategoryId = categories.First(c => c.Name == "Office Supplies").Id }
        };
        context.Products.AddRange(products);
        context.SaveChanges();
    }

    private static void SeedCustomers(AppDbContext context)
    {
        var customerNames = new[]
        {
            "Alice Johnson", "Bob Smith", "Carol Lee", "David Brown", "Eve Clark",
            "Frank Harris", "Grace Lewis", "Hank Walker", "Ivy Young", "Jack King",
            "Kathy Wright", "Leo Scott", "Mona Green", "Nina Adams", "Oscar Baker",
            "Paula Carter", "Quinn Evans", "Rita Foster", "Sam Gray", "Tina Hill"
        };
        foreach (var name in customerNames)
        {
            context.Customers.Add(new Customer { Name = name });
        }
        context.SaveChanges();
    }

    private static void SeedAddresses(AppDbContext context)
    {
        var customers = context.Customers.ToList();
        var addresses = new List<Address>();
        
        // Each customer gets at least one address, some get two
        int addressId = 1;
        foreach (var customer in customers)
        {
            addresses.Add(new Address
            {
                Street = $"{addressId * 10} Main St",
                City = $"City {addressId % 10 + 1}",
                Country = "USA",
                CustomerId = customer.Id
            });
            addressId++;
            // every 4th customer gets a second address
            if (addressId <= 25 && customer.Id % 4 == 0) 
            {
                addresses.Add(new Address
                {
                    Street = $"{addressId * 10} Oak Ave",
                    City = $"City {addressId % 10 + 1}",
                    Country = "USA",
                    CustomerId = customer.Id
                });
                addressId++;
            }
            if (addressId > 25) break;
        }
        context.Addresses.AddRange(addresses);
        context.SaveChanges();
    }

    private static void SeedOrders(AppDbContext context)
    {
        var customers = context.Customers.ToList();
        var random = new Random();
        var orders = new List<Order>();
        foreach (var customer in customers)
        {
            int orderCount = random.Next(5, 16); // 5 to 15 orders per customer
            for (int i = 0; i < orderCount; i++)
            {
                orders.Add(new Order
                {
                    CustomerId = customer.Id,
                    OrderDate = DateTime.UtcNow.AddDays(-random.Next(1, 365))
                });
            }
        }
        context.Orders.AddRange(orders);
        context.SaveChanges();
    }

    private static void SeedOrderItems(AppDbContext context)
    {
        var orders = context.Orders.ToList();
        var products = context.Products.ToList();
        var random = new Random();
        var orderItems = new List<OrderItem>();
        
        foreach (var order in orders)
        {
            int itemCount = random.Next(5, 16); // 5 to 15 items per order
            var usedProductIndexes = new HashSet<int>();
            for (int i = 0; i < itemCount; i++)
            {
                int productIndex;
                do
                {
                    productIndex = random.Next(products.Count);
                } while (usedProductIndexes.Contains(productIndex));
                usedProductIndexes.Add(productIndex);
                var product = products[productIndex];
                orderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = random.Next(1, 6)
                });
            }
        }
        context.OrderItems.AddRange(orderItems);
        context.SaveChanges();
    }

    private static void SeedPayments(AppDbContext context)
    {
        var orders = context.Orders.ToList();
        var orderItems = context.OrderItems.ToList();
        var products = context.Products.ToList();
        var payments = new List<Payment>();
        foreach (var order in orders)
        {
            var items = orderItems.Where(oi => oi.OrderId == order.Id).ToList();
            decimal total = 0m;
            foreach (var item in items)
            {
                var product = products.First(p => p.Id == item.ProductId);
                total += product.Price * item.Quantity;
            }
            payments.Add(new Payment
            {
                OrderId = order.Id,
                Amount = total,
                PaidAt = order.OrderDate.AddHours(1)
            });
        }
        context.Payments.AddRange(payments);
        context.SaveChanges();
    }
}
