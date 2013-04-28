using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneApp.Resources;

namespace PhoneApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            NoDevicesErrorLabel.Visibility = Visibility.Collapsed;

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            LatestMetricsDataModel.Instance.CommsHandler = new HiveSenseCommunications();
            var conn = await LatestMetricsDataModel.Instance.CommsHandler.ConnectToHiveSense();

            NoDevicesErrorLabel.Visibility = conn == false ? Visibility.Visible : Visibility.Collapsed;

            if (conn)
            {
                NavigationService.Navigate(new Uri("/DisplayLatestMetricsPage.xaml", UriKind.Relative));
            }
        }
    }
}