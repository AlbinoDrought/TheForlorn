using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheForlorn;
using ForlornStub;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ForlornTest
{
    [TestClass]
    public class ServerCommandTest
    {
        [TestMethod]
        public void TestIdentify()
        {
            string[] identTag = new string[] { "testing", "123" };

            Command c = new Command(Command.Type.Identify, identTag);
            SocketState ss = GetTestingSocketState();
            CommandState cs = CommandHandler.HandleCommand(c, ss);

            if(((string[])ss.Tag) != identTag)
            {
                throw new Exception("Identity tag not saved correctly");
            }
            if(!cs.UpdateForm)
            {
                throw new Exception("UpdateForm not set!");
            }
        }

        //[TestMethod]
        public void TestScreenshot()
        {
            Bitmap screenshot = Utility.ScreenToBitmap();
            string base64Screenshot;
            ImageFormat imgFormat = ImageFormat.Jpeg;
            byte[] imgBytes;

            using (MemoryStream ms = new MemoryStream())
            {
                EncoderParameters eps = new EncoderParameters(1);
                eps.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 0L);
                screenshot.Save(ms, Utility.GetEncoder(imgFormat), eps);
                base64Screenshot = Convert.ToBase64String(ms.ToArray());
                imgBytes = ms.ToArray();
            }

            SocketState ss = GetTestingSocketState();
            CommandState cs = CommandHandler.HandleCommand(new Command(Command.Type.Screenshot, base64Screenshot), ss);

            if(((byte[])cs.ReturnValue) != imgBytes)
            {
                throw new Exception("Returned wrong image bytes");
            }
        }

        private SocketState GetTestingSocketState()
        {
            return new SocketState();
        }
    }
}
