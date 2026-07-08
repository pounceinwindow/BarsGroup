using Application;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddApplication();
// База данных
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Применение миграций при запуске
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<Infrastructure.BarsContext>();
    dbContext.Database.Migrate();

    /*
     * "localhost/database" - для очистки и добавления тестовых данных в бд
     * [TODO: Андрей] - вынести это в Swagger
     */
}

app.Run();
