using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tizen.Pims.Contacts.ContactsViews;

namespace MyMetronomeApp
{
    public class Playlist
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public int SongsId { get; set; }

        //public List<SongItem> Songs { get; set; }

        public Playlist()
        {
            //Songs = new List<SongItem>();
        }

        public Playlist(string name)
        {
            Name = name;
            //Songs = songs;
        }
    }

    public class Song
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public int Tempo { get; set; }

        public int SongsId { get; set; }

        public Song()
        {
            //Songs = new List<SongItem>();
        }

        public Song(string name, int tempo, int songsId)
        {
            Name = name;
            Tempo = tempo;
            SongsId = songsId;
        }
    }
    
}
