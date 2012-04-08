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

namespace JoggingBuddy
{
    public class Run
    {
        public string Date { get; set; }
        public string Dist { get; set; }
        public string Type { get; set; }

        public Run(string date, string dist)
        {
            this.Date = date;
            this.Dist = dist;
            this.Type = "/personrun.png";
        }
    }
}
