﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Tizen.Wearable.CircularUI.Forms;
using Tizen.Network.Bluetooth;
using Tizen.Applications;
using System.Runtime.CompilerServices;
using MyMetronomeApp.ViewModel;

namespace MyMetronomeApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Player : CirclePage, IRotaryEventReceiver
    {
        MetronomeViewModel mViewModel;
        private Button btn;
        private bool btnPlay = true;
        List<SongItem> pListItems = new List<SongItem>();

        bool _rotating;
        int _angle;

        public Player(MetronomeViewModel data)
        {
            InitializeComponent();
            SongsListView();
            mViewModel = data;
            BindingContext = mViewModel;
            _angle = 0;

            tempo.Text = pListItems[0].Tempo.ToString();
            songName.Text = pListItems[0].Name;
        }

        private void SongsListView()
        {
            pListItems.Add(new SongItem("Naděje svítá nám", 145));
            pListItems.Add(new SongItem("Florentská romance", 140));
        }

        private void OnClick(object sender, EventArgs e)
        {
            btn = sender as Button;

            if (btnPlay)
            {
                btn.ImageSource = "images/stop.png";
                btnPlay = false;
            }

            else
            {
                btn.ImageSource = "images/play.png";
                btnPlay = true;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            mViewModel.timer.Enabled = false;
            mViewModel.isPlaying = false;

            return base.OnBackButtonPressed();
        }

        public void Rotate(RotaryEventArgs args)
        {
            if (_rotating) return;

            _rotating = true;

            _angle += args.IsClockwise ? 1 : -1;

            if(_angle < 1)
            {
                _angle = 0;
            }

            if(_angle >= pListItems.Count())
            {
                _angle = pListItems.Count() - 1; 
            }
            tempo.Text = pListItems[_angle].Tempo.ToString();
            songName.Text = pListItems[_angle].Name;

            _rotating = false;
           
        }
    }
}