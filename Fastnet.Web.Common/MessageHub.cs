using Fastnet.Common;
using Fastnet.EventSystem;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastnet.Web.Common
{
    public static class HubRegister
    {
        public enum ClientType
        {
            Browser,
            DotNet
        }
        public class HubClient
        {
            public string ConnectionId { get; set; }
            public ClientType ClientType { get; set; }
            public string Name { get; set; }
        }
        public static Dictionary<string, HubClient> Connections = new Dictionary<string, HubClient>();
    }
    public class MessageHub : Hub
    {
        public static void Initialise()
        {
            if (ApplicationSettings.Key("MessageHubTracing", false))
            {
                Task.Run(async () =>
                {
                    //int count = 100;
                    int delay = 2000;
                    while (true)
                    {
                        await Task.Delay(delay);
                        MessageHubInformation mhi = new MessageHubInformation();
                        mhi.Send();
                    }
                });
            }
        }
        public override Task OnConnected()
        {
            Debug.Print("MessageHub: OnConnected - {0}", this.Context.ConnectionId);
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            Debug.Print("MessageHub: OnDisconnected - {0}", this.Context.ConnectionId);
            if (HubRegister.Connections.ContainsKey(this.Context.ConnectionId))
            {
                HubRegister.Connections.Remove(this.Context.ConnectionId);
            }
            return base.OnDisconnected(stopCalled);
        }
        public override Task OnReconnected()
        {
            Debug.Print("MessageHub: OnReconnected - {0}", this.Context.ConnectionId);
            return base.OnReconnected();
        }
        public void Register(dynamic data)
        {
            string connectionId = data.connectionId;
            string clientType = data.clientType;
            string name = data.name;
            HubRegister.ClientType ct = (HubRegister.ClientType)Enum.Parse(typeof(HubRegister.ClientType), clientType, true);
            var client = new HubRegister.HubClient { Name = name, ConnectionId = connectionId, ClientType = ct };
            HubRegister.Connections.Add(connectionId, client);
            Debug.Print("Registered client {0}: {1}", connectionId, clientType);
        }
    }
}
