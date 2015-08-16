using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ForlornStub
{
    // https://msdn.microsoft.com/en-us/library/bew39x2a(v=vs.110).aspx
    // see ^
    // the following is a butchered version of that code with client and server support combined

    public class SocketState
    {
        public Socket Connection;
        public const int BufferSize = 1024;
        public byte[] Buffer = new Byte[BufferSize];
        public StringBuilder Contents = new StringBuilder();

        public object[] Tag;
    }

    public class SocketHelper
    {
        #region Server
        private bool Listen = false;
        private Socket Listener;

        public int ListeningPort;
        public ConcurrentDictionary<string, SocketState> Clients = new ConcurrentDictionary<string, SocketState>();


        public SocketHelper(int port)
        {
            ListeningPort = port;
            Listen = true;
            Listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
            Listener.Bind(new IPEndPoint(IPAddress.Any, port));
            Listener.Listen(100); // more than 100 in backlog? not sure

            RegisterAccept();
        }

        private void RegisterAccept()
        {
            Listener.BeginAccept(new AsyncCallback(this.OnBeginAccept), Listener);
        }

        public void OnBeginAccept(IAsyncResult ar)
        {
            if (!Listen) return;

            Socket socket, remoteSocket;
            try
            { // client can disconnect before we handle the connection (... really?)
                socket = (Socket)ar.AsyncState;
                remoteSocket = socket.EndAccept(ar);
            }
            catch (SocketException)
            {
                return;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                return;
            }

            string key = remoteSocket.RemoteEndPoint.ToString();
            SocketState ss = new SocketState()
            {
                Connection = remoteSocket
            };

            Clients[key] = ss;
            if (OnConnectCallback != null)
            {
                OnConnectCallback(ss);
            }

            RegisterReceive(ss);
            RegisterAccept();
        }

        public void Send(SocketState ss, string text)
        {
            if (!Listen) return;

            byte[] encodedText = GetEncodedString(text);
            try
            {
                ss.Connection.BeginSend(encodedText, 0, encodedText.Length, SocketFlags.None, null, null);
            }
            catch (SocketException ex)
            {
                HandleSocketException(ss, ex);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
        }

        public void Send(SocketState ss, Command c)
        {
            this.Send(ss, Utility.Serialize(c));
        }

        public void Stop()
        {
            Command disconnectCommand = new Command(Command.Type.Disconnect);
            foreach (KeyValuePair<string, SocketState> kvp in Clients)
            {
                this.Send(kvp.Value, disconnectCommand);
            }

            Clients = new ConcurrentDictionary<string, SocketState>();
            Listen = false;
            Listener.Close(150);
        }
        #endregion

        #region Client
        // client code is kind of messy and suppresses errors because it was built to relentlessly continue connecting in the background
        private int FailedAttempts = 0;
        private Socket Client;
        private StubSettings Settings;
        private bool Persistent;

        public SocketHelper(StubSettings settings, bool persistent = true)
        {
            this.Settings = settings;
            this.Persistent = persistent;
            Connect();
        }

        private void Connect(IPEndPoint ip)
        {
            try
            {
                if (Client != null && Client.Connected)
                {
                    Client.Close();
                }

                Client = new Socket(SocketType.Stream, ProtocolType.Tcp);
                Client.BeginConnect(ip, new AsyncCallback(OnBeginConnect), Client);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
        }

        private void Connect()
        {
            Connect(GetIPEndPoint());
        }

        public bool Connected
        {
            get
            {
                if (Listen || Client == null) return false;
                return Client.Connected;
            }
        }

        private IPEndPoint GetIPEndPoint()
        {
            IPHostEntry ipHostInfo;
            try
            {
                ipHostInfo = Dns.GetHostEntry(Settings.HostName);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                ipHostInfo = Dns.GetHostEntry("localhost");
            }
            IPAddress ipAddress = (ipHostInfo.AddressList[0].ToString() == "::1") ? ipHostInfo.AddressList[1] : ipHostInfo.AddressList[0];
            return new IPEndPoint(ipAddress, Settings.Port);
        }

        private void OnBeginConnect(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;

            try
            {
                client.EndConnect(ar);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                if (Persistent)
                {
                    System.Threading.Thread.Sleep((5000 * (FailedAttempts++)));
                    FailedAttempts = Math.Min(FailedAttempts, 60);
                    Connect();
                }
                return;
            }

            SocketState ss = new SocketState()
            {
                Connection = client
            };

            if(OnConnectCallback != null)
            {
                OnConnectCallback(ss);
            }

            RegisterReceive(ss);
        }

        public void Send(string text)
        {
            byte[] data = GetEncodedString(text);
            Client.BeginSend(data, 0, data.Length, SocketFlags.None, null, null);
        }

        public void Send(Command c)
        {
            this.Send(Utility.Serialize(c));
        }
        #endregion

        #region Generic
        public IExceptionHandler ExceptionHandler = new QuietExceptionHandler();

        public delegate bool OnReceiveCommandDelegate(SocketState ss, Command c);
        public delegate void OnConnectDelegate(SocketState ss);
        public delegate void OnDisconnectDelegate(SocketState ss);

        public List<OnReceiveCommandDelegate> OnReceiveCommandCallback = new List<OnReceiveCommandDelegate>();
        public OnConnectDelegate OnConnectCallback;
        public OnDisconnectDelegate OnDisconnectCallback;

        private void RegisterReceive(SocketState ss)
        {
            ss.Connection.BeginReceive(ss.Buffer, 0, ss.Buffer.Length, SocketFlags.None, new AsyncCallback(OnBeginReceive), ss);
        }

        private void OnBeginReceive(IAsyncResult ar)
        {
            SocketState ss = (SocketState)ar.AsyncState;
            Socket client = ss.Connection;

            try
            {
                int bytesRead = client.EndReceive(ar);
                if (bytesRead > 0)
                {
                    ss.Contents.Append(Encoding.ASCII.GetString(ss.Buffer, 0, bytesRead));
                    string contents = ss.Contents.ToString();
                    while (contents.IndexOf("<EOF>") > -1)
                    {
                        int index = contents.IndexOf("<EOF>");
                        string commandSubstring = contents.Substring(0, index);

                        if (commandSubstring.Contains("xml"))
                        {
                            Command c = Utility.Deserialize<Command>(commandSubstring);
                            HandleReceiveCommand(ss, c);
                        }
                        ss.Contents = ss.Contents.Replace(commandSubstring + "<EOF>", "");
                        contents = ss.Contents.ToString();
                    }
                }
                else
                {
                    ss.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
                ss.Contents.Clear();
            }

            if (ss.Connection.Connected)
            {
                RegisterReceive(ss);
            }
            else
            {
                Connect();
            }
        }

        /// <summary>
        /// Calls all command handlers (LIFO) and stops when one handles command (returns true)
        /// </summary>
        public void HandleReceiveCommand(SocketState ss, Command c)
        {
            for (int i = OnReceiveCommandCallback.Count - 1; i >= 0; i--)
            {
                if (OnReceiveCommandCallback[i] == null) continue;

                bool handled = OnReceiveCommandCallback[i](ss, c);

                if (handled) break;
            }
        }

        public byte[] GetEncodedString(string text)
        {
            return Encoding.ASCII.GetBytes(text + "<EOF>");
        }

        private void HandleSocketException(SocketState ss, SocketException ex)
        {
            // If the connection is reset, treat as if client disconnected (Remove them from client list)
            if (ex.SocketErrorCode == SocketError.ConnectionReset && Clients != null)
            {
                Clients.TryRemove(ss.Connection.RemoteEndPoint.ToString(), out ss);
                if (OnDisconnectCallback != null)
                {
                    OnDisconnectCallback(ss);
                }
            }
            else
            {
                ExceptionHandler.HandleException(ex);
            }
        }
        #endregion
    }
}
