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
            //obiekt klasy statystyki
            Statystyki statystyki;
            //bufor w centrali
            Fifo kolejka;
            //zmienna do statystyk, liczba odrzuconych zgloszen bez dojscia do kolejki
            int liczbaOdrzuconych;
            //liczba zgloszen wpadajacych do systemu
            int liczbaZgloszen;
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
                statystyki = new Statystyki();
                przypisanieRozkladow();
                
                for(int i = 0; i < liczbaStrumieni; i++)
                {
                    strumien[i] = new Strumien(czasPomiedzyZgloszeniami[i], czasPolaczenia[i], liczbaKanalowZgloszenia[i], maxCzasOczekiwania[i]);
                }
                              
                Random random = new Random();
                for(int x = 0; x < 2; x++)
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
                double[] kolejkaCzas = new double[rozmiarKolejki];
                Array.Resize(ref czasPolaczenia, liczbaKanalow);
               
                Console.WriteLine("Podaj czas symulacji: ");
                temp = double.Parse(Console.ReadLine());
                czasSymulacji = TimeSpan.FromMilliseconds(temp);
                aktualnyCzas = new TimeSpan(0,0,0,0,0);

                int st=0;
                int k, liczbaKanalow_temp = liczbaKanalow, indeksZgloszeniaWsystemie, biezacyRozmiarFifo;
                int[] liczbaKanSystem = new int[liczbaKanalow];
                TimeSpan[] czasWsystemie =new TimeSpan[liczbaKanalow];
                TimeSpan[] czasWkolejce = new TimeSpan[rozmiarKolejki];
                TimeSpan[] odstep = new TimeSpan[liczbaStrumieni];
                zgloszenie = new Zgloszenie[liczbaStrumieni];
                Zgloszenie[] zgloszenieWsystemie = new Zgloszenie[liczbaKanalow];
                Zgloszenie zgloszenieWfifo;
                kolejka = new Fifo(rozmiarKolejki);
                liczbaOdrzuconych = 0;
                iloscOdwiedzenSystemu = 0;
                indeksZgloszeniaWsystemie = 0;
                liczbaZgloszen=0;

                do
                {
                    
                    przypisanieRozkladow();
                    for (int i = 0; i < liczbaStrumieni; i++)
                    {
                        strumien[i] = new Strumien(czasPomiedzyZgloszeniami[i], czasPolaczenia[i], liczbaKanalowZgloszenia[i], maxCzasOczekiwania[i]);
                    }
                    for (int i = 0; i < liczbaStrumieni; i++)
                    {
                        czasPolaczenia[i] = 0;
                        Zgloszenie zgl = new Zgloszenie(); //
                        lista.dodaj(zgl, aktualnyCzas);

                        if(odstep[i] == TimeSpan.Zero)
                        {
                            liczbaZgloszen++;
                            zgloszenie[i] = lista.usun();
                            zgloszenie[i].ilosc_kanalow = strumien[i].liczbaKanalowZwroc();
                            zgloszenie[i].czas_trwania = strumien[i].czasPolaczeniaZwroc();
                            zgloszenie[i].maks_czas_oczekiwania = strumien[i].czasOczekiwaniaZwroc();
        
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
                                        st++;
                                        iloscOdwiedzenSystemu++;
                                        liczbaKanalow_temp = liczbaKanalow_temp - zgloszenie[i].iloscKanalowZwroc();
                                         indeksZgloszeniaWsystemie = 0;
                                        while(liczbaKanSystem[indeksZgloszeniaWsystemie] != 0)
                                        {
                                            indeksZgloszeniaWsystemie++;
                                        }
                                        liczbaKanSystem[indeksZgloszeniaWsystemie] = zgloszenie[i].iloscKanalowZwroc();
                                        czasWsystemie[indeksZgloszeniaWsystemie] = TimeSpan.FromMilliseconds(zgloszenie[i].czas_trwania); ///sprawdzic
                                        czasPolaczenia[indeksZgloszeniaWsystemie] = zgloszenie[i].czasTrwaniaZwroc();//?
                                        
                                    }
                                    else
                                    {
                                        kolejka.dodaj(zgloszenie[i]);
                                        czasWkolejce[kolejka.zwrocRozmiar() - 1] = TimeSpan.FromMilliseconds(zgloszenie[i].maksCzasOczekiwaniaZwroc());
                                    }
                                    break;
                                case 2: //kolejka pelna, zgloszenie odrzucone
                                    liczbaOdrzuconych++;
                                    break;
                                case 3: //wrzucanie do bufora
                                    kolejka.dodaj(zgloszenie[i]);
                                    kolejkaCzas[i] = zgloszenie[i].maksCzasOczekiwaniaZwroc();
                                    czasWkolejce[i] = TimeSpan.FromMilliseconds(zgloszenie[i].maksCzasOczekiwaniaZwroc());
                                    if(kolejka.zwrocRozmiar() == rozmiarKolejki)
                                    {

                                        statystyki.zwiekszZajetoscKolejki(kolejkaCzas[i]);
                                    }
                                    break;
                            }
                        }
                    }
                    // nastepnym etapem jest porownywanie najpierw czasow odstepu na strumieniach, potem czasow trwania polaczenia w systemie, porownania najmniejszych
                    // wynikow ze soba, uzyskanym wynikiem jest najkrotszy czas od danej chwili czasu, ktory bedzie wyznacznikiem nastepnego kroku symulacji
                           
                    //porownywanie czasow
                    TimeSpan nastepnyEtap = new TimeSpan();
                    for (int i = 0; i < liczbaStrumieni - 1; i ++) //porownanie czasow odstepu
                    {
                        if (liczbaStrumieni > 1 && odstep[i+1] != TimeSpan.Zero) 
                        {
                            if (odstep[i].CompareTo(odstep[i + 1]) < 0)
                            {
                                nastepnyEtap = odstep[i];
                            }
                            else
                            {
                                nastepnyEtap = odstep[i+1];
                            }
                        }
                        else
                        {
                            nastepnyEtap = odstep[i];
                        }

                        
                    }
                    TimeSpan nastepnyEtap2 = new TimeSpan();
                    for (int i = 0; i < iloscOdwiedzenSystemu; i++) //porownanie czasow trwania polaczenia w systemie
                    {
                        if (czasWsystemie[1] != TimeSpan.Zero)
                        {
                            if (iloscOdwiedzenSystemu > 1 && czasWsystemie[i + 1] != TimeSpan.Zero)
                            {
                                if (i > 0)
                                {
                                    if (nastepnyEtap2.CompareTo(czasWsystemie[i + 1]) < 0)
                                    {
                                        ;
                                    }
                                    else
                                    {
                                        nastepnyEtap2 = czasWsystemie[i + 1];
                                    }
                                }
                                else
                                {
                                    if (czasWsystemie[i].CompareTo(czasWsystemie[i + 1]) < 0)
                                    {
                                        nastepnyEtap2 = czasWsystemie[i];
                                    }
                                    else
                                    {
                                        nastepnyEtap2 = czasWsystemie[i + 1];
                                    }
                                }
                            }
                            else
                            {
                                if (nastepnyEtap2.CompareTo(czasWsystemie[i + 1]) < 0)
                                    ;
                                else
                                {
                                    if (czasWsystemie[i + 1] != TimeSpan.Zero && i != 0)
                                        nastepnyEtap2 = czasWsystemie[i + 1];
                                }
                                if (i == 0)
                                    nastepnyEtap2 = czasWsystemie[0];

                            }
                        }
                        else
                        {
                            nastepnyEtap2 = czasWsystemie[0];
                        }
                    }

                    if (nastepnyEtap.CompareTo(nastepnyEtap2) < 0 && nastepnyEtap != TimeSpan.Zero)
                    {
                        aktualnyCzas = aktualnyCzas + nastepnyEtap;
                        Zgloszenie doWyrzucenia;

                        //odejmowanie najkrotszego czasu dla czasow odstepu
                        for (int i = 0; i <liczbaStrumieni;i++)
                        {
                            odstep[i] = odstep[i] - nastepnyEtap;// TimeSpan.FromMilliseconds(czasPomiedzyZgloszeniami[i]);
                        }
                        //odejmowanie najkrotszego czasu dla czasow w systemie
                        for (int i = 0; i < iloscOdwiedzenSystemu; i++)
                        {
                            if (czasWsystemie[i] != TimeSpan.FromMilliseconds(0.0))
                            {
                                czasWsystemie[i] = czasWsystemie[i] - nastepnyEtap;
                            }
                        }
                        TimeSpan wyrzuc;
                        //obsluzenie czasowe w kolejce: sprawdzenie czy maksczas oczekiwania nie zostalprzekroczony, jesli nie to zwykle odejmowanie
                        if (kolejka.zwrocRozmiar() != 0)
                        {
                            doWyrzucenia = kolejka.zwrocElementKolejki(kolejka.zwrocRozmiar() - 1);
                            wyrzuc = TimeSpan.FromMilliseconds(doWyrzucenia.maksCzasOczekiwaniaZwroc());
                               
                                if (wyrzuc <= nastepnyEtap)
                                {   
                                    liczbaOdrzuconych++;
                                    kolejka.usun();
                                }
                                else
                                {
                                    for (int i = 0; i < kolejka.zwrocRozmiar(); i++)
                                    {
                                      //  kolejkaCzas[i] = 
                                        czasWkolejce[i] = TimeSpan.FromMilliseconds(doWyrzucenia.maksCzasOczekiwaniaZwroc()) - nastepnyEtap;
                                    }
                                }
                            
                        }
                    }
                    else //przypadek, gdy najkrtoszym czasem jest jeden z czasow trwania obslugi polaczenia na kanalach
                        //wtedy dane zgloszenie konczy transmisje i, jezeli liczba kanalow zgloszenia ostatniego w kolejce
                        //jest niewieksza niz ilosc kanalow dostepnych, wtedy dane zgloszenie obslugujemy i zwalniamy miejsce w kolejce
                    {
                        if (nastepnyEtap2 != TimeSpan.Zero) //przypadek gdy czasy polaczenia w systemie byly krotsze, wtedy wszystkie sa wyzerowane
                        {
                            aktualnyCzas = aktualnyCzas + nastepnyEtap2;

                            //odejmowanie najkrotszego czasu dla czasow odstepu
                            for (int i = 0; i < liczbaStrumieni; i++)
                            {
                                odstep[i] = odstep[i] - nastepnyEtap2;
                            }
                            //obsluga czasow w systemie
                            for (int j = 0; j < indeksZgloszeniaWsystemie + 1; j++)
                            {
                                if (czasWsystemie[j] != TimeSpan.Zero)
                                {
                                    czasWsystemie[j] = czasWsystemie[j] - nastepnyEtap2;
                                }
                            }

                            for (int j = 0; j < indeksZgloszeniaWsystemie + 1; j++)
                            {
                                if (czasWsystemie[j] == TimeSpan.Zero)
                                {
                                    //czas do statystyki sredniego czasu obslugi dopisac

                                    if (j != indeksZgloszeniaWsystemie)
                                    {
                                        for (int i = 0; i < indeksZgloszeniaWsystemie; i++)
                                        {
                                            TimeSpan tmp;
                                            int ttmp = liczbaKanSystem[i];
                                            tmp = czasWsystemie[i];
                                            if (tmp > TimeSpan.Zero)
                                            {
                                                ;
                                            }
                                            else
                                            {
                                                liczbaKanSystem[i] = liczbaKanSystem[i + 1];
                                                liczbaKanSystem[i + 1] = ttmp;

                                                czasWsystemie[i] = czasWsystemie[i + 1];
                                                czasWsystemie[i + 1] = tmp;

                                            }
                                        }
                                    }
                                }
                            }
                            if (czasWsystemie[indeksZgloszeniaWsystemie] == TimeSpan.Zero)
                            {
                                iloscOdwiedzenSystemu--;
                                liczbaKanalow_temp = liczbaKanalow_temp + liczbaKanSystem[indeksZgloszeniaWsystemie];
                                liczbaKanSystem[indeksZgloszeniaWsystemie] = 0;
                            }

                            biezacyRozmiarFifo = kolejka.zwrocRozmiar();
                            if (biezacyRozmiarFifo != 0)
                            {
                                zgloszenieWfifo = kolejka.zwrocElementKolejki(biezacyRozmiarFifo - 1);
                                if (zgloszenieWfifo.iloscKanalowZwroc() <= liczbaKanalow_temp)// jesli element kolejki ma zapotrzebowanie kanalowe niewieksze od zwolnionego, to zostaje obsluzony przez system
                                {
                                    st++;
                                    zgloszenieWfifo = kolejka.usun();
                                    iloscOdwiedzenSystemu++;
                                    indeksZgloszeniaWsystemie = 0;
                                    while (liczbaKanSystem[indeksZgloszeniaWsystemie] != 0)
                                    {
                                        indeksZgloszeniaWsystemie++;
                                    }
                                    liczbaKanSystem[indeksZgloszeniaWsystemie] = zgloszenieWfifo.iloscKanalowZwroc();
                                    czasWsystemie[indeksZgloszeniaWsystemie] = TimeSpan.FromMilliseconds(zgloszenieWfifo.czasTrwaniaZwroc()); ///sprawdzic
                                    czasPolaczenia[indeksZgloszeniaWsystemie] = zgloszenieWfifo.czasTrwaniaZwroc();//?

                                    liczbaKanalow_temp = liczbaKanalow_temp - zgloszenieWfifo.iloscKanalowZwroc();
                                }
                                else if (TimeSpan.FromMilliseconds(zgloszenieWfifo.maksCzasOczekiwaniaZwroc()) <= nastepnyEtap2)
                                {
                                    kolejka.usun();
                                    liczbaOdrzuconych++;
                                }
                            }

                        }
                        else // jesli wszystkie czasy polaczenia zerowe, wtedy -- czasy odstepu
                        {
                            aktualnyCzas = aktualnyCzas + nastepnyEtap;
                            Zgloszenie doWyrzucenia;

                            //odejmowanie najkrotszego czasu dla czasow odstepu
                            for (int i = 0; i < liczbaStrumieni; i++)
                            {
                                odstep[i] = odstep[i] - nastepnyEtap;// TimeSpan.FromMilliseconds(czasPomiedzyZgloszeniami[i]);
                            }
                            //odejmowanie najkrotszego czasu dla czasow w systemie
                            for (int i = 0; i < iloscOdwiedzenSystemu; i++)
                            {
                                if (czasWsystemie[i] != TimeSpan.FromMilliseconds(0.0))
                                {
                                    czasWsystemie[i] = czasWsystemie[i] - nastepnyEtap;
                                }
                            }
                            //obsluzenie czasowe w kolejce: sprawdzenie czy maksczas oczekiwania nie zostalprzekroczony, jesli nie to zwykle odejmowanie
                            if (kolejka.zwrocRozmiar() != 0)
                            {
                                for (int i = 0; i < kolejka.zwrocRozmiar(); i++)
                                {
                                    doWyrzucenia = kolejka.zwrocElementKolejki(i);
                                    if (TimeSpan.FromMilliseconds(doWyrzucenia.maksCzasOczekiwaniaZwroc()) <= nastepnyEtap)
                                    {
                                        liczbaOdrzuconych++;
                                        kolejka.usunDanyElement(i);
                                    }
                                    else
                                    {
                                        czasWkolejce[i] = TimeSpan.FromMilliseconds(doWyrzucenia.maksCzasOczekiwaniaZwroc()) - nastepnyEtap;

                                    }
                                }
                            }
                        }
                   }                     
                }
                while(czasSymulacji >= aktualnyCzas);

                statystyki.obliczPstwoOdrzucenia(liczbaZgloszen, liczbaOdrzuconych);
                Console.WriteLine("{0} , {1}", liczbaOdrzuconych, st);
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
                    czasPolaczenia[i] = lambda[i];
                    czasPomiedzyZgloszeniami[i] = lambda[i];
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
