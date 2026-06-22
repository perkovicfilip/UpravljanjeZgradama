using UpravljanjeZgradama.Model;
using UpravljanjeZgradama.Repository;

namespace UpravljanjeZgradama.Service
{
    public class KorisnikService
    {
        private KorisnikRepository korisnikRepo = new KorisnikRepository();

        public Korisnik Prijava(string email, string lozinka)
        {
            Korisnik korisnik = korisnikRepo.VratiPoEmailu(email);
            if (korisnik != null && korisnik.ProveriLozinku(lozinka))
                return korisnik;
            return null;
        }

        public bool EmailZauzet(string email)
        {
            return korisnikRepo.VratiPoEmailu(email) != null;
        }

        public bool JmbgZauzet(string jmbg)
        {
            return korisnikRepo.VratiPoJmbg(jmbg) != null;
        }

        public bool RegistrujStanara(Stanar stanar)
        {
            if (EmailZauzet(stanar.Email) || JmbgZauzet(stanar.Jmbg))
                return false;
            korisnikRepo.Dodaj(stanar);
            return true;
        }

        public bool RegistrujUpravnika(Upravnik upravnik)
        {
            if (EmailZauzet(upravnik.Email) || JmbgZauzet(upravnik.Jmbg))
                return false;
            korisnikRepo.Dodaj(upravnik);
            return true;
        }

        public List<Korisnik> VratiSve()
        {
            return korisnikRepo.VratiSve();
        }
    }
}