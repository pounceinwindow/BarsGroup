namespace Domain.Models;

public class Competency
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    private Competency()
    {
    }

    /// <summary>
    /// Создаёт компетенцию с названием и опциональным описанием.
    /// </summary>
    public static Competency Create(string name, string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new Competency { Name = name, Description = description };
    }

    /// <summary>
    /// Обновляет данные компетенции.
    /// </summary>
    public void Update(string name, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
        Description = description;
    }
}