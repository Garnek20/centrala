using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolejka
{
    //klasa definiująca drzewo turniejowe
    class DrzewoTurniejowe<D, K> : IKolejkaable<D, K> where K : IComparable
    {
        //tablica obiektów Element
        private Element<D, K>[] element { get; set; }
        //zmienna pokazujaca aktualna wielkosc tablicy 
        private int wskaznik { get; set; }
        //wielkosc maksymalna naszego drzewa
        private int pojemnosc { get; set; }

        //konstruktor domyslny, nadaje wartosci poczatkowe zmiennym
        public DrzewoTurniejowe()
        {
            wskaznik = 0;
            pojemnosc = 1000;
            element = new Element<D, K>[1000];
        }
        //konstruktor z parametrem okreslajacym wielkosc drzewa
        public DrzewoTurniejowe(int wielkosc)
        {
            wskaznik = 0;
            pojemnosc = wielkosc;
            element = new Element<D, K>[wielkosc];
        }
        //funkcja dodajaca elementy do drzewa zgodnie z algorytmem drzewa turniejowego
        public void dodaj(D dan, K klu)
        {
            wskaznik++;
            Element<D, K> nowy = new Element<D, K>(dan, klu);

            if (wskaznik == 1)
            {
                element[0] = nowy;
            }
            if (wskaznik == pojemnosc && pojemnosc < 20 * pojemnosc)
            {
                Element<D, K>[] tmp = new Element<D, K>[2 * pojemnosc];
                for (int i = 0; i < wskaznik; i++)
                    tmp[i] = element[i];
                element = tmp;
                pojemnosc *= 2;

                if (element[wskaznik - 2].zwrocKlucz().CompareTo(nowy.zwrocKlucz()) < 0)
                {
                    Element<D, K> tmp1;
                    tmp1 = element[wskaznik - 1];
                    element[wskaznik - 1] = nowy;
                    nowy = tmp1;
                }
                else
                {
                    element[wskaznik - 1] = nowy;
                }
            }
            if (wskaznik > 1 && wskaznik < pojemnosc)
            {
                element[wskaznik - 1] = nowy;
                if (element[wskaznik - 2].zwrocKlucz().CompareTo(element[wskaznik - 1].zwrocKlucz()) > 0)
                {
                    Element<D, K> tmp;
                    tmp = element[wskaznik - 2];
                    element[wskaznik - 2] = element[wskaznik - 1];
                    element[wskaznik - 1] = tmp;
                }
                else
                {
                    element[wskaznik - 1] = nowy;
                }
            }
        }
        //funkcja usuwajaca ostatni element z drzewa turniejowego
        public D usun()
        {

                wskaznik--;
                for (int i = 0; i < wskaznik; i++)
                {
                    if (element[i].zwrocKlucz().CompareTo(element[i + 1].zwrocKlucz()) > 0)
                    {
                        Element<D, K> tmp;
                        tmp = element[i + 1];
                        element[i + 1] = element[i];
                        element[i] = tmp;
                    }

                }
                return element[wskaznik].zwrocDane();

            
        }
        //funkcja zwracajaca rozmiar drzewa
        public int zwrocRozmiar()
        {
            return wskaznik;
        }
        //funkcja zwracajaca wartosc klucza
        public K klucz(int i)
        {
            return element[i].zwrocKlucz();
        }
        //funkcja zwracajaca wartosc danych
        public D dane(int i)
        {
            return element[i].zwrocDane();
        }

    }
}
