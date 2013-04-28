using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace PhoneApp
{
    public class LatestMetricsDataModel : NotifyPropertyBag
    {
        private static LatestMetricsDataModel _instance;

        public static LatestMetricsDataModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LatestMetricsDataModel();
                }
                return _instance;
            }
        }

        private string _temperature;
        private string _humidity;
        private string _barometricPressure;
        private string _lightLevel;

        public string Temperature
        {
            get { return _temperature; }
            set
            {
                if (value != _temperature)
                {
                    _temperature = value;
                    OnPropertyChanged("Temperature");
                }
            }
        }

        public string Humidity
        {
            get { return _humidity; }
            set
            {
                if (value != _humidity)
                {
                    _humidity = value;
                    OnPropertyChanged("Humidity");
                }
            }
        }

        public string BarometricPressure
        {
            get { return _barometricPressure; }
            set
            {
                if (value != _barometricPressure)
                {
                    _barometricPressure = value;
                    OnPropertyChanged("BarometricPressure");
                }
            }
        }

        public string LightLevel
        {
            get { return _lightLevel; }
            set
            {
                if (value != _lightLevel)
                {
                    _lightLevel = value;
                    OnPropertyChanged("LightLevel");
                }
            }
        }

        public HiveSenseCommunications CommsHandler { get; set; }

        public async void LoadLatestMetrics()
        {
            CommsHandler.GetLatestMetrics(this);
        }
    }
}
