
using CosmeticsDeliverySystem.Functional;
using CosmeticsDeliverySystem.Models;

namespace CosmeticsDeliverySystem.Services;

public class ProductRepository
{
    private readonly IReadOnlyList<CosmeticProduct> _products;

    public ProductRepository(
        IReadOnlyList<CosmeticProduct> products)
    {
        ArgumentNullException.ThrowIfNull(products);
        _products = products.ToList().AsReadOnly();
    }

    public Maybe<CosmeticProduct> FindByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Maybe<CosmeticProduct>.Missing();

        var product = _products.FirstOrDefault(
            p => string.Equals(
                p.Name,
                name,
                StringComparison.OrdinalIgnoreCase));

        return product is null
            ? Maybe<CosmeticProduct>.Missing()
            : Maybe<CosmeticProduct>.Found(product);
    }
}
