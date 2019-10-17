using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace alggengui
{
    /// <summary>
    /// Logika interakcji dla klasy Wynik.xaml
    /// </summary>
    public partial class Wynik : Window
    {
        public Wynik()
        {
            InitializeComponent();
        }

        public void Work(String s)
        {
            WynikiTB.Refresh();
            WynikiTB.Text += s+"\n";
            WynikiTB.Refresh();
        }
    }
}
