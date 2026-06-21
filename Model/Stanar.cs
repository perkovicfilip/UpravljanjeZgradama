using System.Collections.Generic;

namespace UpravljanjeZgradama.Model
{
    public class Stanar : Korisnik
    {
        public List<Zahtev> Zahtevi { get; set; } = new List<Zahtev>();
    }
}