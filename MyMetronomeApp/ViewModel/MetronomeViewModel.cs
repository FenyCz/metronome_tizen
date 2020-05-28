/*
 * Bakalářská práce - Metronom pro mobilní zařízení Android
 *
 * VUT FIT 2019/20
 *
 * Autor: František Pomkla
 */

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

    // třída představující ViewModel v rámci architektury MVVM
    public class MetronomeViewModel : ViewModelBase
    {
        public int currentValue = 120;
        public double currentInterval;
        public Timer timer = new Timer();
        public bool isPlaying = false;
        readonly Vibrator vibrator = Vibrator.Vibrators[0];

        // proměnná pro nastavování tempa
        public int CurrentValue
        {
            set
            {
                if (value != currentValue)
                {
                    currentValue = value;
                    SetProperty(ref currentValue, value);
                    
                    // pokud metronom hraje, nastavíme nový běh metronomu      
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

        // konstruktor
        public MetronomeViewModel()
        {
            startMetronome = new Command(PlayStopCommand);

            timer.Elapsed += ClickEvent;
        }

        // funkce pro zapínání a vypínání metronomu
        private void PlayStopCommand(object obj)
        {
            if (!isPlaying)
            {
                isPlaying = true;

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

        // funkce pro vykonávání vybrování
        private void ClickEvent(object sender, ElapsedEventArgs e)
        {
            vibrator.Vibrate(60, 100);
            timer.Start();
        }

        Command startMetronome;

        public Command StartMetronome
        {
            get { return startMetronome; }
        }
    }
}
