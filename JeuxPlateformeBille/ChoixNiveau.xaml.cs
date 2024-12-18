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
            if (((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).niveau == 0)
            {
                ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).niveau = 1;
                ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).Suivant();
                ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).ControlContent.Content = null;

            }
            
        }

        private void ellipseNiveau2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).niveau == 1)
            {
                ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).niveau = 2;
                ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).Suivant();
                ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).ControlContent.Content = null;

            }
        }
        private void ellipseNiveau3_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).niveau == 2)
            {
                ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).niveau = 3;
                ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).Suivant();
                ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).ControlContent.Content = null;

            }
        }
        private void ellipseNiveau4_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).niveau == 3)
            {
                ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).niveau = 4;
                ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).Suivant();
                ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).ControlContent.Content = null;
            }
        }

        private void retour_Click(object sender, RoutedEventArgs e)
        {
            ChoixNiveau choixDuNiveau = new ChoixNiveau();
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/castle.jpg", UriKind.RelativeOrAbsolute));
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).canvasMainWindow.Background = imageBrush;
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).ControlContent.Content = new Accueil();

        }
        public void ChangerCouleurEllipseNiveau(int niveau)
        {



            if (niveau >= 1)
            {
                ellipseNiveau1.Fill = Brushes.Green;
            }
            if (niveau >=2)
            {
                ellipseNiveau2.Fill = Brushes.Green;
            }
            if (niveau >= 3)
            {
                ellipseNiveau3.Fill = Brushes.Green;
            }
        }

    }
}
