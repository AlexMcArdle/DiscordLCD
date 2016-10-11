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
using Discord.API;
using Newtonsoft.Json;
using System.Data;
using System.Reflection;

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
            var connectedUsers = new Dictionary<ulong, string>();
            var speakers = new List<ulong>();
            ulong currentChannel;
            ulong previousChannel;
            string line0 = "";
            string line1 = "";
            string line2 = "";
            string line3 = "";
            LogitechGSDK.LogiLcdInit("TEST", LogitechGSDK.LOGI_LCD_TYPE_MONO | LogitechGSDK.LOGI_LCD_TYPE_COLOR);


            string clientId = "229746606205829120";
            string[] scopes = new string[] { "rpc", "rpc.api" };
            string rpcToken = "";

            DiscordRpcClient client = new DiscordRpcClient(clientId, "http://127.0.0.1");
            //DiscordRpcConfig config = new DiscordRpcConfig();


            RequestOptions requestOptions = new RequestOptions();

            // Comment out if hardcoding bearertoken
            string token = await client.AuthorizeAsync(scopes, rpcToken);

            Discord.Net.Rest.DefaultRestClient restClient = new Discord.Net.Rest.DefaultRestClient("https://discordapp.com/api/");

            IReadOnlyDictionary<string, object> request = new Dictionary<string, object>
            {
                { "client_id", clientId },
                { "client_secret", "AKt3xJ2-RCQ2Msgxicte1ER1QFXR-RT9" },
                { "grant_type", "authorization_code" },
                { "code", token },
                { "redirect_uri", "http://127.0.0.1" }

            };

            var stream = await restClient.SendAsync("POST", "oauth2/token", request, requestOptions);
            stream.Position = 0;
            var sr = new StreamReader(stream);
            var json = sr.ReadToEnd();

            Dictionary<string, string> response = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            string bearerToken = response["access_token"];
            

            //string bearerToken = "QfwHsIT5g4HPdVTxOD7x3LG9bdYFDa";

            await client.LoginAsync(TokenType.Bearer, bearerToken, false);
            await client.ConnectAsync();

            ulong channelId = 229756130098806785;
            
            await client.SubscribeChannel(channelId, RpcChannelEvent.SpeakingStart);
            await client.SubscribeChannel(channelId, RpcChannelEvent.SpeakingStop);
            await client.SubscribeChannel(channelId, RpcChannelEvent.VoiceStateCreate);
            await client.SubscribeChannel(channelId, RpcChannelEvent.VoiceStateDelete);
            await client.SubscribeChannel(channelId, RpcChannelEvent.VoiceStateUpdate);

#pragma warning disable 1998
            client.ApiClient.ReceivedRpcEvent += async (s1,s2,o1) => {
                Console.WriteLine("ReceivedRpcEvent");
                Console.WriteLine("s1: " + s1);
                Console.WriteLine("s2: " + s2);
                Console.WriteLine("o1: " + o1);
                line2 = s2.ToString();

                if (s2.ToString() == "SPEAKING_START")
                {
                    var speakEvent = JsonConvert.DeserializeObject<Dictionary<String, ulong>>(o1.ToString());
                    ulong userID = speakEvent["user_id"];
                    Console.WriteLine(userID);
                    if(speakers.IndexOf(userID) == -1)
                    {
                        speakers.Add(userID);
                    }
                    
                }
                if (s2.ToString() == "SPEAKING_STOP")
                {
                    var speakEvent = JsonConvert.DeserializeObject<Dictionary<String, ulong>>(o1.ToString());
                    ulong userID = speakEvent["user_id"];
                    Console.WriteLine(userID);
                    if (speakers.IndexOf(speakEvent["user_id"]) != -1)
                    {
                        speakers.Remove(userID);
                    }
                    Console.WriteLine("Users: " + connectedUsers.Count);
                }
                if(s2.ToString() == "VOICE_STATE_UPDATE")
                {
                    Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(o1.ToString());
                    string value;
                    if (!connectedUsers.TryGetValue((ulong)jObject["user"]["id"], out value))
                    {
                        connectedUsers.Add((ulong)jObject["user"]["id"], (string)jObject["user"]["username"]);
                        Console.WriteLine("User added");
                    }
                }
            };
#pragma warning restore 1998

            client.Log += async (logMessage) =>
            {
                Console.WriteLine("Log Event");
                Console.WriteLine(logMessage);
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
            client.VoiceStateUpdated += async (state) =>
            {
                Console.WriteLine("VoiceStateUpdated: " + state);
            };
            client.VoiceStateCreated += async (state) =>
            {
                Console.WriteLine("VoiceStateCreated: " + state);
            };
            client.VoiceStateDeleted += async (state) =>
            {
                Console.WriteLine("VoiceStateDeleted: " + state);
            };
            //client.MessageReceived += async (s, e) => {
            //    Console.WriteLine(e.Channel.ToString());
            //};



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

                // just a counter
                //int l1 = Int32.Parse(line1);
                //l1++;
                //line1 = l1.ToString();

                // put all the speakers in a string. create a copy of the list first
                string speakersString = " ";
                foreach (ulong s in speakers.ToList())
                {
                    string user;
                    if (connectedUsers.TryGetValue(s, out user))
                    {
                        speakersString += user + " ";
                    }
                }

                line2 = speakersString;

                LogitechGSDK.LogiLcdMonoSetText(0, line0);
                LogitechGSDK.LogiLcdMonoSetText(1, line1);
                LogitechGSDK.LogiLcdMonoSetText(2, line2);
                LogitechGSDK.LogiLcdMonoSetText(3, line3);
            }
            LogitechGSDK.LogiLcdShutdown();
        }
    }
}
