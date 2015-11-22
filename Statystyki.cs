using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralaTelefoniczna
{
    class Statystyki
    {
        #region zmienne
        //zmienna okreslajaca prawdopodobienstwo odrzucenia zgloszenia
        private double pstwoOdrzucenia;
        //zmienna okreslajaca srednia zajetosc wiazki (kanalow)
        private double zajetoscWiazki;
        //srednia zajetosc kolejki
        private double zajetoscKolejki;
        //sredni czas obslugi systemu (pakietow, tylko pomyslnie obsluzonych)
        private double sredniCzasObslugi;
        //licznik zajetosci
        private int counterZajetosciWiazki;
        //licznik zajetosci kolejki
        private int counterZajetosciKolejki;
        //licznik obsluzonych zgloszen
        private int counterObslugi;
        #endregion


        public Statystyki()
        {
            pstwoOdrzucenia = 0;
            zajetoscWiazki = 0;
            zajetoscKolejki = 0;
            sredniCzasObslugi = 0;
            counterZajetosciWiazki = 0;
            counterZajetosciKolejki = 0;
            counterObslugi = 0;
        }
        public void obliczPstwoOdrzucenia(int liczbaZgloszen, int liczbaOdrzuconych)
        {
            pstwoOdrzucenia = (double)liczbaOdrzuconych / liczbaZgloszen;
        }
        public double pstwoOdrzuceniaZwroc() { return pstwoOdrzucenia; }
       
        public void zwiekszZajetoscWiazki(double aktualnaZajetosc)
        {
            counterZajetosciWiazki++;
            zajetoscWiazki +=aktualnaZajetosc;
        }
        public double zajetoscWiazkiZwroc() {
            if (counterZajetosciWiazki != 0)
            {
                zajetoscWiazki = zajetoscWiazki / counterZajetosciWiazki;
                return zajetoscWiazki;
            }
            else
                return 0;
        }

        public void zwiekszZajetoscKolejki(double aktualnaZajetosc)
        {
            counterZajetosciKolejki++;
            zajetoscKolejki += aktualnaZajetosc;
        }
        public double zajetoscKolejkiZwroc()
        {
            if (counterZajetosciKolejki != 0)
            {
                zajetoscKolejki = zajetoscKolejki / counterZajetosciKolejki;
                return zajetoscKolejki;
            }
            else
                return 0;
        }

        public void zwiekszCzasObslugi(double aktualnyCzasObslugi)
        {
            counterObslugi++;
            sredniCzasObslugi += aktualnyCzasObslugi;
        }
        public double sredniCzasObslugiZwroc()
        {
            if (counterObslugi != 0)
            {
                sredniCzasObslugi = sredniCzasObslugi / counterObslugi;
                return sredniCzasObslugi;
            }
            else
                return 0;
        }
    }
}
