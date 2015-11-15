using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralaTelefoniczna
{
    class Zgloszenie 
    {
        //zmienna okreslajaca zapotrzebowanie kanalowe
        private int iloscKanalow;
        //maks czas oczekiwania
        private double czasOczek;
        //czas polaczenia danego Zgloszenia
        private double czas_trwania;


        public Zgloszenie(int ileKanalow, double czasOczekujacy, double czasTrwania)
        {
            this.iloscKanalow = ileKanalow;
            this.czasOczek = czasOczekujacy;
            this.czas_trwania = czasTrwania;
        }

        public int iloscKanalowZwroc()
        {
            return iloscKanalow;
        }
        public double czasOczekiwaniaZwroc()
        { return czasOczek; }

        public double czasTrwaniaZwroc()
        { return czas_trwania; }
        //dopisać zmienne do klasy statystyki




    }
}
