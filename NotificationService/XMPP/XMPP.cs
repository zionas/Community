using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using Matrix;
using Matrix.Extensions.Client.Roster;
using Matrix.Network.Resolver;
using Matrix.Xml;
using Matrix.Xmpp;
using Matrix.Xmpp.Avatar;
using Matrix.Xmpp.Client;
using Matrix.Xmpp.Roster;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NotificationService.XMPP
{
    class XMPPClient
    {
        XmppClient client;
        IChannel channel;

        public event EventHandler<EventArgs<Message>> MessageAvailable;
        const string xmppDomain = "DESKTOP-I6T30NO";
        public XMPPClient()
        {
            var pipelineInitializerAction = new Action<IChannelPipeline>(pipeline =>
            {
                pipeline.AddFirst(new XMPPLogHandler());
            });

            client = new XmppClient(pipelineInitializerAction)
            {
                Tls = false,
                XmppDomain = xmppDomain ,
                Resource = "",
                HostnameResolver = new StaticNameResolver(IPAddress.Parse("127.0.0.1"), 5222)
            };
            client.XmppSessionStateObserver.Subscribe(OnSessionStateChanged);
            client.XmppXElementStreamObserver.Subscribe(OnXElementStreamChanged);
        }

        public  async  Task Connect(string userName, string password)
        {
            client.Username = userName;
            client.Password = password;
            channel = await client.ConnectAsync();
            await client.SendAsync(new Presence());// (Show.Chat));
        }

        public async Task Register(string userName, string password)
        {
            client.RegistrationHandler = new RegisterAccountHandler(client);
            client.Username = userName;
            client.Password = password;
            channel = await client.ConnectAsync();
            await client.SendAsync(new Presence()); //Show.Chat));
        }

        public async Task<IEnumerable<string>> GetRoster()
        {
            var res = await client.RequestRosterAsync();
            var roster = res.Query as Roster;
            return roster.GetRoster().Select(r => r.Jid.User);
        }

        private void OnSessionStateChanged(SessionState sessionState)
        {
            Trace.WriteLine(sessionState);
        }

        private void OnXElementStreamChanged(XmppXElement element)
        {
            if (element is Message m)
            {
                MessageAvailable?.Invoke(this, new EventArgs<Message>(m));
            }

            Trace.WriteLine(element);
        }

        public async Task SendPrivateMessage(string message, string to)
        {


            //to = to + "@" + xmppDomain;
            
                await client.SendAsync(new Message(new Jid(to, xmppDomain, null), message));
            
            int i = 9;
            
        }
    }
}
