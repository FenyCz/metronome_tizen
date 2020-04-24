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
using MyMetronomeApp.ViewModel;

namespace MyMetronomeApp.PlayerViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChoosePlaylist : CirclePage
    {
        private MetronomeViewModel mViewModel;

        public ChoosePlaylist(MetronomeViewModel data)
        {
            mViewModel = data;
            BindingContext = mViewModel;
            InitializeComponent();
            PlaylistListView();
        }
        private void PlaylistListView()
        {
            List<MListItem> pListItems = new List<MListItem>
            {
                new MListItem("Majvely"),
            };
            PlaylistList.ItemsSource = pListItems;
        }

        private void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            MListItem item = (MListItem)e.Item;
            if (item.Name.Equals("Majvely"))
            {
                Navigation.PushModalAsync(new Player(mViewModel));
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