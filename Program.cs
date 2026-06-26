using UpravljanjeZgradama.Model;
using UpravljanjeZgradama.Model.Enums;
using UpravljanjeZgradama.Service;

namespace UpravljanjeZgradama
{
    internal class Program
    {
        private static KorisnikService korisnikService = new KorisnikService();
        private static ZgradaService zgradaService = new ZgradaService();
        private static StanService stanService = new StanService();
        private static ZahtevService zahtevService = new ZahtevService();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\n===== UPRAVLJANJE ZGRADAMA =====");
                Console.WriteLine("1. Prijava");
                Console.WriteLine("2. Registracija (stanar)");
                Console.WriteLine("0. Izlaz");
                string izbor = UnesiTekst("Izbor: ");

                if (izbor == "1") Prijava();
                else if (izbor == "2") RegistracijaStanara();
                else if (izbor == "0") break;
                else Poruka("Nepoznat izbor.");
            }
        }

        // ===================== POMOCNE METODE ZA UNOS  =====================

        private static string UnesiTekst(string poruka)
        {
            while (true)
            {
                Console.Write(poruka);
                string unos = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(unos))
                    return unos.Trim();
                Console.WriteLine("Unos ne sme biti prazan.");
            }
        }

        private static string UnesiTekstMozePrazno(string poruka)
        {
            Console.Write(poruka);
            return Console.ReadLine().Trim();
        }

        private static int UnesiBroj(string poruka)
        {
            while (true)
            {
                Console.Write(poruka);
                string unos = Console.ReadLine();
                if (int.TryParse(unos, out int broj) && broj >= 0)
                    return broj;
                Console.WriteLine("Unesite ispravan ceo broj (>= 0).");
            }
        }

        private static string UnesiJmbg(string poruka)
        {
            while (true)
            {
                string unos = UnesiTekst(poruka);
                if (unos.Length == 13 && SveSuCifre(unos))
                    return unos;
                Console.WriteLine("JMBG mora imati tačno 13 cifara.");
            }
        }

        private static string UnesiEmail(string poruka)
        {
            while (true)
            {
                string unos = UnesiTekst(poruka);
                if (unos.Contains("@") && unos.Contains(".") &&
                    unos.IndexOf("@") < unos.LastIndexOf("."))
                    return unos;
                Console.WriteLine("Email mora biti u formatu ime@domen.com");
            }
        }

        private static string UnesiTelefon(string poruka)
        {
            while (true)
            {
                string unos = UnesiTekst(poruka);
                if (unos.StartsWith("0") && unos.Length >= 9 && unos.Length <= 10 && SveSuCifre(unos))
                    return unos;
                Console.WriteLine("Telefon mora počinjati sa 0 i imati 9-10 cifara.");
            }
        }

        private static bool SveSuCifre(string tekst)
        {
            foreach (char c in tekst)
                if (!char.IsDigit(c))
                    return false;
            return true;
        }

        private static void Poruka(string tekst)
        {
            Console.WriteLine(tekst);
        }

        private static void Pauza()
        {
            Console.WriteLine("Pritisnite taster za nastavak...");
            Console.ReadKey();
        }

        // ===================== PRIJAVA =====================

        private static void Prijava()
        {
            string email = UnesiTekst("Email: ");
            string lozinka = UnesiTekst("Lozinka: ");

            Korisnik korisnik = korisnikService.Prijava(email, lozinka);
            if (korisnik == null)
            {
                Poruka("Neuspešna prijava — pogrešan email ili lozinka.");
                return;
            }

            Console.WriteLine($"Dobrodošli, {korisnik.Ime}!");

            if (korisnik is Administrator)
                AdministratorMeni();
            else if (korisnik is Upravnik)
                UpravnikMeni((Upravnik)korisnik);
            else if (korisnik is Stanar)
                StanarMeni((Stanar)korisnik);
        }

        // ===================== ZAJEDNICKO =====================

        private static void PrikaziSveZgrade()
        {
            string odg = UnesiTekstMozePrazno("Sortiraj po broju spratova? (d/n): ");
            List<Zgrada> zgrade = (odg == "d")
                ? zgradaService.SortiranePoSpratovima()
                : zgradaService.VratiVidljive();
            IspisiZgrade(zgrade);
        }

        private static void PretragaZgrada()
        {
            Console.WriteLine("\nPretraga po: 1-adresa  2-naselje  3-spratovi  4-stanovi");
            string izbor = UnesiTekst("Izbor: ");

            List<Zgrada> rezultat = new List<Zgrada>();

            if (izbor == "1")
                rezultat = zgradaService.PretragaPoAdresi(UnesiTekst("Unesi deo adrese: "));
            else if (izbor == "2")
                rezultat = zgradaService.PretragaPoNaselju(UnesiTekst("Unesi deo naselja: "));
            else if (izbor == "3")
                rezultat = zgradaService.PretragaPoSpratovima(UnesiBroj("Broj spratova: "));
            else if (izbor == "4")
                rezultat = PretragaPoStanovima();
            else
            {
                Poruka("Nepoznat izbor.");
                return;
            }

            IspisiZgrade(rezultat);
        }

        private static List<Zgrada> PretragaPoStanovima()
        {
            Console.WriteLine("1-broj soba  2-broj stanara  3-soba i stanara (& ili |)");
            string izbor = UnesiTekst("Izbor: ");

            if (izbor == "1")
                return zgradaService.PretragaPoBrojuSoba(UnesiBroj("Broj soba: "));
            if (izbor == "2")
                return zgradaService.PretragaPoBrojuStanara(UnesiBroj("Broj stanara: "));
            if (izbor == "3")
            {
                string unos = UnesiTekst("Unos (npr. 2 & 3  ili  2 | 3): ");
                if (!unos.Contains("&") && !unos.Contains("|"))
                {
                    Poruka("Morate koristiti & ili | operator.");
                    return new List<Zgrada>();
                }
                bool iOperator = unos.Contains("&");
                string[] delovi = unos.Split(iOperator ? '&' : '|');
                if (delovi.Length != 2 ||
                    !int.TryParse(delovi[0].Trim(), out int soba) ||
                    !int.TryParse(delovi[1].Trim(), out int stanara))
                {
                    Poruka("Neispravan format. Primer: 2 & 3");
                    return new List<Zgrada>();
                }
                return zgradaService.PretragaPoSobamaIStanarima(soba, stanara, iOperator);
            }

            Poruka("Nepoznat izbor.");
            return new List<Zgrada>();
        }

        private static void IspisiZgrade(List<Zgrada> zgrade)
        {
            if (zgrade.Count == 0)
            {
                Poruka("Nema zgrada za prikaz.");
                return;
            }
            Console.WriteLine("\n--- Zgrade ---");
            foreach (Zgrada z in zgrade)
                Console.WriteLine($"[{z.Sifra}] {z.Ulica} {z.Broj}, {z.Naselje}, " +
                                  $"{z.Grad} | spratova: {z.BrojSpratova} | status: {z.Status}");
        }

        // ===================== ADMINISTRATOR =====================

        private static void AdministratorMeni()
        {
            while (true)
            {
                Console.WriteLine("\n----- ADMINISTRATOR -----");
                Console.WriteLine("1. Unos zgrade");
                Console.WriteLine("2. Registracija upravnika");
                Console.WriteLine("3. Prikaz svih zgrada");
                Console.WriteLine("4. Pretraga zgrada");
                Console.WriteLine("0. Odjava");
                string izbor = UnesiTekst("Izbor: ");

                if (izbor == "1") UnosZgrade();
                else if (izbor == "2") RegistracijaUpravnika();
                else if (izbor == "3") PrikaziSveZgrade();
                else if (izbor == "4") PretragaZgrada();
                else if (izbor == "0") break;
                else Poruka("Nepoznat izbor.");
            }
        }

        private static void UnosZgrade()
        {
            Console.WriteLine("\n--- Unos nove zgrade ---");
            Zgrada zgrada = new Zgrada
            {
                Sifra = UnesiTekst("Šifra: "),
                Ulica = UnesiTekst("Ulica: "),
                Broj = UnesiBroj("Broj: "),
                Naselje = UnesiTekst("Naselje: "),
                Grad = UnesiTekst("Grad: "),
                Drzava = UnesiTekst("Država: "),
                BrojSpratova = UnesiBroj("Broj spratova: "),
                JmbgUpravnika = UnesiJmbg("JMBG upravnika: ")
            };

            if (!korisnikService.JmbgZauzet(zgrada.JmbgUpravnika))
            {
                Poruka("UPOZORENJE: ne postoji upravnik sa tim JMBG-om. Zgrada se svejedno unosi.");
            }

            bool uspeh = zgradaService.UnesiZgradu(zgrada);
            Poruka(uspeh
                ? "Zgrada uneta. Čeka da je upravnik odobri."
                : "Greška: zgrada sa tom šifrom već postoji.");
            Pauza();
        }

        private static void RegistracijaUpravnika()
        {
            Console.WriteLine("\n--- Registracija upravnika ---");
            Upravnik upravnik = new Upravnik
            {
                Jmbg = UnesiJmbg("JMBG: "),
                Email = UnesiEmail("Email: "),
                Lozinka = UnesiTekst("Lozinka: "),
                Ime = UnesiTekst("Ime: "),
                Prezime = UnesiTekst("Prezime: "),
                MobilniTelefon = UnesiTelefon("Mobilni telefon: ")
            };

            bool uspeh = korisnikService.RegistrujUpravnika(upravnik);
            Poruka(uspeh ? "Upravnik registrovan." : "Greška: email ili JMBG već postoji.");
            Pauza();
        }

        // ===================== UPRAVNIK =====================

        private static void UpravnikMeni(Upravnik upravnik)
        {
            while (true)
            {
                Console.WriteLine("\n----- UPRAVNIK -----");
                Console.WriteLine("1. Moje zgrade (odobravanje/odbijanje)");
                Console.WriteLine("2. Unos stana");
                Console.WriteLine("3. Obrada zahteva");
                Console.WriteLine("4. Prikaz svih zgrada");
                Console.WriteLine("5. Pretraga zgrada");
                Console.WriteLine("0. Odjava");
                string izbor = UnesiTekst("Izbor: ");

                if (izbor == "1") ObradaZgrada(upravnik);
                else if (izbor == "2") UnosStana(upravnik);
                else if (izbor == "3") ObradaZahteva(upravnik);
                else if (izbor == "4") PrikaziSveZgrade();
                else if (izbor == "5") PretragaZgrada();
                else if (izbor == "0") break;
                else Poruka("Nepoznat izbor.");
            }
        }

        private static List<Zgrada> PrikaziMojeZgrade(Upravnik upravnik)
        {
            List<Zgrada> mojeZgrade = zgradaService.VratiZgradeUpravnika(upravnik.Jmbg);
            if (mojeZgrade.Count == 0)
            {
                Poruka("Nemate dodeljenih zgrada.");
                return mojeZgrade;
            }
            foreach (Zgrada z in mojeZgrade)
                Console.WriteLine($"[{z.Sifra}] {z.Ulica} {z.Broj}, {z.Naselje} | status: {z.Status}");
            return mojeZgrade;
        }

        private static void ObradaZgrada(Upravnik upravnik)
        {
            if (PrikaziMojeZgrade(upravnik).Count == 0) { Pauza(); return; }

            string sifra = UnesiTekstMozePrazno("Šifra zgrade za obradu (prazno = nazad): ");
            if (string.IsNullOrEmpty(sifra)) return;

            if (!zgradaService.ZgradaJeUpravnikova(sifra, upravnik.Jmbg))
            {
                Poruka("Greška: ta zgrada nije vaša ili ne postoji.");
                Pauza();
                return;
            }

            string akcija = UnesiTekst("1-odobri  2-odbij: ");
            if (akcija == "1")
            {
                zgradaService.OdobriZgradu(sifra);
                Poruka("Zgrada odobrena.");
            }
            else if (akcija == "2")
            {
                zgradaService.OdbijZgradu(sifra);
                Poruka("Zgrada odbijena.");
            }
            else Poruka("Nepoznata akcija.");
            Pauza();
        }

        private static void UnosStana(Upravnik upravnik)
        {
            if (PrikaziMojeZgrade(upravnik).Count == 0) { Pauza(); return; }

            string sifra = UnesiTekst("Šifra zgrade: ");
            if (!zgradaService.ZgradaJeUpravnikova(sifra, upravnik.Jmbg))
            {
                Poruka("Greška: ta zgrada nije vaša ili ne postoji.");
                Pauza();
                return;
            }

            Stan stan = new Stan
            {
                SifraZgrade = sifra,
                BrojStana = UnesiBroj("Broj stana: "),
                Opis = UnesiTekst("Opis: "),
                BrojSoba = UnesiBroj("Broj soba: "),
                MaxStanara = UnesiBroj("Max stanara: ")
            };

            bool uspeh = stanService.UnesiStan(stan);
            Poruka(uspeh ? "Stan unet." : "Greška: stan sa tim brojem već postoji u toj zgradi.");
            Pauza();
        }

        private static void ObradaZahteva(Upravnik upravnik)
        {
            if (PrikaziMojeZgrade(upravnik).Count == 0) { Pauza(); return; }

            string sifra = UnesiTekst("Šifra zgrade: ");
            if (!zgradaService.ZgradaJeUpravnikova(sifra, upravnik.Jmbg))
            {
                Poruka("Greška: ta zgrada nije vaša ili ne postoji.");
                Pauza();
                return;
            }

            List<Zahtev> naCekanju = zahtevService.ZahteviZgradePoStatusu(sifra, StatusZahteva.CekaOdobrenje);
            if (naCekanju.Count == 0)
            {
                Poruka("Nema zahteva na čekanju.");
                Pauza();
                return;
            }

            for (int i = 0; i < naCekanju.Count; i++)
            {
                Zahtev z = naCekanju[i];
                Console.WriteLine($"{i + 1}. Stanar {z.StanarJmbg} | stan {z.BrojStana} | {z.DatumKreiranja:dd.MM.yyyy}");
            }

            int izbor = UnesiBroj("Redni broj zahteva: ") - 1;
            if (izbor < 0 || izbor >= naCekanju.Count)
            {
                Poruka("Nepostojeći redni broj.");
                Pauza();
                return;
            }

            Zahtev izabrani = naCekanju[izbor];
            string akcija = UnesiTekst("1-potvrdi  2-odbij: ");
            if (akcija == "1")
            {
                zahtevService.PotvrdiZahtev(izabrani);
                Poruka("Zahtev potvrđen.");
            }
            else if (akcija == "2")
            {
                string razlog = UnesiTekst("Razlog odbijanja: ");
                zahtevService.OdbijZahtev(izabrani, razlog);
                Poruka("Zahtev odbijen.");
            }
            else Poruka("Nepoznata akcija.");
            Pauza();
        }

        // ===================== STANAR =====================

        private static void RegistracijaStanara()
        {
            Console.WriteLine("\n--- Registracija stanara ---");
            Stanar stanar = new Stanar
            {
                Jmbg = UnesiJmbg("JMBG: "),
                Email = UnesiEmail("Email: "),
                Lozinka = UnesiTekst("Lozinka: "),
                Ime = UnesiTekst("Ime: "),
                Prezime = UnesiTekst("Prezime: "),
                MobilniTelefon = UnesiTelefon("Mobilni telefon: ")
            };

            bool uspeh = korisnikService.RegistrujStanara(stanar);
            Poruka(uspeh ? "Registracija uspešna. Možete se prijaviti."
                         : "Greška: email ili JMBG već postoji.");
            Pauza();
        }

        private static void StanarMeni(Stanar stanar)
        {
            while (true)
            {
                Console.WriteLine("\n----- STANAR -----");
                Console.WriteLine("1. Prikaz svih zgrada");
                Console.WriteLine("2. Pretraga zgrada");
                Console.WriteLine("3. Podnošenje zahteva");
                Console.WriteLine("4. Moji zahtevi");
                Console.WriteLine("5. Povlačenje zahteva");
                Console.WriteLine("0. Odjava");
                string izbor = UnesiTekst("Izbor: ");

                if (izbor == "1") PrikaziSveZgrade();
                else if (izbor == "2") PretragaZgrada();
                else if (izbor == "3") PodnosenjeZahteva(stanar);
                else if (izbor == "4") MojiZahtevi(stanar);
                else if (izbor == "5") PovlacenjeZahteva(stanar);
                else if (izbor == "0") break;
                else Poruka("Nepoznat izbor.");
            }
        }

        private static void PodnosenjeZahteva(Stanar stanar)
        {
            IspisiZgrade(zgradaService.VratiVidljive());
            string sifra = UnesiTekst("Šifra zgrade: ");

            Zgrada zgrada = zgradaService.VratiPoSifri(sifra);
            if (zgrada == null || !zgrada.JeVidljiva())
            {
                Poruka("Greška: ne postoji vidljiva zgrada sa tom šifrom.");
                Pauza();
                return;
            }

            int brojStana = UnesiBroj("Broj stana: ");

            if (zahtevService.StanZauzet(sifra, brojStana))
                Poruka("UPOZORENJE: za ovaj stan je već učlanjen stanar (popunjen je). Proverite broj stana.");

            string potvrda = UnesiTekst("Potvrdi kreiranje zahteva? (d/n): ");
            if (potvrda == "d")
            {
                zahtevService.KreirajZahtev(stanar.Jmbg, sifra, brojStana);
                Poruka("Zahtev kreiran. Status: čeka odobrenje.");
            }
            else Poruka("Otkazano.");
            Pauza();
        }

        private static void MojiZahtevi(Stanar stanar)
        {
            List<Zahtev> zahtevi = zahtevService.ZahteviStanara(stanar.Jmbg);
            if (zahtevi.Count == 0)
            {
                Poruka("Nemate zahteva.");
                Pauza();
                return;
            }
            foreach (Zahtev z in zahtevi)
            {
                string red = $"Zgrada {z.SifraZgrade} | stan {z.BrojStana} | status: {z.Status}";
                if (z.Status == StatusZahteva.Odbijen)
                    red += $" | razlog: {z.RazlogOdbijanja}";
                Console.WriteLine(red);
            }
            Pauza();
        }

        private static void PovlacenjeZahteva(Stanar stanar)
        {
            List<Zahtev> zahtevi = zahtevService.ZahteviStanara(stanar.Jmbg);
            List<Zahtev> naCekanju = new List<Zahtev>();
            foreach (Zahtev z in zahtevi)
                if (z.JeNaCekanju())
                    naCekanju.Add(z);

            if (naCekanju.Count == 0)
            {
                Poruka("Nemate zahteva na čekanju.");
                Pauza();
                return;
            }

            for (int i = 0; i < naCekanju.Count; i++)
                Console.WriteLine($"{i + 1}. Zgrada {naCekanju[i].SifraZgrade} | stan {naCekanju[i].BrojStana}");

            int izbor = UnesiBroj("Redni broj za povlačenje: ") - 1;
            if (izbor < 0 || izbor >= naCekanju.Count)
            {
                Poruka("Nepostojeći redni broj.");
                Pauza();
                return;
            }

            zahtevService.PovuciZahtev(naCekanju[izbor]);
            Poruka("Zahtev povučen.");
            Pauza();
        }
    }
}