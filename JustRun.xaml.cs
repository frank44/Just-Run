using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Threading;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Shell;
using System.Device.Location;
using System.IO.IsolatedStorage;
using System.Text;
using System.Diagnostics;

namespace JoggingBuddy
{
    public partial class JustRun : PhoneApplicationPage
    {
        DateTime startTime;
        DispatcherTimer timer;
        bool running;
        GeoCoordinateWatcher watcher;

        public JustRun()
        {
            InitializeComponent();
            running = false;

            Microsoft.Phone.Shell.PhoneApplicationService.Current.ApplicationIdleDetectionMode =
            Microsoft.Phone.Shell.IdleDetectionMode.Disabled;

            // The watcher variable was previously declared as type GeoCoordinateWatcher. 
            if (watcher == null)
            {
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High); // using high accuracy
                //watcher.MovementThreshold = 1; // use MovementThreshold to ignore noise in the signal

                watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
                watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
                watcher.Start();

                Globals.lat = watcher.Position.Location.Latitude;
                Globals.lon = watcher.Position.Location.Longitude;
            }

            Globals.dist = 0.0;

            if (Globals.usingMeters)
            {
                tbUnits.Text = "km";
                tbV.Text = "km/h";
            }
            else
            {
                tbUnits.Text = "mi";
                tbV.Text = "mph";
            }
        }

        // Event handler for the GeoCoordinateWatcher.StatusChanged event.
        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    // The Location Service is disabled or unsupported.
                    // Check to see whether the user has disabled the Location Service.
                    if (watcher.Permission == GeoPositionPermission.Denied)
                    {
                        // The user has disabled the Location Service on their device.
                        //statusTextBlock.Text = "you have this application access to location.";
                    }
                    else
                    {
                        //statusTextBlock.Text = "location is not functioning on this device";
                    }
                    break;

                case GeoPositionStatus.Initializing:
                    // The Location Service is initializing.
                    // Disable the Start Location button.
                    // bStart.IsEnabled = false;
                    break;

                case GeoPositionStatus.NoData:
                    // The Location Service is working, but it cannot get location data.
                    // Alert the user and enable the Stop Location button.
                    //statusTextBlock.Text = "location data is not available.";
                    //bEnd.IsEnabled = true;
                    break;

                case GeoPositionStatus.Ready:
                    // The Location Service is working and is receiving location data.
                    // Show the current position and enable the Stop Location button.
                    //statusTextBlock.Text = "location data is available.";
                    //bEnd.IsEnabled = true;
                    break;
            }
        }

        double toRad(double x)
        {
            return Math.PI * x / 180.0;
        }

        double handleConversion(double r)
        {
            if (Globals.usingMeters) return r;
            else return 0.621371192 * r;
        }

        double distance(double lat1, double lon1, double lat2, double lon2)
        {
            double dLat = toRad(lat2-lat1);
            double dLon = toRad(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2);
            a += Math.Cos(toRad(lat1)) * Math.Cos(toRad(lat2)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double ret = 6371 * c;

            return handleConversion(ret);
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (!running) 
            {
                tbDist.Text = "0.000";
                tbVelo.Text = "0.000";
                return;
            }

            if (Globals.lat != 999)
                Globals.history.Add(new GeoCoordinate(Globals.lat, Globals.lon));

            double nlat = e.Position.Location.Latitude;
            double nlon = e.Position.Location.Longitude;

            double delta = distance(Globals.lat, Globals.lon, nlat, nlon);
            if (delta <= 0.5) //ignore big changes!
            {
                Globals.dist += delta;
            }
            else if (Globals.history.Count >= 2)
                Globals.history.RemoveAt(Globals.history.Count - 2);

            tbDist.Text = String.Format("{0:0.000}", handleConversion(Globals.dist));
            tbVelo.Text = String.Format("{0:0.000}", handleConversion(Globals.dist) / DateTime.Now.Subtract(startTime).TotalHours);
            Globals.lat = nlat;
            Globals.lon = nlon; 

            //ApplicationTitle.Text = Globals.lat + ", " + Globals.lon;
        }

        void OnTimerTick(Object sender, EventArgs args)
        {
            if (!running) return;

            TimeSpan span = DateTime.Now.Subtract(startTime);
            tbHour.Text = span.Hours.ToString("00");
            tbMin.Text = span.Minutes.ToString("00");
            tbSecond.Text = span.Seconds.ToString("00");
            tbMilli.Text = "."+span.Milliseconds.ToString("000");
        }
  
        private void Start_Click(object sender, EventArgs e)
        {
            if (!running)
            {
                running = true;
                ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
                b.IconUri = new Uri("./appbar.cancel.rest.png", UriKind.Relative);

                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(1);
                timer.Start();
                timer.Tick += OnTimerTick;
                startTime = DateTime.Now;
                
            }
            else
            {
                running = false;
                timer.Stop();
                tbHour.Text = "00";
                tbMin.Text = "00";
                tbSecond.Text = "00";
                tbMilli.Text = ".000";

                Globals.dist = 0;
                Globals.lat = 999;
                Globals.lon = 999;
                Globals.history.Clear();

                ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
                b.IconUri = new Uri("./appbar.transport.play.rest.png", UriKind.Relative);
            }
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        private void History_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/History.xaml", UriKind.Relative));
        }

        private void Log_Click(object sender, EventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                watcher.Stop();
            }

            tbResponse.Text = "Session Saved";

            tbResponse.TextAlignment = TextAlignment.Center;

            ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            b.IconUri = new Uri("./appbar.transport.play.rest.png", UriKind.Relative);

            running = false;

            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            string str = tbHour.Text + " " + tbMin.Text + " " + tbSecond.Text + " " + tbMilli.Text + " ";
            str += Globals.dist.ToString("0.000") ;
            str += Globals.history.Count + " ";
            foreach (GeoCoordinate coo in Globals.history)
                str += coo.Latitude + " " + coo.Longitude + " ";

            using (IsolatedStorageFileStream stream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile("SavedSessions.txt", System.IO.FileMode.Append))
            {
                byte[] data = Encoding.UTF8.GetBytes(str);
                stream.Write(data, 0, data.Length);
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (Globals.usingMeters)
                tbUnits.Text = "km";
            else tbUnits.Text = "mi";
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}
