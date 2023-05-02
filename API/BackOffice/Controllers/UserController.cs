using System;
using BackOffice.Models;
using BackOffice.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BackOffice.Controllers
{
    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;
        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public IActionResult ManageUser()
        {
            ViewData["Menu"] = "User";
            ViewData["SubMenu"] = "Manage Users";
            return View();
        }

        public IActionResult EditUser(UserViewModel userVm)
        {
            ViewData["Menu"] = "User";
            ViewData["SubMenu"] = "Edit User";
            return View(userVm);
        }

        public IActionResult GetUsers()
        {
            return new JsonResult(_userRepository.GetUsers());
        }
    }


}

