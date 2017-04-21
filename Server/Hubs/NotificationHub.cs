using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Runtime.Remoting.Contexts;
using Server.DataObjects;
using Server.Utils;
using Server.Controllers;


namespace Server.Hubs
{
    public class NotificationHub : Hub
    {

        public static ConcurrentDictionary<string, string> mapUidToConnection = new ConcurrentDictionary<string, string>();
        public static ConcurrentDictionary<string, AlgoUtils.Status> mapUidToStatus = new ConcurrentDictionary<string, AlgoUtils.Status>();


        public void Register(string ID)
        {
            Trace.TraceInformation(String.Format("Attempting to register user {0}", ID));
            try
            {
                Trace.AutoFlush = true;
                string deadConnectionId;
                string ConnID = Context.ConnectionId;
                mapUidToConnection.TryRemove(ID, out deadConnectionId);
                mapUidToConnection[ID] = ConnID;
                Trace.TraceInformation(String.Format("Added user: {0} connectionId {1}", ID, mapUidToConnection[ID]));
            }
            catch (Exception e)
            {
                Trace.TraceError("Registration of " + ID + " failed: " + e.Message);
                Trace.Flush();
            }

        }

        public void setPatientStatus(string patientID, AlgoUtils.Status status)
        {
            mapUidToStatus[patientID] = status;

            CaregiverController caregiverController = new CaregiverController();
            IEnumerable<Caregiver> caregiversArr = caregiverController.GetCaregiversforPatientID(patientID);
            Trace.TraceInformation(String.Format("Sending Patient {0} status: {1} to all caregivers", patientID,
                status.ToString()));
            foreach (var caregiver in caregiversArr)
            {
                try
                {
                    mapUidToStatus[caregiver.Id] = status;
                    Clients.Client(mapUidToConnection[caregiver.Id]).receivePatientStatus(status);
                }
                catch (Exception e)
                {
                    string deadConnectionId;
                    mapUidToConnection.TryRemove(caregiver.Id, out deadConnectionId);
                }
            }
        }

        public void getPatientStatus(string caregiverID)
        {
            Clients.Client(mapUidToConnection[caregiverID]).receivePatientStatus(mapUidToStatus[caregiverID]);
        }

        public void sendHelpButtonNotificationToCareGivers(string patientID)
        {

            PatientController patientController = new PatientController();
            string patientName = patientController.GetPatientName(patientID);

            Trace.TraceInformation(String.Format("Patient {0} pressed Help Button sending alerts to all caregivers",patientName));

            CaregiverController caregiverController = new CaregiverController();
            IEnumerable<Caregiver> caregiversArr = caregiverController.GetCaregiversforPatientID(patientID);
            foreach (var caregiver in caregiversArr)
            {
                try
                {
                    Clients.Client(mapUidToConnection[caregiver.Id]).receiveHelpButtonAlert(patientName);
                    Clients.Client(mapUidToConnection[patientID]).receiveHelpButtonSMS(caregiver.Phone);
                    Trace.TraceInformation(String.Format("Sending Help Button Alert to user: {0}, for patient {1}. ConnectionId {2}", caregiver.Id, patientName, mapUidToConnection[caregiver.Id]));

                }
                catch (Exception e)
                {
                    string deadConnectionId;
                    mapUidToConnection.TryRemove(caregiver.Id, out deadConnectionId);
                }
            }

            //FOR TESTING TODO: REMOVE !!!!
            Clients.Client(mapUidToConnection[patientID]).receiveHelpButtonAlert(patientName);
            Trace.TraceInformation(String.Format("Sending Help Button Alert to to patient [FOR TESTING] for patient {0}. ConnectionId {1}", patientName, mapUidToConnection[patientID]));
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



        public void sendSMSToCareGivers(string ID, string phoneNumber, AlgoUtils.Status type)
        {
            if (mapUidToConnection.ContainsKey(ID))
            {
                if (type.Equals(AlgoUtils.Status.Wandering))
                {
                    Trace.TraceInformation(String.Format("Sending SMS-Wandering-Message to user: {0}, to number {1}. ConnectionId {2}", ID, phoneNumber, mapUidToConnection[ID]));
                    try
                    {
                        Clients.Client(mapUidToConnection[ID]).receiveWanderingSMS(phoneNumber);
                    }
                    catch (Exception e)
                    {
                        string deadConnectionId;
                        mapUidToConnection.TryRemove(ID, out deadConnectionId);
                    }
                }
                else if (type.Equals(AlgoUtils.Status.Distress))
                {
                    Trace.TraceInformation(String.Format("Sending SMS-Distress-Message to user: {0}, to number {1}. ConnectionId {2}", ID, phoneNumber, mapUidToConnection[ID]));
                    try
                    {
                        Clients.Client(mapUidToConnection[ID]).receiveDistressSMS(phoneNumber);
                    }
                    catch (Exception e)
                    {
                        string deadConnectionId;
                        mapUidToConnection.TryRemove(ID, out deadConnectionId);
                    }
                }
                else if (type.Equals(AlgoUtils.Status.Risk))
                {
                    Trace.TraceInformation(String.Format("Sending SMS-Risk-Message to user: {0}, to number {1}. ConnectionId {2}", ID, phoneNumber, mapUidToConnection[ID]));
                    try
                    {
                        Clients.Client(mapUidToConnection[ID]).receiveRiskSMS(phoneNumber);
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

    }

}
