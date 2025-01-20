using System;
using System.Collections.Generic;

namespace WebApp.Models.Movies;

public partial class Person
{
    public int PersonId { get; set; }
    public string PersonName { get; set; }
    
    // Sprawdź czy jest właściwość dla relacji MovieCasts
    public ICollection<MovieCast> MovieCasts { get; set; } = new List<MovieCast>();

    
}



