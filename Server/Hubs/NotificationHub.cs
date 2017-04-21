using System;
using System.Collections;
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


        public void printConnections()
        {
            Trace.TraceInformation(String.Format("{0} entries in dictionary",
                ConnectionDictionary.mapUidToConnection.Count()));

            foreach (var w in ConnectionDictionary.mapUidToConnection)
            {
                Trace.TraceInformation(String.Format("Key: {0}, Val: {1}", w.Key, w.Value));
            }
        }


        public void Register(string ID)
        {
            Trace.TraceInformation(String.Format("Attempting to register user {0}", ID));
            try
            {
                Trace.AutoFlush = true;
                string deadConnectionId;
                string ConnID = Context.ConnectionId;
                ConnectionDictionary.mapUidToConnection.TryRemove(ID, out deadConnectionId);
                ConnectionDictionary.mapUidToConnection[ID] = ConnID;
                Trace.TraceInformation(String.Format("Added user: {0} connectionId {1}", ID,
                    ConnectionDictionary.mapUidToConnection[ID]));
            }
            catch (Exception e)
            {
                Trace.TraceError("Registration of " + ID + " failed: " + e.Message);
            }

        }



        public void sendNewNotifications(string patientID, string patientName, IEnumerable<Caregiver> caregiversArr,
            AlgoUtils.Status status)
        {
            Trace.TraceInformation(String.Format("Sending Patient {0} status: {1} to all caregivers", patientID,
                status.ToString()));
            ConnectionDictionary.mapUidToStatus[patientID] = status; //update patient status

            Hubs.Message message = new Message();
            message.status = status;
            message.ID = patientID;
            message.name = patientName;

            foreach (var caregiver in caregiversArr)
            {

                try
                {
                    ConnectionDictionary.mapUidToStatus[caregiver.Id] = status;
                    Clients.Client(ConnectionDictionary.mapUidToConnection[caregiver.Id]).receiveNotification(message);
                    Trace.TraceInformation(String.Format("Sent message to caregiver {0}", caregiver.Id));
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.Message);
                }
            }
            Trace.TraceInformation(String.Format("Sending message to patient for testing"));
            try
            {
                Clients.Client(ConnectionDictionary.mapUidToConnection[patientID]).receiveNotification(message);
                Trace.TraceInformation(String.Format("Sent message back to patient {0}", patientName));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }

        }

        public void getPatientStatus(string caregiverID)
        {
            Clients.Client(ConnectionDictionary.mapUidToConnection[caregiverID])
                .receivePatientStatus(ConnectionDictionary.mapUidToStatus[caregiverID]);
        }


        public void sendHelpButtonNotificationToCareGivers(string patientID)
        {

            PatientController patientController = new PatientController();
            string patientName = patientController.GetPatientName(patientID);

            Trace.TraceInformation(String.Format("Patient {0} pressed Help Button sending alerts to all caregivers",
                patientName));

            CaregiverController caregiverController = new CaregiverController();
            IEnumerable<Caregiver> caregiversArr = caregiverController.GetCaregiversforPatientID(patientID);

            sendNewNotifications(patientID, patientName, caregiversArr, AlgoUtils.Status.NeedsAssistance);
        }


        public void sendSMSToCareGivers(string ID, string phoneNumber, AlgoUtils.Status type)
        {
            SMSMessage message = new SMSMessage();
            message.number = phoneNumber;
            message.status = type;


            if (ConnectionDictionary.mapUidToConnection.ContainsKey(ID))
            {

                Trace.TraceInformation(String.Format("Sending SMS {0} message to user: {1}. ConnectionId {2}",
                    type.ToString(), phoneNumber, ConnectionDictionary.mapUidToConnection[ID]));
                try
                {
                    Clients.Client(ConnectionDictionary.mapUidToConnection[ID]).receiveSMS(message);
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.Message);
                }
            }
            else
            {
                Trace.TraceError(String.Format("User {0} doesn't exist", ID));
            }
        }





        //public void setPatientStatus(string patientID, AlgoUtils.Status status)
        //{
        //    ConnectionDictionary.mapUidToStatus[patientID] = status;

        //    CaregiverController caregiverController = new CaregiverController();
        //    IEnumerable<Caregiver> caregiversArr = caregiverController.GetCaregiversforPatientID(patientID);
        //    Trace.TraceInformation(String.Format("Sending Patient {0} status: {1} to all caregivers", patientID,
        //        status.ToString()));
        //    foreach (var caregiver in caregiversArr)
        //    {

        //        try
        //        {
        //            ConnectionDictionary.mapUidToStatus[caregiver.Id] = status;
        //            Clients.Client(ConnectionDictionary.mapUidToConnection[caregiver.Id]).receivePatientStatus(status);
        //        }
        //        catch (Exception e)
        //        {
        //            Trace.TraceError(e.Message);
        //        }
        //    }
        //}


        //public void sendNotificationToCareGivers(string ID, string patientName, AlgoUtils.Status type)
        //{
        //    if (ConnectionDictionary.mapUidToConnection.ContainsKey(ID))
        //    {
        //        if (type.Equals(AlgoUtils.Status.Wandering))
        //        {
        //            Trace.TraceInformation(String.Format("Sending Wandering Alert to user: {0}, for patient {1}. ConnectionId {2}", ID, patientName, ConnectionDictionary.mapUidToConnection[ID]));
        //            try
        //            {
        //                Clients.Client(ConnectionDictionary.mapUidToConnection[ID]).receiveWanderingAlert(patientName);
        //            }
        //            catch (Exception e)
        //            {
        //                Trace.TraceError(e.Message);
        //            }
        //        }
        //        else if (type.Equals(AlgoUtils.Status.Distress))
        //        {
        //            Trace.TraceInformation(String.Format("Sending Distress Alert to user: {0}, for patient {1}. ConnectionId {2}", ID, patientName, ConnectionDictionary.mapUidToConnection[ID]));
        //            try
        //            {
        //                Clients.Client(ConnectionDictionary.mapUidToConnection[ID]).receiveDistressAlert(patientName);
        //            }
        //            catch (Exception e)
        //            {
        //                Trace.TraceError(e.Message);
        //            }
        //        }
        //        else if (type.Equals(AlgoUtils.Status.Risk))
        //        {
        //            Trace.TraceInformation(String.Format("Sending Risk Alert to user: {0}, for patient {1}. ConnectionId {2}", ID, patientName, ConnectionDictionary.mapUidToConnection[ID]));
        //            try
        //            {
        //                Clients.Client(ConnectionDictionary.mapUidToConnection[ID]).receiveRiskAlert(patientName);
        //            }
        //            catch (Exception e)
        //            {
        //                Trace.TraceError(e.Message);
        //            }
        //        }

        //    }
        //    else
        //    {
        //        Trace.TraceError(String.Format("User {0} doesn't exist", ID));
        //    }
        //}






        //public void sendLostConnNotificationToCareGivers(string ID, string patientName)
        //{
        //    if (ConnectionDictionary.mapUidToConnection.ContainsKey(ID))
        //    {
        //        Trace.TraceInformation(String.Format("Sending Lost Connection Alert to user: {0}, for patient {1}. ConnectionId {2}", ID, patientName, ConnectionDictionary.mapUidToConnection[ID]));
        //        try
        //        {
        //            Clients.Client(ConnectionDictionary.mapUidToConnection[ID]).receiveConnectionLostAlert(patientName);

        //            //just for shits TODO delete
        //            Clients.Client(ConnectionDictionary.mapUidToConnection[ID]).receiveHelpButtonAlert(patientName);
        //        }
        //        catch (Exception e)
        //        {
        //            Trace.TraceError(e.Message);
        //        }
        //    }
        //    else
        //    {
        //        Trace.TraceError(String.Format("User {0} doesn't exist", ID));
        //    }



        ////Testing
        public void startWanderingDetection(string ID)
        {
            Trace.TraceInformation("Starting Detection Algo for Patient {0}", ID);
            WanderingAlgo algo = new WanderingAlgo();
            algo.wanderingDetectionAlgo(ID);
        }
    }
}


