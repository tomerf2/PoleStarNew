using Microsoft.WindowsAzure.MobileServices;
using PoleStar.DataModel;
using PoleStar.Utils;
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
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
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
        private MobileServiceCollection<Sample, Sample> samples;

        private IMobileServiceTable<Sample> sampleTable = App.MobileService.GetTable<Sample>();

        public CaregiverMainPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            /*TimeSpan period = TimeSpan.FromSeconds(5);

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

            }, period);*/

            samples = await sampleTable.ToCollectionAsync();

            //Create a list contains all groups of samples with distance up to 150m
            List<List<Sample>> sampleGroups = new List<List<Sample>>();
            List<int> sampleIndexesAdded = new List<int>();
            int maxGroupCount = 0;

            for(int i = 0; i < samples.Count; i++)
            {
                //Get group's index that contains the current sample
                int groupIndex = -1;

                for(int j = 0; j < sampleGroups.Count; j++)
                {
                    if(sampleGroups[j].Contains(samples[i]))
                    {
                        groupIndex = j;
                        break;
                    }
                }

                if(groupIndex == -1)
                {
                    sampleGroups.Add(new List<Sample>() { samples[i] });
                    groupIndex = sampleGroups.Count - 1;

                    sampleIndexesAdded.Add(i);

                    if (maxGroupCount == 0) maxGroupCount = 1;
                }

                //Find more samples to add this group
                for(int j = i + 1; j < samples.Count; j++)
                {
                    if(samples[i].DistanceTo(samples[j]) < 100)
                    {
                        if (!sampleGroups[groupIndex].Contains(samples[j]) & !sampleIndexesAdded.Contains(j))
                        {
                            sampleGroups[groupIndex].Add(samples[j]);
                            sampleIndexesAdded.Add(j);

                            if (maxGroupCount < sampleGroups[groupIndex].Count)
                                maxGroupCount = sampleGroups[groupIndex].Count;
                        }
                    }
                }
            }

            foreach(var sampleGroup in sampleGroups)
            {
                //TODO: Prevent showing groups with up to 20 samples
                if (sampleGroup.Count == 1)
                {
                    MapIcon mi = new MapIcon();
                    mi.Visible = true;
                    mi.Title = "One sample group";
                    mi.Location = new Geopoint(new BasicGeoposition() { Latitude = sampleGroup[0].Latitude, Longitude = sampleGroup[0].Longitude });
                    mcMap.MapElements.Add(mi);
                }
                else
                {
                    List<Sample> chSampleGroup = HeatMap.ConvexHull(sampleGroup);

                    var polygon = new MapPolygon();

                    //Set appearance
                    polygon.FillColor = HeatMap.GetColorByDensity(sampleGroup.Count, maxGroupCount);
                    polygon.StrokeThickness = 0;

                    var geoPosList = new List<BasicGeoposition>();

                    for (int i = 0; i < chSampleGroup.Count; i++)
                        geoPosList.Add(new BasicGeoposition() { Latitude = chSampleGroup[i].Latitude, Longitude = chSampleGroup[i].Longitude });

                    //Create path
                    polygon.Path = new Geopath(geoPosList);

                    //Add to map
                    mcMap.MapElements.Add(polygon);
                }
            }
            
            /*MapIcon mi = new MapIcon();
            mi.Visible = true;
            mi.Title = "Last patient location (4 min ago)";
            mi.Location = new Geopoint(new BasicGeoposition() { Latitude = 32.3029140, Longitude = 34.8761971 });
            mcMap.MapElements.Add(mi);*/

            mcMap.ZoomLevel = 16;
            CenterMap(32.3026161, 34.8748978);
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
