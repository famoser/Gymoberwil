using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Storage;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Threading;

namespace Gymoberwil.Apps
{
    public partial class PivotPage1 : PhoneApplicationPage
    {
        public PivotPage1()
        {
            InitializeComponent();

            Bus61.ItemsSource = Helper.Bus(Mode.Selected, Variables.Bus61Allschwil, Variables.Bus61Oberwil);
            Bus64.ItemsSource = Helper.Bus(Mode.Selected, Variables.Bus64Allschwil, Variables.Bus64Oberwil);
            if (Settings.bus == "bus64")
            {
                BusPivot.SelectedIndex = 1;
            }
            else
            {
                BusPivot.SelectedIndex = 0;
            }
            SetTimer();
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (BusPivot.SelectedIndex == 0)
            {
                Bus61.SetValue(TurnstileFeatherEffect.FeatheringIndexProperty, 1);
                Bus64.SetValue(TurnstileFeatherEffect.FeatheringIndexProperty, -1);
            }
            else
            {
                Bus61.SetValue(TurnstileFeatherEffect.FeatheringIndexProperty, -1);
                Bus64.SetValue(TurnstileFeatherEffect.FeatheringIndexProperty, 1);
            }
        }

        public void SetTimer()
        {
            DispatcherTimer t = new System.Windows.Threading.DispatcherTimer();
            t.Interval = TimeSpan.FromSeconds(1);
            t.Tick += new EventHandler((sender, e) => OnTimerTick(sender, e, false));
            t.Start();
            DispatcherTimer tt = new System.Windows.Threading.DispatcherTimer();
            tt.Interval = TimeSpan.FromSeconds(30);
            tt.Tick += new EventHandler((sender, e) => OnTimerTick(sender, e, true));
            tt.Start();
        }

        public void OnTimerTick(Object sender, EventArgs args, bool dosomething)
        {
            if (dosomething || Variables.reactivated)
            {
                if (Variables.reactivated) { Variables.reactivated = false; }
                List<Bus> list = Bus61.ItemsSource as List<Bus>;
                Bus61.ItemsSource = null;
                Bus61.ItemsSource = list;
                list = Bus64.ItemsSource as List<Bus>;
                Bus64.ItemsSource = null;
                Bus64.ItemsSource = list;
            }
        }
    }
}