using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//iterator dopisac

namespace Kolejka
{
    public class ListaPosortowana<D, K>: IKolejkaable<D, K>  where K :  IComparable
    {
              
        //wskaznik do poruszania się po liście 
        private int wskaznik { get; set; } //properties comment for us
        //tablica elementow z elementami struktury
        private Element<D, K>[] element { get; set; }
        //wielkosc tablicy
        private int pojemnosc { get; set; }

        //konstruktor domyslny
        public ListaPosortowana()
        {
            wskaznik = 0;
            pojemnosc = 1000;
            element = new Element<D, K>[1000];
        }

        //konstruktor pobierajcy wielkosc tablicy
        public ListaPosortowana(int size)
        {
               wskaznik = 0;
               pojemnosc = size;
               element = new Element<D, K>[pojemnosc];
        }

        public void dodaj(D item, K _klucz)
        {
            Element<D, K> nowy = new Element<D,K>(item, _klucz);
            wskaznik ++;
            if(pojemnosc ==  0)
            {
                throw new InvalidOperationException("Kolejka nie ma miejsca, pojemnosc = 0 :-(");
            }
        
            if(wskaznik == 1)
            {
                element[0] = nowy;
                return;
            }

            if (wskaznik > 1 && wskaznik < pojemnosc)
            {
                for (int i = 0; i <= wskaznik; i++)
                {
                    if (i < wskaznik - 1)
                    {
                        if (element[i].zwrocKlucz().CompareTo(nowy.zwrocKlucz()) > 0)
                        {
                            Element<D, K> tmp;
                            tmp = element[i];
                            element[i] = nowy;
                            nowy = tmp;
                        }
                    }
                    else
                        element[wskaznik - 1] = nowy;
                }
            }
            else
            {
                Element<D,K>[] tmp = new Element<D,K>[2 * pojemnosc];
                for (int i = 0; i < wskaznik; i++)
                    tmp[i] = element[i];
                element = tmp;
                pojemnosc *= 2;

                for (int i = 0; i <= wskaznik; i++)
                {
                    if (i < wskaznik - 1)
                    {
                        if (element[i].zwrocKlucz().CompareTo(nowy.zwrocKlucz()) < 0)
                        {
                            Element<D, K> tmp1;
                            tmp1 = element[i];
                            element[i] = nowy;
                            nowy = tmp1;
                        }
                    }
                    else
                        element[wskaznik - 1] = nowy;
                }
            }
        }

        public D usun()
        {

               wskaznik--;
               return element[wskaznik].zwrocDane();
            
            
       }

        public int zwrocRozmiar()
        {
            return wskaznik;
        }
        public K klucz(int i)
        {
            return element[i].zwrocKlucz();
        }
        public D dane(int i)
        {
            return element[i].zwrocDane();
        }
        
    }


}
