using Newtonsoft.Json;

namespace Delete.Api
{
    internal enum MessageType
    {
        DEFAULT = 0,
        RECIPIENT_ADD = 1,
        RECIPIENT_REMOVE = 2,
        CALL = 3,
        CHANNEL_NAME_CHANGE = 4,
        CHANNEL_ICON_CHANGE = 5,
        CHANNEL_PINNED_MESSAGE = 6,
        GUILD_MEMBER_JOIN = 7,
        USER_PREMIUM_GUILD_SUBSCRIPTION = 8,
        USER_PREMIUM_GUILD_SUBSCRIPTION_TIER_1 = 9,
        USER_PREMIUM_GUILD_SUBSCRIPTION_TIER_2 = 10,
        USER_PREMIUM_GUILD_SUBSCRIPTION_TIER_3 = 11,
        CHANNEL_FOLLOW_ADD = 12,
        GUILD_DISCOVERY_DISQUALIFIED = 14,
        GUILD_DISCOVERY_REQUALIFIED = 15,
        GUILD_DISCOVERY_GRACE_PERIOD_INITIAL_WARNING = 16,
        GUILD_DISCOVERY_GRACE_PERIOD_FINAL_WARNING = 17,
        THREAD_CREATED = 18,
        REPLY = 19,
        CHAT_INPUT_COMMAND = 20,
        THREAD_STARTER_MESSAGE = 21,
        GUILD_INVITE_REMINDER = 22,
        CONTEXT_MENU_COMMAND = 23,
    }
    internal class Message
    {
        [JsonProperty("id")]
        public string ID { get; set; }
        [JsonProperty("channel_id")]
        public string ChannelID { get; set; }
        [JsonProperty("type")]
        public MessageType Type { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("author")]
        public User Author { get; set; }
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
        public override string ToString()
        {
            return $"[{Timestamp}] [{ID}] {Author}: {Content}";
        }
    }
}
