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
        }

        public Playlist(string name, int songsId)
        {
            Name = name;
            SongsId = songsId;
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
        }

        public Song(string name, int tempo, int songsId)
        {
            Name = name;
            Tempo = tempo;
            SongsId = songsId;
        }
    }
    
}
