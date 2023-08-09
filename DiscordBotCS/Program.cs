using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotCS.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Victoria;

namespace DiscordBotCS
{
    internal class Program
    {
        private static async Task Main()
        {
            // This builds all our services
            var builder = new HostBuilder();

            // Configuration file.
            builder.ConfigureAppConfiguration(configurationBuilder =>
            {
                var build = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("settings.json", false, true);
                var configuration = build.Build();
                configurationBuilder.AddConfiguration(configuration);
            });

            // Bot configuration
            builder.ConfigureDiscordHost<DiscordSocketClient>((context, configuration) =>
            {
                configuration.SocketConfig = new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose, // API Logging information severity
                    AlwaysDownloadUsers = true,
                    MessageCacheSize = 500
                };
                configuration.Token = context.Configuration["Token"];
            });

            // Command Service configuration
            builder.UseCommandService((context, config) =>
            {
                config = new CommandServiceConfig
                {
                    CaseSensitiveCommands = false,
                    LogLevel = LogSeverity.Verbose
                };
            });

            // Logging Configuration // May remove and keep old one
            builder.ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.SetMinimumLevel(LogLevel.Debug);
            });

            builder.ConfigureServices((context, collection) =>
            {
                // ADD SERVICES HERE
                collection.AddHostedService<CommandHandler>()
                    .AddLavaNode(x =>
                    {
                        x.SelfDeaf = true; // MUSIC BOT
                    })
                    .AddHostedService<ChatService>()
                    .AddHostedService<TimedEvents>()
                    .AddHostedService<DataBaseService>()
                    .AddHostedService<EmailNotifications>();
            }).UseConsoleLifetime();

            // Start Bot
            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}