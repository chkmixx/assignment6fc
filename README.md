# CosmeticsDeliverySystem

# 1. Project Overview

Cosmetics Delivery System is a console application developed in C# using .NET 8.0.

The application manages cosmetic delivery orders, validates customer requests, searches for products, calculates delivery costs, and handles business errors.

The main goal of the project is to demonstrate Functional Programming concepts in C# through a practical delivery management system.

# 2. Technologies Used

- C#
- .NET 8.0
- LINQ
- JSON
- xUnit
- Functional Programming

# 3. Project Structure

The project is divided into the following folders:
 Data
- `SampleData.cs` – provides sample delivery orders.
- `JsonDataLoader.cs` – loads order information from JSON files.
- `orders.json` – contains sample order data.
- 
 Functional
- `DeliveryPipeline.cs` – processes orders through multiple functional stages.
- `DeliveryPolicies.cs` – defines Standard and Express delivery policies.
- `DeliveryWorkflow.cs` – combines product lookup, validation, and quote creation.
- `Maybe.cs` – represents optional values.
- `Result.cs` – represents successful and failed operations.
- `ResultExtensions.cs` – implements Map, Bind, and ToResult operations.

 Models
- `CosmeticProduct.cs` – represents a cosmetic product.
- `DeliveryQuote.cs` – stores calculated delivery information.
- `DeliveryRequest.cs` – represents a customer delivery request.
- `DomainError.cs` – represents structured business errors.

 Services
- `DeliveryCalculator.cs` – calculates delivery costs.
- `DeliveryQuoteBuilder.cs` – creates delivery quotations.
- `OrderValidator.cs` – validates delivery requests.
- `ProductRepository.cs` – searches for cosmetic products.

 Tests
- `CosmeticsDeliverySystem.Tests` – contains automated unit tests using xUnit.

 4. Main Features

- Loading orders from JSON files.
- Validating customer delivery requests.
- Searching for cosmetic products.
- Calculating Standard Delivery costs.
- Calculating Express Delivery costs.
- Sorting delivery quotations by total amount.
- Handling missing products and invalid quantities.
- Returning structured success and failure results.

 5. Functional Programming Concepts

### Pure Functions
Functions that produce predictable outputs without modifying external state.

### Higher-Order Functions
Functions that accept other functions as parameters or return functions.

### Function Composition
Combining smaller functional operations into a larger workflow.

### Maybe
Represents an optional value using `Some` and `None`.

### Result
Represents either a successful operation or a failed operation using `Success` and `Failure`.

### Map
Transforms a successful value while preserving the Result structure.

### Bind
Composes operations that return Result values and propagates errors without executing subsequent successful operations.

### Immutability
Uses immutable records and avoids unnecessary modification of caller-owned data.

## 6. Delivery Policies
The application supports two delivery policies:

Standard Delivery – calculates the regular delivery cost.

Express Delivery – calculates a higher delivery cost for express service.

Both policies can process the same order collection and produce separate quotations.

7. Error Handling

The system handles the following situations:

Product not found.

Invalid quantity.

Insufficient stock.

Invalid delivery request.

Missing JSON file.

Malformed JSON content.

Business errors are represented through DomainError and returned using the Result type.

8. Automated Testing

The project uses xUnit for automated testing.

Current test results:

Metric

Result

Total tests

26

Passed

26

Failed

0

Tests cover validation, delivery calculations, empty datasets, Result, Maybe, Map, Bind, product lookup, workflow errors, and JSON loading.



9. Conclusion

Cosmetics Delivery System demonstrates the practical application of Functional Programming principles in C#.

The project separates models, services, data access, and functional workflows to improve code organization, maintainability, error handling, and testability.
