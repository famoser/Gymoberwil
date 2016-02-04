using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Gymoberwil.Resources;
using Windows.Storage;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Gymoberwil
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Konstruktor
        public MainPage()
        {
            InitializeComponent();
        }


        public async void Load()
        {
            progress.Text = "Beamer wird heruntergeladen...";
            progress.IsIndeterminate = true;
            progress.IsVisible = true;
            bool firsttime = Save.LoadAll();
            if (firsttime)
            {
                Variables.Stunden = new List<List<Stunde>>();

                Wochentag w = Wochentag.Montag;

                for (int a = 0; a < 5; a++)
                {
                    if (a == 1)
                    {
                        w = Wochentag.Dienstag;
                    }
                    else if (a == 2)
                    {
                        w = Wochentag.Mittwoch;
                    }
                    else if (a == 3)
                    {
                        w = Wochentag.Donnerstag;
                    }
                    else if (a == 4)
                    {
                        w = Wochentag.Freitag;
                    }
                    Variables.Stunden.Add(new List<Stunde>());

                    for (int i = 0; i < 12; i++)
                    {
                        Stunde st = new Stunde();
                        st.SetLektion(Lektion.Frei, null);
                        st.Tag = w;
                        st.zeit = i;
                        Variables.Stunden[a].Add(st);
                    }
                }
                var c = MessageBox.Show("Sie öffnen diese App zum ersten Mal. Diese App unterstützt das automatische Herunterladen des Stundenplanes von isy.gymoberwil.ch. Wollen Sie dies jetzt tun?", "Stundenplan herunterladen", MessageBoxButton.OKCancel);
                if (c == MessageBoxResult.OK)
                {
                    NavigationService.Navigate(new Uri("/Stundenplanaktualisieren.xaml?selecteditem=0", UriKind.Relative));
                }
            }

            int tries = 0;
            int matchcounter = 0;
            try
            {
                while (tries < 3)
                {
                    string html = await Helper.DownloadStringAsync(new Uri("https://schulnetz.sbl.ch/gymow/dview/showterminliste.php?id=6zfgfbejsdtwgv3hcuwegujdbg"));
                    if (html != null)
                    {
                        //Lehrer werden formatiert, um eine texterkennung durchzuführen (dem user zu sagen, ob ein Item auf dem Beamer ihn betrifft
                        List<string> Lehrer = new List<string>();
                        foreach (var day in Variables.Stunden)
                        {
                            foreach (var item in day)
                            {
                                Lehrkraft lk = item.GetLehrer;
                                if (lk != null && lk.Vorname != null && lk.Vorname.Count() > 2 && lk.Nachname != null && lk.Nachname.Count() > 2 && !Lehrer.Contains(lk.Vorname.Substring(0, 1) + ". " + lk.Nachname))
                                {
                                    Lehrer.Add(lk.Vorname.Substring(0, 1) + ". " + lk.Nachname);
                                }
                            }
                        }

                        //HTML wird formatiert
                        if (html.Contains("<tr>"))
                        {
                            html = html.Substring(html.IndexOf("<tr>"));
                            //wenn s html mol gfixxt wird....
                            /*
                            if (html.Contains("</tr>"))
                            {
                                html = html.Substring(0, html.LastIndexOf("</tr>") + ("</tr>").Count());
                            }
                             * */
                            html = html.Substring(0, html.LastIndexOf("</table>"));
                            Variables.BeamerItems = HTML.GetBeamerEntries(html.Trim());
                            /*
                            string[] key = new string[] { "&Auml;", "&auml;", "&Ouml;", "&ouml;", "&Uuml;", "&uuml;", "&szlig;" };
                            string[] value = new string[] { "Ä", "ä", "Ö", "ö;", "Ü;", "ü", "&" };
                            */
                            foreach (var item in Variables.BeamerItems)
                            {
                                foreach (var lehrperson in Lehrer)
                                {
                                    if (item.title.Contains(lehrperson))
                                    {
                                        item.important = true;
                                        break;
                                    }
                                }
                                if (item.title.Contains(Settings.klasse) && Settings.klasse != "")
                                {
                                    item.important = true;
                                }
                                if (item.important)
                                {
                                    matchcounter++;
                                }
                            }
                            Save.SaveBeamer();
                            Variables.BeamerActualized = DateTime.Now;
                            if (matchcounter == 0)
                            {
                                progress.Text = "";
                            }
                            else if (matchcounter == 1)
                            {
                                progress.Text = "Eine wichtige Nachricht auf dem Beamer";
                            }
                            else
                            {
                                progress.Text = matchcounter.ToString() + " wichtige Nachrichten auf dem Beamer";
                            }
                            progress.IsIndeterminate = false;
                            break;
                        }
                        else
                        {
                            //abbrechen
                        }
                        
                    }
                    else
                    {
                        if (++tries > 2)
                        {
                            Save.LoadBeamer();
                            progress.Text = "Keine Internetverbindung";
                            progress.IsIndeterminate = false;
                        }
                    }
                }
            }
            catch
            {
                Variables.BeamerItems = new List<Gymoberwil.Beamer>();
                progress.Text = "Beameraktualisierung fehlgeschlagen";
                progress.IsIndeterminate = false;
            }
            MakeNotifications();
            await PutTaskDelay();
            progress.Text = "";
            progress.IsVisible = false;
        }

        async Task PutTaskDelay()
        {
            await Task.Delay(5000);
        }


        private void MakeNotifications()
        {
            //Bus: Wann fährt nächster Bus?
            try
            {
                Bus.Notification = Helper.NächterBus();

                Noten.Notification = Helper.NotenNotification();

                Stundenplan.Notification = Helper.StundenplanNotification();

                //Beamer: Zeige Anzahl Einträge
                if (Variables.BeamerItems.Count == 0)
                {
                    Beamer.Notification = "Keine Einträge";
                }
                else if (Variables.BeamerItems.Count == 1)
                {
                    Beamer.Notification = "Einen Eintrag";
                }
                else
                {
                    Beamer.Notification = Variables.BeamerItems.Count.ToString() + " Einträge";
                }
            }
            catch { return; }
        }

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Beamer.xaml", UriKind.Relative));
        }

        private void Image_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Bus.xaml", UriKind.Relative));
        }

        private void Image_Tap_2(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Noten.xaml?delete=false&pivotitem=0", UriKind.Relative));
        }

        private void Image_Tap_3(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Stundenplan.xaml?delete=false&selecteditem=0", UriKind.Relative));
        }

        private void einstellungen_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Einstellungen.xaml", UriKind.Relative));
        }

        private void über_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Über.xaml", UriKind.Relative));
        }

        public static bool loaded = false; 
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!loaded)
            {
                loaded = true;
                Load();
            }
        }

        // Beispielcode zur Erstellung einer lokalisierten ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // ApplicationBar der Seite einer neuen Instanz von ApplicationBar zuweisen
        //    ApplicationBar = new ApplicationBar();

        //    // Eine neue Schaltfläche erstellen und als Text die lokalisierte Zeichenfolge aus AppResources zuweisen.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Ein neues Menüelement mit der lokalisierten Zeichenfolge aus AppResources erstellen
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}