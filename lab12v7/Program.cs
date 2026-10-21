using lab12v7;

Console.OutputEncoding = System.Text.Encoding.UTF8;

IRepository<Department> repo = new InMemoryRepository<Department>();

repo.Add(new Department { Name = "IT", Location = "Корпус 1" });
repo.Add(new Department { Name = "Бухгалтерія", Location = "Корпус 2" });
repo.Add(new Department { Name = "HR", Location = "Корпус 1" });

Console.WriteLine("Усі відділи:");
foreach (var d in repo.GetAll()) Console.WriteLine(d);

Console.WriteLine($"\nGetById(2): {repo.GetById(2)}");

repo.Update(new Department { Id = 2, Name = "Фінансовий відділ", Location = "Корпус 3" });
Console.WriteLine($"Після Update: {repo.GetById(2)}");

repo.Delete(1);
Console.WriteLine("\nПісля Delete(1):");
foreach (var d in repo.GetAll()) Console.WriteLine(d);

namespace lab12v7
{
    public interface IEntity
    {
        int Id { get; set; }
    }

    public class Department : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        public override string ToString() => $"[{Id}] {Name} ({Location})";
    }

    public interface IRepository<T> where T : class, IEntity
    {
        T? GetById(int id);
        IEnumerable<T> GetAll();
        void Add(T item);
        void Update(T item);
        void Delete(int id);
    }

    public class InMemoryRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly List<T> _items = new();
        private int _nextId = 1;

        public T? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);

        public IEnumerable<T> GetAll() => _items.ToList();

        public void Add(T item)
        {
            ArgumentNullException.ThrowIfNull(item);

            if (item.Id == 0)
            {
                item.Id = _nextId++;
            }
            else if (_items.Any(x => x.Id == item.Id))
            {
                throw new ArgumentException($"Елемент з Id={item.Id} вже існує.");
            }
            else if (item.Id >= _nextId)
            {
                _nextId = item.Id + 1;
            }

            _items.Add(item);
        }

        public void Update(T item)
        {
            ArgumentNullException.ThrowIfNull(item);

            int index = _items.FindIndex(x => x.Id == item.Id);
            if (index < 0)
                throw new KeyNotFoundException($"Елемент з Id={item.Id} не знайдено.");

            _items[index] = item;
        }

        public void Delete(int id)
        {
            int index = _items.FindIndex(x => x.Id == id);
            if (index < 0)
                throw new KeyNotFoundException($"Елемент з Id={id} не знайдено.");

            _items.RemoveAt(index);
        }
    }
}