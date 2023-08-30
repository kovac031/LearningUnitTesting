using PlayPalMini.Model;
using PlayPalMini.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlayPalMini.MVC.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async ActionResult Login(LoginView loginView)
        {
            RegisteredUserDTO userDTO = await 
        }
        public ActionResult Logout()
        {
            return View();
        }
    }
}