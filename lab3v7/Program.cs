using System;

namespace lab3v7
{
    public class SerialPortConnection : IDisposable
    {
        private string _portName;
        private bool _isPortOpen;
        private bool _disposed = false;

        public string PortName => _portName;
        public bool IsPortOpen => _isPortOpen;

        public SerialPortConnection(string portName)
        {
            _portName = portName;
            _isPortOpen = true;
            Console.WriteLine($"[SerialPortConnection] Порт {_portName} відкрито.");
        }

        public void SendData(byte[] data)
        {
            if (_disposed || !_isPortOpen)
            {
                throw new InvalidOperationException($"Помилка: порт {_portName} закритий.");
            }

            if (data == null || data.Length == 0)
            {
                Console.WriteLine($"[SerialPortConnection] Спроба відправити порожні дані.");
                return;
            }

            Console.WriteLine($"[SerialPortConnection] Передано дані ({data.Length} байт): {BitConverter.ToString(data)}");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_isPortOpen)
                    {
                        _isPortOpen = false;
                        Console.WriteLine($"[SerialPortConnection] Порт {_portName} закрито.");
                    }
                }
                _disposed = true;
            }
        }

        ~SerialPortConnection()
        {
            Dispose(false);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== 1. Демонстрація з using ===");
            using (var conn1 = new SerialPortConnection("COM1"))
            {
                conn1.SendData(new byte[] { 0x01, 0x02 });
            }

            Console.WriteLine("\n=== 2. Демонстрація з явним викликом Dispose() ===");
            var conn2 = new SerialPortConnection("COM2");
            conn2.SendData(new byte[] { 0xAA, 0xBB });
            conn2.Dispose();

            Console.WriteLine("\n=== 3. Демонстрація роботи деструктора (GC) ===");
            CreateUnreferencedObject();

            // Примусовий виклик збирача сміття для демонстрації деструктора
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("\nРоботу програми завершено.");
        }

        static void CreateUnreferencedObject()
        {
            var conn3 = new SerialPortConnection("COM3");
            conn3.SendData(new byte[] { 0xFF });
            // Спеціально не викликаємо Dispose()
        }
    }
}