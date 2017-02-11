using Microsoft.WindowsAzure.MobileServices;
using PoleStar.DataModel;
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
    public sealed partial class PatientSignupPage : Page
    {
        private MobileServiceCollection<Group, Group> groups;
        private MobileServiceCollection<Patient, Patient> patients;

        private IMobileServiceTable<Group> groupTable = App.MobileService.GetTable<Group>();
        private IMobileServiceTable<Patient> patientTable = App.MobileService.GetTable<Patient>();

        public PatientSignupPage()
        {
            this.InitializeComponent();
        }

        private void txtName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtName.Text == "Full name")
            {
                txtName.Text = "";
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.Black;
                txtName.Foreground = brush;
            }
        }

        private void txtName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtName.Text == String.Empty)
            {
                txtName.Text = "Full name";
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.Gray;
                txtName.Foreground = brush;
            }
        }

        private void txtEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtEmail.Text == "Email")
            {
                txtEmail.Text = "";
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.Black;
                txtEmail.Foreground = brush;
            }
        }

        private void txtEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtEmail.Text == String.Empty)
            {
                txtEmail.Text = "Email";
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.Gray;
                txtEmail.Foreground = brush;
            }
        }

        private void txtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password == "Private password")
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
                txtPassword.Password = "Private password";
                txtPassword.PasswordRevealMode = PasswordRevealMode.Visible;
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.Gray;
                txtPassword.Foreground = brush;
            }
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

        private void txtCode_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtCode.Password == "Code")
            {
                txtCode.Password = "";
                txtCode.PasswordRevealMode = PasswordRevealMode.Hidden;
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.Black;
                txtCode.Foreground = brush;
            }
        }

        private void txtCode_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtCode.Password == String.Empty)
            {
                txtCode.Password = "Code";
                txtCode.PasswordRevealMode = PasswordRevealMode.Visible;
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.Gray;
                txtCode.Foreground = brush;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PatientLoginPage), null);
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
