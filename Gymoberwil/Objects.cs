using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Gymoberwil
{

    class Bus
    {
        public string richtung;
        public int Stunde;
        public int Minute;
        public Tag Tag;
        public bool isDummy;

        public Bus()
        {

        }

        public Bus(bool isDummy)
        {
            Stunde = DateTime.Now.Hour;
            Minute = DateTime.Now.Minute;
            this.isDummy = true;
        }

        public string Zeit
        {
            get
            {
                string stunde = Stunde.ToString();
                string minute = Minute.ToString();
                if (Stunde < 10)
                {
                    stunde = "0" + stunde;
                }
                if (Minute < 10)
                {
                    minute = "0" + minute;
                }
                return stunde + ":" + minute;
            }
        }
        public string Richtung
        {
            get
            {
                if (isDummy)
                {
                    return "Kein Bus";
                }
                return richtung;
            }
            set
            {
                richtung = value;
            }
        }
        public string Difference
        {
            get
            {
                if (isDummy)
                {
                    return "verkehrt zurzeit";
                }
                int hour = Stunde - DateTime.Now.Hour;
                int minutes = Minute - DateTime.Now.Minute;
                minutes = minutes + hour * 60;
                if (minutes < 0)
                {
                    return "abgefahren";
                }
                else if (minutes == 0)
                {
                    return "Jetzt! RENNNNNN";
                }
                else if (minutes == 1)
                {
                    return "in einer Minute";
                }
                else
                {
                    return "fährt in " + minutes.ToString() + " Minuten";
                }
            }
        }

        public TimeSpan TimeSpanValue
        {
            get
            {
                return (TimeSpan.FromHours(Stunde) + TimeSpan.FromMinutes(Minute));

            }
        }
    }

    public class BeamerSaveObject
    {
        public string title, content,time;
        public bool important;

        public BeamerSaveObject() { }

        public BeamerSaveObject(Beamer b)
        {
            this.title = b.title;
            this.content = b.content;
            this.important = b.important;
            this.time = b.time;
        }
    }

    public class Beamer
    {
        public string title, content, time;
        public bool important;

        public Beamer() { }

        public Beamer(BeamerSaveObject b)
        {
            this.title = b.title;
            this.important = b.important;
            this.content = b.content;
            this.time = b.time;
        }

        public string Title
        {
            get
            {
                return title;
            }
        }
        public string Content
        {
            get
            {
                return content;
            }
        }
        public string Time
        {
            get
            {
                return time;
            }
        }

        public SolidColorBrush Color
        {
            get
            {
                if (important)
                {
                    return new SolidColorBrush(Colors.Red);
                }
                else
                {
                    return new SolidColorBrush(Colors.White);
                }
            }
        }
        
    }

    public class StundeWorkingObject
    {
        public string fachshort, lehrpersonshort, zimmernummer;
        public bool isSchwerpunktfach, isFree, isReady, hasQuestion;
        public int Tag, Zeit;
        public List<StundeWorkingObject> AdditionalHours = new List<StundeWorkingObject>();
    }

    public class FachSaveObject
    {
        public int? lehrkraft;
        public string customfach;
        public Lektion lektion;
        public bool Schwerpunktfach, isActive;
        public int color, index;
        public List<Note> noten = new List<Note>();

        public FachSaveObject() { }

        public FachSaveObject(Fach f)
        {
            this.lehrkraft = f.lehrkraft;
            this.customfach = f.customfach;
            this.lektion = f.lektion;
            this.Schwerpunktfach = f.Schwerpunktfach;
            this.color = f.color;
            this.noten = f.noten;
            this.isActive = f.isActive;
            this.index = f.index;
        }
    }

    public class Fach
    {
        public int? lehrkraft;
        public string customfach;
        public Lektion lektion;
        public bool Schwerpunktfach, isActive;
        public int color, index;
        public List<Note> noten = new List<Note>();

        public Fach()
        {

        }
        
        public Fach(FachSaveObject f)
        {
            this.lehrkraft = f.lehrkraft;
            this.customfach = f.customfach;
            this.lektion = f.lektion;
            this.Schwerpunktfach = f.Schwerpunktfach;
            this.color = f.color;
            this.noten = f.noten;
            this.isActive = f.isActive;
            this.index = f.index;
        }

        public SolidColorBrush Color
        {
            get
            {
                return Variables.Farben[color];
            }
        }

        public string Lehrer
        {
            get
            {
                if (lehrkraft.HasValue)
                {
                    return Variables.Lehrkräfte[lehrkraft.Value].Name;
                }
                else
                {
                    return "";
                }
            }
        }

        public void SetLehrer(string Kürzel, string Name)
        {
            if (Name != Lehrer)
            {
                if (lektion != Lektion.Frei)
                {
                    if (Kürzel != null)
                    {
                        if (Variables.Lehrkräfte.Count(g => g.Kürzel == Kürzel) > 0)
                        {
                            int v = 0;
                            for (v = 0; v < Variables.Lehrkräfte.Count; v++)
                            {
                                if (Kürzel == Variables.Lehrkräfte[v].Kürzel)
                                {
                                    lehrkraft = v;
                                    return;
                                }
                            }
                        }
                        Variables.Lehrkräfte.Add(new Lehrkraft() { Kürzel = Kürzel });
                        lehrkraft = Variables.Lehrkräfte.Count - 1;
                    }
                    else if (Name.Trim().Count() > 0)
                    {
                        if (Name.Trim().Count() == 2)
                        {
                            SetLehrer(Name.Trim(), null);
                            return;
                        }
                        if (Variables.Lehrkräfte.Count(g => g.Customname == Name) > 0)
                        {
                            int v = 0;
                            for (v = 0; v < Variables.Lehrkräfte.Count; v++)
                            {
                                if (Name == Variables.Lehrkräfte[v].Customname)
                                {
                                    lehrkraft = v;
                                    return;
                                }
                            }
                        }
                        else
                        {
                            Variables.Lehrkräfte.Add(new Lehrkraft() { Customname = Name });
                            lehrkraft = Variables.Lehrkräfte.Count - 1;
                        }
                    }
                    else
                    {
                        lehrkraft = null;
                    }
                }
            }
        }

        public string Name
        {
            get
            {
                if (lektion == Lektion.Anderes)
                {
                    if (customfach != null)
                    {
                        return customfach;
                    }
                    else
                    {
                        return "Anderes";
                    }
                }
                else
                {
                    string s = lektion.ToString();
                    if (s.Contains("0"))
                    {
                        s = s.Replace("0", " ");
                    }
                    if (s.Contains("1"))
                    {
                        s = s.Replace("1", " ");
                    }
                    return s;
                }
            }
        }

        public string Durchschnitt
        {
            get
            {
                if (noten.Count > 0)
                {
                    double durchschnitt = 0;
                    double durch = 0;
                    foreach (var note in noten)
                    {
                        durchschnitt = durchschnitt + note.zählweise * note.note;
                        durch = durch + note.zählweise;
                    }
                    return Math.Round(durchschnitt / durch, 2).ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        public double PlusPunkte
        {
            get
            {
                if (noten.Count > 0)
                {
                    double durchschnitt = 0;
                    double durch = 0;
                    foreach (var note in noten)
                    {
                        durchschnitt = durchschnitt + note.zählweise * note.note;
                        durch = durch + note.zählweise;
                    }
                    return (Math.Round(durchschnitt / durch, 2)-4);
                }
                else
                {
                    return 0;
                }

            }
        }

        public string Beschreibung
        {
            get
            {
                if (noten.Count > 0)
                {
                    return "Ø aus " + noten.Count.ToString() + " Noten:";
                }
                else
                {
                    return "Keine Noten erfasst";
                }
            }


        }
    }

    public class Note
    {
        public bool isDummy;
        public double note,zählweise;
        public string beschreibung;
        public DateTime datum;

        public Note()
        {

        }

        public Note(bool isDummy)
        {
            this.isDummy = isDummy;
        }

        public string Name
        {
            get
            {
                if (isDummy)
                {
                    return "Keine Noten erfasst";
                }
                else
                {
                    return beschreibung;
                }
            }
        }
        public string Datum
        {
            get
            {
                if (isDummy)
                {
                    return "";
                }
                else
                {
                    try
                    {
                        return datum.ToShortDateString();
                    }
                    catch { "s".ToString(); }
                    return null;
                }
            }
        }
        public string Zählweise
        {
            get
            {
                if (isDummy)
                {
                    return "";
                }
                else
                {
                    string s = zählweise.ToString();
                    if (s.Count() == 1)
                    {
                        return "x" + s + ".0";
                    }
                    return "x" + s;
                }
            }
        }
        public string Not
        {
            get
            {
                if (isDummy)
                {
                    return "";
                }
                else
                {
                    return note.ToString();
                }
            }
        }

    }

    class StundeSaveObject
    {
        public DateTime customanfangszeit, customendzeit;
        public Wochentag Tag;
        public Woche woche;
        public int zeit, zimmernummer;
        public int fach;

        public StundeSaveObject() { }

        public StundeSaveObject(Stunde s)
        {
            this.customanfangszeit = s.customanfangszeit;
            this.customendzeit = s.customendzeit;
            this.Tag = s.Tag;
            this.woche = s.woche;
            this.zeit = s.zeit;
            this.zimmernummer = s.zimmernummer;
            this.fach = s.fach;
        }

    }

    class Stunde
    {
        public DateTime customanfangszeit, customendzeit;
        public Wochentag Tag;
        public Woche woche;
        public int zeit, zimmernummer;
        public int fach;

        public Stunde() { }

        public Stunde(StundeSaveObject s)
        {
            this.customanfangszeit = s.customanfangszeit;
            this.customendzeit = s.customendzeit;
            this.Tag = s.Tag;
            this.woche = s.woche;
            this.zeit = s.zeit;
            this.zimmernummer = s.zimmernummer;
            this.fach = s.fach;
        }

        public void SetFach(Fach f)
        {
            for (int i = 0; i < Variables.Fächer.Count; i++)
            {
                if (f == Variables.Fächer[i])
                {
                    fach = i;
                    return;
                }
            }
        }

        public void SetLektion(Lektion l, string name)
        {
            for (int i = 0; i < Variables.Fächer.Count; i++)
            {
                if (Variables.Fächer[i].lektion == l)
                {
                    if (l == Lektion.Anderes)
                    {
                        if (Variables.Fächer[i].customfach == name)
                        {
                            fach = i;
                            return;
                        }
                    }
                    else
                    {
                        fach = i;
                        return;
                    }
                }
            }
            Variables.Fächer.Insert(Variables.Fächer.Count-1,new Fach() { lektion = l, customfach = name });
            fach = Variables.Fächer.Count - 2;
        }

        public Fach Fach
        {
            get
            {
                return Variables.Fächer[fach];
            }
        }

        public SolidColorBrush Color
        {
            get
            {
                if (woche == Woche.Immer || woche == Helper.GetWeek())
                {
                    return Variables.Fächer[fach].Color;
                }
                else
                {
                    return Variables.Fächer[0].Color;

                }
            }
        }

        public string Lehrer
        {
            get
            {
                if (woche == Woche.Immer || woche == Helper.GetWeek())
                {
                    return Variables.Fächer[fach].Lehrer;
                }
                else
                {
                    return Variables.Fächer[0].Lehrer;
                }
            }
        }

        public Lehrkraft GetLehrer
        {
            get
            {
                if (woche == Woche.Immer || woche == Helper.GetWeek())
                {
                    if (Variables.Fächer[fach].lehrkraft.HasValue)
                    {
                        return Variables.Lehrkräfte[Variables.Fächer[fach].lehrkraft.Value];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public string FachTeil12
        {
            get
            {
                return FachTeil1 + FachTeil2;
            }
        }

        public string FachTeil1
        {
            get
            {
                if (woche == Woche.Immer || woche == Helper.GetWeek())
                {
                    return Variables.Fächer[fach].Name.Substring(0, 1);
                }
                else
                {
                    return "F";
                }
            }
        }

        public string FachTeil2
        {
            get
            {
                if (woche == Woche.Immer || woche == Helper.GetWeek())
                {
                    return Variables.Fächer[fach].Name.Substring(1);
                }
                else
                {
                    return "rei";
                }
            }
        }

        public string Zimmernummer
        {
            get 
            {
                if (Variables.Fächer[fach].lektion != Lektion.Frei)
                {
                    if (woche == Woche.Immer || woche == Helper.GetWeek())
                    {
                        return zimmernummer.ToString();
                    }
                }
                return "";
            }
        }

        //Opacity
        public double Opacity
        {
            get
            {
                if (Variables.Fächer[fach].lektion != Lektion.Frei)
                {
                    if (woche == Woche.Immer || woche == Helper.GetWeek())
                    {
                        return 1;
                    }
                }
                return 0;
            }
        }

        //Opacity
        public double OpacityContrary
        {
            get
            {
                if (Variables.Fächer[fach].lektion != Lektion.Frei)
                {
                    if (woche == Woche.Immer || woche == Helper.GetWeek())
                    {
                        return 0;
                    }
                }
                return 1;
            }
        }

        public string Zeit
        {
            get
            {
                switch (zeit)
                {
                    case 0: 
                        return "07:50 - 08:35";
                    case 1:
                        return "08:45 - 09:30";
                    case 2:
                        return "09:40 - 10:25";
                    case 3:
                        return "10:40 - 11:25";
                    case 4:
                        return "11:30 - 12:15";
                    case 5:
                        return "12:20 - 13:05";
                    case 6:
                        return "13:10 - 13:55";
                    case 7:
                        return "14:00 - 14:45";
                    case 8:
                        return "14:55 - 15:40";
                    case 9:
                        return "15:45 - 16:30";
                    case 10:
                        return "16:40 - 17:25";
                    case 11:
                        return "17:25 - 18:10";
                    default:
                        return customanfangszeit.ToShortTimeString() + " - " + customendzeit.ToShortTimeString();
                }
            }
        }

        public double RelativeTime
        {
            get
            {
                DateTime dt = DateTime.Now;
                DateTime now = new DateTime();
                now += TimeSpan.FromHours(dt.Hour);
                now += TimeSpan.FromMinutes(dt.Minute);

                TimeSpan ts = Helper.ConvertIndexToTime(zeit)[1] - now;
                return ts.TotalMilliseconds / TimeSpan.FromMinutes(45).TotalMilliseconds;
            }
        }

        public string EndTime
        {
            get
            {
                return "bis " + Helper.ConvertIndexToTime(zeit)[1].ToShortTimeString();
            }
        }

        public string StartTime
        {
            get
            {
                return "bis " + Helper.ConvertIndexToTime(zeit)[0].ToShortTimeString();
            }
        }

        public DateTime StartDateTime
        {
            get
            {
                return Helper.ConvertIndexToTime(zeit)[0];
            }
        }

        public string Lengh
        {
            get
            {
                DateTime dt = DateTime.Now;
                DateTime now = new DateTime();
                now += TimeSpan.FromHours(dt.Hour);
                now += TimeSpan.FromMinutes(dt.Minute);

                TimeSpan ts = Helper.ConvertIndexToTime(zeit)[1] - now;
                string s = ts.TotalMinutes.ToString();
                if (s.Count() == 1)
                {
                    return "0" + s;
                }
                return s;
            }
        }
    }

    public class Pause
    {
        public DateTime starttime, endtime;


        public double RelativeTime
        {
            get
            {
                DateTime dt = DateTime.Now;
                DateTime now = new DateTime();
                now += TimeSpan.FromHours(dt.Hour);
                now += TimeSpan.FromMinutes(dt.Minute);

                TimeSpan ts = endtime - now;
                return ts.TotalMilliseconds /  (endtime-starttime).TotalMilliseconds;
            }
        }

        public string EndTime
        {
            get
            {
                return "bis " + endtime.ToShortTimeString();
            }
        }

        public string Lengh
        {
            get
            {
                DateTime dt = DateTime.Now;
                DateTime now = new DateTime();
                now += TimeSpan.FromHours(dt.Hour);
                now += TimeSpan.FromMinutes(dt.Minute);

                TimeSpan ts = endtime - now;
                string s = ts.TotalMinutes.ToString();
                if (s.Count() == 1)
                {
                    return "0" + s;
                }
                return s;
            }
        }

    }

    public class Frei
    {
        public DateTime endtime;
        public string nächstesfach;
        public string info;

        public string EndTime
        {
            get
            {
                return "bis " + endtime.ToShortTimeString();
            }
        }

        public string Lengh
        {
            get
            {
                DateTime dt = DateTime.Now;
                DateTime now = new DateTime();
                now += TimeSpan.FromHours(dt.Hour);
                now += TimeSpan.FromMinutes(dt.Minute);

                TimeSpan ts = endtime - now;
                string s = ts.TotalMinutes.ToString();
                if (s.Count() == 1)
                {
                    return "0" + s;
                }
                return s;
            }
        }

    }

    public class Lehrkraft
    {
        public string Nachname, Vorname, Kürzel, Customname;

        public string Name
        {
            get
            {
                if (Nachname != null)
                {
                    if (Vorname != null)
                    {
                        return Vorname + " " + Nachname;
                    }
                    else
                    {
                        return Nachname;
                    }
                }
                else
                {
                    if (Vorname != null)
                    {
                        return Vorname;
                    }
                    if (Kürzel != null)
                    {
                        return Kürzel;
                    }
                    if (Customname != null)
                    {
                        return Customname;
                    }
                }
                return "";
            }
        }
    }

    public enum Tag
    {
        Wochentag = 0,
        Samstag = 1,
        Sonntag = 2,
    }

    public enum Woche
    {
        Immer = 0,
        A0Woche = 1,
        B0Woche = 2
    }

    public enum Wochentag
    {
        Montag = 0,
        Dienstag = 1,
        Mittwoch = 2,
        Donnerstag = 3,
        Freitag = 4
    }

    public enum Mode
    {
        All = 0,
        Selected = 1,
    }

    public enum Lektion
    {
        Frei = 0,
        Bildnerisches0Gestalten = 1, //BG
        Biologie = 2, //B
        Chemie = 3, //C
        Deutsch = 4, //D
        Englisch = 5, //E
        Französisch = 6, //F
        Geographie = 7, //GG
        Geschichte = 8, //G
        Griechisch = 9,
        Instrument = 10,
        Italienisch = 11, //I
        Latein = 12, //L
        Informatik = 13,
        Angewandte0Mathematik = 14, //AM
        Mathematik = 15, //M
        Musik = 16, //MS
        Pädagogik1Psychologie = 17,
        Philosophie = 18,
        Physik = 19, //P
        Religionswissenschaft = 20,
        Russisch = 21,
        Spanisch = 22,
        Sport = 23, //SK,SM
        Wirtschaft0und0Recht = 24, //W, WR
        Anderes = 25
    }
}
