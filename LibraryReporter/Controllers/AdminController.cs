using Enums.Users;
using LibraryReporter.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using LibraryReporter.Controllers.AuthAttributes;
using LibraryReporter.Models.Admin;
using LibraryReporter.Services;

namespace LibraryReporter.Controllers
{
    [IsAdmin]
    public class AdminController : Controller
    {
        private IUserRepositryReal _userRepositryReal;
        private EnumHelper _enumHelper;

        public AdminController(
            IUserRepositryReal userRepositryReal, 
            EnumHelper enumHelper)
        {
            _userRepositryReal = userRepositryReal;
            _enumHelper = enumHelper;
        }

        public IActionResult Users()
        {
            var users = _userRepositryReal
                .GetAll()
                .Select(x => new UserViewModel
                {
                    Id = x.Id,
                    Name = x.Login,
                    Roles = _enumHelper.GetNames(x.Role)
                })
                .ToList();

            var viewModel = new AdminUserViewModel();
            viewModel.Users = users;

            viewModel.Roles = _enumHelper.GetSelectListItems<Role>();

            return View(viewModel);
        }

        public IActionResult UpdateRole(Role role, int userId)
        {
            _userRepositryReal.UpdateRole(userId, role);
            return RedirectToAction("Users");
        }
    }
}
