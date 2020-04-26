using System;
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
        PlayerViewModel pViewModel;
        private Button btn;
        private bool btnPlay = true;
        List<SongItem> songList = new List<SongItem>();
        string idPlaylist;

        bool _rotating;
        int _angle;
        bool flagNoSongs = false;

        public Player(PlayerViewModel data, string nameId)
        {
            InitializeComponent();
            pViewModel = data;
            idPlaylist = nameId;
            BindingContext = pViewModel;
            _angle = 0;
            
            for(int k = 0; k < pViewModel.sListItems.Count; k++)
            {
                if (pViewModel.sListItems[k].PlaylistName == idPlaylist)
                {
                    songList.Add(new SongItem(pViewModel.sListItems[k].Name, pViewModel.sListItems[k].Tempo));
                }
            }
            
            if(songList.Count == 0)
            {
                tempo.Text = "100";
                songName.Text = "No songs";
                flagNoSongs = true;
            }
            else
            {
                tempo.Text = songList[0].Tempo.ToString();
                songName.Text = songList[0].Name;
            }
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
            pViewModel.timer.Enabled = false;
            pViewModel.isPlaying = false;

            return base.OnBackButtonPressed();
        }

        public void Rotate(RotaryEventArgs args)
        {
            if (_rotating) return;

            if(flagNoSongs)
            {
                return;
            }

            _rotating = true;

            _angle += args.IsClockwise ? 1 : -1;

            if(_angle < 1)
            {
                _angle = 0;
            }

            if(_angle >= songList.Count())
            {
                _angle = songList.Count() - 1; 
            }
            tempo.Text = songList[_angle].Tempo.ToString();
            songName.Text = songList[_angle].Name;

            _rotating = false;
           
        }
    }
}