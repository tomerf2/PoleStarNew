using Microsoft.WindowsAzure.MobileServices;
using PoleStar.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PoleStar.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CaregiverMainPage : Page
    {
        public CaregiverMainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TimeSpan period = TimeSpan.FromSeconds(5);

            ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
            {
                //
                // Update the UI thread by using the UI core dispatcher.
                //
                await Dispatcher.RunAsync(CoreDispatcherPriority.High,
                    async () =>
                    {
                        await GetLatestSamples();
                    });

            }, period);


            CenterMap(32, 34);

            mcMap.ZoomLevel = 7;
        }

        private async Task GetLatestSamples()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                // This code refreshes the entries in the list view by querying the TodoItems table.
                // The query excludes completed TodoItems.
                /*groupCargivers = await groupCargiverTable
                    .Where(groupCargiver => groupCargiver.CaregiverID == "123" && groupCargiver.GroupID == "123")
                    .ToCollectionAsync();*/
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
            }
            else
            {
                CenterMap(32.109333, 34.855499);
                mcMap.ZoomLevel = 10;
                //ListItems.ItemsSource = items;
                //this.ButtonSave.IsEnabled = true;
            }
        }

        private void CenterMap(double lat, double lon)
        {
            mcMap.Center = new Geopoint(new BasicGeoposition()
            {
                Latitude = lat,
                Longitude = lon
            });
        }
    }
}
