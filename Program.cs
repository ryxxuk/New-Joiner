using System.Threading.Tasks;
using Astro_New_Joiner.Functions;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Astro_New_Joiner
{
    public class Program
    {
        static void Main()
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var services = ConfigureServices();

            var client = services.GetRequiredService<DiscordSocketClient>();
            services.GetRequiredService<CommandService>();
            services.GetRequiredService<LoggingService>();

            // Get the bot token from the Config.jsFunctions.GetConfig();
            var config = Functions.Functions.GetConfig();
            var token = config["token"].Value<string>();

            // Log in to Discord and start the bot.
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            // Run the bot forever.
            await Task.Delay(-1);
        }

        public static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                { 
                    MessageCacheSize = 500,
                    LogLevel = LogSeverity.Info
                }))
                .AddSingleton(new CommandService(new CommandServiceConfig
                { 
                    LogLevel = LogSeverity.Info,
                    DefaultRunMode = RunMode.Async,
                    CaseSensitiveCommands = false 
                }))
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<LoggingService>()
                .BuildServiceProvider();
        }
    }
}