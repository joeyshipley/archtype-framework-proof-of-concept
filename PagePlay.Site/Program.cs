using PagePlay.Site.Infrastructure.IoC;
using PagePlay.Site.Infrastructure.Routing;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
DependencyResolver.Bind(builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapEndpoints();
app.UseHttpsRedirection();

app.Run();
