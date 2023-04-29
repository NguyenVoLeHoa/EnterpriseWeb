using App.Areas.Identity.Data;
using App.Entities;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Security.Claims;

namespace App.Web.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;
        public PostsController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Posts.Include(p => p.Event);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Event)
                .Include(q => q.Comments)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }
            post.View++;
            _context.Update(post);
            await _context.SaveChangesAsync();
           
            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create(int? Id)
        {
            if (Id == null || _context.Events == null)
            {
                return NotFound();
            }

            var events = new List<Event>();
            Event? eventItem = _context.Events.FirstOrDefault(x => x.EventID == Id);
            if (eventItem == null)
            {
                return NotFound();
            }
            events.Add(eventItem);
            ViewData ["EventId"] = new SelectList(events, "EventID", "EventDecription");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,EventId,Title,Brief")] Post post)
        {
            if (ModelState.IsValid)
            {
                var postExists = _context.Posts.Any(x => x.Title.ToLower() == post.Title.ToLower());
                if (postExists)
                {
                    _toastNotification.AddErrorToastMessage("Title was used in system. Please choose another title ");
                }
                else
                {
                    _context.Add(post);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "Events", new { Id = post.EventId });
                }
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventID", "EventDecription", post.EventId);

            string referer = Request.Headers["Referer"].ToString();

            return Redirect(referer);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventID", "EventDecription", post.EventId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,EventId,Title,Brief,View,Like,Dislike")] Post post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
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
            ViewData["EventId"] = new SelectList(_context.Events, "EventID", "EventDecription", post.EventId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Event)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Posts'  is null.");
            }
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return (_context.Posts?.Any(e => e.PostId == id)).GetValueOrDefault();
        }

        // GET: Posts/Like/5
        public async Task<IActionResult> Like(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            var log = _context.UserActionLogs.Where(x => x.PostId == post.PostId && x.UserId == userId).SingleOrDefault();

            if (log != null)
            {
                if (log.Liked)
                {
                    // already liked, remove the like
                    post.Like--;
                    _context.UserActionLogs.Remove(log);
                }
                else
                {
                    // disliked, change to like
                    post.Dislike--;
                    post.Like++;
                    log.Liked = true;
                    log.UpdatedDate = DateTime.Now;
                    _context.UserActionLogs.Update(log);
                }
            }
            else
            {
                // not liked or disliked, add the like
                post.Like++;
                _context.UserActionLogs.Add(new Entities.UserActionLog
                {
                    Liked = true,
                    PostId = post.PostId,
                    UpdatedDate = DateTime.Now,
                    UserId = userId,
                });
            }
            post.Like = post.Like < 0 ? 0 : post.Like;
            post.Dislike = post.Dislike < 0 ? 0 : post.Dislike;
            _context.Update(post);

            await _context.SaveChangesAsync();

            string referer = Request.Headers["Referer"].ToString();

            return Redirect(referer);
        }


        // GET: Posts/DisLike/5
        public async Task<IActionResult> DisLike(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }
            var log = _context.UserActionLogs.Where(x => x.PostId == post.PostId && x.UserId == userId).SingleOrDefault();

            if (log != null)
            {
                if (!log.Liked)
                {
                    // already disliked, remove the dislike
                    post.Dislike--;
                    _context.UserActionLogs.Remove(log);
                }
                else
                {
                    // liked, change to dislike
                    post.Like--;
                    post.Dislike++;
                    log.Liked = false;
                    log.UpdatedDate = DateTime.Now;
                    _context.UserActionLogs.Update(log);
                }
            }
            else
            {
                // not liked or disliked, add the dislike
                post.Dislike++;
                _context.UserActionLogs.Add(new Entities.UserActionLog
                {
                    Liked = false,
                    PostId = post.PostId,
                    UpdatedDate = DateTime.Now,
                    UserId = userId,
                });
            }
            post.Like = post.Like < 0 ? 0 : post.Like;
            post.Dislike = post.Dislike < 0 ? 0 : post.Dislike;
            _context.Update(post);

            await _context.SaveChangesAsync();
            string referer = Request.Headers["Referer"].ToString();

            return Redirect(referer);
        }

        public IActionResult AddComment()
        {
            ViewData["CommentID"] = new SelectList(_context.Comments, "CommentID","PostId");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int Id, [Bind("Content")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(new Comment
                {
                    PostId = Id,
                    ContentDate= DateTime.Now,
                    Content = comment.Content,
                });
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { Id = Id});
            }
            ViewData["CommentID"] = new SelectList(_context.Comments, "CommentID","PostId", comment.CommentID);

            string referer = Request.Headers["Referer"].ToString();

            return Redirect(referer);
        }

    }
}
