using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Westwind.AspNetCore.Markdown;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddDataAccess()
    .AddDomainServices()
    .AddMarkdown()
    .AddControllersWithViews();

var app = builder.Build();

app.UseMarkdown();

app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors(policy =>
{
    policy.WithOrigins("http://localhost:5186") // Replace with your allowed origin(s)
          .AllowAnyHeader()
          .AllowAnyMethod();
});

app.UseRouting();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
