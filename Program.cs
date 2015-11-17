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
            Symulacja sym = new Symulacja();
            sym.symulacja();

            Console.ReadLine();
        }
    }
}
