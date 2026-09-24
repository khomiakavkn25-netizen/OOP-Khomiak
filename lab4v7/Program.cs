using System;

namespace Lab4Variant7
{
    public class ComplexNumber
    {
        private double _real;
        private double _imaginary;

        public static ComplexNumber I => new ComplexNumber(0, 1);

        public ComplexNumber(double real = 0, double imaginary = 0)
        {
            Real = real;
            Imaginary = imaginary;
        }

        public double Real
        {
            get => _real;
            set
            {
                if (double.IsNaN(value) || double.IsInfinity(value))
                {
                    throw new ArgumentException("Дійсна частина повинна бути скінченним числом.");
                }
                _real = value;
            }
        }

        public double Imaginary
        {
            get => _imaginary;
            set
            {
                if (double.IsNaN(value) || double.IsInfinity(value))
                {
                    throw new ArgumentException("Уявна частина повинна бути скінченним числом.");
                }
                _imaginary = value;
            }
        }

        public double this[int index]
        {
            get
            {
                return index switch
                {
                    0 => Real,
                    1 => Imaginary,
                    _ => throw new IndexOutOfRangeException("Некоректний індекс. Використовуйте 0 для Real або 1 для Imaginary.")
                };
            }
            set
            {
                switch (index)
                {
                    case 0:
                        Real = value;
                        break;
                    case 1:
                        Imaginary = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Некоректний індекс. Використовуйте 0 для Real або 1 для Imaginary.");
                }
            }
        }

        public static ComplexNumber operator +(ComplexNumber a, ComplexNumber b)
        {
            if (a is null || b is null) throw new ArgumentNullException();
            return new ComplexNumber(a.Real + b.Real, a.Imaginary + b.Imaginary);
        }

        public static ComplexNumber operator *(ComplexNumber a, ComplexNumber b)
        {
            if (a is null || b is null) throw new ArgumentNullException();
            double realPart = a.Real * b.Real - a.Imaginary * b.Imaginary;
            double imaginaryPart = a.Real * b.Imaginary + a.Imaginary * b.Real;
            return new ComplexNumber(realPart, imaginaryPart);
        }

        public static bool operator ==(ComplexNumber left, ComplexNumber right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(ComplexNumber left, ComplexNumber right) => !(left == right);

        public override bool Equals(object? obj)
        {
            if (obj is ComplexNumber other)
            {
                const double tolerance = 1e-9;
                return Math.Abs(Real - other.Real) < tolerance && 
                       Math.Abs(Imaginary - other.Imaginary) < tolerance;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Math.Round(Real, 9), Math.Round(Imaginary, 9));
        }

        public override string ToString()
        {
            if (Imaginary == 0) return $"{Real}";
            if (Real == 0) return $"{Imaginary}i";
            
            string sign = Imaginary > 0 ? "+" : "-";
            return $"{Real} {sign} {Math.Abs(Imaginary)}i";
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== ДЕМОНСТРАЦІЯ РОБОТИ КЛАСУ ComplexNumber ===\n");

            var c1 = new ComplexNumber(3, 4);
            var c2 = new ComplexNumber(1, -2);
            var c3 = new ComplexNumber(3, 4);

            Console.WriteLine($"c1 = {c1}");
            Console.WriteLine($"c2 = {c2}");
            Console.WriteLine($"c3 = {c3}");

            Console.WriteLine($"\nСтатична уявна одиниця ComplexNumber.I = {ComplexNumber.I}");

            Console.WriteLine("\n--- Математичні операції ---");
            ComplexNumber sum = c1 + c2;
            Console.WriteLine($"c1 + c2 = {sum}");

            ComplexNumber product = c1 * c2;
            Console.WriteLine($"c1 * c2 = {product}");

            ComplexNumber iSquared = ComplexNumber.I * ComplexNumber.I;
            Console.WriteLine($"I * I = {iSquared} (очікується -1)");

            Console.WriteLine("\n--- Порівняння об'єктів ---");
            Console.WriteLine($"c1 == c3: {c1 == c3}");
            Console.WriteLine($"c1 == c2: {c1 == c2}");
            Console.WriteLine($"c1.Equals(c3): {c1.Equals(c3)}");

            Console.WriteLine("\n--- Використання індексатора ---");
            Console.WriteLine($"c1[0] (Real): {c1[0]}");
            Console.WriteLine($"c1[1] (Imaginary): {c1[1]}");

            c1[0] = 5;
            c1[1] = 12;
            Console.WriteLine($"Модифікований c1 через індексатор: {c1}");

            Console.WriteLine("\n--- Перевірка валідації ---");
            try
            {
                c1.Real = double.NaN;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Перехоплено виключення валідації: {ex.Message}");
            }
        }
    }
}