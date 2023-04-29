
using App.Areas.Identity.Data;
using App.Core;
using App.Models;
using App.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace App.Web.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IToastNotification _toastNotification;
        public UsersController(
            ApplicationDbContext context, 
            UserManager<User> userManager,
            IToastNotification toastNotification)
        {
            _context = context;
            _userManager = userManager;
            _toastNotification = toastNotification;
        }
        [Authorize(Roles = $"{nameof(Enums.Roles.Admin)},{nameof(Enums.Roles.Manager)}")]
        public IActionResult Index()
        {
            var data = _context.Users.ToList();
            return View(data);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
            try
            {
                var newUser = new User
                {
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                    Email = model.Email,
                    UserName = model.Email,
                    Genre = model.Genre,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };
                await _userManager.CreateAsync(newUser, model.Pwd);
                _toastNotification.AddSuccessToastMessage($"Create a user {newUser.Email} successfully!");
            }
            catch (Exception e)
            {

                _toastNotification.AddErrorToastMessage("Create a user was failed!");
            }
            
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(string Id)
        {
            //here, get the users from the database in the real application

            //getting a user from collection for demo purpose
            var user = await _context.Users.Where(s => s.Id == Id).FirstOrDefaultAsync();
            if (user == null)
            {
                return RedirectToAction("Index");
            }
            var roles = await _context.Roles.ToListAsync();

            var userRoles = await _context.UserRoles.Where(x => x.UserId == user.Id).ToListAsync();

            var roleItems = roles.Select(s =>
                new SelectListItem(s.Name, s.Id, userRoles.Any(a => a.RoleId.Contains(s.Id)))).ToList();
            var viewModel = new EditUserViewModel
            {
                User = user,
                Roles = roleItems
            };

            return View(viewModel);
        }

        public ActionResult Delete(string Id)
        {
            //here, get the users from the database in the real application

            //getting a user from collection for demo purpose
            var existuser = _context.Users.FirstOrDefault(s => s.Id == Id);
            if (existuser != null)
            {
                _context.Users.Remove(existuser);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            try
            {
                //update user in DB using EntityFramework in real-life application

                //update list by removing old user and adding updated user for demo purpose
                var existuser = _context.Users.FirstOrDefault(s => s.Id == model.User.Id);
                if (existuser != null)
                {
                    existuser.Firstname = model.User.Firstname;
                    existuser.Lastname = model.User.Lastname;
                    _context.Users.Update(existuser);

                    var userroles = await _context.UserRoles.Where(x => x.UserId == existuser.Id).ToListAsync();
                    var rolesToAdd = new List<string>();
                    var rolesToDelete = new List<string>();


                    foreach (var role in model.Roles)
                    {
                        var assignedRole = userroles.FirstOrDefault(x => x.RoleId == role.Value);
                        if (role.Selected)
                        {
                            if (assignedRole == null)
                            {
                                rolesToAdd.Add(role.Text);
                            }
                        }
                        else
                        {
                            if (assignedRole != null)
                            {
                                rolesToDelete.Add(role.Text);
                            }
                        }
                    }
                    if (rolesToAdd.Any())
                    {
                        await _userManager.AddToRolesAsync(existuser, rolesToAdd);
                    }
                    if (rolesToDelete.Any())
                    {
                        await _userManager.RemoveFromRolesAsync(existuser, rolesToDelete);
                    }
                    _context.SaveChanges();
                    _toastNotification.AddSuccessToastMessage("Save Role successfully!");
                }
            }
            catch (Exception e)
            {

                _toastNotification.AddErrorToastMessage("Save Role was failed");
            }

            return RedirectToAction("Index");
        }
    }
}
