using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace DiscordBotCS.Modules
{
    [Summary("Unipi Commands.")]
    [Name("Unipi")]
    public class UnipiCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<UnipiCommands> _logger;
        private readonly Random _rand = new((int) DateTime.Now.Ticks);

        public UnipiCommands(ILogger<UnipiCommands> logger)
        {
            _logger = logger;
        }

        [Summary("Displays the weeks schedule.")]
        [Command("Schedule")]
        [Alias("programa", "sc", "killme")]
        public async Task Schedule()
        {
            var schedule = DiscordBotCS.Schedule.Schedule.GetWeeksSchedule();

            var builder = new EmbedBuilder()
                .WithTitle("ΠΡΟΓΡΑΜΜΑ ΔΙΔΑΣΚΑΛΙΑΣ (ΕΞΑΜΗΝΟ: 4)")
                .WithColor(new Color(_rand.Next(256), _rand.Next(256), _rand.Next(256)))
                .AddField("Δευτέρα",
                    string.Join(Environment.NewLine,
                        schedule.Where(x => x.Day.Equals("Monday")).Select(x => x.ToString())))
                .AddField("Τρίτη",
                    string.Join(Environment.NewLine,
                        schedule.Where(x => x.Day.Equals("Tuesday")).Select(x => x.ToString())))
                .AddField("Τετάρτη",
                    string.Join(Environment.NewLine,
                        schedule.Where(x => x.Day.Equals("Wednesday")).Select(x => x.ToString())))
                .AddField("Πέμπτη",
                    string.Join(Environment.NewLine,
                        schedule.Where(x => x.Day.Equals("Thursday")).Select(x => x.ToString())))
                .AddField("Παρασκεύη",
                    string.Join(Environment.NewLine,
                        schedule.Where(x => x.Day.Equals("Friday")).Select(x => x.ToString())))
                .WithFooter("This is torture...");
            var embed = builder.Build();
            await ReplyAsync(null, false, embed);
        }

        [Summary("Displays todays schedule.")]
        [Command("Today")]
        [Alias("sct")]
        public async Task TodaysSchedule()
        {
            var schedule = DiscordBotCS.Schedule.Schedule.GetTodaysSchedule();

            var builder = new EmbedBuilder()
                .WithTitle("ΠΡΟΓΡΑΜΜΑ ΔΙΔΑΣΚΑΛΙΑΣ (ΕΞΑΜΗΝΟ: 4)")
                .WithColor(new Color(_rand.Next(256), _rand.Next(256), _rand.Next(256)))
                .AddField($"ΣΗΜΕΡΑ ({DateTime.Now.Date:dd/MM/yyyy})",
                    string.Join(Environment.NewLine,
                        schedule.Select(x => x.ToString())))
                .WithFooter("This is torture...");
            var embed = builder.Build();
            await ReplyAsync(null, false, embed);
        }
    }
}