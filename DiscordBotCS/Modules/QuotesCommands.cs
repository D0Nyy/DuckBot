using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace DiscordBotCS.Modules
{
    [Summary("Quote commands.")]
    [Name("Quotes")]
    public class QuotesCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<QuotesCommands> _logger;
        private readonly Random r = new();

        private readonly List<string> Squotes = new()
        {
            "I am the one true God, I am the Duck", "Love yourself, Duck Bot Loves you"
        };

        public QuotesCommands(ILogger<QuotesCommands> logger)
        {
            _logger = logger;
        }

        [Summary("Duck Quote.")]
        [Command("DuckQuote")]
        [Alias("dq")]
        public async Task DuckQuote()
        {
            var ind = r.Next(Squotes.Count);
            await ReplyAsync(Squotes[ind], true);
        }
    }
}
