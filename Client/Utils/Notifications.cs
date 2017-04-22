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
            if (StoredData.isCaregiver())
            {
                NotificationHubProxy.Invoke("RegisterCaregiver", Utils.StoredData.getUserGUID());
                NotificationHubProxy.On<Message>("receiveNotification", NotificationResponse);
            }
            else //patient
            {
                NotificationHubProxy.Invoke("RegisterPatient", Utils.StoredData.getUserGUID());
                NotificationHubProxy.On<SMSMessage>("receiveSMS", SMSResponse);
                NotificationHubProxy.On<Message>("receiveNotification", NotificationResponse); //TODO - Remove
            }
        }


        //CAREGIVER LISTENERS
        private static void NotificationResponse(Message message)
        {
            OnReceivePatientStatus(message.status);

            //switch (message.status)
            //{
            //        case Status.ConnectionLost:
            //            OnLostConnAlert(message.name);
            //            break;
            //        case Status.Distress:
            //            OnDistressAlert(message.name);
            //            break;
            //        case Status.NeedsAssistance:
            //            OnHelpButtonAlert(message.name);
            //            break;
            //        case Status.Risk:
            //            OnRiskAlert(message.name);
            //            break;
            //        case Status.Wandering:
            //            OnWanderingAlert(message.name);
            //            break;
            //}
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


        private static void OnReceivePatientStatus(Status status)
        {
            DialogBox.ShowOk("Status", "PoleStar detects that " + status.ToString());

            //switch (status)
            //{
            //    case Status.ConnectionLost:
            //        CaregiverMainPage.SetPatientStatus("   CONNECION LOST", new SolidColorBrush(Colors.Red));
            //        return;
            //    case Status.Distress:
            //        CaregiverMainPage.SetPatientStatus("   DISTRESS", new SolidColorBrush(Colors.Yellow));
            //        return;
            //    case Status.NeedsAssistance:
            //        CaregiverMainPage.SetPatientStatus("   NEEDS ASSISTANCE", new SolidColorBrush(Colors.Red));
            //        return;
            //    case Status.Risk:
            //        CaregiverMainPage.SetPatientStatus("   RISK", new SolidColorBrush(Colors.Red));
            //        return;
            //    case Status.Wandering:
            //        CaregiverMainPage.SetPatientStatus("   WANDERING", new SolidColorBrush(Colors.LightCoral));
            //        return;
            //    case Status.Learning:
            //        CaregiverMainPage.SetPatientStatus("   LEARNING", new SolidColorBrush(Colors.Orange));
            //        return;
            //    case Status.Safety:
            //        CaregiverMainPage.SetPatientStatus("   OK", new SolidColorBrush(Colors.Green));
            //        return;
            //}
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
    }
    class SMSMessage
    {
        public string number;
        public Notifications.Status status;
    }
}


