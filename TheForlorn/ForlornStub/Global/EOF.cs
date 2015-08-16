using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForlornStub
{
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;

    public static class EOF
    {
        public static void SetSettings(string filename, StubSettings settings)
        {
            using(FileStream fs = File.OpenWrite(filename))
            {
                XmlSerializer xs = new XmlSerializer(typeof(StubSettings));
                fs.Position = fs.Length;
                xs.Serialize(fs, settings);
            }
        }

        private static string[] ReadAndSplitFile(string filename)
        {
            string fileAsText = File.ReadAllText(filename);
            return fileAsText.Split(new string[] { Global.Split }, StringSplitOptions.None);
        }

        public static bool HasSettings(string filename)
        {
            if(!File.Exists(filename)) { return false; }

            string[] splitFile = ReadAndSplitFile(filename);

            return splitFile.Length > 1;
        }

        public static bool HasSettings(string[] splitFile)
        {
            return splitFile.Length > 1;
        }

        public static StubSettings GetSettings(string filename)
        {
            //string thisFileName = (filename == "-1") ? System.Reflection.Assembly.GetEntryAssembly().Location : filename;
            string thisFileName = filename;
            string[] splitFile = ReadAndSplitFile(thisFileName);

            // only attempt to retrieve settings if they exist
            return HasSettings(splitFile)
                       ? Utility.Deserialize<StubSettings>(splitFile[splitFile.Length - 1])
                       : StubSettings.Default;
        }

        public static StubSettings GetSettings()
        { // use current executable
            return GetSettings(System.Reflection.Assembly.GetEntryAssembly().Location);
        }
    }
}
