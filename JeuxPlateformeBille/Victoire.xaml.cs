﻿using System;
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
    /// Logique d'interaction pour Victoire.xaml
    /// </summary>
    public partial class Victoire : UserControl
    {
        public Victoire()
        {
            InitializeComponent();
            
        }

        private void butRecommencer_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).niveau = 0;
            ChoixNiveau choixDuNiveau = new ChoixNiveau();
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/castle.jpg", UriKind.RelativeOrAbsolute));
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).canvasMainWindow.Background = imageBrush;
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).ControlContent.Content = new Accueil();
        }
    }
}
