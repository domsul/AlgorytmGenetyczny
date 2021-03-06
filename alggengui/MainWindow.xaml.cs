﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace alggengui
{
    class Funkcja
    {
        private static Funkcja instance = null;
        private double xp;
        private double xk;
        private double zmiennosc;
        private bool max;

        private Funkcja(double xp, double xk, bool max)
        {
            this.xp = xp;
            this.xk = xk;
            zmiennosc = this.xk - this.xp;
            this.max = max;
        }
        public static Funkcja GetInstance(double xp, double xk, bool max)
        {
            if (instance == null)
                instance = new Funkcja(xp, xk, max);

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

        public bool Max
        {
            get { return max; }
            set { max = value; }
        }
        public double Oblicz_funkcje(double x)
        {
            //return 2 * x + 3;
            return x * Math.Sin(10.0 * Math.PI * x) + 1.0;
            //return Math.Sin(2.0 * x) + (Math.Cos(4.0 * x) * Math.Cos(4.0 * x) * Math.Cos(4.0 * x));
        }

        public double Oblicz_pochodna(double x)
        {
            //return 2;
            return -10.0 * Math.PI * x;
            //return 2.0 * Math.Cos(2.0 * x) - 12.0 * Math.Sin(4.0 * x) * Math.Cos(4.0 * x) * Math.Cos(4.0 * x);
        }
    }

    class Chromosom_wlasciwosci
    {
        private static Chromosom_wlasciwosci instance = null;
        private static double dokladnosc;
        private static int dlugosc_wektora = 0;
        protected Funkcja funkcja = Funkcja.GetInstance();

        protected Chromosom_wlasciwosci()
        {

        }
        public static Chromosom_wlasciwosci GetInstance(double d)
        {
            if (instance == null)
            {
                instance = new Chromosom_wlasciwosci();
                dokladnosc = Math.Pow(10, d);
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


    class Chromosom : Chromosom_wlasciwosci
    {
        private int[] osobnik;
        private double fitness;

        public Chromosom()
        {
            Random r = new Random();

            osobnik = new int[Dlugosc_wektora()];
            for (int i = 0; i < osobnik.Length; i++)
                osobnik[i] = r.Next(0, 2);
            Eval();
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

        public void Inwersja()
        {
            int[] podzial = new int[2];
            int flaga;
            int pom;
            Random r = new Random();

            for (int i = 0; i < 2; i++)
                podzial[i] = r.Next(0, osobnik.Length);
            do
            {
                flaga = 0;
                if (podzial[0] == podzial[1])
                {
                    podzial[1] = r.Next(0, osobnik.Length);
                    flaga = 1;
                }
            } while (flaga > 0);
            if (podzial[0] > podzial[1])
            {
                pom = podzial[0];
                podzial[0] = podzial[1];
                podzial[1] = pom;
            }
            for (int i = podzial[0]; i < podzial[1]; i++)
            {
                if (osobnik[i] == 1)
                    osobnik[i] = 0;
                else
                    osobnik[i] = 1;
            }
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
            int flaga;
            Chromosom child;
            do
            {
                flaga = 0;
                child = new Chromosom();
                for (int i = 0; i < population.Count; i++)
                    if (child.Binarny_na_dziesietny() == population[i].Binarny_na_dziesietny())
                        flaga = 1;
            } while (flaga > 0);
            population.Add(new Chromosom());
        }

        public void Selekcja_krzyzowanie(ITypSelekcji s1, ITypKrzyzowania k1)
        {
            List<Chromosom> nowas = new List<Chromosom>();
            List<Chromosom> nowak = new List<Chromosom>();
            Select s = new Select();

            s.Ustaw_typ_selekcji(s1);
            nowas.AddRange(s.Selekcja(population));
            nowak.AddRange(Krzyzowanie(k1));
            nowas.AddRange(nowak);
            population.Clear();
            population.AddRange(nowas);
        }

        private List<Chromosom> Krzyzowanie(ITypKrzyzowania k1)
        {
            Random r = new Random();
            List<Chromosom> p = new List<Chromosom>();
            Krzyzowanie k = new Krzyzowanie();
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
            k.Ustaw_typ_krzyzowania(k1);
            p.AddRange(k.Krzyzuj(population[mama], population[tata], population));

            return p;
        }

        public void Szukaj_najlepszego()
        {
            if (Funkcja.GetInstance().Max == false)
            {
                najlepszy = 0;
                for (int i = 0; i < population.Count; i++)
                    if (population[i].Fitness < population[najlepszy].Fitness)
                        najlepszy = i;
            }
            else
            {
                najlepszy = 0;
                for (int i = 0; i < population.Count; i++)
                    if (population[i].Fitness > population[najlepszy].Fitness)
                        najlepszy = i;
            }
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

            for (int j = ilosc_przedzialow; j < populacja.Count; j += 2)
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
                syn.Inwersja();
                corka.Inwersja();
                syn.Eval();
                corka.Eval();
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

            for (int j = ilosc_przedzialow; j < populacja.Count; j += 2)
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
                syn.Inwersja();
                corka.Inwersja();
                syn.Eval();
                corka.Eval();
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

            for (int k = ilosc_przedzialow; k < populacja.Count; k += 2)
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
                syn.Inwersja();
                corka.Inwersja();
                syn.Eval();
                corka.Eval();
                dzieci.Add(syn);
                dzieci.Add(corka);
            }

            return dzieci;
        }
    }

    class Krzyzowanie : ITypKrzyzowania
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

            for (int i = 0; i < ilosc_przedzialow; i++)
            {
                mini = i * dlugosc_przedzialow;
                for (int j = mini; j < (i + 1) * dlugosc_przedzialow; j++)
                {
                    if (Funkcja.GetInstance().Max == false)
                    {
                        if (populacja[j].Fitness < populacja[mini].Fitness)
                            mini = j;
                    }
                    else
                    {
                        if (populacja[j].Fitness > populacja[mini].Fitness)
                            mini = j;
                    }
                }
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
            List<Chromosom> sortp;
            int ilosc_przedzialow = 20;

            if (Funkcja.GetInstance().Max == false)
                sortp = populacja.OrderBy(x => x.Fitness).ToList();
            else
                sortp = populacja.OrderByDescending(x => x.Fitness).ToList();
            for (int j = 0; j < ilosc_przedzialow; j++)
                nowa_p.Add(sortp[j]);

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

    interface IWynik
    {
        String Wyswietl_wynik();
    }

    class Naglowek : IWynik
    {
        public Naglowek()
        {

        }

        public String DodajNaglowek()
        {
            return "Populacja\tx min\t\tf(x) min";
        }

        public String Wyswietl_wynik()
        {
            

            return DodajNaglowek();
        }
    }

    class Tymczasowy : IWynik
    {
        private int a;
        private double b;
        private double c;
        public Tymczasowy(int a, double b, double c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }
        public String Wypisz_tymczasowy()
        {
            return a+"\t\t"+b+"\t\t"+c;
        }

        public String Wyswietl_wynik()
        {
            

            return Wypisz_tymczasowy();
        }
    }

    class Ostateczny : IWynik
    {
        private double a;
        private double b;

        public Ostateczny(double a, double b)
        {
            this.a = a;
            this.b = b;
        }

        public String Wypisz_ostateczny()
        {
            //MessageBox.Show("W punkcie: "+b+"\nWynik:    "+a);
            return "W punkcie: " + b + "\nWynik:    " + a;
        }

        public String Wyswietl_wynik()
        {
            

            return Wypisz_ostateczny();
        }
    }

    class Oczekiwane : IWynik
    {
        public Oczekiwane()
        {

        }

        public String Wypisz_oczekiwany()
        {
            return "Znaleziono oczekiwane rozwiazanie";
        }

        public String Wyswietl_wynik()
        {
            

            return Wypisz_oczekiwany();
        }
    }

    class WyswietlanieWyniku
    {
        public WyswietlanieWyniku()
        {

        }
        public String Wynik(IWynik w)
        {
            IWynik wynik = w;

            return w.Wyswietl_wynik();
        }
    }

    public static class ExtensionMethods
    {
        private static Action EmptyDelegate = delegate () { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double xp;
        double xk;
        double d;
        int najlepszy = 0;
        bool max;
        ITypSelekcji selekcja;
        ITypKrzyzowania krzyzowanie;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MinimumRB_Checked(object sender, RoutedEventArgs e)
        {
            max = false;
        }

        private void MaksimumRB_Checked(object sender, RoutedEventArgs e)
        {
            max = true;
        }

        private void JpRB_Checked(object sender, RoutedEventArgs e)
        {
            krzyzowanie = new Jednopunktowe();
        }

        private void DpRB_Checked(object sender, RoutedEventArgs e)
        {
            krzyzowanie = new Dwupunktowe();
        }

        private void TpRB_Checked(object sender, RoutedEventArgs e)
        {
            krzyzowanie = new Trzypunktowe();
        }

        private void TRB_Checked(object sender, RoutedEventArgs e)
        {
            selekcja = new Turniejowa();
        }

        private void RRB_Checked(object sender, RoutedEventArgs e)
        {
            selekcja = new Rankingowa();
        }

        private void ObliczB_Click(object sender, RoutedEventArgs e)
        {
            if (double.Parse(OdTB.Text) >= double.Parse(DoTB.Text) || double.Parse(DTB.Text) > 8)
                MessageBox.Show("Wartość 'od' musi być MNIEJSZA od wartości 'do'.\nDokładność musi być MNIEJSZA od 8.");
            else
            {
                xp = double.Parse(OdTB.Text);
                xk = double.Parse(DoTB.Text);
                d = double.Parse(DTB.Text);


                Funkcja funkcja = Funkcja.GetInstance(xp, xk, max);
                Chromosom_wlasciwosci data = Chromosom_wlasciwosci.GetInstance(d);
                Populacja p = new Populacja();
                WyswietlanieWyniku wynik = new WyswietlanieWyniku();
                Wynik w = new Wynik();

                WynikL.Content = "Proszę czekać, trwa wyszukiwanie optymalnego rozwiązania...";
                WynikL.Refresh();
                for (int i = 0; i < 400; i++)
                {
                    p.Dodaj_Osobnik();
                }

                w.Show();
                w.Work(wynik.Wynik(new Naglowek()));

                for (int j = 0; j < 2000; j++)
                {
                    p.Szukaj_najlepszego();
                    if (najlepszy != p.Najlepszy)
                    {
                        najlepszy = p.Najlepszy;
                        w.Work(wynik.Wynik(new Tymczasowy(j, Math.Round(p.Population[najlepszy].Binarny_na_dziesietny(), (int)d), Math.Round(p.Population[najlepszy].Fitness, (int)d))));
                    }
                    if (Math.Abs(funkcja.Oblicz_pochodna(p.Population[najlepszy].Binarny_na_dziesietny())) < 10e-4)
                    {
                        w.Work(wynik.Wynik(new Oczekiwane()));
                        break;
                    }
                    p.Selekcja_krzyzowanie(selekcja, krzyzowanie);
                    WynikL.Refresh();
                    WynikL.Content = (j / 20) + "%";
                    WynikL.Refresh();
                }
                WynikL.Refresh();
                WynikL.Content = "Znaleziono rozwiązanie!";
                WynikL.Refresh();
                najlepszy = p.Najlepszy;
                MessageBox.Show(wynik.Wynik(new Ostateczny(Math.Round(p.Population[najlepszy].Fitness, (int)d), Math.Round(p.Population[najlepszy].Binarny_na_dziesietny(), (int)d))));
            }
        }
    }
}

