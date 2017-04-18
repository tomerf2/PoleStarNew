
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PoleStar.Views
{

    public sealed partial class PatientMainPage : Page
    {
        //private MobileServiceCollection<Sample, Sample> samples;
#if OFFLINE_SYNC_ENABLED
        private IMobileServiceSyncTable<Sample> sampleTable = App.MobileService.GetSyncTable<Sample>(); // offline sync
#else
        private IMobileServiceTable<Sample> sampleTable = App.MobileService.GetTable<Sample>();
#endif

        static BandManager bandInstance;
        Measurements measurements;
        int SendRateMinutes = 2;

        public PatientMainPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //samples = await sampleTable.ToCollectionAsync();
            bandInstance = new BandManager();
            await bandInstance.BandInit();
            measurements = new Measurements();

            //await measurements.GetAllMeasurements(bandInstance);
            //await InsertSample();

            TimeSpan period = TimeSpan.FromMinutes(SendRateMinutes);

            ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High,
                    async () =>
                    {
                        await measurements.GetAllMeasurements(bandInstance);
                        await InsertSample();
                    });

            }, period);

        }

        private async Task InsertSample()
        {
            //initiate new sample object with current measurement parameters
            Sample sample = new Sample();
            sample.Id = Guid.NewGuid().ToString();
            sample.PatientID = StoredData.getUserGUID(); // previously "6a758ca8-dc2a-4e35-ba12-82a92b7919cf";
            if (measurements.has_loc)
            {
                sample.Latitude = (float)measurements.Location.Coordinate.Latitude;
                sample.Longitude = (float)measurements.Location.Coordinate.Longitude;
            }
            if (measurements.has_heart)
            {
                sample.HeartRate = measurements.Heartrate;
            }
            //save on server
            await sampleTable.InsertAsync(sample);


#if OFFLINE_SYNC_ENABLED
            await App.MobileService.SyncContext.PushAsync(); // offline sync
#endif
        }

        private async void btnAssist_Click(object sender, RoutedEventArgs e)
        {

            //await measurements.GetAllMeasurements(bandInstance);
            await InsertSample();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LocationsPage), null);
        }
    }
}
