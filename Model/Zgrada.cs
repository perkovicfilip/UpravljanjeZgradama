using UpravljanjeZgradama.Model.Enums;

namespace UpravljanjeZgradama.Model
{
    public class Zgrada
    {
        public string Sifra { get; set; }
        public string Ulica { get; set; }
        public int Broj { get; set; }
        public string Naselje { get; set; }
        public string Grad { get; set; }
        public string Drzava { get; set; }
        public int BrojSpratova { get; set; }
        public string JmbgUpravnika { get; set; }
        public StatusZgrade Status { get; set; }
        public List<Stan> Stanovi { get; set; } = new List<Stan>();

        public void Odobri()
        {
            Status = StatusZgrade.Odobrena;
        }

        public void Odbij()
        {
            Status = StatusZgrade.Odbijena;
        }

        public void DodajStan(Stan stan)
        {
            Stanovi.Add(stan);
        }

        public bool JeVidljiva()
        {
            return Status == StatusZgrade.Odobrena;
        }
    }
}