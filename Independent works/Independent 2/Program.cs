using System;

namespace IndependentWork2;

public class Product
{
    private int _id;
    private string _name;
    private decimal _price;
    private string _category;
    private int _stockCount;

    public int Id
    {
        get { return _id; }
    }

    public string Name
    {
        get { return _name; }
    }

    public decimal Price
    {
        get { return _price; }
    }

    public string Category
    {
        get { return _category; }
    }

    public int StockCount
    {
        get { return _stockCount; }
    }

    public Product(int id, string name, decimal price, string category, int stockCount)
    {
        _id = id;
        _name = name;
        _price = price;
        _category = category;
        _stockCount = stockCount;
    }

    public Product(int id, string name, decimal price) 
        : this(id, name, price, "Uncategorized", 0)
    {
    }

    public Product(Product other) 
        : this(other._id, other._name, other._price, other._category, other._stockCount)
    {
    }

    public override string ToString()
    {
        return $"ID: {Id}, Name: {Name}, Price: {Price:C}, Category: {Category}, Stock: {StockCount}";
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("Створення товарів");

        Product p1 = new Product(101, "Laptop", 35000.00m, "Electronics", 15);
        Console.WriteLine($"Товар 1 (основний конструктор): {p1}");

        Product p2 = new Product(102, "Mouse", 800.00m);
        Console.WriteLine($"Товар 2 (скорочений конструктор): {p2}");

        Product p3 = new Product(p1);
        Console.WriteLine($"Товар 3 (конструктор копіювання): {p3}");
    }
}