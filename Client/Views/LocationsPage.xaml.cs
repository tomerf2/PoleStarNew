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
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using PoleStar.Utils;
using Windows.UI.Xaml.Controls.Maps;
using Microsoft.AspNet.SignalR.Client;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PoleStar.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocationsPage : Page
    {
        private double mLastLat = 0;
        private double mLastLong = 0;
        private string patientID = StoredData.getPatientID();

        private MobileServiceCollection<Location, Location> locations;

        private IMobileServiceTable<Location> locationTable = App.MobileService.GetTable<Location>();

        private List <Location> patientKnowLocations;

        public LocationsPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                BeginLoadingIcon();

                locations = await locationTable.ToCollectionAsync();

                patientKnowLocations = locations.Where(p => p.PatientID == patientID).ToList(); //filtered by patient

                for (int i = 0; i < patientKnowLocations.Count; i++)
                    AddLocation(patientKnowLocations[i]);

                EndLoadingIcon();
            }
            catch (Exception ConnFail)
            {
                EndLoadingIcon();
                DialogBox.ShowOk("Error", "Communication error with Azure server, attempting to reconnect.");
                this.Frame.Navigate(typeof(LocationsPage), null);
            }

        }
    
        private void BeginLoadingIcon()
        {
            symLoading.IsActive = true;
            symLoading.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void EndLoadingIcon()
        {
            symLoading.IsActive = false;
            symLoading.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private async void AddLocation(Location loc)
        {
            Grid grid = new Grid();

            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());

            TextBlock tbDescription = new TextBlock();
            tbDescription.Text = loc.Description;
            tbDescription.FontWeight = FontWeights.Bold;
            tbDescription.VerticalAlignment = VerticalAlignment.Bottom;
            Grid.SetRow(tbDescription, 0);
            Grid.SetColumn(tbDescription, 0);

            MapAddress mapAddr = await Coordinates.CoordsToAddress(loc.Latitude, loc.Longitude);
            TextBlock tbAddress = new TextBlock();
            tbAddress.Text = "(" + loc.Latitude + ", " + loc.Longitude + ")";

            if (mapAddr.Street != "")
            {
                if (mapAddr.StreetNumber != "")
                    tbAddress.Text = mapAddr.StreetNumber + " " + mapAddr.Street;
                else
                    tbAddress.Text = mapAddr.Street;
            }


            if (mapAddr.Town != "")
            {
                if (mapAddr.Street != "")
                    tbAddress.Text += ", " + mapAddr.Town;
                else
                    tbAddress.Text = mapAddr.Town;
            }

            if (mapAddr.Country != "")
            {
                if (mapAddr.Town != "")
                    tbAddress.Text += ", " + mapAddr.Country;
                else
                    tbAddress.Text = mapAddr.Country;
            }

            tbAddress.FontSize = 12;
            tbAddress.Foreground = new SolidColorBrush(Colors.Red);
            tbAddress.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetRow(tbAddress, 1);
            Grid.SetColumn(tbAddress, 0);

            Image img = new Image();
            img.Source = new BitmapImage(new Uri("ms-appx:///Assets/delete.png"));
            img.Stretch = Stretch.Fill;
            img.Height = 25;
            img.Width = 25;

            Button btnDelete = new Button();
            btnDelete.Content = img;
            btnDelete.Background = new SolidColorBrush(Colors.Transparent);
            btnDelete.HorizontalAlignment = HorizontalAlignment.Right;
            btnDelete.Tag = loc;
            btnDelete.Click += BtnDelete_Click;

            Grid.SetRow(btnDelete, 0);
            Grid.SetColumn(btnDelete, 1);
            Grid.SetRowSpan(btnDelete, 2);

            grid.Children.Add(tbDescription);
            grid.Children.Add(tbAddress);
            grid.Children.Add(btnDelete);

            ListBoxItem lbi = new ListBoxItem();
            lbi.Tag = loc;
            lbi.Content = grid;
            lbi.GotFocus += Lbi_GotFocus;

            lbLocations.Items.Add(lbi);

            MapIcon mi = new MapIcon();
            mi.Visible = true;
            mi.Title = loc.Description;
            mi.Location = new Geopoint(new BasicGeoposition() { Latitude = loc.Latitude, Longitude = loc.Longitude });
            mcMap.MapElements.Add(mi);
        }

        private void Lbi_GotFocus(object sender, RoutedEventArgs e)
        {
            BeginLoadingIcon();

            ListBoxItem lbi = (ListBoxItem)sender;
            Location loc = (Location)lbi.Tag;
            CenterMap(loc.Latitude, loc.Longitude, 17);

            mLastLat = 0;
            mLastLong = 0;

            EndLoadingIcon();
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BeginLoadingIcon();

                Button btn = (Button) sender;
                Location loc = (Location) btn.Tag;

                await locationTable.DeleteAsync(loc);
                patientKnowLocations.Remove(loc);
                lbLocations.Items.Remove(((FrameworkElement) btn.Parent).Parent);

                EndLoadingIcon();
            }
            catch (Exception connErr)
            {
                EndLoadingIcon();
                DialogBox.ShowOk("Error", "Communication error with Azure server, please try again.");
                
            }

        }

        private void txtAddress_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtAddress.Text == "Address")
            {
                txtAddress.Text = "";
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.Black;
                txtAddress.Foreground = brush;
            }
        }

        private void txtAddress_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtAddress.Text == String.Empty)
            {
                txtAddress.Text = "Address";
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.Gray;
                txtAddress.Foreground = brush;
            }
        }

        private void txtDescription_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtDescription.Text == "Description")
            {
                txtDescription.Text = "";
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.Black;
                txtDescription.Foreground = brush;
            }
        }

        private void txtDescription_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtDescription.Text == String.Empty)
            {
                txtDescription.Text = "Description";
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.Gray;
                txtDescription.Foreground = brush;
            }
        }

        private async void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            BeginLoadingIcon();

            string addressToGeocode = txtAddress.Text;

            // The nearby location to use as a query hint.
            BasicGeoposition queryHint = new BasicGeoposition();
            queryHint.Latitude = 47.643;
            queryHint.Longitude = -122.131;
            Geopoint hintPoint = new Geopoint(queryHint);

            MapLocationFinderResult result =
                  await MapLocationFinder.FindLocationsAsync(
                                    addressToGeocode,
                                    hintPoint,
                                    3);

            if (result.Status == MapLocationFinderStatus.Success)
            {
                mLastLat = result.Locations[0].Point.Position.Latitude;
                mLastLong = result.Locations[0].Point.Position.Longitude;
                CenterMap(mLastLat, mLastLong, 17);
            }

            EndLoadingIcon();
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PatientMainPage), null);
        }
        
        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            StoredData.removeAllSavedData();
            this.Frame.Navigate(typeof(MainPage), null);
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BeginLoadingIcon();

                if ((mLastLat == 0) || (mLastLong == 0))
                    DialogBox.ShowOk("Error", "Please write an address and press the browse button.");
                else if (txtDescription.Text == "Description")
                    DialogBox.ShowOk("Error", "Please write a description.");
                else
                {
                    Location location = new Location()
                    {
                        Id = Guid.NewGuid().ToString(),
                        PatientID = patientID,
                        Description = txtDescription.Text,
                        HeartRate = 0,
                        Latitude = (float) mLastLat,
                        Longitude = (float) mLastLong
                    };

                    patientKnowLocations.Add(location); //add to local list
                    await locationTable.InsertAsync(location); //save to server

                    AddLocation(location);

                    mLastLat = 0;
                    mLastLong = 0;
                }

                EndLoadingIcon();
            }

            catch (Exception connFail)
            {
                EndLoadingIcon();
                DialogBox.ShowOk("Error", "Communication error with Azure server, please try again.");
                mLastLat = 0;
                mLastLong = 0;
            }

        }
    }
}
