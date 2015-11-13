using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kolejka
{
    public class Element<D,K> where K: IComparable
    {
        public Element(D dan, K kluc)
        {
            dane = dan;
            klucz = kluc;
        }

        private D dane;
        private K klucz;

        public K zwrocKlucz() { return klucz; }
        public D zwrocDane() { return dane; }
        
    }
}
