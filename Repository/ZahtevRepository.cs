using System.Globalization;
using UpravljanjeZgradama.Model;
using UpravljanjeZgradama.Model.Enums;

namespace UpravljanjeZgradama.Repository
{
    public class ZahtevRepository
    {
        private string putanjaFajla = "Data/zahtevi.txt";

        public List<Zahtev> VratiSve()
        {
            List<Zahtev> zahtevi = new List<Zahtev>();
            if (!File.Exists(putanjaFajla))
                return zahtevi;

            foreach (string linija in File.ReadAllLines(putanjaFajla))
            {
                if (linija.Trim() == "")
                    continue;
                zahtevi.Add(LinijaUZahtev(linija));
            }
            return zahtevi;
        }

        public List<Zahtev> VratiPoStanaru(string jmbg)
        {
            return VratiSve().Where(z => z.StanarJmbg == jmbg).ToList();
        }

        public List<Zahtev> VratiPoZgradi(string sifraZgrade)
        {
            return VratiSve().Where(z => z.SifraZgrade == sifraZgrade).ToList();
        }

        public void Dodaj(Zahtev zahtev)
        {
            List<Zahtev> zahtevi = VratiSve();
            zahtevi.Add(zahtev);
            SacuvajSve(zahtevi);
        }

        public void Izmeni(Zahtev izmenjen)
        {
            List<Zahtev> zahtevi = VratiSve();
            for (int i = 0; i < zahtevi.Count; i++)
            {
                if (JeIsti(zahtevi[i], izmenjen))
                {
                    zahtevi[i] = izmenjen;
                    break;
                }
            }
            SacuvajSve(zahtevi);
        }

        public void Obrisi(Zahtev zahtev)
        {
            List<Zahtev> zahtevi = VratiSve();
            zahtevi.RemoveAll(z => JeIsti(z, zahtev));
            SacuvajSve(zahtevi);
        }

        private bool JeIsti(Zahtev a, Zahtev b)
        {
            return a.StanarJmbg == b.StanarJmbg &&
                   a.SifraZgrade == b.SifraZgrade &&
                   a.BrojStana == b.BrojStana &&
                   a.DatumKreiranja == b.DatumKreiranja;
        }

        private void SacuvajSve(List<Zahtev> zahtevi)
        {
            List<string> linije = new List<string>();
            foreach (Zahtev zahtev in zahtevi)
                linije.Add(ZahtevULiniju(zahtev));
            File.WriteAllLines(putanjaFajla, linije);
        }

        private string ZahtevULiniju(Zahtev z)
        {
            return z.StanarJmbg + ";" + z.SifraZgrade + ";" + z.BrojStana + ";" +
                   z.DatumKreiranja.ToString("o", CultureInfo.InvariantCulture) + ";" +
                   z.Status + ";" + z.RazlogOdbijanja;
        }

        private Zahtev LinijaUZahtev(string linija)
        {
            string[] d = linija.Split(';');
            return new Zahtev
            {
                StanarJmbg = d[0],
                SifraZgrade = d[1],
                BrojStana = int.Parse(d[2]),
                DatumKreiranja = DateTime.Parse(d[3], CultureInfo.InvariantCulture),
                Status = (StatusZahteva)Enum.Parse(typeof(StatusZahteva), d[4]),
                RazlogOdbijanja = d.Length > 5 ? d[5] : ""
            };
        }
    }
}