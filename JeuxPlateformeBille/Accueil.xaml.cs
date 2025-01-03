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
            ChoixNiveau choixDuNiveau = new ChoixNiveau();
            choixDuNiveau.ChangerCouleurEllipseNiveau(((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).niveau);
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/choixduniveau.jpg", UriKind.RelativeOrAbsolute));
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).canvasMainWindow.Background = imageBrush;
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).ControlContent.Content = choixDuNiveau;

        }

        private void butParametres_Click(object sender, RoutedEventArgs e)
        {

            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).ControlContent.Content = new Parametres();
        }

        private void butQuitter_Click(object sender, RoutedEventArgs e)
        {
           ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).Close();
        }

        private void butRegle_Click(object sender, RoutedEventArgs e)
        {
            ReglesDuJeu reglesdujeu = new ReglesDuJeu();
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/reglesdujeu.png", UriKind.RelativeOrAbsolute));
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).canvasMainWindow.Background = imageBrush;
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).ControlContent.Content = reglesdujeu;
        }

        private void butCredits_Click(object sender, RoutedEventArgs e)
        {
            ReglesDuJeu reglesdujeu = new ReglesDuJeu();
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/credits.png", UriKind.RelativeOrAbsolute));
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).canvasMainWindow.Background = imageBrush;
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).ControlContent.Content = reglesdujeu;
        }
    }
}
