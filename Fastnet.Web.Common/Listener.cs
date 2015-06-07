using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastnet.Web.Common
{
    public class Listener
    {
        private static Dictionary<string, ListenerConnection> listeners = new Dictionary<string, ListenerConnection>();
        private class ListenerConnection
        {
            public HubConnection HubConnection { get; set; }
            public IHubProxy Proxy { get; set; }
        }
        private ListenerConnection _listener;
        public Listener()
        {
            //if (!listeners.ContainsKey(url))
            //{
            //    var connection = new HubConnection(url);
            //    ListenerConnection lc = new ListenerConnection
            //    {
            //        HubConnection = connection,
            //        Proxy = connection.CreateHubProxy("MessageHub")
            //    };
            //    listeners.Add(url, lc);
            //    lc.HubConnection.Start();
            //    lc.Proxy.Invoke("Register", new { connectionId = lc.HubConnection.ConnectionId, clientType = "DotNet" });
            //}
            //_listener = listeners[url];
        }
        public async Task Connect(string url, string name)
        {
            if (!listeners.ContainsKey(url))
            {
                var connection = new HubConnection(url);
                ListenerConnection lc = new ListenerConnection
                {
                    HubConnection = connection,
                    Proxy = connection.CreateHubProxy("MessageHub")
                };
                listeners.Add(url, lc);
                await lc.HubConnection.Start();
                await lc.Proxy.Invoke("Register", new { name = name, connectionId = lc.HubConnection.ConnectionId, clientType = "DotNet" });
            }
            _listener = listeners[url];
        }
        public IDisposable Add(Action<dynamic> onNotification)
        {
            return _listener.Proxy.On<dynamic>("receiveMessage", data => onNotification(data));
        }
        public IDisposable Add<T>(Action<T> onNotification)
        {
            return _listener.Proxy.On<T>("receiveMessage", data => onNotification(data));
        }
        //private async Task Start()
        //{            
        //    await connection.Start();
        //}
    }
}
