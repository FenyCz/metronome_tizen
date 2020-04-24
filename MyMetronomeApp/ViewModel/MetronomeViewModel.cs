using MyMetronomeApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Tizen.System;
using Xamarin.Forms;

namespace MyMetronomeApp.ViewModel
{

    public class MetronomeViewModel : ViewModelBase
    {
        public int currentValue = 120;
        public double currentInterval;
        //MetronomeModel mModel = new MetronomeModel();
        public Timer timer = new Timer();
        public bool isPlaying = false;
        Vibrator vibrator = Vibrator.Vibrators[0];

        [DllImport("libcapi-system-device.so.0", EntryPoint = "device_power_request_lock", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int DevicePowerRequestLock(int type, int timeout_ms);

        [DllImport("libcapi-system-device.so.0", EntryPoint = "device_power_release_lock", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int DevicePowerReleaseLock(int type);

        enum Power_Type { CPU = 0, DISPLAY = 1, DISPLAY_DIM = 2 };

        public int CurrentValue
        {
            set
            {
                if (value != currentValue)
                {
                    currentValue = value;
                    SetProperty(ref currentValue, value);
                    //Tizen.Wearable.CircularUI.Forms.Toast.DisplayText(currentValue.ToString(), 5000);

                    //OnPropertyChanged();
                    //mModel.Tempo = currentValue;
                          
                    if (isPlaying)
                    {
                        timer.Stop();
                        Task.Delay(500);
                        currentInterval = 59000 / currentValue;
                        timer.AutoReset = false;
                        timer.Interval = currentInterval;
                        timer.Start();
                    }

                }

            }

            get { return currentValue; }
        }

        public MetronomeViewModel()
        {
            //CurrentValue = mModel.Tempo;

            startMetronome = new Command(PlayStopCommand);

            timer.Elapsed += ClickEvent;
        }

        private void PlayStopCommand(object obj)
        {
            if (!isPlaying)
            {
                isPlaying = true;
                //DevicePowerRequestLock((int)Power_Type.CPU, 0);
                currentInterval = 59000 / currentValue;
                timer.AutoReset = false;
                timer.Interval = currentInterval;
                timer.Start();
            }

            else
            {
                isPlaying = false;
                timer.Stop();
            }
        }

        private void ClickEvent(object sender, ElapsedEventArgs e)
        {
            vibrator.Vibrate(60, 100);
            //timer.AutoReset = true;
            timer.Start();
        }

        Command startMetronome;

        public Command StartMetronome
        {
            get { return startMetronome; }
        }
    }
}
