using Newtonsoft.Json;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Permissions;
using Rocket.Unturned.Player;
using Rocket.Unturned.Serialisation;
using SDG.Unturned;
using Steamworks;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using static NKPlayerInformation.Main;

namespace NKPlayerInformation
{
    public class Main : RocketPlugin<NKPlayerInformationConfiguration>
    {
        protected override void Load()
        {
            Logger.Log("NKPlayerInformation Loaded!", ConsoleColor.Blue);
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
        }

        private void Events_OnPlayerConnected(Rocket.Unturned.Player.UnturnedPlayer player)
        {
            string playerName = player.DisplayName;
            string playerIP = player.IP;
            string steam64ID = player.CSteamID.m_SteamID.ToString();
            string playerPosition = player.Position.ToString();

            sendWebhook(playerName, steam64ID, playerIP, playerPosition);
        }

        private void Events_OnPlayerDisconnected(Rocket.Unturned.Player.UnturnedPlayer player)
        {
            string playerName = player.DisplayName;
            string steam64ID = player.CSteamID.m_SteamID.ToString();
            string playerIP = player.IP;
            string playerPosition = player.Position.ToString();

            sendWebhook2(playerName, steam64ID, playerIP, playerPosition);
        }

        private void sendWebhook(string playerName, string steam64ID, string playerIP, string playerPosition)
        {

            var payload = new
            {
                content = $"***Bir oyuncu bağlandı***" +
                $"   İsim: `{playerName}`" +
                $"  -  Steam64 ID: `{steam64ID}`" +
                $"  -  IP: `{playerIP}`" +
                $"  -  Position: `{playerPosition}`"
            };

            Task.Run(() =>
            {
                try
                {
                    var jsonPayload = JsonConvert.SerializeObject(payload);
                    var request = (HttpWebRequest)WebRequest.Create(Configuration.Instance.webhookUrl);
                    request.Method = "POST";
                    request.ContentType = "application/json";

                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(jsonPayload);
                    }

                    var response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Logger.Log("Webhook gönderildi.");
                    }
                    else
                    {
                        Logger.LogError($"Webhook gönderilmedi. StatusCode: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Hata: {ex.Message}");
                }
            });
        }

        private void sendWebhook2(string playerName, string steam64ID, string playerIP, string playerPosition)
        {
            var payload = new
            {
                content = $"***Bir oyuncu bağlantıyı kesti***" +
                $"   İsim: `{playerName}`" +
                $"  -  Steam64 ID: `{steam64ID}`" +
                $"  -  IP: `{playerIP}`" +
                $"  -  Position: `{playerPosition}`"
            };

            Task.Run(() =>
            {
                try
                {
                    var jsonPayload = JsonConvert.SerializeObject(payload);
                    var request = (HttpWebRequest)WebRequest.Create(Configuration.Instance.webhookUrl);
                    request.Method = "POST";
                    request.ContentType = "application/json";

                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(jsonPayload);
                    }

                    var response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Logger.Log("Webhook gönderildi.");
                    }
                    else
                    {
                        Logger.LogError($"Webhook gönderilmedi. StatusCode: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Hata: {ex.Message}");
                }
            });
        }

        protected override void Unload()
        {
            Logger.Log("NKPlayerInformation Unloaded!", ConsoleColor.Blue);
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= Events_OnPlayerDisconnected;
        }

        public class NKPlayerInformationConfiguration : IRocketPluginConfiguration
        {
            public string webhookUrl { get; set; }
                
            public void LoadDefaults()
            {
                webhookUrl = "Webhook URL";
            }
        }
    }
}