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

namespace JeuxPlateformeBille
{
    /// <summary>
    /// Logique d'interaction pour Parametres.xaml
    /// </summary>
    public partial class Parametres : UserControl
    {
        public Key toucheGaucheUC = Key.Q;
        private Key toucheDroiteUC = Key.D;
        private Key toucheSautUC = Key.Space;
        private double volumeUC = 1;

        private List<Key> touchesDisponibles = new List<Key> // liste des key disponibles pour la personnalisation des touches
    {
        Key.Q, Key.D, Key.Z, Key.S, Key.A, Key.E, Key.Left, Key.Right, Key.Up, Key.Down, Key.Space, Key.W
    };
        public Parametres()
        {
            InitializeComponent();
            RemplirComboBox();
            slideBarMusique.Value = volumeUC;
        }
        private void RemplirComboBox()
        {
            //remplissage des listes déroulantes avec la liste des propositions
            foreach (Key touche in touchesDisponibles)
            {
                ComboBoxGauche.Items.Add(touche);
                ComboBoxDroite.Items.Add(touche);
                ComboBoxSaut.Items.Add(touche);
            }

            /*toucheDroiteUC = ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).toucheDroite;
            toucheGaucheUC = ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).toucheGauche;
            toucheSautUC = ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).toucheSaut;
            volumeUC = ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).volumeMusique;*/
            ComboBoxGauche.SelectedItem = toucheGaucheUC;
            ComboBoxDroite.SelectedItem = toucheDroiteUC;
            ComboBoxSaut.SelectedItem = toucheSautUC;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // quand une selection est changée, met dans une variable locale la key selectionnée
            if (ComboBoxGauche.SelectedItem != null)
                toucheGaucheUC = (Key)ComboBoxGauche.SelectedItem;

            if (ComboBoxDroite.SelectedItem != null)
                toucheDroiteUC = (Key)ComboBoxDroite.SelectedItem;

            if (ComboBoxSaut.SelectedItem != null)
                toucheSautUC = (Key)ComboBoxSaut.SelectedItem;
        }


        private void retour_Click(object sender, RoutedEventArgs e)
        {
            // quand retour est cliqué, changement des variables dans le main en fonction des paramètres selectionnés
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).toucheDroite = toucheDroiteUC;
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).toucheGauche = toucheGaucheUC;
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).toucheSaut = toucheSautUC;
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).volumeMusique = volumeUC;
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).ModifVolumeMusique(volumeUC);
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).ControlContent.Content = new Accueil();
        }
        // ci-dessous change la variable de difficulté dans la mainwindow en fonction du bouton cliqué
        private void butFacile_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).difficulte = 1;
        }

        private void butModere_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).difficulte = 2;
        }

        private void butDifficile_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)((Canvas)((ContentControl)this.Parent).Parent).Parent).difficulte = 3;
        }

        private void slideBarMusique_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //si valeur bar glissante changée alors stockée dans une variable locale
            volumeUC = slideBarMusique.Value;
        }
    }
}
