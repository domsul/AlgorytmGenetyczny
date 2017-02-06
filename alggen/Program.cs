using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alggen
{
    class Funkcja
    {
        private static Funkcja instance = null;
        private double xp;
        private double xk;
        private double zmiennosc;

        private Funkcja(double xp, double xk)
        {
            this.xp = xp;
            this.xk = xk;
            zmiennosc = this.xk - this.xp;
        }
        public static Funkcja GetInstance(double xp, double xk)
        {
            if (instance == null)
                instance = new Funkcja(xp, xk);

            return instance;
        }

        public static Funkcja GetInstance()
        {
           return instance;
        }

        public double Xp
        {
            get { return xp; }
            set { xp = value; }
        }

        public double Xk
        {
            get { return xk; }
            set { xk = value; }
        }

        public double Zmiennosc
        {
            get { return zmiennosc; }
            set { zmiennosc = value; }
        }
        public double Oblicz_funkcje(double x)
        {
            return x * Math.Sin(10 * Math.PI * x) + 1.0;
        }

        public double Oblicz_pochodna(double x)
        {
            return -10 * Math.PI * x;
        }
    }

    class Chromosom_wlasciwosci
    {
        private static Chromosom_wlasciwosci instance = null;
        private static double dokladnosc;
        private static int dlugosc_wektora=0;
        protected Funkcja funkcja=Funkcja.GetInstance();

        protected Chromosom_wlasciwosci()
        {
            
        }
        public static Chromosom_wlasciwosci GetInstance(double d)
        {
            if (instance == null)
            {
                instance = new Chromosom_wlasciwosci();
                dokladnosc = Math.Pow(10, d);
                //instance.funkcja = Funkcja.GetInstance();
            }

            return instance;
        }

        public static Chromosom_wlasciwosci GetInstance()
        {
            return instance;
        }

        public double Dokladnosc
        {
            get { return dokladnosc; }
            set { dokladnosc = value; }
        }

        public int Dlugosc_wektora()
        {
            if (dlugosc_wektora == 0)
            {
                double x = 0;
                double l_podprzedzialow = funkcja.Zmiennosc * dokladnosc;
                double potega = 1;

                do
                {
                    if (potega < l_podprzedzialow)
                    {
                        x++;
                        potega *= 2;
                    }
                } while (potega < l_podprzedzialow);
                dlugosc_wektora = (int)x;
            }

            return dlugosc_wektora;
        }
    }

    class Chromosom:Chromosom_wlasciwosci
    {
        private int[] osobnik;
        private double fitness;
        
        public Chromosom()
        {
            Random r = new Random();

            osobnik = new int[Dlugosc_wektora()];
            for (int i = 0; i < osobnik.Length; i++)
                osobnik[i] = r.Next(0, 2);
            fitness = 0;
        }

        public int[] Osobnik
        {
            get { return osobnik; }
            set { osobnik = value; }
        }

        public double Fitness
        {
            get { return fitness; }
            set { fitness = value; }
        }

        public double Binarny_na_dziesietny()
        {
            int xprim = 0;
            int potega = 1;
            double x;

            for (int i = 0; i < osobnik.Length; i++)
            {
                xprim += osobnik[i] * potega;
                potega *= 2;
            }
            x = funkcja.Xp + funkcja.Zmiennosc * xprim / (potega - 1);

            return x;
        }

        public void Eval()
        {
            fitness = funkcja.Oblicz_funkcje(Binarny_na_dziesietny());
        }

        public void Mutacja()
        {
            int gen;
            Random r = new Random();

            if (Prawdopodobienstwo_mutacji() <= 0.1)
            {
                gen = r.Next(0, osobnik.Length);
                if (osobnik[gen] == 1)
                    osobnik[gen] = 0;
                else
                    osobnik[gen] = 1;
            }
        }

        public double Prawdopodobienstwo_mutacji()
        {
            Random r = new Random();

            return r.NextDouble();
        }
    }

    class Populacja
    {
        private List<Chromosom> population;
        private int najlepszy;
        Krzyzowanie typ = new Krzyzowanie();
        Select rodzaj = new Select();

        public Populacja()
        {
            population = new List<Chromosom>();
            najlepszy = 0;
        }

        public List<Chromosom> Population
        {
            get { return population; }
            set { population = value; }
        }

        public int Najlepszy
        {
            get { return najlepszy; }
            set { najlepszy = value; }
        }

        public void Dodaj_Osobnik()
        {
            population.Add(new Chromosom());
        }

        public void Selekcja_krzyzowanie()
        {
            List<Chromosom> nowas = new List<Chromosom>();
            List<Chromosom> nowak = new List<Chromosom>();
            Turniejowa s1 = new Turniejowa();

            nowas.AddRange(s1.Selekcja(population));
            nowak.AddRange(Krzyzowanie());
            nowas.AddRange(nowak);
            population.Clear();
            population.AddRange(nowas);
        }

        private List<Chromosom> Krzyzowanie()
        {
            Random r = new Random();
            List<Chromosom> p = new List<Chromosom>();
            Jednopunktowe k1 = new Jednopunktowe();
            int mama;
            int tata;
            int flaga;

            do
            {
                flaga = 0;
                mama = r.Next(0, population.Count);
                tata = r.Next(0, population.Count);
                if (mama == tata)
                    flaga = 1;
            } while (flaga > 0);
            p.AddRange(k1.Krzyzuj(population[mama], population[tata], population));

            return p;
        }

        public void Szukaj_najlepszego()
        {
            for (int i = 0; i < population.Count; i++)
                if (population[najlepszy].Fitness > population[i].Fitness)
                    najlepszy = i;
        }
    }

    interface ITypKrzyzowania
    {
        List<Chromosom> Krzyzuj(Chromosom mama, Chromosom tata, List<Chromosom> populacja);
    }

    class Jednopunktowe : ITypKrzyzowania
    {
        public List<Chromosom> Krzyzuj(Chromosom mama, Chromosom tata, List<Chromosom> populacja)
        {
            int podzial;
            int ilosc_przedzialow = 20;
            List<Chromosom> dzieci = new List<Chromosom>();
            Random r = new Random();

            for (int j = ilosc_przedzialow; j < populacja.Count; j+=2)
            {
                Chromosom syn = new Chromosom();
                Chromosom corka = new Chromosom();
                podzial = r.Next(0, mama.Osobnik.Length);
                for (int i = 0; i < podzial; i++)
                {
                    syn.Osobnik[i] = mama.Osobnik[i];
                    corka.Osobnik[i] = tata.Osobnik[i];
                }
                for (int i = podzial; i < mama.Osobnik.Length; i++)
                {
                    syn.Osobnik[i] = mama.Osobnik[i];
                    corka.Osobnik[i] = tata.Osobnik[i];
                }
                syn.Mutacja();
                corka.Mutacja();
                dzieci.Add(syn);
                dzieci.Add(corka);
            }

            return dzieci;
        }
    }

    class Dwupunktowe : ITypKrzyzowania
    {
        public List<Chromosom> Krzyzuj(Chromosom mama, Chromosom tata, List<Chromosom> populacja)
        {
            int[] podzial = new int[2];
            int ilosc_przedzialow = 20;
            int flaga;
            int pom;
            List<Chromosom> dzieci = new List<Chromosom>();
            Random r = new Random();

            for (int j = ilosc_przedzialow; j < populacja.Count; j++)
            {
                Chromosom syn = new Chromosom();
                Chromosom corka = new Chromosom();
                for (int i = 0; i < 2; i++)
                    podzial[i] = r.Next(0, mama.Osobnik.Length);
                do
                {
                    flaga = 0;
                    if (podzial[0] == podzial[1])
                    {
                        podzial[1] = r.Next(0, mama.Osobnik.Length);
                        flaga = 1;
                    }
                } while (flaga > 0);
                if (podzial[0] > podzial[1])
                {
                    pom = podzial[0];
                    podzial[0] = podzial[1];
                    podzial[1] = pom;
                }
                for (int i = 0; i < podzial[0]; i++)
                {
                    syn.Osobnik[i] = mama.Osobnik[i];
                    corka.Osobnik[i] = tata.Osobnik[i];
                }
                for (int i = podzial[0]; i < podzial[1]; i++)
                {
                    syn.Osobnik[i] = mama.Osobnik[i];
                    corka.Osobnik[i] = tata.Osobnik[i];
                }
                for (int i = podzial[1]; i < mama.Osobnik.Length; i++)
                {
                    syn.Osobnik[i] = mama.Osobnik[i];
                    corka.Osobnik[i] = tata.Osobnik[i];
                }
                syn.Mutacja();
                corka.Mutacja();
                dzieci.Add(syn);
                dzieci.Add(corka);
            }

            return dzieci;
        }
    }

    class Trzypunktowe : ITypKrzyzowania
    {
        public List<Chromosom> Krzyzuj(Chromosom mama, Chromosom tata, List<Chromosom> populacja)
        {
            int[] podzial = new int[3];
            int ilosc_przedzialow = 20;
            int flaga;
            int pom;
            List<Chromosom> dzieci = new List<Chromosom>();
            Random r = new Random();

            for (int k = ilosc_przedzialow; k < populacja.Count; k++)
            {
                Chromosom syn = new Chromosom();
                Chromosom corka = new Chromosom();
                for (int i = 0; i < 3; i++)
                    podzial[i] = r.Next(0, mama.Osobnik.Length);
                do
                {
                    flaga = 0;
                    if (podzial[0] == podzial[1])
                    {
                        podzial[1] = r.Next(0, mama.Osobnik.Length);
                        flaga = 1;
                    }
                    if (podzial[0] == podzial[2] || podzial[1] == podzial[2])
                    {
                        podzial[2] = r.Next(0, mama.Osobnik.Length);
                        flaga = 1;
                    }
                } while (flaga > 0);
                for (int i = 0; i < 3; i++)
                    for (int j = 1; j < 2; j++)
                    {
                        if (podzial[j - 1] > podzial[j])
                        {
                            pom = podzial[j - 1];
                            podzial[j - 1] = podzial[j];
                            podzial[j] = pom;
                        }
                    }

                for (int i = 0; i < podzial[0]; i++)
                {
                    syn.Osobnik[i] = mama.Osobnik[i];
                    corka.Osobnik[i] = tata.Osobnik[i];
                }
                for (int i = podzial[0]; i < podzial[1]; i++)
                {
                    syn.Osobnik[i] = mama.Osobnik[i];
                    corka.Osobnik[i] = tata.Osobnik[i];
                }
                for (int i = podzial[1]; i < podzial[2]; i++)
                {
                    syn.Osobnik[i] = mama.Osobnik[i];
                    corka.Osobnik[i] = tata.Osobnik[i];
                }
                for (int i = podzial[2]; i < mama.Osobnik.Length; i++)
                {
                    syn.Osobnik[i] = mama.Osobnik[i];
                    corka.Osobnik[i] = tata.Osobnik[i];
                }
                syn.Mutacja();
                corka.Mutacja();
                dzieci.Add(syn);
                dzieci.Add(corka);
            }

            return dzieci;
        }
    }

    class Krzyzowanie:ITypKrzyzowania
    {
        private ITypKrzyzowania k;

        public void Ustaw_typ_krzyzowania(ITypKrzyzowania t)
        {
            k = t;
        }

        public ITypKrzyzowania Gettyp()
        {
            return k;
        }

        public List<Chromosom> Krzyzuj(Chromosom mama, Chromosom tata, List<Chromosom> populacja)
        {
            return k.Krzyzuj(mama, tata, populacja);
        }
    }

    interface ITypSelekcji
    {
        List<Chromosom> Selekcja(List<Chromosom> populacja);
    }

    class Turniejowa : ITypSelekcji
    {
        public List<Chromosom> Selekcja(List<Chromosom> populacja)
        {
            List<Chromosom> nowa_p = new List<Chromosom>();
            int ilosc_przedzialow = 20;
            int dlugosc_przedzialow = populacja.Count / ilosc_przedzialow;
            int mini;

            for(int i=0;i<ilosc_przedzialow;i++)
            {
                mini = i * dlugosc_przedzialow;
                for (int j = mini; j < (i + 1) * dlugosc_przedzialow; j++)
                    if (populacja[j].Fitness < populacja[mini].Fitness)
                        mini = j;
                nowa_p.Add(populacja[mini]);
            }

            return nowa_p;
        }
    }

    class Rankingowa : ITypSelekcji
    {
        public List<Chromosom> Selekcja(List<Chromosom> populacja)
        {
            List<Chromosom> nowa_p = new List<Chromosom>();
            int ilosc_przedzialow = 20;
            int min = 0;

            populacja.OrderBy(x => x.Fitness).ToList();

            for (int j = 0; j < ilosc_przedzialow; j++)
            {
                /*for (int i = 0; i < populacja.Count; i++)
                    if (populacja[i].Fitness < populacja[min].Fitness)
                        min = i;
                nowa_p.Add(populacja[min]);*/
                nowa_p.Add(populacja[j]);
            }

            return nowa_p;
        }
    }

    class Select : ITypSelekcji
    {
        private ITypSelekcji s;

        public void Ustaw_typ_selekcji(ITypSelekcji t)
        {
            s = t;
        }

        public ITypSelekcji Gettyp()
        {
            return s;
        }

        public List<Chromosom> Selekcja(List<Chromosom> populacja)
        {
            return s.Selekcja(populacja);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            double xp = -1.0;
            double xk = 2.0;
            double d = 6.0;
            int najlepszy;
            Funkcja funkcja = Funkcja.GetInstance(xp, xk);
            Chromosom_wlasciwosci data = Chromosom_wlasciwosci.GetInstance(d);
            Populacja p = new Populacja();
            for(int i=0;i<200;i++)
                p.Dodaj_Osobnik();

            for (int j = 0; j < 200; j++)
            {
                for (int i = 0; i < p.Population.Count; i++)
                    p.Population[i].Eval();
                p.Szukaj_najlepszego();
                p.Selekcja_krzyzowanie();
            }
            najlepszy = p.Najlepszy;
            Console.WriteLine("Najlepszy osobnik: {0}", najlepszy);
            Console.WriteLine("Wynik: {0}", p.Population[najlepszy].Fitness);

            Console.ReadKey();
        }
    }
}
