using System;
using UpravljanjeZgradama.Model.Enums;

namespace UpravljanjeZgradama.Model
{
    public class Zahtev
    {
        public string StanarJmbg { get; set; }
        public string SifraZgrade { get; set; }
        public int BrojStana { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public StatusZahteva Status { get; set; }
        public string RazlogOdbijanja { get; set; }

        public void Potvrdi()
        {
            Status = StatusZahteva.Potvrdjen;
        }

        public void Odbij(string razlog)
        {
            Status = StatusZahteva.Odbijen;
            RazlogOdbijanja = razlog;
        }

        public bool JeNaCekanju()
        {
            return Status == StatusZahteva.CekaOdobrenje;
        }
    }
}