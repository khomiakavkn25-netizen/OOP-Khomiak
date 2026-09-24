using System;
using System.Collections;
using System.Collections.Generic;

namespace Lab5Variant7
{
    public class StringQueue
    {
        private readonly List<string> _items;

        public StringQueue()
        {
            _items = new List<string>();
        }

        public StringQueue(IEnumerable<string> collection)
        {
            _items = new List<string>(collection ?? throw new ArgumentNullException(nameof(collection)));
        }

        public int Count => _items.Count;

        public string this[int index]
        {
            get
            {
                ValidateIndex(index);
                return _items[index];
            }
            set
            {
                ValidateIndex(index);
                _items[index] = value;
            }
        }

        public void Enqueue(string item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Елемент не може бути null.");

            _items.Add(item);
        }

        public string Dequeue()
        {
            if (_items.Count == 0)
                throw new InvalidOperationException("Черга порожня.");

            string item = _items[0];
            _items.RemoveAt(0);
            return item;
        }

        public static StringQueue operator +(StringQueue queue1, StringQueue queue2)
        {
            if (queue1 is null) throw new ArgumentNullException(nameof(queue1));
            if (queue2 is null) throw new ArgumentNullException(nameof(queue2));

            var result = new StringQueue();
            foreach (var item in queue1._items)
            {
                result.Enqueue(item);
            }
            foreach (var item in queue2._items)
            {
                result.Enqueue(item);
            }

            return result;
        }

        public static bool operator ==(StringQueue? a, StringQueue? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(StringQueue? a, StringQueue? b)
        {
            return !(a == b);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not StringQueue other) return false;
            if (ReferenceEquals(this, other)) return true;
            if (_items.Count != other._items.Count) return false;

            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] != other._items[i])
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (var item in _items)
            {
                hash.Add(item);
            }
            return hash.ToHashCode();
        }

        public override string ToString()
        {
            if (_items.Count == 0)
                return "Queue [Порожня]";

            return $"Queue [{string.Join(", ", _items)}]";
        }

        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= _items.Count)
            {
                throw new IndexOutOfRangeException($"Індекс {index} виходить за межі черги (розмір: {_items.Count}).");
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Лабораторна робота №5: StringQueue ===\n");

            StringQueue queue1 = new StringQueue();
            queue1.Enqueue("Перший");
            queue1.Enqueue("Другий");
            queue1.Enqueue("Третій");

            StringQueue queue2 = new StringQueue();
            queue2.Enqueue("Четвертий");
            queue2.Enqueue("П'ятий");

            Console.WriteLine($"Черга 1: {queue1} (Кількість: {queue1.Count})");
            Console.WriteLine($"Черга 2: {queue2} (Кількість: {queue2.Count})\n");

            Console.WriteLine("--- Доступ через індексатор (читання) ---");
            Console.WriteLine($"queue1[0]: {queue1[0]}");
            Console.WriteLine($"queue1[1]: {queue1[1]}");

            Console.WriteLine("\nЗміна значення queue1[1] = 'Оновлений Другий'...");
            queue1[1] = "Оновлений Другий";
            Console.WriteLine($"Оновлена Черга 1: {queue1}\n");

            Console.WriteLine("--- Перевантаження оператора + ---");
            StringQueue combinedQueue = queue1 + queue2;
            Console.WriteLine($"Результат (queue1 + queue2): {combinedQueue}");
            Console.WriteLine($"Кількість у новій черзі: {combinedQueue.Count}\n");

            Console.WriteLine("--- Перевантаження операторів == та != ---");

            StringQueue queue3 = new StringQueue();
            queue3.Enqueue("Перший");
            queue3.Enqueue("Оновлений Другий");
            queue3.Enqueue("Третій");

            Console.WriteLine($"Черга 1: {queue1}");
            Console.WriteLine($"Черга 3: {queue3}");

            Console.WriteLine($"queue1 == queue3 : {queue1 == queue3}");
            Console.WriteLine($"queue1 != queue2 : {queue1 != queue2}\n");

            Console.WriteLine("--- Демонстрація методів Enqueue / Dequeue ---");
            string dequeuedItem = queue1.Dequeue();
            Console.WriteLine($"Вилучено з queue1: '{dequeuedItem}'");
            Console.WriteLine($"Черга 1 після Dequeue(): {queue1}");
            Console.WriteLine($"queue1 == queue3 після вилучення: {queue1 == queue3}");
        }
    }
}