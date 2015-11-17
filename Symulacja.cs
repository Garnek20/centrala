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
            //zgloszenia do systemu
            Zgloszenie[] zgloszenie;

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
                strumien = new Strumien[liczbaStrumieni];
                przypisanieRozkladow();
                
                for(int i = 0; i < liczbaStrumieni; i++)
                {
                    strumien[i] = new Strumien(czasPomiedzyZgloszeniami[i], czasPolaczenia[i], liczbaKanalowZgloszenia[i], maxCzasOczekiwania[i]);
                 
                }

                

                Random random = new Random();
                for(int x = 0; x < 30; x++)
                {
                    
                        
                    for (int i = 0; i<liczbaStrumieni; i++)
                    {                   
                        if( x % liczbaStrumieni == 0)
                        {
                            Zgloszenie zgloszenie = new Zgloszenie();
                            lista.dodaj(zgloszenie, aktualnyCzas);
                            TimeSpan nowy = new TimeSpan(0,0,0,0,random.Next(0, 10));
                            aktualnyCzas = aktualnyCzas + nowy;
                        }
                        else
                        {
                            Zgloszenie zgloszenie = new Zgloszenie();
                            lista.dodaj(zgloszenie, aktualnyCzas);
                            TimeSpan nowy = new TimeSpan(0, 0, 0, 0, random.Next(0, 10));
                            aktualnyCzas = aktualnyCzas + nowy;
                        }
                    }
                }
            }
            
            public void symulacja()
            {
                double temp;
                Console.WriteLine("Podaj czas symulacji: ");
                temp = double.Parse(Console.ReadLine());
                czasSymulacji = TimeSpan.FromMilliseconds(temp);
                aktualnyCzas = new TimeSpan(0,0,0,0,0);

                int k, liczbaKanalow_temp = liczbaKanalow, indeksZgloszeniaWsystemie, biezacyRozmiarFifo;
                TimeSpan[] czasWsystemie =new TimeSpan[liczbaKanalow];
                TimeSpan[] czasWkolejce = new TimeSpan[rozmiarKolejki];
                TimeSpan[] odstep = new TimeSpan[liczbaStrumieni];
              //  strumien = new Strumien[liczbaStrumieni];
                zgloszenie = new Zgloszenie[liczbaStrumieni];
                Zgloszenie[] zgloszenieWsystemie = new Zgloszenie[liczbaKanalow];
                Zgloszenie zgloszenieWfifo;
                kolejka = new Fifo(rozmiarKolejki);
                liczbaOdrzuconych = 0;
                iloscOdwiedzenSystemu = 0;
                indeksZgloszeniaWsystemie = 0;

                for (int i = 0; i < liczbaStrumieni; i++)
                {
                    odstep[i] =  TimeSpan.FromMilliseconds(0.0);
                }

               

                do
                {
                   

                    for (int i = 0; i < liczbaStrumieni; i++)
                    {
                        Zgloszenie zgl = new Zgloszenie(); //
                        lista.dodaj(zgl, aktualnyCzas);

                        if(odstep[i] == TimeSpan.FromMilliseconds(0.0))
                        {
                            zgloszenie[i] = lista.usun();
                            zgloszenie[i].ilosc_kanalow = strumien[i].liczbaKanalowZwroc();
                            zgloszenie[i].czas_trwania = strumien[i].czasPolaczeniaZwroc();

        
                            czasPomiedzyZgloszeniami[i] = strumien[i].czasDoNastepnegoZwroc();
                            odstep[i] = TimeSpan.FromMilliseconds(czasPomiedzyZgloszeniami[i]);

                            //sprawdzamy zawartosc kolejki
                            if (kolejka.zwrocRozmiar() == 0)
                                k = 1;
                            else if (kolejka.zwrocRozmiar() == rozmiarKolejki) k = 2;
                            else k = 3;

                            switch (k)
                            {
                                case 1: //przypadek, gdy kolejka jest pusta (jesli kanaly wolne to wrzucamy, jesli nie to do kolejki)
                                    if (liczbaKanalow_temp >= zgloszenie[i].iloscKanalowZwroc())
                                    {
                                        iloscOdwiedzenSystemu++;
                                        liczbaKanalow_temp = liczbaKanalow_temp - zgloszenie[i].iloscKanalowZwroc();
                                        
                                        while(zgloszenieWsystemie[indeksZgloszeniaWsystemie] != null)
                                        {
                                            indeksZgloszeniaWsystemie++;
                                        }
                                        zgloszenieWsystemie[indeksZgloszeniaWsystemie] = zgloszenie[i];
                                        czasWsystemie[i] = TimeSpan.FromMilliseconds(zgloszenie[i].czas_trwania);
                                    }
                                    else
                                    {
                                        kolejka.dodaj(zgloszenie[i]);
                                        czasWkolejce[i] = TimeSpan.FromMilliseconds(zgloszenie[i].maksCzasOczekiwaniaZwroc());
                                    }
                                    break;
                                case 2: //kolejka pelna, zgloszenie odrzucone
                                    liczbaOdrzuconych++;
                            //      zgloszenie[i] = null;
                                    break;
                                case 3: //wrzucanie do bufora
                                    kolejka.dodaj(zgloszenie[i]);
                                    czasWkolejce[i] = TimeSpan.FromMilliseconds(zgloszenie[i].maksCzasOczekiwaniaZwroc());
                                    break;

                            }


                        }

                    }
  // nastepnym etapem jest porownywanie najpierw czasow odstepu na strumieniach, potem czasow trwania polaczenia w systemie, porownania najmniejszych
  // wynikow ze soba, uzyskanym wynikiem jest najkrotszy czas od danej chwili czasu, ktory bedzie wyznacznikiem nastepnego kroku symulacji
                           
                    //porownywanie czasow
                    double nastepnyEtap;
                    for (int i = 0; i < liczbaStrumieni - 1; i ++) //porownanie czasow odstepu
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
                    for (int i = 0; i < iloscOdwiedzenSystemu - 1; i++) //porownanie czasow trwania polaczenia w systemie
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

                    //w przypadku, gdy czas odstepu, ktoregos ze strumieni, jest najkrotszym to zwalniane jest dany strumien,
                    //a od reszty czasow odejmujemy dany skok czasowy
                    if(czasPomiedzyZgloszeniami[liczbaStrumieni -1].CompareTo(czasPolaczenia[iloscOdwiedzenSystemu - 1]) < 0)
                    {
                        TimeSpan temp1;
                        temp1 = aktualnyCzas;
                        aktualnyCzas = TimeSpan.FromMilliseconds(czasPomiedzyZgloszeniami[liczbaStrumieni - 1]);
                        aktualnyCzas = aktualnyCzas + temp1;
                        Zgloszenie doWyrzucenia;

                        for (int i = 0; i <liczbaStrumieni;i++)
                        {
                            czasPomiedzyZgloszeniami[i] = czasPomiedzyZgloszeniami[i] - czasPomiedzyZgloszeniami[liczbaStrumieni - 1];
                        }
                        if (kolejka.zwrocRozmiar() != 0)
                        {
                            for (int i = 0; i < kolejka.zwrocRozmiar(); i++)
                            {
                                doWyrzucenia = kolejka.zwrocElementKolejki(i);
                                if (doWyrzucenia.maksCzasOczekiwaniaZwroc() >= czasPomiedzyZgloszeniami[liczbaStrumieni - 1])
                                {   
                                    liczbaOdrzuconych++;
                                    kolejka.usunDanyElement(i);
                                }
                                else
                                {
                                    czasWkolejce[i] = TimeSpan.FromMilliseconds(doWyrzucenia.maksCzasOczekiwaniaZwroc()) - TimeSpan.FromMilliseconds(czasPomiedzyZgloszeniami[liczbaStrumieni -1]);
                                    
                                }
                            }
                        }
                    }
                    else //przypadek, gdy najkrtoszym czasem jest jeden z czasow trwania obslugi polaczenia na kanalach
                        //wtedy dane zgloszenie konczy transmisje i, jezeli liczba kanalow zgloszenia ostatniego w kolejce
                        //jest niewieksza niz ilosc kanalow dostepnych, wtedy dane zgloszenie obslugujemy i zwalniamy miejsce w kolejce
                    {
                        TimeSpan temp2;
                        temp2 = aktualnyCzas;
                        aktualnyCzas = TimeSpan.FromMilliseconds(czasPolaczenia[iloscOdwiedzenSystemu - 1]);
                        aktualnyCzas = aktualnyCzas + temp2;
                        

                        for (int i = 0; i < iloscOdwiedzenSystemu; i++)
                        {
                            if (czasWsystemie[i] != TimeSpan.FromMilliseconds(0.0))
                            czasWsystemie[i] = czasWsystemie[i] - TimeSpan.FromMilliseconds(czasPolaczenia[iloscOdwiedzenSystemu - 1]);
                        }

                        for (int j = 0; j <indeksZgloszeniaWsystemie; j++)
                        {
                            if(zgloszenieWsystemie[j].czasTrwaniaZwroc() >= czasPolaczenia[iloscOdwiedzenSystemu - 1])
                            {
                                //czas do statystyki sredniego czasu obslugi dopisac
                                liczbaKanalow_temp = liczbaKanalow_temp + zgloszenieWsystemie[j].iloscKanalowZwroc();
                                biezacyRozmiarFifo =  kolejka.zwrocRozmiar();
                                if (biezacyRozmiarFifo != 0)
                                {
                                    zgloszenieWfifo = kolejka.zwrocElementKolejki(biezacyRozmiarFifo);
                                    if (zgloszenieWfifo.iloscKanalowZwroc() <= liczbaKanalow_temp)
                                    {
                                        indeksZgloszeniaWsystemie = 0;
                                        while (zgloszenieWsystemie[indeksZgloszeniaWsystemie] != null)
                                        {
                                            indeksZgloszeniaWsystemie++;
                                        }
                                        zgloszenieWsystemie[indeksZgloszeniaWsystemie] = kolejka.usun();
                                        liczbaKanalow_temp = liczbaKanalow_temp - zgloszenieWsystemie[indeksZgloszeniaWsystemie].iloscKanalowZwroc();
                                    }
                                }
                                indeksZgloszeniaWsystemie = 0;
                                
                            }
                        }
                        

                    }
                        
                }while(czasSymulacji >= aktualnyCzas);
                Console.WriteLine("{0} , {1}", liczbaOdrzuconych, iloscOdwiedzenSystemu);
            }
            //metoda wczytujaca dane z pliku
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
