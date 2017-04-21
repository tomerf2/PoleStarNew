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
using Windows.UI;
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
    public sealed partial class PatientLoginPage : Page
    {
        private MobileServiceCollection<Group, Group> groups;
        private MobileServiceCollection<Patient, Patient> patients;

        private IMobileServiceTable<Group> groupTable = App.MobileService.GetTable<Group>();
        private IMobileServiceTable<Patient> patientTable = App.MobileService.GetTable<Patient>();

        public PatientLoginPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            groups = await groupTable.ToCollectionAsync();
            patients = await patientTable.ToCollectionAsync();
        }

        private void txtGroupname_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtGroupname.Text == "Groupname")
            {
                txtGroupname.Text = "";
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.Black;
                txtGroupname.Foreground = brush;
            }
        }

        private void txtGroupname_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtGroupname.Text == String.Empty)
            {
                txtGroupname.Text = "Groupname";
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.Gray;
                txtGroupname.Foreground = brush;
            }
        }

        private void txtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password == "Password")
            {
                txtPassword.Password = "";
                txtPassword.PasswordRevealMode = PasswordRevealMode.Hidden;
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.Black;
                txtPassword.Foreground = brush;
            }
        }

        private void txtPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password == String.Empty)
            {
                txtPassword.Password = "Password";
                txtPassword.PasswordRevealMode = PasswordRevealMode.Visible;
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.Gray;
                txtPassword.Foreground = brush;
            }
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            symLoading.IsActive = true;
            symLoading.Visibility = Windows.UI.Xaml.Visibility.Visible;

            this.Frame.Navigate(typeof(PatientSignupPage), null);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            symLoading.IsActive = true;
            symLoading.Visibility = Windows.UI.Xaml.Visibility.Visible;

            this.Frame.Navigate(typeof(MainPage), null);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            symLoading.IsActive = true;
            symLoading.Visibility = Windows.UI.Xaml.Visibility.Visible;

            if ((txtGroupname.Text != "Groupname") && (txtPassword.Password != "Password"))
            {
                var resultGroups = groups.Where(g => g.Name == txtGroupname.Text);
                if (resultGroups.Count() > 0)
                {
                    Group group = resultGroups.ToList()[0];
                    var resultPatients = patients.Where(p => p.GroupID == group.Id);
                    Patient patient = resultPatients.ToList()[0];

                    if (patient.Password == Crypto.CreateMD5Hash(txtPassword.Password))
                    {
                        StoredData.storePatientData(patient.Id); //store in local app data
                        StoredData.loadUserData();
                        this.Frame.Navigate(typeof(PatientMainPage), null);
                    }
                    else
                        DialogBox.ShowOk("Error", "Wrong Groupname or Password. Please try again.");
                }
                else
                    DialogBox.ShowOk("Error", "Wrong Groupname or Password. Please try again.");
            }
            else
                DialogBox.ShowOk("Error", "Please fill all the fields to login.");

            symLoading.IsActive = false;
            symLoading.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
    }
}
