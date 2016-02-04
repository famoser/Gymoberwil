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
    public partial class Fachbearbeiten : PhoneApplicationPage
    {
        public Fachbearbeiten()
        {
            InitializeComponent();

            List<Fach> list = Helper.MakeFachListContrary();
            if (Variables.activeFach != null)
            {
                list.Insert(0,Variables.activeFach);
                Löschen.Opacity = 1;
                Löschen.IsHitTestVisible = true;
                title.Text = "FACH BEARBEITEN";
            }
            Farbe.ItemsSource = Helper.MakeColors();
            fachpicker.ItemsSource = list;
            Fill();
        }

        private void Fill()
        {
            if (Variables.activeFach != null)
            {
                Farbe.SelectedIndex = Variables.activeFach.color;
                Lehrperson.Text = Variables.activeFach.Lehrer;
                fachpicker.SelectedIndex = 0;
                fachpicker.IsEnabled = false;
            }
        }

        private bool fächeradded = false;
        public TextBlock fächer;
        public TextBox fächer2;
        private void fachpicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Fach f = fachpicker.SelectedItem as Fach;
            if (f.lektion == Lektion.Anderes && f.customfach == null)
            {
                if (fächeradded) { }
                else
                {
                    //benutzerdefiniertes Fach
                    fächer = new TextBlock() { Text = "Fach: ", FontSize = 34, Margin = new Thickness(7,0,0,0) };
                    fächer.SetValue(Grid.RowProperty, 1);
                    fächer2 = new TextBox();
                    fächer2.SetValue(Grid.RowProperty, 1);
                    fächer2.SetValue(Grid.ColumnProperty, 1);
                    ContentPanel.Children.Add(fächer);
                    ContentPanel.Children.Add(fächer2);
                    fächeradded = true;
                }
            }
            else
            {
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

        private void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Variables.activeFach != null)
            {
                Variables.activeFach.color = Farbe.SelectedIndex;
                Variables.activeFach.SetLehrer(null, Lehrperson.Text);
            }
            else
            {
                List<Fach> list = Helper.MakeFachListContrary();
                Fach f = fachpicker.SelectedItem as Fach;
                if (Lehrperson.Text.Trim().Count() == 0)
                {
                    Lehrperson.Background = new SolidColorBrush(Colors.Red);
                    return;
                }
                else
                {
                    Lehrperson.Background = new SolidColorBrush(Colors.White);
                }
                if (f.lektion == Lektion.Anderes && f.customfach == null)
                {
                    if (fächer2.Text.Trim() != "")
                    {
                        Fach fach = new Fach() { color = Farbe.SelectedIndex, index = 1000, isActive = true, lektion = Lektion.Anderes, customfach = fächer2.Text };
                        fach.SetLehrer(null, Lehrperson.Text);
                        Variables.Fächer.Insert(Variables.Fächer.Count - 1, fach);
                        NavigationService.Navigate(new Uri("/Fachsortieren.xaml?delete=true", UriKind.Relative));
                        return;
                    }
                    else
                    {
                        fächer2.Background = new SolidColorBrush(Colors.Red);
                        return;
                    }
                }
                list[fachpicker.SelectedIndex].isActive = true;
                list[fachpicker.SelectedIndex].index = Helper.MakeFachList().Count;
                list[fachpicker.SelectedIndex].color = Farbe.SelectedIndex;
                list[fachpicker.SelectedIndex].SetLehrer(null, Lehrperson.Text);
            }
            NavigationService.Navigate(new Uri("/Fachsortieren.xaml?delete=true", UriKind.Relative));
        }

        private void Button_Tap2(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Variables.activeFach.isActive = false;
            Variables.activeFach.index = 0;
            NavigationOutTransition navigateOutTransition = new NavigationOutTransition();
            navigateOutTransition.Backward = new TurnstileFeatherTransition { Mode = TurnstileFeatherTransitionMode.BackwardOut };
            navigateOutTransition.Forward = new TurnstileFeatherTransition { Mode = TurnstileFeatherTransitionMode.BackwardOut };
            TransitionService.SetNavigationOutTransition(this, navigateOutTransition);
            NavigationService.Navigate(new Uri("/Fachsortieren.xaml?delete=true", UriKind.Relative));
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            NavigationService.Navigate(new Uri("/Fachsortieren.xaml?delete=true", UriKind.Relative));
        }
    }
}