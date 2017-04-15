using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Runtime.Remoting.Contexts;


namespace Server.Hubs
{
    public class NotificationHub : Hub
    {

        public static ConcurrentDictionary<string, string> mapUidToConnection = new ConcurrentDictionary<string, string>();

        public void Register(string ID)
        {
            try
            {
                string deadConnectionId;
                string ConnID = Context.ConnectionId;
                mapUidToConnection.TryRemove(ID, out deadConnectionId);
                mapUidToConnection[ID] = ConnID;
                Trace.TraceInformation(String.Format("Added user: {0} connectionId {1}", ID, mapUidToConnection[ID]));
                Trace.Flush();
                Clients.Client(ConnID).wanderingAlert("This is a test");
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }

        }

        public string Send(string ID)
        {
            string test;
            try
            {
                string deadConnectionId;
                string ConnID = Context.ConnectionId;
                mapUidToConnection.TryRemove(ID, out deadConnectionId);
                mapUidToConnection[ID] = ConnID;
                test = "woked";
                Trace.TraceInformation(String.Format("Added user: {0} connectionId {1}", ID, mapUidToConnection[ID]));
                Trace.Flush();
            }
            catch (Exception e)
            {
                test = e.Message;
            }
       
            return test;
        }

        public void Send2(String message)
        {
            Clients.All.broadcastMessage(message);
        }

        public void SendWanderingAlert(string ID)
        {
            if (mapUidToConnection.ContainsKey(ID))
            {
                //Trace.TraceInformation(String.Format("Sending to user: {0} connectionId {1}", msg.DestUserId, mapUidToConnection[msg.DestUserId]));

                try
                {
                    Clients.Client(mapUidToConnection[ID]).wanderingAlert();
                }
                catch (Exception e)
                {
                    string deadConnectionId;
                    mapUidToConnection.TryRemove(ID, out deadConnectionId);
                }
            }
            else
            {
                //Trace.TraceInformation(String.Format("User {0} doesn't exist", msg.DestUserId));
            }
        }
    }

    //public void Send(string name, string message)
    //{
    //    // Call the broadcastMessage method to update clients.
    //    Clients.All.broadcastMessage(name, message);
    //}
}
