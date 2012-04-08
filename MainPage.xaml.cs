/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using System.Collections.Generic;
using System;
using System.Windows.Media;
using Microsoft.Phone.Shell;

namespace JoggingBuddy
{
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// Constructor
        /// </summary>

        bool road;
        public MainPage()
        {
            InitializeComponent();

            if (Globals.lat != 999)
            {
                Pushpin p = new Pushpin();
                p.Location = new GeoCoordinate(Globals.lat, Globals.lon);
                p.Content = "You";
                p.Background = new SolidColorBrush(Colors.Orange);
                p.Foreground = new SolidColorBrush(Colors.Black);
                mapControl.Items.Add(p);
                map1.Center = new GeoCoordinate(Globals.lat, Globals.lon);
                map1.ZoomLevel = 18;
            }

            drawPath();

            road = false;
            map1.Mode = new AerialMode();
        }

        public void drawPath()
        {
            MapPolyline _joggingPolyLine = new MapPolyline();
            _joggingPolyLine.Stroke = new SolidColorBrush(Colors.Blue);
            _joggingPolyLine.StrokeThickness = 15;
            _joggingPolyLine.Opacity = 0.7;
            
            _joggingPolyLine.Locations = new LocationCollection();
            for (int i=1; i<Globals.history.Count; i++)
                _joggingPolyLine.Locations.Add(Globals.history[i]);

            map1.Children.Add(_joggingPolyLine);
        }

        private void MapView_Click(object sender, EventArgs e)
        {
            if (road)
                map1.Mode = new AerialMode();
            else 
                map1.Mode = new RoadMode();

            road = !road;
        }

    }
}
