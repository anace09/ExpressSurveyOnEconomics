public static class EnumerableExtensions
{
    private static readonly Random _rnd = Random.Shared;

    public static List<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(_ => _rnd.Next()).ToList();
    }
}
