using Tizen.Network.Bluetooth;

namespace MyMetronomeApp
{
    public class SongItem
    {
        public string Name { get; set; }

        public int Tempo { get; set; }

        public SongItem(string name, int tempo)
        {
            Name = name;
            Tempo = tempo;
        }
    }
}