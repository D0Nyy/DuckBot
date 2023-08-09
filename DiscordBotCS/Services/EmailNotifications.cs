using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotCS.Services.Other;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DiscordBotCS.Services
{
    public class EmailNotifications : InitializedService
    {
        // Gmail Api
        private static readonly string[] Scopes =
        {
            GmailService.Scope.GmailReadonly,
            GmailService.Scope.GmailInsert,
            GmailService.Scope.GmailModify,
            GmailService.Scope.MailGoogleCom
        };

        public static GmailService Service;
        private static readonly string ApplicationName = @"Gmail API.NET Quickstart";
        private static UserCredential userCredential;
        private static Profile profile;
        private static string labelID;
        private static int lastEmailCount;
        private static Timer timer;
        private readonly CommandService _commands;
        private readonly IConfiguration _config;
        private readonly DiscordSocketClient _discord;
        private readonly ILogger<EmailNotifications> _logger;
        private readonly IServiceProvider _provider;

        private readonly Random _rand = new((int) DateTime.Now.Ticks);

        public EmailNotifications(IServiceProvider provider, DiscordSocketClient discord, CommandService commands,
            IConfiguration config, ILogger<EmailNotifications> logger)
        {
            _provider = provider;
            _discord = discord;
            _commands = commands;
            _config = config;
            _logger = logger;
        }

        public override Task InitializeAsync(CancellationToken cancellationToken)
        {
            _discord.Ready += Ready;
            return Task.CompletedTask;
        }

        private async Task Ready()
        {
            // The file token.json stores the user's access and refresh tokens, and is created
            // automatically when the authorization flow completes for the first time.
            using (var stream = File.Open("credentials.json", FileMode.Open, FileAccess.Read))
            {
                var credPath = "token.json";
                userCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets, Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Gmail API service.
            Service = new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = userCredential,
                ApplicationName = ApplicationName
            });

            profile = Service.Users.GetProfile("me").ExecuteAsync().Result;
            var request = Service.Users.Labels.List("me").ExecuteAsync().Result;
            var label = Service.Users.Labels.Get("me", request.Labels.First(x => x.Name == "UNIPI").Id).ExecuteAsync()
                .Result;
            labelID = label.Id;
            lastEmailCount = label.MessagesTotal.Value;
            _logger.LogInformation("Gmail Notifications Are Up!");

            // Create timer and return
            timer = new Timer(Tick, null, 0, 300000); //5 minutes (288) requests in a day 300000
            await Task.CompletedTask;
        }

        private async void Tick(object sender)
        {
            try
            {
                await Check();
            }
            catch (Exception e) // if exception then ignore messages
            {
                _logger.LogError(e.Message);
                var label = Service.Users.Labels.Get("me", labelID).ExecuteAsync().Result;
                lastEmailCount = label.MessagesTotal.Value;
            }
        }

        private async Task Check()
        {
            var label = Service.Users.Labels.Get("me", labelID).ExecuteAsync().Result;
            if (label.MessagesTotal > lastEmailCount)
            {
                _logger.LogInformation("New mail/Response");

                // Get list of messages
                var request = Service.Users.Messages.List("me");
                request.Q = "is:unread";
                request.Q = "label:UNIPI";
                var response = request.ExecuteAsync().Result;

                // Get the new messages only
                for (var i = 0; i < label.MessagesTotal.Value - lastEmailCount; i++)
                {
                    // Get message parts
                    var message = Service.Users.Messages.Get("me", response.Messages[i].Id).ExecuteAsync().Result;
                    var from = message.Payload.Headers.First(x => x.Name.Equals("From")).Value;
                    var subject = message.Payload.Headers.First(x => x.Name.Equals("Subject")).Value;
                    var body = Tools.Base64UrlDecode(message.Payload.Parts[0].Body.Data);
                    try
                    {
                        body = "Μήνυμα εκπαιδευτή:" + body.Split("Μήνυμα εκπαιδευτή:")[1];
                    }
                    catch (Exception)
                    {
                        //
                    }

                    if (body.Length > 1024) // embed limit
                        body = body.Substring(0, 1020) + "...";

                    if (string.IsNullOrEmpty(subject)) subject = "No Subject";

                    // Build embed message
                    var builder = new EmbedBuilder()
                        .WithAuthor(from, "https://ssl.gstatic.com/ui/v1/icons/mail/profile_mask2.png")
                        .WithColor(new Color(_rand.Next(256), _rand.Next(256), _rand.Next(256)))
                        .AddField("Subject: ", subject)
                        .AddField("Body:", body)
                        .WithTimestamp(DateTimeOffset.FromUnixTimeMilliseconds(message.InternalDate.Value).DateTime
                            .ToLocalTime());
                    var embed = builder.Build();

                    // Send message to every guild the bot is joined
                    foreach (var discordGuild in _discord.Guilds)
                        await discordGuild.DefaultChannel.SendMessageAsync(null, false, embed);
                }

                lastEmailCount = label.MessagesTotal.Value;
            }

            await Task.CompletedTask;
        }
    }
}