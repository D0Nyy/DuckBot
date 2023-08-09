using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace DiscordBotCS.Services
{
    // here we can have the messages checking staff
    public class ChatService : InitializedService
    {
        private readonly CommandService _commands;
        private readonly IConfiguration _config;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _provider;

        public ChatService(DiscordSocketClient discord, CommandService commands, IConfiguration config,
            IServiceProvider provider)
        {
            _provider = provider;
            _discord = discord;
            _commands = commands;
            _config = config;
        }

        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            _discord.JoinedGuild += OnJoinedGuild;
            await Task.CompletedTask;
        }

        // When bot joins a server
        private async Task OnJoinedGuild(SocketGuild arg)
        {
            // First message
            var builder = new EmbedBuilder()
                .WithTitle("DuckBot is here!! *quack*")
                .WithThumbnailUrl(_discord.CurrentUser.GetAvatarUrl())
                .WithDescription("Write command @Help for the list of commands.");
            var embed = builder.Build();
            await arg.DefaultChannel.SendMessageAsync(null, false, embed);
        }

        public static async Task OnMessageReceived(SocketMessage arg)
        {
            //var user = (SocketGuildUser) arg.Author;
            //var message = (SocketUserMessage)arg;

            await Task.CompletedTask;
        }
    }
}