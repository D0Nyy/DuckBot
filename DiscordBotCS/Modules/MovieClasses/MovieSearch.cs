using System.Collections.Generic;

namespace DiscordBotCS.Modules.MovieClasses
{
    public class MovieSearch
    {
        public IEnumerable<Movie> Search { get; set; }
        public string totalResponses { get; set; }
        public string Response { get; set; }
    }
}