using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyMetronomeApp
{
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
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => Tizen.System.Power.RequestCpuLock(0));
            //Tizen.System.Power.RequestCpuLock(0);
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => Tizen.System.Power.ReleaseCpuLock());
            //Tizen.System.Power.ReleaseCpuLock();
        }
    }
}

