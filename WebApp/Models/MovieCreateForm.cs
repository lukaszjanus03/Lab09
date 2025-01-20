namespace WebApp.Models.ViewModels;

public class MovieCreateForm
{
    public string Title { get; set; }
    public int? Budget { get; set; }
    public string Homepage { get; set; }
    public string Overview { get; set; }
    public double? Popularity { get; set; }
    public DateTime? ReleaseDate { get; set; } // Zmienione na DateTime?
    public long? Revenue { get; set; }
    public int? Runtime { get; set; }
    public string MovieStatus { get; set; }
    public string Tagline { get; set; }
    public double? VoteAverage { get; set; }
    public int? VoteCount { get; set; }
}