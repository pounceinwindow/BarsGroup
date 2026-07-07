using Domain.Enums;
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
                ""CompetencyMatrices"",
                ""VacancyCompetencies"",
                ""Interviews"",
                ""Candidates"",
                ""Vacancies"",
                ""Competencies"",
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
                Role = UserRole.Admin,
                FirstName = "Админ",
                LastName = "Системы",
                PasswordHash = Hash("admin123"),
                CreatedAt = DateOnly.FromDateTime(DateTime.Now)
            },
            new User
            {
                Id = 2,
                Role = UserRole.HR,
                FirstName = "Елена",
                LastName = "Петрова",
                PasswordHash = Hash("hr123"),
                CreatedAt = DateOnly.FromDateTime(DateTime.Now)
            },
            new User
            {
                Id = 3,
                Role = UserRole.Decider,
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
        var competencies = new List<Competency>
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
        await context.Competencies.AddRangeAsync(competencies);
        await context.SaveChangesAsync();

        // 4. Связь вакансий с компетенциями
        var vacancyCompetencies = new List<VacancyCompetency>
        {
            // Junior (1-5)
            new VacancyCompetency { VacancyId = 1, CompetencyId = 1 },
            new VacancyCompetency { VacancyId = 1, CompetencyId = 2 },
            new VacancyCompetency { VacancyId = 1, CompetencyId = 3 },
            new VacancyCompetency { VacancyId = 1, CompetencyId = 4 },
            new VacancyCompetency { VacancyId = 1, CompetencyId = 5 },
            
            // Middle (6-10)
            new VacancyCompetency { VacancyId = 2, CompetencyId = 6 },
            new VacancyCompetency { VacancyId = 2, CompetencyId = 7 },
            new VacancyCompetency { VacancyId = 2, CompetencyId = 8 },
            new VacancyCompetency { VacancyId = 2, CompetencyId = 9 },
            new VacancyCompetency { VacancyId = 2, CompetencyId = 10 },
            
            // Senior (11-15)
            new VacancyCompetency { VacancyId = 3, CompetencyId = 11 },
            new VacancyCompetency { VacancyId = 3, CompetencyId = 12 },
            new VacancyCompetency { VacancyId = 3, CompetencyId = 13 },
            new VacancyCompetency { VacancyId = 3, CompetencyId = 14 },
            new VacancyCompetency { VacancyId = 3, CompetencyId = 15 }
        };
        await context.VacancyCompetencies.AddRangeAsync(vacancyCompetencies);
        await context.SaveChangesAsync();

        // 5. Кандидаты (10 штук)
        var candidates = new List<Candidate>
        {
            new Candidate
            {
                Id = 1, FullName = "Алексей Иванов", City = "Москва",
                Status = CandidateStatus.LookingForWork,
                Phone = "+79991112233", Email = "ivanov@mail.ru",
                Education = new[] { "МГУ, факультет ВМК" },
                PreviousWork = Array.Empty<string>(),
                Skills = new[] { "C#", "SQL", "Git" }
            },
            new Candidate
            {
                Id = 2, FullName = "Мария Сидорова", City = "Санкт-Петербург",
                Status = CandidateStatus.LookingForWork,
                Phone = "+79992223344", Email = "sidorova@mail.ru", Telegram = "@masha_dev",
                Education = new[] { "СПбГУ", "Магистратура" },
                PreviousWork = new[] { "ООО Технологии" },
                Skills = new[] { "C#", "EF Core", "REST" }
            },
            new Candidate
            {
                Id = 3, FullName = "Дмитрий Козлов", City = "Новосибирск",
                Status = CandidateStatus.Hired,
                Phone = "+79993334455", Email = "kozlov@yandex.ru",
                Education = new[] { "НГУ" },
                PreviousWork = new[] { "Яндекс", "ООО Стартап" },
                Skills = new[] { "C#", "Microservices", "Docker" }
            },
            new Candidate
            {
                Id = 4, FullName = "Екатерина Новикова", City = "Екатеринбург",
                Status = CandidateStatus.LookingForWork,
                Phone = "+79994445566", Email = "novikova@gmail.com",
                Education = new[] { "УрФУ" },
                PreviousWork = Array.Empty<string>(),
                Skills = new[] { "C#", "OOP" }
            },
            new Candidate
            {
                Id = 5, FullName = "Сергей Морозов", City = "Казань",
                Status = CandidateStatus.LookingForWork,
                Phone = "+79995556677", Email = "morozov@mail.ru", Telegram = "@sergey_m",
                Education = new[] { "КФУ" },
                PreviousWork = new[] { "Татнефть" },
                Skills = new[] { "C#", "SQL", "WPF" }
            },
            new Candidate
            {
                Id = 6, FullName = "Анна Волкова", City = "Москва",
                Status = CandidateStatus.Hired,
                Phone = "+79996667788", Email = "volkova@inbox.ru",
                Education = new[] { "МФТИ", "Бакалавриат", "Магистратура" },
                PreviousWork = new[] { "Mail.ru Group" },
                Skills = new[] { "C#", "ASP.NET Core", "PostgreSQL" }
            },
            new Candidate
            {
                Id = 7, FullName = "Андрей Соколов", City = "Нижний Новгород",
                Status = CandidateStatus.LookingForWork,
                Phone = "+79997778899", Email = "sokolov@mail.ru",
                Education = new[] { "ННГУ" },
                PreviousWork = Array.Empty<string>(),
                Skills = new[] { "C#", "Git" }
            },
            new Candidate
            {
                Id = 8, FullName = "Ольга Лебедева", City = "Самара",
                Status = CandidateStatus.LookingForWork,
                Phone = "+79998889900", Email = "lebedeva@yandex.ru", Telegram = "@olga_lb",
                Education = new[] { "Самарский университет" },
                PreviousWork = new[] { "Сбербанк" },
                Skills = new[] { "C#", "EF Core", "Unit Testing" }
            },
            new Candidate
            {
                Id = 9, FullName = "Павел Кузнецов", City = "Ростов-на-Дону",
                Status = CandidateStatus.Hired,
                Phone = "+79999990011", Email = "kuznetsov@gmail.com",
                Education = new[] { "ЮФУ" },
                PreviousWork = new[] { "Ростелеком" },
                Skills = new[] { "C#", "Architecture", "Microservices" }
            },
            new Candidate
            {
                Id = 10, FullName = "Татьяна Попова", City = "Краснодар",
                Status = CandidateStatus.LookingForWork,
                Phone = "+79990001122", Email = "popova@mail.ru",
                Education = new[] { "КубГУ" },
                PreviousWork = Array.Empty<string>(),
                Skills = new[] { "C#", "OOP", "SQL" }
            }
        };
        await context.Candidates.AddRangeAsync(candidates);
        await context.SaveChangesAsync();

        var guid = Guid.NewGuid();
        // 6. Интервью (каждый кандидат хотя бы на одну вакансию)
        var interviews = new List<Interview>
        {
            new Interview { Id = 1, VacancyId = 1, CandidateId = 1, ProcessId = Guid.NewGuid(), Date = DateTime.UtcNow.AddDays(-10), Status = InterviewStatus.Completed },
            new Interview { Id = 2, VacancyId = 1, CandidateId = 2, ProcessId = Guid.NewGuid(),  Date = DateTime.UtcNow.AddDays(-8), Status = InterviewStatus.Completed },
            new Interview { Id = 3, VacancyId = 2, CandidateId = 3, ProcessId = Guid.NewGuid(),  Date = DateTime.UtcNow.AddDays(-7), Status = InterviewStatus.Completed },
            new Interview { Id = 4, VacancyId = 2, CandidateId = 4, ProcessId = Guid.NewGuid(),  Date = DateTime.UtcNow.AddDays(-5), Status = InterviewStatus.WaitingForVerdict },
            new Interview { Id = 5, VacancyId = 3, CandidateId = 5, ProcessId = Guid.NewGuid(),  Date = DateTime.UtcNow.AddDays(-4), Status = InterviewStatus.WaitingForVerdict },
            new Interview { Id = 6, VacancyId = 3, CandidateId = 6, ProcessId = Guid.NewGuid(),  Date = DateTime.UtcNow.AddDays(-3), Status = InterviewStatus.Completed },
            new Interview { Id = 7, VacancyId = 1, CandidateId = 7, ProcessId = Guid.NewGuid(),  Date = DateTime.UtcNow.AddDays(-2), Status = InterviewStatus.WaitingForVerdict },
            new Interview { Id = 8, VacancyId = 2, CandidateId = 8, ProcessId = Guid.NewGuid(),  Date = DateTime.UtcNow.AddDays(-1), Status = InterviewStatus.WaitingForVerdict },
            new Interview { Id = 9, VacancyId = 3, CandidateId = 9, ProcessId = Guid.NewGuid(),  Date = DateTime.UtcNow, Status = InterviewStatus.Completed },
            new Interview { Id = 10, VacancyId = 1, CandidateId = 10, ProcessId = Guid.NewGuid(),  Date = DateTime.UtcNow, Status = InterviewStatus.WaitingForVerdict },
            // Некоторые кандидаты на несколько вакансий
            new Interview { Id = 11, VacancyId = 2, CandidateId = 1, ProcessId = Guid.NewGuid(),  Date = DateTime.UtcNow.AddDays(-6), Status = InterviewStatus.WaitingForVerdict },
            new Interview { Id = 12, VacancyId = 3, CandidateId = 2, ProcessId = Guid.NewGuid(),  Date = DateTime.UtcNow.AddDays(-4), Status = InterviewStatus.WaitingForVerdict },
            // Несколько этапов
            new Interview { Id = 13, VacancyId = 3, CandidateId = 4, ProcessId = guid, Date = DateTime.UtcNow.AddDays(-7), Status = InterviewStatus.Completed },
            new Interview { Id = 14, VacancyId = 3, CandidateId = 4, ProcessId = guid, Date = DateTime.UtcNow.AddDays(2), Status = InterviewStatus.Scheduled },
            // Запланированные интервью
            new Interview { Id = 15, VacancyId = 3, CandidateId = 3, ProcessId = Guid.NewGuid(), Date = DateTime.UtcNow.AddDays(1), Status = InterviewStatus.Scheduled },
            new Interview { Id = 16, VacancyId = 3, CandidateId = 7, ProcessId = Guid.NewGuid(), Date = DateTime.UtcNow.AddDays(2), Status = InterviewStatus.Scheduled },
        };
        await context.Interviews.AddRangeAsync(interviews);
        await context.SaveChangesAsync();

        // 7. Матрица компетенций (оценки кандидатов по компетенциям)
        var competencyMatrices = new List<CompetencyMatrix>();
        var random = new Random(42); // фиксируем seed для воспроизводимости

        // Оценки для Junior (компетенции 1-5)
        for (int interviewId = 1; interviewId <= 12; interviewId++)
        {
            for (int competencyId = 1; competencyId <= 5; competencyId++)
            {
                competencyMatrices.Add(new CompetencyMatrix
                {
                    InterviewId = interviewId,
                    CompetencyId = competencyId,
                    Score = random.Next(1, 6),
                    Comment = interviewId % 3 == 0 ? "Хорошие знания" : null
                });
            }
        }

        // Оценки для Middle (компетенции 6-10)
        for (int interviewId = 1; interviewId <= 12; interviewId++)
        {
            for (int competencyId = 6; competencyId <= 10; competencyId++)
            {
                competencyMatrices.Add(new CompetencyMatrix
                {
                    InterviewId = interviewId,
                    CompetencyId = competencyId,
                    Score = random.Next(1, 6),
                    Comment = interviewId % 2 == 0 ? "Отличный уровень" : null
                });
            }
        }

        // Оценки для Senior (компетенции 11-15)
        for (int interviewId = 1; interviewId <= 13; interviewId++)
        {
            for (int competencyId = 11; competencyId <= 15; competencyId++)
            {
                competencyMatrices.Add(new CompetencyMatrix
                {
                    InterviewId = interviewId,
                    CompetencyId = competencyId,
                    Score = random.Next(1, 6),
                    Comment = interviewId % 4 == 0 ? "Экспертный уровень" : null
                });
            }
        }

        await context.CompetencyMatrices.AddRangeAsync(competencyMatrices);
        await context.SaveChangesAsync();

        // 8. Вердикты от решалы (UserId = 3)
        var verdicts = new List<Verdict>
        {
            new Verdict { InterviewId = 1, UserId = 3, Decision = DeciderVerdict.Hired, Comment = "Хорошо показал себя на интервью" },
            new Verdict { InterviewId = 2, UserId = 3, Decision = DeciderVerdict.Rejected, Comment = "Недостаточно знаний" },
            new Verdict { InterviewId = 3, UserId = 3, Decision = DeciderVerdict.Hired, Comment = "Отличный кандидат" },
            new Verdict { InterviewId = 6, UserId = 3, Decision = DeciderVerdict.Hired, Comment = "Рекомендую" },
            new Verdict { InterviewId = 9, UserId = 3, Decision = DeciderVerdict.Hired, Comment = "Сильный специалист" },
            new Verdict { InterviewId = 13, UserId = 3, Decision = DeciderVerdict.NextStage, Comment = "Перепроверить навыки наставничества" }
        };
        await context.Verdicts.AddRangeAsync(verdicts);
        await context.SaveChangesAsync();
    }

    private static string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}