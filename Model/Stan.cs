namespace UpravljanjeZgradama.Model
{
    public class Stan
    {
        public int BrojStana { get; set; }
        public string Opis { get; set; }
        public int BrojSoba { get; set; }
        public int MaxStanara { get; set; }
        public string SifraZgrade { get; set; }
        public List<string> JmbgoviStanara { get; set; } = new List<string>();

        public bool JePun()
        {
            return JmbgoviStanara.Count >= MaxStanara;
        }
    }
}