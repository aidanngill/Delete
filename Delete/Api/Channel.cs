namespace Delete.Api
{
    public enum ChannelType
    {
        GUILD_TEXT = 0,
        DM = 1,
        GROUP_DM = 3,
        GUILD_PUBLIC_THREAD = 11,
        GUILD_PRIVATE_THREAD = 12,
    }
    internal class Channel
    {
        public string ID { get; set; }
        public ChannelType Type { get; set; }
        public string? GuildID { get; set; }
        public int Position { get; set; }
        public string? Name { get; set; }
        public string? Topic { get; set; }
        public bool? NSFW { get; set; }
        public List<User>? Recipients { get; set; }
        public override string ToString()
        {
            if (Recipients != null && Recipients.Count > 0)
            {
                return string.Join(", ", Recipients);
            }
            else
            {
                return $"#{Name}";
            }
        }
    }
}
