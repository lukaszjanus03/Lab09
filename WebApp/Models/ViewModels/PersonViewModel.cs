namespace WebApp.Models.ViewModels
{
    public class PersonViewModel
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public int MovieCount { get; set; }
        public string MostPopularMovie { get; set; }
        public List<string> PopularMovieCharacters { get; set; } 
    }
}