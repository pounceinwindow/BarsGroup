using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;

namespace PdfDocuments;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPdfDocuments(this IServiceCollection services)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        services.AddSingleton<IPdfDocumentService, PdfDocumentService>();
        return services;
    }
}
