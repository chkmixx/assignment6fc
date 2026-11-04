
using System.Text.Json;
using CosmeticsDeliverySystem.Functional;
using CosmeticsDeliverySystem.Models;

namespace CosmeticsDeliverySystem.Data;

public static class JsonDataLoader
{
    public static Result<IReadOnlyList<DeliveryRequest>, DomainError>
        LoadOrders(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return Result<IReadOnlyList<DeliveryRequest>, DomainError>
                .Fail(new DomainError(
                    "INVALID_PATH",
                    "File path is required."));
        }

        try
        {
            string json = File.ReadAllText(filePath);

            var orders =
                JsonSerializer.Deserialize<List<DeliveryRequest>>(json);

            if (orders is null)
            {
                return Result<IReadOnlyList<DeliveryRequest>, DomainError>
                    .Fail(new DomainError(
                        "EMPTY_JSON",
                        "JSON does not contain an order list."));
            }

            IReadOnlyList<DeliveryRequest> result =
                orders.AsReadOnly();

            return Result<IReadOnlyList<DeliveryRequest>, DomainError>
                .Ok(result);
        }
        catch (FileNotFoundException)
        {
            return Result<IReadOnlyList<DeliveryRequest>, DomainError>
                .Fail(new DomainError(
                    "FILE_NOT_FOUND",
                    $"File was not found: {filePath}"));
        }
        catch (DirectoryNotFoundException)
        {
            return Result<IReadOnlyList<DeliveryRequest>, DomainError>
                .Fail(new DomainError(
                    "DIRECTORY_NOT_FOUND",
                    $"Directory was not found: {filePath}"));
        }
        catch (JsonException)
        {
            return Result<IReadOnlyList<DeliveryRequest>, DomainError>
                .Fail(new DomainError(
                    "INVALID_JSON",
                    "The JSON file contains malformed data."));
        }
        catch (IOException)
        {
            return Result<IReadOnlyList<DeliveryRequest>, DomainError>
                .Fail(new DomainError(
                    "FILE_ERROR",
                    $"Could not read file: {filePath}"));
        }
        catch (UnauthorizedAccessException)
        {
            return Result<IReadOnlyList<DeliveryRequest>, DomainError>
                .Fail(new DomainError(
                    "ACCESS_DENIED",
                    $"Access denied: {filePath}"));
        }
    }
}
