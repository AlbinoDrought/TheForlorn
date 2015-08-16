using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ForlornStub;
using System.IO;

namespace TheForlorn
{
    public partial class WatchForm : Form
    {
        SocketState cs;
        SocketHelper sh;
        private bool requesting = false;
        //SocketHelper.OnReceiveDataDelegate original;

        public WatchForm(SocketHelper sh, SocketState cs)
        {
            this.sh = sh;
            this.cs = cs;
            //this.original = sh.OnReceiveDataCallback;

            InitializeComponent();
        }

        private void WatchForm_Load(object sender, EventArgs e)
        {
            sh.OnReceiveCommandCallback.Add(OnReceiveCommand);
            Controls.SetChildIndex(pbScreenshot, 0);
            //sh.OnReceiveDataCallback = OnReceiveData;
        }

        private bool OnReceiveCommand(SocketState cs, Command c)
        {
            if(!requesting || this.IsDisposed || c.CommandType != Command.Type.Screenshot || cs.Connection.RemoteEndPoint.ToString() != this.cs.Connection.RemoteEndPoint.ToString())
            {
                return false;
            }

            CommandState cmdState = CommandHandler.HandleCommand(c, cs);
            try
            {
                pbScreenshot.Invoke((MethodInvoker)delegate
                {
                    tmrScreenshot.Enabled = requesting;
                    tmrScreenshotTimeout.Enabled = false;
                    pbScreenshot.Image = byteArrayToImage((byte[])cmdState.ReturnValue);
                }, null);
            }
            catch (Exception)
            {
                //sh.OnReceiveDataCallback = original;
                return false;
            }

            return true;
        }

        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        private void btnScreenshot_Click(object sender, EventArgs e)
        {
            requesting = !requesting;
            tmrScreenshot.Enabled = requesting;
            

            //sh.OnReceiveDataCallback = requesting ? OnReceiveData : original;
        }

        private void tmrScreenshot_Tick(object sender, EventArgs e)
        {
            if (!requesting) return;
            tmrScreenshot.Enabled = false;
            tmrScreenshotTimeout.Interval = tmrScreenshot.Interval * 10;
            tmrScreenshotTimeout.Enabled = true;
            Command c = new Command(Command.Type.Screenshot, cbFormat.SelectedItem == null ? "jpg" : cbFormat.SelectedItem.ToString(), nudQuality.Value.ToString());
            sh.Send(cs, c);
            
        }

        private void tbFrequency_Scroll(object sender, EventArgs e)
        {
            tmrScreenshot.Interval = tbFrequency.Value;
        }

        private void WatchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //sh.OnReceiveDataCallback = original;
            requesting = false;
            sh.OnReceiveCommandCallback.Remove(OnReceiveCommand);
        }

        private void pbScreenshot_MouseMove(object sender, MouseEventArgs e)
        {
            if (pbScreenshot.Image == null || !requesting || !chkInput.Checked) return;

            Point relativePoint = Utility.ControlToScreen(e.X, e.Y, pbScreenshot.Image.Width, pbScreenshot.Image.Height, pbScreenshot.Width, pbScreenshot.Height);
            Command c = new Command(Command.Type.CursorPosition, (relativePoint.X).ToString(), (relativePoint.Y).ToString());

            sh.Send(cs, c);
        }

        private void pbScreenshot_MouseClick(object sender, MouseEventArgs e)
        {
            if (pbScreenshot.Image == null || !requesting || !chkInput.Checked) return;

            Command c = new Command(
                    e.Button == System.Windows.Forms.MouseButtons.Left ? Command.Type.MouseClick :
                    e.Button == System.Windows.Forms.MouseButtons.Right ? Command.Type.MouseRightClick :
                    e.Button == System.Windows.Forms.MouseButtons.Middle ? Command.Type.MouseMiddleClick :
                    Command.Type.MouseClick
                );

            sh.Send(cs, c);
        }

        private void tmrScreenshotTimeout_Tick(object sender, EventArgs e)
        {
            if (!requesting || tmrScreenshot.Enabled) return;
            tmrScreenshotTimeout.Enabled = false;
            tmrScreenshot.Enabled = true;
        }

        private void txtKeyboard_TextChanged(object sender, EventArgs e)
        {
            if (pbScreenshot.Image == null || !requesting || !chkInput.Checked) return;

            string textToSend = txtKeyboard.Text;
            txtKeyboard.Text = "";

            Command c = new Command(Command.Type.SendText, textToSend);

            sh.Send(cs, c);
        }

        private void WatchForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            requesting = false;
            sh.OnReceiveCommandCallback.Remove(OnReceiveCommand);
        }
    }
}
