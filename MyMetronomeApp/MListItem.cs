namespace MyMetronomeApp
{
    internal class MListItem
    {
        public string Name { get; set; }

        public string IconPath { get; set; }

        public MListItem(string name, string path)
        {
            Name = name;
            IconPath = path;
        }
    }
}