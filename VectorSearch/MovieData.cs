using System;
using System.Collections.Generic;
using System.Text;

namespace VectorSearch
{
    internal static class MovieData
    {
        public static List<Movie> Movies => [

            new Movie
            {
                key = 1,
                Title = "The Matrix",
                Description = "A computer hacker learns about the true nature of his reality and his role in the war against its controllers.",
            },
            new Movie
            {
                key = 2,
                Title = "Inception",
                Description = "A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a CEO.",
            },
            new Movie
            {
                key = 3,
                Title = "Interstellar",
                Description = "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.",
            },
            new Movie
            {
                key = 4,
                Title = "The Dark Knight",
                Description = "When the menace known as the Joker emerges from his mysterious past, he wreaks havoc and chaos on the people of Gotham.",
            },
            new Movie
            {
                key = 5,
                Title = "Pulp Fiction",
                Description = "The lives of two mob hitmen, a boxer, a gangster's wife, and a pair of diner bandits intertwine in four tales of violence and redemption.",
            },
            new Movie
            {
                key = 6,
                Title = "The Lord of the Rings: The Fellowship of the Ring",
                Description = "A meek Hobbit from the Shire and eight companions set out on a journey to destroy the powerful One Ring and save Middle-earth from the Dark Lord Sauron.",
            }



            ];
    }
}
