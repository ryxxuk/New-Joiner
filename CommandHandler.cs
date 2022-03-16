using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Astro_New_Joiner
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;

        public CommandHandlingService(IServiceProvider services)
        {

            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            // Event handlers
            _client.Ready += ClientReadyAsync;
            _client.UserJoined += JoinedGuildAysnc;
        }

        private async Task JoinedGuildAysnc(SocketGuildUser arg)
        {
            if (arg.IsBot) return;

            var channel = await arg.Guild.CreateTextChannelAsync($"Welcome - {arg.Username}", prop => prop.CategoryId = 832250645310341150);

            await channel.AddPermissionOverwriteAsync(arg, new OverwritePermissions(viewChannel:PermValue.Allow, sendMessages:PermValue.Allow)); // 640106665986752553

            await channel.SendMessageAsync($"||<@!640029374040768537> <@&810238562053914695> <@&833408645898829845>|| \nWelcome <@{arg.Id}> to the server!");

            _ = Task.Run(() =>
              {
                  Thread.Sleep(30 * 1000);
                  channel.SendMessageAsync($"Hey <@{arg.Id}>!\nMake sure you head over to <#852105825144930345> and setup your roles to ensure you get the most of out of the server.\nAlso make sure you've read the guide here: https://guides.astroalerts.co.uk/ \nWe also have a handy video guide for getting started here: https://youtu.be/J9Vei6piWh0 \nIf you have any queries or questions about starting, please ask them here!\n\nThis is the first step of your reselling journey, here's to many more.");
                  var logText = $"{DateTime.UtcNow:hh:mm:ss} [Msg] New joiner channel created for {arg.Username}";
                  Console.Out.WriteLineAsync(logText); // Write the log text to the console
            });
        }

        private async Task ClientReadyAsync()
            => await Functions.Functions.SetBotStatusAsync(_client);

        public async Task InitializeAsync()
            => await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    }
}