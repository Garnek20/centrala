using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralaTelefoniczna
{
    class Fifo
    {
        //wskaznik do poruszania się po liście 
        private int wskaznik { get; set; } 
        //tablica elementow z elementami struktury
        private Zgloszenie[] element { get; set; }
        //wielkosc tablicy
        private int pojemnosc { get; set; }

        //konstruktor 
        public Fifo(int rozmiar) //wczytywany z pliku rozmiar kolejki w centrali
        {
            wskaznik = 0;
            pojemnosc = rozmiar;
            element = new Zgloszenie[3*rozmiar];
        }

        public void dodaj(Zgloszenie zglaszam, int ileKanlowDlaZgloszenia, int maksCzasOczekiwaniaZgloszenia)
        {
            //Zgloszenie nowy = new Zgloszenie(ileKanlowDlaZgloszenia, maksCzasOczekiwaniaZgloszenia);
            wskaznik ++;
            if(pojemnosc ==  0)
            {
                throw new InvalidOperationException("Kolejka nie ma miejsca, pojemnosc = 0 :-(");
            }
        
            if(wskaznik == 1)
            {
                element[0] = zglaszam;
                return;
            }

            if (wskaznik > 1 && wskaznik < pojemnosc)
            {
                 element[wskaznik - 1] = zglaszam;
            }
            else
            {
                return;
            }
        }

        public Zgloszenie usun()
        {
            if (wskaznik == 0)
            {
                return null; //zwraca nulla, gdy nie ma czego usunac. Nalezy to uwzgledniac w symulacji
            }
            else
            {
               wskaznik--;
               return element[wskaznik];
            }
            
       }

        public int zwrocRozmiar()
        {
            return wskaznik;
        }

        
    }
}

