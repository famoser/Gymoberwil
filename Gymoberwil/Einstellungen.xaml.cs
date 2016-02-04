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
    public partial class Einstellungen : PhoneApplicationPage
    {
        public Einstellungen()
        {
            InitializeComponent();
            if (Settings.bus == "bus64")
            {
                buspicker.SelectedIndex = 1;
            }
            if (Settings.busrichtung == "allschwil")
            {
                richtungpicker.SelectedIndex = 1;
            }
            klasse.Text = Settings.klasse;
        }

        private void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (buspicker.SelectedIndex == 1)
            {
                Settings.bus = "bus64";
            }
            else
            {
                Settings.bus = "bus61";
            }
            if (richtungpicker.SelectedIndex == 1)
            {
                Settings.busrichtung = "allschwil";
            }
            else
            {
                Settings.busrichtung = "oberwil";
            }
            Settings.klasse = klasse.Text;
            Save.SaveSettings();
            MessageBox.Show("gespeichert");
        }
    }
}