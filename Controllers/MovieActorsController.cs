using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Fall2025_Project3_gbward.Data;
using Fall2025_Project3_gbward.Models;
using Fall2025_Project3_gbward.ViewModels;

namespace Fall2025_Project3_gbward.Controllers
{
    public class MovieActorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MovieActorsController> _logger;

        public MovieActorsController(ApplicationDbContext context, ILogger<MovieActorsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: MovieActors
        public async Task<IActionResult> Index()
        {
            var query = _context.MovieActors
                .Include(ma => ma.Movie)
                .Include(ma => ma.Actor);

            return View(await query.ToListAsync());
        }

        // GET: MovieActors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var movieActor = await _context.MovieActors
                .Include(ma => ma.Movie)
                .Include(ma => ma.Actor)
                .FirstOrDefaultAsync(ma => ma.Id == id);

            if (movieActor == null)
                return NotFound();

            return View(movieActor);
        }

        // GET: MovieActors/Create
        public async Task<IActionResult> Create()
        {
            var movies = await _context.Movies.OrderBy(m => m.Title).ToListAsync();
            var actors = await _context.Actors.OrderBy(a => a.Name).ToListAsync();

            var vm = new CreateMovieActorViewModel
            {
                Movies = new SelectList(movies, "Id", "Title"),
                Actors = new SelectList(actors, "Id", "Name")
            };

            return View(vm);
        }

        // POST: MovieActors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMovieActorViewModel vm)
        {
            // Basic validation
            if (vm.MovieId <= 0)
                ModelState.AddModelError("MovieId", "Please select a movie.");

            if (vm.ActorId <= 0)
                ModelState.AddModelError("ActorId", "Please select an actor.");

            // Check for duplicate pairing
            var exists = await _context.MovieActors.AnyAsync(ma => ma.MovieId == vm.MovieId && ma.ActorId == vm.ActorId);
            if (exists)
                ModelState.AddModelError(string.Empty, "This movie–actor relationship already exists.");

            if (!ModelState.IsValid)
            {
                var moviesList = await _context.Movies.OrderBy(m => m.Title).ToListAsync();
                var actorsList = await _context.Actors.OrderBy(a => a.Name).ToListAsync();

                vm.Movies = new SelectList(moviesList, "Id", "Title", vm.MovieId);
                vm.Actors = new SelectList(actorsList, "Id", "Name", vm.ActorId);

                return View(vm);
            }

            try
            {
                var movieActor = new MovieActor
                {
                    MovieId = vm.MovieId,
                    ActorId = vm.ActorId
                };

                _context.Add(movieActor);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating MovieActor relationship. MovieId={MovieId}, ActorId={ActorId}", vm.MovieId, vm.ActorId);
                ModelState.AddModelError(string.Empty, "Unable to save relationship.");

                vm.Movies = new SelectList(await _context.Movies.OrderBy(m => m.Title).ToListAsync(), "Id", "Title", vm.MovieId);
                vm.Actors = new SelectList(await _context.Actors.OrderBy(a => a.Name).ToListAsync(), "Id", "Name", vm.ActorId);

                return View(vm);
            }
        }

        // GET: MovieActors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var movieActor = await _context.MovieActors.FindAsync(id);
            if (movieActor == null)
                return NotFound();

            var vm = new CreateMovieActorViewModel
            {
                Id = movieActor.Id,
                MovieId = movieActor.MovieId,
                ActorId = movieActor.ActorId,
                Movies = new SelectList(await _context.Movies.OrderBy(m => m.Title).ToListAsync(), "Id", "Title", movieActor.MovieId),
                Actors = new SelectList(await _context.Actors.OrderBy(a => a.Name).ToListAsync(), "Id", "Name", movieActor.ActorId)
            };

            return View(vm);
        }

        // POST: MovieActors/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateMovieActorViewModel vm)
        {
            if (vm == null)
                return NotFound();

            var existing = await _context.MovieActors.FirstOrDefaultAsync(ma => ma.Id == vm.Id);
            if (existing == null)
                return NotFound();

            // Basic validation
            if (vm.MovieId <= 0)
                ModelState.AddModelError("MovieId", "Please select a movie.");

            if (vm.ActorId <= 0)
                ModelState.AddModelError("ActorId", "Please select an actor.");

            // Prevent duplicate
            var duplicate = await _context.MovieActors.AnyAsync(ma =>
                ma.MovieId == vm.MovieId &&
                ma.ActorId == vm.ActorId &&
                ma.Id != vm.Id);

            if (duplicate)
                ModelState.AddModelError(string.Empty, "This relationship already exists.");

            if (!ModelState.IsValid)
            {
                vm.Movies = new SelectList(await _context.Movies.OrderBy(m => m.Title).ToListAsync(), "Id", "Title", vm.MovieId);
                vm.Actors = new SelectList(await _context.Actors.OrderBy(a => a.Name).ToListAsync(), "Id", "Name", vm.ActorId);

                return View(vm);
            }

            existing.MovieId = vm.MovieId;
            existing.ActorId = vm.ActorId;

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieActorExists(existing.Id))
                    return NotFound();

                throw;
            }
        }

        // GET: MovieActors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var movieActor = await _context.MovieActors
                .Include(ma => ma.Movie)
                .Include(ma => ma.Actor)
                .FirstOrDefaultAsync(ma => ma.Id == id);

            if (movieActor == null)
                return NotFound();

            return View(movieActor);
        }

        // POST: MovieActors/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movieActor = await _context.MovieActors.FindAsync(id);
            if (movieActor != null)
            {
                _context.MovieActors.Remove(movieActor);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MovieActorExists(int id)
        {
            return _context.MovieActors.Any(e => e.Id == id);
        }
    }
}
