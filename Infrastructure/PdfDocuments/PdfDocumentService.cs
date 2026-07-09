using Application.PdfDocuments;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.PdfDocuments;

internal sealed class PdfDocumentService : IPdfDocumentService
{
    private const string Ink = "#10243E";
    private const string Teal = "#087F78";
    private const string Pale = "#EAF6F4";
    private const string Line = "#DCE5E8";

    public byte[] CreateInterviewProtocol(InterviewProtocolModel interview) =>
        Document.Create(document => document.Page(page =>
        {
            ConfigurePage(page);
            page.Header().Element(container => Header(container, "Протокол собеседования", interview.Vacancy));
            page.Content().PaddingVertical(18).Column(column =>
            {
                column.Spacing(14);

                var metadata = new List<(string Label, string Value)>
                {
                    ("Кандидат", interview.CandidateName),
                    ("Дата и время (UTC)", interview.DateUtc.ToString("dd.MM.yyyy HH:mm")),
                    ("HR", interview.Hr),
                    ("Статус", interview.Status)
                };

                if (!string.IsNullOrWhiteSpace(interview.Approver))
                    metadata.Add(("Согласующий", interview.Approver));

                column.Item().Element(container => Metadata(container, metadata));

                column.Item().Text("Матрица компетенций").FontSize(15).SemiBold().FontColor(Ink);
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.ConstantColumn(55);
                        columns.RelativeColumn(4);
                    });
                    table.Header(header =>
                    {
                        Cell(header.Cell()).Text("Компетенция").SemiBold();
                        Cell(header.Cell()).AlignCenter().Text("Оценка").SemiBold();
                        Cell(header.Cell()).Text("Комментарий").SemiBold();
                    });
                    foreach (var competency in interview.Competencies)
                    {
                        Cell(table.Cell()).Text(competency.Name).SemiBold();
                        Cell(table.Cell()).AlignCenter().Text(competency.Score == 0 ? "—" : $"{competency.Score}/5").FontColor(Teal).SemiBold();
                        Cell(table.Cell()).Text(string.IsNullOrWhiteSpace(competency.Comment) ? "—" : competency.Comment);
                    }
                });

                column.Item().Element(container => Note(container, "Комментарий HR", interview.HrComment));
                column.Item().Element(container => Note(container, "Решение", FormatDecision(interview.Decision, interview.VerdictComment)));
            });
            page.Footer().Element(Footer);
        })).GeneratePdf();

    public byte[] CreateCandidateCard(CandidateCardModel candidate) =>
        Document.Create(document => document.Page(page =>
        {
            ConfigurePage(page);
            page.Header().Element(container => Header(container, "Карточка кандидата", candidate.FullName));
            page.Content().PaddingVertical(18).Column(column =>
            {
                column.Spacing(14);
                column.Item().Element(container => Metadata(container,
                [
                    ("Телефон", candidate.Phone),
                    ("Email", candidate.Email),
                    ("Город", candidate.City),
                    ("Telegram", candidate.Telegram ?? "—"),
                    ("Статус", candidate.Status),
                    ("№", candidate.Id.ToString("0000"))
                ]));
                column.Item().Element(container => Note(container, "Образование", candidate.Education));
                column.Item().Element(container => Note(container, "Опыт работы", candidate.PreviousWork));

                if (candidate.Skills.Count > 0)
                {
                    column.Item().Text("Навыки").FontSize(15).SemiBold().FontColor(Ink);
                    column.Item().Text(string.Join(" · ", candidate.Skills)).FontSize(9).LineHeight(1.4f);
                }

                column.Item().Text("Отклики и собеседования").FontSize(15).SemiBold().FontColor(Ink);
                if (candidate.Processes.Count == 0)
                {
                    column.Item().Text("Откликов пока нет").FontSize(9).FontColor(Colors.Grey.Darken1);
                }

                foreach (var process in candidate.Processes)
                {
                    column.Item().Border(1).BorderColor(Line).Padding(12).Column(card =>
                    {
                        card.Item().Text(process.VacancyName).SemiBold().FontColor(Ink);
                        foreach (var item in process.Interviews)
                        {
                            card.Item().PaddingTop(6).Row(row =>
                            {
                                row.RelativeItem().Text($"{item.DateUtc:dd.MM.yyyy HH:mm} (UTC)").FontSize(9);
                                row.AutoItem().Text(item.Status).FontColor(Teal).SemiBold().FontSize(9);
                            });
                        }
                    });
                }
            });
            page.Footer().Element(Footer);
        })).GeneratePdf();

    public byte[] CreateApplicationLetter(ApplicationLetterModel letter, LetterType type)
    {
        var invitation = type == LetterType.Invitation;
        var title = invitation ? "Приглашение" : "Уведомление по отклику";
        var body = invitation
            ? $"Приглашаем вас продолжить процесс отбора на вакансию «{letter.Vacancy}». Представитель отдела кадров свяжется с вами для согласования даты и формата следующего этапа."
            : $"Благодарим за интерес к вакансии «{letter.Vacancy}». По результатам рассмотрения мы не готовы продолжить процесс отбора по этому отклику.";

        return Document.Create(document => document.Page(page =>
        {
            ConfigurePage(page);
            page.Header().Element(container => Header(container, title, "Спектр · подбор и оценка"));
            page.Content().PaddingVertical(28).Column(column =>
            {
                column.Spacing(18);
                column.Item().Text($"Здравствуйте, {letter.CandidateName}!").FontSize(16).SemiBold().FontColor(Ink);
                column.Item().Text(body).FontSize(11).LineHeight(1.5f);
                column.Item().PaddingTop(18).Text("С уважением,\nкоманда по подбору персонала").FontSize(10);
            });
            page.Footer().Element(Footer);
        })).GeneratePdf();
    }

    private static string FormatDecision(string? decision, string? verdictComment)
    {
        if (string.IsNullOrWhiteSpace(decision))
            return "Решение не принято";

        return string.IsNullOrWhiteSpace(verdictComment)
            ? decision
            : $"{decision}. {verdictComment}";
    }

    private static void ConfigurePage(PageDescriptor page)
    {
        page.Size(PageSizes.A4);
        page.Margin(34);
        page.DefaultTextStyle(style => style.FontFamily("Lato").FontSize(10).FontColor(Ink));
    }

    private static void Header(IContainer container, string eyebrow, string title) =>
        container.BorderBottom(2).BorderColor(Ink).PaddingBottom(14).Row(row =>
        {
            row.ConstantItem(52).Height(52).Border(1).BorderColor(Teal).AlignCenter().AlignMiddle()
                .Text("СП").FontSize(18).Bold().FontColor(Teal);
            row.RelativeItem().PaddingLeft(14).Column(column =>
            {
                column.Item().Text(eyebrow.ToUpperInvariant()).FontSize(8).LetterSpacing(0.12f).FontColor(Teal);
                column.Item().PaddingTop(4).Text(title).FontSize(20).SemiBold().FontColor(Ink);
            });
        });

    private static void Metadata(IContainer container, IEnumerable<(string Label, string Value)> items) =>
        container.Background(Pale).Padding(12).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });
            foreach (var (label, value) in items)
            {
                table.Cell().Padding(6).Column(column =>
                {
                    column.Item().Text(label.ToUpperInvariant()).FontSize(7).LetterSpacing(0.08f).FontColor(Colors.Grey.Darken1);
                    column.Item().PaddingTop(3).Text(value).FontSize(9).SemiBold();
                });
            }
        });

    private static void Note(IContainer container, string title, string? value) =>
        container.Border(1).BorderColor(Line).Padding(12).Column(column =>
        {
            column.Item().Text(title).FontSize(9).SemiBold().FontColor(Teal);
            column.Item().PaddingTop(5).Text(string.IsNullOrWhiteSpace(value) ? "—" : value).LineHeight(1.35f);
        });

    private static IContainer Cell(IContainer container) => container.BorderBottom(1).BorderColor(Line).Padding(8);

    private static void Footer(IContainer container) => container.AlignCenter().Text(text =>
    {
        text.Span("Спектр · ").FontColor(Teal);
        text.CurrentPageNumber();
        text.Span(" / ");
        text.TotalPages();
    });
}
