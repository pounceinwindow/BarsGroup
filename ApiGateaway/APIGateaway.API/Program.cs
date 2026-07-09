using APIGateaway.API.Configuration;

var builder = WebApplication.CreateBuilder();

builder.ConfigureDependencies();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGatewayEndpoints();
app.MapReverseProxy();

app.Run();
