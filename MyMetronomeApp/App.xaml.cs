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

namespace MyMetronomeApp
{
    // třída, která se stará různé stavy aplikace
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MyMetronomeApp.MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps

            // v případě že vypneme displej, metronom hraje dál
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => Tizen.System.Power.RequestCpuLock(0));
        }

        protected override void OnResume()
        {
            // Handle when your app resumes

            // v případě že displej zapneme uvolníme potřebné zdroje
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => Tizen.System.Power.ReleaseCpuLock());
        }
    }
}

