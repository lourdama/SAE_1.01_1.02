using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Policy;
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
        private static BitmapImage imgBille, fond;
        private bool gauche, droite, saut, enSaut, billeBouge = false;
        private int vitesseJoueur = 8, gravite = 8, toleranceColision = 8, nbtouche = 0, nbStockBille = 10;
        System.Drawing.Rectangle hitBoxSol, hitBoxJoueur,hitBoxBille, hitBoxEnnemi;
        private static Point clickPosition;
        private static double vitesseSaut, graviteBille = 4;
        private static int[,,] typeBilles = { { },
                                          { },
                                          { } };
        private static int[,,] typeEnnemis = { { },
                                           { },
                                           { } };

        private static List<Image> billesEnJeu = billesEnJeu = new List<Image>();
        private static List<double[]> vitesseBilles = new List<double[]>();

        public MainWindow()
        {
            InitializeComponent();
            canvasMainWindow.Focus();
            imgBille = new BitmapImage(new Uri("pack://application:,,,/img/balle.jpg"));
            hitBoxSol = new System.Drawing.Rectangle((int)Canvas.GetLeft(sol), (int)Canvas.GetTop(sol) - gravite/2, (int)sol.Width, (int)sol.Height);
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
            tir(e);
        }

        

        private void Jeu(object? sender, EventArgs e)
        {
            deplacement();
            deplacementEnnemi();
            if (VerifTouche())
            {
                FinJeu();
            }

            for (int i = 0; i < billesEnJeu.Count; i++)
            {
                if (billeLance(billesEnJeu[i], vitesseBilles[i]))
                {
                    canvasMainWindow.Children.Remove(billesEnJeu[i]);
                    billesEnJeu.RemoveAt(i);
                }
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

               /* if (Canvas.GetTop(joueur) > hitBoxSol.Top - joueur.Height + gravite + toleranceColision)
                {
                    Canvas.SetTop(joueur, hitBoxSol.Top - joueur.Height);
                } Remonter automatiquement sur plateforme si touché*/
                gravite = 0;
                ReinitialisationSaut();
            }
             
            else
            {
                gravite = 8;
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
                    vitesseSaut = vitesseSaut + gravite / 6;
                    Canvas.SetLeft(barreSaut, Canvas.GetLeft(barreSaut) - 5);
                }
            }
        }

        private void tir(MouseButtonEventArgs e)
        {
            if (nbStockBille > 0)
            {
                clickPosition = e.GetPosition(this);
                billesEnJeu.Insert(0, new Image());
                billesEnJeu[0].Source = imgBille;
                billesEnJeu[0].Width = 15;
                billesEnJeu[0].Height = 15;
                canvasMainWindow.Children.Add(billesEnJeu[0]);
                Canvas.SetTop(billesEnJeu[0], Canvas.GetTop(joueur));
                Canvas.SetLeft(billesEnJeu[0], Canvas.GetLeft(joueur));
                vitesseBilles.Insert(0, new double[2]);
                vitesseBilles[0] = [clickPosition.X - Canvas.GetLeft(billesEnJeu[0]), clickPosition.Y - Canvas.GetTop(billesEnJeu[0])];
                billesEnJeu[0].Visibility = Visibility.Visible;
                billeBouge = true;
                nbStockBille = nbStockBille - 1;
                StockBille.Content = "Stock De Billes : " + nbStockBille;
            }
        }
        private bool billeLance(Image bille, double[] vitesse)
        {
            
            if (Canvas.GetLeft(bille) < 0 || Canvas.GetLeft(bille) > this.ActualWidth || Canvas.GetTop(bille) > this.ActualHeight)
            {
                return true;
            }
            else
            {
                Canvas.SetLeft(bille, Canvas.GetLeft(bille) + (vitesse[0] / 25));
                Canvas.SetTop(bille, Canvas.GetTop(bille) + vitesse[1] / 25);
                vitesse[1] = vitesse[1] + graviteBille;
                vitesse[0] = vitesse[0] * 0.985;
                hitBoxBille = new System.Drawing.Rectangle((int)Canvas.GetLeft(bille), (int)Canvas.GetTop(bille), (int)bille.Width, (int)bille.Height);
                colisionEnnemi();
            }
            hitBoxBille = new System.Drawing.Rectangle((int)Canvas.GetLeft(bille), (int)Canvas.GetTop(bille), (int)bille.Width - 1, (int)bille.Height - 1);
            if (hitBoxBille.IntersectsWith(hitBoxSol))
            {
                Canvas.SetLeft(joueur, Canvas.GetLeft(bille));
                Canvas.SetTop(joueur, Canvas.GetTop(bille) - joueur.Height);
                return true;
            }
            return false;
            
            
            
        }

        private void deplacementEnnemi()
        {
            /*Canvas.SetLeft(ennemi, Canvas.GetLeft(ennemi) + Math.Sign(Canvas.GetLeft(joueur) - Canvas.GetLeft(ennemi)) * 2);
            Canvas.SetTop(ennemi, Canvas.GetTop(ennemi) + Math.Sign(Canvas.GetTop(joueur) - Canvas.GetTop(ennemi)) );
            hitBoxEnnemi = new System.Drawing.Rectangle((int)Canvas.GetLeft(ennemi), (int)Canvas.GetTop(ennemi), (int)ennemi.Width, (int)ennemi.Height);
            Canvas.SetLeft(EnnemiVie, Canvas.GetLeft(ennemi) + 15);
            Canvas.SetLeft(EnnemiVie2, Canvas.GetLeft(ennemi) + 30);
            Canvas.SetTop(EnnemiVie, Canvas.GetTop(ennemi) - 10);
            Canvas.SetTop(EnnemiVie2, Canvas.GetTop(ennemi) - 10);*/
        }

        private void colisionEnnemi()
        {
            if (hitBoxBille.IntersectsWith(hitBoxEnnemi) )
            {
                //bille.Visibility = Visibility.Hidden;
                //Canvas.SetLeft(bille, -10);
                if (nbtouche == 1)
                {
                    //ennemi.Visibility = Visibility.Hidden;
                    //EnnemiVie.Visibility = Visibility.Hidden;
                    //EnnemiVie2.Visibility = Visibility.Hidden;
                    ReinitialisationSaut();
                }
                else
                {
                    nbtouche = nbtouche + 1;
                    //EnnemiVie2.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                }
            }
        }
        private void deplacementEnnemi2()
        {

        }
        private bool VerifTouche()
        {
            /*if (ennemi.Visibility == Visibility.Visible)
            {
                hitBoxJoueur = new System.Drawing.Rectangle((int)Canvas.GetLeft(joueur), (int)Canvas.GetTop(joueur), (int)joueur.Width - 2, (int)joueur.Height - 2);
                hitBoxEnnemi = new System.Drawing.Rectangle((int)Canvas.GetLeft(ennemi), (int)Canvas.GetTop(ennemi), (int)ennemi.Width - 2, (int)ennemi.Height - 2);
                bool ennemiTouche = hitBoxEnnemi.IntersectsWith(hitBoxJoueur);
                return ennemiTouche;
            }*/
            return false;    
        }

        private void ReinitialisationSaut()
        {
            enSaut = false;
            vitesseSaut = -35;
            Canvas.SetLeft(barreSaut, 0);
        }
        private void FinJeu()
        {
            minuterie.Stop();
        }
    }
}