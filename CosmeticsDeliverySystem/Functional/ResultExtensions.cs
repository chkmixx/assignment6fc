
namespace CosmeticsDeliverySystem.Functional;

public static class ResultExtensions
{
    public static Result<U, E> Map<T, U, E>(
        this Result<T, E> result,
        Func<T, U> mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);

        return result switch
        {
            Result<T, E>.Success success =>
                Result<U, E>.Ok(mapper(success.Value)),

            Result<T, E>.Failure failure =>
                Result<U, E>.Fail(failure.Error),

            _ => throw new InvalidOperationException()
        };
    }

    public static Result<U, E> Bind<T, U, E>(
        this Result<T, E> result,
        Func<T, Result<U, E>> binder)
    {
        ArgumentNullException.ThrowIfNull(binder);

        return result switch
        {
            Result<T, E>.Success success =>
                binder(success.Value),

            Result<T, E>.Failure failure =>
                Result<U, E>.Fail(failure.Error),

            _ => throw new InvalidOperationException()
        };
    }

    public static Result<T, E> ToResult<T, E>(
        this Maybe<T> maybe,
        E error)
    {
        return maybe switch
        {
            Maybe<T>.Some some =>
                Result<T, E>.Ok(some.Value),

            Maybe<T>.None =>
                Result<T, E>.Fail(error),

            _ => throw new InvalidOperationException()
        };
    }
}
