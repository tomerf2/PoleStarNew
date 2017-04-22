using Microsoft.WindowsAzure.MobileServices;
using PoleStar.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

        private MobileServiceCollection<Location, Location> locations;

        private IMobileServiceTable<Location> locationTable = App.MobileService.GetTable<Location>();

        public LocationsPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            symLoading.IsActive = true;
            symLoading.Visibility = Windows.UI.Xaml.Visibility.Visible;

            locations = await locationTable.ToCollectionAsync();

            for (int i = 0; i < locations.Count; i++)
            {
                Grid grid = new Grid();

                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());

                TextBlock tbDescription = new TextBlock();
                tbDescription.Text = locations[i].Description;
                tbDescription.FontWeight = FontWeights.Bold;
                tbDescription.VerticalAlignment = VerticalAlignment.Bottom;
                Grid.SetRow(tbDescription, 0);
                Grid.SetColumn(tbDescription, 0);

                MapAddress mapAddr = await Coordinates.CoordsToAddress(locations[i].Latitude, locations[i].Longitude);
                TextBlock tbAddress = new TextBlock();
                tbAddress.Text = "(" + locations[i].Latitude + ", " + locations[i].Longitude + ")";

                if (mapAddr.Street != "")
                {
                    if (mapAddr.StreetNumber != "")
                        tbAddress.Text = mapAddr.StreetNumber + " " + mapAddr.Street;
                    else
                        tbAddress.Text = mapAddr.Street;
                }


                if(mapAddr.Town != "")
                {
                    if (mapAddr.Street != "")
                        tbAddress.Text += ", " + mapAddr.Town;
                    else
                        tbAddress.Text = mapAddr.Town;
                }

                if(mapAddr.Country != "")
                {
                    if(mapAddr.Town != "")
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
                
                Grid.SetRow(btnDelete, 0);
                Grid.SetColumn(btnDelete, 1);
                Grid.SetRowSpan(btnDelete, 2);

                grid.Children.Add(tbDescription);
                grid.Children.Add(tbAddress);
                grid.Children.Add(btnDelete);

                ListBoxItem lbi = new ListBoxItem();
                lbi.Tag = new BasicGeoposition() { Latitude = locations[i].Latitude, Longitude = locations[i].Longitude };
                lbi.Content = grid;
                lbi.GotFocus += Lbi_GotFocus;

                lbLocations.Items.Add(lbi);

                MapIcon mi = new MapIcon();
                mi.Visible = true;
                mi.Title = locations[i].Description;
                mi.Location = new Geopoint((BasicGeoposition)lbi.Tag);
                mcMap.MapElements.Add(mi);
            }

            symLoading.IsActive = false;
            symLoading.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void Lbi_GotFocus(object sender, RoutedEventArgs e)
        {
            ListBoxItem lbi = (ListBoxItem)sender;
            BasicGeoposition geoPos = (BasicGeoposition)lbi.Tag;
            CenterMap(geoPos.Latitude, geoPos.Longitude, 17);
            mLastLat = 0;
            mLastLong = 0;
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
            // The address or business to geocode.
            string addressToGeocode = txtAddress.Text;

            // The nearby location to use as a query hint.
            BasicGeoposition queryHint = new BasicGeoposition();
            queryHint.Latitude = 47.643;
            queryHint.Longitude = -122.131;
            Geopoint hintPoint = new Geopoint(queryHint);

            // Geocode the specified address, using the specified reference point
            // as a query hint. Return no more than 3 results.
            MapLocationFinderResult result =
                  await MapLocationFinder.FindLocationsAsync(
                                    addressToGeocode,
                                    hintPoint,
                                    3);

            // If the query returns results, display the coordinates
            // of the first result.
            if (result.Status == MapLocationFinderStatus.Success)
            {
                mLastLat = result.Locations[0].Point.Position.Latitude;
                mLastLong = result.Locations[0].Point.Position.Longitude;
                CenterMap(mLastLat, mLastLong, 17);
            }
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

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if ((mLastLat == 0) || (mLastLong == 0))
                DialogBox.ShowOk("Error", "Please write an address and press the browse button.");
            else if(txtDescription.Text == "Description")
                DialogBox.ShowOk("Error", "Please write a description.");
            else
            {
                //TODO: get correct patient id
                Location location = new Location()
                {
                    Id = Guid.NewGuid().ToString(),
                    PatientID = Guid.NewGuid().ToString(),
                    Description = txtDescription.Text,
                    HeartRate = 0,
                    Latitude = (float)mLastLat,
                    Longitude = (float)mLastLong
                };

                await locationTable.InsertAsync(location);
            }
        }
    }
}
