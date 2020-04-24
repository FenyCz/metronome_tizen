using System;
using System.Collections.Generic;
using System.Timers;
using Tizen.System;
using Xamarin.Forms;
using Tizen.Wearable.CircularUI.Forms;
using SQLite;
using SQLitePCL;
using System.IO;

namespace MyMetronomeApp.ViewModel
{
    public class PlayerViewModel
    {
        public Command startMetronome;
        public int currentValue = 120;
        public double currentInterval;
        public Timer timer = new Timer();
        public bool isPlaying = false;
        readonly Vibrator vibrator = Vibrator.Vibrators[0];

        public List<Playlist> pListItems = new List<Playlist>();

        SQLiteConnection dbConnection;
        static string databasePath;

        public Playlist playlist1;
        public Playlist playlist2;

        public PlayerViewModel() {

            InitDatabase();

            var playlistList = dbConnection.Table<Playlist>();
            foreach (var item in playlistList) { pListItems.Add(new Playlist(item.Name)); }

            startMetronome = new Command(PlayStopCommand);

            timer.Elapsed += ClickEvent;
        }

        private void InitDatabase()
        {
            bool needCreateTable = false;

            // inicializace databaze
            raw.SetProvider(new SQLite3Provider_sqlite3());
            raw.FreezeProvider(true);

            string writablePath = global::Tizen.Applications.Application.Current.DirectoryInfo.Data;

            databasePath = Path.Combine(writablePath, "SQLite.db3");

            // Check the database file to decide table creation.
            if (!File.Exists(databasePath))
            {
                needCreateTable = true;
            }

            dbConnection = new SQLiteConnection(databasePath);
            if (needCreateTable)
            {
                dbConnection.CreateTable<Playlist>();
                //dbConnection.CreateTable<Song>();
            }

            //insert into table
            playlist1 = new Playlist
            {
                Name = "Majvely",
                SongsId = 1,
            };

            //playlist1.Songs.Add(new SongItem("Naděje svítá nám", 145));
            //playlist1.Songs.Add(new SongItem("Florentská romance", 140));

            /*playlist2 = new Playlist
            {
                Name = "Calienté",
                SongsId = 2,
            };*/

            //playlist2.Songs.Add(new SongItem("Calienté", 145));
            //playlist2.Songs.Add(new SongItem("Výtahovej song", 140));

            dbConnection.Insert(playlist1);
            //dbConnection.Insert(playlist2);

            var inList = dbConnection.Table<Playlist>();

            foreach (var item in inList)
            {
                if (string.Compare(item.Name, playlist1.Name) != 0)
                {
                    dbConnection.Insert(playlist1);
                }

                if (string.Compare(item.Name, playlist2.Name) != 0)
                {
                    dbConnection.Insert(playlist2);
                }
            }

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

        public Command StartMetronome
        {
            get { return startMetronome; }
        }
    }
}
