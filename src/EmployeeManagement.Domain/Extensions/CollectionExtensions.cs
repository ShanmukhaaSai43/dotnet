namespace EmployeeManagement.Domain.Extensions;

/// <summary>
/// Custom LINQ extension method demonstrating deferred execution using 'yield return'.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Custom filter demonstrating lazy/deferred evaluation.
    /// Elements are evaluated one-by-one only when the consumer iterates over the returned IEnumerable.
    /// </summary>
    public static IEnumerable<T> WhereLazy<T>(this IEnumerable<T> source, Func<T, bool> predicate, Action<T>? onInspected = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        foreach (var item in source)
        {
            onInspected?.Invoke(item);
            if (predicate(item))
            {
                yield return item; // Yield return provides lazy evaluation
            }
        }
    }
}
