using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;

namespace Gymoberwil
{
    public partial class Notehinzufügen : PhoneApplicationPage
    {
        public Notehinzufügen()
        {
            InitializeComponent();
            foreach (var item in Variables.Fächer)
            {
                fächerpickerlist.Add(item.Name);
            }
            list = Helper.MakeFachList();
            fächerpicker.ItemsSource = list;
            Datum.Text = DateTime.Now.ToShortDateString();
        }

        public static List<Fach> list;
        public bool replace = false;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.Count > 0)
            {
                if (NavigationContext.QueryString["activenote"] == "true")
                {
                    Fill();
                    Löschen.Opacity = 1;
                    fächerpicker.IsEnabled = false;
                    replace = true;
                    title.Text = "NOTE BEARBEITEN";
                }
                else
                {
                    int c = Convert.ToInt32(NavigationContext.QueryString["active"]);
                    if (c != 0)
                    {
                        fächerpicker.SelectedIndex = --c;
                    }
                }
            }
        }



        public List<string> fächerpickerlist = new List<string>();
        private void fächerpicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Fill()
        {
            Note.Text = Variables.activeNote.note.ToString();
            Datum.Text = Variables.activeNote.Datum;
            Zählweise.Text = Variables.activeNote.zählweise.ToString();
            Beschreibung.Text = Variables.activeNote.beschreibung;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].noten.Contains(Variables.activeNote))
                {
                    fächerpicker.SelectedIndex = i;
                }
            }
        }

        private void TextBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text == "4.25" || tb.Text == "1" || tb.Text == DateTime.Now.ToShortDateString() || tb.Text == "sinus funktionen")
            {
                tb.Text = "";
            }
        }

        private void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Eingabe überprüfen
            bool faillure = false;

            Beschreibung.Background = new SolidColorBrush(Colors.Green);
            DateTime t = new DateTime();
            if (!DateTime.TryParse(Datum.Text, out t))
            {
                faillure = true;
                Datum.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Datum.Background = new SolidColorBrush(Colors.Green);
            }
            try
            {
                Convert.ToDouble(Note.Text);
                Note.Background = new SolidColorBrush(Colors.Green);
            }
            catch
            {
                Note.Background = new SolidColorBrush(Colors.Red);
                faillure = true;
            }
            try
            {
                Convert.ToDouble(Zählweise.Text);
                Zählweise.Background = new SolidColorBrush(Colors.Green);
            }
            catch
            {
                Zählweise.Background = new SolidColorBrush(Colors.Red);
                faillure = true;
            }
            if (faillure)
            {
                return;
            }
            else
            {
                Note n = new Note() { datum = t, note = Convert.ToDouble(Note.Text), zählweise = Convert.ToDouble(Zählweise.Text), beschreibung = Beschreibung.Text };
                if (replace)
                {
                    for (int i = 0; i < Variables.Fächer[fächerpicker.SelectedIndex].noten.Count; i++)
			        {
			            if (Variables.Fächer[fächerpicker.SelectedIndex].noten[i] == Variables.activeNote)
                        {
                            Variables.Fächer[fächerpicker.SelectedIndex].noten[i] = n;
                        }
			        }
                }
                else
                {
                    List<Fach> list = Helper.MakeFachList();
                    list[fächerpicker.SelectedIndex].noten.Add(n);
                }

                NavigationOutTransition navigateOutTransition = new NavigationOutTransition();
                navigateOutTransition.Backward = new TurnstileFeatherTransition { Mode = TurnstileFeatherTransitionMode.BackwardOut };
                navigateOutTransition.Forward = new TurnstileFeatherTransition { Mode = TurnstileFeatherTransitionMode.BackwardOut };
                TransitionService.SetNavigationOutTransition(this, navigateOutTransition);
                NavigationService.Navigate(new Uri("/Noten.xaml?delete=true&pivotitem=" + NavigationContext.QueryString["active"].ToString(), UriKind.Relative));
            }
        }

        private void Delete_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            for (int i = 0; i < Variables.Fächer.Count; i++)
			{
			    if (Variables.Fächer[i].noten.Contains(Variables.activeNote))
                {
                    Variables.Fächer[i].noten.Remove(Variables.activeNote);
                    break;
                }
			}
            NavigationOutTransition navigateOutTransition = new NavigationOutTransition();
            navigateOutTransition.Backward = new TurnstileFeatherTransition { Mode = TurnstileFeatherTransitionMode.BackwardOut };
            navigateOutTransition.Forward = new TurnstileFeatherTransition { Mode = TurnstileFeatherTransitionMode.BackwardOut };
            TransitionService.SetNavigationOutTransition(this, navigateOutTransition);
            NavigationService.Navigate(new Uri("/Noten.xaml?delete=true&pivotitem=" + NavigationContext.QueryString["active"].ToString(), UriKind.Relative));
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            NavigationService.Navigate(new Uri("/Noten.xaml?delete=true&pivotitem=" + NavigationContext.QueryString["active"].ToString(), UriKind.Relative));
        }

    }
}