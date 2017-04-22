using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.MobileServices;
using PoleStar.DataModel;
using PoleStar.Utils;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PoleStar.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Testing : Page
    {
        private IMobileServiceTable<Sample> sampleTable = App.MobileService.GetTable<Sample>();

        public Testing()
        {
            this.InitializeComponent();
        }


        private async Task InsertSample(double lat, double lon, int heartrate)
        {
            //initiate new sample object with current measurement parameters
            Sample sample = new Sample();
            sample.Id = Guid.NewGuid().ToString();
            sample.PatientID = StoredData.getUserGUID();

            sample.Latitude = (float)lat;
            sample.Longitude = (float)lon;
            sample.HeartRate = heartrate;
            //save on server
            await sampleTable.InsertAsync(sample);
            Notifications.startWanderingAlgo();
            
        }


        private void SampleButton2_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                InsertSample(CreateData.sample2Lat, CreateData.sample2Long, 75);
            }
            catch (Exception) { }

        }

        private void SampleButton1_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                InsertSample(CreateData.sample1Lat, CreateData.sample1Long, 75);

            }
            catch (Exception) { }
        }

        private void SampleButton3_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                InsertSample(CreateData.sample3Lat, CreateData.sample3Long, 100);
            }
            catch (Exception) { }
        }

        private void SampleButton4_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                InsertSample(CreateData.sample4Lat, CreateData.sample4Long, 140);
            }
            catch (Exception) { }
        }

        private void SampleButton5_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                InsertSample(CreateData.sample5Lat, CreateData.sample5Long, 75);
            }
            catch (Exception) { }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PatientMainPage), null);
        }
    }
}
