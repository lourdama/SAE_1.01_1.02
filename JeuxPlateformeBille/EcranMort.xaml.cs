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
using static System.Net.Mime.MediaTypeNames;

namespace JeuxPlateformeBille
{
    /// <summary>
    /// Logique d'interaction pour EcranMort.xaml
    /// </summary>
    public partial class EcranMort : UserControl
    {
        public EcranMort()
        {
            InitializeComponent();
        }

        private void butAccueil_Click(object sender, RoutedEventArgs e)
        {
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/img/castle.jpg", UriKind.RelativeOrAbsolute));
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).canvasMainWindow.Background = imageBrush;
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).ControlContent.Content = new Accueil();
        }

        private void butRejouer_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).niveau ++;
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).Suivant();
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).ControlContent.Content = null;
            
        }
    }
}
