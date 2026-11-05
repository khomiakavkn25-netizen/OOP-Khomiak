using System.Text;

namespace lab16v7;

public record Department(int Id, string Name, int NumStudents);

public record Faculty(int Id, string Name, List<Department> Departments);

public record University(int Id, string Name, List<Faculty> Faculties);

public static class DepartmentExtensions
{
    public static int TotalStudents(this IEnumerable<Department> departments)
    {
        ArgumentNullException.ThrowIfNull(departments);
        return departments.Sum(d => d.NumStudents);
    }
}

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        int departmentId = 0;
        int facultyId = 0;
        int universityId = 0;

        Department D(string name, int students) => new(++departmentId, name, students);
        Faculty F(string name, params Department[] departments) => new(++facultyId, name, departments.ToList());
        University U(string name, params Faculty[] faculties) => new(++universityId, name, faculties.ToList());

        var universities = new List<University>
        {
            U("КНУ імені Шевченка",
                F("Факультет інформаційних технологій",
                    D("Кафедра програмної інженерії", 220),
                    D("Кафедра штучного інтелекту", 180),
                    D("Кафедра кібербезпеки", 150)),
                F("Механіко-математичний факультет",
                    D("Кафедра алгебри", 90),
                    D("Кафедра математичного аналізу", 110)),
                F("Економічний факультет",
                    D("Кафедра економічної теорії", 200),
                    D("Кафедра фінансів", 240),
                    D("Кафедра обліку", 170)),
                F("Юридичний факультет",
                    D("Кафедра цивільного права", 260),
                    D("Кафедра кримінального права", 230))),
            U("Львівська політехніка",
                F("Інститут комп'ютерних наук",
                    D("Кафедра систем штучного інтелекту", 210),
                    D("Кафедра інформаційних систем", 190),
                    D("Кафедра програмного забезпечення", 250)),
                F("Інститут будівництва",
                    D("Кафедра будівельних конструкцій", 140),
                    D("Кафедра архітектури", 120)),
                F("Інститут телекомунікацій",
                    D("Кафедра телекомунікацій", 130),
                    D("Кафедра радіоелектроніки", 100))),
            U("Острозька академія",
                F("Факультет права",
                    D("Кафедра теорії права", 95),
                    D("Кафедра міжнародного права", 85)),
                F("Факультет історії",
                    D("Кафедра всесвітньої історії", 70),
                    D("Кафедра історії України", 80)),
                F("Факультет гуманітарних наук",
                    D("Кафедра філософії", 60),
                    D("Кафедра філології", 75),
                    D("Кафедра педагогіки", 65)),
                F("Факультет економіки",
                    D("Кафедра економіки", 88),
                    D("Кафедра менеджменту", 92)))
        };

        Console.WriteLine($"Університетів: {universities.Count}");
        Console.WriteLine($"Факультетів: {universities.Sum(u => u.Faculties.Count)}");
        Console.WriteLine($"Кафедр: {universities.Sum(u => u.Faculties.Sum(f => f.Departments.Count))}\n");

        Console.WriteLine("=== Запит 1: SelectMany — усі кафедри в одному списку ===");

        var departmentsQuery =
            (from u in universities
             from f in u.Faculties
             from d in f.Departments
             select d).ToList();

        var departmentsMethod = universities
            .SelectMany(u => u.Faculties)
            .SelectMany(f => f.Departments)
            .ToList();

        Console.WriteLine("  Синтаксис методів:");
        foreach (var d in departmentsMethod)
            Console.WriteLine($"    #{d.Id,-2} {d.Name,-36} студентів: {d.NumStudents}");
        Console.WriteLine($"  Кількість (синтаксис запитів): {departmentsQuery.Count}");
        Console.WriteLine($"  Кількість (синтаксис методів): {departmentsMethod.Count}");
        Console.WriteLine($"  Результати однакові: {departmentsQuery.SequenceEqual(departmentsMethod)}\n");

        Console.WriteLine("  SelectMany з результуючим селектором (університет → факультет → кафедра):");
        var detailed = universities
            .SelectMany(u => u.Faculties,
                (u, f) => new { University = u.Name, Faculty = f })
            .SelectMany(x => x.Faculty.Departments,
                (x, d) => new { x.University, Faculty = x.Faculty.Name, Department = d.Name })
            .Take(5);
        foreach (var r in detailed)
            Console.WriteLine($"    {r.University} | {r.Faculty} | {r.Department}");
        Console.WriteLine("    ...\n");

        Console.WriteLine("=== Запит 2: пагінація — факультети по 3 на сторінку ===");

        const int pageSize = 3;
        var faculties = universities.SelectMany(u => u.Faculties).ToList();
        int totalPages = (int)Math.Ceiling(faculties.Count / (double)pageSize);

        for (int page = 1; page <= totalPages; page++)
        {
            var items = faculties
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            Console.WriteLine($"  Сторінка {page} з {totalPages} (елементів: {items.Count}):");
            foreach (var f in items)
                Console.WriteLine($"    #{f.Id,-2} {f.Name}");
        }
        Console.WriteLine();

        Console.WriteLine("=== Запит 3: метод розширення TotalStudents() ===");

        Console.WriteLine($"  Усі кафедри: {departmentsMethod.TotalStudents()} студентів");

        Console.WriteLine("  За факультетами:");
        foreach (var f in faculties)
            Console.WriteLine($"    {f.Name,-38} {f.Departments.TotalStudents()}");

        Console.WriteLine("  За університетами:");
        foreach (var u in universities)
        {
            var total = u.Faculties.SelectMany(f => f.Departments).TotalStudents();
            Console.WriteLine($"    {u.Name,-24} {total}");
        }

        var big = departmentsMethod.Where(d => d.NumStudents >= 200);
        Console.WriteLine($"  Кафедри з 200+ студентів: {big.Count()} шт., разом {big.TotalStudents()} студентів");

        Console.WriteLine($"  Порожня колекція: {new List<Department>().TotalStudents()}");
    }
}