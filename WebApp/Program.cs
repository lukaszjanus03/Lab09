using Microsoft.EntityFrameworkCore;
using WebApp.Models.Movies;
using Microsoft.Extensions.Logging; 
using Microsoft.AspNetCore.Authentication.Cookies; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MoviesDbContext>(options =>
    options.UseSqlite("data source=C:\\data\\movies.db") 
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging());

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login"; 
    });

builder.Services.AddAuthorization();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();