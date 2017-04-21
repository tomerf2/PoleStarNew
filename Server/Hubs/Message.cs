using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.Utils;

namespace Server.Hubs
{
    public class Message
    {
        public string name;
        public string ID;
        public AlgoUtils.Status status;
    }

    public class SMSMessage
    {
        public string number;
        public AlgoUtils.Status status;
    }
}