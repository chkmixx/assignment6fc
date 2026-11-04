
namespace CosmeticsDeliverySystem.Functional;

public abstract record Result<T, E>
{
    private Result() { }

    public sealed record Success(T Value) : Result<T, E>;

    public sealed record Failure(E Error) : Result<T, E>;

    public static Result<T, E> Ok(T value)
    {
        return new Success(value);
    }

    public static Result<T, E> Fail(E error)
    {
        return new Failure(error);
    }

    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<E, TResult> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        return this switch
        {
            Success success => onSuccess(success.Value),
            Failure failure => onFailure(failure.Error),
            _ => throw new InvalidOperationException(
                "Unknown result state.")
        };
    }
}
