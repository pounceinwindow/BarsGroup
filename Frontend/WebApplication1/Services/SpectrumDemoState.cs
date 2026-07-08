using Application.Interview.DTO;
using Domain.Enums;
using Domain.Models;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Services;

public sealed class SpectrumDemoState
{
    public const string HrRole = "Отдел кадров";
    public const string DeciderRole = "Решала";
    public const string AdminRole = "Администратор";

    private readonly BarsContext _context;
    private readonly string[] _roles = [HrRole, DeciderRole, AdminRole];

    public SpectrumDemoState(BarsContext context)
    {
        _context = context;
        Reload();
    }

    public bool IsAuthenticated { get; private set; }
    public string Role { get; private set; } = HrRole;
    public IReadOnlyList<string> Roles => _roles;

    public List<DemoCandidate> Candidates { get; } = [];
    public List<DemoInterview> Interviews { get; } = [];
    public List<DemoVacancy> Vacancies { get; } = [];
    public List<DemoCompetency> Competencies { get; } = [];
    public List<DemoUser> Users { get; } = [];

    public IEnumerable<string> VacancyTitles =>
        Vacancies
            .Where(vacancy => vacancy.IsActive)
            .Select(vacancy => vacancy.Title)
            .Distinct()
            .OrderBy(title => title);

    public IEnumerable<string> VacancyFilterOptions => new[] { "Все вакансии" }.Concat(VacancyTitles);

    public IReadOnlyList<string> Statuses =>
    [
        "Все статусы",
        "Новый",
        "Собеседование запланировано",
        "На согласовании",
        "Принят",
        "Резерв",
        "Отклонён"
    ];

    public IReadOnlyList<string> InterviewFormats => ["Онлайн", "Очно", "Телефон"];
    public IReadOnlyList<string> DecisionOptions => ["Принять", "Следующий этап", "Отказать"];

    public string CurrentUserName => Role switch
    {
        AdminRole => Users.FirstOrDefault(user => user.Role == AdminRole)?.Name ?? "Админ Системы",
        DeciderRole => Users.FirstOrDefault(user => user.Role == DeciderRole)?.Name ?? "Иван Решалов",
        _ => Users.FirstOrDefault(user => user.Role == HrRole)?.Name ?? "Елена Петрова"
    };

    public string CurrentUserShort => ToShortName(CurrentUserName);
    // TODO: убрать заглушку на релизе
    public int CurrentUserId => Role switch
    {
        AdminRole => Users.FirstOrDefault(user => user.Role == AdminRole)?.Id ?? 1,
        DeciderRole => Users.FirstOrDefault(user => user.Role == DeciderRole)?.Id ?? 3,
        _ => Users.FirstOrDefault(user => user.Role == HrRole)?.Id ?? 2
    };

    public bool CanEditProtocol(DemoInterview interview) =>
        interview.DbStatus == InterviewStatus.Scheduled && CurrentUserId == interview.HrId;

    public bool CanCancelInterview(DemoInterview interview) =>
        interview.DbStatus == InterviewStatus.Scheduled
        && (CurrentUserId == interview.HrId || Role == AdminRole);
    public int PendingCount => Candidates.Count(candidate => LatestStatus(candidate) == "На согласовании");

    public IEnumerable<DemoInterview> RecentDecisions =>
        Interviews
            .Where(interview => interview.Decision is not null)
            .OrderByDescending(interview => interview.Date)
            .Take(4);

    public void Reload()
    {
        var competencyLinks = _context.VacancyCompetencies
            .AsNoTracking()
            .ToList();

        var dbVacancies = _context.Vacancies
            .AsNoTracking()
            .OrderBy(vacancy => vacancy.Id)
            .ToList();

        var dbCompetencies = _context.Competencies
            .AsNoTracking()
            .OrderBy(competency => competency.Id)
            .ToList();

        var dbUsers = _context.Users
            .AsNoTracking()
            .OrderBy(user => user.Id)
            .ToList();

        var dbCandidates = _context.Candidates
            .AsNoTracking()
            .OrderBy(candidate => candidate.Id)
            .ToList();

        var dbInterviews = _context.Interviews
            .AsNoTracking()
            .Include(interview => interview.Candidate)
            .Include(interview => interview.Vacancy)
            .Include(interview => interview.Hr)
            .Include(interview => interview.Verdict)
            .Include(interview => interview.MatrixRows)
                .ThenInclude(row => row.Competency)
            .OrderByDescending(interview => interview.Date)
            .ToList();

        Vacancies.Clear();
        Vacancies.AddRange(dbVacancies.Select(vacancy => new DemoVacancy
        {
            Id = vacancy.Id,
            Title = vacancy.Name,
            Department = "Разработка",
            Status = "Открыта",
            Description = $"Вакансия из таблицы Vacancies, Id={vacancy.Id}",
            IsActive = true,
            CompetencyIds = competencyLinks
                .Where(link => link.VacancyId == vacancy.Id)
                .Select(link => link.CompetencyId)
                .ToHashSet()
        }));

        Competencies.Clear();
        Competencies.AddRange(dbCompetencies.Select(competency => new DemoCompetency
        {
            Id = competency.Id,
            Name = competency.Name,
            Description = competency.Description ?? string.Empty,
            IsActive = true
        }));

        Users.Clear();
        Users.AddRange(dbUsers.Select(MapUser));

        Interviews.Clear();
        Interviews.AddRange(dbInterviews.Select(MapInterview));

        Candidates.Clear();
        Candidates.AddRange(dbCandidates.Select(candidate => MapCandidate(candidate, dbInterviews)));
    }

    public void Login(string role)
    {
        Role = _roles.Contains(role) ? role : HrRole;
        IsAuthenticated = true;
    }

    public void Logout()
    {
        IsAuthenticated = false;
        Role = HrRole;
    }

    public DemoCandidate? GetCandidate(int id) => Candidates.FirstOrDefault(candidate => candidate.Id == id);
    public DemoInterview? GetInterview(string id) => Interviews.FirstOrDefault(interview => interview.Id == id);
    public DemoInterview? GetInterviewByResponse(DemoResponse response) => response.InterviewId is null ? null : GetInterview(response.InterviewId);
    public DemoCandidate? GetCandidateForInterview(DemoInterview interview) => GetCandidate(interview.CandidateId);

    public IEnumerable<DemoInterview> CandidateInterviews(int candidateId) =>
        Interviews
            .Where(interview => interview.CandidateId == candidateId)
            .OrderByDescending(interview => interview.Date)
            .ThenByDescending(interview => interview.Time);

    public IEnumerable<DemoCompetency> CompetenciesForInterview(DemoInterview interview)
    {
        var vacancyCompetencyIds = Vacancies.FirstOrDefault(vacancy => vacancy.Id == interview.VacancyId)?.CompetencyIds ?? [];
        return Competencies
            .Where(competency => vacancyCompetencyIds.Contains(competency.Id) && competency.IsActive)
            .OrderBy(competency => competency.Id);
    }

    public IReadOnlyList<DemoCandidate> FilterCandidates(string search, string vacancyFilter, string statusFilter, bool onlyPending = false)
    {
        IEnumerable<DemoCandidate> source = onlyPending
            ? Candidates.Where(candidate => LatestStatus(candidate) == "На согласовании")
            : Candidates;

        return source
            .Where(candidate =>
            {
                var searchMatch = string.IsNullOrWhiteSpace(search)
                    || candidate.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || candidate.Email.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || candidate.Responses.Any(response => response.Vacancy.Contains(search, StringComparison.OrdinalIgnoreCase));
                var vacancyMatch = vacancyFilter == "Все вакансии"
                    || candidate.Responses.Any(response => response.Vacancy == vacancyFilter);
                var statusMatch = statusFilter == "Все статусы"
                    || LatestStatus(candidate) == statusFilter;

                return searchMatch && vacancyMatch && statusMatch;
            })
            .OrderByDescending(candidate => candidate.Responses.Select(response => response.Date).DefaultIfEmpty().Max())
            .ToList();
    }

    public IReadOnlyList<DemoInterview> FilterInterviews(string search, string vacancyFilter, string statusFilter)
    {
        return Interviews
            .Where(interview =>
            {
                var candidate = GetCandidate(interview.CandidateId);
                var searchMatch = string.IsNullOrWhiteSpace(search)
                    || interview.Vacancy.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || (candidate?.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false);
                var vacancyMatch = vacancyFilter == "Все вакансии" || interview.Vacancy == vacancyFilter;
                var statusMatch = statusFilter == "Все статусы" || interview.Status == statusFilter;

                return searchMatch && vacancyMatch && statusMatch;
            })
            .OrderByDescending(interview => interview.Date)
            .ThenByDescending(interview => interview.Time)
            .ToList();
    }

    public string LatestStatus(DemoCandidate candidate)
    {
        var latestInterview = CandidateInterviews(candidate.Id).FirstOrDefault();
        return latestInterview?.Status ?? candidate.Status;
    }

    public double AverageScore(DemoInterview interview) =>
        interview.Scores.Values.Where(score => score > 0).DefaultIfEmpty().Average();

    public double CompetencyAverage(string competencyName)
    {
        var competencyId = Competencies.FirstOrDefault(competency => competency.Name == competencyName)?.Id;
        if (competencyId is null)
            return 0;

        return Interviews
            .Select(interview => interview.Scores.TryGetValue(competencyId.Value, out var score) ? score : 0)
            .Where(score => score > 0)
            .DefaultIfEmpty()
            .Average();
    }

    public DemoInterview EnsureInterview(DemoCandidate candidate, DemoResponse response)
    {
        var existing = GetInterviewByResponse(response);
        if (existing is not null)
            return existing;

        var vacancyId = FindVacancyId(response.Vacancy);
        var date = EnsureFutureUtc(response.Date);
        var interview = Interview.ScheduleNewProcess(candidate.Id, vacancyId, CurrentUserId, date);

        _context.Interviews.Add(interview);
        _context.SaveChanges();
        Reload();

        return GetInterview(interview.Id.ToString())!;
    }

    public DemoCandidate CreateCandidate(CandidateForm form)
    {
        var candidate = Candidate.Create(
            RequiredOrDefault(form.FullName, "Новый кандидат"),
            RequiredOrDefault(form.Phone, "+7 900 000-00-00"),
            RequiredOrDefault(form.Email, "candidate@mail.ru"),
            NullIfWhiteSpace(form.Telegram),
            RequiredOrDefault(form.City, "Казань"),
            SplitArray(form.Education),
            SplitArray(form.Experience),
            SplitTags(form.Skills));

        _context.Candidates.Add(candidate);
        _context.SaveChanges();

        if (!string.IsNullOrWhiteSpace(form.Vacancy))
        {
            var interview = Interview.ScheduleNewProcess(
                candidate.Id,
                FindVacancyId(form.Vacancy),
                CurrentUserId,
                DateTime.UtcNow.AddDays(1));
            _context.Interviews.Add(interview);
            _context.SaveChanges();
        }

        Reload();
        return GetCandidate(candidate.Id)!;
    }

    public void UpdateCandidate(int id, CandidateForm form)
    {
        var fullName = RequiredOrDefault(form.FullName, "Новый кандидат");
        var city = RequiredOrDefault(form.City, "Казань");
        var phone = RequiredOrDefault(form.Phone, "+7 900 000-00-00");
        var email = RequiredOrDefault(form.Email, "candidate@mail.ru");
        var telegram = NullIfWhiteSpace(form.Telegram);
        var education = SplitArray(form.Education);
        var previousWork = SplitArray(form.Experience);
        var skills = SplitTags(form.Skills);

        _context.Database.ExecuteSqlInterpolated($"""
            UPDATE "Candidates"
            SET "FullName" = {fullName},
                "City" = {city},
                "Phone" = {phone},
                "Email" = {email},
                "Telegram" = {telegram},
                "Education" = {education},
                "PreviousWork" = {previousWork},
                "Skills" = {skills}
            WHERE "Id" = {id};
            """);

        Reload();
    }

    public DemoInterview CreateInterview(InterviewForm form)
    {
        var candidateId = Candidates.Any(candidate => candidate.Id == form.CandidateId)
            ? form.CandidateId
            : Candidates.First().Id;
        var vacancyId = FindVacancyId(form.Vacancy);
        var date = EnsureFutureUtc(ParseDateTime(form.Date, form.Time));

        var interview = Interview.ScheduleNewProcess(candidateId, vacancyId, CurrentUserId, date);
        _context.Interviews.Add(interview);
        _context.SaveChanges();

        Reload();
        return GetInterview(interview.Id.ToString())!;
    }

    public DemoVacancy SaveVacancy(VacancyForm form, int? id = null)
    {
        var name = RequiredOrDefault(form.Title, "Новая вакансия");

        if (id is null)
        {
            _context.Vacancies.Add(Vacancy.Create(name));
            _context.SaveChanges();
        }
        else
        {
            _context.Database.ExecuteSqlInterpolated($"""
                UPDATE "Vacancies"
                SET "Name" = {name}
                WHERE "Id" = {id.Value};
                """);
        }

        Reload();
        return Vacancies.Last(vacancy => vacancy.Title == name);
    }

    public DemoCompetency SaveCompetency(CompetencyForm form, int? id = null)
    {
        var name = RequiredOrDefault(form.Name, "Новая компетенция");
        var description = NullIfWhiteSpace(form.Description);

        if (id is null)
        {
            _context.Competencies.Add(Competency.Create(name, description));
            _context.SaveChanges();
        }
        else
        {
            _context.Database.ExecuteSqlInterpolated($"""
                UPDATE "Competencies"
                SET "Name" = {name},
                    "Description" = {description}
                WHERE "Id" = {id.Value};
                """);
        }

        Reload();
        return Competencies.Last(competency => competency.Name == name);
    }

    public DemoUser SaveUser(UserForm form, int? id = null)
    {
        var (firstName, lastName) = SplitName(RequiredOrDefault(form.Name, "Новый Пользователь"));
        var role = ParseRole(form.Role);

        if (id is null)
        {
            _context.Users.Add(User.Create(
                firstName,
                lastName,
                "demo-password-hash",
                role,
                DateOnly.FromDateTime(DateTime.Today)));
            _context.SaveChanges();
        }
        else
        {
            var roleValue = role.ToString();
            DateOnly? revokedAt = form.IsActive ? null : DateOnly.FromDateTime(DateTime.Today);
            string? revokedBy = form.IsActive ? null : CurrentUserShort;

            _context.Database.ExecuteSqlInterpolated($"""
                UPDATE "Users"
                SET "FirstName" = {firstName},
                    "LastName" = {lastName},
                    "Role" = {roleValue},
                    "RevokedAt" = {revokedAt},
                    "RevokedBy" = {revokedBy}
                WHERE "Id" = {id.Value};
                """);
        }

        Reload();
        return Users.Last(user => user.Name.StartsWith(firstName, StringComparison.OrdinalIgnoreCase));
    }

    public void SetScore(DemoInterview interview, int competencyId, int score)
    {
        if (!CanEditProtocol(interview))
            return;

        interview.Scores[competencyId] = Math.Clamp(score, 0, 5);
    }

    public void SaveDecision(DemoInterview interview, string decision)
    {
        var dbDecision = ToDbDecision(decision);
        var deciderId = _context.Users
            .AsNoTracking()
            .Where(user => user.Role == UserRole.Decider)
            .Select(user => user.Id)
            .FirstOrDefault();

        if (deciderId == 0)
            deciderId = _context.Users.AsNoTracking().Select(user => user.Id).First();

        _context.Database.ExecuteSqlInterpolated($"""
            INSERT INTO "Verdicts" ("InterviewId", "UserId", "Decision", "Comment")
            VALUES ({interview.DbId}, {deciderId}, {dbDecision}, NULL)
            ON CONFLICT ("InterviewId", "UserId")
            DO UPDATE SET "Decision" = EXCLUDED."Decision";
            """);

        var completed = InterviewStatus.Completed.ToString();
        _context.Database.ExecuteSqlInterpolated($"""
            UPDATE "Interviews"
            SET "Status" = {completed}
            WHERE "Id" = {interview.DbId};
            """);

        Reload();
    }

    public static CandidateForm ToForm(DemoCandidate candidate) => new()
    {
        FullName = candidate.Name,
        City = candidate.City,
        Phone = candidate.Phone,
        Email = candidate.Email,
        Telegram = candidate.Telegram,
        Education = candidate.Education,
        Experience = candidate.Experience,
        Skills = string.Join(", ", candidate.Skills),
        Vacancy = candidate.Responses.FirstOrDefault()?.Vacancy ?? string.Empty
    };

    public static VacancyForm ToForm(DemoVacancy vacancy) => new()
    {
        Title = vacancy.Title,
        Department = vacancy.Department,
        Status = vacancy.Status,
        Description = vacancy.Description,
        IsActive = vacancy.IsActive
    };

    public static CompetencyForm ToForm(DemoCompetency competency) => new()
    {
        Name = competency.Name,
        Description = competency.Description,
        IsActive = competency.IsActive
    };

    public static UserForm ToForm(DemoUser user) => new()
    {
        Name = user.Name,
        Role = user.Role,
        Email = user.Email,
        IsActive = user.IsActive
    };

    public static string DisplayDate(DateTime date) => date.ToString("dd.MM.yyyy");

    public static string CandidateDisplayStatus(CandidateStatus status) => status switch
    {
        CandidateStatus.Hired => "Принят",
        CandidateStatus.Archived => "Отклонён",
        _ => "В поиске"
    };

    public static string InterviewDisplayStatus(InterviewResponse interview) =>
        interview.Status switch
        {
            InterviewStatus.Scheduled => "Собеседование запланировано",
            InterviewStatus.WaitingForVerdict => "На согласовании",
            InterviewStatus.Canceled => "Отменён",
            InterviewStatus.Completed => interview.Decision switch
            {
                DeciderVerdict.Hired => "Принять",
                DeciderVerdict.NextStage => "Следующий этап",
                DeciderVerdict.Rejected => "Отказать",
                _ => "Согласовано"
            },
            _ => "Новый"
        };

    public static bool CanScheduleInterviewInProcess(
        ApplicationProcessResponse process,
        InterviewResponse interview)
    {
        if (process.Interviews.Count == 0)
            return false;

        var latest = process.Interviews.MaxBy(item => item.Date);
        if (latest is null || interview.Id != latest.Id)
            return false;

        if (interview.Status == InterviewStatus.Canceled)
            return true;

        return interview.Decision == DeciderVerdict.NextStage;
    }

    public static string MonthName(DateTime date) => date.Month switch
    {
        1 => "янв",
        2 => "фев",
        3 => "мар",
        4 => "апр",
        5 => "май",
        6 => "июн",
        7 => "июл",
        8 => "авг",
        9 => "сен",
        10 => "окт",
        11 => "ноя",
        _ => "дек"
    };

    private DemoInterview MapInterview(Interview interview) => new()
    {
        Id = interview.Id.ToString(),
        DbId = interview.Id,
        DbStatus = interview.Status,
        CandidateId = interview.CandidateId,
        VacancyId = interview.VacancyId,
        HrId = interview.HrId,
        Vacancy = interview.Vacancy?.Name ?? $"Вакансия #{interview.VacancyId}",
        Date = interview.Date.Date,
        Time = interview.Date.ToString("HH:mm"),
        Format = string.Empty,
        Hr = ToShortName($"{interview.Hr.FirstName} {interview.Hr.LastName}"),
        Approver = Users.FirstOrDefault(user => user.Role == DeciderRole)?.ShortName ?? "Иван Р.",
        Scores = interview.MatrixRows.ToDictionary(row => row.CompetencyId, row => row.Score),
        ScoreComments = interview.MatrixRows.ToDictionary(row => row.CompetencyId, row => row.Comment),
        Comment = interview.SummaryComment ?? string.Empty,
        Decision = ToUiDecision(interview.Verdict?.Decision),
        Status = ToUiStatus(interview.Status, interview.Verdict?.Decision)
    };

    private static DemoCandidate MapCandidate(Candidate candidate, IReadOnlyCollection<Interview> interviews)
    {
        var candidateInterviews = interviews
            .Where(interview => interview.CandidateId == candidate.Id)
            .OrderByDescending(interview => interview.Date)
            .ToList();

        return new DemoCandidate
        {
            Id = candidate.Id,
            Name = candidate.FullName,
            City = candidate.City,
            Phone = candidate.Phone,
            Email = candidate.Email,
            Telegram = candidate.Telegram ?? string.Empty,
            Education = string.Join("; ", candidate.Education ?? []),
            Experience = string.Join("; ", candidate.PreviousWork ?? []),
            Skills = (candidate.Skills ?? []).ToList(),
            Status = ToUiCandidateStatus(candidate.Status),
            Responses = candidateInterviews.Select(interview => new DemoResponse
            {
                Id = interview.ProcessId.ToString("N"),
                Vacancy = interview.Vacancy?.Name ?? $"Вакансия #{interview.VacancyId}",
                Date = interview.Date.Date,
                InterviewId = interview.Id.ToString()
            }).ToList()
        };
    }

    private static DemoUser MapUser(User user)
    {
        var name = $"{user.FirstName} {user.LastName}";
        return new DemoUser
        {
            Id = user.Id,
            Name = name,
            ShortName = ToShortName(name),
            Role = ToUiRole(user.Role),
            Email = $"user-{user.Id}@company.local",
            IsActive = user.RevokedAt is null
        };
    }

    private int FindVacancyId(string title)
    {
        var vacancy = _context.Vacancies.FirstOrDefault(vacancy => vacancy.Name == title)
            ?? _context.Vacancies.OrderBy(vacancy => vacancy.Id).First();

        return vacancy.Id;
    }

    private static string ToUiCandidateStatus(CandidateStatus status) => status switch
    {
        CandidateStatus.Hired => "Принят",
        CandidateStatus.Archived => "Отклонён",
        _ => "Новый"
    };

    private static string ToUiStatus(InterviewStatus status, DeciderVerdict? decision) => decision switch
    {
        DeciderVerdict.Hired => "Принят",
        DeciderVerdict.NextStage => "Собеседование запланировано",
        DeciderVerdict.Rejected => "Отклонён",
        _ => status switch
        {
            InterviewStatus.Scheduled => "Собеседование запланировано",
            InterviewStatus.WaitingForVerdict => "На согласовании",
            InterviewStatus.Canceled => "Отменён",
            InterviewStatus.Completed => "На согласовании",
            _ => "Новый"
        }
    };

    private static string? ToUiDecision(DeciderVerdict? decision) => decision switch
    {
        DeciderVerdict.Hired => "Принять",
        DeciderVerdict.NextStage => "Следующий этап",
        DeciderVerdict.Rejected => "Отказать",
        _ => null
    };

    private static string ToDbDecision(string decision) => decision switch
    {
        "Принять" => DeciderVerdict.Hired.ToString(),
        "Следующий этап" => DeciderVerdict.NextStage.ToString(),
        _ => DeciderVerdict.Rejected.ToString()
    };

    private static UserRole ParseRole(string role) => role switch
    {
        AdminRole => UserRole.Admin,
        DeciderRole => UserRole.Decider,
        _ => UserRole.HR
    };

    private static string ToUiRole(UserRole role) => role switch
    {
        UserRole.Admin => AdminRole,
        UserRole.Decider => DeciderRole,
        _ => HrRole
    };

    private static string RequiredOrDefault(string value, string fallback) =>
        string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();

    private static string? NullIfWhiteSpace(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string[] SplitArray(string value) =>
        value
            .Split([';', '\n'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

    private static string[] SplitTags(string value) =>
        value
            .Split([',', ';', '\n'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

    private static DateTime ParseDateTime(string dateValue, string timeValue)
    {
        var date = DateTime.TryParse(dateValue, out var parsedDate)
            ? parsedDate.Date
            : DateTime.Today.AddDays(1);

        if (TimeSpan.TryParse(timeValue, out var time))
            date = date.Add(time);

        return date;
    }

    private static DateTime EnsureFutureUtc(DateTime date)
    {
        var utcDate = date.Kind switch
        {
            DateTimeKind.Utc => date,
            DateTimeKind.Local => date.ToUniversalTime(),
            _ => DateTime.SpecifyKind(date, DateTimeKind.Utc)
        };

        return utcDate <= DateTime.UtcNow
            ? DateTime.UtcNow.AddDays(1)
            : utcDate;
    }

    private static (string FirstName, string LastName) SplitName(string fullName)
    {
        var parts = fullName.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return ("Новый", "Пользователь");
        if (parts.Length == 1)
            return (parts[0], "Пользователь");

        return (parts[0], string.Join(' ', parts.Skip(1)));
    }

    private static string ToShortName(string fullName)
    {
        var parts = fullName.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return "—";
        if (parts.Length == 1)
            return parts[0];

        return $"{parts[0]} {parts[1][0]}.";
    }
}

public sealed class DemoCandidate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telegram { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public string Experience { get; set; } = string.Empty;
    public string Status { get; set; } = "Новый";
    public List<string> Skills { get; set; } = [];
    public List<DemoResponse> Responses { get; set; } = [];
}

public sealed class DemoResponse
{
    public string Id { get; set; } = string.Empty;
    public string Vacancy { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? InterviewId { get; set; }
}

public sealed class DemoInterview
{
    public string Id { get; set; } = string.Empty;
    public int DbId { get; set; }
    public InterviewStatus DbStatus { get; set; }
    public int CandidateId { get; set; }
    public int VacancyId { get; set; }
    public int HrId { get; set; }
    public string Vacancy { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Time { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public string Hr { get; set; } = string.Empty;
    public string Approver { get; set; } = string.Empty;
    public Dictionary<int, int> Scores { get; set; } = [];
    public Dictionary<int, string?> ScoreComments { get; set; } = [];
    public string Comment { get; set; } = string.Empty;
    public string? Decision { get; set; }
    public string Status { get; set; } = "Новый";
}

public sealed class DemoVacancy
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public HashSet<int> CompetencyIds { get; set; } = [];
}

public sealed class DemoCompetency
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public sealed class DemoUser
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public sealed class CandidateForm
{
    [Required] public string FullName { get; set; } = string.Empty;
    [Required] public string City { get; set; } = "Казань";
    [Required] public string Phone { get; set; } = string.Empty;
    [Required] public string Email { get; set; } = string.Empty;
    public string Telegram { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public string Experience { get; set; } = string.Empty;
    public string Skills { get; set; } = string.Empty;

    //[TODO:] - убрать вакансию
    public string Vacancy { get; set; } = string.Empty;
}

public sealed class InterviewForm
{
    public int CandidateId { get; set; } = 1;
    public string Vacancy { get; set; } = "Junior Developer";
    public string Date { get; set; } = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
    public string Time { get; set; } = "10:00";
    public string Format { get; set; } = "Онлайн";
    public string Hr { get; set; } = "Елена П.";
    public string Approver { get; set; } = "Иван Р.";
}

public sealed class VacancyForm
{
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = "Разработка";
    public string Status { get; set; } = "Открыта";
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public sealed class CompetencyForm
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public sealed class UserForm
{
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = SpectrumDemoState.HrRole;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
