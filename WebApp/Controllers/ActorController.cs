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
            // 1. Znajdź aktora w People
            var actor = _context.People.FirstOrDefault(p => p.PersonId == id);
            if (actor == null)
            {
                return NotFound();
            }

            // 2. Pobierz listę wszystkich filmów
            var allMovies = _context.Movies.ToList();

            // 3. Przygotuj ViewModel
            var vm = new AddMovieViewModel
            {
                // actor.PersonId, bo tak nazwałeś w encji People
                ActorId = actor.PersonId,
        
                // actor.PersonName, bo tak nazwałeś w encji People
                ActorName = actor.PersonName,

                // Tworzymy listę SelectListItem z listy filmów
                Movies = allMovies.Select(m => new SelectListItem 
                {
                    // klucz w Movie to prawdopodobnie MovieId
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
            // 1. Walidacja modelu
            if (!ModelState.IsValid)
            {
                // Ponownie wypełniamy listę filmów, aby SELECT nie był pusty:
                var allMovies = _context.Movies.ToList();
                model.Movies = allMovies.Select(m => new SelectListItem
                {
                    Value = m.MovieId.ToString(),
                    Text = m.Title
                }).ToList();

                // Zostajemy w widoku, który będzie wyświetlał błędy
                return View(model);
            }

            // 2. Tworzymy wpis w MovieCasts
            var movieCast = new MovieCast
            {
                PersonId = model.ActorId,
                MovieId = model.SelectedMovieId,
                CharacterName = model.CharacterName
            };

            // 3. Zapis w bazie
            _context.MovieCasts.Add(movieCast);
            _context.SaveChanges();

            // 4. Przekierowanie z powrotem do listy filmów
            return RedirectToAction("MoviesByActor", "Movie", new { actorId = model.ActorId });
        }

    }
}