using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.Utils;

namespace Server.Hubs
{
    public static class ConnectionDictionary
    {
        public static ConcurrentDictionary<string, string> mapUidToConnection =
            new ConcurrentDictionary<string, string>();
        public static ConcurrentDictionary<string, AlgoUtils.Status> mapUidToStatus = new ConcurrentDictionary<string, AlgoUtils.Status>();
    }
}