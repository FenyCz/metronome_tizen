using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Tizen.Wearable.CircularUI.Forms;
using MyMetronomeApp.SettingsViews;
using MyMetronomeApp.BTHandler;

namespace MyMetronomeApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : CirclePage
    {
        static BluetoothHandler mHandler;

        public Settings(BluetoothHandler data)
        {
            InitializeComponent();
            SettingsListView();
            mHandler = data;
            //BindingContext = mHandler;
        }

        private void SettingsListView()
        {
            List<MListItem> sListItems = new List<MListItem>
            {
                new MListItem("Bluetooth"),
                //new MListItem("Vibration"),
                //new MListItem("Sound"),

            };
            SettingsList.ItemsSource = sListItems;
        }

        private void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            MListItem item = (MListItem)e.Item;
            if (item.Name.Equals("Bluetooth"))
            {
                Navigation.PushModalAsync(new Bluetooth(mHandler));
            }
            /*else if (item.Name.Equals("Player"))
            {
                Navigation.PushModalAsync(new Player());
            }
            else if (item.Name.Equals("Settings"))
            {
                //Navigation.PushModalAsync(new Settings());
            }*/
        }
    }
}