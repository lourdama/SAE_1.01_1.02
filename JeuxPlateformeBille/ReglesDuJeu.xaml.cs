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
    /// Logique d'interaction pour ReglesDuJeu.xaml
    /// </summary>
    public partial class ReglesDuJeu : UserControl
    {
        public ReglesDuJeu()
        {
            InitializeComponent();
        }

        private void butRetour_Click(object sender, RoutedEventArgs e)
        {
            ChoixNiveau choixDuNiveau = new ChoixNiveau();
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/castle.jpg", UriKind.RelativeOrAbsolute));
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).canvasMainWindow.Background = imageBrush;
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).ControlContent.Content = new Accueil();
        }
    }
}
