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
using HtmlAgilityPack;
using System.Windows.Threading;

namespace Gymoberwil
{
    public partial class PivotPage2 : PhoneApplicationPage
    {
        public PivotPage2()
        {
            InitializeComponent();
            if (Helper.GetWeek() == Woche.B0Woche)
            {
                Stundenplan.Title = "STUNDENPLAN                                b-woche";
            }

            if (Variables.Stunden != null)
            {
                if (Variables.Stunden.Count == 5)
                {
                    Montag.ItemsSource = Variables.Stunden[0];
                    Dienstag.ItemsSource = Variables.Stunden[1];
                    Mittwoch.ItemsSource = Variables.Stunden[2];
                    Donnerstag.ItemsSource = Variables.Stunden[3];
                    Freitag.ItemsSource = Variables.Stunden[4];
                }
            }
            SetTimer();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.Count > 0)
            {
                if (Variables.Stunden.Count == 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Variables.Stunden.Add(new List<Stunde>());
                    }
                    for (int a = 0; a < Variables.Stunden.Count; a++)
                    {
                        for (int b = 0; b < 12; b++)
                        {
                            Variables.Stunden[a].Add(new Stunde() { Tag = (Wochentag)a, zeit = b });
                            Variables.Stunden[a][Variables.Stunden[a].Count - 1].SetLektion(Lektion.Frei, null);
                        }
                    }
                }
                if (NavigationContext.QueryString["delete"] == "true")
                {
                    NavigationService.RemoveBackEntry();
                    NavigationService.RemoveBackEntry();
                }
                Stundenplan.SelectedIndex = Convert.ToInt32(NavigationContext.QueryString["selecteditem"]);
            
        
            }
        }

        public DispatcherTimer t = new DispatcherTimer();
        public void SetTimer()
        {
            string[] str = Helper.SplitTime(DateTime.Now);
            Stelle1.Text = str[0];
            Stelle2.Text = str[1];
            Stelle3.Text = str[2];
            Stelle4.Text = str[3];
            Stelle5.Text = str[4];
            Stelle6.Text = str[5];
            OnTimerTick(null, null);
            t.Interval = TimeSpan.FromSeconds(0.95);
            t.Tick += OnTimerTick;
            t.Start();
        }

        public void OnTimerTick(Object sender, EventArgs args)
        {
            string[] str = Helper.SplitTime(DateTime.Now);
            Stelle1.Text = str[0];
            Stelle2.Text = str[1];
            Stelle3.Text = str[2];
            Stelle4.Text = str[3];
            Stelle5.Text = str[4];
            Stelle6.Text = str[5];
            if (str[4] == "0" && str[5] == "0" || sender == null || Variables.reactivated)
            {
                if (Variables.reactivated)
                {
                    Variables.reactivated = false;
                }
                Jetzt.ItemsSource = Helper.GetNextHours();
                object ob = Helper.GetNow();
                if (ob == null)
                {
                    Frei f = ob as Frei;
                    Balken.Height = 334;
                    Balken.Background = Variables.Fächer[0].Color;
                    bigletter.Text = "F";
                    smalletter.Text = "rei";
                    endzeit.Text ="";
                    Minuten.Text = "";
                }
                else if (ob.GetType() == typeof(Frei))
                {
                    Frei f = ob as Frei;
                    Balken.Height = 334;
                    Balken.Background = Variables.Fächer[0].Color;
                    bigletter.Text = "F";
                    smalletter.Text = "rei";
                    endzeit.Text = f.EndTime;
                    Minuten.Text = f.Lengh;
                }
                else if (ob.GetType() == typeof(Stunde))
                {
                    Stunde stunde = ob as Stunde;
                    Balken.Height = stunde.RelativeTime * 334;
                    Balken.Background = stunde.Color;
                    bigletter.Text = stunde.FachTeil1;
                    smalletter.Text = stunde.FachTeil2;
                    endzeit.Text = stunde.EndTime;
                    Minuten.Text = stunde.Lengh;
                }
                else if (ob.GetType() == typeof(Pause))
                {
                    Pause f = ob as Pause;
                    Balken.Height = f.RelativeTime * 334;
                    Balken.Background = Variables.Fächer[0].Color;
                    bigletter.Text = "P";
                    smalletter.Text = "ause";
                    endzeit.Text = f.EndTime;
                    Minuten.Text = f.Lengh;
                }
            }
        }

        private void download_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Stundenplanaktualisieren.xaml?selecteditem=" + Stundenplan.SelectedIndex.ToString(), UriKind.Relative));
        }

        private void Longlist_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LongListSelector s = sender as LongListSelector;
            Variables.activeStunde = (Stunde)s.SelectedItem;
            NavigationService.Navigate(new Uri("/Stundeerstellen.xaml?selecteditem=" + Stundenplan.SelectedIndex.ToString(), UriKind.Relative));
        }
    }
}