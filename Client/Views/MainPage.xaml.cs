using Microsoft.WindowsAzure.MobileServices;
using PoleStar.DataModel;
using PoleStar.Utils;
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //uncomment to remove all stored data and start app from main screen
            StoredData.removeAllSavedData();

            //Example Patient Data
            //StoredData.storePatientData("Patient Guid", "Password");

            //CreateData.createAndInsertData(); //FOR TESTING

            //set display to on


            if (StoredData.checkForPreviousLogin()) //also loads user data, if found
            {
                if (StoredData.isCaregiver())
                    this.Frame.Navigate(typeof(CaregiverMainPage), null);
                else if (StoredData.isPatient())
                    this.Frame.Navigate(typeof(PatientMainPage), null);
            }
        }
    }

}
