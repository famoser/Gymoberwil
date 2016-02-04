using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Windows.ApplicationModel.Store;

namespace Gymoberwil
{
    public partial class Über : PhoneApplicationPage
    {
        public Über()
        {
            InitializeComponent();
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.Subject = "Gymoberwil App";
            emailComposeTask.Body = "";
            emailComposeTask.To = "GymoberwilApp@outlook.com";
            emailComposeTask.Show();
        }


        public string receipt;

        async private void BuyMeABeer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            //50 Points - Consumable
                var listing = await CurrentApp.LoadListingInformationAsync();
                var fiftypoints =
                    listing.ProductListings.FirstOrDefault(p => p.Value.ProductId == "onebeer" && p.Value.ProductType == ProductType.Consumable);

                await CurrentApp.RequestProductPurchaseAsync(fiftypoints.Value.ProductId, false);

                Settings.boughtbeers += 1;
                if (Settings.boughtbeers == 1)
                {
                    MessageBox.Show("Vielen Dank für das erste Bier!");
                }
                else if (Settings.boughtbeers == 2)
                {
                    MessageBox.Show("Vieleen Dank für das zweite Bier! Zögere nicht, mir einen Kater zu verpassen!");
                }
                else if (Settings.boughtbeers == 3)
                {
                    MessageBox.Show("Viln Dankk für dassss dritte Bier (oder vierete weis nich alter)!");
                }
                else
                {
                    MessageBox.Show("http://tiny.cc/ichbintot" + "(Bier Nummer " + Settings.boughtbeers.ToString() + ")");
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Vorgang fehlgeschlagen.");
            }
        }

    }
}