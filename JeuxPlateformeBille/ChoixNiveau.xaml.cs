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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JeuxPlateformeBille
{
    /// <summary>
    /// Logique d'interaction pour ChoixNiveau.xaml
    /// </summary>
    public partial class ChoixNiveau : UserControl
    {
        public ChoixNiveau()
        {
            InitializeComponent();
        }

        private void ellipseNiveau1_MouseDown(object sender, MouseButtonEventArgs e)
        {

            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).niveau = 0;
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).Suivant();
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).ControlContent.Content = null;
        }

        private void ellipseNiveau2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).niveau = 1;
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).Suivant(); 
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).ControlContent.Content = null;
        }

        private void retour_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).ControlContent.Content = new Accueil();

        }
        public void ChangerCouleurEllipseNiveau(int niveau)
        {



            if (niveau >= 0)
            {
                ellipseNiveau1.Fill = Brushes.Green;
            }
            if (niveau >=1)
            {
                ellipseNiveau2.Fill = Brushes.Green;
            }
            if (niveau >= 2)
            {
                ellipseNiveau3.Fill = Brushes.Green;
            }
        }

    }
}
