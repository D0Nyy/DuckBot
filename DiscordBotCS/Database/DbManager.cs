using System;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace DiscordBotCS.Database
{
    public static class DbManager
    {
        private static IServiceProvider _provider;
        private static DiscordSocketClient _discord;
        private static CommandService _commands;
        private static IConfiguration _config;
        private static string ConnectionString { get; } = @"Data Source=../../../Database/BotDatabase.db;Version=3;";

        public static void Start(IServiceProvider provider, DiscordSocketClient discord, CommandService commands,
            IConfiguration config)
        {
            _provider = provider;
            _discord = discord;
            _commands = commands;
            _config = config;
        }
    }
}