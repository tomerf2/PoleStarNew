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
using Microsoft.WindowsAzure.MobileServices;
using PoleStar.DataModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PoleStar.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PatientMainPage : Page
    {
        private MobileServiceCollection<Sample, Sample> samples;
#if OFFLINE_SYNC_ENABLED
        private IMobileServiceSyncTable<Sample> sampleTable = App.MobileService.GetSyncTable<Sample>(); // offline sync
#else
        private IMobileServiceTable<Sample> sampleTable = App.MobileService.GetTable<Sample>();
#endif

        //static BandManager bandInstance;
        Measurements measurements;

        public PatientMainPage()
        {
            this.InitializeComponent();
        }

        private /*async*/ void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //samples = await sampleTable.ToCollectionAsync();
            //bandInstance = new BandManager();
            //await bandInstance.BandInit();
            //measurements = new Measurements();
        }

        private async Task InsertSample(/*Measurements measurement*/)
        {
            //initiate new sample object with current meassurement parameters
            Sample sample = new Sample() {
                Id = Guid.NewGuid().ToString(),
                HeartRate = 100,
                Latitude = 32.1F,
                Longitude = 33.2F,
                PatientID = "6a758ca8-dc2a-4e35-ba12-82a92b7919cf"
            };
            //if (measurement.has_loc)
            //{
            //    sample.Latitude = (float)measurement.Location.Coordinate.Latitude;
            //    sample.Latitude = (float)measurement.Location.Coordinate.Longitude;
            //}
            //if (measurement.has_heart)
            //{
            //    sample.HeartRate = measurement.Heartrate;
            //}
            //save on server
            await sampleTable.InsertAsync(sample);
            //samples.Add(sample);

#if OFFLINE_SYNC_ENABLED
            await App.MobileService.SyncContext.PushAsync(); // offline sync
#endif
        }

        private async void btnAssist_Click(object sender, RoutedEventArgs e)
        {

            //await measurements.GetAllMeasurements(bandInstance);
            await InsertSample(/*this.measurements*/);


        }
    }
}
