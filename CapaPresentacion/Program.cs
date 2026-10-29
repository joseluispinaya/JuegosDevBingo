using CapaData.Configuracion;
using CapaData.Implementacion;
using CapaData.Interfaces;
using CapaPresentacion.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));

// Inyección de dependencias de Repositorios (Scoped)
builder.Services.AddScoped<IPasswordHelper, PasswordHelper>();

builder.Services.AddScoped<ITipoJuegoRepositorio, TipoJuegoRepositorio>();
builder.Services.AddScoped<IFiguraRepositorio, FiguraRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
//builder.Services.AddScoped<IPasswordHelper, PasswordHelper>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(option =>
{
    option.LoginPath = "/Entrar/Login";
    //option.AccessDeniedPath = "/Acceso/Denegado";
    option.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Entrar}/{action=Login}/{id?}");
//pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
