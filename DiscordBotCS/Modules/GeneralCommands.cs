using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace DiscordBotCS.Modules
{
    [Summary("General Commands.")]
    [Name("General")]
    public class GeneralCommands : ModuleBase<SocketCommandContext> // ModuleBase = this file can be used to execute commands
    {
        private readonly CommandService _commandService;

        private readonly ILogger<GeneralCommands> _logger;
        private readonly Random _rand = new((int) DateTime.Now.Ticks);

        public GeneralCommands(ILogger<GeneralCommands> logger, CommandService commandService)
        {
            _logger = logger;
            _commandService = commandService;
        }

        [Summary("Quacks back!")]
        [Command("Quack")]
        public async Task Quack()
        {
            //Context.Message.Author.SendMessageAsync("hey"); // This is how to send a dm
            await Context.Channel.SendMessageAsync("Quack ♥️");
        }

        [Summary("Shows server info.")]
        [Command("Server")]
        public async Task Server()
        {
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .WithDescription("Server info.")
                .WithTitle($"{Context.Guild.Name} Information.")
                .WithColor(new Color(_rand.Next(256), _rand.Next(256), _rand.Next(256)))
                .AddField("Created at", Context.Guild.CreatedAt.ToString("dd/MM/yyyy"), true)
                .AddField("Member count", Context.Guild.MemberCount + " Members", true)
                .AddField("Online Users",
                    Context.Guild.Users.Count(on => on.Status != UserStatus.Offline) + " Members",
                    true); // Doesnt work for some reason FIX: go threw every user using getUser.
            var embed = builder.Build();

            await ReplyAsync(null, false, embed);
        }

        // Send Player Info
        [Summary("See your or onother users info.")]
        [Command("Info")]
        public async Task Info([Summary("The user.")] SocketGuildUser user = null)
        {
            if (user == null) user = Context.User as SocketGuildUser;

            var builder = new EmbedBuilder()
                .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                .WithDescription($"{user.Username}#{user.Discriminator} Info.")
                .WithColor(new Color(_rand.Next(256), _rand.Next(256), _rand.Next(256)))
                .AddField("User ID: ", user.Id)
                .AddField("Created at:", user.CreatedAt.ToString("dd/MM/yyyy"), true)
                .AddField("Joined at:", user.JoinedAt.Value.ToString("dd/MM/yyyy"), true)
                .AddField("Roles:",
                    string.Join(", ",
                        user.Roles.Select(x =>
                            x.Mention))) // This will show the roles the user has. with select(lambda expression) we choose to show the role as a mention and not as a plain string.(it will look nicer)(it doesn't actually mention the role)
                .WithTimestamp(DateTimeOffset.Now);

            var embed = builder.Build();

            await ReplyAsync(null, false, embed);
        }

        [Command("Help")]
        [Summary("Displays help")]
        public async Task Help()
        {
            var builder = new EmbedBuilder()
                .WithTitle("List of Duckbot's commands:")
                .WithThumbnailUrl(@"https://cdn2.iconfinder.com/data/icons/app-types-in-grey/512/info_512pxGREY.png")
                .WithColor(new Color(_rand.Next(256), _rand.Next(256), _rand.Next(256)));

            foreach (var module in _commandService.Modules)
            {
                var moduleCommands = "";
                foreach (var command in module.Commands)
                {
                    var commandAliases = "";
                    foreach (var commandAlias in command.Aliases)
                    {
                        if (commandAlias.Equals(command.Name.ToLower())) continue;
                        commandAliases += $"`@{commandAlias}` ";
                    }

                    moduleCommands += $"`@{command.Name}` {commandAliases} {command.Summary ?? "No description"}\n";
                }

                builder.AddField(module.Summary, moduleCommands);
            }

            var embed = builder.Build();

            await ReplyAsync(null, false, embed);
        }
    }
}