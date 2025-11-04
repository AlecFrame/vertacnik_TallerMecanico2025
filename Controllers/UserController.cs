using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using vertacnik_TallerMecanico2025.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;


namespace vertacnik_TallerMecanico2025.Controllers;

public class UserController : Controller
{
    private readonly UsuarioRepo _usuarioRepo;

    public UserController(UsuarioRepo usuarioRepo)
    {
        _usuarioRepo = usuarioRepo;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string email, string password)
    {
        var usuario = _usuarioRepo.AuthenticateUser(email, password);
        if (usuario != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.NombreCompleto),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
                new Claim("IdUsuario", usuario.IdUsuario.ToString()),
                new Claim("NombreCompleto", usuario.NombreCompleto),
                new Claim("DNI", usuario.Dni),
                new Claim("Rol", usuario.Rol.ToString()),
                new Claim("Email", usuario.Email),
                new Claim("Telefono", usuario.Telefono),
                new Claim("Avatar", usuario.Avatar ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(25)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
        return RedirectToAction("Login", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Home");
    }

    // Actions for UserController go here
}