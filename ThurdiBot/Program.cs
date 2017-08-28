using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Threading;
using System.Linq;

namespace ThurdiBot {
    public class Program {
        public static void Main(string[] args) =>
            new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandHandler _handler;

        public async Task Start() {
            _client = new DiscordSocketClient();

            _handler = new CommandHandler(_client);

            //ThurdiBot
            await _client.LoginAsync(TokenType.Bot, "thurdiBot_Key");
            
            await _client.StartAsync();
            
            await Task.Delay(-1);
        }
    }
}