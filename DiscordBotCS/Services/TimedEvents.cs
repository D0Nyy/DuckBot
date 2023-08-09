using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotCS.Schedule;
using DiscordBotCS.Services.Other;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;

namespace DiscordBotCS.Services
{
    public class TimedEvents : InitializedService
    {
        private static Timer _quackTimer;
        private static Timer _changeStatus;
        private static Timer _checkSchedule;
        private readonly CommandService _commands;
        private readonly IConfiguration _config;
        private readonly DiscordSocketClient _discord;

        private readonly LavaNode _lavaNode;
        private readonly ILogger<TimedEvents> _logger;
        private readonly IServiceProvider _provider;
        private readonly Random _rand = new((int) DateTime.Now.Ticks);
        private DateTime lastChecked = DateTime.Now;
        private List<Lesson> schedule = Schedule.Schedule.GetTodaysSchedule();

        public TimedEvents(DiscordSocketClient discord, CommandService commands, IConfiguration config,
            IServiceProvider provider, ILogger<TimedEvents> logger, LavaNode lavaNode)
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
            _discord.Ready += OnReady;
            _lavaNode.OnTrackEnded += OnQuackEnded;

            await Task.CompletedTask;
        }

        private async Task OnReady()
        {
            // Create & Start timer
            _changeStatus = new Timer(NextStatus, null, 0, 1000 * 60 * 30); // This will repeat every 30 minutes

            _quackTimer = new Timer(Quack, null, 0, 1000 * 60 * 30); // every 30 minutes

            _checkSchedule = new Timer(CheckSchedule, null, 0, 1000 * 60 * 5); // every 5 minutes //1000 * 60 * 5

            await Task.CompletedTask;
        }

        private async void CheckSchedule(object state)
        {
            if (DateTime.Now.Subtract(lastChecked).Hours >= 12)
            {
                schedule = Schedule.Schedule.GetTodaysSchedule();
                lastChecked = DateTime.Now;
            }

            var now = DateTime.Now.TimeOfDay.Hours.ToString();

            foreach (var lesson in schedule)
            {
                if (lesson.Time is null) continue;

                var time = lesson.Time.Split('-')[0];
                var difference = DateTime.Parse(time).TimeOfDay.Subtract(DateTime.Now.TimeOfDay);
                if (difference.Hours == 0 && difference.Minutes <= 10 && difference.Minutes > 0 && !lesson.passed)
                {
                    var completeTime =
                        $"{DateTime.Today.Date.Month.ToString()}/{DateTime.Today.Date.Day.ToString()}/{DateTime.Today.Date.Year.ToString()} {time}";
                    var builder = new EmbedBuilder()
                        .WithColor(new Color(_rand.Next(256), _rand.Next(256), _rand.Next(256)))
                        .WithTitle($"{lesson.Name}")
                        .WithFooter("Starting")
                        .WithTimestamp(DateTime.Parse(completeTime));
                    var embed = builder.Build();

                    foreach (var guild in _discord.Guilds)
                    {
                        lesson.passed = true;
                        await guild.DefaultChannel.SendMessageAsync(null, false, embed);
                    }
                }
            }
        }

        private async void NextStatus(object state)
        {
            if (DateTime.Now.Hour > 0 && DateTime.Now.Hour < 8)
            {
                await _discord.SetActivityAsync(new Game("you sleep :)", ActivityType.Watching));
                _logger.LogInformation($"{DateTime.Now:HH:mm:ss} Changed Bot Status to 'Watching you sleep'");
            }
            else
            {
                var playing = Playing.GetNextGame();
                await _discord.SetActivityAsync(new Game(playing));
                _logger.LogInformation($"{DateTime.Now:HH:mm:ss} Changed Bot Status to 'PLaying {playing}'");
            }
        }

        private async void Quack(object state)
        {
            // Get all guilds the bot has joined
            var guilds = _discord.Guilds.AsEnumerable();
            foreach (var guild in guilds)
            {
                var connectedVoice = guild.VoiceChannels.Where(x => x.Users.Count > 0).Select(x => x); // linq
                foreach (var socketVoiceChannel in connectedVoice)
                {
                    //Console.WriteLine($"in the server {guild.Name} in the voice channel {socketVoiceChannel.Name} there are {socketVoiceChannel.Users.Count} users connected");
                    if (_lavaNode.HasPlayer(socketVoiceChannel.Guild)) break; // go to next server
                    try
                    {
                        // get quack sound effect
                        var searchResponse =
                            await _lavaNode.SearchYouTubeAsync("https://www.youtube.com/watch?v=Fw3RB7xnb80");

                        if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                            searchResponse.LoadStatus == LoadStatus.NoMatches)
                        {
                            _logger.LogWarning("Failed to find sound");
                            return;
                        }

                        // join voice channel first (lavaNode doesn't create a player for the channel until we join one).
                        await _lavaNode.JoinAsync(socketVoiceChannel);

                        // Get channel's music player and play song
                        var player = _lavaNode.GetPlayer(socketVoiceChannel.Guild);
                        var track = searchResponse.Tracks[0]; //get first result
                        await player.PlayAsync(track); // play track
                        // We leave the voice channel in the event OnQuackEnded down below.
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message);
                    }
                }
            }
        }

        private async Task OnQuackEnded(TrackEndedEventArgs arg)
        {
            await _lavaNode.LeaveAsync(arg.Player.VoiceChannel);
        }
    }
}