using Application.PdfDocuments;
using Infrastructure.PdfDocuments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;

namespace Infrastructure;

/*
 * EF Core должен знать, какой проект является точкой входа для миграций. 
 * Запускайте команды из проекта Web, указывая проект с контекстом:
 * 
 * Примеры:
 * dotnet ef migrations add InitialCreate --project ../Infrastructure/Infrastructure.csproj --startup-project Web.csproj
 * dotnet ef database update --project ../Infrastructure/Infrastructure.csproj --startup-project Web.csproj
 */

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BarsContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        QuestPDF.Settings.License = LicenseType.Community;
        services.AddSingleton<IPdfDocumentService, PdfDocumentService>();

        return services;
    }
}
