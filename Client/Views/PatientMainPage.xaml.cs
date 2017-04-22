
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

        int SendRateMinutes = 5;
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

            //initiate connection with server set listeners:
            try
            {
                await Notifications.initHubConnection();
            }
            catch (Exception)
            {
                DialogBox.ShowOk("Error", "Communication error with Azure server, attempting to reconnect.");
                this.Frame.Navigate(typeof(PatientMainPage), null);
            }
            //start periodic timer
            //StartTimer();


            //await CreateData.insertSamples(250, CreateData.HouseLat, CreateData.HouseLong, 75);
            //await CreateData.insertSamples(62, CreateData.ClubLat, CreateData.ClubLong, 80);
            //await CreateData.insertSamples(240, CreateData.DaugtherLat, CreateData.DaugtherLong, 78);
            //await CreateData.insertSamples(22, CreateData.StoreLat, CreateData.StoreLong, 75);
            //await CreateData.insertSamples(12, CreateData.GrandsonLat, CreateData.GrandsonLong, 75);
            //await CreateData.insertSamples(86, CreateData.FeldenLat, CreateData.FeldenLong, 103);
            //await CreateData.insertSamples(22, CreateData.DocLat, CreateData.DocLong, 83);


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
                Notifications.startWanderingAlgo();
            }
        }


        private void btnAssist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Notifications.NotificationHubProxy.Invoke("sendHelpButtonNotificationToCareGivers",
                    Utils.StoredData.getUserGUID());
            }
            catch (Exception ex)
            {
                DialogBox.ShowOk("Error", "Communication error with Azure server, please try again.");
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        private async void buttonTest_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Testing), null);

        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LocationsPage), null);
        }

        private async void MeaurementsButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                await measurements.GetAllMeasurements(bandInstance);
                await InsertSample();
                DialogBox.ShowOk("Success", "sent measurements to server");

            }
            catch (Exception ex)
            {
                DialogBox.ShowOk("Error", "Communication error with Azure server, please try again.");
            }
        }
    }
}