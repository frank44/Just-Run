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

namespace JoggingBuddy
{
    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            InitializeComponent();

            if (Globals.usingMeters)
                rbKm.IsChecked = true;
            else
                rbMiles.IsChecked = true;
        }

        private void TextBlock_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.

        }

        private void rbKm_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void rbMiles_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if ((bool)rbMiles.IsChecked)
                Globals.usingMeters = false;
            else
                Globals.usingMeters = true;
        }


    }
}
