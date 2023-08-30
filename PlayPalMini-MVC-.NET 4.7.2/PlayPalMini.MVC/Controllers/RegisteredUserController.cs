using PlayPalMini.Model;
using PlayPalMini.MVC.Models;
using PlayPalMini.Service;
using PlayPalMini.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PlayPalMini.MVC.Controllers
{
    public class RegisteredUserController : Controller
    {
        public IRegisteredUserService RegisteredUserService { get; set; }
        public RegisteredUserController(IRegisteredUserService service)
        {
            RegisteredUserService = service;
        }

        //-------------SIGN UP / CREATE NEW USER-------------
        [HttpGet]
        public async Task<ActionResult> SignUpAsync()
        {
            RegisteredUserView userView = new RegisteredUserView();
            RegisteredUserDTO userDTO = new RegisteredUserDTO();
            userView.Id = userDTO.Id;
            userView.Username = userDTO.Username;
            userView.Pass = userDTO.Pass;
            userView.UserRole = userDTO.UserRole;
            userView.CreatedBy = userDTO.CreatedBy;
            userView.UpdatedBy = userDTO.UpdatedBy;
            userView.DateCreated = userDTO.DateCreated;
            userView.DateUpdated = userDTO.DateUpdated;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> SignUpAsync(RegisteredUserView userView)
        {
            List<RegisteredUserDTO> check = await RegisteredUserService.GetAllUsersAsync();
            if (check.Any(u => u.Username == userView.Username))
            {
                ModelState.AddModelError("Username", "The username is already taken.");
                return View(userView);
            }

            RegisteredUserDTO userDTO = new RegisteredUserDTO()
            {
                Id = Guid.NewGuid(),
                Username = userView.Username,
                Pass = userView.Pass,
                UserRole = "User",
                CreatedBy = "signup",
                UpdatedBy = "n/a",
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };
            await RegisteredUserService.SignUpAsync(userDTO);
            return View();
        }

        //------------------AUTHENTICATION / LOGIN------------------
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(RegisteredUserView userView)
        {
            RegisteredUserDTO userDTO = await RegisteredUserService.FindOneUserAsync(userView.Username, userView.Pass);
            if (userDTO != null)
            {
                //FormsAuthentication.SetAuthCookie(userDTO.Username, false);

                string userId = userDTO.Id.ToString();

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1, userDTO.Username, DateTime.Now, DateTime.Now.AddMinutes(15), false, userId);
                
                string encryptedTicket = FormsAuthentication.Encrypt(ticket);
                
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                
                HttpContext.Response.Cookies.Add(cookie);

                return RedirectToAction("GetAllBoardGamesAsync","BoardGame");
                
            }
            else
            {
                ModelState.AddModelError("Pass", "Incorrect username or password");
                return View();
            }            
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        //------------------EDIT USER-----------------
        public async Task<ActionResult> EditUserAsync(Guid id)
        {
            RegisteredUserDTO userDTO = await RegisteredUserService.GetUserByIdAsync(id);
            RegisteredUserView userView = new RegisteredUserView();

            userView.Id = userDTO.Id;
            userView.Username = userDTO.Username;
            userView.Pass = userDTO.Pass;
            userView.UserRole = userDTO.UserRole;
            userView.UpdatedBy = userDTO.UpdatedBy;
            userView.DateUpdated = userDTO.DateUpdated;

            return View(userView);
        }
        [HttpPost]
        public async Task<ActionResult> EditUserAsync(RegisteredUserView userView)
        {
            string userName = "";
            if (User.Identity is FormsIdentity identity)
            {
                userName = identity.Name;
            }

            List<RegisteredUserDTO> check = await RegisteredUserService.GetAllUsersAsync();
            if (check.Any(u => u.Username == userView.Username))
            {
                ModelState.AddModelError("Username", "The username is already taken.");
                return View(userView);
            }

            RegisteredUserDTO userDTO = new RegisteredUserDTO();
            userDTO.Id = userView.Id;
            userDTO.Username = userView.Username;
            userDTO.Pass = userView.Pass;
            userDTO.UpdatedBy = userName;
            userDTO.DateUpdated = DateTime.Now;

            if (User.IsInRole("Administrator"))
            {
                userDTO.UserRole = userView.UserRole;
            }
            else
            {
                userDTO.UserRole = "User";
            }

            await RegisteredUserService.EditUserAsync(userDTO.Id, userDTO);
            TempData["ConfirmationMessage"] = "Edited successfully!";
            return View();
        }

        //--------------------GET ALL-------------
        public async Task<ActionResult> GetAllUsersAsync()
        {
            List<RegisteredUserDTO> listDTO = await RegisteredUserService.GetAllUsersAsync();
            List<RegisteredUserView> listView = new List<RegisteredUserView>();
            foreach (RegisteredUserDTO userDTO in listDTO)
            {
                RegisteredUserView userView = new RegisteredUserView();
                userView.Id = userDTO.Id;
                userView.Username = userDTO.Username;
                userView.UserRole = userDTO.UserRole;
                userView.CreatedBy = userDTO.CreatedBy;
                userView.DateCreated = userDTO.DateCreated;
                userView.UpdatedBy = userDTO.UpdatedBy;
                userView.DateUpdated = userDTO.DateUpdated;
                listView.Add(userView);
            }
            return View(listView);
        }
    }
}