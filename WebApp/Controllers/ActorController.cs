using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebApp.Models.Movies;
using WebApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Controllers

{   
    public class ActorController : Controller
    {
        private readonly MoviesDbContext _context;

        public ActorController(MoviesDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1, int size = 10)
        {
            var totalItems = _context.People.Count();

            var actors = _context.People
                .OrderBy(p => p.PersonName)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(p => new PersonViewModel
                {
                    MostPopularMovie = _context.MovieCasts
                        .Where(mc => mc.PersonId == p.PersonId)
                        .OrderByDescending(mc => mc.Movie.Popularity).FirstOrDefault().Movie.Title,
                    PersonId = p.PersonId,
                    PersonName = p.PersonName,
                    MovieCount = _context.MovieCasts
                        .Count(mc => mc.PersonId == p.PersonId),
                    PopularMovieCharacters = _context.MovieCasts
                        .Where(mc => mc.PersonId == p.PersonId)
                        .OrderByDescending(mc => mc.Movie.Popularity)
                        .Select(mc => mc.CharacterName)
                        .ToList()
                        
                })
                .ToList();

            ViewData["CurrentPage"] = page;
            ViewData["PageSize"] = size;
            ViewData["TotalItems"] = totalItems;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalItems / size);

            return View(actors);
        }
        
        
        [Authorize]
        [HttpGet]
        public IActionResult AddMovie(int id)
        {
            var actor = _context.People.FirstOrDefault(p => p.PersonId == id);
            if (actor == null)
            {
                return NotFound();
            }
            
            var allMovies = _context.Movies.ToList();
            
            var vm = new AddMovieViewModel
            {
                ActorId = actor.PersonId,
                
                ActorName = actor.PersonName,
                
                Movies = allMovies.Select(m => new SelectListItem 
                {
                    Value = m.MovieId.ToString(),
                    Text = m.Title
                }).ToList()
            };

            return View(vm);
        }

        
        
        [Authorize]
        [HttpPost]
        public IActionResult AddMovie(AddMovieViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var allMovies = _context.Movies.ToList();
                model.Movies = allMovies.Select(m => new SelectListItem
                {
                    Value = m.MovieId.ToString(),
                    Text = m.Title
                }).ToList();
                
                return View(model);
            }
            
            var movieCast = new MovieCast
            {
                PersonId = model.ActorId,
                MovieId = model.SelectedMovieId,
                CharacterName = model.CharacterName
            };
            
            _context.MovieCasts.Add(movieCast);
            _context.SaveChanges();
            
            return RedirectToAction("MoviesByActor", "Movie", new { actorId = model.ActorId });
        }

    }
}