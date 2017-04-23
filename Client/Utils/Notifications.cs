using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Microsoft.AspNet.SignalR.Client;
using PoleStar;
using Microsoft.WindowsAzure.MobileServices;
using PoleStar.DataModel;
using PoleStar.Views;


namespace PoleStar.Utils
{
    class Notifications
    {
        //private static Notifications _Instancet;
        private static HubConnection NotificationHubConnection { get; set; }
        public static IHubProxy NotificationHubProxy { get; set; }

        public enum Status
        {
            Safety,
            Wandering,
            Distress,
            Risk,
            ConnectionLost,
            NeedsAssistance, //help button pressed
            Learning //not enough samples collected
        }

        public static async Task initHubConnection()
        {
            await ConnectHub();
        }

        internal static async Task ConnectHub()
        {
            NotificationHubConnection =
                new HubConnection(App.MobileService.MobileAppUri.AbsoluteUri);
            NotificationHubProxy = NotificationHubConnection.CreateHubProxy("NotificationHub");
            await NotificationHubConnection.Start();

            //register
            if (StoredData.isPatient())
            {
                NotificationHubProxy.Invoke("RegisterPatient", Utils.StoredData.getUserGUID());
                NotificationHubProxy.On<SMSMessage>("receiveSMS", SMSResponse);
                NotificationHubProxy.On<Message>("receiveNotification", NotificationResponse); //TODO - Remove

            }//caregiver listeners are in caregiver main page
        }


        //CAREGIVER LISTENERS TODO REMOVE
        private static void NotificationResponse(Message message)
        {
            //store patient id
            if (StoredData.getPatientID() == null)
            {
                StoredData.setPatientID(message.ID);
            }
            switch (message.status)
            {
                case Status.ConnectionLost:
                    OnLostConnAlert(message.name);
                    break;
                case Status.Distress:
                    OnDistressAlert(message.name);
                    break;
                case Status.NeedsAssistance:
                    OnHelpButtonAlert(message.name);
                    break;
                case Status.Risk:
                    OnRiskAlert(message.name);
                    break;
                case Status.Wandering:
                    OnWanderingAlert(message.name);
                    break;
            }
        }

        private static void SMSResponse(SMSMessage message)
        {
            switch (message.status)
            {
                case Status.Distress:
                    OnDistressSMSAlert(message.number);
                    break;
                case Status.NeedsAssistance:
                    OnHelpButtonSMSAlert(message.number);
                    break;
                case Status.Risk:
                    OnRiskSMSAlert(message.number);
                    break;
                case Status.Wandering:
                    OnWanderingSMSAlert(message.number);
                    break;
            }
        }




        public static void OnWanderingAlert(string patientName)
        {
            DialogBox.ShowOk("Wandering Alert", "PoleStar detects that " + patientName + " is at risk of wandering. Please check the PoleStar app for more details");
        }

        public static void OnRiskAlert(string patientName)
        {
            DialogBox.ShowOk("Serious Risk", "PoleStar detects that " + patientName + " is at risk. Please immediately check the PoleStar app for more details");
        }

        public static void OnDistressAlert(string patientName)
        {
            DialogBox.ShowOk("Distress", "PoleStar detects that " + patientName + " is in distress. Please check the PoleStar app for more details");
        }

        public static void OnHelpButtonAlert(string patientName)
        {
            DialogBox.ShowOk("Needs Assistance", patientName + " has pressed the distress button and requires your assistance. Please check the PoleStar app for " + patientName + "'s current location");
        }

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

        public static void OnLostConnAlert(string patientName)
        {
            DialogBox.ShowOk("Lost Connection", "PoleStar lost connection with " + patientName + ". Please check the PoleStar app for " + patientName + "'s last known location");
        }
        public static void startWanderingAlgo()
        {
            NotificationHubProxy.Invoke("startWanderingDetection", Utils.StoredData.getUserGUID());
        }
    }

    class Message
    {
        public string name;
        public string ID;
        public Notifications.Status status;
        public float lat;
        public float lon;
    }
    class SMSMessage
    {
        public string number;
        public Notifications.Status status;
    }
}


