namespace WebApp.Models.ViewModels
{
    public class MovieByActorViewModels
    {

        public string Title { get; set; }
        public decimal Budget { get; set; }
        public double Popularity { get; set; }
        public string Homepage { get; set; }
        public string ReleaseDate { get; set; }  // Zmieniamy na string
        public string MovieStatus { get; set; }
        

        // Dodaj nową właściwość na listę postaci
        public List<string> Characters { get; set; } = new List<string>();
        public int MovieId { get; set; }
    }
}