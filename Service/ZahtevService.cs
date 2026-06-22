using UpravljanjeZgradama.Model;
using UpravljanjeZgradama.Model.Enums;
using UpravljanjeZgradama.Repository;

namespace UpravljanjeZgradama.Service
{
    public class ZahtevService
    {
        private ZahtevRepository zahtevRepo = new ZahtevRepository();
        private StanRepository stanRepo = new StanRepository();

        public Zahtev KreirajZahtev(string stanarJmbg, string sifraZgrade, int brojStana)
        {
            Zahtev zahtev = new Zahtev
            {
                StanarJmbg = stanarJmbg,
                SifraZgrade = sifraZgrade,
                BrojStana = brojStana,
                DatumKreiranja = DateTime.Now,
                Status = StatusZahteva.CekaOdobrenje,
                RazlogOdbijanja = ""
            };
            zahtevRepo.Dodaj(zahtev);
            return zahtev;
        }

        public bool StanZauzet(string sifraZgrade, int brojStana)
        {
            Stan stan = stanRepo.VratiPoZgradi(sifraZgrade)
                                .FirstOrDefault(s => s.BrojStana == brojStana);
            return stan != null && stan.JePun();
        }

        public List<Zahtev> ZahteviStanara(string jmbg)
        {
            return zahtevRepo.VratiPoStanaru(jmbg);
        }

        public List<Zahtev> ZahteviZgradePoStatusu(string sifraZgrade, StatusZahteva status)
        {
            return zahtevRepo.VratiPoZgradi(sifraZgrade)
                             .Where(z => z.Status == status).ToList();
        }

        public void PotvrdiZahtev(Zahtev zahtev)
        {
            zahtev.Potvrdi();
            zahtevRepo.Izmeni(zahtev);
        }

        public void OdbijZahtev(Zahtev zahtev, string razlog)
        {
            zahtev.Odbij(razlog);
            zahtevRepo.Izmeni(zahtev);
        }

        public void PovuciZahtev(Zahtev zahtev)
        {
            zahtevRepo.Obrisi(zahtev);
        }
    }
}