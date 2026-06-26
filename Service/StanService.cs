using UpravljanjeZgradama.Model;
using UpravljanjeZgradama.Repository;

namespace UpravljanjeZgradama.Service
{
    public class StanService
    {
        private StanRepository stanRepo = new StanRepository();

        public List<Stan> VratiSvePoZgradi(string sifraZgrade)
        {
            return stanRepo.VratiPoZgradi(sifraZgrade);
        }

        public bool UnesiStan(Stan stan)
        {
            if (PostojiStan(stan.SifraZgrade, stan.BrojStana))
                return false;
            stanRepo.Dodaj(stan);
            return true;
        }

        public Stan VratiStan(string sifraZgrade, int brojStana)
        {
            List<Stan> stanovi = stanRepo.VratiPoZgradi(sifraZgrade);
            foreach (Stan s in stanovi)
                if (s.BrojStana == brojStana)
                    return s;
            return null;
        }

        public bool PostojiStan(string sifraZgrade, int brojStana)
        {
            return VratiStan(sifraZgrade, brojStana) != null;
        }
    }
}