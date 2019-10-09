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
using Xceed.Wpf.Toolkit;

namespace TraitementImage
{
    /// <summary>
    /// Logique d'interaction pour ThreshholdSelector.xaml
    /// </summary>
    public partial class ThreshholdSelector : Window
    {
        private MainWindow _mainwindow;

        public ThreshholdSelector()
        {
            InitializeComponent();
        }

        public ThreshholdSelector(int nbrThresh, MainWindow mw)
        {
            Mainwindow = mw;

            var b = new Button();
            InitializeComponent();
            for(int i=0;i<nbrThresh;i++)
            {
                var l = new Label();
                l.Content = "threshold" + i;
                sp.Children.Add(l);
                sp.Children.Add(new IntegerUpDown());
                sp.Children.Add(new IntegerUpDown());
                sp.Children.Add(new IntegerUpDown());
            }
            b.Name = "ok";
            b.Content = "Valider";
            b.Click += B_Click;
            sp.Children.Add(b);
        }

        private void B_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public MainWindow Mainwindow { get => _mainwindow; set => _mainwindow = value; }
    }
}
