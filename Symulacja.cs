using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CentralaTelefoniczna
{
    class Symulacja
    {
        #region zmienne
        //czas polaczenia w milisekundach
        double[] czasPolaczenia;
        //czas pomiedzy zgloszeniami
        double[] czasPomiedzyZgloszeniami;
        //sciezka do pliku.txt
        string sciezka;
        //parametry do rozkladow lambda
        double[] lambda;
        //nazwa poszczegolnego rozkladu
        string[] nazwaRozkladu;
        //ilosc rozkladow
        int iloscRozkladow;
        //nazwa systemu
        string nazwaSystemu;
        //liczba kanalow na zlaczu wyjsciowym
        int liczbaKanalow;
        //rozmiar kolejki w centrali
        int rozmiarKolejki;
        //liczba strumieni
        int liczbaStrumieni;
        //nazwy strumieni
        string[] nazwaStrumienia;
        //maksymalny czas oczekiwania
        double[] maxCzasOczekiwania;
        //liczba kanalow poszczegolnego zgloszenia
        int[] liczbaKanalowZgloszenia;
        #endregion
        public Symulacja()
        {

        }
        public void wczytywanie()
        {
            string[] wyrazy;
            string linia = "";

                Console.WriteLine("Podaj sciezke do pliku");
                sciezka = Console.ReadLine();
                if (sciezka[0] == '\"') sciezka = sciezka.Substring(1, sciezka.Length - 2);
                
                StreamReader read;
                read = new StreamReader(sciezka);
                #region nazwa systemu
                while (linia.Length < 2 || linia[0] == '#')
                {
                    linia = read.ReadLine();
                }
                wyrazy = linia.Split(' ');
                if (wyrazy[0] != "SYSTEM" || wyrazy[2] == "")
                    throw (new Exception("Zly format pliku :( "));
                else
                nazwaSystemu = wyrazy[2];
                #endregion

                //nazwa juz jest wczytana

                #region liczba Kanalow
                linia = " ";
                while (linia.Length < 2 || linia[0] == '#')
                {
                    linia = read.ReadLine();
                }
                wyrazy = linia.Split(' ');
                if (wyrazy[0] != "KANALY" || wyrazy[2] == "")
                    throw (new Exception("Zly format pliku :( "));
                else
                    liczbaKanalow = int.Parse(wyrazy[2]);
                #endregion
                //liczba kanalow tez już jest za nami
                #region rozmiar kolejki
                linia = " ";
                while (linia.Length < 2 || linia[0] == '#')
                {
                    linia = read.ReadLine();
                }
                wyrazy = linia.Split(' ');
                if (wyrazy[0] != "KOLEJKA" || wyrazy[2] == "")
                    throw (new Exception("Zly format pliku :( "));
                else
                    rozmiarKolejki = int.Parse(wyrazy[2]);
                #endregion
                //rozmiar kolejki wczytany

                #region ilosc rozkladow
                linia = " ";
                while (linia.Length < 2 || linia[0] == '#')
                {
                    linia = read.ReadLine();
                }
                wyrazy = linia.Split(' ');
                if (wyrazy[0] != "ROZKLADY" || wyrazy[2] == "")
                    throw (new Exception("Zly format pliku :( "));
                else
                    iloscRozkladow = int.Parse(wyrazy[2]);
                #endregion

                #region rozklady
                nazwaRozkladu = new string[iloscRozkladow];
                lambda = new double[iloscRozkladow];

                //ilosc rozkladow wczytana
                for (int i = 0; i < iloscRozkladow; i++)
                {

                    linia = " ";
                    while (linia.Length < 2 || linia[0] == '#')
                    {
                        linia = read.ReadLine();
                    }
                    wyrazy = linia.Split(' ');
                    if (wyrazy[0] != "NAZWA" || wyrazy[2] == "")
                        throw (new Exception("Zly format pliku :( "));
                    else
                        nazwaRozkladu[i] = wyrazy[2];


                    linia = " ";
                    while (linia.Length < 2 || linia[0] == '#')
                    {
                        linia = read.ReadLine();
                    }
                    wyrazy = linia.Split(' ');
                    if (wyrazy[0] != "LAMBDA" || wyrazy[2] == "")
                        throw (new Exception("Zly format pliku :( "));
                    else
                        lambda[i] = double.Parse(wyrazy[2]);
                }
                #endregion
                //dane dotyczace rozkladow wczytane

                #region liczba strumieni
                linia = " ";
                while (linia.Length < 2 || linia[0] == '#')
                {
                    linia = read.ReadLine();
                }
                wyrazy = linia.Split(' ');
                if (wyrazy[0] != "STRUMIENIE" || wyrazy[2] == "")
                    throw (new Exception("Zly format pliku :( "));
                else
                    liczbaStrumieni = int.Parse(wyrazy[2]);
                #endregion
                //liczba strumieni wczytana

                #region dane strumienia
                nazwaStrumienia = new string[liczbaStrumieni];
                liczbaKanalowZgloszenia = new int[liczbaStrumieni];
                maxCzasOczekiwania = new double[liczbaStrumieni];
                czasPolaczenia = new double[liczbaStrumieni];
                czasPomiedzyZgloszeniami = new double[liczbaStrumieni];

            for(int i=0;i < liczbaStrumieni; i++)
            {
                linia = " ";
                while (linia.Length < 2 || linia[0] == '#')
                {
                    linia = read.ReadLine();
                }
                wyrazy = linia.Split(' ');
                if (wyrazy[0] != "NAZWA" || wyrazy[2] == "")
                    throw (new Exception("Zly format pliku :( "));
                else
                {
                    nazwaStrumienia[i] = wyrazy[2];
                    liczbaKanalowZgloszenia[i] = int.Parse(wyrazy[5]);
                    maxCzasOczekiwania[i] = double.Parse(wyrazy[8]);

                    //czas polaczenia i czas odstepu maja przypisane na razie wartosci lambda,
                    //dodac tu funkcje rozkladu, zwroc

                    for(int j = 0; j < iloscRozkladow;j++)
                    {
                        if (nazwaRozkladu[j] == wyrazy[11])
                            czasPolaczenia[i] = lambda[j];
                    }

                    for (int k = 0; k < iloscRozkladow; k++)
                    {
                        if (nazwaRozkladu[k] == wyrazy[14])
                            czasPomiedzyZgloszeniami[i] = lambda[k];
                            
                    }
                }
            }

                #endregion
        }
     
    }
}
