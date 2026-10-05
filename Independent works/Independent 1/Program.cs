using System;

namespace IndependentWork1;

public class Rectangle
{
    private double _width;
    private double _height;

    public double Width
    {
        get { return _width; }
        set { _width = value; }
    }

    public double Height
    {
        get { return _height; }
    }

    public Rectangle(double width, double height)
    {
        _width = width;
        _height = height;
    }

    public double CalculateArea()
    {
        return _width * _height;
    }

    public bool IsSquare()
    {
        return _width == _height;
    }
}

public class Employee
{
    private string _fullName;
    private double _monthlySalary;

    public string FullName
    {
        get { return _fullName; }
        set { _fullName = value; }
    }

    public double MonthlySalary
    {
        get { return _monthlySalary; }
    }

    public Employee(string fullName, double monthlySalary)
    {
        _fullName = fullName;
        _monthlySalary = monthlySalary;
    }

    public double CalculateBonus(double percentage)
    {
        return _monthlySalary * (percentage / 100);
    }
}

public class Playlist
{
    private string _title;
    private int _trackCount;

    public string Title
    {
        get { return _title; }
        set { _title = value; }
    }

    public int TrackCount
    {
        get { return _trackCount; }
    }

    public Playlist(string title, int trackCount)
    {
        _title = title;
        _trackCount = trackCount;
    }

    public double GetTotalDuration(double avgTrackLength)
    {
        return _trackCount * avgTrackLength;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Rectangle rect = new Rectangle(5.0, 4.0);
        Console.WriteLine($"Прямокутник: {rect.Width} на {rect.Height}");
        Console.WriteLine($"Площа: {rect.CalculateArea()}");
        Console.WriteLine($"Квадрат: {rect.IsSquare()}");
        Console.WriteLine();

        Employee emp = new Employee("Іван Петренко", 25000);
        Console.WriteLine($"Працівник: {emp.FullName}, Зарплата: {emp.MonthlySalary} грн");
        Console.WriteLine($"Премія (15%): {emp.CalculateBonus(15)} грн");
        Console.WriteLine();

        Playlist playlist = new Playlist("Спортивний мікс", 12);
        Console.WriteLine($"Плейлист: {playlist.Title}, треків: {playlist.TrackCount}");
        Console.WriteLine($"Приблизний час: {playlist.GetTotalDuration(3.5)} хв");
    }
}