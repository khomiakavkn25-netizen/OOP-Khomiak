using System;

namespace Lab2
{
    public class Phone
    {
        private string _brand;
        private string _model;
        private int _batteryLevel;

        public string Brand
        {
            get { return _brand; }
            set { _brand = value; }
        }

        public string Model
        {
            get { return _model; }
            set { _model = value; }
        }

        public int BatteryLevel
        {
            get { return _batteryLevel; }
            set
            {
                if (value < 0) _batteryLevel = 0;
                else if (value > 100) _batteryLevel = 100;
                else _batteryLevel = value;
            }
        }

        public Phone(string brand, string model, int batteryLevel)
        {
            Brand = brand;
            Model = model;
            BatteryLevel = batteryLevel;
            Console.WriteLine($"[Конструктор] Створено телефон {Brand} {Model}.");
        }

        public Phone() : this("Generic", "Basic", 50)
        {
            Console.WriteLine($"[Конструктор] Викликано конструктор за замовчуванням.");
        }

        ~Phone()
        {
            Console.WriteLine($"[Деструктор] Об'єкт {Brand} {Model} знищується.");
        }

        public void Call(string number)
        {
            if (BatteryLevel > 0)
            {
                Console.WriteLine($"[{Brand} {Model}] Дзвінок на номер {number}...");
                BatteryLevel -= 5;
                Console.WriteLine($"   Залишок заряду: {BatteryLevel}%");
            }
            else
            {
                Console.WriteLine($"[{Brand} {Model}] Неможливо здійснити дзвінок. Батарея розряджена!");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("--- Creating objects ---");
            
            Phone phone1 = new Phone();
            Phone phone2 = new Phone("Samsung", "Galaxy S23", 100);
            Phone phone3 = new Phone("Apple", "iPhone 13", 3);

            Console.WriteLine("\n--- Objects created. Using them ---");
            phone1.Call("+380111111111");
            phone2.Call("+380222222222");
            
            phone3.Call("+380333333333");
            phone3.Call("+380333333333");

            Console.WriteLine("\n--- End of Main, preparing for GC ---");
            
            phone1 = null;
            phone2 = null;
            phone3 = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            Console.WriteLine("Програма завершила роботу.");
        }
    }
}