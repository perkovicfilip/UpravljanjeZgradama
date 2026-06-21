namespace UpravljanjeZgradama.Model
{
    public abstract class Korisnik
    {
        public string Jmbg { get; set; }
        public string Email { get; set; }
        public string Lozinka { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string MobilniTelefon { get; set; }

        public bool ProveriLozinku(string unetaLozinka)
        {
            return Lozinka == unetaLozinka;
        }
    }
}