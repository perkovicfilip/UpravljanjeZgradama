using UpravljanjeZgradama.Model;
using UpravljanjeZgradama.Model.Enums;

namespace UpravljanjeZgradama.Repository
{
    public class ZgradaRepository
    {
        private string putanjaFajla = "Data/zgrade.txt";

        public List<Zgrada> VratiSve()
        {
            List<Zgrada> zgrade = new List<Zgrada>();
            if (!File.Exists(putanjaFajla))
                return zgrade;

            foreach (string linija in File.ReadAllLines(putanjaFajla))
            {
                if (linija.Trim() == "")
                    continue;
                zgrade.Add(LinijaUZgradu(linija));
            }
            return zgrade;
        }

        public Zgrada VratiPoSifri(string sifra)
        {
            return VratiSve().FirstOrDefault(z => z.Sifra == sifra);
        }

        public void Dodaj(Zgrada zgrada)
        {
            List<Zgrada> zgrade = VratiSve();
            zgrade.Add(zgrada);
            SacuvajSve(zgrade);
        }

        public void Izmeni(Zgrada izmenjena)
        {
            List<Zgrada> zgrade = VratiSve();
            for (int i = 0; i < zgrade.Count; i++)
            {
                if (zgrade[i].Sifra == izmenjena.Sifra)
                {
                    zgrade[i] = izmenjena;
                    break;
                }
            }
            SacuvajSve(zgrade);
        }

        public void Obrisi(string sifra)
        {
            List<Zgrada> zgrade = VratiSve();
            zgrade.RemoveAll(z => z.Sifra == sifra);
            SacuvajSve(zgrade);
        }

        private void SacuvajSve(List<Zgrada> zgrade)
        {
            List<string> linije = new List<string>();
            foreach (Zgrada zgrada in zgrade)
                linije.Add(ZgradaULiniju(zgrada));
            File.WriteAllLines(putanjaFajla, linije);
        }

        private string ZgradaULiniju(Zgrada z)
        {
            return z.Sifra + ";" + z.Ulica + ";" + z.Broj + ";" + z.Naselje + ";" +
                   z.Grad + ";" + z.Drzava + ";" + z.BrojSpratova + ";" +
                   z.JmbgUpravnika + ";" + z.Status;
        }

        private Zgrada LinijaUZgradu(string linija)
        {
            string[] d = linija.Split(';');
            return new Zgrada
            {
                Sifra = d[0],
                Ulica = d[1],
                Broj = int.Parse(d[2]),
                Naselje = d[3],
                Grad = d[4],
                Drzava = d[5],
                BrojSpratova = int.Parse(d[6]),
                JmbgUpravnika = d[7],
                Status = (StatusZgrade)System.Enum.Parse(typeof(StatusZgrade), d[8])
            };
        }
    }
}