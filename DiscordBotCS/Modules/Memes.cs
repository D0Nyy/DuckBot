using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;

namespace DiscordBotCS.Modules
{
    [Summary("Meme Commands.")]
    [Name("Memes")]
    public class Memes : ModuleBase
    {
        private readonly Random _rand = new((int) DateTime.Now.Ticks);
        //private string subReddit = "dankmemes"; // read that from a file or something // also make a command that for the admin so he can change it

        [Summary("Searches the given subReddit and returns a random post.")]
        [Alias("Reddit")]
        [Command("Meme")]
        public async Task Meme([Summary("The subReddit.")] string subReddit = null)
        {
            var client = new HttpClient();
            var result =
                await client.GetStringAsync(
                    $"https://www.reddit.com/r/{subReddit ?? "memes"}/random.json?limit=1"); // put a max time to this
            if (!result.StartsWith("["))
            {
                await ReplyAsync("This subreddit doesn't exist");
                return;
            }

            var arr = JArray.Parse(result);
            var post = JObject.Parse(arr[0]["data"]["children"][0]["data"].ToString());

            var builder = new EmbedBuilder()
                .WithImageUrl(post["url"].ToString())
                .WithColor(new Color(_rand.Next(256), _rand.Next(256), _rand.Next(256)))
                .WithTitle(post["title"].ToString())
                .WithUrl("https://www.reddit.com" + post["permalink"])
                .WithFooter($"🗨{post["num_comments"]} ⬆️ {post["ups"]}");
            var embed = builder.Build();

            await ReplyAsync(null, false, embed);
        }
    }
}