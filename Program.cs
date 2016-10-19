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
using Discord.Rest;
using Newtonsoft.Json;
using System.Data;
using System.Reflection;
using System.ComponentModel;
using System.Configuration;

namespace DiscordLCD
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncContext.Run(() => MainAsync(args));
        }

        public static async void MainAsync(string[] args)
        {
            // Might need this stuff later
            var speakers = new List<ulong>();
            Dictionary<ulong, string> connectedUsers = new Dictionary<ulong, string>();
            string[] scopes = new string[] { "rpc", "rpc.api" };
            string rpcToken = "";

            // These variables are set using the App.config file
            ulong channelID = DiscordLCD.Properties.Settings.Default.channelID;
            ulong serverID = DiscordLCD.Properties.Settings.Default.guildID;
            string clientID = DiscordLCD.Properties.Settings.Default.clientID;
            string clientSecret = DiscordLCD.Properties.Settings.Default.clientSecret;
            string bearerToken = DiscordLCD.Properties.Settings.Default.bearerToken;
            bool resetKey = DiscordLCD.Properties.Settings.Default.reset;

            DiscordRpcClient client = new DiscordRpcClient(clientID, "http://127.0.0.1");
            RequestOptions requestOptions = new RequestOptions();

            // Initialize the LCD app
            LogitechGSDK.LogiLcdInit("DiscordLCD", LogitechGSDK.LOGI_LCD_TYPE_MONO | LogitechGSDK.LOGI_LCD_TYPE_COLOR);
            UpdateLCD(client, null, null, speakers, connectedUsers);

            Console.WriteLine(bearerToken);
            if(resetKey)
            {
                DiscordLCD.Properties.Settings.Default.Reset();
            }
            if (bearerToken == "")
            {
                // get auth code from Discord client
                string authcode = await client.AuthorizeAsync(scopes, rpcToken);
                
                // get token using authcode
                Discord.Net.Rest.DefaultRestClient restClient = new Discord.Net.Rest.DefaultRestClient("https://discordapp.com/api/");
                IReadOnlyDictionary<string, object> request = new Dictionary<string, object>
                {
                    { "client_id", clientID },
                    { "client_secret", clientSecret },
                    { "grant_type", "authorization_code" },
                    { "code", authcode },
                    { "redirect_uri", "http://127.0.0.1" }
                };
                
                var cancelToken = new System.Threading.CancellationToken();
                var restReponse = await restClient.SendAsync("POST", "oauth2/token", request, cancelToken, false);
                restReponse.Stream.Position = 0;
                var sr = new StreamReader(restReponse.Stream);
                var json = sr.ReadToEnd();
                Dictionary<string, string> response = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                bearerToken = response["access_token"];
                DiscordLCD.Properties.Settings.Default.bearerToken = bearerToken;
                DiscordLCD.Properties.Settings.Default.Save();
            }

            // Login and connect!
            await client.LoginAsync(TokenType.Bearer, bearerToken, false);
            await client.ConnectAsync();

            var server = await client.GetRpcGuildAsync(serverID);
            var serverName = server.Name;
            var channel = await client.GetRpcChannelAsync(channelID);
            var channelName = channel.Name;

            await client.SubscribeChannel(channelID, RpcChannelEvent.SpeakingStart);
            await client.SubscribeChannel(channelID, RpcChannelEvent.SpeakingStop);
            await client.SubscribeChannel(channelID, RpcChannelEvent.VoiceStateCreate);
            await client.SubscribeChannel(channelID, RpcChannelEvent.VoiceStateDelete);
            await client.SubscribeChannel(channelID, RpcChannelEvent.VoiceStateUpdate);

            
            
// i don't like the color of await warnings
#pragma warning disable 1998
            client.ApiClient.ReceivedRpcEvent += async (s1,s2,o1) => {
                Console.WriteLine("ReceivedRpcEvent");
                Console.WriteLine("s1: " + s1);
                Console.WriteLine("s2: " + s2);
                Console.WriteLine("o1: " + o1);

                Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(o1.ToString());

                if (s2.ToString() == "SPEAKING_START")
                {
                    ulong userID = (ulong)jObject["user_id"];
                    Console.WriteLine(userID);
                    if(speakers.IndexOf(userID) == -1)
                    {
                        speakers.Add(userID);
                    }
                }
                if (s2.ToString() == "SPEAKING_STOP")
                {
                    ulong userID = (ulong)jObject["user_id"];
                    Console.WriteLine(userID);
                    if (speakers.IndexOf(userID) != -1)
                    {
                        speakers.Remove(userID);
                    }
                    Console.WriteLine("Users: " + connectedUsers.Count);
                }
                if(s2.ToString() == "VOICE_STATE_UPDATE")
                {
                    string value;
                    if (!connectedUsers.TryGetValue((ulong)jObject["user"]["id"], out value))
                    {
                        var name = jObject["nick"].ToString() ?? jObject["user"]["username"].ToString();
                        connectedUsers.Add((ulong)jObject["user"]["id"], name);
                        Console.WriteLine("User added to connectedUsers. Total: " + connectedUsers.Count);
                    }
                }
                if(s2.ToString() == "VOICE_STATE_DELETE")
                {
                    if (jObject["user"]["username"].ToString() == client.CurrentUser.Username)
                    {
                        speakers = new List<ulong>();
                    }
                }
                if(s2.ToString() == "VOICE_STATE_CREATE")
                {
                    //
                }

                // Refresh the LCD screen
                UpdateLCD(client, serverName, channelName, speakers, connectedUsers);
            };
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
            client.MessageReceived += async (m) => {
                Console.WriteLine("MessageReceived: " + m.ToString());
            };
#pragma warning restore 1998




            Console.WriteLine("Current Status: " + client.ConnectionState);

            // Wait for keypress before closing
            Console.Read();
        }

        private static void UpdateLCD(DiscordRpcClient client, string serverName, string channelName, List<ulong> speakers, Dictionary<ulong, string> connectedUsers)
        {
            Console.WriteLine("Length: " + speakers.Count);

            string line0 = "";
            string line1 = "";
            string line2 = "";
            string line3 = "";

            // put all the speakers in a string. create a copy of the list first
            string speakersString = "";
            foreach (ulong s in speakers.ToList())
            {
                string user;
                if (connectedUsers.TryGetValue(s, out user))
                {
                    speakersString += user + " ";
                }
            }
            if (speakersString.Length != 0)
            {
                speakersString = "🎤" + speakersString;
            }

            line0 = serverName ?? "No server";
            line1 = channelName ?? "No channel";
            line2 = speakersString;
            line3 = client.ConnectionState.ToString();

            Console.WriteLine("speakerstring: " + speakersString);

            LogitechGSDK.LogiLcdMonoSetText(0, line0);
            LogitechGSDK.LogiLcdMonoSetText(1, line1);
            LogitechGSDK.LogiLcdMonoSetText(2, line2);
            LogitechGSDK.LogiLcdMonoSetText(3, line3);
            LogitechGSDK.LogiLcdUpdate();
        }
    }
}
