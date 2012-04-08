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
using System.IO.IsolatedStorage;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace JoggingBuddy
{
    public partial class History : PhoneApplicationPage
    {
        public History()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            List<Run> runList = new List<Run>();
            string time, dist;

            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream("SavedSessions.txt", FileMode.Open, IsolatedStorageFile.GetUserStoreForApplication()))
            {
                if (!stream.CanRead) return;

                long length = stream.Length;
                byte[] decoded = new byte[length];
                stream.Read(decoded, 0, (int)length);

                string str = "";
                for (int i = 0; i < decoded.Length; i++)
                    str += (char)decoded[i];

                string[] val = Regex.Split(str, " ");

                time = val[0] + ":" + val[1] + ":" + val[2] + val[3];
                dist = val[4];

                runList.Add(new Run(time, dist));
            }

            TransactionList.ItemsSource = runList;
        }
    }
}