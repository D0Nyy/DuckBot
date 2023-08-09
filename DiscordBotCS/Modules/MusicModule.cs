using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Victoria;
using Victoria.Enums;

namespace DiscordBotCS.Modules
{
    [Summary("MusicBot Commands")]
    [Name("Music")]
    public class MusicModule : ModuleBase<SocketCommandContext>
    {
        private readonly LavaNode _lavaNode;

        public MusicModule(LavaNode lavaNode)
        {
            _lavaNode = lavaNode;
        }

        [Summary("Makes the bot join the voice channel you are in.")]
        [Command("Join", RunMode = RunMode.Async)]
        public async Task JoinAsync()
        {
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm already connected to a voice channel!");
                return;
            }

            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"Joined {voiceState.VoiceChannel.Name}!");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [Summary("Makes the bot leave the voice channel you are in.")]
        [Command("Leave", RunMode = RunMode.Async)]
        public async Task LeaveAsync()
        {
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I am not connected to a channel");
                return;
            }

            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("I am not connected to a channel");
                return;
            }

            try
            {
                await _lavaNode.LeaveAsync(voiceState.VoiceChannel);
                await ReplyAsync($"Left {voiceState.VoiceChannel.Name}!");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [Summary("Play the specified song.")]
        [Command("Play", RunMode = RunMode.Async)]
        public async Task PlayAsync([Summary("The song")] [Remainder] string query)
        {
            //var thumbnail = await track.FetchArtworkAsync()  //<--- VIDEO THUMBNAIL

            if (string.IsNullOrWhiteSpace(query))
            {
                await ReplyAsync("Please provide search terms.");
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            var searchResponse = await _lavaNode.SearchYouTubeAsync(query);
            if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                searchResponse.LoadStatus == LoadStatus.NoMatches)
            {
                await ReplyAsync($"I wasn't able to find anything for `{query}`.");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);

            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
            {
                var track = searchResponse.Tracks[0];

                player.Queue.Enqueue(track);
                await ReplyAsync($"Enqueued: {track.Title}");
            }
            else
            {
                var track = searchResponse.Tracks[0];

                await player.PlayAsync(track);
                await ReplyAsync($"Now Playing: {track.Title}");
            }
        }

        [Summary("Skip current song and play the next one in the queue.")]
        [Command("Skip", RunMode = RunMode.Async)]
        public async Task Skip()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel!");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("You need to be to the same voice channel as me!");
                return;
            }

            if (player.Queue.Count == 0)
            {
                await ReplyAsync("There are no more songs in the queue");
                return;
            }

            await player.SkipAsync();
            await ReplyAsync($"Skipped! Now playing `{player.Track.Title}`");
        }

        [Summary("Pause the currently playing song.")]
        [Command("Pause", RunMode = RunMode.Async)]
        public async Task Pause()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel!");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("You need to be to the same voice channel as me!");
                return;
            }

            if (player.PlayerState == PlayerState.Paused || player.PlayerState == PlayerState.Stopped)
            {
                await ReplyAsync("Music is already paused.");
                return;
            }

            await player.PauseAsync();
            await ReplyAsync("Paused music.");
        }

        [Summary("Resume the paused song.")]
        [Command("Resume", RunMode = RunMode.Async)]
        public async Task Resume()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel!");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("You need to be to the same voice channel as me!");
                return;
            }

            if (player.PlayerState == PlayerState.Playing)
            {
                await ReplyAsync("Music is already playing.");
                return;
            }

            await player.ResumeAsync();
            await ReplyAsync("Reasumed the music.");
        }
    }
}