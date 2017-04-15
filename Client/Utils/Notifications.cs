using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.AspNet.SignalR.Client;
using PoleStar;


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
            NotificationHubProxy.On<string>("wanderingAlert", OnWanderingAlert);
            //NotificationHubProxy.On<string>("Send", OnSend);


            await NotificationHubProxy.Invoke("Register", Utils.StoredData.getUserGUID());


            return true;
        }

        private static void OnWanderingAlert(string obj)
        {
            DialogBox.ShowOk("Error", "Wandering Alert!!" + obj);

        }

        private static void OnSend(string obj)
        {
            DialogBox.ShowOk("Error", obj);

        }
    }

}


