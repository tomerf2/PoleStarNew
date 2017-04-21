
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Band;
using System.Threading.Tasks;
using PoleStar.Band;
using PoleStar.Utils;
using Microsoft.WindowsAzure.MobileServices;
using PoleStar.DataModel;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Popups;
using Microsoft.AspNet.SignalR.Client;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PoleStar.Views
{

    public sealed partial class PatientMainPage : Page
    {

        private IMobileServiceTable<Sample> sampleTable = App.MobileService.GetTable<Sample>();
        static BandManager bandInstance;
        Measurements measurements;

        int SendRateMinutes = 4;
        ThreadPoolTimer timer;


        public PatientMainPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            /*Random rand = new Random();
            for (int i = 0; i < 200; i++)
            {
                Sample sample = new Sample();
                sample.Id = Guid.NewGuid().ToString();
                sample.PatientID = StoredData.getUserGUID();
                sample.Latitude = (float)(32.3026 + 0.000001 * rand.Next(-999, 999));
                sample.Longitude = (float)(34.8748 + 0.000001 * rand.Next(-999, 999));
                sample.HeartRate = rand.Next(60, 110);
                //save on server
                await sampleTable.InsertAsync(sample);
            }*/

            bandInstance = new BandManager();
            await bandInstance.BandInit();
            measurements = new Measurements();

            //initiate connection with server:
            await Notifications.initHubConnection();
            Notifications.NotificationHubProxy.On<string>("receiveHelpButtonAlert", OnHelpButtonAlert);


        //start timer
        //StartTimer();

    }
        private static void OnHelpButtonAlert(string patientName)
        {
            DialogBox.ShowOk("Needs Assistance", patientName + "'s has pressed the distress button and requires your assistance. Please check the PoleStar app for " + patientName + "'s current location");
        }

        public void StartTimer()
        {
            timer = ThreadPoolTimer.CreatePeriodicTimer(TimerElapsedHandler, new TimeSpan(0, SendRateMinutes, 0));
        }

        private async void TimerElapsedHandler(ThreadPoolTimer timer)
        {
            try
            {
                await measurements.GetAllMeasurements(bandInstance);
                await InsertSample();
            }
            catch (Exception e)
            {
                DialogBox.ShowOk("Error", "Could not connect to band or Azure server");
            }
        }


        private async Task InsertSample()
        {
            //initiate new sample object with current measurement parameters
            Sample sample = new Sample();
            sample.Id = Guid.NewGuid().ToString();
            sample.PatientID = StoredData.getUserGUID(); // previously "6a758ca8-dc2a-4e35-ba12-82a92b7919cf";
            if (measurements.has_loc && measurements.has_heart)
            {
                sample.Latitude = (float) measurements.Location.Coordinate.Latitude;
                sample.Longitude = (float) measurements.Location.Coordinate.Longitude;
                sample.HeartRate = measurements.Heartrate;
                //save on server
                await sampleTable.InsertAsync(sample);
            }
        }


        private void btnAssist_Click(object sender, RoutedEventArgs e)
        {
            Notifications.sendHelpButtonAlert();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        private async void buttonMeasure_Click(object sender, RoutedEventArgs e)
        {
            await measurements.GetAllMeasurements(bandInstance);
            await InsertSample();
        }

        private void buttonAlgo_Click(object sender, RoutedEventArgs e)
        {
            Notifications.startWanderingAlgo();
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LocationsPage), null);
        }
    }
}