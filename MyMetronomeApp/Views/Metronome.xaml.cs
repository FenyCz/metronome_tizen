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

    }

}