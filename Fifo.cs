﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralaTelefoniczna
{
    class Fifo
    {
        //wskaznik do poruszania się po liście 
        private int wskaznik;
        //tablica elementow z elementami struktury
        private Zgloszenie[] element;
        //wielkosc tablicy
        private int pojemnosc;

        //konstruktor 
        public Fifo(int rozmiar) //wczytywany z pliku rozmiar kolejki w centrali
        {
            wskaznik = 0;
            pojemnosc = rozmiar;
            element = new Zgloszenie[3 * rozmiar];
        }

        public void dodaj(Zgloszenie zglaszam)
        {
            //Zgloszenie nowy = new Zgloszenie(ileKanlowDlaZgloszenia, maksCzasOczekiwaniaZgloszenia);
            wskaznik++;
            if (pojemnosc == 0)
            {
                wskaznik--;
                return;
            }

            if (wskaznik == 1)
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
                wskaznik--;
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
            
        public Zgloszenie zwrocElementKolejki(int wsk)
        { return element[wsk]; }

        public void usunDanyElement(int wsk)
        {
            Zgloszenie[] zgloszenieNowe = new Zgloszenie[3 * pojemnosc];
            int j = 0;

            while (j != wsk)
            {
                zgloszenieNowe[j] = element[j];
                j++;
            }
            while (j != pojemnosc - 1)
            {
                zgloszenieNowe[j] = element[wsk + 1];
                j++;
            }
            element = zgloszenieNowe;
            wskaznik--;
            
        }
    }
}
