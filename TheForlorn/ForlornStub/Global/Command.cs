using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ForlornStub
{
    [Serializable]
    public class Command
    {
        public enum Type
        {
            Identify,
            Disconnect,
            // process
            Open,
            Kill,
            // file I/O
            Create,
            Move,
            Copy,
            Delete,
            List,
            // system I/O
            CursorPosition,
            MouseClick,
            MouseRightClick,
            MouseMiddleClick,
            MouseScroll,
            SendText,
            // misc
            Screenshot,
            Error
        }

        public Command() { }

        public Command(Type t)
        {
            this.CommandType = t;
        }

        public Command(Type t, params string[] args)
        {
            this.CommandType = t;
            Arguments = args;
        }

        public Type CommandType;

        public string[] Arguments;

        public static Command Deserialize(string text)
        {
            Command deserialCommand;
            XmlSerializer xs = new XmlSerializer(typeof(Command));

            using (TextReader tr = new StringReader(text))
            {
                deserialCommand = (Command)xs.Deserialize(tr);
            }

            return deserialCommand;
        }
    }

    [System.AttributeUsage(AttributeTargets.Method)]
    public class HandlesCommand : System.Attribute
    {
        public Command.Type TypeHandled;

        public HandlesCommand(Command.Type typeToHandle)
        {
            this.TypeHandled = typeToHandle;
        }
    }
}
