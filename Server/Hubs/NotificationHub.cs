using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Runtime.Remoting.Contexts;
using Server.Utils;


namespace Server.Hubs
{
    public class NotificationHub : Hub
    {

        public static ConcurrentDictionary<string, string> mapUidToConnection = new ConcurrentDictionary<string, string>();


        public void Register(string ID)
        {
            try
            {
                Trace.AutoFlush = true;
                string deadConnectionId;
                string ConnID = Context.ConnectionId;
                mapUidToConnection.TryRemove(ID, out deadConnectionId);
                mapUidToConnection[ID] = ConnID;
                Trace.TraceInformation(String.Format("Added user: {0} connectionId {1}", ID, mapUidToConnection[ID]));
                Trace.Flush();
            }
            catch (Exception e)
            {
                Trace.TraceError("Registration of " + ID + " failed: " + e.Message);
                Trace.Flush();
            }

        }

        public void sendNotificationToCareGivers(string ID, string patientName, AlgoUtils.Status type)
        {
            if (mapUidToConnection.ContainsKey(ID))
            {
                if (type.Equals(AlgoUtils.Status.Wandering))
                {
                    Trace.TraceInformation(String.Format("Sending Wandering Alert to user: {0}, for patient {1}. ConnectionId {2}", ID, patientName, mapUidToConnection[ID]));
                    try
                    {
                        Clients.Client(mapUidToConnection[ID]).receiveWanderingAlert(patientName);
                    }
                    catch (Exception e)
                    {
                        string deadConnectionId;
                        mapUidToConnection.TryRemove(ID, out deadConnectionId);
                    }
                }
                else if (type.Equals(AlgoUtils.Status.Distress))
                {
                    Trace.TraceInformation(String.Format("Sending Distress Alert to user: {0}, for patient {1}. ConnectionId {2}", ID, patientName, mapUidToConnection[ID]));
                    try
                    {
                        Clients.Client(mapUidToConnection[ID]).receiveDistressAlert(patientName);
                    }
                    catch (Exception e)
                    {
                        string deadConnectionId;
                        mapUidToConnection.TryRemove(ID, out deadConnectionId);
                    }
                }
                else if (type.Equals(AlgoUtils.Status.Risk))
                {
                    Trace.TraceInformation(String.Format("Sending Risk Alert to user: {0}, for patient {1}. ConnectionId {2}", ID, patientName, mapUidToConnection[ID]));
                    try
                    {
                        Clients.Client(mapUidToConnection[ID]).receiveRiskAlert(patientName);
                    }
                    catch (Exception e)
                    {
                        string deadConnectionId;
                        mapUidToConnection.TryRemove(ID, out deadConnectionId);
                    }
                }

            }
            else
            {
                Trace.TraceError(String.Format("User {0} doesn't exist", ID));
            }
        }


        public void sendSMSToCareGivers(string ID, string patientName, AlgoUtils.Status type)
        {
            if (mapUidToConnection.ContainsKey(ID))
            {
                if (type.Equals(AlgoUtils.Status.Wandering))
                {
                    Trace.TraceInformation(String.Format("Sending SMS-Status-Message to user: {0}, for patient {1}. ConnectionId {2}", ID, patientName, mapUidToConnection[ID]));
                    try
                    {
                        Clients.Client(mapUidToConnection[ID]).receiveWanderingSMS(patientName);
                    }
                    catch (Exception e)
                    {
                        string deadConnectionId;
                        mapUidToConnection.TryRemove(ID, out deadConnectionId);
                    }
                }
                else if (type.Equals(AlgoUtils.Status.Distress))
                {
                    Trace.TraceInformation(String.Format("Sending SMS-Distress-Message to user: {0}, for patient {1}. ConnectionId {2}", ID, patientName, mapUidToConnection[ID]));
                    try
                    {
                        Clients.Client(mapUidToConnection[ID]).receiveDistressSMS(patientName);
                    }
                    catch (Exception e)
                    {
                        string deadConnectionId;
                        mapUidToConnection.TryRemove(ID, out deadConnectionId);
                    }
                }
                else if (type.Equals(AlgoUtils.Status.Risk))
                {
                    Trace.TraceInformation(String.Format("Sending SMS-Risk-Message to user: {0}, for patient {1}. ConnectionId {2}", ID, patientName, mapUidToConnection[ID]));
                    try
                    {
                        Clients.Client(mapUidToConnection[ID]).receiveRiskSMS(patientName);
                    }
                    catch (Exception e)
                    {
                        string deadConnectionId;
                        mapUidToConnection.TryRemove(ID, out deadConnectionId);
                    }
                }

            }
            else
            {
                Trace.TraceError(String.Format("User {0} doesn't exist", ID));
            }
        }



        public void sendLostConnNotificationToCareGivers(string ID, string patientName)
        {
            if (mapUidToConnection.ContainsKey(ID))
            {
                Trace.TraceInformation(String.Format("Sending Lost Connection Alert to user: {0}, for patient {1}. ConnectionId {2}", ID, patientName, mapUidToConnection[ID]));
                try
                {
                    Clients.Client(mapUidToConnection[ID]).receiveConnectionLostAlert(patientName);
                }
                catch (Exception e)
                {
                    string deadConnectionId;
                    mapUidToConnection.TryRemove(ID, out deadConnectionId);
                }
            }
            else
            {
                Trace.TraceError(String.Format("User {0} doesn't exist", ID));
            }
        }



        //Testing
        public void startWanderingDetection(string ID)
        {
            Trace.TraceInformation("Starting Detection Algo for Patient {0}", ID);
            WanderingAlgo algo = new WanderingAlgo();
            algo.wanderingDetectionAlgo(ID);
        }





        //Other
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

    }






    //public void Send(string name, string message)
    //{
    //    // Call the broadcastMessage method to update clients.
    //    Clients.All.broadcastMessage(name, message);
    //}
}
