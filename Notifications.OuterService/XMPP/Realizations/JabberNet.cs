using jabber.client;
using jabber.protocol;
using jabber.protocol.client;
using NLog;
using Notifications.OuterService.XMPP.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Notifications.OuterService.XMPP.Realizations
{
    public  class JabberNet : IXMPPClient, IDisposable
    {
        private  static JabberClient jabberClient;
       
        private Interfaces.ILogger Logger;

        public JabberNet(Interfaces.ILogger logger)
        {
            Logger = logger;
        }


        public void SendNotification(IEnumerable<string> sendto, string text)
        {

            //Message msg = new Message(jabberClient.Document);
            foreach (string recipient in sendto)
            {
                jabberClient.Message(recipient, text);
                //msg.To = recipient;
                //msg.Subject = txtSubject.Text;
                //msg.Body = text;
                //jabberClient.Write(msg);
            }


        }

        public void Close()
        {
            if (IsConnected())
            {
                jabberClient.Close();
            }
        }

        public bool IsConnected()
        {
            if (jabberClient != null)
              return jabberClient.IsAuthenticated;

            return false;
        }

        public void Connect(string server, int port, string user, string password, float autoreconnect)
        {

            JabberClient jc = new JabberClient();
            jc.User = user;
            jc.Server = server;
            jc.Port = port;
            jc.Password = password;

            // don't do extra stuff, please.
            
            jc.AutoPresence = false;
            jc.AutoRoster = false;
            jc.AutoReconnect = autoreconnect;
            
            jc.OnInvalidCertificate += new System.Net.Security.RemoteCertificateValidationCallback(OnInvalidCertificate);

            jc.OnDisconnect += new bedrock.ObjectHandler(OnDisconnect);

            jc.OnConnect += OnConnect;

            // listen for errors.  Always do this!
            jc.OnError += new bedrock.ExceptionHandler(OnError);

            // what to do when login completes
            jc.OnAuthenticate += new bedrock.ObjectHandler(OnAuthenticate);
            //jc.OnAuthError += new ProtocolHandler(OnAuthError);

            //jc.OnConnect += new jabber.connection.StanzaStreamHandler(OnConnect);
            

            jc.OnReadText += new bedrock.TextHandler(OnReadText);
            jc.OnWriteText += new bedrock.TextHandler(OnWriteText);
            
            jc.Connect();

            
            jabberClient = jc;

            //done.WaitOne();

        }

        private void OnConnect(object sender, jabber.connection.StanzaStream stream)
        {
            JabberClient j = (JabberClient)sender;
            Logger.Info(String.Format("Connecting (Server:{0}, Port:{1}, AutoReconnect:{2})", j.Server,j.Port,j.AutoReconnect));
        }

        private void OnDisconnect(object sender)
        {
            Logger.Info("Disconnect");
            
        }

        private void OnAuthError(object sender, XmlElement rp)
        {
            Logger.Error("Authentication error: " + rp);
        }

        static bool OnInvalidCertificate(object sender,
                             System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                             System.Security.Cryptography.X509Certificates.X509Chain chain,
                             System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
       
                return true;
         
        }

         void OnWriteText(object sender, string txt)
        {
            if (txt == " ") return;  // ignore keep-alive spaces
            Logger.Info("Send: " + txt);
        }

        void OnReadText(object sender, string txt)
        {
            if (txt != "<" )
            {
                Logger.Info("Recv: " + txt);
            }
            else
            {
                return;  // ignore keep-alive spaces
            }
                
             
        }

        void OnAuthenticate(object sender)
        {
            // Sender is always the JabberClient.
            JabberClient j = (JabberClient)sender;
            Logger.Info(String.Format("Authentication (User:{0})",j.User));
            //done.Set();
            j.Presence(PresenceType.available, "Рассылка", "Online", 0);
        }


        void OnError(object sender, Exception ex)
        {
            // There was an error!
            Logger.Error("Error: " + ex.ToString());
            //done.Set();
            
        }

        public string GetUserName(string name, string domain)
        {
            return name + "@" + domain;
        }

        public IEnumerable<string> ReadLog(DateTime begin, DateTime end)
        {
            return Logger.ReadLog(begin,end);
        }


        public void Dispose()
        {
            if (jabberClient != null)
            {
                jabberClient.Close();
                jabberClient.Dispose();
                
            }
           
        }
    }
}
