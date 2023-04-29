using App.Areas.Identity.Data;
using App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace App.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Posts.Include(p => p.Event);
            return View(await applicationDbContext.ToListAsync());

        }
        public async Task<IActionResult> Details(int? id)
        {
            return RedirectToAction("Details", "Posts", new { id = id });
        }
        public async Task<IActionResult> Edit(int? id)
        {
            return RedirectToAction("Edit", "Posts", new { id = id });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Statics()
        {
            if (_context.Events == null)
            {
                return NotFound();
            }

            var events = await _context.Events
                .Include(e => e.Posts)
                .ThenInclude(p => p.Comments)
                .ToListAsync();

            foreach (var @event in events)
            {
                foreach (var post in @event.Posts)
                {
                    post.View++;
                    _context.Update(post);
                }
            }

            await _context.SaveChangesAsync();

            return View(events);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }


}