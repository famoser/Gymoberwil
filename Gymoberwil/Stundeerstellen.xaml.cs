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
    public partial class Stundeerstellen : PhoneApplicationPage
    {
        public Stundeerstellen()
        {
            InitializeComponent();
            Farbe.ItemsSource = Helper.MakeColors();
            List<string> listt = Enum.GetNames(typeof(Woche)).ToList();
            for (int i = 0; i < listt.Count; i++)
            {
                if (listt[i].Contains("0"))
                {
                    listt[i] = listt[i].Replace("0", " ");
                }
            }
            wochenpicker.ItemsSource = listt;
            zeitpicker.ItemsSource = new List<string>() { "07:50 - 08:35", "08:45 - 09:30", "09:40 - 10:25", "10:40 - 11:25", "11:30 - 12:15", "12:20 - 13:05", "13:10 - 13:55", "14:00 - 14:45", "14:55 - 15:40", "15:45 - 16:30", "16:40 - 17:25", "17:25 - 18:10" };
            fachpicker.ItemsSource = Variables.Fächer;
            Fill();
        }

        private void Fill()
        {
            if (Variables.activeStunde != null)
            {
                zeitpicker.SelectedIndex = Variables.activeStunde.zeit;
                int c = Variables.activeStunde.fach;
                fachpicker.SelectedIndex = c;
                Zimmernummer.Text = Variables.activeStunde.Zimmernummer;
                Lehrperson.Text = Variables.Fächer[Variables.activeStunde.fach].Lehrer;
                c = (int)Variables.activeStunde.woche;
                wochenpicker.SelectedIndex = c;
            }
        }

        private int Aufrufe = 0;
        private void zeitpicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ignoriere die ersten drei Aufrufe
            if (Aufrufe < 3)
            {
                Aufrufe++;
                return;
            }
            int activelist;
            for (activelist = 0; activelist < Variables.Stunden.Count; activelist++)
			{
			    if (Variables.Stunden[activelist].Contains(Variables.activeStunde));
                {
                    break;
                }
			}

            Variables.activeStunde = Variables.Stunden[activelist][zeitpicker.SelectedIndex];
            Fill();
            Speichern.IsEnabled = true;
            Speichern.Content = "Speichern";
        }

        private TextBlock fächer;
        private TextBox fächer2;
        private bool fächeradded = false;
        private void fachpicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fachpicker.SelectedIndex == 0)
            {
                //frei
                Zimmernummer.IsEnabled = false;
                Lehrperson.IsEnabled = false;
                Lehrperson.Text = "";
                Fach f = fachpicker.SelectedItem as Fach;
                try
                {
                    Farbe.SelectedIndex = f.color;
                }
                catch { }
                if (fächeradded)
                {
                    ContentPanel.Children.Remove(fächer);
                    ContentPanel.Children.Remove(fächer2);
                    fächeradded = false;
                }
            }
            else
            {
                Zimmernummer.IsEnabled = true;
                Lehrperson.IsEnabled = true;
                if (fachpicker.SelectedIndex == fachpicker.Items.Count - 1)
                {
                    if (fächeradded) { }
                    else
                    {
                        Zimmernummer.Text = "";
                        Lehrperson.Text = "";
                        //benutzerdefiniertes Fach
                        fächer = new TextBlock() { Text = "Fach: ", FontSize = 34, Margin = new Thickness(7, 0, 0, 0) };
                        fächer.SetValue(Grid.RowProperty, 3);
                        fächer2 = new TextBox();
                        fächer2.SetValue(Grid.RowProperty, 3);
                        fächer2.SetValue(Grid.ColumnProperty, 1);
                        ContentPanel.Children.Add(fächer);
                        ContentPanel.Children.Add(fächer2);
                        Lehrperson.Text = "";
                        fächeradded = true;
                    }
                }
                else
                {
                    Fach f = fachpicker.SelectedItem as Fach;
                    Lehrperson.Text = f.Lehrer;
                    Farbe.SelectedIndex = f.color;
                    if (fächeradded)
                    {
                        ContentPanel.Children.Remove(fächer);
                        ContentPanel.Children.Remove(fächer2);
                        fächeradded = false;
                    }
                }
            }
        }


        private void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            bool faillure = false;
            if (fachpicker.SelectedIndex == fachpicker.Items.Count - 1)
            {
                if (fächer2.Text.Trim().Count() == 0)
                {
                    faillure = true;
                    fächer2.Background = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    Variables.activeStunde.SetLektion(Lektion.Anderes,fächer2.Text);
                    fächer2.Background = new SolidColorBrush(Colors.Green);
                }
            }
            else
            {
                Fach f = fachpicker.SelectedItem as Fach;
                Variables.activeStunde.SetFach(f);
            }
            if (Zimmernummer.IsEnabled)
            {
                try
                {
                    Variables.activeStunde.zimmernummer = Convert.ToInt32(Zimmernummer.Text);
                    Zimmernummer.Background = new SolidColorBrush(Colors.Green);
                }
                catch
                {

                    Zimmernummer.Background = new SolidColorBrush(Colors.Red);
                    faillure = true;
                }
                if (Lehrperson.Text.Trim() == "")
                {
                    Lehrperson.Background = new SolidColorBrush(Colors.Red);
                    faillure = true;
                }
                else
                {
                    Lehrperson.Background = new SolidColorBrush(Colors.Green);
                }
                Variables.activeStunde.Fach.SetLehrer(null, Lehrperson.Text);
            }
            Variables.activeStunde.woche = (Woche)wochenpicker.SelectedIndex;
            Helper.Colorobject co = Farbe.SelectedItem as Helper.Colorobject;
            Variables.activeStunde.Fach.color = co.index;
            if (faillure == true)
            {

            }
            else
            {
                /*NavigationOutTransition navigateOutTransition = new NavigationOutTransition();
                navigateOutTransition.Backward = new TurnstileFeatherTransition { Mode = TurnstileFeatherTransitionMode.BackwardOut };
                navigateOutTransition.Forward = new TurnstileFeatherTransition { Mode = TurnstileFeatherTransitionMode.BackwardOut };
                TransitionService.SetNavigationOutTransition(this, navigateOutTransition);*/
                Save.SaveHorario();
                Save.SaveFach();
                NavigationService.Navigate(new Uri("/Stundenplan.xaml?delete=true&selecteditem=" + NavigationContext.QueryString["selecteditem"], UriKind.Relative));
            }
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            /*NavigationOutTransition navigateOutTransition = new NavigationOutTransition();
            navigateOutTransition.Backward = new TurnstileFeatherTransition { Mode = TurnstileFeatherTransitionMode.BackwardOut };
            navigateOutTransition.Forward = new TurnstileFeatherTransition { Mode = TurnstileFeatherTransitionMode.BackwardOut };
            TransitionService.SetNavigationOutTransition(this, navigateOutTransition);*/
            e.Cancel = true;
            NavigationService.Navigate(new Uri("/Stundenplan.xaml?delete=true&selecteditem=" + NavigationContext.QueryString["selecteditem"], UriKind.Relative));
        }
    }
}