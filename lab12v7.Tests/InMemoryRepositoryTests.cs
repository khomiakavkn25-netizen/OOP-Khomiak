using lab12v7;

namespace lab12v7.Tests;

public class InMemoryRepositoryTests
{
    private static InMemoryRepository<Department> CreateRepo() => new();

    private static Department NewDept(string name = "IT", string loc = "Корпус 1")
        => new() { Name = name, Location = loc };

    [Fact]
    public void Add_NewItem_IsStoredAndGetsId()
    {
        var repo = CreateRepo();
        var d = NewDept();

        repo.Add(d);

        Assert.NotEqual(0, d.Id);
        Assert.Single(repo.GetAll());
    }

    [Fact]
    public void Add_DuplicateId_Throws()
    {
        var repo = CreateRepo();
        repo.Add(new Department { Id = 5, Name = "A", Location = "L" });

        Assert.Throws<ArgumentException>(() =>
            repo.Add(new Department { Id = 5, Name = "B", Location = "L" }));
    }

    [Fact]
    public void Add_Null_Throws()
    {
        var repo = CreateRepo();
        Assert.Throws<ArgumentNullException>(() => repo.Add(null!));
    }

    [Fact]
    public void GetById_Existing_ReturnsItem()
    {
        var repo = CreateRepo();
        var d = NewDept("HR");
        repo.Add(d);

        var result = repo.GetById(d.Id);

        Assert.NotNull(result);
        Assert.Equal("HR", result!.Name);
    }

    [Fact]
    public void GetById_NonExisting_ReturnsNull()
    {
        var repo = CreateRepo();
        Assert.Null(repo.GetById(999));
    }

    [Fact]
    public void GetAll_ReturnsAllItems()
    {
        var repo = CreateRepo();
        repo.Add(NewDept("A"));
        repo.Add(NewDept("B"));
        repo.Add(NewDept("C"));

        Assert.Equal(3, repo.GetAll().Count());
    }

    [Fact]
    public void GetAll_EmptyRepository_ReturnsEmpty()
    {
        Assert.Empty(CreateRepo().GetAll());
    }

    [Fact]
    public void Update_Existing_ChangesItem()
    {
        var repo = CreateRepo();
        var d = NewDept("Old", "L1");
        repo.Add(d);

        repo.Update(new Department { Id = d.Id, Name = "New", Location = "L2" });

        var result = repo.GetById(d.Id);
        Assert.Equal("New", result!.Name);
        Assert.Equal("L2", result.Location);
    }

    [Fact]
    public void Update_NonExisting_Throws()
    {
        var repo = CreateRepo();
        Assert.Throws<KeyNotFoundException>(() =>
            repo.Update(new Department { Id = 42, Name = "X", Location = "Y" }));
    }

    [Fact]
    public void Delete_Existing_RemovesItem()
    {
        var repo = CreateRepo();
        var d = NewDept();
        repo.Add(d);

        repo.Delete(d.Id);

        Assert.Null(repo.GetById(d.Id));
        Assert.Empty(repo.GetAll());
    }

    [Fact]
    public void Delete_NonExisting_Throws()
    {
        var repo = CreateRepo();
        Assert.Throws<KeyNotFoundException>(() => repo.Delete(123));
    }
}