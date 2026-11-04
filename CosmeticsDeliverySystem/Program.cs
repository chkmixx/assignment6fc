
using CosmeticsDeliverySystem.Data;
using CosmeticsDeliverySystem.Functional;
using CosmeticsDeliverySystem.Models;
using CosmeticsDeliverySystem.Services;

namespace CosmeticsDeliverySystem;

internal class Program
{
    private static void Main()
    {
        Console.WriteLine("=== COSMETICS DELIVERY SYSTEM ===");
        Console.WriteLine();

        RunJsonDemo();
        RunDeliveryDemo();
        RunErrorDemo();

        Console.WriteLine();
        Console.WriteLine("Program finished.");
    }

    private static void RunJsonDemo()
    {
        Console.WriteLine("=== JSON LOADING ===");

        string path = Path.Combine(
            AppContext.BaseDirectory,
            "Data",
            "orders.json");

        var result = JsonDataLoader.LoadOrders(path);

        result.Match(
            orders =>
            {
                Console.WriteLine(
                    $"Successfully loaded {orders.Count} orders.");
                return true;
            },
            error =>
            {
                Console.WriteLine(
                    $"Loading failed: {error.Code} - {error.Message}");
                return false;
            });

        Console.WriteLine();
    }

    private static void RunDeliveryDemo()
    {
        Console.WriteLine("=== DELIVERY POLICIES ===");

        var orders = SampleData.GetOrders();

        RunPolicy(
            "STANDARD DELIVERY",
            DeliveryPolicies.Standard,
            orders);

        RunPolicy(
            "EXPRESS DELIVERY",
            DeliveryPolicies.Express,
            orders);
    }

    private static void RunPolicy(
        string name,
        Func<decimal, decimal> policy,
        IReadOnlyList<DeliveryRequest> orders)
    {
        Console.WriteLine($"--- {name} ---");

        var quotes = DeliveryPipeline.Run(orders, policy);

        foreach (var quote in quotes)
        {
            Console.WriteLine(
                $"Order #{quote.OrderId} | " +
                $"{quote.CustomerName} | " +
                $"Total: {quote.TotalAmount:F2} KZT");
        }

        Console.WriteLine($"Accepted orders: {quotes.Count}");
        Console.WriteLine();
    }

    private static void RunErrorDemo()
    {
        Console.WriteLine("=== RESULT WORKFLOW ===");

        var products = new List<CosmeticProduct>
        {
            new("Lipstick", 5000m, 10),
            new("Cream", 7000m, 5),
            new("Perfume", 12000m, 3)
        };

        var repository = new ProductRepository(products);

        var order = new DeliveryRequest(
            10,
            "Aruzhan",
            0m,
            2.0,
            false);

        var success = DeliveryWorkflow.CreateQuote(
            repository,
            "Lipstick",
            2,
            order,
            DeliveryPolicies.Standard);

        success.Match(
            quote =>
            {
                Console.WriteLine(
                    $"Success: {quote.CustomerName}, " +
                    $"Total: {quote.TotalAmount} KZT");
                return true;
            },
            error =>
            {
                Console.WriteLine(
                    $"Failure: {error.Code} - {error.Message}");
                return false;
            });

        var invalid = DeliveryWorkflow.CreateQuote(
            repository,
            "Lipstick",
            -2,
            order,
            DeliveryPolicies.Standard);

        invalid.Match(
            quote =>
            {
                Console.WriteLine("Order accepted.");
                return true;
            },
            error =>
            {
                Console.WriteLine(
                    $"Expected error: {error.Code} - {error.Message}");
                return false;
            });

        var missing = DeliveryWorkflow.CreateQuote(
            repository,
            "Unknown Product",
            1,
            order,
            DeliveryPolicies.Standard);

        missing.Match(
            quote =>
            {
                Console.WriteLine("Product found.");
                return true;
            },
            error =>
            {
                Console.WriteLine(
                    $"Lookup error: {error.Code} - {error.Message}");
                return false;
            });

        Console.WriteLine();
    }
}
