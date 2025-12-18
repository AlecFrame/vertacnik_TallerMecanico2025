using vertacnik_TallerMecanico2025.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<UsuarioRepo>();
builder.Services.AddScoped<ClienteRepo>();
builder.Services.AddScoped<VehiculoRepo>();
builder.Services.AddScoped<RepuestoRepo>();
builder.Services.AddScoped<TipoServicioRepo>();
builder.Services.AddScoped<PedidoRepo>();
builder.Services.AddScoped<ServicioRepo>();
builder.Services.AddScoped<DetalleRepuestoRepo>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
        options.LogoutPath = "/User/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Home/Restringido";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Administrador"));
    options.AddPolicy("MecanicOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Mecanico", "Administrador"));
    options.AddPolicy("RecepcionOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Recepcionista", "Administrador"));
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
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
