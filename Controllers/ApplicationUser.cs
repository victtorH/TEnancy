using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ModuloMVC.Controllers
{

    public class ApplicationUser : Controller
    {
        public readonly UserManager<IdentityUser> _usermanager;
        public readonly SignInManager<IdentityUser> _signinmanager;

        public ApplicationUser(UserManager<IdentityUser> usermanager, SignInManager<IdentityUser> signInManager)
        {
            _usermanager = usermanager;
            _signinmanager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SingIn(string email, string numero, string senha)
        {
            var NewUser = new IdentityUser{ UserName = email, Email = email, PhoneNumber = numero};
            await _usermanager.CreateAsync(NewUser,senha);
            var acesso = await _signinmanager.PasswordSignInAsync(email ,senha,false,false);

                if(acesso.Succeeded)
                return RedirectToAction("Index", "Tarefa");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email ,string senha, bool lembrar = false)
        {
                var acesso = await _signinmanager.PasswordSignInAsync(email ,senha,lembrar,false);
                
                if(acesso.Succeeded)
                return RedirectToAction("Index", "Tarefa");
            
                return RedirectToAction("Index");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}