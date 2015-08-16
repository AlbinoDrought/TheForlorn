using ForlornStub;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting.Channels;

namespace TheForlorn
{
    

    public partial class FileBrowser : Form
    {
        SocketState cs;
        SocketHelper sh;

        Label upLabel;

        Label sourceControl;

        string currentDirectory = "C:\\";

        string copiedFile = "";
        Label copiedLabel;
        Color originalColor;

        public FileBrowser(SocketState cs, SocketHelper sh)
        {
            this.cs = cs;
            this.sh = sh;
            sh.OnReceiveCommandCallback.Add(OnReceiveCommand);

            upLabel = new Label()
            {
                Text = "..",
                ForeColor = Color.DarkRed,
                Tag = false,
            };
            upLabel.Click += upLabel_Click;

            InitializeComponent();
        }

        private void FileBrowser_Load(object sender, EventArgs e)
        {
            UpdateDirectory();
        }

        private void UpdateDirectory()
        {
            Command c = new Command(Command.Type.List, currentDirectory);
            sh.Send(cs, c);

            flowLayout.Invoke((MethodInvoker)delegate
            {
                this.Text = currentDirectory;
            });
        }

        public bool OnReceiveCommand(SocketState cs, Command c)
        {
            if (c.CommandType != Command.Type.List || cs.Connection.RemoteEndPoint.ToString() != this.cs.Connection.RemoteEndPoint.ToString()) return false;

            bool isFiles = false;

            List<Control> controlsToAdd = new List<Control>();
            controlsToAdd.Add(upLabel);

            foreach (string item in c.Arguments)
            {
                if (item == "--FILES--")
                {
                    isFiles = true;
                    continue;
                }

                Label fileLabel = new Label();
                fileLabel.Tag = isFiles;
                fileLabel.Text = item.Replace(currentDirectory, "");
                fileLabel.ForeColor = isFiles ? Color.Black : Color.DarkBlue;
                fileLabel.BorderStyle = BorderStyle.FixedSingle;
                fileLabel.MouseClick += fileLabel_Click;
                fileLabel.DoubleClick += fileLabel_DoubleClick;
                if(isFiles) fileLabel.ContextMenuStrip = cmsFile;

                if (currentDirectory + fileLabel.Text == copiedFile) fileLabel.BackColor = Color.LightBlue;

                controlsToAdd.Add(fileLabel);
            }

            flowLayout.Invoke((MethodInvoker)delegate
            {
                flowLayout.Controls.Clear();
                flowLayout.Controls.AddRange(controlsToAdd.ToArray());
            });

            return true;
        }

        void fileLabel_DoubleClick(object sender, EventArgs e)
        {
            Label lblSender = (Label)sender;
            string fullName = currentDirectory + lblSender.Text;

            sh.Send(cs, new Command(Command.Type.Open, fullName));
        }

        void fileLabel_Click(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

            Label lbl = (Label)sender;
            if((bool)lbl.Tag)
            { // file

            }
            else
            { // folder
                currentDirectory += lbl.Text + "\\";
                UpdateDirectory();
            }
        }

        private bool IsDirectory(Label lbl)
        {
            return lbl.BackColor == Color.DarkBlue;
        }

        void upLabel_Click(object sender, EventArgs e)
        {
            //string folderName = System.IO.Path.Get(currentDirectory);
            currentDirectory = Path.GetDirectoryName(Path.GetDirectoryName(currentDirectory));
            if(currentDirectory == null)
            {
                currentDirectory = "C:\\";
            }
            if(!currentDirectory.EndsWith("\\"))
            {
                currentDirectory += "\\";
            }
            UpdateDirectory();
        }

        private void FileBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            sh.OnReceiveCommandCallback.Remove(OnReceiveCommand);
        }

        private string GetFullPathFilename(object sender)
        {
            Label lblSender;

            if (sender.GetType() == typeof(ContextMenuStrip))
            {
                lblSender = (Label)((System.Windows.Forms.ContextMenuStrip)sender).SourceControl;
            }
            else
            {
                lblSender = (Label)sender;
            }

            return currentDirectory + lblSender.Text;
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sh.Send(cs, new Command(Command.Type.Open, this.GetFullPathFilename(sourceControl)));
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copiedFile = this.GetFullPathFilename(sourceControl);

            if (copiedLabel != null) copiedLabel.BackColor = originalColor;

            originalColor = sourceControl.BackColor;
            sourceControl.BackColor = Color.LightBlue;
            copiedLabel = sourceControl;
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (copiedFile.Length <= 0) return;

            sh.Send(cs, new Command(Command.Type.Copy, copiedFile, this.GetFullPathFilename(sourceControl)));
            this.UpdateDirectory();
        }

        private void moveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newFileName = Prompt.ShowDialog("Enter new full path:", "Move");

            if (newFileName.Trim().Length <= 0) return;

            sh.Send(cs, new Command(Command.Type.Move, this.GetFullPathFilename(sourceControl), newFileName));
            this.UpdateDirectory();
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newFileName = Prompt.ShowDialog("Enter new name:", "Rename");

            if (newFileName.Trim().Length <= 0) return;

            sh.Send(cs, new Command(Command.Type.Move, this.GetFullPathFilename(sourceControl), currentDirectory + newFileName));
            this.UpdateDirectory();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sh.Send(cs, new Command(Command.Type.Delete, this.GetFullPathFilename(sourceControl)));
            this.UpdateDirectory();
        }

        private void cmsFile_Opening(object sender, CancelEventArgs e)
        {
            sourceControl = (Label)((ContextMenuStrip)sender).SourceControl;
        }

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void uploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
