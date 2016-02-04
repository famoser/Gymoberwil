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
    public partial class Fachsortieren : PhoneApplicationPage
    {
        public Fachsortieren()
        {
            InitializeComponent();
            Fächer.ItemsSource = Helper.MakeFachList();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.Count > 0)
            {
                if (NavigationContext.QueryString["delete"] == "true")
                {
                    NavigationService.RemoveBackEntry();
                    NavigationService.RemoveBackEntry();
                }
            }
        }


        bool arrowup = false;
        bool arrowdown = false;
        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            arrowup = true;
        }

        private void Image_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            arrowdown = true;
        }

        private void Fächer_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!arrowdown && arrowup)
            {
                arrowup = false;
                Fach sp = Fächer.SelectedItem as Fach;
                List<Fach> list = Fächer.ItemsSource as List<Fach>;
                for (int i = 1; i < list.Count()-1; i++)
                {
                    if (sp == list[i])
                    {
                        list[i].index--;
                        list[i-1].index++;
                            Fächer.ItemsSource = null;
                            Fächer.ItemsSource = Helper.MakeFachList();
                            break;
                    }
                }
            }
            else if (!arrowup && arrowdown)
            {
                arrowdown = false;
                Fach sp = Fächer.SelectedItem as Fach;
                List<Fach> list = Fächer.ItemsSource as List<Fach>;
                for (int i = 0; i < list.Count() - 1; i++)
                {
                    if (sp == list[i])
                    {
                        list[i].index++;
                        list[i - 1].index--;
                        Fächer.ItemsSource = null;
                        Fächer.ItemsSource = Helper.MakeFachList();
                        break;
                    }
                }
            }
            else
            {
                Fach sp = Fächer.SelectedItem as Fach;
                Variables.activeFach = sp;
                NavigationService.Navigate(new Uri("/Fachbearbeiten.xaml", UriKind.Relative));
            }
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            Variables.activeFach = null;
            NavigationService.Navigate(new Uri("/Fachbearbeiten.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            NavigationOutTransition navigateOutTransition = new NavigationOutTransition();
            navigateOutTransition.Backward = new TurnstileFeatherTransition { Mode = TurnstileFeatherTransitionMode.BackwardOut };
            navigateOutTransition.Forward = new TurnstileFeatherTransition { Mode = TurnstileFeatherTransitionMode.BackwardOut };
            TransitionService.SetNavigationOutTransition(this, navigateOutTransition);
            NavigationService.Navigate(new Uri("/Noten.xaml?delete=true&pivotitem=0", UriKind.Relative));
        }


    }
}