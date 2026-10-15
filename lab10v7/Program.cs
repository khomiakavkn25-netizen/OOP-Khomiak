using System;
using System.Collections.Generic;

namespace Lab10v7
{
    public interface INotifier
    {
        void SendNotification(string message);
    }

    public class EmailNotifier : INotifier
    {
        public void SendNotification(string message)
        {
            Console.WriteLine($"[Email] Надіслано лист: {message}");
        }
    }

    public class SmsNotifier : INotifier
    {
        public void SendNotification(string message)
        {
            Console.WriteLine($"[SMS] Надіслано повідомлення: {message}");
        }
    }

    public abstract class Authenticator
    {
        public abstract bool Authenticate(string username, string password);

        public void LogAttempt(string username, bool success)
        {
            string status = success ? "успішно" : "невдало";
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Спроба входу користувача '{username}': {status}");
        }
    }

    public class DatabaseAuthenticator : Authenticator
    {
        private readonly Dictionary<string, string> _users = new()
        {
            { "admin", "admin123" },
            { "student", "qwerty" }
        };

        public override bool Authenticate(string username, string password)
        {
            Console.WriteLine("Перевірка через базу даних...");
            bool result = _users.TryGetValue(username, out var stored) && stored == password;
            LogAttempt(username, result);
            return result;
        }
    }

    public class LdapAuthenticator : Authenticator
    {
        private const string Domain = "corp.local";

        public override bool Authenticate(string username, string password)
        {
            Console.WriteLine($"Перевірка через LDAP ({Domain})...");
            bool result = !string.IsNullOrWhiteSpace(username) && password.Length >= 8;
            LogAttempt($"{username}@{Domain}", result);
            return result;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Колекція INotifier ===");
            List<INotifier> notifiers = new()
            {
                new EmailNotifier(),
                new SmsNotifier()
            };

            foreach (INotifier notifier in notifiers)
            {
                notifier.SendNotification("Вітаємо у системі!");
            }

            Console.WriteLine();
            Console.WriteLine("=== Колекція Authenticator ===");
            List<Authenticator> authenticators = new()
            {
                new DatabaseAuthenticator(),
                new LdapAuthenticator()
            };

            foreach (Authenticator auth in authenticators)
            {
                bool ok1 = auth.Authenticate("admin", "admin123");
                Console.WriteLine($"Результат: {ok1}\n");

                bool ok2 = auth.Authenticate("guest", "123");
                Console.WriteLine($"Результат: {ok2}\n");
            }
        }
    }
}