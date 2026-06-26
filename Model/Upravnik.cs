namespace UpravljanjeZgradama.Model
{
    public class Upravnik : Korisnik
    {
        public List<Zgrada> Zgrade { get; set; } = new List<Zgrada>();
    }
}