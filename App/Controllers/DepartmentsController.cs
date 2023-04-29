using App.Areas.Identity.Data;
using App.Entities;
using App.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace App.Web.Controllers
{

    //TODO: Create DepartmentController
    //TODO: Create Models in Models folder
    //TODO: Create Views (refer the Users Page)

    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        //private readonly IToastNotification _toastNotification;
        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var data = _context.Departments.AsAsyncEnumerable();
            return View(data);
        }


        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddDepartmentView context)
        {
            try
            {
                var newDepartment = new Department
                {
                    
                    DepartmentName = context.DepartmentName

                };
                _context.Departments.Add(newDepartment);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int Id)
        {
            //here, get the roles from the database in the real application

            //getting a role from collection for demo purpose

            var depart = _context.Departments.Where(x => x.DepartmentID == Id).FirstOrDefault();

            return View(depart);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Department department)
        {
            //update role in DB using EntityFramework in real-life application

            var departt = _context.Departments.Where(x => x.DepartmentID == department.DepartmentID).FirstOrDefault();

 
            departt.DepartmentName = department.DepartmentName;

            _context.Departments.Update(departt);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Departments == null)
            {
                return NotFound();
            }

            var @depart = await _context.Departments
                .FirstOrDefaultAsync(m => m.DepartmentID == id);
            if (@depart == null)
            {
                return NotFound();
            }

            return View(@depart);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Departments == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Departments'  is null.");
            }
            var @depart = await _context.Departments.FindAsync(id);
            if (@depart != null)
            {
                _context.Departments.Remove(@depart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
