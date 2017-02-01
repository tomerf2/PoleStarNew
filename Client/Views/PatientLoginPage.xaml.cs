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
        public PatientLoginPage()
        {
            this.InitializeComponent();
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
            this.Frame.Navigate(typeof(PatientSignupPage), null);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), null);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PatientMainPage), null);
        }

    }
}
