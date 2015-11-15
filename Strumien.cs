using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralaTelefoniczna
{
    //klasa definiujaca strumien wejsciowy
    class Strumien
    {
        #region zmienne
        //zmienna okreslajaca czas nadejscia nastepnego zgloszenia
        private double czasDoNastepnego;
        //czas trwania danego polaczenie 
        private double czasPolaczenia;
        //liczba wymaganych kanalow dla danego zgloszenia 
        private int liczbaKanalow;
        //maksymalny czas oczekiwania na odbior polaczenia
        private double czasOczekiwania;
        #endregion

        public Strumien(double czasDoNastepnego_, double czasPolaczenia_, int liczbaKanalow_, double czasOczekiwania_)
        {
            this.czasDoNastepnego = czasDoNastepnego_;
            this.czasPolaczenia = czasPolaczenia_;
            this.liczbaKanalow = liczbaKanalow_;
            this.czasOczekiwania = czasOczekiwania_;
        }
        public int liczbaKanalowZwroc() { return liczbaKanalow; }
        public double czasDoNastepnegoZwroc() { return czasDoNastepnego; }
        public double czasOczekiwaniaZwroc() { return czasOczekiwania; }
        public double czasPolaczeniaZwroc() { return czasPolaczenia; }



    }
}
