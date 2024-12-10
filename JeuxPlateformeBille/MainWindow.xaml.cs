using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace JeuxPlateformeBille
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer minuterie;
        private bool gauche, droite, saut, enSaut, billeBouge = false;
        private int vitesseJoueur = 8, gravite = 3, toleranceColision = 8;
        System.Drawing.Rectangle hitBoxSol, hitBoxJoueur;
        private static Point clickPosition;
        private static double vitesseBilleX, vitesseBilleY, vitesseSaut, graviteBille = 4;

        public MainWindow()
        {
            InitializeComponent();
            hitBoxSol = new System.Drawing.Rectangle((int)Canvas.GetLeft(sol), (int)Canvas.GetTop(sol), (int)sol.Width, (int)sol.Height);
            this.KeyDown += new KeyEventHandler(Window_KeyDown);
            this.KeyUp += new KeyEventHandler(Window_KeyUp);
            InitTimer();   
            
        }

        private void InitTimer()
        {
            minuterie = new DispatcherTimer();
            minuterie.Interval = TimeSpan.FromMilliseconds(17);
            minuterie.Tick += Jeu;
            minuterie.Start();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                gauche = true;
            }

            else if (e.Key == Key.D)
            {
                droite = true;
            }

            else if (e.Key == Key.Space)
            {
                saut = true;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                gauche = false;
            }
            else if (e.Key == Key.D) 
            {
                droite = false;
            }
                
            else if (e.Key == Key.Space)
            {
                saut = false;
            }
            else if (e.Key == Key.Space)
            {
                saut = false;
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            clickPosition = e.GetPosition(this);
            Canvas.SetTop(bille, Canvas.GetTop(joueur));
            Canvas.SetLeft(bille, Canvas.GetLeft(joueur));
            vitesseBilleX = (clickPosition.X - Canvas.GetLeft(bille));
            vitesseBilleY = (clickPosition.Y - Canvas.GetTop(bille));
            bille.Visibility = Visibility.Visible;
            billeBouge = true;
        }

        private void Jeu(object? sender, EventArgs e)
        {
            deplacement();
            deplacementEnnemi();
            if (billeBouge)
            {
                billeLance();
            }
            if (enSaut)
            {
                SautEnCours();
            }
            

        }
        private void deplacement()
        {

             
            if (auSol())
            {

                if (Canvas.GetTop(joueur) > hitBoxSol.Top - joueur.Height + gravite + toleranceColision)
                {
                    Canvas.SetTop(joueur, hitBoxSol.Top - joueur.Height);
                }
                gravite = 0;
                enSaut = false;
                vitesseSaut = -20;
                Canvas.SetLeft(barreSaut, 0);
            }
             
            else
            {
                gravite = 5;
            }
            

            Canvas.SetTop(joueur, Canvas.GetTop(joueur) + gravite);
            
            if (droite)
            {
                if ((Canvas.GetLeft(joueur) + vitesseJoueur) + joueur.Width < this.ActualWidth)
                {
                    Canvas.SetLeft(joueur, Canvas.GetLeft(joueur) + vitesseJoueur);
                }
                
            }

            if (gauche)
            {
                if ((Canvas.GetLeft(joueur) - vitesseJoueur) > 0)
                {
                    Canvas.SetLeft(joueur, Canvas.GetLeft(joueur) - vitesseJoueur);
                }
                    
            }
            if (saut && enSaut == false)
            {
                enSaut = true;
            }

        }
        private bool auSol()
        {

            hitBoxJoueur = new System.Drawing.Rectangle((int)Canvas.GetLeft(joueur), (int)Canvas.GetTop(joueur), (int)joueur.Width, (int)joueur.Height);
            bool estAuSol = hitBoxSol.IntersectsWith(hitBoxJoueur);
            return estAuSol;
        }
        private void SautEnCours()
        {
            if (vitesseSaut < 0)
            {
                if (saut)
                {
                    Canvas.SetTop(joueur, Canvas.GetTop(joueur) + vitesseSaut);
                    vitesseSaut = vitesseSaut + gravite / 5;
                    Canvas.SetLeft(barreSaut, Canvas.GetLeft(barreSaut) - 5);
                }
                
            }
            
        }

        private void billeLance()
        {
            if (Canvas.GetLeft(bille) < 0 || Canvas.GetLeft(bille) > this.ActualWidth || Canvas.GetTop(bille) < 0 || Canvas.GetTop(bille) > this.ActualHeight)
            {
                billeBouge = false;
                bille.Visibility = Visibility.Hidden;
            }
            else
            {
                Canvas.SetLeft(bille, Canvas.GetLeft(bille) + (vitesseBilleX / 25));
                Canvas.SetTop(bille, Canvas.GetTop(bille) + vitesseBilleY / 25);
                vitesseBilleY = vitesseBilleY + graviteBille;
                vitesseBilleX = vitesseBilleX * 0.985;
            }
            
        }

        private void deplacementEnnemi()
        {
            Canvas.SetLeft(ennemi, Canvas.GetLeft(ennemi) + Math.Sign(Canvas.GetLeft(joueur) - Canvas.GetLeft(ennemi)) * 3);
            Canvas.SetTop(ennemi, Canvas.GetTop(ennemi) + Math.Sign(Canvas.GetTop(joueur) - Canvas.GetTop(ennemi)) *3);
        }
    }
}