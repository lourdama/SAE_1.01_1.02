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
    /// Logique d'interaction pour Pause.xaml
    /// </summary>
    public partial class Pause : UserControl
    {
        public Pause()
        {
            InitializeComponent();
        }

        private void Pause1_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).Reprendre();
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).ControlContent.Content = null;
        }
    }
}
