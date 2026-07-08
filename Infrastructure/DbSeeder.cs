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
        // Данные сидятся через raw SQL, а не через доменные фабрики и методы агрегатов:
        // нужны фиксированные Id, даты в прошлом/будущем, статусы Completed/WaitingForVerdict
        // без прохождения полного lifecycle (Schedule → SubmitProtocol → Complete).
        // Домен остаётся строгим для application-кода

        var adminHash = Hash("admin123");
        var hrHash = Hash("hr123");
        var deciderHash = Hash("decider123");

        var utcNow = DateTime.UtcNow;
        var processGuid1314 = Guid.NewGuid();

        var interviewProcessIds = new[]
        {
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(),
            processGuid1314,
            processGuid1314,
            Guid.NewGuid(), Guid.NewGuid()
        };

        // 1. Пользователи
        await context.Database.ExecuteSqlRawAsync($"""
                                                   INSERT INTO "Users" ("Id", "FirstName", "LastName", "PasswordHash", "Role", "CreatedAt", "RevokedAt", "RevokedBy")
                                                   OVERRIDING SYSTEM VALUE VALUES
                                                   (1, 'Админ', 'Системы', {Sql(adminHash)}, 'Admin', CURRENT_DATE, NULL, NULL),
                                                   (2, 'Елена', 'Петрова', {Sql(hrHash)}, 'HR', CURRENT_DATE, NULL, NULL),
                                                   (3, 'Иван', 'Решалов', {Sql(deciderHash)}, 'Decider', CURRENT_DATE, NULL, NULL);
                                                   """);

        // 2. Вакансии
        await context.Database.ExecuteSqlRawAsync("""
                                                  INSERT INTO "Vacancies" ("Id", "Name")
                                                  OVERRIDING SYSTEM VALUE VALUES
                                                  (1, 'Junior Developer'),
                                                  (2, 'Middle Developer'),
                                                  (3, 'Senior Developer');
                                                  """);

        // 3. Компетенции (5 на каждую вакансию = 15)
        await context.Database.ExecuteSqlRawAsync("""
                                                  INSERT INTO "Competencies" ("Id", "Name", "Description")
                                                  OVERRIDING SYSTEM VALUE VALUES
                                                  -- Junior Developer
                                                  (1, 'C# Basics', 'Основы языка C#'),
                                                  (2, 'OOP', 'Объектно-ориентированное программирование'),
                                                  (3, 'SQL Basics', 'Основы SQL запросов'),
                                                  (4, 'Git', 'Система контроля версий'),
                                                  (5, 'Algorithms', 'Базовые алгоритмы и структуры данных'),
                                                  -- Middle Developer
                                                  (6, 'Advanced C#', 'Продвинутые возможности C#'),
                                                  (7, 'Entity Framework', 'ORM фреймворк'),
                                                  (8, 'REST API', 'Проектирование REST API'),
                                                  (9, 'Unit Testing', 'Модульное тестирование'),
                                                  (10, 'Design Patterns', 'Паттерны проектирования'),
                                                  -- Senior Developer
                                                  (11, 'Architecture', 'Архитектура приложений'),
                                                  (12, 'Microservices', 'Микросервисная архитектура'),
                                                  (13, 'Performance', 'Оптимизация производительности'),
                                                  (14, 'Security', 'Безопасность приложений'),
                                                  (15, 'Mentoring', 'Наставничество и лидерство');
                                                  """);

        // 4. Связь вакансий с компетенциями
        await context.Database.ExecuteSqlRawAsync("""
                                                  INSERT INTO "VacancyCompetencies" ("VacancyId", "CompetencyId") VALUES
                                                  -- Junior (1-5)
                                                  (1, 1), (1, 2), (1, 3), (1, 4), (1, 5),
                                                  -- Middle (6-10)
                                                  (2, 6), (2, 7), (2, 8), (2, 9), (2, 10),
                                                  -- Senior (11-15)
                                                  (3, 11), (3, 12), (3, 13), (3, 14), (3, 15);
                                                  """);

        // 5. Кандидаты (10 штук)
        await context.Database.ExecuteSqlRawAsync($"""
                                                   INSERT INTO "Candidates" ("Id", "FullName", "City", "Status", "Phone", "Email", "Telegram", "Education", "PreviousWork", "Skills")
                                                   OVERRIDING SYSTEM VALUE VALUES
                                                   (1, 'Алексей Иванов', 'Москва', 'LookingForWork', '+79991112233', 'ivanov@mail.ru', NULL,
                                                       {SqlArray(["МГУ, факультет ВМК"])}, {SqlArray([])}, {SqlArray(["C#", "SQL", "Git"])}),
                                                   (2, 'Мария Сидорова', 'Санкт-Петербург', 'LookingForWork', '+79992223344', 'sidorova@mail.ru', '@masha_dev',
                                                       {SqlArray(["СПбГУ", "Магистратура"])}, {SqlArray(["ООО Технологии"])}, {SqlArray(["C#", "EF Core", "REST"])}),
                                                   (3, 'Дмитрий Козлов', 'Новосибирск', 'Hired', '+79993334455', 'kozlov@yandex.ru', NULL,
                                                       {SqlArray(["НГУ"])}, {SqlArray(["Яндекс", "ООО Стартап"])}, {SqlArray(["C#", "Microservices", "Docker"])}),
                                                   (4, 'Екатерина Новикова', 'Екатеринбург', 'LookingForWork', '+79994445566', 'novikova@gmail.com', NULL,
                                                       {SqlArray(["УрФУ"])}, {SqlArray([])}, {SqlArray(["C#", "OOP"])}),
                                                   (5, 'Сергей Морозов', 'Казань', 'LookingForWork', '+79995556677', 'morozov@mail.ru', '@sergey_m',
                                                       {SqlArray(["КФУ"])}, {SqlArray(["Татнефть"])}, {SqlArray(["C#", "SQL", "WPF"])}),
                                                   (6, 'Анна Волкова', 'Москва', 'Hired', '+79996667788', 'volkova@inbox.ru', NULL,
                                                       {SqlArray(["МФТИ", "Бакалавриат", "Магистратура"])}, {SqlArray(["Mail.ru Group"])}, {SqlArray(["C#", "ASP.NET Core", "PostgreSQL"])}),
                                                   (7, 'Андрей Соколов', 'Нижний Новгород', 'LookingForWork', '+79997778899', 'sokolov@mail.ru', NULL,
                                                       {SqlArray(["ННГУ"])}, {SqlArray([])}, {SqlArray(["C#", "Git"])}),
                                                   (8, 'Ольга Лебедева', 'Самара', 'LookingForWork', '+79998889900', 'lebedeva@yandex.ru', '@olga_lb',
                                                       {SqlArray(["Самарский университет"])}, {SqlArray(["Сбербанк"])}, {SqlArray(["C#", "EF Core", "Unit Testing"])}),
                                                   (9, 'Павел Кузнецов', 'Ростов-на-Дону', 'Hired', '+79999990011', 'kuznetsov@gmail.com', NULL,
                                                       {SqlArray(["ЮФУ"])}, {SqlArray(["Ростелеком"])}, {SqlArray(["C#", "Architecture", "Microservices"])}),
                                                   (10, 'Татьяна Попова', 'Краснодар', 'LookingForWork', '+79990001122', 'popova@mail.ru', NULL,
                                                       {SqlArray(["КубГУ"])}, {SqlArray([])}, {SqlArray(["C#", "OOP", "SQL"])});
                                                   """);

        // 6. Интервью (каждый кандидат хотя бы на одну вакансию)
        await context.Database.ExecuteSqlRawAsync($"""
                                                   INSERT INTO "Interviews" ("Id", "VacancyId", "CandidateId", "ProcessId", "Date", "Status", "SummaryComment", "HrId")
                                                   OVERRIDING SYSTEM VALUE VALUES
                                                   (1, 1, 1, {Sql(interviewProcessIds[0])}, {SqlTimestamp(utcNow.AddDays(-10))}, 'Completed', NULL, 2),
                                                   (2, 1, 2, {Sql(interviewProcessIds[1])}, {SqlTimestamp(utcNow.AddDays(-8))}, 'Completed', NULL, 2),
                                                   (3, 2, 3, {Sql(interviewProcessIds[2])}, {SqlTimestamp(utcNow.AddDays(-7))}, 'Completed', NULL, 2),
                                                   (4, 2, 4, {Sql(interviewProcessIds[3])}, {SqlTimestamp(utcNow.AddDays(-5))}, 'WaitingForVerdict', NULL, 2),
                                                   (5, 3, 5, {Sql(interviewProcessIds[4])}, {SqlTimestamp(utcNow.AddDays(-4))}, 'WaitingForVerdict', NULL, 2),
                                                   (6, 3, 6, {Sql(interviewProcessIds[5])}, {SqlTimestamp(utcNow.AddDays(-3))}, 'Completed', NULL, 2),
                                                   (7, 1, 7, {Sql(interviewProcessIds[6])}, {SqlTimestamp(utcNow.AddDays(-2))}, 'WaitingForVerdict', NULL, 2),
                                                   (8, 2, 8, {Sql(interviewProcessIds[7])}, {SqlTimestamp(utcNow.AddDays(-1))}, 'WaitingForVerdict', NULL, 2),
                                                   (9, 3, 9, {Sql(interviewProcessIds[8])}, {SqlTimestamp(utcNow)}, 'Completed', NULL, 2),
                                                   (10, 1, 10, {Sql(interviewProcessIds[9])}, {SqlTimestamp(utcNow)}, 'WaitingForVerdict', NULL, 2),
                                                   -- Некоторые кандидаты на несколько вакансий
                                                   (11, 2, 1, {Sql(interviewProcessIds[10])}, {SqlTimestamp(utcNow.AddDays(-6))}, 'WaitingForVerdict', NULL, 2),
                                                   (12, 3, 2, {Sql(interviewProcessIds[11])}, {SqlTimestamp(utcNow.AddDays(-4))}, 'WaitingForVerdict', NULL, 2),
                                                   -- Несколько этапов
                                                   (13, 3, 4, {Sql(interviewProcessIds[12])}, {SqlTimestamp(utcNow.AddDays(-7))}, 'Completed', NULL, 2),
                                                   (14, 3, 4, {Sql(interviewProcessIds[13])}, {SqlTimestamp(utcNow.AddDays(2))}, 'Scheduled', NULL, 2),
                                                   -- Запланированные интервью
                                                   (15, 3, 3, {Sql(interviewProcessIds[14])}, {SqlTimestamp(utcNow.AddDays(1))}, 'Scheduled', NULL, 2),
                                                   (16, 3, 7, {Sql(interviewProcessIds[15])}, {SqlTimestamp(utcNow.AddDays(2))}, 'Scheduled', NULL, 2);
                                                   """);

        // 7. Матрица компетенций (оценки кандидатов по компетенциям)
        var competencyMatrixSql = BuildCompetencyMatrixInsertSql();
        await context.Database.ExecuteSqlRawAsync(competencyMatrixSql);

        // 8. Вердикты от решалы (UserId = 3)
        await context.Database.ExecuteSqlRawAsync("""
                                                  INSERT INTO "Verdicts" ("InterviewId", "UserId", "Decision", "Comment") VALUES
                                                  (1, 3, 'Hired', 'Хорошо показал себя на интервью'),
                                                  (2, 3, 'Rejected', 'Недостаточно знаний'),
                                                  (3, 3, 'Hired', 'Отличный кандидат'),
                                                  (6, 3, 'Hired', 'Рекомендую'),
                                                  (9, 3, 'Hired', 'Сильный специалист'),
                                                  (13, 3, 'NextStage', 'Перепроверить навыки наставничества');
                                                  """);

        await context.Database.ExecuteSqlRawAsync("""
                                                  SELECT setval(pg_get_serial_sequence('"Users"', 'Id'), (SELECT COALESCE(MAX("Id"), 1) FROM "Users"));
                                                  SELECT setval(pg_get_serial_sequence('"Vacancies"', 'Id'), (SELECT COALESCE(MAX("Id"), 1) FROM "Vacancies"));
                                                  SELECT setval(pg_get_serial_sequence('"Competencies"', 'Id'), (SELECT COALESCE(MAX("Id"), 1) FROM "Competencies"));
                                                  SELECT setval(pg_get_serial_sequence('"Candidates"', 'Id'), (SELECT COALESCE(MAX("Id"), 1) FROM "Candidates"));
                                                  SELECT setval(pg_get_serial_sequence('"Interviews"', 'Id'), (SELECT COALESCE(MAX("Id"), 1) FROM "Interviews"));
                                                  """);
    }

    private static string BuildCompetencyMatrixInsertSql()
    {
        var competencyMatrices = new List<string>();
        var random = new Random(42); // фиксируем seed для воспроизводимости

        // Оценки для Junior (компетенции 1-5)
        for (int interviewId = 1; interviewId <= 12; interviewId++)
        {
            for (int competencyId = 1; competencyId <= 5; competencyId++)
            {
                var comment = interviewId % 3 == 0 ? "Хорошие знания" : null;
                competencyMatrices.Add(
                    $"({interviewId}, {competencyId}, {random.Next(1, 6)}, {Sql(comment)})");
            }
        }

        // Оценки для Middle (компетенции 6-10)
        for (int interviewId = 1; interviewId <= 12; interviewId++)
        {
            for (int competencyId = 6; competencyId <= 10; competencyId++)
            {
                var comment = interviewId % 2 == 0 ? "Отличный уровень" : null;
                competencyMatrices.Add(
                    $"({interviewId}, {competencyId}, {random.Next(1, 6)}, {Sql(comment)})");
            }
        }

        // Оценки для Senior (компетенции 11-15)
        for (int interviewId = 1; interviewId <= 13; interviewId++)
        {
            for (int competencyId = 11; competencyId <= 15; competencyId++)
            {
                var comment = interviewId % 4 == 0 ? "Экспертный уровень" : null;
                competencyMatrices.Add(
                    $"({interviewId}, {competencyId}, {random.Next(1, 6)}, {Sql(comment)})");
            }
        }

        return $"""
                INSERT INTO "CompetencyMatrices" ("InterviewId", "CompetencyId", "Score", "Comment") VALUES
                {string.Join(",\n", competencyMatrices)};
                """;
    }

    private static string Sql(string? value) =>
        value is null ? "NULL" : $"'{value.Replace("'", "''")}'";

    private static string Sql(Guid value) => $"'{value}'";

    private static string SqlTimestamp(DateTime value) =>
        $"'{value:yyyy-MM-dd HH:mm:ss.ffffff}+00'";

    private static string SqlArray(string[] values)
    {
        if (values.Length == 0)
            return "ARRAY[]::varchar(200)[]";

        var items = string.Join(", ", values.Select(v => $"'{v.Replace("'", "''")}'"));
        return $"ARRAY[{items}]::varchar(200)[]";
    }

    private static string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}