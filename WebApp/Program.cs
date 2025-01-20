using Microsoft.EntityFrameworkCore;
using WebApp.Models.Movies;

var builder = WebApplication.CreateBuilder(args);

// Dodaj logowanie SQL podczas konfiguracji DbContext
builder.Services.AddDbContext<MoviesDbContext>(options =>
    options.UseSqlite("data source=C:\\data\\movies.db") // Ścieżka do Twojej bazy danych SQLite
        .LogTo(Console.WriteLine, LogLevel.Information) // Logowanie zapytań SQL do konsoli
        .EnableSensitiveDataLogging()); // Opcjonalne: loguje wartości parametrów (tylko do debugowania!)

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();