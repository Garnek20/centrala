using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralaTelefoniczna
{
    class Zgloszenie
    {
        #region zmienne
        //zmienna okreslajaca zapotrzebowanie kanalowe
        private int iloscKanalow;
        //czas polaczenia danego Zgloszenia
        private double czasTrwania;
        //maksymalny czas oczekiwania danego zgloszenia w FIFO
        private double maksCzasOczekiwania;
        #endregion
        public double maks_czas_oczekiwania { get { return maksCzasOczekiwania; } set { maksCzasOczekiwania = value; } }
        public int ilosc_kanalow { get { return iloscKanalow; } set { iloscKanalow = value; } }
        public double czas_trwania { get { return czasTrwania; } set { czasTrwania = value; } }

        public Zgloszenie()
        {
            iloscKanalow = 0;
            czasTrwania = 0;
            maksCzasOczekiwania = 0;
        }
        public int iloscKanalowZwroc()
        {
            return iloscKanalow;
        }
        public double czasTrwaniaZwroc()
        { return czas_trwania; }
        public double maksCzasOczekiwaniaZwroc()
        { return maksCzasOczekiwania; }





    }
}
