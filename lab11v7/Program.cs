using System;
using System.Collections.Generic;

namespace lab11v7;

abstract class EmailSender
{
    public string Recipient { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; protected set; } = string.Empty;
    public List<string> Attachments { get; } = new();
    public bool IsValid { get; protected set; }
    public bool IsSent { get; protected set; }
    public DateTime? SentAt { get; protected set; }

    public void Send()
    {
        Console.WriteLine($"--- {GetType().Name}: початок відправки ---");

        ValidateRecipient();
        if (!IsValid)
        {
            Console.WriteLine("Відправку скасовано: некоректний отримувач.");
            LogResult();
            return;
        }

        ComposeMessage();
        AttachFiles();
        SendViaServer();
        LogResult();
    }

    protected abstract void ComposeMessage();
    protected abstract void SendViaServer();

    protected virtual void ValidateRecipient()
    {
        IsValid = !string.IsNullOrWhiteSpace(Recipient)
                  && Recipient.Contains('@')
                  && Recipient.IndexOf('@') > 0
                  && Recipient.IndexOf('@') < Recipient.Length - 1;
        Console.WriteLine($"Перевірка отримувача '{Recipient}': {(IsValid ? "OK" : "помилка")}");
    }

    protected virtual void AttachFiles()
    {
        if (Attachments.Count == 0)
        {
            Console.WriteLine("Вкладень немає.");
            return;
        }
        foreach (var file in Attachments)
            Console.WriteLine($"Додано вкладення: {file}");
    }

    protected virtual void LogResult()
    {
        string status = IsSent ? $"успішно ({SentAt:HH:mm:ss})" : "не відправлено";
        Console.WriteLine($"[LOG] Лист для {Recipient}: {status}");
        Console.WriteLine();
    }
}

class GMailSender : EmailSender
{
    public string ApiToken { get; set; } = "gmail-token";

    protected override void ComposeMessage()
    {
        Body = $"[Gmail] Тема: {Subject}. Вітаємо!";
        Console.WriteLine("Складено повідомлення у форматі Gmail (HTML).");
    }

    protected override void SendViaServer()
    {
        Console.WriteLine($"Відправка через smtp.gmail.com з токеном {ApiToken}");
        IsSent = true;
        SentAt = DateTime.Now;
    }
}

class OutlookSender : EmailSender
{
    public string TenantId { get; set; } = "tenant-001";

    protected override void ComposeMessage()
    {
        Body = $"[Outlook] Тема: {Subject}. З повагою.";
        Console.WriteLine("Складено повідомлення у форматі Outlook.");
    }

    protected override void ValidateRecipient()
    {
        base.ValidateRecipient();
        if (IsValid && Recipient.EndsWith("@spam.com"))
        {
            IsValid = false;
            Console.WriteLine("Outlook: домен заблоковано політикою тенанта.");
        }
    }

    protected override void SendViaServer()
    {
        Console.WriteLine($"Відправка через Microsoft Graph (tenant {TenantId})");
        IsSent = true;
        SentAt = DateTime.Now;
    }
}

class CustomSMTPSender : EmailSender
{
    public string Host { get; set; } = "smtp.example.com";
    public int Port { get; set; } = 587;

    protected override void ComposeMessage()
    {
        Body = $"[SMTP] Тема: {Subject}. Звичайний текстовий лист.";
        Console.WriteLine("Складено простий текстовий лист.");
    }

    protected override void AttachFiles()
    {
        const int maxAttachments = 2;
        for (int i = 0; i < Attachments.Count; i++)
        {
            if (i >= maxAttachments)
            {
                Console.WriteLine($"Пропущено вкладення (ліміт {maxAttachments}): {Attachments[i]}");
                continue;
            }
            Console.WriteLine($"Додано вкладення: {Attachments[i]}");
        }
    }

    protected override void SendViaServer()
    {
        Console.WriteLine($"Відправка через {Host}:{Port}");
        IsSent = true;
        SentAt = DateTime.Now;
    }

    protected override void LogResult()
    {
        Console.WriteLine($"[SMTP LOG] Сервер {Host}:{Port}, відправлено: {IsSent}");
        base.LogResult();
    }
}

class Program
{
    static void Main()
    {
        var senders = new List<EmailSender>
        {
            new GMailSender
            {
                Recipient = "student@gmail.com",
                Subject = "Лабораторна робота"
            },
            new OutlookSender
            {
                Recipient = "teacher@college.edu.ua",
                Subject = "Звіт",
                Attachments = { "report.pdf" }
            },
            new CustomSMTPSender
            {
                Recipient = "info@example.com",
                Subject = "Новини",
                Attachments = { "a.txt", "b.txt", "c.txt" }
            },
            new GMailSender
            {
                Recipient = "invalid-email",
                Subject = "Помилковий отримувач"
            }
        };

        foreach (var sender in senders)
            sender.Send();
    }
}