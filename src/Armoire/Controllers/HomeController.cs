using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Armoire.Models;
using NHibernate;
using Armoire.Common;

namespace Armoire.Controllers
{
    public class HomeController : Controller
    {
        private IUserService _userService;
        private IAutomapperMapping _mapper;

        public HomeController(IUserService userService, IAutomapperMapping mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Login()
        {
            ViewData["Message"] = TempData["Message"];
            return View();
        }

        
        public IActionResult Register()
        {
            ViewData["Message"] = "Registration Page";
            var register = new RegisterViewModel();
            return View(register);
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var dto = _mapper.Map<RegisterViewModel, PatronDto>(viewModel);
                dto = _userService.Register(dto);
                TempData["Message"] = "You've successfully registered, please log in";
                return RedirectToAction("Login");
            } else {
                return View();
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
