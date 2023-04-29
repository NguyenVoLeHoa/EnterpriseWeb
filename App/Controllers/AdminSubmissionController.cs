using App.Areas.Identity.Data;
using App.Entities;
using App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;


namespace App.Web.Controllers
{
    public class AdminSubmissionController : Controller
    {

        private readonly ApplicationDbContext _context;
        //private readonly IToastNotification _toastNotification;
        public AdminSubmissionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var data = _context.Events.AsAsyncEnumerable();
            return View(data);
        }
        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.Include(p => p.Posts)
                .FirstOrDefaultAsync(m => m.EventID == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddAdminSubmissionViewModel context)
        {
            try
            {
                var newEvent = new Event
                {

                    EventName = context.EventName,
                    EventDecription = context.EventDecription,
                    OpenDate = context.OpenDate,
                    preClosureDate = context.preClosureDate,
                    ClosureDate = context.ClosureDate

                };
                _context.Events.Add(newEvent);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {

            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int Id)
        {
            //here, get the event from the database in the real application

            //getting a role from collection for demo purpose

            var depart = _context.Events.Where(x => x.EventID == Id).FirstOrDefault();

            return View(depart);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Event department)
        {
            //update role in DB using EntityFramework in real-life application

            var departt = _context.Events.Where(x => x.EventID == department.EventID).FirstOrDefault();


            departt.EventName = department.EventName;
            departt.OpenDate = department.OpenDate;
            departt.ClosureDate = department.OpenDate;


            _context.Events.Update(departt);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            var @depart = await _context.Events
                .FirstOrDefaultAsync(m => m.EventID == id);
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
            if (_context.Events == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Departments'  is null.");
            }
            var @depart = await _context.Events.FindAsync(id);
            if (@depart != null)
            {
                _context.Events.Remove(@depart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


    }
}
