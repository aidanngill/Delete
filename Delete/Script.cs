﻿using CommandLine;
using Delete.Api;
using Delete.Api.Response;
using Serilog;

namespace Delete
{
    internal class Script
    {
        private Account account;
        private BaseOptions options;
        private readonly List<Channel> channelList = new();
        private readonly List<Guild> guildList = new();
        private readonly Random random = new();
        private int dryRunCount = 0;
        private async Task RunAllGuildsOption()
        {
            foreach (var guild in await account.GetGuildsAsync())
            {
                guildList.Add(guild);
            }
        }
        private async Task RunAllUsersOption()
        {
            foreach (var channel in await account.GetUserChannels())
            {
                channelList.Add(channel);
            }
        }
        private async Task RunAllBlockedOption()
        {
            foreach (var relationship in await account.GetRelationshipsAsync(RelationshipType.BLOCKED))
            {
                Channel? maybeChannel = await account.GetChannelFromUser(relationship.User);

                if (maybeChannel != null)
                {
                    channelList.Add(maybeChannel);
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
            Log.Information("Started {DeleteOrCount} all messages with type '{Choice}'", Util.DeleteOrCount(options.DryRun), opts.Choice);

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
                guildList.Add(await account.GetGuildAsync(guild));
            }

            return 0;
        }
        private async Task<int> RunChannelOptions(ChannelOptions opts)
        {
            foreach (string channel in opts.Channels)
            {
                channelList.Add(await account.GetChannelAsync(channel));
            }

            return 0;
        }
        private async Task<int?> DeleteFromList(Guild guild)
        {
            Log.Information("Started {DeleteOrCount} messages from {Guild}", Util.DeleteOrCount(options.DryRun), guild);

            Search search;
            int? totalAmount = null;
            int deletedAmount = 0;

            do
            {
                search = await account.SearchMessages(guild, account.User, sortOrder: options.SortOrder);

                if (options.DryRun)
                {
                    return search.TotalResults;
                }

                if (totalAmount == null)
                {
                    totalAmount = search.TotalResults;
                }

                foreach (Message message in search.Messages)
                {

                    Channel channel = await account.GetChannelAsync(message.ChannelID);
                    await account.DeleteMessage(channel, message);
                    deletedAmount++;

                    Log.Information("[{Deleted}/{Total}] {MessageAuthor} to {Guild} ({Channel}): {MessageContent}",
                        deletedAmount, totalAmount, message.Author, guild, channel, message.Content);

                    await Task.Delay(random.Next(options.MinDelay, options.MaxDelay));
                }
            } while (search.TotalResults > 0);

            return null;
        }
        private async Task<int?> DeleteFromList(Channel channel)
        {
            Log.Information("Started {DeleteOrCount} messages from {Channel}", Util.DeleteOrCount(options.DryRun), channel);

            Search search;
            int? totalAmount = null;
            int deletedAmount = 0;

            do
            {
                search = await account.SearchMessages(channel, account.User, sortOrder: options.SortOrder);

                if (options.DryRun)
                {
                    return search.TotalResults;
                }

                if (totalAmount == null)
                {
                    totalAmount = search.TotalResults;
                }

                foreach (Message message in search.Messages)
                {
                    await account.DeleteMessage(channel, message);
                    deletedAmount++;

                    Log.Information("[{Deleted}/{Total}] {MessageAuthor} to {Channel}: {MessageContent}",
                        deletedAmount, totalAmount, message.Author, channel, message.Content);

                    await Task.Delay(random.Next(options.MinDelay, options.MaxDelay));
                }
            } while (search.TotalResults > 0);

            return null;
        }
        public async Task Run(string[] args)
        {
            options = Parser.Default.ParseArguments<BaseOptions>(args).Value;

            if (options == default)
            {
                return;
            }

            try
            {
                account = await Account.CreateAsync(options.Token);
                Log.Information("Successfully logged in as {User}", account.User);
            }
            catch (HttpRequestException ex)
            {
                Log.Fatal("Failed to log in with the given authorization ({StatusCode})", ex.StatusCode);
                return;
            }

            await Parser.Default.ParseArguments<AllOptions, GuildOptions, ChannelOptions>(args).MapResult(
                (AllOptions o) => RunAllOptions(o),
                (GuildOptions o) => RunGuildOptions(o),
                (ChannelOptions o) => RunChannelOptions(o),
                errs => Task.FromResult(0)
            );

            foreach (Guild guild in guildList)
            {
                int? count = await DeleteFromList(guild);

                if (options.DryRun)
                {
                    dryRunCount += count.Value;
                }
            }

            foreach (Channel channel in channelList)
            {
                int? count = await DeleteFromList(channel);

                if (options.DryRun)
                {
                    dryRunCount += count.Value;
                }

                await Task.Delay(random.Next(options.MinDelay, options.MaxDelay));
            }

            if (options.DryRun)
            {
                Log.Information("{Integer} messages would be deleted", dryRunCount);
            }
        }
    }
}