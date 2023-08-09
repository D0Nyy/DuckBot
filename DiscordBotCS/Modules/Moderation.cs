using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DiscordBotCS.Modules
{
    [Summary("Moderation Commands.")]
    [Name("Moderation")]
    public class Moderation : ModuleBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<Moderation> _logger;

        public Moderation(ILogger<Moderation> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        //[RequireOwner] Only we can use it
        [Summary(@"Delete x messages from this channel.")]
        [Alias("Clean", "Clear")]
        [Command("Purge")]
        [RequireUserPermission(GuildPermission.ManageMessages)] //Permissions
        public async Task Purge([Summary("Number of messages.")] int amount)
        {
            // We can use Context.channel.sendMessageAsync() instead of ReplyAsync
            if (amount <= 0)
            {
                await ReplyAsync("Number of messages to be removed must be bigger than 0.");
                return;
            }

            // Delete the command message
            await Context.Message.DeleteAsync();

            // get amount messages from channel
            var messages = await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, amount)
                .FlattenAsync();

            // Filter them to < 14 days
            var filteredMessages = messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);

            if (!messages.Any()) // if no messages found.
            {
                await ReplyAsync("Nothing to delete here.");
            }
            else
            {
                if (filteredMessages.Any()) // if there messages newer than two weeks.
                {
                    // Delete all filtered messages.
                    await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(filteredMessages);

                    IUserMessage message;
                    // show how many messages got deleted for a specific time but also stored it in a variable ( so we can delete it if we want to)
                    if (messages.Count() > filteredMessages.Count())
                        message = await ReplyAsync(
                            $"{filteredMessages.Count()} messages deleted successfully.{Environment.NewLine}" +
                            "Cant delete messages that are more than 2 weeks old.");
                    else
                        message = await ReplyAsync($"{filteredMessages.Count()} messages deleted successfully.");

                    await Task.Delay(2500);

                    await message.DeleteAsync();
                }
                else // if all messages are older than two weeks.
                {
                    await ReplyAsync("Cant delete messages that are more than 2 weeks old.");
                }
            }
        }
    }
}