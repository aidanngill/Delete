using CommandLine;

/**
 * `Delete.exe -t (token) --min-delay (min-delay) --max-delay (max-delay) -d (dry-run)
 *  -> Basic arguments that apply to everything. `token` is required for every command.
 *
 * `Delete.exe all (guilds?|users?|blocked?`)
 *  -> Deletes everything from everywhere. All guilds, all users, including blocked.
 *  -> guilds -> Deletes all messages from every guild.
 *  -> users -> Deletes all messages from every user, including blocked.
 *  -> blocked -> Deletes all messages from every _blocked_ user, and **only** blocked users.
 *
 * `Delete.exe guild (id1) (id2)`
 *  -> Deletes from specific guilds.
 * 
 * `Delete.exe channel (id1) (id2)`
 *  -> Deletes from specific channels.
 */

namespace Delete
{
    public class BaseOptions
    {
        [Option('t', "token", Required = true, HelpText = "Discord user account token, acts as your password so don't share it!")]
        public string Token { get; set; }
        [Option("min-delay", Required = false, HelpText = "Minimum time to wait after deleting a message before continuing", Default = 5_000)]
        public int MinDelay { get; set; }
        [Option("max-delay", Required = false, HelpText = "Maximum time to wait after deleting a message before continuing", Default = 10_000)]
        public int MaxDelay { get; set; }
        [Option('d', "is-dry-run", Required = false, HelpText = "Counts up how many messages will be deleted without actually deleting anything", Default = false)]
        public bool IsDryRun { get; set; }
        [Option('o', "order", Required = false, HelpText = "Order to delete messages in, can be either 'asc' or 'desc'.", Default = "asc")]
        public string SortOrder { get; set; }
    }
    [Verb("all", HelpText = "Deletes all messages from the given choice")]
    public class AllOptions : BaseOptions
    {
        [Value(0, HelpText = "Valid values include '', 'guilds', 'users', 'blocked'.")]
        public string Choice { get; set; }
    }
    [Verb("guild", aliases: new[] { "guilds" }, HelpText = "Deletes messages from the given guilds")]
    public class GuildOptions : BaseOptions
    {
        [Value(0, HelpText = "List of guilds to delete from", Min = 1)]
        public IEnumerable<string> Guilds { get; set; }
    }
    [Verb("channel", aliases: new[] { "channels" }, HelpText = "Deletes messages from the given channels")]
    public class ChannelOptions : BaseOptions
    {
        [Value(0, HelpText = "List of channels to delete from", Min = 1)]
        public IEnumerable<string> Channels { get; set; }
    }
}
