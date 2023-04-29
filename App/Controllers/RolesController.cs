
using App.Areas.Identity.Data;
using App.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = nameof(Enums.Roles.Admin))]
        public IActionResult Index()
        {
            var data = _context.Roles.ToList();
            return View(data);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IdentityRole role)
        {
            _context.Roles.Add(role);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(string Id)
        {
            //here, get the roles from the database in the real application

            //getting a role from collection for demo purpose
            var role = _context.Roles.Where(s => s.Id == Id).FirstOrDefault();

            return View(role);
        }

        public ActionResult Delete(string Id)
        {
            //here, get the roles from the database in the real application

            //getting a role from collection for demo purpose
            var existrole = _context.Roles.FirstOrDefault(s => s.Id == Id);
            if (existrole != null)
            {
                _context.Roles.Remove(existrole);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(Roles role)
        {
            //update role in DB using EntityFramework in real-life application

            //update list by removing old role and adding updated role for demo purpose
            //var existrole = _context.Roles.FirstOrDefault(s => s.Id == role.Id);
            //if (existrole != null)
            //{

            //    _context.Roles.Update(existrole);
            //    _context.SaveChanges();
            //}

            return RedirectToAction("Index");
        }
    }
}
