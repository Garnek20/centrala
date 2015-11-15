using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
//using System.DateTime;



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
            //lista zgloszen inicjowana na poczatku
            Kolejka.DrzewoTurniejowe<Zgloszenie, TimeSpan> lista;
            //czas Zgloszen do kolejki
            System.TimeSpan aktualnyCzas;
            //czas calej symulacji
            System.TimeSpan czasSymulacji;
            //tablica obiektow strumien (wielkosc zalezna od ilosci strumieni)
            Strumien[] strumien;
            //bufor w centrali
            Fifo kolejka;
            //zmienna do statystyk, liczba odrzuconych zgloszen bez dojscia do kolejki
            int liczbaOdrzuconych;
            //ilosc odwiedzen systemu
            int iloscOdwiedzenSystemu;
            #endregion

            //konstruktor domyslny
            public Symulacja()
            {
                //wczytywanie wszystkich parametrow
                wczytywanie();
                //inicjacja listy zgloszen o elementach Zgloszenie i kluczu czasu przyjscia ( wzgledem tego jest sortowana)
                lista = new Kolejka.DrzewoTurniejowe<Zgloszenie,TimeSpan>();
                aktualnyCzas = new TimeSpan();
                czasSymulacji = new TimeSpan();
                Random random = new Random();
                for(int x = 0; x < 300; x++)
                {
                     
                     przypisanieRozkladow();
                        
                    for (int i = 0; i<liczbaStrumieni; i++)
                    {                   
                        if( x % liczbaStrumieni == 0)
                        {
                            Zgloszenie zgloszenie = new Zgloszenie(liczbaKanalowZgloszenia[x%liczbaStrumieni],maxCzasOczekiwania[x%liczbaStrumieni],czasPolaczenia[x%liczbaStrumieni]);
                            lista.dodaj(zgloszenie, aktualnyCzas);
                            TimeSpan nowy = new TimeSpan(0,0,0,0,random.Next(0, 10));
                            aktualnyCzas = aktualnyCzas + nowy;
                        }
                        else
                        {
                            Zgloszenie zgloszenie = new Zgloszenie(liczbaKanalowZgloszenia[x % liczbaStrumieni], maxCzasOczekiwania[x % liczbaStrumieni], czasPolaczenia[x % liczbaStrumieni]);
                            lista.dodaj(zgloszenie, aktualnyCzas);
                            TimeSpan nowy = new TimeSpan(0, 0, 0, 0, random.Next(0, 10));
                            aktualnyCzas = aktualnyCzas + nowy;
                        }
                    }
                }
            }
            //metoda wczytujaca dane z pliku
            public void symulacja()
            {
                double temp;
                Console.WriteLine("Podaj czas symulacji: ");
                temp = double.Parse(Console.ReadLine());
                czasSymulacji = TimeSpan.FromMilliseconds(temp);
                aktualnyCzas = new TimeSpan(0,0,0,0,0);

                int k, liczbaKanalow_temp = liczbaKanalow;
                TimeSpan[] czasWsystemie =new TimeSpan[liczbaKanalow];
                TimeSpan[] czasWkolejce = new TimeSpan[rozmiarKolejki];
                TimeSpan odstep = new TimeSpan(0,0,0,0,0);
                strumien = new Strumien[liczbaStrumieni];
                Zgloszenie[] zgloszenie = new Zgloszenie[liczbaStrumieni];
                kolejka = new Fifo(rozmiarKolejki);
                liczbaOdrzuconych = 0;
                iloscOdwiedzenSystemu = 0;

                for (int i = 0; i < liczbaStrumieni; i++)
                {
                    strumien[i] = new Strumien(czasPomiedzyZgloszeniami[i],czasPolaczenia[i], liczbaKanalowZgloszenia[i],maxCzasOczekiwania[i]);
                }

                do
                {

                    for (int i = 0; i < liczbaStrumieni; i++)
                    {
                        zgloszenie[i] = lista.usun();

                        if (zgloszenie[i].iloscKanalowZwroc() == strumien[i].liczbaKanalowZwroc() && odstep == TimeSpan.FromMilliseconds(0.0))
                        {
                            czasPomiedzyZgloszeniami[i] = strumien[i].czasDoNastepnegoZwroc();
                            odstep = TimeSpan.FromMilliseconds(czasPomiedzyZgloszeniami[i]);
                        }
                    }
                           //sprawdzamy zawartosc kolejki
                            if (kolejka.zwrocRozmiar() == 0)
                                k = 1;
                            else if (kolejka.zwrocRozmiar() == rozmiarKolejki) k = 2;
                            else k = 3;
                           
                        for(int i = 0; i < liczbaStrumieni; i++)
                        {
                            switch (k)
                            {
                                case 1: //przypadek, gdy kolejka jest pusta (jesli kanaly wolne to wrzucamy, jesli nie to do kolejki)
                                    if (liczbaKanalow_temp >= zgloszenie[i].iloscKanalowZwroc())
                                    {
                                        iloscOdwiedzenSystemu++;
                                        liczbaKanalow_temp = liczbaKanalow_temp - zgloszenie[i].iloscKanalowZwroc();
                                        czasWsystemie[i] = TimeSpan.FromMilliseconds(czasPolaczenia[i]);
                                    }
                                    else
                                    {
                                        kolejka.dodaj(zgloszenie[i]);
                                        czasWkolejce[i] = TimeSpan.FromMilliseconds(zgloszenie[i].czasOczekiwaniaZwroc());
                                    }
                                    break;
                                case 2: //kolejka pelna, zgloszenie odrzucone
                                    liczbaOdrzuconych++;
                                    zgloszenie[i] = null;
                                    break;
                                case 3: //wrzucanie do bufora
                                    kolejka.dodaj(zgloszenie[i]);
                                    czasWkolejce[i] = TimeSpan.FromMilliseconds(zgloszenie[i].czasOczekiwaniaZwroc());
                                    break;

                            }
                        }
                    //porownywanie czasow
                    double nastepnyEtap;
                    for (int i = 0; i < liczbaStrumieni; i ++) //porownanie czasow odstepu
                    {
                        if (liczbaStrumieni > 0)
                        {
                            if (czasPomiedzyZgloszeniami[i].CompareTo(czasPomiedzyZgloszeniami[i + 1]) < 0)
                            {
                                nastepnyEtap = czasPomiedzyZgloszeniami[i];
                                czasPomiedzyZgloszeniami[i] = czasPomiedzyZgloszeniami[i + 1];
                                czasPomiedzyZgloszeniami[i + 1] = nastepnyEtap;
                            }
                        }
                        
                    }
                    double nastepnyEtap2;
                    for (int i = 0; i < iloscOdwiedzenSystemu; i++) //porownanie czasow trwania polaczenia w systemie
                    {
                        if (iloscOdwiedzenSystemu > 0)
                        {
                            if (czasPolaczenia[i].CompareTo(czasPolaczenia[i + 1]) < 0)
                            {
                                nastepnyEtap2 = czasPolaczenia[i];
                                czasPolaczenia[i] = czasPolaczenia[i + 1];
                                czasPolaczenia[i + 1] = nastepnyEtap2;
                            }
                        }
                    }
                    
                    if(czasPomiedzyZgloszeniami[liczbaStrumieni -1].CompareTo(czasPolaczenia[iloscOdwiedzenSystemu - 1]) < 0)
                    {
                        TimeSpan temp1;
                        temp1 = aktualnyCzas;
                        aktualnyCzas = TimeSpan.FromMilliseconds(czasPomiedzyZgloszeniami[liczbaStrumieni - 1]);
                        aktualnyCzas = aktualnyCzas + temp1;
                        //na tym stanelismy
                    }
                        
                }while(czasSymulacji >= aktualnyCzas);
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

                for (int i = 0; i < liczbaStrumieni; i++)
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

                        for (int j = 0; j < iloscRozkladow; j++)
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
            //metoda przypisujaca wartosci czasu polaczenia i odstepu miedzy zgloszeniami za pomoca rozkladow
            public void przypisanieRozkladow()
            {
                Random random = new Random();
                double y, z;

                for(int i =0; i < liczbaStrumieni; i++)
                {
                    y = random.NextDouble();
                    z = random.NextDouble();
                    for(int j = 0; j < iloscRozkladow; j++)
                    {
                        if(czasPolaczenia[i] == lambda[j])
                        {
                            Rozklad rozklad1 = new Rozklad(lambda[j], nazwaRozkladu[j]);
                            czasPolaczenia[i] = rozklad1.zwrocRozklad(y);
                        }
                        if(czasPomiedzyZgloszeniami[i] == lambda[j])
                        {
                            Rozklad rozklad2 = new Rozklad(lambda[j], nazwaRozkladu[j]);
                            czasPomiedzyZgloszeniami[i] = rozklad2.zwrocRozklad(z);
                        }
                    }
                }
            }
        }
    }
