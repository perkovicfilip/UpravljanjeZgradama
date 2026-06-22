using UpravljanjeZgradama.Model;
using UpravljanjeZgradama.Model.Enums;

namespace UpravljanjeZgradama.Repository
{
    public class KorisnikRepository
    {
        private string putanjaFajla = "Data/korisnici.txt";

        public List<Korisnik> VratiSve()
        {
            List<Korisnik> korisnici = new List<Korisnik>();
            if (!File.Exists(putanjaFajla))
                return korisnici;

            foreach (string linija in File.ReadAllLines(putanjaFajla))
            {
                if (linija.Trim() == "")
                    continue;
                korisnici.Add(LinijaUKorisnika(linija));
            }
            return korisnici;
        }

        public Korisnik VratiPoEmailu(string email)
        {
            return VratiSve().FirstOrDefault(k => k.Email == email);
        }

        public Korisnik VratiPoJmbg(string jmbg)
        {
            return VratiSve().FirstOrDefault(k => k.Jmbg == jmbg);
        }

        public void Dodaj(Korisnik korisnik)
        {
            List<Korisnik> korisnici = VratiSve();
            korisnici.Add(korisnik);
            SacuvajSve(korisnici);
        }

        public void Izmeni(Korisnik izmenjen)
        {
            List<Korisnik> korisnici = VratiSve();
            for (int i = 0; i < korisnici.Count; i++)
            {
                if (korisnici[i].Jmbg == izmenjen.Jmbg)
                {
                    korisnici[i] = izmenjen;
                    break;
                }
            }
            SacuvajSve(korisnici);
        }

        private void SacuvajSve(List<Korisnik> korisnici)
        {
            List<string> linije = new List<string>();
            foreach (Korisnik korisnik in korisnici)
                linije.Add(KorisnikULiniju(korisnik));
            File.WriteAllLines(putanjaFajla, linije);
        }

        private string KorisnikULiniju(Korisnik k)
        {
            TipKorisnika tip = OdrediTip(k);
            return tip + ";" + k.Jmbg + ";" + k.Email + ";" + k.Lozinka + ";" +
                   k.Ime + ";" + k.Prezime + ";" + k.MobilniTelefon;
        }

        private TipKorisnika OdrediTip(Korisnik k)
        {
            if (k is Administrator)
                return TipKorisnika.Administrator;
            if (k is Upravnik)
                return TipKorisnika.Upravnik;
            return TipKorisnika.Stanar;
        }

        private Korisnik LinijaUKorisnika(string linija)
        {
            string[] d = linija.Split(';');
            TipKorisnika tip = (TipKorisnika)System.Enum.Parse(typeof(TipKorisnika), d[0]);

            Korisnik k = NapraviPraznog(tip);
            k.Jmbg = d[1];
            k.Email = d[2];
            k.Lozinka = d[3];
            k.Ime = d[4];
            k.Prezime = d[5];
            k.MobilniTelefon = d[6];
            return k;
        }

        private Korisnik NapraviPraznog(TipKorisnika tip)
        {
            if (tip == TipKorisnika.Administrator)
                return new Administrator();
            if (tip == TipKorisnika.Upravnik)
                return new Upravnik();
            return new Stanar();
        }
    }
}