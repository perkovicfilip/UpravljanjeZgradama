using UpravljanjeZgradama.Model;
using UpravljanjeZgradama.Model.Enums;
using UpravljanjeZgradama.Repository;

namespace UpravljanjeZgradama.Service
{
    public class ZgradaService
    {
        private ZgradaRepository zgradaRepo = new ZgradaRepository();
        private StanRepository stanRepo = new StanRepository();

        public bool UnesiZgradu(Zgrada zgrada)
        {
            if (zgradaRepo.VratiPoSifri(zgrada.Sifra) != null)
                return false;
            zgrada.Status = StatusZgrade.CekaOdobrenje;
            zgradaRepo.Dodaj(zgrada);
            return true;
        }

        public void OdobriZgradu(string sifra)
        {
            Zgrada zgrada = zgradaRepo.VratiPoSifri(sifra);
            if (zgrada != null)
            {
                zgrada.Odobri();
                zgradaRepo.Izmeni(zgrada);
            }
        }

        public void OdbijZgradu(string sifra)
        {
            Zgrada zgrada = zgradaRepo.VratiPoSifri(sifra);
            if (zgrada != null)
            {
                zgrada.Odbij();
                zgradaRepo.Izmeni(zgrada);
            }
        }

        public List<Zgrada> VratiVidljive()
        {
            return zgradaRepo.VratiSve().Where(z => z.JeVidljiva()).ToList();
        }

        public List<Zgrada> VratiZgradeUpravnika(string jmbgUpravnika)
        {
            return zgradaRepo.VratiSve()
                             .Where(z => z.JmbgUpravnika == jmbgUpravnika).ToList();
        }

        public List<Zgrada> SortiranePoSpratovima()
        {
            return VratiVidljive().OrderBy(z => z.BrojSpratova).ToList();
        }

        // ===== PRETRAGA =====

        public List<Zgrada> PretragaPoAdresi(string unos)
        {
            string u = unos.ToLower();
            return VratiVidljive()
                .Where(z => (z.Ulica + " " + z.Broj).ToLower().Contains(u))
                .ToList();
        }

        public List<Zgrada> PretragaPoNaselju(string unos)
        {
            string u = unos.ToLower();
            return VratiVidljive()
                .Where(z => z.Naselje.ToLower().Contains(u))
                .ToList();
        }

        public List<Zgrada> PretragaPoSpratovima(int brojSpratova)
        {
            return VratiVidljive()
                .Where(z => z.BrojSpratova == brojSpratova)
                .ToList();
        }

        public List<Zgrada> PretragaPoBrojuSoba(int brojSoba)
        {
            return VratiVidljive()
                .Where(z => ImaStanSa(z, s => s.BrojSoba == brojSoba))
                .ToList();
        }

        public List<Zgrada> PretragaPoBrojuStanara(int maxStanara)
        {
            return VratiVidljive()
                .Where(z => ImaStanSa(z, s => s.MaxStanara == maxStanara))
                .ToList();
        }

        public List<Zgrada> PretragaPoSobamaIStanarima(int brojSoba, int brojStanara, bool iOperator)
        {
            return VratiVidljive()
                .Where(z => ZadovoljavaSobeIStanare(z, brojSoba, brojStanara, iOperator))
                .ToList();
        }

        private bool ZadovoljavaSobeIStanare(Zgrada z, int brojSoba, int brojStanara, bool iOperator)
        {
            if (iOperator)
                return ImaStanSa(z, s => s.BrojSoba == brojSoba && s.MaxStanara == brojStanara);
            else
                return ImaStanSa(z, s => s.BrojSoba == brojSoba) ||
                       ImaStanSa(z, s => s.MaxStanara == brojStanara);
        }

        private bool ImaStanSa(Zgrada z, System.Func<Stan, bool> uslov)
        {
            List<Stan> stanovi = stanRepo.VratiPoZgradi(z.Sifra);
            return stanovi.Any(uslov);
        }
        public bool ZgradaJeUpravnikova(string sifra, string jmbgUpravnika)
        {
            Zgrada z = zgradaRepo.VratiPoSifri(sifra);
            return z != null && z.JmbgUpravnika == jmbgUpravnika;
        }

        public Zgrada VratiPoSifri(string sifra)
        {
            return zgradaRepo.VratiPoSifri(sifra);
        }
    }
}