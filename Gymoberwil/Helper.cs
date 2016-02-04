using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Windows.Storage;

namespace Gymoberwil
{


    class Helper
    {

        public static async Task<string> DownloadStringAsync(Uri url)
        {
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    var bytes = await client.GetByteArrayAsync(url);
                    return System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(bytes,0,bytes.Count());
                }
            }
            catch (Exception e) { return null; }
        }

        public static string NächterBus()
        {
            if (Settings.bus == "bus64")
            {
                if (Settings.busrichtung == "oberwil")
                {
                    Bus b = FirstBus(Variables.Bus64Oberwil);
                    return b.Difference;
                }
                else
                {
                    Bus b = FirstBus(Variables.Bus64Allschwil);
                    return b.Difference;
                }
            }
            else
            {
                if (Settings.busrichtung == "oberwil")
                {
                    Bus b = FirstBus(Variables.Bus61Oberwil);
                    return b.Difference;
                }
                else
                {
                    Bus b = FirstBus(Variables.Bus61Allschwil);
                    return b.Difference;
                }
            }
        }

        public static string NotenNotification()
        {
            double Pluspunkte = 0;
            foreach (var fach in Variables.Fächer)
            {
                double c = fach.PlusPunkte;
                if (c < 0)
                {
                    c = c*2;
                }
                Pluspunkte += c;
            }
            if (Pluspunkte == 0)
            {
                return "Keine Pluspunkte";
            }
            else if (Pluspunkte < 0)
            {
                return Pluspunkte.ToString() + " Minuspunkte";
            }
            else
            {
                return Pluspunkte.ToString() + " Pluspunkte";
            }
        }

        public static string StundenplanNotification()
        {
            List<Stunde> list = Helper.GetNextHours();
            object ob = Helper.GetNow();
            if (ob == null)
            {
                return "keine Einträge im Stundenplan";
            }
            else if (ob.GetType() == typeof(Frei))
            {
                return "Du hast Frei zurzeit";
            }
            else if (ob.GetType() == typeof(Stunde))
            {
                Stunde stunde = ob as Stunde;
                return "Du hast " + stunde.FachTeil12 + " bis am " + stunde.EndTime;
            }
            else if (ob.GetType() == typeof(Pause))
            {
                Pause pause = ob as Pause;
                return pause.EndTime + " im Zimmer " + list[0].Zimmernummer;
            }
            else
            {
                return "";
            }
        }

        public static Bus FirstBus(List<Bus> list)
        {
            List<Bus> b = new List<Bus>();

            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                foreach (var item in list)
                {
                    if (item.Tag == Tag.Samstag)
                    {
                        b.Add(item);
                    }
                }
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                foreach (var item in list)
                {
                    if (item.Tag == Tag.Sonntag)
                    {
                        b.Add(item);
                    }
                }
            }
            else
            {
                foreach (var item in list)
                {
                    if (item.Tag == Tag.Wochentag)
                    {
                        b.Add(item);
                    }
                }
            }

            for (int i = 0; i < b.Count; i++)
            {
                int hour = b[i].Stunde - DateTime.Now.Hour;
                int minutes = b[i].Minute - DateTime.Now.Minute;
                minutes = minutes + hour * 60;
                if (minutes > 0 && minutes < Settings.maxbusminutes) { }
                else
                {
                    b.RemoveAt(i);
                    i--;
                }

            }
            if (b.Count == 0)
            {
                b.Add(new Bus(true));
            }

            for (int i = 1; i < b.Count; i++)
            {
                if (b[i].TimeSpanValue < b[i - 1].TimeSpanValue)
                {
                    Bus tmp = b[i];
                    b[i] = b[i - 1];
                    b[i - 1] = tmp;
                    i = 0;
                }
            }

            return b[0];
        }

        public static List<Bus> Bus(Mode mode, List<Bus> list1, List<Bus> list2)
        {
            List<Bus> b = new List<Bus>();

            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                foreach (var item in list1)
                {
                    if (item.Tag == Tag.Samstag)
                    {
                        b.Add(item);
                    }
                }
                foreach (var item in list2)
                {
                    if (item.Tag == Tag.Samstag)
                    {
                        b.Add(item);
                    }
                }
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                foreach (var item in list1)
                {
                    if (item.Tag == Tag.Sonntag)
                    {
                        b.Add(item);
                    }
                }
                foreach (var item in list2)
                {
                    if (item.Tag == Tag.Sonntag)
                    {
                        b.Add(item);
                    }
                }
            }
            else
            {
                foreach (var item in list1)
                {
                    if (item.Tag == Tag.Wochentag)
                    {
                        b.Add(item);
                    }
                }
                foreach (var item in list2)
                {
                    if (item.Tag == Tag.Wochentag)
                    {
                        b.Add(item);
                    }
                }
            }
            if (mode == Mode.Selected)
            {
                for (int i = 0; i < b.Count; i++)
                {
                    int hour = b[i].Stunde - DateTime.Now.Hour;
                    int minutes = b[i].Minute - DateTime.Now.Minute;
                    minutes = minutes + hour * 60;
                    if (minutes > -10 && minutes < Settings.maxbusminutes) { }
                    else
                    {
                        b.RemoveAt(i);
                        i--;
                    }

                }
                if (b.Count == 0)
                {
                    b.Add(new Bus(true));
                }
            }
            for (int i = 1; i < b.Count; i++)
            {
                if (b[i].TimeSpanValue < b[i - 1].TimeSpanValue)
                {
                    Bus tmp = b[i];
                    b[i] = b[i - 1];
                    b[i - 1] = tmp;
                    i = 0;
                }
            }

            return b;
        }

        public static List<Colorobject> MakeColors()
        {
            List<Colorobject> colorobjects = new List<Colorobject>();
            colorobjects.Add(new Colorobject("Lime", "#A4C400",0));
            colorobjects.Add(new Colorobject("Green", "#60A917",1));
            colorobjects.Add(new Colorobject("Emerald", "#008A00",2));
            colorobjects.Add(new Colorobject("Teal", "#00ABA9",3));
            colorobjects.Add(new Colorobject("Cyan", "#1BA1E2",4));
            colorobjects.Add(new Colorobject("Cobalt", "#0050EF",5));
            colorobjects.Add(new Colorobject("Indigo", "#6A00FF",6));
            colorobjects.Add(new Colorobject("Violet", "#AA00FF",7));
            colorobjects.Add(new Colorobject("Pink", "#F472D0",8));
            colorobjects.Add(new Colorobject("Magenta", "#D80073",9));
            colorobjects.Add(new Colorobject("Crimson", "#A20025",10));
            colorobjects.Add(new Colorobject("Red", "#E51400",11));
            colorobjects.Add(new Colorobject("Orange", "#FA6800",12));
            colorobjects.Add(new Colorobject("Amber", "#F0A30A",13));
            colorobjects.Add(new Colorobject("Yellow", "#E3C800",14));
            colorobjects.Add(new Colorobject("Brown", "#825A2C",15));
            colorobjects.Add(new Colorobject("Olive", "#6D8764",16));
            colorobjects.Add(new Colorobject("Steel", "#647687",17));
            colorobjects.Add(new Colorobject("Mauve", "#76608A",18));
            colorobjects.Add(new Colorobject("Taupe", "#87794E",19));

            return colorobjects;
        }

        public static Woche GetWeek()
        {
            GregorianCalendar cal = new GregorianCalendar(GregorianCalendarTypes.Localized);
            int c = cal.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            if (c % 2 == 0)
            {
                return Woche.B0Woche;
            }
            else
            {
                return Woche.A0Woche;
            }
        }

        public class Colorobject
        {
            public string name;
            public SolidColorBrush couleur;
            public int index;

            public Colorobject(string name, string HEX, int index)
            {
                this.index = index;
                this.name = name;
                SolidColorBrush scb = new SolidColorBrush();
                scb.Color = Color.FromArgb(255,Convert.ToByte(HEX.Substring(1, 2), 16), Convert.ToByte(HEX.Substring(3, 2), 16),Convert.ToByte(HEX.Substring(5, 2), 16));
                this.couleur = scb;
            }

            public string Name
            {
                get
                {
                    return name;
                }
            }
            public SolidColorBrush Couleur
            {
                get
                {
                    return couleur;
                }
            }
        }

        public static List<Fach> MakeFachList()
        {
            List<Fach> fächer = new List<Fach>();
            foreach (var item in Variables.Fächer)
            {
                if (item.isActive)
                {
                    fächer.Add(item);
                }
            }

            for (int i = 1; i < fächer.Count; i++)
            {
                if (fächer[i - 1].index > fächer[i].index)
                {
                    Fach f = fächer[i-1];
                    fächer[i-1] = fächer[i];
                    fächer[i] = f;
                    i = 0;
                }
            }
            for (int i = 0; i < fächer.Count; i++)
            {
                fächer[i].index = i;
            }
            return fächer;
        }

        public static List<Fach> MakeFachListContrary()
        {
            List<Fach> fächer = new List<Fach>();
            foreach (var item in Variables.Fächer)
            {
                if (!item.isActive)
                {
                    fächer.Add(item);
                }
            }
            fächer.RemoveAt(0);
            return fächer;
        }

        public static List<FachSaveObject> ToSave(List<Fach> f)
        {
            List<FachSaveObject> list = new List<FachSaveObject>();
            foreach (var item in f)
            {
                list.Add(new FachSaveObject(item));
            }
            return list;
        }

        public static List<BeamerSaveObject> ToSave(List<Beamer> b)
        {
            List<BeamerSaveObject> list = new List<BeamerSaveObject>();
            foreach (var item in b)
            {
                list.Add(new BeamerSaveObject(item));
            }
            return list;
        }

        public static List<List<StundeSaveObject>> ToSave(List<List<Stunde>> f)
        {
            List<List<StundeSaveObject>> list = new List<List<StundeSaveObject>>();
            foreach (var items in f)
            {
                list.Add(new List<StundeSaveObject>());
                foreach (var item in items)
                {
                    list[list.Count - 1].Add(new StundeSaveObject(item));
                }
            }
            return list;
        }

        public static List<Fach> FromSave(List<FachSaveObject> f)
        {
            List<Fach> list = new List<Fach>();
            foreach (var item in f)
            {
                list.Add(new Fach(item));
            }
            return list;
        }

        public static List<Beamer> FromSave(List<BeamerSaveObject> f)
        {
            List<Beamer> list = new List<Beamer>();
            foreach (var item in f)
            {
                list.Add(new Beamer(item));
            }
            return list;
        }

        public static List<List<Stunde>> FromSave(List<List<StundeSaveObject>> f)
        {
            List<List<Stunde>> list = new List<List<Stunde>>();
            foreach (var items in f)
            {
                list.Add(new List<Stunde>());
                foreach (var item in items)
                {
                    list[list.Count - 1].Add(new Stunde(item));
                }
            }
            return list;
        }

        public static string[] SplitTime(DateTime d)
        {
            string[] str = new string[6];

            string f = d.Hour.ToString();
            if (f.Count() == 1)
            {
                str[0] = "0";
                str[1] = f;
            }
            else
            {
                str[0] = f.Substring(0,1);
                str[1] = f.Substring(1,1);
            }

            f = d.Minute.ToString();
            if (f.Count() == 1)
            {
                str[2] = "0";
                str[3] = f;
            }
            else
            {
                str[2] = f.Substring(0, 1);
                str[3] = f.Substring(1, 1);
            }

            f = d.Second.ToString();
            if (f.Count() == 1)
            {
                str[4] = "0";
                str[5] = f;
            }
            else
            {
                str[4] = f.Substring(0, 1);
                str[5] = f.Substring(1, 1);
            }
            return str;
        }

        public static DateTime Nextlesson;
        public static Stunde Jetzt;
        public static object GetNow()
        {
            DateTime dt = DateTime.Now;
            DateTime now = new DateTime();
            now += TimeSpan.FromHours(dt.Hour);
            now += TimeSpan.FromMinutes(dt.Minute);
            now += TimeSpan.FromSeconds(dt.Second + 1);
            if (active != null)
            {
                if (active.GetType() != typeof(bool))
                {
                    return active;
                }
            }
            if (Nextlesson == DateTime.MinValue)
            {
                return null;
            }
            else if (now < Nextlesson)
            {
                return new Frei() { endtime = Nextlesson };
            }
            else
            {
                return Jetzt;
            }
            //List<DateTime> Zeiten = GetTimeList();
            //for (int i = 1; i < Zeiten.Count; i++)
            //{
            //    if (Zeiten[i - 1] < now && Zeiten[i] > now)
            //    {
            //        if (Zeiten[i] - Zeiten[i - 1] == TimeSpan.FromMinutes(45))
            //        {
            //            return null;
            //        }
            //        else
            //        {
            //            return new Pause() { starttime = Zeiten[i-1], endtime = Zeiten[i]};
            //        }
            //    }
            //}
            //if (now > Zeiten[0])
            //{
            //    return new Pause() { starttime = now, endtime = Zeiten[0] + TimeSpan.FromDays(1) };
            //}
            //else
            //{
            //    //gibts nicht
            //}
            //return null;
        }

        public static List<Stunde> GetNextHours()
        {
            DateTime dt = DateTime.Now;
            DateTime now = new DateTime();
            now += TimeSpan.FromHours(dt.Hour);
            now += TimeSpan.FromMinutes(dt.Minute);
            now += TimeSpan.FromSeconds(dt.Second + 1);
            int wochentag = (int)DateTime.Now.DayOfWeek;
            wochentag--;
            int count = 0;
            if (0 > wochentag || wochentag > 4)
            {
                if (wochentag == -1)
                {
                    //sonntag
                    count = 1;
                }
                else
                {
                    //samstag
                    count = 2;
                }
                wochentag = 0;
                // now == 0
                now = new DateTime();
            }
            List<Stunde> list = NextHours(now, wochentag);
            if (list.Count > 0)
            {
                Nextlesson = list[0].StartDateTime;
                Nextlesson = Nextlesson.AddDays(count);
            }
            else
            {
                //keine Stunden mehr heute
                if (wochentag == 4)
                {
                    list = NextHours(new DateTime(), 0);
                    count = 2;
                }
                else
                {
                    list = NextHours(new DateTime(), ++wochentag);
                }
                if (list.Count == 0)
                {
                    Nextlesson = DateTime.MinValue;
                    return null;
                }
                Nextlesson = list[0].StartDateTime;
                Nextlesson = Nextlesson.AddDays(1);
                Nextlesson = Nextlesson.AddDays(count);
            }

            if (now < Nextlesson)
            {
                return list;
            }
            else
            {
                Jetzt = list[0];
                return list.GetRange(1, list.Count-1);
            }
        }

        public static object active;
        private static List<Stunde> NextHours(DateTime now, int wochentag)
        {
            List<DateTime> Zeiten = GetTimeList();

            List<Stunde> stunden = new List<Stunde>();
            if (now < Zeiten[0])
            {
                stunden = Variables.Stunden[wochentag].GetRange(0, Variables.Stunden[wochentag].Count - 1);
            }
            else if (now > Zeiten[Zeiten.Count-1])
            {
                return stunden;
            }
            else
            {
                for (int i = 1; i < Zeiten.Count; i++)
                {
                    if (Zeiten[i - 1] < now && Zeiten[i] > now)
                    {
                        int c = Convert.ToInt32(i / 2);
                        if (Variables.Stunden[wochentag].Count - c - 1 > 0)
                        {
                            stunden = Variables.Stunden[wochentag].GetRange(c, Variables.Stunden[wochentag].Count - c - 1);
                        }
                        if (Zeiten[i] - Zeiten[i - 1] != TimeSpan.FromMinutes(45))
                        {
                            active = new Pause() { starttime = Zeiten[i - 1], endtime = Zeiten[i] };
                        }
                        break;
                    }
                }
            }
            //frei am nachmittag abschneiden
            for (int a = stunden.Count - 1; a > -1; a--)
            {
                if (stunden[a].FachTeil12 == "Frei")
                {
                    stunden.RemoveAt(a);
                }
                else
                {
                    break;
                }
            }
            //frei vor der zeit abschneiden
            for (int a = 0; a < stunden.Count - 1; a++)
            {
                if (stunden[a].FachTeil12 == "Frei")
                {
                    active = false;
                    stunden.RemoveAt(a);
                    a--;
                }
                else
                {
                    break;
                }
            }

            return stunden;
        }

        public static DateTime[] ConvertIndexToTime(int index)
        {
            List<DateTime> Zeiten = GetTimeList();
            int a = index * 2;
            return new DateTime[] { Zeiten[a], Zeiten[a + 1] };
        }

        public static List<DateTime> GetTimeList()
        {
            if (Variables.Zeiten.Count() != 0)
            {
                return Variables.Zeiten;
            }
            else
            {
                List<int> AnfangsStunde = new List<int>() { 7, 8, 9, 10, 11, 12, 13, 14, 14, 15, 16, 17 };
                List<int> AnfangsMinute = new List<int>() { 50, 45, 40, 40, 30, 20, 10, 00, 55, 45, 40, 25 };

                Variables.Zeiten = new List<DateTime>();
                for (int i = 0; i < AnfangsStunde.Count; i++)
                {
                    DateTime anfangszeit = new DateTime();
                    anfangszeit += TimeSpan.FromHours(AnfangsStunde[i]);
                    anfangszeit += TimeSpan.FromMinutes(AnfangsMinute[i]);
                    Variables.Zeiten.Add(anfangszeit);
                    Variables.Zeiten.Add(anfangszeit + TimeSpan.FromMinutes(45));
                }
                return Variables.Zeiten;
            }
        }
    }

    class HTML
    {
        public static string RemoveOuterNodes(string s)
        {
            s = s.Substring(s.IndexOf(">"));
            s = s.Substring(s.IndexOf("<"));
            s = s.Substring(0, s.LastIndexOf("<"));
            s = s.Substring(0, s.LastIndexOf(">") + 1);
            return s;
        }

        public static List<Beamer> GetBeamerEntries(string html)
        {
            List<Beamer> beamers = new List<Beamer>();
            List<string> l = new List<string>();
            l = html.Split(new string[] {"<tr>"},StringSplitOptions.RemoveEmptyEntries).ToList();

            for (int i = 0; i < l.Count; i++)
            {
                List<string> Line = new List<string>();
                int index = 0;
                l[i] = l[i].Trim();
                while (true)
                {
                    string beginn = l[i].Substring(l[i].IndexOf("\">") + ("\">").Count());
                    if (beginn.Substring(1).Contains("<td"))
                    {
                        l[i] = beginn.Substring(beginn.Substring(1).IndexOf("<td"));
                        beginn = beginn.Substring(0, beginn.IndexOf("<td"));
                        Line.Add(beginn.Trim());
                    }
                    else
                    {
                        Line.Add(beginn.Trim());
                        break;
                    }
                }
                string time;
                if (Line[1] != "&nbsp;" && Line[1].Count() > 2)
                {
                    time = "Von " + Line[0] + " bis " +Line[1];
                }
                else
                {
                    time = Line[0] + " " + Line[2] + " - " + Line[3];
                }
               
                Beamer b = new Beamer { time = time, content = Line[5], title = Line[4], important = false };
                beamers.Add(b);
            }

            return beamers;
        }

        public static List<string> Split(string html)
        {
            List<string> SplitItems = new List<string>();
            string tag = html.Substring(1, html.IndexOf(" ") - 1);
            string gegentag = "</" + tag + ">";
            tag = "<" + tag;
            int oldindex = 0;
            int counter = 0;
            int index = 0;
            while (true)
            {
                if (html.Substring(index).IndexOf(tag) < html.Substring(index).IndexOf(gegentag))
                {
                    if (html.Substring(index).IndexOf(tag) == -1)
                    {
                        counter--;
                        index = html.Substring(index).IndexOf(gegentag) + 1 + index;
                        if (counter == 0)
                        {
                            SplitItems.Add(html.Substring(oldindex).Trim());
                            break;
                        }
                    }
                    else
                    {
                        counter++;
                        index = html.Substring(index).IndexOf(tag) + 1 + index;
                    }
                }
                else
                {
                    if (html.Substring(index).IndexOf(gegentag) == -1)
                    {
                        return null;
                    }
                    counter--;
                    index = html.Substring(index).IndexOf(gegentag) + 1 + index;
                    if (counter == 0)
                    {
                        SplitItems.Add(html.Substring(oldindex, (index - 1) + gegentag.Count() - oldindex).Trim());
                        oldindex = (index - 1) + gegentag.Count();
                    }
                }
            }
            return SplitItems;
        }

        public static List<string> SplitText(string html)
        {
            List<string> l = new List<string>();
            int index = 0;
            index = html.IndexOf("<p>") + 3;
            while (true)
            {
                string s = html.Substring(index);
                l.Add(s.Substring(0, s.IndexOf("</p>")));
                if (s.IndexOf("<p>") == -1)
                {
                    break;
                }
                else
                {
                    index = s.IndexOf("<p>") + index + 3;
                }
            }
            return l;
        }

        public static string ExtractID(string html, string id)
        {
            if (!html.Contains(id)) { return null; }
            int c = html.IndexOf("id=\"" + id + "\"");
            html = html.Substring(html.LastIndexOf("<", c, c - 1));
            string tag = html.Substring(1, html.IndexOf(" ") - 1);
            string gegentag = "</" + tag + ">";
            tag = "<" + tag;
            int counter = 0;
            int index = 0;
            while (true)
            {
                if (html.Substring(index).IndexOf(tag) < html.Substring(index).IndexOf(gegentag))
                {
                    if (html.Substring(index).IndexOf(tag) == -1)
                    {
                        counter--;
                        index = html.Substring(index).IndexOf(gegentag) + 1 + index;
                        if (counter == 0)
                        {
                            html = html.Substring(0, (index - 1) + gegentag.Count());
                            break;
                        }
                    }
                    else
                    {
                        counter++;
                        index = html.Substring(index).IndexOf(tag) + 1 + index;
                    }

                }
                else
                {
                    if (html.Substring(index).IndexOf(gegentag) == -1)
                    {
                        return null;
                    }
                    counter--;
                    index = html.Substring(index).IndexOf(gegentag) + 1 + index;
                    if (counter == 0)
                    {
                        html = html.Substring(0, (index - 1) + gegentag.Count());
                        break;
                    }
                }
            }
            return html;
        }

        public static string RemoveComments(string html)
        {
            string output = string.Empty;
            string[] temp = System.Text.RegularExpressions.Regex.Split(html, "<!--");
            foreach (string s in temp)
            {
                string str = string.Empty;
                if (!s.Contains("-->"))
                {
                    str = s;
                }
                else
                {
                    str = s.Substring(s.IndexOf("-->") + 3);
                }
                if (str.Trim() != string.Empty)
                {
                    output = output + str.Trim();
                }
            }
            return output;
        }
    }


    class Variables
    {
        public static List<Bus> Bus64Allschwil = new List<Bus>();
        public static List<Bus> Bus64Oberwil = new List<Bus>();
        public static List<Bus> Bus61Allschwil = new List<Bus>();
        public static List<Bus> Bus61Oberwil = new List<Bus>();
        public static List<Beamer> BeamerItems = new List<Beamer>();
        public static DateTime BeamerActualized = DateTime.Now;

        public static List<Fach> Fächer = new List<Fach>();

        

        public static List<List<Stunde>> Stunden = new List<List<Stunde>>();

        //Laufzeitvariablen
        public static Stunde activeStunde;
        public static Note activeNote;
        public static Fach activeFach;
        public static bool reactivated = false;

        //Dictionaris
        public static List<SolidColorBrush> Farben = new List<SolidColorBrush>();
        public static List<Lehrkraft> Lehrkräfte = new List<Lehrkraft>();
        public static List<DateTime> Zeiten = new List<DateTime>();

    }

    class Settings
    {
        public static int maxbusminutes = 120;
        public static string bus = "bus64";
        public static string klasse = "";
        public static string busrichtung = "allschwil";
        public static int boughtbeers = 0;
    }

    class Save
    {
        public static bool LoadAll()
        {
            LoadSettings();
            LoadBeamer();
            LoadBus();
            LoadColors();
            LoadFach();
            LoadHorario();
            LoadLehrer();



            //firsttime control
            IsolatedStorageFile myFile = IsolatedStorageFile.GetUserStoreForApplication();
            if (!myFile.FileExists("firsttime"))
            {
                
                StreamWriter sw = new StreamWriter(new IsolatedStorageFileStream("firsttime", FileMode.Create, myFile));
                sw.Write("firsttime");
                sw.Close();
                if (myFile.FileExists("version1.0"))
                {
                    myFile.DeleteFile("version1.0");
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public static void SaveAll()
        {
            SaveSettings();
            SaveBeamer();
            SaveColors();
            SaveFach();
            SaveHorario();
            SaveLehrer();
        }

        public static void LoadSettings()
        {
            IsolatedStorageFile myFile = IsolatedStorageFile.GetUserStoreForApplication();
            string sFile = "settings";
            //myFile.DeleteFile(sFile);
            if (!myFile.FileExists(sFile))
            {
                return;
            }
            else
            {
                try
                {
                    StreamReader reader = new StreamReader(new IsolatedStorageFileStream(sFile, FileMode.Open, myFile));
                    string rawData = reader.ReadToEnd();
                    reader.Close();
                    if (rawData == "")
                    {
                        return;
                    }
                    string[] split = new string[] { "<Lvl1>" };
                    string[] level1 = rawData.Split(split, StringSplitOptions.RemoveEmptyEntries);

                    Settings.bus = level1[0];
                    Settings.busrichtung = level1[1];
                    Settings.maxbusminutes = Convert.ToInt16(level1[2]);
                    if (level1.Count() > 3)
                    {
                        Settings.boughtbeers = Convert.ToInt16(level1[3]);
                        if (level1.Count() > 4)
                        {
                            Settings.klasse = level1[4];
                        }
                        else
                        {
                            Settings.klasse = "";
                        }
                    }
                    else
                    {
                        Settings.boughtbeers = 0;
                    }

                    Variables.BeamerItems = JsonConvert.DeserializeObject<List<Beamer>>(level1[1]);
                }
                catch (Exception e) { e.ToString(); }
            }
        }

        public static void SaveSettings()
        {
            string savestring = Settings.bus + "<Lvl1>" + Settings.busrichtung + "<Lvl1>" + Settings.maxbusminutes.ToString() + "<Lvl1>" + Settings.boughtbeers.ToString() + "<Lvl1>" + Settings.klasse;
            IsolatedStorageFile myFile = IsolatedStorageFile.GetUserStoreForApplication();
            string sFile = "settings";
            StreamWriter sw = new StreamWriter(new IsolatedStorageFileStream(sFile, FileMode.Create, myFile));
            sw.Write(savestring);
            sw.Close();
        }
        

        public static void LoadBeamer()
        {
            IsolatedStorageFile myFile = IsolatedStorageFile.GetUserStoreForApplication();
            string sFile = "beamer";
            //myFile.DeleteFile(sFile);
            if (!myFile.FileExists(sFile))
            {
                return;
            }
            else
            {
                try
                {
                    StreamReader reader = new StreamReader(new IsolatedStorageFileStream(sFile, FileMode.Open, myFile));
                    string rawData = reader.ReadToEnd();
                    reader.Close();
                    if (rawData == "")
                    {
                        return;
                    }
                    string[] split = new string[] { "<Lvl1>" };
                    string[] level1 = rawData.Split(split, StringSplitOptions.RemoveEmptyEntries);

                    Variables.BeamerActualized = Convert.ToDateTime(level1[0]);

                    Variables.BeamerItems = Helper.FromSave(JsonConvert.DeserializeObject<List<BeamerSaveObject>>(level1[1]));
                }
                catch (Exception e) { e.ToString(); }
            }
        }

        public static void SaveBeamer()
        {

            string savestring = Variables.BeamerActualized + "<Lvl1>"+ JsonConvert.SerializeObject(Helper.ToSave(Variables.BeamerItems));
            IsolatedStorageFile myFile = IsolatedStorageFile.GetUserStoreForApplication();
            string sFile = "beamer";
            StreamWriter sw = new StreamWriter(new IsolatedStorageFileStream(sFile, FileMode.Create, myFile));
            sw.Write(savestring);
            sw.Close();
        }

        public static void LoadHorario()
        {
            IsolatedStorageFile myFile = IsolatedStorageFile.GetUserStoreForApplication();
            string sFile = "horario";

            if (!myFile.FileExists(sFile))
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
                return;
            }
            else
            {
                try
                {
                    StreamReader reader = new StreamReader(new IsolatedStorageFileStream(sFile, FileMode.Open, myFile));
                    string rawData = reader.ReadToEnd();
                    reader.Close();
                    if (rawData == "")
                    {
                        return;
                    }

                    Variables.Stunden = Helper.FromSave(JsonConvert.DeserializeObject<List<List<StundeSaveObject>>>(rawData));
                }
                catch (Exception e) { e.ToString(); }
            }
        }

        public static void SaveHorario()
        {
            string savestring = JsonConvert.SerializeObject(Helper.ToSave(Variables.Stunden));
            IsolatedStorageFile myFile = IsolatedStorageFile.GetUserStoreForApplication();
            string sFile = "horario";
            StreamWriter sw = new StreamWriter(new IsolatedStorageFileStream(sFile, FileMode.Create, myFile));
            sw.Write(savestring);
            sw.Close();
        }

        public static void LoadFach()
        {
            IsolatedStorageFile myFile = IsolatedStorageFile.GetUserStoreForApplication();
            string sFile = "fach";

            if (!myFile.FileExists(sFile))
            {
                List<Lektion> list = Enum.GetValues(typeof(Lektion)).Cast<Lektion>().ToList();
                Variables.Fächer = new List<Fach>();
                foreach (var item in list)
                {
                    Variables.Fächer.Add(new Fach() { lektion = item });
                }
                return;
            }
            else
            {
                try
                {
                    StreamReader reader = new StreamReader(new IsolatedStorageFileStream(sFile, FileMode.Open, myFile));
                    string rawData = reader.ReadToEnd();
                    reader.Close();
                    if (rawData == "")
                    {
                        return;
                    }

                    Variables.Fächer = Helper.FromSave(JsonConvert.DeserializeObject<List<FachSaveObject>>(rawData));
                    return;
                }
                catch (Exception e) { e.ToString(); }
            }
        }

        public static void SaveFach()
        {
            string savestring = JsonConvert.SerializeObject(Helper.ToSave(Variables.Fächer));
            IsolatedStorageFile myFile = IsolatedStorageFile.GetUserStoreForApplication();
            string sFile = "fach";
            StreamWriter sw = new StreamWriter(new IsolatedStorageFileStream(sFile, FileMode.Create, myFile));
            sw.Write(savestring);
            sw.Close();
        }

        public static void SaveLehrer()
        {
            string savestring = JsonConvert.SerializeObject(Variables.Lehrkräfte);
            IsolatedStorageFile myFile = IsolatedStorageFile.GetUserStoreForApplication();
            string sFile = "lehrer";
            StreamWriter sw = new StreamWriter(new IsolatedStorageFileStream(sFile, FileMode.Create, myFile));
            sw.Write(savestring);
            sw.Close();
        }

        public static void LoadLehrer()
        {
            IsolatedStorageFile myFile = IsolatedStorageFile.GetUserStoreForApplication();
            string sFile = "lehrer";
            //myFile.DeleteFile(sFile);
            if (!myFile.FileExists(sFile))
            {
                return;
            }
            else
            {
                try
                {
                    StreamReader reader = new StreamReader(new IsolatedStorageFileStream(sFile, FileMode.Open, myFile));
                    string rawData = reader.ReadToEnd();
                    reader.Close();
                    if (rawData == "")
                    {
                        return;
                    }
                    Variables.Lehrkräfte = JsonConvert.DeserializeObject<List<Lehrkraft>>(rawData);
                }
                catch (Exception e) { e.ToString(); }
            }



        }

        public static void SaveColors()
        {


        }

        public static void LoadColors()
        {
            IsolatedStorageFile myFile = IsolatedStorageFile.GetUserStoreForApplication();
            string sFile = "colors";
            //myFile.DeleteFile(sFile);
            if (!myFile.FileExists(sFile))
            {
                List<Helper.Colorobject> list = Helper.MakeColors();
                Variables.Farben = new List<SolidColorBrush>();
                foreach (var item in list)
                {
                    Variables.Farben.Add(item.couleur);
                }

            }
            else
            {
                try
                {
                    StreamReader reader = new StreamReader(new IsolatedStorageFileStream(sFile, FileMode.Open, myFile));
                    string rawData = reader.ReadToEnd();
                    reader.Close();
                    if (rawData == "")
                    {
                        return;
                    }
                    Variables.BeamerItems = JsonConvert.DeserializeObject<List<Beamer>>(rawData);
                }
                catch (Exception e) { e.ToString(); }
            }
        }

        public static async void LoadBus()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Content/64Allschwiljson.txt"));
            Stream stream = await file.OpenStreamForReadAsync();
            StreamReader s = new StreamReader(stream);
            string text = s.ReadToEnd();
            text = text.Replace("\n", "");
            text = text.Replace("\r", "");
            List<Bus> lists = JsonConvert.DeserializeObject<List<Bus>>(text);

            Variables.Bus64Allschwil = lists;
            file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Content/64Oberwiljson.txt"));
            stream = await file.OpenStreamForReadAsync();
            s = new StreamReader(stream);
            text = s.ReadToEnd();
            text = text.Replace("\n", "");
            text = text.Replace("\r", "");
            lists = JsonConvert.DeserializeObject<List<Bus>>(text);

            Variables.Bus64Oberwil = lists;

            file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Content/61Allschwiljson.txt"));
            stream = await file.OpenStreamForReadAsync();
            s = new StreamReader(stream);
            text = s.ReadToEnd();
            text = text.Replace("\n", "");
            text = text.Replace("\r", "");
            lists = JsonConvert.DeserializeObject<List<Bus>>(text);

            Variables.Bus61Allschwil = lists;

            file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Content/61Oberwiljson.txt"));
            stream = await file.OpenStreamForReadAsync();
            s = new StreamReader(stream);
            text = s.ReadToEnd();
            text = text.Replace("\n", "");
            text = text.Replace("\r", "");
            lists = JsonConvert.DeserializeObject<List<Bus>>(text);

            Variables.Bus61Oberwil = lists;

        }

    }



}
