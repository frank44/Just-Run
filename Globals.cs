using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Device.Location;
using System.IO.IsolatedStorage;

namespace JoggingBuddy
{
    public static class Globals
    {
        public static bool usingMeters = true;
        public static double lat=999, lon=999, dist;
        public static List<GeoCoordinate> history = new List<GeoCoordinate>();
    }
}

