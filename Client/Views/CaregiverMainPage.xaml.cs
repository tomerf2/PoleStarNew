using Microsoft.WindowsAzure.MobileServices;
using PoleStar.DataModel;
using PoleStar.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
using Microsoft.AspNet.SignalR.Client;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PoleStar.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CaregiverMainPage : Page
    {
        private List<Sample> samplesList;
        private IMobileServiceTable<Sample> sampleTable = App.MobileService.GetTable<Sample>();
        private static Notifications.Status patientStatus;
        private MapIcon mLastPatientLocIcon = new MapIcon();

        public CaregiverMainPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //var caregiver = await App.MobileService.GetTable<Caregiver>().LookupAsync(StoredData.getUserGUID());
            //var patients = await App.MobileService.GetTable<Patient>().ToCollectionAsync();
            //var parameters = new Dictionary<string, string>();
            //parameters.Add("patientID", "0ef25de3-50a5-4592-a365-155a45a357a1");
            //samples = await sampleTable.WithParameters(parameters).ToCollectionAsync();
            //var samples1 = await App.MobileService.InvokeApiAsync<IQueryable<Sample>>("Values/GetAllSamplesByPatientID", HttpMethod.Get, parameters);
            //var samples1 = await App.MobileService.InvokeApiAsync<IQueryable<Sample>>("values/GetAllSamplesByPatientID", HttpMethod.Get,
            //    new Dictionary<string, string>() { { "patientID", "0ef25de3-50a5-4592-a365-155a45a357a1" } });


            //init connection and set listeners
            try
            {
                await Notifications.initHubConnection();
                Notifications.NotificationHubProxy.On<Message>("receiveNotification", NotificationResponse);
                await Notifications.NotificationHubProxy.Invoke("RegisterCaregiver", Utils.StoredData.getUserGUID());
            }
            catch (Exception connFail)
            {
                DialogBox.ShowOk("Error", "Could not connect Azure server, retrying");
                this.Frame.Navigate(typeof(CaregiverMainPage), null);
            }

            if (patientStatus != Notifications.Status.Learning)
            {
                await CreateHeatMap();

                TimeSpan hmPeriod = TimeSpan.FromSeconds(24 * 60 * 60);
                ThreadPoolTimer hmPeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
                {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low,
                        async () =>
                        {
                            await CreateHeatMap();
                        });

                }, hmPeriod);
            }
        }

        private async Task CreateHeatMap()
        {
            var samples = await sampleTable.ToCollectionAsync();
            samplesList = samples.Where(s => s.PatientID == StoredData.getPatientID()).ToList();

            //Create a list contains all groups of samples with distance up to 100m
            List<List<Sample>> sampleGroups = new List<List<Sample>>();
            List<int> sampleIndexesAdded = new List<int>();
            int maxGroupCount = 0;

            for (int i = 0; i < samplesList.Count; i++)
            {
                //Get group's index that contains the current sample
                int groupIndex = -1;

                for (int j = 0; j < sampleGroups.Count; j++)
                {
                    if (sampleGroups[j].Contains(samplesList[i]))
                    {
                        groupIndex = j;
                        break;
                    }
                }

                if (groupIndex == -1)
                {
                    sampleGroups.Add(new List<Sample>() { samplesList[i] });
                    groupIndex = sampleGroups.Count - 1;

                    sampleIndexesAdded.Add(i);

                    if (maxGroupCount == 0) maxGroupCount = 1;
                }

                //Find more samples to add this group
                for (int j = i + 1; j < samplesList.Count; j++)
                {
                    if (samplesList[i].DistanceTo(samplesList[j]) < 100)
                    {
                        if (!sampleGroups[groupIndex].Contains(samplesList[j]) & !sampleIndexesAdded.Contains(j))
                        {
                            sampleGroups[groupIndex].Add(samplesList[j]);
                            sampleIndexesAdded.Add(j);

                            if (maxGroupCount < sampleGroups[groupIndex].Count)
                                maxGroupCount = sampleGroups[groupIndex].Count;
                        }
                    }
                }
            }

            foreach (var sampleGroup in sampleGroups)
            {
                if (sampleGroup.Count > 2)
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
        }

        private void ShowLatestSample(float lat, float lon)
        {
            mcMap.MapElements.Remove(mLastPatientLocIcon);
            mLastPatientLocIcon.Visible = true;
            mLastPatientLocIcon.Title = "Patient is here :)";
            mLastPatientLocIcon.Location = new Geopoint(new BasicGeoposition() { Latitude = lat, Longitude = lon });
            mcMap.MapElements.Add(mLastPatientLocIcon);

            CenterMap(lat, lon, 16);
        }

        private void CenterMap(double lat, double lon, int zoom)
        {
            mcMap.ZoomLevel = zoom;

            mcMap.Center = new Geopoint(new BasicGeoposition()
            {
                Latitude = lat,
                Longitude = lon
            });
        }

        public void SetPatientStatus(string status, Brush color)
        {
            patientStatusInd.Text = status;
            patientStatusInd.Foreground = color;
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LocationsPage), null);
        }

        private void NotificationResponse(Message message)
        {
            //store patient id
            if (StoredData.getPatientID() == null)
            {
                StoredData.setPatientID(message.ID);
            }
            patientStatus = message.status;//set status
            OnReceivePatientStatus(message.status);

            ShowLatestSample(message.lat, message.lon);

            switch (message.status)
            {
                case Notifications.Status.ConnectionLost:
                    Notifications.OnLostConnAlert(message.name);
                    break;
                case Notifications.Status.Distress:
                    Notifications.OnDistressAlert(message.name);
                    break;
                case Notifications.Status.NeedsAssistance:
                    Notifications.OnHelpButtonAlert(message.name);
                    break;
                case Notifications.Status.Risk:
                    Notifications.OnRiskAlert(message.name);
                    break;
                case Notifications.Status.Wandering:
                    Notifications.OnWanderingAlert(message.name);
                    break;
            }
        }
        private async void OnReceivePatientStatus(Notifications.Status status)
        {
            switch (status)
            {
                case Notifications.Status.ConnectionLost:
                    await patientStatusInd.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { patientStatusInd.Text = " CONNECION LOST"; patientStatusInd.Foreground = new SolidColorBrush(Colors.Red); });
                    return;
                case Notifications.Status.Distress:
                    await patientStatusInd.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { patientStatusInd.Text = " DISTRESS"; patientStatusInd.Foreground = new SolidColorBrush(Colors.Yellow); });
                    return;
                case Notifications.Status.NeedsAssistance:
                    await patientStatusInd.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { patientStatusInd.Text = " NEEDS ASSISTANCE"; patientStatusInd.Foreground = new SolidColorBrush(Colors.Red); });
                    return;
                case Notifications.Status.Risk:
                    await patientStatusInd.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { patientStatusInd.Text = " RISK"; patientStatusInd.Foreground = new SolidColorBrush(Colors.Red); });
                    return;
                case Notifications.Status.Wandering:
                    await patientStatusInd.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { patientStatusInd.Text = " WANDERING"; patientStatusInd.Foreground = new SolidColorBrush(Colors.LightCoral); });
                    return;
                case Notifications.Status.Learning:
                    await patientStatusInd.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { patientStatusInd.Text = " LEARNING"; patientStatusInd.Foreground = new SolidColorBrush(Colors.Orange); });
                    return;
                case Notifications.Status.Safety:
                    await patientStatusInd.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { patientStatusInd.Text = " OK"; patientStatusInd.Foreground = new SolidColorBrush(Colors.Green); });
                    return;
            }
        }

        private async void setStatus(string status, Brush color)
        {
            await patientStatusInd.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => { patientStatusInd.Text = status; patientStatusInd.Foreground = color;});
        }
    }
}
