
using CosmeticsDeliverySystem.Data;
using CosmeticsDeliverySystem.Functional;
using CosmeticsDeliverySystem.Models;
using CosmeticsDeliverySystem.Services;

namespace CosmeticsDeliverySystem.Tests;

public class UnitTest1
{
    // ASSIGNMENT 5: ORIGINAL PIPELINE TESTS

    [Fact]
    public void ValidOrder_ShouldPassValidation()
    {
        var order = new DeliveryRequest(
            1, "Aruzhan", 10000m, 2.0, false);

        Assert.True(OrderValidator.IsValid(order));
    }

    [Fact]
    public void NegativeDistance_ShouldFailValidation()
    {
        var order = new DeliveryRequest(
            4, "Madina", 12000m, -3.0, false);

        Assert.False(OrderValidator.IsValid(order));

        Assert.Equal(
            "Distance must be a finite non-negative number.",
            OrderValidator.GetErrorMessage(order));
    }

    [Fact]
    public void ZeroDistance_ShouldBeAccepted()
    {
        var order = new DeliveryRequest(
            3, "Amina", 7000m, 0.0, false);

        Assert.True(OrderValidator.IsValid(order));
    }

    [Fact]
    public void StandardAndExpress_ShouldProduceDifferentPrices()
    {
        decimal standard = DeliveryCalculator.Calculate(
            2.0, DeliveryPolicies.Standard);

        decimal express = DeliveryCalculator.Calculate(
            2.0, DeliveryPolicies.Express);

        Assert.Equal(700m, standard);
        Assert.Equal(1100m, express);
        Assert.NotEqual(standard, express);
    }

    [Fact]
    public void Pipeline_ShouldRejectInvalidOrder()
    {
        var orders = new List<DeliveryRequest>
        {
            new(1, "Aruzhan", 10000m, 2.0, false),
            new(4, "Madina", 12000m, -3.0, false)
        };

        var result = DeliveryPipeline.Run(
            orders, DeliveryPolicies.Standard);

        Assert.Single(result);
        Assert.Equal(1, result[0].OrderId);
    }

    [Fact]
    public void Pipeline_ShouldNotModifyOriginalCollection()
    {
        var orders = new List<DeliveryRequest>
        {
            new(1, "Aruzhan", 10000m, 2.0, false),
            new(3, "Amina", 7000m, 0.0, false)
        };

        var originalOrders = orders.ToArray();

        DeliveryPipeline.Run(
            orders, DeliveryPolicies.Standard);

        Assert.Equal(originalOrders, orders);
    }

    [Fact]
    public void EmptyDataset_ShouldReturnEmptyResult()
    {
        var result = DeliveryPipeline.Run(
            new List<DeliveryRequest>(),
            DeliveryPolicies.Standard);

        Assert.Empty(result);
    }

    [Fact]
    public void MissingDataset_ShouldThrowException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            DeliveryPipeline.Run(
                null!, DeliveryPolicies.Standard));
    }

    // ASSIGNMENT 6: RESULT TESTS

    [Fact]
    public void Result_Ok_ShouldContainValue()
    {
        var result = Result<int, string>.Ok(100);

        var value = result.Match(
            success => success,
            error => -1);

        Assert.Equal(100, value);
    }

    [Fact]
    public void Result_Fail_ShouldContainError()
    {
        var result = Result<int, string>.Fail(
            "Invalid quantity");

        var value = result.Match(
            success => "Success",
            error => error);

        Assert.Equal("Invalid quantity", value);
    }

    // MAYBE TESTS

    [Fact]
    public void Maybe_ShouldFindProduct()
    {
        var maybe = Maybe<int>.Found(100);

        Assert.True(maybe is Maybe<int>.Some);
    }

    [Fact]
    public void Maybe_ShouldRepresentMissingValue()
    {
        var maybe = Maybe<int>.Missing();

        Assert.True(maybe is Maybe<int>.None);
    }

    [Fact]
    public void Maybe_ShouldAllowZeroAsAValidValue()
    {
        var maybe = Maybe<int>.Found(0);

        var result = maybe.Match(
            value => value,
            () => -1);

        Assert.Equal(0, result);
    }

    // MAP AND BIND TESTS

    [Fact]
    public void Map_ShouldTransformSuccessfulValue()
    {
        var result = Result<int, string>.Ok(10);

        var mapped = result.Map(value => value * 2);

        Assert.Equal(
            20,
            mapped.Match(value => value, error => -1));
    }

    [Fact]
    public void Bind_ShouldContinueSuccessfulOperation()
    {
        var result = Result<int, string>.Ok(10);

        var bound = result.Bind(
            value => Result<string, string>.Ok(
                $"Price: {value}"));

        Assert.Equal(
            "Price: 10",
            bound.Match(value => value, error => error));
    }

    [Fact]
    public void MapAndBind_ShouldNotExecuteAfterFailure()
    {
        int counter = 0;

        var failed = Result<int, string>.Fail(
            "Product not found");

        var mapped = failed.Map(value =>
        {
            counter++;
            return value * 2;
        });

        var bound = mapped.Bind(value =>
        {
            counter++;
            return Result<int, string>.Ok(value);
        });

        Assert.Equal(0, counter);

        Assert.Equal(
            "Product not found",
            bound.Match(
                value => "Success",
                error => error));
    }

    // PRODUCT LOOKUP TESTS

    private static ProductRepository CreateRepository()
    {
        return new ProductRepository(
            new List<CosmeticProduct>
            {
                new("Lipstick", 5000m, 10),
                new("Cream", 7000m, 5)
            });
    }

    [Fact]
    public void ProductRepository_ShouldFindExistingProduct()
    {
        var repository = CreateRepository();

        var result = repository.FindByName("Lipstick");

        Assert.True(result is Maybe<CosmeticProduct>.Some);
    }

    [Fact]
    public void ProductRepository_ShouldReturnNoneForMissingProduct()
    {
        var repository = CreateRepository();

        var result = repository.FindByName("Unknown");

        Assert.True(result is Maybe<CosmeticProduct>.None);
    }

    // DOMAIN FAILURE TESTS

    private static DeliveryRequest CreateOrder()
    {
        return new DeliveryRequest(
            10, "Aruzhan", 0m, 2.0, false);
    }

    [Fact]
    public void Workflow_ShouldCreateSuccessfulQuote()
    {
        var result = DeliveryWorkflow.CreateQuote(
            CreateRepository(),
            "Lipstick",
            2,
            CreateOrder(),
            DeliveryPolicies.Standard);

        var quote = result.Match(
            value => value,
            error => throw new Exception(error.Message));

        Assert.Equal(10000m, quote.OrderAmount);
        Assert.Equal(700m, quote.DeliveryCost);
        Assert.Equal(10700m, quote.TotalAmount);
    }

    [Fact]
    public void Workflow_ShouldRejectMissingProduct()
    {
        var result = DeliveryWorkflow.CreateQuote(
            CreateRepository(),
            "Unknown",
            1,
            CreateOrder(),
            DeliveryPolicies.Standard);

        var error = Assert.IsType<
            Result<DeliveryQuote, DomainError>.Failure>(result);

        Assert.Equal("PRODUCT_NOT_FOUND", error.Error.Code);
    }

    [Fact]
    public void Workflow_ShouldRejectInvalidQuantity()
    {
        var result = DeliveryWorkflow.CreateQuote(
            CreateRepository(),
            "Lipstick",
            0,
            CreateOrder(),
            DeliveryPolicies.Standard);

        var error = Assert.IsType<
            Result<DeliveryQuote, DomainError>.Failure>(result);

        Assert.Equal("INVALID_QUANTITY", error.Error.Code);
    }

    [Fact]
    public void Workflow_ShouldRejectInsufficientStock()
    {
        var result = DeliveryWorkflow.CreateQuote(
            CreateRepository(),
            "Lipstick",
            20,
            CreateOrder(),
            DeliveryPolicies.Standard);

        var error = Assert.IsType<
            Result<DeliveryQuote, DomainError>.Failure>(result);

        Assert.Equal("INSUFFICIENT_STOCK", error.Error.Code);
    }

    [Fact]
    public void Workflow_ShouldRejectInvalidDeliveryOrder()
    {
        var invalidOrder = new DeliveryRequest(
            0, "Aruzhan", 0m, -2.0, false);

        var result = DeliveryWorkflow.CreateQuote(
            CreateRepository(),
            "Lipstick",
            1,
            invalidOrder,
            DeliveryPolicies.Standard);

        var error = Assert.IsType<
            Result<DeliveryQuote, DomainError>.Failure>(result);

        Assert.Equal("INVALID_ORDER", error.Error.Code);
    }

    // JSON FILE TESTS

    [Fact]
    public void JsonLoader_ShouldReturnMissingFileError()
    {
        string path = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid() + ".json");

        var result = JsonDataLoader.LoadOrders(path);

        var error = Assert.IsType<
            Result<IReadOnlyList<DeliveryRequest>, DomainError>
                .Failure>(result);

        Assert.Equal("FILE_NOT_FOUND", error.Error.Code);
    }

    [Fact]
    public void JsonLoader_ShouldRejectMalformedJson()
    {
        string path = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid() + ".json");

        try
        {
            File.WriteAllText(path, "{ invalid json }");

            var result = JsonDataLoader.LoadOrders(path);

            var error = Assert.IsType<
                Result<IReadOnlyList<DeliveryRequest>, DomainError>
                    .Failure>(result);

            Assert.Equal("INVALID_JSON", error.Error.Code);
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }

    [Fact]
    public void JsonLoader_ShouldLoadValidOrders()
    {
        string path = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid() + ".json");

        try
        {
            File.WriteAllText(
                path,
                """
                [
                  {
                    "OrderId": 1,
                    "CustomerName": "Aruzhan",
                    "OrderAmount": 10000,
                    "DistanceKm": 2,
                    "IsExpress": false
                  }
                ]
                """);

            var result = JsonDataLoader.LoadOrders(path);

            var orders = result.Match(
                value => value,
                error => throw new Exception(error.Message));

            Assert.Single(orders);
            Assert.Equal("Aruzhan", orders[0].CustomerName);
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
