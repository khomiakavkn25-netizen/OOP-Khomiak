using System.Diagnostics;
using System.Text;

namespace lab14v7;

public record MailMessage(string To, string Subject);

public record MailingRecord(string Subject, int Recipients, DateTime SentAt);

public class SubscriptionSystem
{
    private readonly HashSet<string> _subscribers = new(StringComparer.OrdinalIgnoreCase);
    private readonly Queue<MailMessage> _mailQueue = new();
    private readonly Stack<MailingRecord> _history = new();

    public int SubscribersCount => _subscribers.Count;
    public int QueueCount => _mailQueue.Count;
    public int HistoryCount => _history.Count;

    public bool Subscribe(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return false;
        return _subscribers.Add(email.Trim());
    }

    public bool Unsubscribe(string email) => _subscribers.Remove(email.Trim());

    public bool IsSubscribed(string email) => _subscribers.Contains(email.Trim());

    public IEnumerable<string> GetSubscribers() => _subscribers;

    public int ImportSubscribers(IEnumerable<string> emails)
    {
        int before = _subscribers.Count;
        foreach (var e in emails)
            Subscribe(e);
        return _subscribers.Count - before;
    }

    public void EnqueueMailing(string subject)
    {
        foreach (var email in _subscribers)
            _mailQueue.Enqueue(new MailMessage(email, subject));
    }

    public MailMessage? PeekNext() => _mailQueue.Count > 0 ? _mailQueue.Peek() : null;

    public int ProcessQueue(int? limit = null)
    {
        int sent = 0;
        string? lastSubject = null;

        while (_mailQueue.Count > 0 && (limit == null || sent < limit))
        {
            var msg = _mailQueue.Dequeue();
            Console.WriteLine($"   Відправлено: {msg.To} <- \"{msg.Subject}\"");
            lastSubject = msg.Subject;
            sent++;
        }

        if (sent > 0 && lastSubject != null)
            _history.Push(new MailingRecord(lastSubject, sent, DateTime.Now));

        return sent;
    }

    public MailingRecord? PeekLastMailing() => _history.Count > 0 ? _history.Peek() : null;

    public MailingRecord? UndoLastMailing() => _history.Count > 0 ? _history.Pop() : null;

    public IEnumerable<MailingRecord> GetHistory() => _history;
}

public class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        var system = new SubscriptionSystem();

        Console.WriteLine("=== 1. HashSet: унікальні підписники ===");
        Console.WriteLine($"Підписати anna@mail.com: {system.Subscribe("anna@mail.com")}");
        Console.WriteLine($"Підписати ivan@mail.com: {system.Subscribe("ivan@mail.com")}");
        Console.WriteLine($"Підписати anna@mail.com повторно: {system.Subscribe("anna@mail.com")} (дублікат)");
        Console.WriteLine($"Підписати ANNA@MAIL.COM (інший регістр): {system.Subscribe("ANNA@MAIL.COM")} (дублікат)");
        Console.WriteLine($"Підписати 'не-email': {system.Subscribe("не-email")} (невалідний)");

        int added = system.ImportSubscribers(new[]
        {
            "olena@mail.com", "petro@mail.com", "ivan@mail.com", "olena@mail.com", "maria@mail.com"
        });
        Console.WriteLine($"Імпорт зі списку з дублікатами: додано нових = {added}");
        Console.WriteLine($"Усього підписників: {system.SubscribersCount}");
        Console.WriteLine($"Чи підписаний petro@mail.com? {system.IsSubscribed("petro@mail.com")}");
        Console.WriteLine($"Відписати petro@mail.com: {system.Unsubscribe("petro@mail.com")}");
        Console.WriteLine($"Чи підписаний petro@mail.com? {system.IsSubscribed("petro@mail.com")}");
        Console.WriteLine("Поточні підписники: " + string.Join(", ", system.GetSubscribers()));

        Console.WriteLine("\n=== 2. Queue: черга email для розсилки ===");
        system.EnqueueMailing("Новини тижня");
        Console.WriteLine($"У чергу додано листів: {system.QueueCount}");
        Console.WriteLine($"Наступний на відправку (Peek): {system.PeekNext()?.To}");
        Console.WriteLine("Обробка черги (порядок FIFO — як додавали):");
        int sent = system.ProcessQueue();
        Console.WriteLine($"Відправлено: {sent}, залишилось у черзі: {system.QueueCount}");

        system.EnqueueMailing("Акція вихідного дня");
        Console.WriteLine("\nЧасткова відправка (лише 2 листи):");
        system.ProcessQueue(limit: 2);
        Console.WriteLine($"Залишилось у черзі: {system.QueueCount}");
        Console.WriteLine("Дообробка решти черги:");
        system.ProcessQueue();

        Console.WriteLine("\n=== 3. Stack: історія розсилок ===");
        Console.WriteLine("Історія (від останньої до першої, LIFO):");
        foreach (var r in system.GetHistory())
            Console.WriteLine($"   \"{r.Subject}\" — {r.Recipients} отримувачів, {r.SentAt:HH:mm:ss}");

        var last = system.PeekLastMailing();
        Console.WriteLine($"Остання розсилка (Peek): \"{last?.Subject}\"");
        var undone = system.UndoLastMailing();
        Console.WriteLine($"Скасовано запис (Pop): \"{undone?.Subject}\"");
        Console.WriteLine($"Записів в історії залишилось: {system.HistoryCount}");

        Console.WriteLine("\n=== 4. Порівняння продуктивності: HashSet vs List ===");
        const int n = 50_000;
        var list = new List<string>();
        var set = new HashSet<string>();
        for (int i = 0; i < n; i++)
        {
            string email = $"user{i}@mail.com";
            list.Add(email);
            set.Add(email);
        }

        string target = $"user{n - 1}@mail.com";
        const int searches = 1000;

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < searches; i++) list.Contains(target);
        sw.Stop();
        long listMs = sw.ElapsedMilliseconds;

        sw.Restart();
        for (int i = 0; i < searches; i++) set.Contains(target);
        sw.Stop();
        long setMs = sw.ElapsedMilliseconds;

        Console.WriteLine($"{searches} пошуків серед {n} елементів:");
        Console.WriteLine($"   List<string>.Contains    — {listMs} мс  (O(n))");
        Console.WriteLine($"   HashSet<string>.Contains — {setMs} мс  (O(1) у середньому)");
    }
}