using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForlornStub
{
    using System.IO;
    using System.Xml.Serialization;

    [Serializable]
    public class StubSettings
    {
        [XmlElement("HostName")]
        public string HostName;

        [XmlElement("Port")]
        public int Port;

        [XmlElement("Identifier")]
        public string Identifier;

        [XmlElement("Startup")]
        public bool RunOnStartup;

        public static StubSettings Default
        {
            get
            {
                StubSettings s = new StubSettings() { HostName = "localhost", Identifier = "Default", Port = 8008, RunOnStartup = false };
#if DEBUG
                s.Identifier = "DEBUG";
#endif
                return s;
            }
        }
    }
}
