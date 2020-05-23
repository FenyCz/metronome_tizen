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
        private PlayerViewModel pViewModel;

        public ChoosePlaylist(PlayerViewModel data)
        {
            pViewModel = data;
            BindingContext = pViewModel;
            InitializeComponent();
            PlaylistList.ItemsSource = pViewModel.pListItems;
        }

        private void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Playlist item = (Playlist)e.Item;
            for (int i = 0; i < pViewModel.pListItems.Count(); i++)
            {
                if (item.Name.Equals(pViewModel.pListItems[i].Name))
                {
                    Navigation.PushModalAsync(new Player(pViewModel, pViewModel.pListItems[i].Name));
                }
            }
        }
    }
}