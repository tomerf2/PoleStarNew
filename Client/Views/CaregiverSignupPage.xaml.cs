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
    public sealed partial class CaregiverSignupPage : Page
    {
        private MobileServiceCollection<Group, Group> groups;
        private MobileServiceCollection<Caregiver, Caregiver> caregivers;

        private IMobileServiceTable<Group> groupTable = App.MobileService.GetTable<Group>();
        private IMobileServiceTable<Caregiver> caregiverTable = App.MobileService.GetTable<Caregiver>();

        public CaregiverSignupPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            groups = await groupTable.ToCollectionAsync();
            caregivers = await caregiverTable.ToCollectionAsync();
        }

        private void txtPhone_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtPhone.Text == "Phone")
            {
                txtPhone.Text = "";
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.Black;
                txtPhone.Foreground = brush;
            }
        }

        private void txtPhone_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtPhone.Text == String.Empty)
            {
                txtPhone.Text = "Phone";
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.Gray;
                txtPhone.Foreground = brush;
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
            this.Frame.Navigate(typeof(CaregiverLoginPage), null);
        }

        private async void btnForward_Click(object sender, RoutedEventArgs e)
        {
            symLoading.IsActive = true;
            symLoading.Visibility = Windows.UI.Xaml.Visibility.Visible;

            if ((txtEmail.Text != "Email") && (txtPhone.Text != "Phone") &&
                (txtGroupname.Text != "Groupname") && (txtCode.Password != "Code"))
            {
                var resultGroups = groups.Where(g => g.Name == txtGroupname.Text);
                if (resultGroups.Count() == 1)
                {
                    Group group = resultGroups.ToList()[0];
                    if (group.Code == txtCode.Password)
                    {
                        Caregiver newCaregiver = new Caregiver() { Id = Guid.NewGuid().ToString(), GroupID = group.Id, Phone = txtPhone.Text, Email = txtEmail.Text, IsApproved = false };

                        await caregiverTable.InsertAsync(newCaregiver);

                        this.Frame.Navigate(typeof(CaregiverMainPage), null);
                    }
                    else
                        DialogBox.ShowOk("Error", "Group's name does not exist or wrong code.");
                }
                else
                    DialogBox.ShowOk("Error", "Group's name does not exist or wrong code.");
            }
            else
                DialogBox.ShowOk("Error", "Please fill all the fields.");

            symLoading.IsActive = false;
            symLoading.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
    }
}
