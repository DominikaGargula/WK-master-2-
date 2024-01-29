using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WydarzeniaKulturalne.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WydarzeniaKulturalneContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WydarzeniaKulturalneContext") ?? 
    throw new InvalidOperationException("Connection string 'WydarzeniaKulturalneMVCContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // Sets the default scheme to cookies
    .AddCookie(options =>
    {
        options.LoginPath = "/Uzytkownik/Logowanie";
        options.AccessDeniedPath = "/Errors/Error401";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Uzytkownik",
        policy => policy.RequireClaim("Uzytkownik"));

    options.AddPolicy("Admin",
        policy => policy.RequireClaim("Admin"));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Errors/Error500");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
