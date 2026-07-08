namespace Domain.Models;

public class Vacancy
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;

    private Vacancy()
    {
    }

    /// <summary>
    /// Создаёт вакансию с указанным названием.
    /// </summary>
    public static Vacancy Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new Vacancy { Name = name };
    }
}