using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Gymoberwil
{
    public partial class Page1 : PhoneApplicationPage
    {
        public Page1()
        {
            InitializeComponent();
            Beamer.ItemsSource = Variables.BeamerItems;
            TimeSpan s = DateTime.Now - Variables.BeamerActualized;
            if (s.Hours == 0 && s.Days == 0)
            {
                if (s.Minutes == 1)
                {
                    BeamerDate.Text = "vor einer Minute";
                }
                else if (s.Minutes != 0)
                {
                    BeamerDate.Text = "vor " + s.Minutes + " Minuten";
                }
                else
                {
                    BeamerDate.Text = "gerade eben";
                }
            }
            else if (s.Days == 0)
            {
                if (s.Hours == 1)
                {
                    BeamerDate.Text = "vor einer Stunde";
                }
                else
                {
                    BeamerDate.Text = "vor " + s.Hours + " Stunden";
                }
            }
            else
            {
                if (s.Days == 1)
                {
                    BeamerDate.Text = "vor einem Tag";
                }
                else
                {
                    BeamerDate.Text = "vor " + s.Days + " Tagen";
                }
            }
        }
    }
}