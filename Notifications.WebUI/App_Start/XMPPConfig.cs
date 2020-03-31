using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace Notifications.WebUI.App_Start
{
    [Serializable]
    public class XMPPSettings
    {
        [XmlAttribute]
        public bool Enable { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public string Domain { get; set; }
        public float AutoReconnect { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string MessageTemplate { get; set; }
    }
    

    public static class XMPPConfig
    {
        public static void SaveSettings(XMPPSettings settings, string filename)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(XMPPSettings));
            TextWriter writer = new StreamWriter(filename, false, Encoding.GetEncoding(1251));
            formatter.Serialize(writer, settings);
            writer.Close();

        }

        public static XMPPSettings ReadSettings(string filename)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(XMPPSettings));

            using (StreamReader fs = new StreamReader(filename, Encoding.GetEncoding(1251), false))
            {
                XMPPSettings settings = (XMPPSettings)formatter.Deserialize(fs);
                fs.Close();
                return settings;
            }
        }

    }
}