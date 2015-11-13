using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CentralaTelefoniczna
{
    class Program
    {
        static void Main(string[] args)
        {
            int pomocnicza;
           // Rozklad rozklad = new Rozklad();
            
          //  pomocnicza = rozklad.generowanieCzasutrwaniaPolaczenia(0.5,1);
           // Console.WriteLine(pomocnicza);
            Symulacja sym = new Symulacja();
            sym.wczytywanie();

            Console.ReadLine();
        }
    }
}
