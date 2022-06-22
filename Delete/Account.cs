using Delete.Api;
using Delete.Api.Request;
using Newtonsoft.Json;
using Serilog;
using System.Net.Http.Headers;
using System.Web;

namespace Delete
{
    internal class Account
    {
        public User User;
        private readonly HttpClient Http;
        private Account(User user, HttpClient http)
        {
            User = user;
            Http = http;
        }
        public static async Task<Account> CreateAsync(string authorization)
        {
            HttpClient http = new();

            http.BaseAddress = new Uri("https://discord.com");
            http.DefaultRequestHeaders.Add("Authorization", authorization);
            http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; CrOS x86_64 8172.45.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.64 Safari/537.36");
            http.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var res = await http.GetAsync("/api/v9/users/@me");
            res.EnsureSuccessStatusCode();

            string content = await res.Content.ReadAsStringAsync();
            User? user = JsonConvert.DeserializeObject<User>(content);

            Log.Debug("Successfully logged in as {User}", user);

            return new Account(user, http);
        }
        public async Task<List<Relationship>> GetRelationshipsAsync(RelationshipType relationshipType = RelationshipType.NORMAL)
        {
            var res = await Http.GetAsync("/api/v9/users/@me/relationships");
            res.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<List<Relationship>>(await res.Content.ReadAsStringAsync()).FindAll(e => e.Type == relationshipType);
        }
        public async Task<Guild> GetGuildAsync(string guildID)
        {
            var res = await Http.GetAsync($"/api/v9/guilds/{guildID}");
            res.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<Guild>(await res.Content.ReadAsStringAsync());
        }
        private List<Channel> channelCache = new();
        public async Task<Channel> GetChannelAsync(string channelID)
        {
            foreach (Channel channel in channelCache)
            {
                if (channel.ID == channelID)
                {
                    return channel;
                }
            }

            var res = await Http.GetAsync($"/api/v9/channels/{channelID}");
            res.EnsureSuccessStatusCode();

            Channel foundChannel = JsonConvert.DeserializeObject<Channel>(await res.Content.ReadAsStringAsync());
            channelCache.Add(foundChannel);

            return foundChannel;
        }
        public async Task<List<Guild>> GetGuildsAsync()
        {
            var res = await Http.GetAsync("/api/v9/users/@me/guilds");
            res.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<List<Guild>>(await res.Content.ReadAsStringAsync());
        }
        public async Task<List<Channel>> GetChannelsAsync(string guildID)
        {
            var res = await Http.GetAsync($"/api/v9/guild/{guildID}/channels");
            res.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<List<Channel>>(await res.Content.ReadAsStringAsync());
        }
        public async Task<List<Channel>> GetChannelsAsync(Guild guild)
        {
            return await GetChannelsAsync(guild.ID);
        }
        public async Task<Api.Response.Search> SearchMessages( string mode, string modeID, SearchOptions searchOptions)
        {
            var res = await Http.GetAsync($"/api/v9/{mode}/{modeID}/messages/search?{searchOptions}");
            res.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<Api.Response.Search>(await res.Content.ReadAsStringAsync());
        }
        public async Task<Api.Response.Search> SearchMessages(Guild guild, SearchOptions searchOptions)
        {
            return await SearchMessages("guilds", guild.ID, searchOptions);
        }
        public async Task<Api.Response.Search> SearchMessages(Channel channel, SearchOptions searchOptions)
        {
            return await SearchMessages("channels", channel.ID, searchOptions);
        }
        public async Task<List<Channel>> GetUserChannels()
        {
            var res = await Http.GetAsync("/api/v9/users/@me/channels");
            res.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<List<Channel>>(await res.Content.ReadAsStringAsync());
        }
        public async Task<Channel?> GetChannelFromUser(User user)
        {
            foreach (Channel channel in await GetUserChannels())
            {
                if (channel.Recipients == null)
                {
                    continue;
                }

                if (channel.Recipients.Count != 1)
                {
                    continue;
                }

                if (channel.Recipients.First().ID != user.ID)
                {
                    continue;
                }

                return channel;
            }

            Console.WriteLine($"No channel detected for {user}");

            return null;
        }
        public async Task DeleteMessage(Channel channel, Message message)
        {
            var res = await Http.DeleteAsync($"/api/v9/channels/{channel.ID}/messages/{message.ID}");
            res.EnsureSuccessStatusCode();
        }
    }
}
