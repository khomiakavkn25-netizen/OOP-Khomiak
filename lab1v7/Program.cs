using System;

namespace PhoneLab
{
    
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== 1. Створення об'єкта та виклик методів ===");
            Phone myPhone = new Phone("Apple", "iPhone 15", 85);

            myPhone.Call("+380971234567");

            Console.WriteLine("\n=== 2. Демонстрація інкапсуляції (перевірка межі заряду) ===");

            myPhone.BatteryLevel = 150; 
            Console.WriteLine($"Заряд після спроби встановити 150%: {myPhone.BatteryLevel}%");

            myPhone.BatteryLevel = -20;
            Console.WriteLine($"Заряд після спроби встановити -20%: {myPhone.BatteryLevel}%");

            Console.WriteLine("\n=== 3. Демонстрація роботи деструктора ===");
            CreateTemporaryObject();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("\nПрограму завершено.");
        }

        static void CreateTemporaryObject()
        {
            Phone tempPhone = new Phone("Samsung", "Galaxy S24", 50);
            tempPhone.Call("+380507654321");
        }
    }

    public class Phone
    {
        private string brand;
        private string model;
        private int batteryLevel;

        public int BatteryLevel
        {
            get => batteryLevel;
            set
            {
                if (value < 0) batteryLevel = 0;
                else if (value > 100) batteryLevel = 100;
                else batteryLevel = value;
            }
        }

        public string Brand => brand;
        public string Model => model;

        public Phone()
        {
            brand = "Невідомо";
            model = "Невідомо";
            BatteryLevel = 100;
            Console.WriteLine("[Конструктор]: Створено базовий об'єкт Phone.");
        }

        public Phone(string brand, string model, int initialBattery)
        {
            this.brand = brand;
            this.model = model;
            BatteryLevel = initialBattery;
            Console.WriteLine($"[Конструктор]: Створено об'єкт {brand} {model}.");
        }

        ~Phone()
        {
            Console.WriteLine($"[Деструктор]: Об'єкт {brand} {model} видалено з пам'яті.");
        }

        public void Call(string number)
        {
            if (BatteryLevel <= 0)
            {
                Console.WriteLine($"Неможливо зателефонувати на {number}: телефон розряджений.");
                return;
            }

            Console.WriteLine($"Дзвінок на {number} з пристрою {brand} {model}...");
            BatteryLevel -= 10;
            Console.WriteLine($"Залишок батареї: {BatteryLevel}%");
        }
    }
}