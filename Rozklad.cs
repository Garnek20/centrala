using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace CentralaTelefoniczna
{
    class Rozklad
    {

        private double lambda_;
        private string nazwa_;

        public Rozklad(double lambda, string nazwa)
        {
            this.lambda_ = lambda;
            this.nazwa_ = nazwa;
        }

        public double zwrocRozklad(double x)
        {
            return -(Math.Log(1 - x) / lambda_);
        }



    }
}
