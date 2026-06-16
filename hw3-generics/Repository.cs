public interface IEntity
{
    int Id { get; }
}

public interface IRepository<T> where T : class, IEntity
{
    int Count { get; }
    void Add(T item);
    bool Remove(int id);
    T? GetById(int id);
    // Maybe returning IEnumerable would be a better API? 
    IReadOnlyList<T> GetAll();
    // Signature change shoud still respect covariance with Predicate
    IReadOnlyList<T> Find(Func<T, bool> predicate);
}

public class Repository<T> : IRepository<T> where T : class, IEntity
{
    // Avoided ConcurrentDictionary assuming Repository is used only as a singleton
    private readonly Dictionary<int, T> _items = [];

    public int Count => _items.Count;

    public void Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));

        if (!_items.TryAdd(item.Id, item))
            throw new InvalidOperationException($"Duplicate id");

    }

    public bool Remove(int id)
    {
        return _items.Remove(id);
    }

    public T? GetById(int id)
    {
        return _items.GetValueOrDefault(id);
    }

    public IReadOnlyList<T> GetAll()
    {
        return _items.Values.ToList();
    }

    public IReadOnlyList<T> Find(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

        return _items.Values.Where(predicate).ToList();
    }
}

public record Product(int Id, string Name, decimal Price) : IEntity;
public record User(int Id, string Name) : IEntity;

public static class Program
{
    public static void Main()
    {
        var products = new Repository<Product>();

        Console.WriteLine($"Empty repository products count: {products.Count}.");

        products.Add(new Product(101, "Espresso Machine", 850));
        products.Add(new Product(102, "Coffee Grinder", 120));
        products.Add(new Product(103, "Commercial Blender", 1100));

        Console.WriteLine($"Products count: {products.Count}.");

        var foundProductId = 101;
        var product = products.GetById(foundProductId);
        Console.WriteLine(product is not null
            ? $"Product {product.Name} (id={foundProductId}, price={product.Price}) found. PASS."
            : $"Product (id = {foundProductId}) not found. FAIL.");

        var missingProductId = 999;
        var missingProduct = products.GetById(missingProductId);
        Console.WriteLine(missingProduct is null
            ? $"Product (id = {missingProductId}) not found. PASS."
            : $"Product {missingProduct.Name} (id = {missingProductId}) found. FAIL.");

        var expensiveProducts = products.Find(product => product.Price > 1000);
        Console.WriteLine("Expensive products:");
        foreach (var expensiveProduct in expensiveProducts)
        {
            Console.WriteLine($"* {expensiveProduct.Name}");
        }

        var duplicateProductID = foundProductId;
        try
        {
            products.Add(new Product(duplicateProductID, "Upgraded Espresso Machine", 950));
            Console.WriteLine($"Added product (id = {duplicateProductID}). FAIL.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Failed to add product (id = {duplicateProductID}): {ex.Message}. PASS.");
        }

        var removedProductId = 102;
        if (products.Remove(removedProductId))
        {
            Console.WriteLine($"Product (id = {removedProductId}) removed. PASS.");
        }
        else
        {
            Console.WriteLine($"Product (id = {removedProductId}) not found. FAIL.");
        }

        Console.WriteLine($"Products count: {products.Count}");

        var users = new Repository<User>();

        users.Add(new User(7, "Sarah"));
        users.Add(new User(8, "David"));
        users.Add(new User(9, "Elena"));

        Console.WriteLine("Users:");
        foreach (var user in users.GetAll())
        {
            Console.WriteLine($"* {user.Name} (Id={user.Id})");
        }
    }
}
