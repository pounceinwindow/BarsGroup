using APIGateaway.API.Configuration;

var builder = WebApplication.CreateBuilder();

builder.ConfigureDependencies();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();
