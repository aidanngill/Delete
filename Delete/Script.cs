using CommandLine;
using Delete.Api;
using Delete.Api.Request;
using Delete.Api.Response;
using Serilog;

namespace Delete
{
    internal class Script
    {
        private Account Account;
        private BaseOptions Options;
        private SearchOptions SearchOptions;
        private readonly List<Channel> ChannelList = new();
        private readonly List<Guild> GuildList = new();
        public int DeletedMessageCount { get; private set; }
        public Script()
        {
            DeletedMessageCount = 0;
        }
        private async Task RunAllGuildsOption()
        {
            foreach (var guild in await Account.GetGuildsAsync())
            {
                GuildList.Add(guild);
            }
        }
        private async Task RunAllUsersOption()
        {
            foreach (var channel in await Account.GetUserChannels())
            {
                ChannelList.Add(channel);
            }
        }
        private async Task RunAllBlockedOption()
        {
            foreach (var relationship in await Account.GetRelationshipsAsync(RelationshipType.BLOCKED))
            {
                Channel? maybeChannel = await Account.GetChannelFromUser(relationship.User);

                if (maybeChannel != null)
                {
                    ChannelList.Add(maybeChannel);
                }
            }
        }
        private async Task RunAllOption()
        {
            await RunAllGuildsOption();
            await RunAllUsersOption();
        }
        private async Task<int> RunAllOptions(AllOptions opts)
        {
            Log.Information("Started {DeleteOrCount} all messages with type '{Choice}'",
                Util.DeleteOrCount(Options.IsDryRun), opts.Choice);

            switch (opts.Choice.Trim().ToLower())
            {
                case "guilds": await RunAllGuildsOption(); break;
                case "users": await RunAllUsersOption(); break;
                case "blocked": await RunAllBlockedOption(); break;
                case "": await RunAllOption(); break;
            }

            return 0;
        }
        private async Task<int> RunGuildOptions(GuildOptions opts)
        {
            foreach (string guild in opts.Guilds)
            {
                GuildList.Add(await Account.GetGuildAsync(guild));
            }

            return 0;
        }
        private async Task<int> RunChannelOptions(ChannelOptions opts)
        {
            foreach (string channel in opts.Channels)
            {
                ChannelList.Add(await Account.GetChannelAsync(channel));
            }

            return 0;
        }
        private async Task<int?> DeleteFromList(Func<Task<Search>> lambdaProvideMessages, Guild? guild = null)
        {
            Search search;

            int? totalAmount = null;
            int deletedAmount = 0;

            do
            {
                search = await lambdaProvideMessages();

                if (Options.IsDryRun)
                {
                    return search.TotalResults;
                }

                if (totalAmount == null)
                {
                    totalAmount = search.TotalResults;
                }

                foreach (Message message in search.Messages)
                {
                    Channel channel = await Account.GetChannelAsync(message.ChannelID);
                    await Account.DeleteMessage(channel, message);

                    deletedAmount++;
                    DeletedMessageCount++;

                    if (guild != null)
                    {
                        Log.Information("[{Deleted}/{Total}] {MessageAuthor} to {Guild} ({Channel}): {MessageContent}",
                            deletedAmount, totalAmount, message.Author, guild, channel, message.Content);
                    }
                    else
                    {
                        Log.Information("[{Deleted}/{Total}] {MessageAuthor} to {Channel}: {MessageContent}",
                            deletedAmount, totalAmount, message.Author, channel, message.Content);
                    }

                    await Util.WaitRandomTime(Options.MinDelay, Options.MaxDelay);
                }
            } while (search.TotalResults > 0);

            return null;
        }
        private async Task<int?> DeleteFromList(Guild guild)
        {
            Log.Information("Started {DeleteOrCount} messages from {Guild}",
                Util.DeleteOrCount(Options.IsDryRun), guild);

            return await DeleteFromList(async () => await Account.SearchMessages(guild, SearchOptions), guild);
        }
        private async Task<int?> DeleteFromList(Channel channel)
        {
            Log.Information("Started {DeleteOrCount} messages from {Channel}",
                Util.DeleteOrCount(Options.IsDryRun), channel);

            return await DeleteFromList(async () => await Account.SearchMessages(channel, SearchOptions));
        }
        public async Task Run(string[] args)
        {
            Options = Parser.Default.ParseArguments<BaseOptions>(args).Value;

            if (Options == default)
            {
                return;
            }

            try
            {
                Account = await Account.CreateAsync(Options.Token);
                Log.Information("Successfully logged in as {User}", Account.User);
            }
            catch (HttpRequestException ex)
            {
                Log.Fatal("Failed to log in with the given authorization ({StatusCode})", ex.StatusCode);
                return;
            }

            await Parser
                .Default
                .ParseArguments<AllOptions, GuildOptions, ChannelOptions>(args)
                .MapResult(
                    (AllOptions o) => RunAllOptions(o),
                    (GuildOptions o) => RunGuildOptions(o),
                    (ChannelOptions o) => RunChannelOptions(o),
                    errs => Task.FromResult(0)
                );

            SearchOptions = new()
            {
                Author = Account.User,
                SortOrder = Options.SortOrder
            };

            int dryRunCount = 0;

            foreach (Guild guild in GuildList)
            {
                int? count = await DeleteFromList(guild);

                if (Options.IsDryRun)
                {
                    dryRunCount += count.Value;
                }

                await Task.Delay(1_000);
            }

            foreach (Channel channel in ChannelList)
            {
                int? count = await DeleteFromList(channel);

                if (Options.IsDryRun)
                {
                    dryRunCount += count.Value;
                }

                await Task.Delay(1_000);
            }

            if (Options.IsDryRun)
            {
                Log.Information("{Integer} messages would be deleted", dryRunCount);
            }
        }
    }
}
