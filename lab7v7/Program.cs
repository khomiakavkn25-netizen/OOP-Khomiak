using System;

namespace Lab7
{
    public class Notification
    {
        public string Message { get; set; }

        public Notification(string message)
        {
            Message = message;
        }

        public virtual void Send()
        {
            Console.WriteLine("Загальне сповіщення: " + Message);
        }
    }

    public class Email : Notification
    {
        public string RecipientEmail { get; set; }

        public Email(string message, string recipientEmail) : base(message)
        {
            RecipientEmail = recipientEmail;
        }

        public override void Send()
        {
            Console.WriteLine($"Надсилання Email на {RecipientEmail}: {Message}");
        }
    }

    public class SMS : Notification
    {
        public string PhoneNumber { get; set; }

        public SMS(string message, string phoneNumber) : base(message)
        {
            PhoneNumber = phoneNumber;
        }

        public new void Send()
        {
            Console.WriteLine($"Надсилання SMS на {PhoneNumber}: {Message}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Notification baseNotif = new Notification("Тестове повідомлення");
            Email emailNotif = new Email("Ваш код підтвердження: 1234", "user@gmail.com");
            SMS smsNotif = new SMS("Ваш баланс поповнено", "+380971112233");

            Console.WriteLine("--- Виклик через власні типи ---");
            baseNotif.Send();
            emailNotif.Send();
            smsNotif.Send();

            Console.WriteLine("\n--- Виклик через посилання базового класу (Notification) ---");
            Notification n1 = emailNotif;
            Notification n2 = smsNotif;

            n1.Send();
            n2.Send();

            Console.WriteLine("\n--- Виклик з явним приведенням типів ---");
            ((Email)n1).Send();
            ((SMS)n2).Send();

            Console.ReadLine();
        }
    }
}