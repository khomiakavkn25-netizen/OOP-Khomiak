using System;
using System.Collections.Generic;

namespace Lab8v7
{
    public class Notification
    {
        public string Message { get; set; }

        public string SentLog { get; protected set; } = string.Empty;

        public Notification(string message)
        {
            Message = message;
        }

        public virtual void Send()
        {
            SentLog = $"[Загальне] {Message}";
            Console.WriteLine(SentLog);
        }
    }

    public class EmailNotification : Notification
    {
        public string RecipientEmail { get; set; }

        public EmailNotification(string message, string recipientEmail)
            : base(message)
        {
            RecipientEmail = recipientEmail;
        }

        public override void Send()
        {
            SentLog = $"[Email] Кому: {RecipientEmail} | Текст: {Message}";
            Console.WriteLine(SentLog);
        }
    }

    public class SMSNotification : Notification
    {
        public string PhoneNumber { get; set; }

        public SMSNotification(string message, string phoneNumber)
            : base(message)
        {
            PhoneNumber = phoneNumber;
        }

        public override void Send()
        {
            SentLog = $"[SMS] Номер: {PhoneNumber} | Текст: {Message}";
            Console.WriteLine(SentLog);
        }
    }

    public class PushNotification : Notification
    {
        public string DeviceId { get; set; }

        public PushNotification(string message, string deviceId)
            : base(message)
        {
            DeviceId = deviceId;
        }

        public override void Send()
        {
            SentLog = $"[Push] Пристрій: {DeviceId} | Текст: {Message}";
            Console.WriteLine(SentLog);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            List<Notification> notifications = new List<Notification>
            {
                new EmailNotification("Ваше замовлення підтверджено", "ivan@example.com"),
                new SMSNotification("Код підтвердження: 4821", "+380501234567"),
                new PushNotification("Нове повідомлення в чаті", "device-9F3A21"),
                new EmailNotification("Знижка 20% на всі товари", "olena@example.com"),
                new SMSNotification("Ваш кур'єр прибуде за 10 хв", "+380671112233")
            };

            Console.WriteLine("=== Відправлення повідомлень ===");
            foreach (Notification n in notifications)
            {
                n.Send();
            }

            List<string> sentMessages = new List<string>();
            foreach (Notification n in notifications)
            {
                sentMessages.Add(n.SentLog);
            }

            Console.WriteLine();
            Console.WriteLine("=== Усі відправлені повідомлення ===");
            for (int i = 0; i < sentMessages.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {sentMessages[i]}");
            }

            Console.WriteLine();
            Console.WriteLine($"Всього відправлено: {sentMessages.Count}");
        }
    }
}