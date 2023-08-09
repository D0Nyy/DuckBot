using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotCS.Database;
using Microsoft.Extensions.Configuration;

namespace DiscordBotCS.Services
{
    public class DataBaseService : InitializedService
    {
        private readonly CommandService _commands;
        private readonly IConfiguration _config;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _provider;

        public DataBaseService(IServiceProvider provider, DiscordSocketClient discord, CommandService commands,
            IConfiguration config)
        {
            _provider = provider;
            _discord = discord;
            _commands = commands;
            _config = config;
        }

        public override Task InitializeAsync(CancellationToken cancellationToken)
        {
            DbManager.Start(_provider, _discord, _commands, _config);
            return Task.CompletedTask;
        }
    }
}