using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.IO;
using Nito.AsyncEx;
using Discord;
using Discord.Rpc;
using Newtonsoft.Json;

namespace DiscordLCD
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncContext.Run(() => MainAsync(args));
        }

        static async void MainAsync(string[] args)
        {
            bool done = false;
            string line0 = "";
            string line1 = "";
            string line2 = "";
            string line3 = "";
            LogitechGSDK.LogiLcdInit("TEST", LogitechGSDK.LOGI_LCD_TYPE_MONO | LogitechGSDK.LOGI_LCD_TYPE_COLOR);

            string clientId = "229746606205829120";
            string[] scopes = new string[] { "rpc", "rpc.api" };
            string rpcToken = "";

            DiscordRpcClient client = new DiscordRpcClient(clientId, "http://127.0.0.1");

            /* Comment out if hardcoding bearertoken
            
            string token = await client.AuthorizeAsync(scopes, rpcToken);
            Discord.Net.Rest.DefaultRestClient restClient = new Discord.Net.Rest.DefaultRestClient("https://discordapp.com/api/");

            IReadOnlyDictionary<string, object> request = new Dictionary<string, object>
            {
                { "client_id", "229746606205829120" },
                { "client_secret", "AKt3xJ2-RCQ2Msgxicte1ER1QFXR-RT9" },
                { "grant_type", "authorization_code" },
                { "code", token },
                { "redirect_uri", "http://127.0.0.1" }

            };
            
            var stream = await restClient.SendAsync("POST", "oauth2/token", request);
            stream.Position = 0;
            var sr = new StreamReader(stream);
            var json = sr.ReadToEnd();

            Dictionary<string, string> response = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            string bearerToken = response["access_token"];
            */

            string bearerToken = "QfwHsIT5g4HPdVTxOD7x3LG9bdYFDa";

            await client.LoginAsync(TokenType.Bearer, bearerToken, false);
            await client.ConnectAsync();

            var guild = await client.GetGuildAsync(140540231567933440);
            var x = await client.GetChannelAsync(229756130098806785);

            ulong channelId = 230144259603431424;

            await client.SubscribeChannel(channelId, RpcChannelEvent.SpeakingStart);
            await client.SubscribeChannel(channelId, RpcChannelEvent.SpeakingStop);
            await client.SubscribeChannel(channelId, RpcChannelEvent.VoiceStateCreate);
            await client.SubscribeChannel(channelId, RpcChannelEvent.VoiceStateDelete);
            await client.SubscribeChannel(channelId, RpcChannelEvent.VoiceStateUpdate);


            client.ApiClient.ReceivedRpcEvent += async (s1,s2,o1) => {
                Console.WriteLine("ReceivedRpcEvent");
                Console.WriteLine("s1: " + s1);
                Console.WriteLine("s2: " + s2);
                Console.WriteLine("o1: " + o1);
                line2 = s2.ToString();
            };
            client.Log += async (logMessage) =>
            {
                Console.WriteLine("Log Event");
            };
            client.Ready += async () =>
            {
                Console.WriteLine("Ready Event");
            };
            client.Connected += async () =>
            {
                Console.WriteLine("Connected Event");
            };
            client.Disconnected += async (e) =>
            {
                Console.WriteLine("Disconnected Event");
            };
            client.VoiceStateUpdated += async () =>
            {
                Console.WriteLine("VoiceStateUpdated");
            };
            client.MessageReceived += async (s, e) => {
                Console.WriteLine(e.Channel.ToString());
            };



            Console.WriteLine("Current Status: " + client.ConnectionState);
            line1 = "0";

            while (!done)
            {
                LogitechGSDK.LogiLcdUpdate();

                //string text = Console.ReadLine();
                //if (text.Equals("#!done"))
                //{
                //    LogitechGSDK.LogiLcdShutdown();
                //    done = true;
                //}
                //line0 = line1;
                //line1 = line2;
                line3 = client.ConnectionState.ToString();

                int l1 = Int32.Parse(line1);
                l1++;
                line1 = l1.ToString();

                LogitechGSDK.LogiLcdMonoSetText(0, line0);
                LogitechGSDK.LogiLcdMonoSetText(1, line1);
                LogitechGSDK.LogiLcdMonoSetText(2, line2);
                LogitechGSDK.LogiLcdMonoSetText(3, line3);
            }
            LogitechGSDK.LogiLcdShutdown();
        }
    }
}
