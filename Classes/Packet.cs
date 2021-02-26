using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Classes
{

    [Serializable]
    public struct Packet
    {
        public string senderName;
        public string senderMessage;
        public ConsoleColor senderColor;
    }
}
