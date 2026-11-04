using CosmeticsDeliverySystem.Models;
using CosmeticsDeliverySystem.Services;

namespace CosmeticsDeliverySystem.Functional;

public static class DeliveryPipeline
{
    public static IReadOnlyList<DeliveryQuote> Run(
    IReadOnlyList<DeliveryRequest> orders,
    Func<decimal, decimal> deliveryPolicy)
    {
        ArgumentNullException.ThrowIfNull(orders);
        ArgumentNullException.ThrowIfNull(deliveryPolicy);

    var validOrders = orders
        .Where(OrderValidator.IsValid)
        .ToList();

      
        var quotes = validOrders
            .Select(order => DeliveryQuoteBuilder.CreateQuote(
                order,
                deliveryPolicy))
            .ToList();

        var result = quotes
            .OrderBy(quote => quote.TotalAmount)
            .ToList();

        return result.AsReadOnly();
    }


}
