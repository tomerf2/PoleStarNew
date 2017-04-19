using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.AspNet.SignalR.Client;
using PoleStar;
using Microsoft.WindowsAzure.MobileServices;
using PoleStar.DataModel;


namespace PoleStar.Utils
{
    class Notifications
    {
        //private static Notifications _Instance;
        private static HubConnection NotificationHubConnection { get; set; }
        public static IHubProxy NotificationHubProxy { get; set; }

        public enum Status
        {
            Safety,
            Wandering,
            Distress,
            Risk,
            ConnectionLost,
            NeedsAssistance //help button pressed
        }



        public static async Task initHubConnection()
        {
            await ConnectHub();
        }

        internal static async Task<bool> ConnectHub()
        {
            NotificationHubConnection =
                new HubConnection(App.MobileService.MobileAppUri.AbsoluteUri);
            NotificationHubProxy = NotificationHubConnection.CreateHubProxy("NotificationHub");
            await NotificationHubConnection.Start();
            if (NotificationHubConnection.State != ConnectionState.Connected)
            {
                DialogBox.ShowOk("Error", "Could connect to server. Ensure You have internet access and try again.");
                return false;
            }

            //set listeners:
            NotificationHubProxy.On<string>("receiveWanderingAlert", OnWanderingAlert);
            NotificationHubProxy.On<string>("receiveRiskAlert", OnRiskAlert);
            NotificationHubProxy.On<string>("receiveDistressAlert", OnDistressAlert);
            NotificationHubProxy.On<string>("receiveConnectionLostAlert", OnLostConnAlert);
            NotificationHubProxy.On<string>("receiveHelpButtonAlert", OnHelpButtonAlert);
            NotificationHubProxy.On<Status>("receivePatientStatus", OnReceivePatientStatus);


            NotificationHubProxy.On<string>("receiveWanderingSMS", OnWanderingSMSAlert);
            NotificationHubProxy.On<string>("receiveDistressSMS", OnDistressSMSAlert);
            NotificationHubProxy.On<string>("receiveRiskSMS", OnRiskSMSAlert);
            NotificationHubProxy.On<string>("receiveHelpButtonSMS", OnHelpButtonSMSAlert);

            //register
            await NotificationHubProxy.Invoke("Register", Utils.StoredData.getUserGUID());

            await NotificationHubProxy.Invoke("sendHelpButtonNotificationToCareGivers", Utils.StoredData.getUserGUID());
            return true;
        }

        //INVOKATIONS
        public static void startWanderingAlgo()
        {
            NotificationHubProxy.Invoke("startWanderingDetection", Utils.StoredData.getUserGUID());
        }

        public static void sendHelpButtonAlert()
        {
            NotificationHubProxy.Invoke("sendHelpButtonNotificationToCareGivers", Utils.StoredData.getUserGUID());
            NotificationHubProxy.Invoke("sendPatientStatus", Utils.StoredData.getUserGUID(), Status.NeedsAssistance);
        }


        //CAREGIVER LISTENERS
        private static void OnReceivePatientStatus(Status status)
        {
            //TODO: set status on screen
        }
        private static void OnWanderingAlert(string patientName)
        {
            DialogBox.ShowOk("Wandering Alert", "PoleStar detects that " + patientName + " is at risk of wandering. Please check the PoleStar app for more details");
        }

        private static void OnRiskAlert(string patientName)
        {
            DialogBox.ShowOk("Serious Risk", "PoleStar detects that " + patientName + " is at risk. Please immediately check the PoleStar app for more details");
        }

        private static void OnDistressAlert(string patientName)
        {
            DialogBox.ShowOk("Distress", "PoleStar detects that " + patientName + " is in distress. Please check the PoleStar app for more details");
        }

        private static void OnHelpButtonAlert(string patientName)
        {
            DialogBox.ShowOk("Needs Assistance", patientName + " has pressed the distress button and requires your assistance. Please check the PoleStar app for " + patientName + "'s current location");
        }

        //PATIENT LISTENERS
        private static void OnWanderingSMSAlert(string phoneNumber)
        {
            string message = "PoleStar Wandering Alert!\n Please check the PoleStar app for more details";
            SMS.sendSMS(phoneNumber, message);
        }

        private static void OnDistressSMSAlert(string phoneNumber)
        {
            string message = "PoleStar Distress Alert!\n Please check the PoleStar app for more details";
            SMS.sendSMS(phoneNumber, message);
        }

        private static void OnRiskSMSAlert(string phoneNumber)
        {
            string message = "PoleStar Risk Alert!\n Please check the PoleStar app for more details";
            SMS.sendSMS(phoneNumber, message);
        }

        private static void OnHelpButtonSMSAlert(string phoneNumber)
        {
            string message = "PoleStar Requires Assistance Alert!\n Please check the PoleStar app for more details";
            SMS.sendSMS(phoneNumber, message);
        }

        private static void OnLostConnAlert(string patientName)
        {
            DialogBox.ShowOk("Lost Connection", "PoleStar lost connection with " + patientName + ". Please check the PoleStar app for " + patientName + "'s last known location");
        }
    }

}


