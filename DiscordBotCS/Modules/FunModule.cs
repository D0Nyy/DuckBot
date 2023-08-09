using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBotCS.Modules.MovieClasses;
using Google.Apis.Auth.OAuth2.Web;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace DiscordBotCS.Modules
{
    [Summary("Fun Commands")]
    [Name("Fun")]
    public class FunModule : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<UnipiCommands> _logger;
        private readonly Random _rand = new((int) DateTime.Now.Ticks);

        public FunModule(ILogger<UnipiCommands> logger)
        {
            _logger = logger;
        }

        [Command("Movie")]
        [Summary("Searches for a movie in imdb.")]
        [Alias("MV")]
        public async Task SearchMovie([Summary("Specified movie")] [Remainder] string quary)
        {
            // Create http client in order to get results
            var client = new HttpClient();
            
            // Normal Search
            var result = await client.GetStringAsync($"http://www.omdbapi.com/?t={quary}&apikey=db9bf499");

            var data = JsonSerializer.Deserialize<Movie>(result);

            // Check if movie was found
            if (data.Response.Equals("False"))
            {
                // Search for similar
                result = await client.GetStringAsync($"http://www.omdbapi.com/?s={quary}&apikey=db9bf499");
                var searchResult = JsonSerializer.Deserialize<MovieSearch>(result);

                // check for results
                if (searchResult.Response.Equals("False"))
                {
                    await ReplyAsync($"Did not find `{quary}` or any similar.");
                    return;
                }
                
                await ReplyAsync($"`{quary}` not found!\n Did you mean any of the following?");

                // Send results
                foreach (var movie in searchResult.Search)
                {
                    var builder2 = new EmbedBuilder()
                        .WithAuthor($"{movie.Title}",
                            @"https://upload.wikimedia.org/wikipedia/commons/thumb/c/cc/IMDb_Logo_Square.svg/1200px-IMDb_Logo_Square.svg.png")
                        .WithThumbnailUrl(
                            @$"https://www.publicdomainpictures.net/pictures/280000/velka/not-found-image-15383864787lu.jpg")
                        .WithTitle("Check on IMDB")
                        .WithUrl(@$"https://www.imdb.com/title/{movie.imdbID}/")
                        .WithDescription($"**Release:** {movie.Year} | **Type:** {movie.Type}");
                    
                    if (!movie.Poster.Equals("N/A"))
                    {
                        builder2.ThumbnailUrl = movie.Poster;
                    }
                    
                    var embed2 = builder2.Build();

                    await ReplyAsync(null, false, embed2);
                }
                
                return;
            }

            // If the movie was found
            var builder = new EmbedBuilder()
                .WithAuthor($"{data.Title}   {data.imdbRating}⭐",
                    @"https://upload.wikimedia.org/wikipedia/commons/thumb/c/cc/IMDb_Logo_Square.svg/1200px-IMDb_Logo_Square.svg.png")
                .AddField($"Description", $"{data.Plot}")
                .WithTitle($"Check on IMDB")
                .WithImageUrl(@$"https://www.publicdomainpictures.net/pictures/280000/velka/not-found-image-15383864787lu.jpg")
                .WithUrl(@$"https://www.imdb.com/title/{data.imdbID}/")
                .AddField($"Released", $"{data.Released}", true)
                .AddField($"Duration", $"{data.Runtime}, {data.Language}", true)
                .AddField($"Director", $"{data.Director}", true)
                .AddField($"Genre", $"{data.Genre}", true)
                .AddField($"Awards", $"{data.Awards}", true)
                .AddField($"Actors", $"{data.Actors}", true)
                .WithColor(new Color(_rand.Next(256), _rand.Next(256), _rand.Next(256)))
                .WithFooter($"{string.Join(" | ",data.Ratings.Select(x=>x.ToString()))}");

            // Check if series
            if (data.Type.Equals("series"))
            {
                builder.AddField("Total Seasons", data.totalSeasons);
            }

            // Get image if possible
            if (!data.Poster.Equals("N/A"))
            {
                builder.ImageUrl = data.Poster;
            }
            
            var embed = builder.Build();
            
            await ReplyAsync(null, false, embed);
            await Task.CompletedTask;
        }
        
        [Command("MovieSearch")]
        [Summary("Searches for multiple movies in imdb.")]
        [Alias("MVS")]
        public async Task SearchMovieMultiple([Summary("Specified movie")] [Remainder] string quary)
        {
            var client = new HttpClient();
            
            var result = await client.GetStringAsync($"http://www.omdbapi.com/?s={quary}&apikey=db9bf499"); // put a max time to this
            var searchResult = JsonSerializer.Deserialize<MovieSearch>(result);

            if (searchResult.Response.Equals("False"))
            {
                await ReplyAsync($"Did not find `{quary}` or any similar.");
                return;
            }
            
            foreach (var movie in searchResult.Search)
            {
                var builder = new EmbedBuilder()
                    .WithAuthor($"{movie.Title}",
                        @"https://upload.wikimedia.org/wikipedia/commons/thumb/c/cc/IMDb_Logo_Square.svg/1200px-IMDb_Logo_Square.svg.png")
                    .WithThumbnailUrl(
                        @$"https://www.publicdomainpictures.net/pictures/280000/velka/not-found-image-15383864787lu.jpg")
                    .WithTitle("Check on IMDB")
                    .WithUrl(@$"https://www.imdb.com/title/{movie.imdbID}/")
                    .WithDescription($"**Release:** {movie.Year} | **Type:** {movie.Type}");

                if (!movie.Poster.Equals("N/A"))
                {
                    builder.ThumbnailUrl = movie.Poster;
                }

                var embed = builder.Build();
                await ReplyAsync(null, false, embed);
            }
        }
    }
}