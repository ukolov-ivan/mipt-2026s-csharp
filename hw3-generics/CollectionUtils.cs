public static class CollectionUtils
{
    public static List<T> Distinct<T>(List<T> source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        var result = new List<T>();
        var seen = new HashSet<T>();

        for (int i = 0; i < source.Count; i++)
        {
            var item = source[i];
            if (seen.Add(item))
            {
                result.Add(item);
            }
        }

        return result;
    }

    public static Dictionary<TKey, List<TValue>> GroupBy<TValue, TKey>(
        List<TValue> source,
        Func<TValue, TKey> keySelector) where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(keySelector, nameof(keySelector));

        var result = new Dictionary<TKey, List<TValue>>();

        foreach (var item in source)
        {
            var key = keySelector(item);
            if (!result.TryGetValue(key, out var list))
            {
                list = [];
                result.Add(key, list); // It's new
            }
            list.Add(item);
        }

        return result;
    }

    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(
        Dictionary<TKey, TValue> first,
        Dictionary<TKey, TValue> second,
        Func<TValue, TValue, TValue> conflictResolver) where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(first, nameof(first));
        ArgumentNullException.ThrowIfNull(second, nameof(second));
        ArgumentNullException.ThrowIfNull(conflictResolver, nameof(conflictResolver));

        // Assuming first.Comparer == second.Comparer
        var result = new Dictionary<TKey, TValue>(first.Count + second.Count, first.Comparer);

        foreach (var pair in first)
        {
            result.Add(pair.Key, pair.Value);
        }

        foreach (var pair in second)
        {
            if (result.TryGetValue(pair.Key, out var existingValue))
            {
                result[pair.Key] = conflictResolver(existingValue, pair.Value);
            }
            else
            {
                result.Add(pair.Key, pair.Value); // It's new
            }
        }

        return result;
    }

    public static T MaxBy<T, TKey>(List<T> source, Func<T, TKey> selector)
        where TKey : IComparable<TKey>
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(selector, nameof(selector));

        if (source.Count == 0)
            throw new InvalidOperationException("Empty collection");

        T max = source[0];
        TKey maxKey = selector(max);

        var comparer = Comparer<TKey>.Default;
        for (int i = 1; i < source.Count; i++)
        {
            var item = source[i];
            var key = selector(item);
            if (comparer.Compare(key, maxKey) > 0) {
                maxKey = key;
                max = item;
            }
        }

        return max;
    }
}

public record Product(string Name, decimal Price);

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Distinct integers:");
        var ints = new List<int> { 10, 5, 10, 15, 20, 5, 30, 35 };
        var distinctInts = CollectionUtils.Distinct(ints);
        Console.WriteLine(string.Join(", ", distinctInts));

        Console.WriteLine("Distinct words:");
        var words = new List<string> { "cat", "dog", "elephant", "cat", "tiger", "dog", "fox" };
        var distinctWords = CollectionUtils.Distinct(words);
        Console.WriteLine(string.Join(", ", distinctWords));

        Console.WriteLine("Words grouped by length:");
        var grouped = CollectionUtils.GroupBy(words, word => word.Length);
        foreach (var bucket in grouped)
        {
            Console.WriteLine($"{bucket.Key}: [{string.Join(", ", bucket.Value)}]");
        }

        Console.WriteLine("Merge word counts (resolver - sum)");
        var first = new Dictionary<string, int>
        {
            ["hello"] = 5,
            ["world"] = 2,
            ["code"] = 3
        };
        var second = new Dictionary<string, int>
        {
            ["world"] = 4,
            ["code"] = 1,
            ["csharp"] = 7
        };
        
        var merged = CollectionUtils.Merge(first, second, (left, right) => left + right);
        foreach (var kv in merged)
        {
            Console.WriteLine($"{kv.Key}: {kv.Value}");
        }

        Console.WriteLine("Most expensive product:");
        var products = new List<Product>
        {
            new("Bicycle", 450m),
            new("Car", 25000m),
            new("Motorcycle", 5500m),
            new("Skateboard", 120m)
        };
        
        Console.WriteLine($"{CollectionUtils.MaxBy(products, product => product.Price)}");
    }
}