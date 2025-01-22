using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models.Movies;
using WebApp.Models.ViewModels;

namespace WebApp.Controllers
{
    public class MovieController : Controller
    {
        private readonly MoviesDbContext _context;

        public MovieController(MoviesDbContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index(int page = 1, int size = 20)
        {
            var movies = await _context.Movies
                .OrderBy(e => e.Title)
                .Skip((page - 1) * size)
                .Take(size)
                .AsNoTracking()
                .ToListAsync();

            var totalItems = await _context.Movies.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / size);
            
            ViewData["CurrentPage"] = page;
            ViewData["PageSize"] = size;
            ViewData["TotalItems"] = totalItems;
            ViewData["TotalPages"] = totalPages;

            return View(movies);
        }
        
        public async Task<IActionResult> MoviesByActor(int actorId)
        {
            var movies = await _context.MovieCasts
                .Where(mc => mc.PersonId == actorId)
                .Select(mc => new MovieByActorViewModels
                {
                    Title = mc.Movie.Title,
                    Budget = mc.Movie.Budget ?? 0m, 
                    Popularity = mc.Movie.Popularity ?? 0.0, 
                    Homepage = mc.Movie.Homepage,
                    ReleaseDate = mc.Movie.ReleaseDate.HasValue ? mc.Movie.ReleaseDate.Value.ToString("yyyy-MM-dd") : null,
                    MovieId = mc.Movie.MovieId,
                    Characters = mc.Movie.MovieCasts
                        .Where(m => m.MovieId == mc.Movie.MovieId)
                        .Select(m => m.CharacterName)
                        .ToList()
                }).OrderByDescending(o=>o.Popularity)
                .ToListAsync();

            if (!movies.Any())
            {
                return NotFound($"No movies found for actor with ID {actorId}.");
            }
            ViewData["actorId"] = actorId;
            return View(movies);
            
        }
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }
        
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieCreateForm form)
        {
            if (ModelState.IsValid)
            {
                var movie = new Movie
                {
                    Title = form.Title,
                    Budget = form.Budget,
                    Homepage = form.Homepage,
                    Overview = form.Overview,
                    Popularity = form.Popularity,
                    ReleaseDate = form.ReleaseDate.HasValue
                        ? DateOnly.FromDateTime(form.ReleaseDate.Value)
                        : null, 
                    Revenue = form.Revenue,
                    Runtime = form.Runtime,
                    MovieStatus = form.MovieStatus,
                    Tagline = form.Tagline,
                    VoteAverage = form.VoteAverage,
                    VoteCount = form.VoteCount
                };

                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(form);
        }
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MovieId,Title,Budget,Homepage,Overview,Popularity,ReleaseDate,Revenue,Runtime,MovieStatus,Tagline,VoteAverage,VoteCount")] Movie movie)
        {
            if (id != movie.MovieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.MovieId))
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
            return View(movie);
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.MovieId == id);
        }
    }
}
