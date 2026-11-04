
namespace CosmeticsDeliverySystem.Functional;

public abstract record Maybe<T>
{
    private Maybe() { }

    public sealed record Some(T Value) : Maybe<T>;

    public sealed record None : Maybe<T>;

    public static Maybe<T> Found(T value)
    {
        return new Some(value);
    }

    public static Maybe<T> Missing()
    {
        return new None();
    }

    public TResult Match<TResult>(
        Func<T, TResult> onSome,
        Func<TResult> onNone)
    {
        ArgumentNullException.ThrowIfNull(onSome);
        ArgumentNullException.ThrowIfNull(onNone);

        return this switch
        {
            Some some => onSome(some.Value),
            None => onNone(),
            _ => throw new InvalidOperationException(
                "Unknown Maybe state.")
        };
    }
}
