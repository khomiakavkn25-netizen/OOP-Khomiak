using System.Diagnostics;

namespace lab15v7;

public record Manager(int Id, string Name, string Department);

public record Project(int Id, string Name, DateTime StartDate, DateTime EndDate, int ManagerId);

public static class Program
{
    private static (List<T> Result, double Ms) Measure<T>(Func<IEnumerable<T>> query)
    {
        var sw = Stopwatch.StartNew();
        var result = query().ToList();
        sw.Stop();
        return (result, sw.Elapsed.TotalMilliseconds);
    }

    private static (T Result, double Ms) MeasureOne<T>(Func<T> query)
    {
        var sw = Stopwatch.StartNew();
        var result = query();
        sw.Stop();
        return (result, sw.Elapsed.TotalMilliseconds);
    }

    private static double Average<T>(Func<IEnumerable<T>> query, int runs = 20)
    {
        query().ToList();
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < runs; i++) query().ToList();
        sw.Stop();
        return sw.Elapsed.TotalMilliseconds / runs;
    }

    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var managers = new List<Manager>
        {
            new(1, "Іван Петренко", "IT"),
            new(2, "Олена Коваль", "IT"),
            new(3, "Андрій Шевчук", "IT"),
            new(4, "Марія Бондар", "IT"),
            new(5, "Наталія Мельник", "Finance"),
            new(6, "Сергій Ткачук", "Finance"),
            new(7, "Оксана Гончар", "HR"),
            new(8, "Дмитро Кравець", "HR"),
            new(9, "Юлія Лисенко", "Marketing"),
            new(10, "Віктор Савченко", "Marketing"),
            new(11, "Тарас Олійник", "Finance")
        };

        var projects = new List<Project>
        {
            new(1,  "CRM-система",          new(2023, 1, 10),  new(2023, 9, 30),  1),
            new(2,  "Мобільний застосунок", new(2023, 3, 1),   new(2024, 2, 15),  2),
            new(3,  "Міграція в хмару",     new(2022, 11, 5),  new(2023, 6, 20),  1),
            new(4,  "Корпоративний портал", new(2023, 5, 15),  new(2023, 12, 31), 3),
            new(5,  "Бюджет 2024",          new(2023, 8, 1),   new(2023, 12, 15), 5),
            new(6,  "Аудит безпеки",        new(2023, 2, 20),  new(2023, 4, 25),  4),
            new(7,  "Рекрутинг-платформа",  new(2023, 4, 10),  new(2024, 1, 20),  7),
            new(8,  "Рекламна кампанія",    new(2023, 6, 1),   new(2023, 10, 1),  9),
            new(9,  "ERP-інтеграція",       new(2021, 9, 1),   new(2024, 5, 30),  3),
            new(10, "Data Warehouse",       new(2023, 10, 5),  new(2024, 8, 10),  2),
            new(11, "Навчання персоналу",   new(2024, 1, 15),  new(2024, 6, 15),  8),
            new(12, "Ребрендинг",           new(2022, 7, 1),   new(2023, 3, 1),   10),
            new(13, "Фінансова звітність",  new(2024, 2, 1),   new(2024, 7, 1),   6),
            new(14, "DevOps-пайплайн",      new(2023, 7, 12),  new(2023, 11, 20), 1),
            new(15, "Чат-бот підтримки",    new(2024, 3, 1),   new(2024, 9, 1),   4),
            new(16, "Оптимізація витрат",   new(2023, 9, 1),   new(2024, 3, 1),   5),
            new(17, "Онбординг HR",         new(2023, 11, 1),  new(2024, 4, 30),  7),
            new(18, "SEO-просування",       new(2024, 4, 1),   new(2024, 10, 1),  9),
            new(19, "API-шлюз",             new(2023, 12, 1),  new(2024, 7, 15),  2),
            new(20, "Резервне копіювання",  new(2023, 1, 25),  new(2023, 5, 25),  3)
        };

        Console.WriteLine($"Менеджерів: {managers.Count}, проєктів: {projects.Count}\n");

        Console.WriteLine("=== Запит 1: проєкти 2023 року відділу IT ===");

        var (q1Query, t1Query) = Measure(() =>
            from p in projects
            join m in managers on p.ManagerId equals m.Id
            where p.StartDate.Year == 2023 && m.Department == "IT"
            select new { p.Id, p.Name, p.StartDate, Manager = m.Name });

        var (q1Method, t1Method) = Measure(() =>
            projects
                .Join(managers, p => p.ManagerId, m => m.Id, (p, m) => new { p, m })
                .Where(x => x.p.StartDate.Year == 2023 && x.m.Department == "IT")
                .Select(x => new { x.p.Id, x.p.Name, x.p.StartDate, Manager = x.m.Name }));

        foreach (var r in q1Query)
            Console.WriteLine($"  #{r.Id,-2} {r.Name,-22} {r.StartDate:dd.MM.yyyy}  ({r.Manager})");
        Console.WriteLine($"  Синтаксис запитів: {t1Query:F4} мс, кількість: {q1Query.Count}");
        Console.WriteLine($"  Синтаксис методів: {t1Method:F4} мс, кількість: {q1Method.Count}\n");

        Console.WriteLine("=== Запит 2: проєкти з менеджерами (Join) ===");

        var (q2Query, t2Query) = Measure(() =>
            from p in projects
            join m in managers on p.ManagerId equals m.Id
            select new { Project = p.Name, Manager = m.Name });

        var (q2Method, t2Method) = Measure(() =>
            projects.Join(managers,
                p => p.ManagerId,
                m => m.Id,
                (p, m) => new { Project = p.Name, Manager = m.Name }));

        foreach (var r in q2Query)
            Console.WriteLine($"  {r.Project,-22} -> {r.Manager}");
        Console.WriteLine($"  Синтаксис запитів: {t2Query:F4} мс, кількість: {q2Query.Count}");
        Console.WriteLine($"  Синтаксис методів: {t2Method:F4} мс, кількість: {q2Method.Count}\n");

        Console.WriteLine("=== Запит 3: кількість проєктів для кожного менеджера ===");

        var (q3Query, t3Query) = Measure(() =>
            from p in projects
            group p by p.ManagerId into g
            join m in managers on g.Key equals m.Id
            orderby g.Count() descending
            select new { Manager = m.Name, Count = g.Count() });

        var (q3Method, t3Method) = Measure(() =>
            managers.GroupJoin(projects,
                    m => m.Id,
                    p => p.ManagerId,
                    (m, ps) => new { Manager = m.Name, Count = ps.Count() })
                .OrderByDescending(x => x.Count));

        Console.WriteLine("  GroupBy (синтаксис запитів):");
        foreach (var r in q3Query)
            Console.WriteLine($"    {r.Manager,-18} {r.Count}");
        Console.WriteLine($"  Час: {t3Query:F4} мс");

        Console.WriteLine("  GroupJoin (синтаксис методів, включно з нулями):");
        foreach (var r in q3Method)
            Console.WriteLine($"    {r.Manager,-18} {r.Count}");
        Console.WriteLine($"  Час: {t3Method:F4} мс\n");

        Console.WriteLine("=== Запит 4: проєкт з найдовшою тривалістю ===");

        var (q4Max, t4Max) = MeasureOne(() => projects.MaxBy(p => p.EndDate - p.StartDate)!);

        var (q4Order, t4Order) = MeasureOne(() =>
            (from p in projects
             orderby (p.EndDate - p.StartDate) descending
             select p).First());

        var (q4Agg, t4Agg) = MeasureOne(() =>
            projects.Aggregate((best, p) =>
                (p.EndDate - p.StartDate) > (best.EndDate - best.StartDate) ? p : best));

        var days = (q4Max.EndDate - q4Max.StartDate).Days;
        Console.WriteLine($"  {q4Max.Name}: {q4Max.StartDate:dd.MM.yyyy} – {q4Max.EndDate:dd.MM.yyyy} ({days} днів)");
        Console.WriteLine($"  MaxBy:             {t4Max:F4} мс");
        Console.WriteLine($"  OrderByDescending: {t4Order:F4} мс");
        Console.WriteLine($"  Aggregate:         {t4Agg:F4} мс\n");

        Console.WriteLine("=== Відкладене та негайне виконання ===");
        var swDef = Stopwatch.StartNew();
        var deferred = projects.Where(p => p.StartDate.Year == 2023);
        swDef.Stop();
        var swImm = Stopwatch.StartNew();
        var immediate = deferred.ToList();
        swImm.Stop();
        Console.WriteLine($"  Створення запиту (Where):  {swDef.Elapsed.TotalMilliseconds:F4} мс");
        Console.WriteLine($"  Виконання (ToList):        {swImm.Elapsed.TotalMilliseconds:F4} мс, елементів: {immediate.Count}");

        projects.Add(new(21, "Новий проєкт 2023", new(2023, 6, 6), new(2023, 7, 7), 1));
        Console.WriteLine($"  Після додавання елемента: deferred.Count() = {deferred.Count()}, immediate.Count = {immediate.Count}\n");
        projects.RemoveAt(projects.Count - 1);

        Console.WriteLine("=== Продуктивність на великих даних (середнє з 20 запусків) ===");
        var rnd = new Random(42);
        var depts = new[] { "IT", "Finance", "HR", "Marketing" };
        var bigManagers = Enumerable.Range(1, 1_000)
            .Select(i => new Manager(i, $"Manager {i}", depts[rnd.Next(depts.Length)]))
            .ToList();
        var bigProjects = Enumerable.Range(1, 20_000)
            .Select(i =>
            {
                var start = new DateTime(2020, 1, 1).AddDays(rnd.Next(0, 365 * 5));
                return new Project(i, $"Project {i}", start, start.AddDays(rnd.Next(30, 700)),
                    rnd.Next(1, bigManagers.Count + 1));
            })
            .ToList();
        Console.WriteLine($"  Менеджерів: {bigManagers.Count}, проєктів: {bigProjects.Count}");

        Console.WriteLine($"  Where + Join (запит 1):             {Average(() => from p in bigProjects join m in bigManagers on p.ManagerId equals m.Id where p.StartDate.Year == 2023 && m.Department == "IT" select p.Name):F4} мс");
        Console.WriteLine($"  Where перед Join (фільтр раніше):   {Average(() => bigProjects.Where(p => p.StartDate.Year == 2023).Join(bigManagers.Where(m => m.Department == "IT"), p => p.ManagerId, m => m.Id, (p, m) => p.Name)):F4} мс");
        Console.WriteLine($"  Join (запит 2):                     {Average(() => bigProjects.Join(bigManagers, p => p.ManagerId, m => m.Id, (p, m) => new { Project = p.Name, Manager = m.Name })):F4} мс");
        Console.WriteLine($"  Вкладений пошук First (O(n*m)):     {Average(() => bigProjects.Select(p => new { p.Name, Manager = bigManagers.First(m => m.Id == p.ManagerId).Name }), 3):F4} мс");
        Console.WriteLine($"  GroupBy (запит 3):                  {Average(() => bigProjects.GroupBy(p => p.ManagerId).Select(g => new { g.Key, C = g.Count() })):F4} мс");
        Console.WriteLine($"  GroupJoin (запит 3):                {Average(() => bigManagers.GroupJoin(bigProjects, m => m.Id, p => p.ManagerId, (m, ps) => new { m.Name, C = ps.Count() })):F4} мс");

        Console.WriteLine($"  MaxBy (запит 4):                    {Average(() => new[] { bigProjects.MaxBy(p => p.EndDate - p.StartDate)! }):F4} мс");
        Console.WriteLine($"  OrderByDescending.First (запит 4):  {Average(() => new[] { bigProjects.OrderByDescending(p => p.EndDate - p.StartDate).First() }):F4} мс");
    }
}