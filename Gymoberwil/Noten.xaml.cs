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
    public partial class PivotPage1 : PhoneApplicationPage
    {
        public PivotPage1()
        {
            InitializeComponent();
            List<Fach> list = Helper.MakeFachList();
            Übersicht.ItemsSource = list;
            foreach (var item in list)
            {
                MakeLongList(item);
            }
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
                Noten.SelectedIndex = Convert.ToInt32(NavigationContext.QueryString["pivotitem"]);
            }
        }

        List<LongListSelector> longlistselectors = new List<LongListSelector>();

        private void MakeLongList(Fach Fach)
        {
            try
            {
                Grid g = new Grid();
                RowDefinition r = new RowDefinition() { Height = GridLength.Auto };
                g.RowDefinitions.Add(r);
                LongListSelector listBox = new LongListSelector
                {
                    IsGroupingEnabled = false,
                    ItemTemplate = (DataTemplate)Resources["FachTemplate"],
                    
               };
                Button b = new Button();
                b.Content = "zur Übersicht";
                b.Tap += b_Tap;
                b.Margin = new Thickness(0, 30, 0, 80);
                listBox.ListFooter = b;
                if (Fach.noten.Count == 0)
                {
                    List<Note> note = new List<Note>() { new Note(true) };
                    listBox.ItemsSource = note;
                }
                else
                {
                    listBox.ItemsSource = Fach.noten;
                }
                listBox.Tap += listBox_Tap;
                listBox.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Visible);
                listBox.SetValue(TurnstileFeatherEffect.FeatheringIndexProperty, -1);
                longlistselectors.Add(listBox);
                g.Children.Add(listBox);
                ScrollViewer sv = new ScrollViewer();
                sv.Content = g;
                PivotItem pi = new PivotItem();
                TextBlock t = new TextBlock() { Text = Fach.Name };
                pi.Header = t;
                pi.Content = sv;
                Noten.Items.Add(pi);
            }
            catch (Exception e) { MessageBox.Show("Darstellung fehlgeschlagen. Exeption: " + e); }
        }

        void b_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Noten.SelectedIndex = 0;
        }

        private void listBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LongListSelector lls = sender as LongListSelector;
            lls.SetValue(TurnstileFeatherEffect.FeatheringIndexProperty, 1);
            Note n = lls.SelectedItem as Note;
            if (n != null)
            {
                Variables.activeNote = n;
                NavigationService.Navigate(new Uri("/Notehinzufügen.xaml?activenote=true&active=" + Noten.SelectedIndex.ToString(), UriKind.Relative));
            }
        }

        private void übersicht_Tag(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LongListSelector lls = sender as LongListSelector;
            Fach f = lls.SelectedItem as Fach;
            List<Fach> list = Helper.MakeFachList();
            Noten.SelectedIndex = list.IndexOf(f) + 1;
        }

        private void editfaecher_Click(object sender, EventArgs e)
        {
            int a = Noten.SelectedIndex;
            if (a == 0)
            {
                Übersicht.SetValue(TurnstileFeatherEffect.FeatheringIndexProperty, 1);
            }
            else
            {
                a--;
                longlistselectors[a].SetValue(TurnstileFeatherEffect.FeatheringIndexProperty, 1);
            }
            NavigationService.Navigate(new Uri("/Fachsortieren.xaml?active=" + Noten.SelectedIndex.ToString()+"&delete=false", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            int a = Noten.SelectedIndex;
            if (a == 0)
            {
                Übersicht.SetValue(TurnstileFeatherEffect.FeatheringIndexProperty, 1);
            }
            else
            {
                a--;
                longlistselectors[a].SetValue(TurnstileFeatherEffect.FeatheringIndexProperty, 1);
            }
            NavigationService.Navigate(new Uri("/Notehinzufügen.xaml?activenote=false&active=" + Noten.SelectedIndex.ToString(), UriKind.Relative));
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int a = Noten.SelectedIndex;
            if (a == 0)
            {
                Übersicht.SetValue(TurnstileFeatherEffect.FeatheringIndexProperty, 1);
            }
            else
            {
                a--;
                longlistselectors[a].SetValue(TurnstileFeatherEffect.FeatheringIndexProperty, 1);
            }
        }
    }
}