using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Victoria;

/*
 * We can exclude all modules at the start of OnMessageReceived
 * and according if the message is from a Guild Or a DM
 * add the required ones.
 */

namespace DiscordBotCS.Services
{
    public class CommandHandler : InitializedService
    {
        private readonly CommandService _commands;
        private readonly IConfiguration _config;
        private readonly DiscordSocketClient _discord;

        // Music bot
        private readonly LavaNode _lavaNode;
        private readonly ILogger<CommandHandler> _logger;
        private readonly IServiceProvider _provider;

        public CommandHandler(DiscordSocketClient discord, CommandService commands, IConfiguration config,
            IServiceProvider provider, ILogger<CommandHandler> logger, LavaNode lavaNode)
        {
            _provider = provider;
            _discord = discord;
            _commands = commands;
            _config = config;
            _logger = logger;
            _lavaNode = lavaNode;
        }

        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            _discord.MessageReceived += OnMessageReceived;
            _commands.CommandExecuted += OnCommandExecuted;
            _discord.Ready += OnReadyAsync;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }

        // This runs every time message is send
        private async Task OnMessageReceived(SocketMessage arg)
        {
            var message = (SocketUserMessage) arg;
            if (arg.Author.IsBot) return;

            var pos = 0; // this will run when there's !in the start of the message and when a user mentions the bot
            // Get the prefix of the server from the database. if null then use the default one from settings.json
            var prefix = _config["Prefix"];
            if (message.HasStringPrefix(prefix, ref pos) || message.HasMentionPrefix(_discord.CurrentUser, ref pos))
            {
                // This is the command context. this specifies the command that is going to be executed
                var context = new SocketCommandContext(_discord, message);

                // If a command is send from a guild channel Execute command
                try
                {
                    if (message.Channel is ITextChannel) await _commands.ExecuteAsync(context, pos, _provider);

                    // If a command is send from a DM channel
                    else if (message.Channel is IDMChannel) _logger.LogInformation("DM command send");
                }
                catch (Exception)
                {
                    await arg.Channel.SendMessageAsync("Command not found! Use `@help` for the command list.");
                }
            }
            else
            {
                await ChatService.OnMessageReceived(arg);
            }
        }

        // This will run every time a command is executed
        private async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // IsSpecified makes it so it only runs if the user actually typed a valid command name
            if (!result.IsSuccess && command.IsSpecified)
            {
                // Display error message
                await context.Channel.SendMessageAsync(result.ToString());
                Console.WriteLine(result.ToString());

                // Delete fail message
                await context.Message.DeleteAsync();
            }

            _logger.LogInformation(
                $"{DateTime.Now:HH:mm:ss} {context.User.Username}#{context.User.Discriminator} executed the {command.Value.Name} command!");
        }

        private async Task OnReadyAsync()
        {
            if (!_lavaNode.IsConnected) await _lavaNode.ConnectAsync();
        }
    }
}