using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

/// <summary>
/// Заполняет БД тестовыми данными
/// </summary>
public static class DbSeeder
{
    public static async Task ClearAndSeedAsync(BarsContext context)
    {
        // Очистка всех таблиц в правильном порядке (учитывая foreign keys)
        await ClearAllTablesAsync(context);

        // Заполнение данными
        await SeedAsync(context);
    }

    private static async Task ClearAllTablesAsync(BarsContext context)
    {
        // Используем TRUNCATE с CASCADE для очистки всех таблиц
        // Порядок важен из-за foreign key constraints
        var sql = @"
            TRUNCATE TABLE 
                ""Verdicts"",
                ""CompetitionsMatrix"",
                ""VacancyCompetition"",
                ""Interviews"",
                ""Candidates"",
                ""Vacancies"",
                ""Competitions"",
                ""Users""
            RESTART IDENTITY CASCADE;
        ";

        await context.Database.ExecuteSqlRawAsync(sql);
    }

    private static async Task SeedAsync(BarsContext context)
    {
        // 1. Пользователи
        var users = new List<User>
        {
            new User
            {
                Id = 1,
                Role = "Admin",
                FirstName = "Админ",
                LastName = "Системы",
                PasswordHash = Hash("admin123"),
                CreatedAt = DateOnly.FromDateTime(DateTime.Now)
            },
            new User
            {
                Id = 2,
                Role = "HR",
                FirstName = "Елена",
                LastName = "Петрова",
                PasswordHash = Hash("hr123"),
                CreatedAt = DateOnly.FromDateTime(DateTime.Now)
            },
            new User
            {
                Id = 3,
                Role = "Decider",
                FirstName = "Иван",
                LastName = "Решалов",
                PasswordHash = Hash("decider123"),
                CreatedAt = DateOnly.FromDateTime(DateTime.Now)
            }
        };
        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        // 2. Вакансии
        var vacancies = new List<Vacancy>
        {
            new Vacancy { Id = 1, Name = "Junior Developer" },
            new Vacancy { Id = 2, Name = "Middle Developer" },
            new Vacancy { Id = 3, Name = "Senior Developer" }
        };
        await context.Vacancies.AddRangeAsync(vacancies);
        await context.SaveChangesAsync();

        // 3. Компетенции (5 на каждую вакансию = 15)
        var competitions = new List<Competency>
        {
            // Junior Developer
            new Competency { Id = 1, Name = "C# Basics", Description = "Основы языка C#" },
            new Competency { Id = 2, Name = "OOP", Description = "Объектно-ориентированное программирование" },
            new Competency { Id = 3, Name = "SQL Basics", Description = "Основы SQL запросов" },
            new Competency { Id = 4, Name = "Git", Description = "Система контроля версий" },
            new Competency { Id = 5, Name = "Algorithms", Description = "Базовые алгоритмы и структуры данных" },
            
            // Middle Developer
            new Competency { Id = 6, Name = "Advanced C#", Description = "Продвинутые возможности C#" },
            new Competency { Id = 7, Name = "Entity Framework", Description = "ORM фреймворк" },
            new Competency { Id = 8, Name = "REST API", Description = "Проектирование REST API" },
            new Competency { Id = 9, Name = "Unit Testing", Description = "Модульное тестирование" },
            new Competency { Id = 10, Name = "Design Patterns", Description = "Паттерны проектирования" },
            
            // Senior Developer
            new Competency { Id = 11, Name = "Architecture", Description = "Архитектура приложений" },
            new Competency { Id = 12, Name = "Microservices", Description = "Микросервисная архитектура" },
            new Competency { Id = 13, Name = "Performance", Description = "Оптимизация производительности" },
            new Competency { Id = 14, Name = "Security", Description = "Безопасность приложений" },
            new Competency { Id = 15, Name = "Mentoring", Description = "Наставничество и лидерство" }
        };
        await context.Competitions.AddRangeAsync(competitions);
        await context.SaveChangesAsync();

        // 4. Связь вакансий с компетенциями
        var vacancyCompetitions = new List<VacancyCompetition>
        {
            // Junior (1-5)
            new VacancyCompetition { VacancyId = 1, CompetitionId = 1 },
            new VacancyCompetition { VacancyId = 1, CompetitionId = 2 },
            new VacancyCompetition { VacancyId = 1, CompetitionId = 3 },
            new VacancyCompetition { VacancyId = 1, CompetitionId = 4 },
            new VacancyCompetition { VacancyId = 1, CompetitionId = 5 },
            
            // Middle (6-10)
            new VacancyCompetition { VacancyId = 2, CompetitionId = 6 },
            new VacancyCompetition { VacancyId = 2, CompetitionId = 7 },
            new VacancyCompetition { VacancyId = 2, CompetitionId = 8 },
            new VacancyCompetition { VacancyId = 2, CompetitionId = 9 },
            new VacancyCompetition { VacancyId = 2, CompetitionId = 10 },
            
            // Senior (11-15)
            new VacancyCompetition { VacancyId = 3, CompetitionId = 11 },
            new VacancyCompetition { VacancyId = 3, CompetitionId = 12 },
            new VacancyCompetition { VacancyId = 3, CompetitionId = 13 },
            new VacancyCompetition { VacancyId = 3, CompetitionId = 14 },
            new VacancyCompetition { VacancyId = 3, CompetitionId = 15 }
        };
        await context.VacancyCompetition.AddRangeAsync(vacancyCompetitions);
        await context.SaveChangesAsync();

        // 5. Кандидаты (10 штук)
        var candidates = new List<Candidate>
        {
            new Candidate { Id = 1, FirstName = "Алексей", LastName = "Иванов", City = "Москва", Status = "Новый", Phone = "+79991112233", Education = "МГУ", PreviousWork = "Нет опыта" },
            new Candidate { Id = 2, FirstName = "Мария", LastName = "Сидорова", City = "Санкт-Петербург", Status = "В резерве", Phone = "+79992223344", Education = "СПбГУ", PreviousWork = "ООО Технологии" },
            new Candidate { Id = 3, FirstName = "Дмитрий", LastName = "Козлов", City = "Новосибирск", Status = "Принят", Phone = "+79993334455", Education = "НГУ", PreviousWork = "Яндекс" },
            new Candidate { Id = 4, FirstName = "Екатерина", LastName = "Новикова", City = "Екатеринбург", Status = "Новый", Phone = "+79994445566", Education = "УрФУ", PreviousWork = "Нет опыта" },
            new Candidate { Id = 5, FirstName = "Сергей", LastName = "Морозов", City = "Казань", Status = "В резерве", Phone = "+79995556677", Education = "КФУ", PreviousWork = "Татнефть" },
            new Candidate { Id = 6, FirstName = "Анна", LastName = "Волкова", City = "Москва", Status = "Принят", Phone = "+79996667788", Education = "МФТИ", PreviousWork = "Mail.ru Group" },
            new Candidate { Id = 7, FirstName = "Андрей", LastName = "Соколов", City = "Нижний Новгород", Status = "Новый", Phone = "+79997778899", Education = "ННГУ", PreviousWork = "Нет опыта" },
            new Candidate { Id = 8, FirstName = "Ольга", LastName = "Лебедева", City = "Самара", Status = "В резерве", Phone = "+79998889900", Education = "Самарский университет", PreviousWork = "Сбербанк" },
            new Candidate { Id = 9, FirstName = "Павел", LastName = "Кузнецов", City = "Ростов-на-Дону", Status = "Принят", Phone = "+79999990011", Education = "ЮФУ", PreviousWork = "Ростелеком" },
            new Candidate { Id = 10, FirstName = "Татьяна", LastName = "Попова", City = "Краснодар", Status = "Новый", Phone = "+79990001122", Education = "КубГУ", PreviousWork = "Нет опыта" }
        };
        await context.Candidates.AddRangeAsync(candidates);
        await context.SaveChangesAsync();

        // 6. Интервью (каждый кандидат хотя бы на одну вакансию)
        var interviews = new List<Interview>
        {
            new Interview { Id = 1, VacancyId = 1, CandidateId = 1, Date = DateTime.UtcNow.AddDays(-10), Status = "Принят" },
            new Interview { Id = 2, VacancyId = 1, CandidateId = 2, Date = DateTime.UtcNow.AddDays(-8), Status = "Отклонено" },
            new Interview { Id = 3, VacancyId = 2, CandidateId = 3, Date = DateTime.UtcNow.AddDays(-7), Status = "Принят" },
            new Interview { Id = 4, VacancyId = 2, CandidateId = 4, Date = DateTime.UtcNow.AddDays(-5), Status = "Следующий этап" },
            new Interview { Id = 5, VacancyId = 3, CandidateId = 5, Date = DateTime.UtcNow.AddDays(-4), Status = "Ожидает решения" },
            new Interview { Id = 6, VacancyId = 3, CandidateId = 6, Date = DateTime.UtcNow.AddDays(-3), Status = "Принят" },
            new Interview { Id = 7, VacancyId = 1, CandidateId = 7, Date = DateTime.UtcNow.AddDays(-2), Status = "Следующий этап" },
            new Interview { Id = 8, VacancyId = 2, CandidateId = 8, Date = DateTime.UtcNow.AddDays(-1), Status = "Ожидает решения" },
            new Interview { Id = 9, VacancyId = 3, CandidateId = 9, Date = DateTime.UtcNow, Status = "Принят" },
            new Interview { Id = 10, VacancyId = 1, CandidateId = 10, Date = DateTime.UtcNow, Status = "Ожидает решения" },
            // Некоторые кандидаты на несколько вакансий
            new Interview { Id = 11, VacancyId = 2, CandidateId = 1, Date = DateTime.UtcNow.AddDays(-6), Status = "Следующий этап" },
            new Interview { Id = 12, VacancyId = 3, CandidateId = 2, Date = DateTime.UtcNow.AddDays(-4), Status = "Ожидает решения" }
        };
        await context.Interviews.AddRangeAsync(interviews);
        await context.SaveChangesAsync();

        // 7. Матрица компетенций (оценки кандидатов по компетенциям)
        var competitionsMatrix = new List<CompetitionsMatrix>();

        // Оценки для Junior (компетенции 1-5)
        for (int candidateId = 1; candidateId <= 10; candidateId++)
        {
            for (int competitionId = 1; competitionId <= 5; competitionId++)
            {
                competitionsMatrix.Add(new CompetitionsMatrix
                {
                    CandidateId = candidateId,
                    CompetitionId = competitionId,
                    Score = new Random().Next(1, 6), // Оценка от 1 до 5
                    Comment = candidateId % 3 == 0 ? "Хорошие знания" : null
                });
            }
        }

        // Оценки для Middle (компетенции 6-10)
        for (int candidateId = 1; candidateId <= 10; candidateId++)
        {
            for (int competitionId = 6; competitionId <= 10; competitionId++)
            {
                competitionsMatrix.Add(new CompetitionsMatrix
                {
                    CandidateId = candidateId,
                    CompetitionId = competitionId,
                    Score = new Random().Next(1, 6),
                    Comment = candidateId % 2 == 0 ? "Отличный уровень" : null
                });
            }
        }

        // Оценки для Senior (компетенции 11-15)
        for (int candidateId = 1; candidateId <= 10; candidateId++)
        {
            for (int competitionId = 11; competitionId <= 15; competitionId++)
            {
                competitionsMatrix.Add(new CompetitionsMatrix
                {
                    CandidateId = candidateId,
                    CompetitionId = competitionId,
                    Score = new Random().Next(1, 6),
                    Comment = candidateId % 4 == 0 ? "Экспертный уровень" : null
                });
            }
        }

        await context.CompetitionsMatrix.AddRangeAsync(competitionsMatrix);
        await context.SaveChangesAsync();

        // 8. Вердикты от решалы (UserId = 3)
        var verdicts = new List<Verdict>
        {
            new Verdict { InterviewId = 1, UserId = 3, Decision = "Принять", Comment = "Хорошо показал себя на интервью" },
            new Verdict { InterviewId = 2, UserId = 3, Decision = "Отклонить", Comment = "Недостаточно знаний" },
            new Verdict { InterviewId = 3, UserId = 3, Decision = "Принять", Comment = "Отличный кандидат" },
            new Verdict { InterviewId = 6, UserId = 3, Decision = "Принять", Comment = "Рекомендую" },
            new Verdict { InterviewId = 9, UserId = 3, Decision = "Принять", Comment = "Сильный специалист" }
        };
        await context.Verdicts.AddRangeAsync(verdicts);
        await context.SaveChangesAsync();
    }

    private static string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}
