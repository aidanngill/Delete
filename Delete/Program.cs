using Serilog;
using Serilog.Events;

namespace Delete
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ramadan", "delete");
            Directory.CreateDirectory(path);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.File(
                    Path.Join(path, "log.txt"),
                    restrictedToMinimumLevel: LogEventLevel.Debug
                )
                .CreateLogger();

            var script = new Script();

            Console.CancelKeyPress += delegate
            {
                if (script.deletedMessageCount > 0)
                {
                    Log.Information("Successfully deleted {Amount} messages", script.deletedMessageCount);
                }
            };

            try
            {
                await script.Run(args);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
