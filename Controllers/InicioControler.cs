﻿using Microsoft.AspNetCore.Mvc;

using ProyectoLogin.Models;
using ProyectoLogin.Recursos;
using ProyectoLogin.Servicios.Contrato;


using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;


namespace ProyectoLogin.Controllers
{
    public class InicioControler : Controller
    {
        private readonly IUsuarioService _usuarioServicio;

        public InicioControler(IUsuarioService usuarioServicio)
        {
            _usuarioServicio = usuarioServicio;

        }


        public IActionResult Registrarse()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registrarse(Usuario modelo)
        {
            modelo.Clave = Utilidades.EncriptarClave(clave: modelo.Clave);

            Usuario usuario_creado = await _usuarioServicio.SaveUsuario(modelo);

            if (usuario_creado.IdUsuario > 0)
                return RedirectToAction("IniciarSesion", "Inicio");

            ViewData["Mensaje"] = "No se pudo crear el usuario";
            return View();
        }
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(string correo, string clave, Usuario usuario_encontrado, Usuario usuario, string? nombreUsuario)
        {

            if (await _usuarioServicio.GetUsuario(correo, Utilidades.EncriptarClave(clave)) == null) {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }

#pragma warning disable CS8604 // Posible argumento de referencia nulo
            List<Claim> claims = new List<Claim>() {
                new(ClaimTypes.Name, value: nombreUsuario)
            };
#pragma warning restore CS8604 // Posible argumento de referencia nulo

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
                );


            return RedirectToAction("Index", "Home");
        }
    }
}