using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.Areas.Identity.Data;
using App.Entities;
using System.IO.Compression;
using App.Models;

namespace App.Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var eventList = new List<EventModel>();
            var events = await _context.Events.ToListAsync();
            foreach (var item in events)
            {
                eventList.Add(new EventModel
                {
                    EventID = item.EventID,
                    ClosureDate = item.ClosureDate,
                    EventDecription = item.EventDecription,
                    EventName = item.EventName,
                    OpenDate = item.OpenDate,
                    Posts = item.Posts,
                    preClosureDate = item.preClosureDate,
                    HoursRemain = (item.ClosureDate - DateTime.Now).TotalDays
                });
            }
            return _context.Events != null ? 
                          View(eventList) :
                          Problem("Entity set 'ApplicationDbContext.Events'  is null.");
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.Include(p=>p.Posts)
                .FirstOrDefaultAsync(m => m.EventID == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventID,EventName,EventDecription,OpenDate,preClosureDate,ClosureDate")] Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventID,EventName,EventDecription,OpenDate,preClosureDate,ClosureDate")] Event @event)
        {
            if (id != @event.EventID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.EventID == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Events == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Events'  is null.");
            }
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
          return (_context.Events?.Any(e => e.EventID == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> DownloadAsZip(int eventId)
        {
            // Get the event and its posts from the database
            var eventEntity = _context.Events
                .Include(e => e.Posts)
                .FirstOrDefault(e => e.EventID == eventId);

            if (eventEntity == null)
            {
                return NotFound();
            }

            // Create a temporary folder to store the idea files
            var tempFolderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolderPath);

            // Save each idea as a separate text file in the temporary folder
            foreach (var post in eventEntity.Posts)
            {
                var postContent = $"Title: {post.Title}\n" +
                  $"Brief: {post.Brief}\n" +
                  $"View: {post.View}\n" +
                  $"Like: {post.Like}\n" +
                  $"Dislike: {post.Dislike}\n";

                // Write the post content to a text file
                var postFilePath = Path.Combine(tempFolderPath, $"{post.Title}.txt");
                System.IO.File.WriteAllText(postFilePath, postContent);
            }

            // Create a ZIP archive from the temporary folder
            var zipFileName = $"{eventEntity.EventName}.zip";
            var zipFilePath = Path.Combine(Path.GetTempPath(), zipFileName);
            ZipFile.CreateFromDirectory(tempFolderPath, zipFilePath);

            // Return the ZIP file for download
            var fileBytes = System.IO.File.ReadAllBytes(zipFilePath);
            var contentType = "application/zip";
            var fileContentResult = new FileContentResult(fileBytes, contentType)
            {
                FileDownloadName = zipFileName
            };

            // Clean up the temporary folder and ZIP file
            Directory.Delete(tempFolderPath, true);
            System.IO.File.Delete(zipFilePath);

            return fileContentResult;
        }

        public async Task<IActionResult> DownloadAsExcel(int eventId)
        {
            // Get the event and its posts from the database
            var eventEntity = _context.Events
                .Include(e => e.Posts)
                .FirstOrDefault(e => e.EventID == eventId);

            if (eventEntity == null)
            {
                return NotFound();
            }

            // Create a new Excel workbook and worksheet
            var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Posts");

            // Set the column headers
            worksheet.Cell(1, 1).Value = "Title";
            worksheet.Cell(1, 2).Value = "Brief";
            worksheet.Cell(1, 3).Value = "View";
            worksheet.Cell(1, 4).Value = "Like";
            worksheet.Cell(1, 5).Value = "Dislike";

            // Populate the worksheet with the data from the event's posts
            for (int i = 0; i < eventEntity.Posts.Count; i++)
            {
                var post = eventEntity.Posts[i];
                worksheet.Cell(i + 2, 1).Value = post.Title;
                worksheet.Cell(i + 2, 2).Value = post.Brief;
                worksheet.Cell(i + 2, 3).Value = post.View;
                worksheet.Cell(i + 2, 4).Value = post.Like;
                worksheet.Cell(i + 2, 5).Value = post.Dislike;
            }

            // Set the response headers to indicate that the response should be an Excel file
            var contentDisposition = new System.Net.Mime.ContentDisposition
            {
                FileName = $"{eventEntity.EventName}.xlsx",
                Inline = false
            };
            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            // Write the Excel file to the response stream
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(Response.Body);
            }

            return new EmptyResult();
        }

    }
}
