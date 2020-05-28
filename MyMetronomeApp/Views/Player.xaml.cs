/*
 * Bakalářská práce - Metronom pro mobilní zařízení Android
 *
 * VUT FIT 2019/20
 *
 * Autor: František Pomkla
 */

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
    // třída představující stránku s playlistem
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Player : CirclePage, IRotaryEventReceiver
    {
        PlayerViewModel pViewModel;
        private Button btn;
        private bool btnPlay = true;
        List<SongItem> songList = new List<SongItem>();
        string idPlaylist;

        bool rotating;
        int angle;
        bool flagNoSongs = false;

        public Player(PlayerViewModel data, string nameId)
        {
            InitializeComponent();
            pViewModel = data;
            idPlaylist = nameId;
            BindingContext = pViewModel;
            angle = 0;
            
            // vložení všech písniček daného playlistu do playeru
            for(int k = 0; k < pViewModel.sListItems.Count; k++)
            {
                if (pViewModel.sListItems[k].PlaylistName == idPlaylist)
                {
                    songList.Add(new SongItem(pViewModel.sListItems[k].Name, pViewModel.sListItems[k].Tempo));
                }
            }
            
            // pokud není v playlistu žádná písnička
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

        // funkce reagující na stisknutí tlačítka
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

        // funkce pro ukončení playeru při návratu zpět 
        protected override bool OnBackButtonPressed()
        {
            pViewModel.timer.Enabled = false;
            pViewModel.isPlaying = false;

            return base.OnBackButtonPressed();
        }

        // funkce pro listování mezi písničkami playlistu
        public void Rotate(RotaryEventArgs args)
        {
            if (rotating) return;

            // pokud neobsahuje playlist žádný song
            if(flagNoSongs)
            {
                return;
            }

            rotating = true;

            angle += args.IsClockwise ? 1 : -1;

            // pokud jsme jdeme ze začátku na konec playlistu
            if(angle < 1)
            {
                angle = songList.Count() - 1;
            }

            // pokud jdeme z konce na začátek playlistu
            else if(angle >= songList.Count())
            {
                angle = 0; 
            }

            tempo.Text = songList[angle].Tempo.ToString();
            songName.Text = songList[angle].Name;

            rotating = false;
           
        }
    }
}