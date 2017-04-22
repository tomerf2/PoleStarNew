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
        private static IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();

        public static void printConnections()
        {
            Trace.TraceInformation(String.Format("{0} entries in dictionary",
                ConnectionDictionary.mapUidToConnection.Count()));

            foreach (var w in ConnectionDictionary.mapUidToConnection)
            {
                Trace.TraceInformation(String.Format("Key: {0}, Val: {1}", w.Key, w.Value));
            }
        }


        public void RegisterPatient(string ID)
        {
            Trace.TraceInformation(String.Format("Attempting to register patient: {0}", ID));
            try
            {
                Trace.AutoFlush = true;
                string deadConnectionId;
                string ConnID = Context.ConnectionId;
                ConnectionDictionary.mapUidToConnection.TryRemove(ID, out deadConnectionId);
                ConnectionDictionary.mapUidToConnection[ID] = ConnID;
                Trace.TraceInformation(String.Format("Added patient: {0} connectionId {1}", ID,
                    ConnectionDictionary.mapUidToConnection[ID]));
            }
            catch (Exception e)
            {
                Trace.TraceError("Registration of " + ID + " failed: " + e.Message);
            }
        }

        public void RegisterCaregiver(string ID)
        {
            Trace.TraceInformation(String.Format("Attempting to register cargiver: {0}", ID));
            try
            {
                Trace.AutoFlush = true;
                string deadConnectionId;
                string ConnID = Context.ConnectionId;
                ConnectionDictionary.mapUidToConnection.TryRemove(ID, out deadConnectionId);
                ConnectionDictionary.mapUidToConnection[ID] = ConnID;
                Trace.TraceInformation(String.Format("Added caregiver: {0} connectionId {1}", ID,
                    ConnectionDictionary.mapUidToConnection[ID]));


                Caregiver currCaregiver = CaregiverController.GetCaregiverObject(ID);
                Patient currPatient = PatientController.GetPatientObjectbyGroupID(currCaregiver.GroupID);
                //send patient ID to caregiver
                Trace.TraceInformation("Sending message back to caregiver with patient ID");
                Message message = new Message() {ID = currPatient.Id, status = AlgoUtils.Status.Safety, name = ""};
                Clients.Client(ConnectionDictionary.mapUidToConnection[ID]).receiveNotification(message);
                WanderingAlgo algo = new WanderingAlgo();
                Trace.TraceInformation("Starting Detection Algo for Patient {0} due to caregiver registration", currPatient.Id);
                algo.wanderingDetectionAlgo(currPatient.Id);
            }
            catch (Exception e)
            {
                Trace.TraceError("Registration of " + ID + " failed or Exception in wandering Algo: " + e.Message);
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

            static_send(patientID, patientName, caregiversArr, AlgoUtils.Status.NeedsAssistance);
        }


        public static void static_sendSMS(string ID, string phoneNumber, AlgoUtils.Status type)
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
                    hubContext.Clients.Client(ConnectionDictionary.mapUidToConnection[ID]).receiveSMS(message);
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



        public static void static_send(string patientID, string patientName, IEnumerable<Caregiver> caregiversArr,
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
                    hubContext.Clients.Client(ConnectionDictionary.mapUidToConnection[caregiver.Id]).receiveNotification(message);
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
                hubContext.Clients.Client(ConnectionDictionary.mapUidToConnection[patientID]).receiveNotification(message);
                Trace.TraceInformation(String.Format("Sent message back to patient {0}", patientName));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }

        }

        //public void getPatientID(string caregiverID)
        //{
        //    Trace.TraceInformation(String.Format("Retrieving patient ID for caregiver"));
        //    Caregiver caregiverObj = CaregiverController.GetCaregiverObject(caregiverID);
        //    Patient patientObj = PatientController.GetPatientObjectbyGroupID(caregiverObj.GroupID);

        //    Trace.TraceInformation(String.Format("Sending patientID to caregiver {0} in response to request", caregiverID));
        //    Clients.Client(ConnectionDictionary.mapUidToConnection[caregiverID]).patientID(patientObj.Id);
        //}

        ////Testing
        public void startWanderingDetection(string ID)
        {
            Trace.TraceInformation("Starting Detection Algo for Patient {0}", ID);
            WanderingAlgo algo = new WanderingAlgo();
            algo.wanderingDetectionAlgo(ID);
        }
    }
}


