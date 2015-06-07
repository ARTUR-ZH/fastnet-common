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
    public abstract class MessageBase
    {
        private static bool traceMessages = ApplicationSettings.Key("MessageHub:TraceMessages", false);
        public string Machine { get; set; }
        public int AppDomainId { get; protected set; }
        public int ProcessId { get; set; }
        public string Ident { get; private set; }
        public int Index { get; protected set; }

        public MessageBase(int index)
        {
            this.Index = index;
            Ident = this.GetType().Name;
            Machine = Environment.MachineName.ToLower();
            AppDomainId = AppDomain.CurrentDomain.Id;
            ProcessId = System.Diagnostics.Process.GetCurrentProcess().Id;
        }
        public void Send()
        {
            MessageBase.Send(this);
        }
        //public static void Send(dynamic data)
        //{
        //    IHubContext htx = GlobalHost.ConnectionManager.GetHubContext<MessageHub>();
        //    //Debug.Print("Sending {0} to {1} clients", (string)data.Ident, HubRegister.Connections.Count());
        //    htx.Clients.All.receiveMessage(data);
        //}
        public static void Send(MessageBase data)
        {            
            IHubContext htx = GlobalHost.ConnectionManager.GetHubContext<MessageHub>();
            //Debug.Print("Sending {0} to {1} clients", (string)data.Ident, HubRegister.Connections.Count());
            htx.Clients.All.receiveMessage(data);
            if (traceMessages)
            {
                Log.Write("MessageHub to {0} clients: {1}", HubRegister.Connections.Count(), data.ToString());
            }
        }
        public override string ToString()
        {
            return string.Format("Message {0}({1}) [{2}:{3}:{4}]", Ident, Index, Machine, ProcessId, AppDomainId);
        }
    }
    public class MessageHubInformation : MessageBase
    {
        private static int count;
        //public string Machine { get; private set; }
        //public int ProcessId { get; private set; }
        public int ClientTotal { get; private set; }
        public string Clients { get; set; }
        public MessageHubInformation InnerMessage { get; set; }
        //public string Message { get; set; }
        public MessageHubInformation()
            : base(++count)
        {
            //Machine = Environment.MachineName.ToLower();
            //ProcessId = System.Diagnostics.Process.GetCurrentProcess().Id;
            ClientTotal = HubRegister.Connections.Count();
            var names = HubRegister.Connections.GroupBy(g => g.Value.Name)
                .Select(x => new { Name = x.Key, Count = x.Count() })
                .Select(y => string.Format("{0} x {1}", y.Count, y.Name))
                .ToArray();
            Clients = string.Join(", ", names);
        }
        public override string ToString()
        {
            return string.Format("{0}: Clients: {1} (total {2}){3}", base.ToString(), Clients, ClientTotal, InnerMessage != null ? " << " + InnerMessage.ToString() : "");
        }
    }
}
