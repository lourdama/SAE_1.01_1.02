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
    /// Logique d'interaction pour Accueil.xaml
    /// </summary>
    public partial class Accueil : UserControl
    {
        public Accueil()
        {
            InitializeComponent();
        }

        private void butJouer_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).Suivant();
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).ControlContent.Content = null;

        }

        private void butParametres_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).ControlContent.Content = new Parametres();
        }
    }
}
