using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace PhoneApp
{
    public partial class DisplayLatestMetricsPage : PhoneApplicationPage
    {
        public DisplayLatestMetricsPage()
        {
            InitializeComponent();

            this.DataContext = LatestMetricsDataModel.Instance;
            LatestMetricsDataModel.Instance.LoadLatestMetrics();
        }
    }
}