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
        private static IHubProxy NotificationHubProxy { get; set; }



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

            NotificationHubProxy.On<string>("receiveWanderingSMS", OnWanderingSMSAlert);
            NotificationHubProxy.On<string>("receiveDistressSMS", OnDistressSMSAlert);
            NotificationHubProxy.On<string>("receiveRiskSMS", OnRiskSMSAlert);


            //register
            await NotificationHubProxy.Invoke("Register", Utils.StoredData.getUserGUID());
            return true;
        }

        public static void startWanderingAlgo()
        {
            NotificationHubProxy.Invoke("startWanderingDetection", Utils.StoredData.getUserGUID());
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

        private static void OnWanderingSMSAlert(string patientName)
        {
            string message = string.Format(
                "PoleStar detects that {0} is at risk of wandering. Please check the PoleStar app for more details",
                patientName);
        }

        private static void OnDistressSMSAlert(string patientName)
        {
            string message = string.Format(
                "PoleStar detects that {0} is at risk of wandering. Please check the PoleStar app for more details",
                patientName);
        }

        private static void OnRiskSMSAlert(string patientName)
        {
            string message = string.Format(
                "PoleStar detects that {0} is at risk of wandering. Please check the PoleStar app for more details",
                patientName);
        }

        private static void OnLostConnAlert(string patientName)
        {
            DialogBox.ShowOk("Lost Connection", "PoleStar lost connection with " + patientName + ". Please check the PoleStar app for " + patientName + "'s last known location");
        }
    }

}


