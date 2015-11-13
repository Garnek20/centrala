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
        //czas polaczenia w milisekundach
        int czasPolaczenia;
        //czas pomiedzy zgloszeniami
        int czasPomiedzyZgloszeniami;
        //sciezka do pliku.txt
        string sciezka;
        //parametry do rozkladow lambda
        double[] lambda;
        //nazwa poszczegolnego rozkladu
        string[] nazwaRozkladu;
        //ilosc rozkladow
        int iloscRozkladow;
        

        //funkcja generowania czasu polaczenia (jesli ktory == 1) lub czasu pomiedzy zgloszeniami (w pozostalych przypadkach)
        public int generowanieCzasutrwaniaPolaczenia(double lambda, int ktory)
        {
            double EX, VARX;
            int lewa, prawa;
            EX = 1 / lambda;
            VARX = 1 / (lambda * lambda);
            Random random = new Random();

            //mnozenie w celu uzyskania milisekund
            lewa = (int)((EX - VARX)*1000);
            

            if (lewa < 0)
            {
                lewa =(int)0;
                prawa = (int)((EX + lambda*VARX) * 1000);

            }
            else
            {
                prawa = (int)((EX + VARX) * 1000);
            }
            
            
            if(ktory == 1)
            {
                czasPolaczenia = random.Next(lewa, prawa);
                return czasPolaczenia;
            }
            else
            {
                czasPomiedzyZgloszeniami = random.Next(lewa, prawa);
                return czasPomiedzyZgloszeniami;
            }
        }
/*
        public void wczytywanie(string lokalizacja)
        {
            sciezka = lokalizacja;
           
 
            string linia;
            string[] temp;
            int licznik=0, pomoc1= 0, pomoc2 = 0;

            System.IO.StreamReader file = new System.IO.StreamReader(sciezka);
  
            //
            while((linia = file.ReadLine()) != null)
            {
             if(linia[0]!='#' && linia[0]!='K' && linia[0]!=' ')
             {
                licznik++;
                 if(licznik == 1)
                 {
                    iloscRozkladow=(int)linia[2];
                    nazwaRozkladu = new string[iloscRozkladow];
                    lambda = new double[iloscRozkladow];
                 }
                 else if((licznik%2) == 0)
                 {
                     nazwaRozkladu[pomoc1]=linia;
                     pomoc1++;
                 }
                 else
                 {
                     int[] x;
                     char z;
                     int dlugosc = linia.Length;
                     x = new int[dlugosc -1];

                     for(int i = 0; i <dlugosc; i++)
                     {
                         char tmp = linia[i];
                         if (tmp != ',')
                             x[i] = tmp;
                         else
                             z = ',';
                     }
                     lambda[pomoc2] = x[;
                     pomoc2++;
                 }
             }  
            }

            file.Close();

            if()
        }
  */     
    }
}
