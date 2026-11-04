
using CosmeticsDeliverySystem.Functional;
using CosmeticsDeliverySystem.Models;
using CosmeticsDeliverySystem.Services;

namespace CosmeticsDeliverySystem.Functional;

public static class DeliveryWorkflow
{
    public static Result<DeliveryQuote, DomainError> CreateQuote(
        ProductRepository repository,
        string productName,
        int quantity,
        DeliveryRequest order,
        Func<decimal, decimal> deliveryPolicy)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(deliveryPolicy);

        var productResult = repository
            .FindByName(productName)
            .ToResult(new DomainError(
                "PRODUCT_NOT_FOUND",
                $"Product '{productName}' was not found."));

        return productResult
            .Bind(product => ValidateQuantity(product, quantity))
            .Bind(product =>
            {
                var updatedOrder = order with
                {
                    OrderAmount = product.Price * quantity
                };

                if (!OrderValidator.IsValid(updatedOrder))
                {
                    return Result<DeliveryRequest, DomainError>.Fail(
                        new DomainError(
                            "INVALID_ORDER",
                            OrderValidator.GetErrorMessage(updatedOrder)));
                }

                return Result<DeliveryRequest, DomainError>.Ok(
                    updatedOrder);
            })
            .Map(validOrder =>
                DeliveryQuoteBuilder.CreateQuote(
                    validOrder,
                    deliveryPolicy));
    }

    private static Result<CosmeticProduct, DomainError>
        ValidateQuantity(
            CosmeticProduct product,
            int quantity)
    {
        if (quantity <= 0)
        {
            return Result<CosmeticProduct, DomainError>.Fail(
                new DomainError(
                    "INVALID_QUANTITY",
                    "Quantity must be greater than zero."));
        }

        if (quantity > product.Quantity)
        {
            return Result<CosmeticProduct, DomainError>.Fail(
                new DomainError(
                    "INSUFFICIENT_STOCK",
                    "Not enough products in stock."));
        }

        return Result<CosmeticProduct, DomainError>.Ok(product);
    }
}
