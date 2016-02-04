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

namespace Gymoberwil
{
    public partial class Stundenplanaktualisieren : PhoneApplicationPage
    {
        public Stundenplanaktualisieren()
        {
            InitializeComponent();
            Klasse.Text = Settings.klasse;
        }


        private void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Abbrechen
            NavigationService.Navigate(new Uri("/Stundenplan.xaml?delete=true&selecteditem=" + NavigationContext.QueryString["selecteditem"], UriKind.Relative));
        }

        private void Button_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Weiter
            Einrichten1.IsHitTestVisible = false;
            Einrichten1.Opacity = 0;
            Einrichten2.IsHitTestVisible = true;
            Einrichten2.Opacity = 1;
        }
        public static string authentication;


        public List<List<StundeWorkingObject>> Stundeen = new List<List<StundeWorkingObject>>();
        private async void Button_Tap_2(object sender, System.Windows.Input.GestureEventArgs e)
        {
            /*
            //Step 3
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Content/Isy-redirect.html"));
            Stream stream = await file.OpenStreamForReadAsync();
            StreamReader s = new StreamReader(stream);
            string text = s.ReadToEnd();
            text = text.Replace("AUTHKEY", authentication);
            text = text.Replace("CLASS", Klasse.Text);

            Webbrowser.NavigateToString(text);
             * */


            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Content/Isy-Html.html"));
            Stream stream = await file.OpenStreamForReadAsync();
            StreamReader s = new StreamReader(stream);
            string text = s.ReadToEnd();
            Webbrowser.IsScriptEnabled = true;
            Webbrowser.NavigateToString(text);

            Einrichten2.IsHitTestVisible = false;
            Einrichten2.Opacity = 0;
            Einrichten3.IsHitTestVisible = true;
            Einrichten3.Opacity = 1;
        }


        public int Aufrufe = 0;
        private void Webbrowser_Navigated(object sender, NavigationEventArgs e)
        {
            if (Aufrufe == 0)
            {
                Aufrufe++;
            }
            else
            {
                if (Aufrufe == 1)
                {
                    Aufrufe++;
                    string s = Klasse.Text;
                    if (Klasse.Text.Contains("_"))
                    {
                        s = Klasse.Text.Substring(0, Klasse.Text.IndexOf("_"));
                    }
                    Settings.klasse = s;
                    Webbrowser.Navigate(new Uri("http://isy.gymoberwil.com/navigation/dispatcher.php?n=3&m=85&p=272&f=1000100&fp1=" + s));
                    
                    Einrichten3.Opacity = 0;
                    Einrichten3.IsHitTestVisible = true;
                    Einrichten4.Opacity = 1;
                    Einrichten4.IsHitTestVisible = true;
                }
                else if (Aufrufe == 2)
                {
                    Aufrufe++;
                    //stundenplan nun geladen
                    //ExtractHorario(Webbrowser.SaveToString());
                }
            }

            /*
            if (Webbrowser.SaveToString().Contains("Abmeldung"))
            {
                CookieCollection cc = Webbrowser.GetCookies();
                if (cc.Count > 0)
                {
                    Cookie c = cc["PHPSESSID"];
                    authentication = c.ToString();
                    Einrichten2.IsHitTestVisible = false;
                    Einrichten2.Opacity = 0;
                    Einrichten3.IsHitTestVisible = true;
                    Einrichten3.Opacity = 1;
                }
            }
             * */
        }

        

        List<int[]> PositionCell = new List<int[]>();
        List<string> DownloadableHorarioItems = new List<string>();
        private async void Webbrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (Aufrufe == 3)
            {
                Aufrufe++;
                ExtractHorario(Webbrowser.SaveToString());
                if (Klasse.Text.Contains("_"))
                {
                    Webbrowser.Navigate(new Uri("http://isy.gymoberwil.com/navigation/dispatcher.php?n=3&m=85&p=272&f=1000100&fp1=" + Klasse.Text.Trim()));
                }
                else
                {
                    //Schritt überpringen
                    Aufrufe++;
                    Webbrowser.Navigate(new Uri("http://isy.gymoberwil.com/navigation/dispatcher.php?n=3&m=86&p=273&f=1000000"));
                }
            }
            else if (Aufrufe == 4)
            {
                FillHorario(Webbrowser.SaveToString(), true);
                Aufrufe++;
                Webbrowser.Navigate(new Uri("http://isy.gymoberwil.com/navigation/dispatcher.php?n=3&m=86&p=273&f=1000000"));
                
            }
            else if (Aufrufe == 5)
            {
                Aufrufe++;
                string s = Webbrowser.SaveToString();
                if (!s.Contains("Server Fehler"))
                {
                    ExtractLehrpersonen(s);
                }
                Webbrowser_LoadCompleted(this, e);
                /*Einrichten4.Opacity = 0;
                Einrichten4.IsHitTestVisible = false;
                Einrichten6.Opacity = 1;
                Einrichten6.IsHitTestVisible = true;
                */

                //begin with downloading the single cells
                /*
                if (DownloadableHorarioItems.Count > 0)
                {
                    Webbrowser_LoadCompleted(this, e);
                }
                else
                {
                    Webbrowser_LoadCompleted(this, e);
                    //ready
                }
                 */ 
            }
            else if (Aufrufe > 5)
            {
                string s = Webbrowser.SaveToString();
                if (!s.Contains("Server Fehler"))
                {
                    //FillSingeCell(Webbrowser.SaveToString(), PositionCell[Aufrufe - 4]);
                }
                else
                {

                }
                
                Einrichten4.Opacity = 0;
                Einrichten4.IsHitTestVisible = false;
                
                Einrichten5.Opacity = 1;
                Einrichten5.IsHitTestVisible = true;
                MakeObjects();
                

                /*
                if (Aufrufe - 5 < DownloadableHorarioItems.Count)
                {
                    Webbrowser.Navigate(new Uri(DownloadableHorarioItems[Aufrufe - 4]));
                }
                else
                {
                    Stunden.ToString();
                    MessageBox.Show("Download erfolgreich abgeschlossen!");
                }
                 * */
            }
        }



        public void ExtractHorario(string html)
        {
            for (int i = 0; i < 5; i++)
            {
                Stundeen.Add(new List<StundeWorkingObject>());
            }
            for (int a = 0; a < Stundeen.Count; a++)
            {
                for (int b = 0; b < 12; b++)
                {
                    Stundeen[a].Add(new StundeWorkingObject() { Tag = a, Zeit = b });
                }
            }
            FillHorario(html, false);
        }

        public void FillHorario(string html, bool isSchwerpunkfach)
        {
            html = html.Replace("<table class=\"timetable_table\">", "<table id=\"timetable_table\">");
            html = HTML.ExtractID(html, "timetable_table");
            html = HTML.RemoveComments(html);
            html = HTML.RemoveOuterNodes(html);
            html = HTML.RemoveOuterNodes(html);
            List<string> Rows = HTML.Split(html);
            for (int a = 3; a < Rows.Count; a++)
            {
                string column = HTML.RemoveOuterNodes(Rows[a]);
                column = column.Substring(column.IndexOf("</th>") + ("</th>").Count());
                List<string> Columns = HTML.Split(column);
                for (int b = 0; b < Columns.Count - 1; b++)
                {
                    string content = Columns[b].Substring(Columns[b].IndexOf(">") + 1);
                    content = content.Substring(0, content.LastIndexOf("<"));
                    if (content.Contains("?"))
                    {
                        string g = Klasse.Text;
                        if (Klasse.Text.Contains("_"))
                        {
                            g = Klasse.Text.Substring(0, Klasse.Text.IndexOf("_"));
                        }
                        DownloadableHorarioItems.Add("http://isy.gymoberwil.ch/navigation/dispatcher.php?n=0&m=85&p=272&f=1000200&code=timetable%200:%20HOUR=" + (a - 2).ToString() + ";%20DAY=" + (b + 1).ToString() + ";%20CLASS=" + g + ";%20%20dim0:%20content%20:%20SUBJECT;%20TEACHER;%20ROOM;%20resolution%20:%205;");
                        PositionCell.Add(new int[] { b + 1, a - 2 });
                        Stundeen[b][a - 3].hasQuestion = true;
                    }
                    else
                    {
                        if (content == " ")
                        {
                            if (Stundeen[b][a - 3].isReady)
                            {

                            }
                            else
                            {
                                Stundeen[b][a - 3].isFree = true;
                                Stundeen[b][a - 3].isReady = true;
                            }
                        }
                        else
                        {
                            string[] inhalt = content.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            if (inhalt.Count() == 3)
                            {
                                if (Stundeen[b][a - 3].isReady && Stundeen[b][a - 3].isFree || !Stundeen[b][a - 3].isReady)
                                {
                                    Stundeen[b][a - 3].fachshort = inhalt[0].Trim();
                                    Stundeen[b][a - 3].lehrpersonshort = inhalt[1].Trim();
                                    Stundeen[b][a - 3].zimmernummer = inhalt[2].Trim();
                                    Stundeen[b][a - 3].isSchwerpunktfach = isSchwerpunkfach;
                                    Stundeen[b][a - 3].isFree = false;
                                    Stundeen[b][a - 3].isReady = true;
                                }
                                else
                                {
                                    StundeWorkingObject ob = new StundeWorkingObject() { fachshort = inhalt[0], lehrpersonshort = inhalt[1], zimmernummer = inhalt[2], isSchwerpunktfach = isSchwerpunkfach };
                                    Stundeen[b][a - 3].AdditionalHours.Add(ob);
                                }
                            }
                        }
                    }
                }
            }
        }

        //currently not in use, deautheticat problem
        public void FillSingeCell(string html, int[] position)
        {
            html = html.Replace("<table class=\"timetable_table\">", "<table id=\"timetable_table\">");
            html = HTML.ExtractID(html, "timetable_table");
            html = HTML.RemoveComments(html);
            html = HTML.RemoveOuterNodes(html);
            html = HTML.RemoveOuterNodes(html);
            List<string> Rows = HTML.Split(html);
            for (int a = 4; a < Rows.Count; a++)
            {
                string column = HTML.RemoveOuterNodes(Rows[a]);
                column = column.Substring(column.IndexOf("</th>") + ("</th>").Count());
                List<string> Columns = HTML.Split(column);

                StundeWorkingObject ob = new StundeWorkingObject();

                string content = Columns[0].Substring(Columns[0].IndexOf(">") + 1);
                content = content.Substring(0, content.LastIndexOf("<"));

                ob.fachshort = content;

                content = Columns[1].Substring(Columns[1].IndexOf(">") + 1);
                content = content.Substring(0, content.LastIndexOf("<"));

                ob.lehrpersonshort = content;

                content = Columns[2].Substring(Columns[2].IndexOf(">") + 1);
                content = content.Substring(0, content.LastIndexOf("<"));

                ob.zimmernummer = content;

                ob.isReady = true;

                ob.isFree = false;

                if (Stundeen[position[0]][position[1]].isReady)
                {
                    Stundeen[position[0]][position[1]].AdditionalHours.Add(ob);
                }
                else
                {
                    Stundeen[position[0]][position[1]] = ob;
                }
            }
        }

        public void ExtractLehrpersonen(string html)
        {
            Variables.Lehrkräfte = new List<Lehrkraft>();
            html = html.Replace("<div class=\"nav_contentbox\">", "<div id=\"nav_contentboxxx\">");
            html = HTML.ExtractID(html, "nav_contentboxxx");
            html = html.Substring(html.IndexOf("<br>")+"<br>".Count()).Trim();
            html = HTML.RemoveComments(html);
            html = HTML.RemoveOuterNodes(html);
            html = HTML.RemoveOuterNodes(html);
            html = html.Substring(0, html.LastIndexOf("</"));
            List<string> Rows = HTML.Split(html);
            for (int a = 1; a < Rows.Count; a++)
            {
                Lehrkraft lk = new Lehrkraft();
                string column = HTML.RemoveOuterNodes(Rows[a]);
                List<string> Columns = HTML.Split(column);

                string content = Columns[1].Substring(Columns[1].IndexOf(">") + 1);
                content = content.Substring(0, content.LastIndexOf("<"));
                lk.Nachname = content.Trim();

                content = Columns[2].Substring(Columns[2].IndexOf(">") + 1);
                content = content.Substring(0, content.LastIndexOf("<"));
                lk.Vorname = content.Trim();

                content = Columns[3].Substring(Columns[3].IndexOf(">") + 1);
                content = content.Substring(0, content.LastIndexOf("<"));
                lk.Kürzel = content.Trim();
                Variables.Lehrkräfte.Add(lk);
            }
        }

        public void MakeObjects()
        {
            Variables.Stunden = new List<List<Stunde>>();
            List<string> shortLektions = new List<string>() { "BG", "B", "C", "D", "E", "F", "GG", "G", "I", "L", "AM", "M", "MS", "P", "SK", "SP", "W", "WR" };
            List<int> index = new List<int>() { 1,2,3,4,5,6,7,8,11,12,14,15,16,19,23,23,24,24 };

            for (int a = 0; a < 5; a++)
            {
                Variables.Stunden.Add(new List<Stunde>());
                foreach (var item in Stundeen[a])
                {
                    Stunde st = new Stunde();
                    if (item.isFree)
                    {
                        st.SetLektion(Lektion.Frei,null);
                        st.Tag = (Wochentag)item.Tag;
                        st.zeit = item.Zeit;
                    }
                    else
                    {
                        if (shortLektions.Contains(item.fachshort))
                        {
                            int v = 0;
                            for (v = 0; v < shortLektions.Count; v++)
                            {
                                if (shortLektions[v] == item.fachshort)
                                {
                                    st.SetLektion((Lektion)index[v], null);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            st.SetLektion(Lektion.Anderes, item.fachshort);
                        }
                        st.Fach.Schwerpunktfach = item.isSchwerpunktfach;
                        st.Fach.SetLehrer(item.lehrpersonshort,null);
                        st.Tag = (Wochentag)item.Tag;
                        st.zeit = item.Zeit;
                        st.zimmernummer = Convert.ToInt16(item.zimmernummer);
                    }
                    Variables.Stunden[Variables.Stunden.Count - 1].Add(st);
                }
            }


            List<int> allfächer = new List<int>();
            foreach (var items in Variables.Stunden)
            {
                foreach (var item in items)
                {
                    allfächer.Add(item.fach);
                }
            }
            List<int> noduplicates = allfächer.Distinct().ToList();
            noduplicates.Remove(0);

            for (int i = 0; i < noduplicates.Count; i++)
            {
                Variables.Fächer[noduplicates[i]].isActive = true;
                Variables.Fächer[noduplicates[i]].index = i;
                Variables.Fächer[noduplicates[i]].color = i+1;
            }

            Einrichten5.Opacity = 0;
            Einrichten5.IsHitTestVisible = false;
            Einrichten6.Opacity = 1;
            Einrichten6.IsHitTestVisible = true;
            Save.SaveAll();
        }

    }
}