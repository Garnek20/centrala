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
        private TimeSpan sredniCzasObslugi;
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
            sredniCzasObslugi = TimeSpan.Zero;
            counterZajetosciWiazki = 0;
            counterZajetosciKolejki = 0;
            counterObslugi = 0;
        }
        public void obliczPstwoOdrzucenia(int liczbaZgloszen, int liczbaOdrzuconych)
        {
            pstwoOdrzucenia = (double)liczbaOdrzuconych / liczbaZgloszen;
        }
        public double pstwoOdrzuceniaZwroc() { return pstwoOdrzucenia; }
       
        public void zwiekszZajetoscWiazki()
        {
            counterZajetosciWiazki++;

        }
        public double zajetoscWiazkiZwroc(int liczbaZgloszenWsystemie) 
        {
            if (counterZajetosciWiazki != 0)
            {
                zajetoscWiazki = (double)counterZajetosciWiazki / liczbaZgloszenWsystemie;
                return zajetoscWiazki;
            }
            else
                return 0;
        }

        public void zwiekszZajetoscKolejki()
        {
            counterZajetosciKolejki++;
        }
        public double zajetoscKolejkiZwroc(int liczbaZgloszenWKolejce)
        {
            if (counterZajetosciKolejki != 0)
            {
                zajetoscKolejki = (double)counterZajetosciKolejki/liczbaZgloszenWKolejce;
                return zajetoscKolejki;
            }
            else
                return 0;
        }

        public void zwiekszCzasObslugi(TimeSpan aktualnyCzasObslugi)
        {
            counterObslugi++;
            sredniCzasObslugi = sredniCzasObslugi + aktualnyCzasObslugi;
        }
        public double sredniCzasObslugiZwroc()
        {
            double temp;
            if (counterObslugi != 0)
            {
                temp = sredniCzasObslugi.TotalMilliseconds;
                temp = temp/ counterObslugi;
                return temp;
            }
            else
                return 0;
        }
    }
}
