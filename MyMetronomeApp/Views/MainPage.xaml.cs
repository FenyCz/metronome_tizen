﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Tizen.Wearable.CircularUI.Forms;
using MyMetronomeApp.Model;
using MyMetronomeApp.ViewModel;
using Tizen.Applications;
using MyMetronomeApp.SettingsViews;
using MyMetronomeApp.BTHandler;
using MyMetronomeApp.PlayerViews;

namespace MyMetronomeApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : CirclePage
    {
        MetronomeViewModel mViewModel = new MetronomeViewModel();
        PlayerViewModel pViewModel = new PlayerViewModel();
        BluetoothHandler mHandler;

        public MainPage()
        {
            mHandler = new BluetoothHandler(pViewModel);
            InitializeComponent();
            MakeMainListView();
        }
        private void MakeMainListView()
        {
            List<MListItem> mListItems = new List<MListItem>
            {
                new MListItem("Metronome","images/iconMetronome.png"),
                new MListItem("Player","images/iconPlayer.png"),
                new MListItem("Settings","images/iconSettings.png"),

            };
            MainList.ItemsSource = mListItems;
        }

        private void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            MListItem item = (MListItem)e.Item;
            if (item.Name.Equals("Metronome"))
            {
                Navigation.PushModalAsync(new Metronome(mViewModel));
            }
            else if (item.Name.Equals("Player"))
            {
                Navigation.PushModalAsync(new ChoosePlaylist(pViewModel));
            }
            else if (item.Name.Equals("Settings"))
            {
                Navigation.PushModalAsync(new Settings(mHandler));
            }
        }
    }
}