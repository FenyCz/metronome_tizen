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
using MyMetronomeApp.Model;
using MyMetronomeApp.ViewModel;

namespace MyMetronomeApp
{
    // třída představující stránku metronomu
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Metronome : CirclePage
    {

        MetronomeViewModel mViewModel;
        bool btnPlay = true;
        Button btn;

        public Metronome(MetronomeViewModel data)
        {
            mViewModel = data;
            BindingContext = mViewModel;
            InitializeComponent();
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

        // funkce pro ukončení metronomu při návratu zpět na hlavní stránku
        protected override bool OnBackButtonPressed()
        {
            mViewModel.timer.Enabled = false;
            mViewModel.isPlaying = false;

            return base.OnBackButtonPressed();
        }

    }

}