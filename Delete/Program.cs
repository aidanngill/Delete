using Serilog;
using Serilog.Events;

namespace Delete
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.File("./log.txt", restrictedToMinimumLevel: LogEventLevel.Debug, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            var script = new Script();

            try
            {
                await script.Run(args);
            }
            finally
            {
                // Print out final stats here


                Log.CloseAndFlush();
            }
        }
    }
}
