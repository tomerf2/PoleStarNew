
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

        int SendRateMinutes = 8;
        ThreadPoolTimer timer;


        public PatientMainPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            bandInstance = new BandManager();
            await bandInstance.BandInit();
            measurements = new Measurements();

            //set screen on
            //Display display = new Display();
            //display.ActivateDisplay();

            //initiate connection with server set listeners:
            await Notifications.initHubConnection();

            //await Notifications.NotificationHubProxy.Invoke("Register", Utils.StoredData.getUserGUID());

            //set listeners
            //Notifications.NotificationHubProxy.On<string>("receiveWanderingSMS", OnWanderingSMSAlert);
            //Notifications.NotificationHubProxy.On<string>("receiveDistressSMS", OnDistressSMSAlert);
            //Notifications.NotificationHubProxy.On<string>("receiveRiskSMS", OnRiskSMSAlert);
            //Notifications.NotificationHubProxy.On<string>("receiveHelpButtonSMS", OnHelpButtonSMSAlert);

            //TODO: DELETE CAREGIVER LISTENERS - FOR TESTING ONLY
            //resetListeners();
            //Notifications.NotificationHubProxy.On<Notifications.Status>("receivePatientStatus", OnReceivePatientStatus);

            //start periodic timer
            //StartTimer();

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
            sample.PatientID = StoredData.getUserGUID();
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
            try
            {
                Notifications.NotificationHubProxy.Invoke("sendHelpButtonNotificationToCareGivers",
                    Utils.StoredData.getUserGUID());
                Notifications.NotificationHubProxy.Invoke("printConnections");
            }
            catch (Exception ex)
            {
                DialogBox.ShowOk("Error", ex.Message);
            }

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        private async void buttonMeasure_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await measurements.GetAllMeasurements(bandInstance);
                await InsertSample();
                DialogBox.ShowOk("Success", "sent measurements to server");

            }
            catch (Exception ex)
            {
                DialogBox.ShowOk("Error", ex.Message);
            }
        }

        private void buttonAlgo_Click(object sender, RoutedEventArgs e)
        {
            Notifications.startWanderingAlgo();
            Notifications.NotificationHubProxy.Invoke("printConnections");
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LocationsPage), null);
        }


        //PATIENT NOTIFICATION LISTENERS
        private void OnWanderingSMSAlert(string phoneNumber)
        {
            string message = "PoleStar Wandering Alert!\n Please check the PoleStar app for more details";
            SMS.sendSMS(phoneNumber, message);
        }

        private void OnDistressSMSAlert(string phoneNumber)
        {
            string message = "PoleStar Distress Alert!\n Please check the PoleStar app for more details";
            SMS.sendSMS(phoneNumber, message);
        }

        private void OnRiskSMSAlert(string phoneNumber)
        {
            string message = "PoleStar Risk Alert!\n Please check the PoleStar app for more details";
            SMS.sendSMS(phoneNumber, message);
        }

        private void OnHelpButtonSMSAlert(string phoneNumber)
        {
            string message = "PoleStar Requires Assistance Alert!\n Please check the PoleStar app for more details";
            SMS.sendSMS(phoneNumber, message);
        }


        private void resetListeners()
        {
            Notifications.NotificationHubProxy.On<string>("receiveWanderingAlert", OnWanderingAlert);
            Notifications.NotificationHubProxy.On<string>("receiveRiskAlert", OnRiskAlert);
            Notifications.NotificationHubProxy.On<string>("receiveDistressAlert", OnDistressAlert);
            Notifications.NotificationHubProxy.On<string>("receiveConnectionLostAlert", OnLostConnAlert);
            Notifications.NotificationHubProxy.On<string>("receiveHelpButtonAlert", OnHelpButtonAlert);
        }
        //TODO CAREGIVER LISTENERS - DELETE
        private void OnReceivePatientStatus(Notifications.Status status)
        {
            //TODO: set status on screen
        }
        private void OnWanderingAlert(string patientName)
        {
            DialogBox.ShowOk("Wandering Alert", "PoleStar detects that " + patientName + " is at risk of wandering. Please check the PoleStar app for more details");
            resetListeners();
        }

        private void OnRiskAlert(string patientName)
        {
            DialogBox.ShowOk("Serious Risk", "PoleStar detects that " + patientName + " is at risk. Please immediately check the PoleStar app for more details");
            resetListeners();
        }

        private void OnDistressAlert(string patientName)
        {
            DialogBox.ShowOk("Distress", "PoleStar detects that " + patientName + " is in distress. Please check the PoleStar app for more details");
            resetListeners();
        }

        private void OnHelpButtonAlert(string patientName)
        {
            DialogBox.ShowOk("Needs Assistance", patientName + " has pressed the distress button and requires your assistance. Please check the PoleStar app for " + patientName + "'s current location");
            resetListeners();
        }

        private void OnLostConnAlert(string patientName)
        {
            DialogBox.ShowOk("Lost Connection", "PoleStar lost connection with " + patientName + ". Please check the PoleStar app for " + patientName + "'s last known location");
            resetListeners();
        }
    }
}