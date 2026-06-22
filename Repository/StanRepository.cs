using UpravljanjeZgradama.Model;

namespace UpravljanjeZgradama.Repository
{
    public class StanRepository
    {
        private string putanjaFajla = "Data/stanovi.txt";

        public List<Stan> VratiSve()
        {
            List<Stan> stanovi = new List<Stan>();
            if (!File.Exists(putanjaFajla))
                return stanovi;

            foreach (string linija in File.ReadAllLines(putanjaFajla))
            {
                if (linija.Trim() == "")
                    continue;
                stanovi.Add(LinijaUStan(linija));
            }
            return stanovi;
        }

        public List<Stan> VratiPoZgradi(string sifraZgrade)
        {
            return VratiSve().Where(s => s.SifraZgrade == sifraZgrade).ToList();
        }

        public void Dodaj(Stan stan)
        {
            List<Stan> stanovi = VratiSve();
            stanovi.Add(stan);
            SacuvajSve(stanovi);
        }

        public void Izmeni(Stan izmenjen)
        {
            List<Stan> stanovi = VratiSve();
            for (int i = 0; i < stanovi.Count; i++)
            {
                if (stanovi[i].SifraZgrade == izmenjen.SifraZgrade &&
                    stanovi[i].BrojStana == izmenjen.BrojStana)
                {
                    stanovi[i] = izmenjen;
                    break;
                }
            }
            SacuvajSve(stanovi);
        }

        private void SacuvajSve(List<Stan> stanovi)
        {
            List<string> linije = new List<string>();
            foreach (Stan stan in stanovi)
                linije.Add(StanULiniju(stan));
            File.WriteAllLines(putanjaFajla, linije);
        }

        private string StanULiniju(Stan s)
        {
            string stanari = string.Join(",", s.JmbgoviStanara);
            return s.BrojStana + ";" + s.Opis + ";" + s.BrojSoba + ";" +
                   s.MaxStanara + ";" + s.SifraZgrade + ";" + stanari;
        }

        private Stan LinijaUStan(string linija)
        {
            string[] d = linija.Split(';');
            Stan s = new Stan
            {
                BrojStana = int.Parse(d[0]),
                Opis = d[1],
                BrojSoba = int.Parse(d[2]),
                MaxStanara = int.Parse(d[3]),
                SifraZgrade = d[4]
            };
            if (d.Length > 5 && d[5].Trim() != "")
                s.JmbgoviStanara = d[5].Split(',').ToList();
            return s;
        }
    }
}