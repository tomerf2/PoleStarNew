using Microsoft.WindowsAzure.MobileServices;
using PoleStar.DataModel;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PoleStar.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MobileServiceCollection<Group, Group> groups;

        private IMobileServiceTable<Group> groupTable = App.MobileService.GetTable<Group>();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void btnPatient_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PatientLoginPage), null);
        }

        private void btnCaregiver_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CaregiverLoginPage), null);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            groups = await groupTable.ToCollectionAsync();
            // groups[0].Code
        }
    }

}
