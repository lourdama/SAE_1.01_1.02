using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
using System.Windows.Media.TextFormatting;
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
        private int vitesseJoueur = 8, gravite = 8, toleranceColision = 8, nbtouche = 0, nbStockBille = 1000;
        System.Drawing.Rectangle hitBoxSol, hitBoxJoueur, hitBoxBille, hitBoxEnnemi;
        private static Point clickPosition;
        private static double vitesseSaut, graviteBille = 4, coefReductionDeplacementSaut;
        /*  private static List<Image> billesEnJeu = billesEnJeu = new List<Image>();
          private static List<double[]> vitesseBilles = new List<double[]>();
          private static List<Image> ennemisEnJeu = ennemisEnJeu = new List<Image>();
          private static List<double[]> vitesseEnnemis = new List<double[]>();
          private static List<double[]> InitVarEnnemis = new List<double[]>();*/
        private static Ennemis fantome = new Ennemis();
        private static List<Ennemis> ennemisEnJeu = new List<Ennemis>();
        private static List<Billes> billesEnJeu = new List<Billes>();
        private static List<Plateformes> plateformesEnJeu = new List<Plateformes>();
        private static int[,] coordonneesPlateformes =
        {
            {50, 500,1000 },
            {300, 600, 900 }
        };
        private static Random aleatoire = new Random();

        public MainWindow()
        {
            InitializeComponent();

        }
        private void butJouer_Click(object sender, RoutedEventArgs e)
        {
            Suivant();
        }
        private void butCredits_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new Parametres();

        }
        private void Suivant()
        {
            InitJeu();
            InitTimer();
            InitEnnemis();
            InitPlateformes();
            //spawnEnnemi();
        }

        private void InitJeu()
        {
            canvasMainWindow.Focus();
            imgBille = new BitmapImage(new Uri("pack://application:,,,/img/balle.jpg"));
            hitBoxSol = new System.Drawing.Rectangle((int)Canvas.GetLeft(sol), (int)Canvas.GetTop(sol) - gravite / 2, (int)sol.Width, (int)sol.Height);
            joueur.Visibility = Visibility.Visible;
            sol.Visibility = Visibility.Visible;
            barreSaut.Visibility = Visibility.Visible;
            StockBille.Visibility = Visibility.Visible;
            butCredits.Visibility = Visibility.Hidden;
            butJouer.Visibility = Visibility.Hidden;
            butParametres.Visibility = Visibility.Hidden;
            butQuitter.Visibility = Visibility.Hidden;
            butRegle.Visibility = Visibility.Hidden;

        }

        private void InitEnnemis()
        {
            // Initialisation ennemi classique
            fantome.Texture = new Image();
            fantome.Texture.Source = imgBille;
            fantome.Texture.Width = 50;
            fantome.Texture.Height = 30;
            fantome.CoordonneeX = 500;
            fantome.CoordonneeY = 500;
            fantome.Vitesse = 2;
            ennemisEnJeu.Insert(0, fantome);
            canvasMainWindow.Children.Add(ennemisEnJeu[0].Texture);
            Canvas.SetTop(ennemisEnJeu[0].Texture, ennemisEnJeu[0].CoordonneeY);
            Canvas.SetLeft(ennemisEnJeu[0].Texture, ennemisEnJeu[0].CoordonneeX);
        }


        private void InitTimer()
        {
            minuterie = new DispatcherTimer();
            minuterie.Interval = TimeSpan.FromMilliseconds(17);
            minuterie.Tick += Jeu;
            minuterie.Start();

        }

        private void InitPlateformes()
        {
            for (int i = 0; i < coordonneesPlateformes.GetLength(1); i++)
            {
                Plateformes nouvellePlateforme = new Plateformes(new Image(), new int(), new int(), new System.Drawing.Rectangle()); 
                nouvellePlateforme.Texture.Source = new BitmapImage(new Uri("pack://application:,,,/img/plateforme.png"));
                nouvellePlateforme.Texture.Width = 300;
                //nouvellePlateforme.Texture.Height = 25;
                nouvellePlateforme.BoiteCollision = new System.Drawing.Rectangle(coordonneesPlateformes[0, i], coordonneesPlateformes[1, i], (int)nouvellePlateforme.Texture.Width, (int)nouvellePlateforme.Texture.Height);
                plateformesEnJeu.Insert(i, nouvellePlateforme);
                canvasMainWindow.Children.Add(nouvellePlateforme.Texture);
                Canvas.SetTop(nouvellePlateforme.Texture, coordonneesPlateformes[0, i]);
                Canvas.SetLeft(nouvellePlateforme.Texture, coordonneesPlateformes[1, i]);
               
                
                
            }
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

        private void butQuitter_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
                if (billeLance(billesEnJeu[i]))
                {

                    canvasMainWindow.Children.Remove(billesEnJeu[i].Texture);
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
                coefReductionDeplacementSaut = 1;
                ReinitialisationSaut();
            }

            else
            {
                gravite = 8;
            }


            Canvas.SetTop(joueur, Canvas.GetTop(joueur) + gravite);

            if (droite)
            {
                if ((Canvas.GetLeft(joueur) + vitesseJoueur * coefReductionDeplacementSaut) + joueur.Width < this.ActualWidth)
                {
                    Canvas.SetLeft(joueur, Canvas.GetLeft(joueur) + vitesseJoueur * coefReductionDeplacementSaut);
                }

            }

            if (gauche)
            {
                if ((Canvas.GetLeft(joueur) - vitesseJoueur * coefReductionDeplacementSaut) > 0)
                {
                    Canvas.SetLeft(joueur, Canvas.GetLeft(joueur) - vitesseJoueur * coefReductionDeplacementSaut);
                }


            }
            if (saut && enSaut == false)
            {
                enSaut = true;
                coefReductionDeplacementSaut = 0.5;
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
                Billes nouvelleBille = new Billes(new Image(), 0, 0);
                nouvelleBille.Texture.Source = imgBille;
                nouvelleBille.Texture.Width = 15;
                nouvelleBille.Texture.Height = 15;
                clickPosition = e.GetPosition(this);
                double vitesseX = clickPosition.X - Canvas.GetLeft(joueur);
                double vitesseY = clickPosition.Y - Canvas.GetTop(joueur);
                nouvelleBille.Vitesse[0] = vitesseX; 
                nouvelleBille.Vitesse[1] = vitesseY;
                billesEnJeu.Insert(0, nouvelleBille);
                canvasMainWindow.Children.Add(nouvelleBille.Texture);
                Canvas.SetTop(nouvelleBille.Texture, Canvas.GetTop(joueur));
                Canvas.SetLeft(nouvelleBille.Texture, Canvas.GetLeft(joueur));
                nbStockBille--;
                StockBille.Content = "Stock De Billes : " + nbStockBille;
                
                
            }
        }
        private bool billeLance(Billes bille)
        {

            if (Canvas.GetLeft(bille.Texture) < 0 || Canvas.GetLeft(bille.Texture) > this.ActualWidth || Canvas.GetTop(bille.Texture) > this.ActualHeight)
            {
                return true;
            }
            else
            {
                Canvas.SetLeft(bille.Texture, Canvas.GetLeft(bille.Texture) + (bille.Vitesse[0] / 25));
                Canvas.SetTop(bille.Texture, Canvas.GetTop(bille.Texture) + bille.Vitesse[1] / 25);
                bille.Vitesse[1] = bille.Vitesse[1] + graviteBille;
                bille.Vitesse[0] = bille.Vitesse[0] * 0.985;
                hitBoxBille = new System.Drawing.Rectangle((int)Canvas.GetLeft(bille.Texture), (int)Canvas.GetTop(bille.Texture), (int)bille.Texture.Width, (int)bille.Texture.Height);
                if (colisionEnnemi())
                {
                    return true;
                }
            }
            hitBoxBille = new System.Drawing.Rectangle((int)Canvas.GetLeft(bille.Texture), (int)Canvas.GetTop(bille.Texture), (int)bille.Texture.Width - 1, (int)bille.Texture.Height - 1);
            if (hitBoxBille.IntersectsWith(hitBoxSol))
            {
                Canvas.SetLeft(joueur, Canvas.GetLeft(bille.Texture));
                Canvas.SetTop(joueur, Canvas.GetTop(bille.Texture) - joueur.Height);
                return true;
            }
            return false;



        }
        /*  private void spawnEnnemi()
          {
              ennemisEnJeu.Insert(0, new Image());
              ennemisEnJeu[0].Source = imgBille;
              ennemisEnJeu[0].Width = 30;
              ennemisEnJeu[0].Height = 50;
              canvasMainWindow.Children.Add(ennemisEnJeu[0]);
              Canvas.SetTop(ennemisEnJeu[0], aleatoire.Next(0, 900));
              Canvas.SetLeft(ennemisEnJeu[0], aleatoire.Next(0, 1500));
              vitesseEnnemis.Insert(0, new double[2]);
              vitesseEnnemis[0] = [2];
          }*/

        private void deplacementEnnemi()
        {
            Canvas.SetLeft(ennemisEnJeu[0].Texture, Canvas.GetLeft(ennemisEnJeu[0].Texture) + Math.Sign(Canvas.GetLeft(joueur) - Canvas.GetLeft(ennemisEnJeu[0].Texture)) * 2);
            Canvas.SetTop(ennemisEnJeu[0].Texture, Canvas.GetTop(ennemisEnJeu[0].Texture) + Math.Sign(Canvas.GetTop(joueur) - Canvas.GetTop(ennemisEnJeu[0].Texture)));
            hitBoxEnnemi = new System.Drawing.Rectangle((int)Canvas.GetLeft(ennemisEnJeu[0].Texture), (int)Canvas.GetTop(ennemisEnJeu[0].Texture), (int)ennemisEnJeu[0].Texture.Width, (int)ennemisEnJeu[0].Texture.Height);
            /* Canvas.SetLeft(EnnemiVie, Canvas.GetLeft(ennemi) + 15);
             Canvas.SetLeft(EnnemiVie2, Canvas.GetLeft(ennemi) + 30);
             Canvas.SetTop(EnnemiVie, Canvas.GetTop(ennemi) - 10);
             Canvas.SetTop(EnnemiVie2, Canvas.GetTop(ennemi) - 10);*/
        }

        private bool colisionEnnemi()
        {
            if (hitBoxBille.IntersectsWith(hitBoxEnnemi))
            {
                if (nbtouche == 1)
                {
                    ennemisEnJeu[0].Texture.Visibility = Visibility.Hidden;
                    //EnnemiVie.Visibility = Visibility.Hidden;
                    //EnnemiVie2.Visibility = Visibility.Hidden;
                    ReinitialisationSaut();
                }
                else
                {
                    nbtouche = nbtouche + 1;
                    //EnnemiVie2.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                }
                return true;
            }
            return false;
        }
        private void deplacementEnnemi2()
        {

        }
        private bool VerifTouche()
        {
            if (ennemisEnJeu[0].Texture.Visibility == Visibility.Visible)
            {
                hitBoxJoueur = new System.Drawing.Rectangle((int)Canvas.GetLeft(joueur), (int)Canvas.GetTop(joueur), (int)joueur.Width - 2, (int)joueur.Height - 2);
                hitBoxEnnemi = new System.Drawing.Rectangle((int)Canvas.GetLeft(ennemisEnJeu[0].Texture), (int)Canvas.GetTop(ennemisEnJeu[0].Texture), (int)ennemisEnJeu[0].Texture.Width - 2, (int)ennemisEnJeu[0].Texture.Height - 2);
                bool ennemiTouche = hitBoxEnnemi.IntersectsWith(hitBoxJoueur);
                return ennemiTouche;
            }
            return false;
        }

        private void ReinitialisationSaut()
        {
            enSaut = false;
            vitesseSaut = -25;
            Canvas.SetLeft(barreSaut, 0);
        }
        private void FinJeu()
        {
            minuterie.Stop();
        }
    }
    public partial class Ennemis
    {
        private int coordonneeX, coordonneeY, typeDeplacement;
        private Image texture;
        private double vitesse;

        public Image Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        public int CoordonneeX
        {
            get { return coordonneeX; }
            set { coordonneeX = value; }
        }
        public int CoordonneeY
        {
            get { return coordonneeY; }
            set { coordonneeY = value; }
        }
        public int TypeDeplacement
        {
            get { return typeDeplacement; }
            set { typeDeplacement = value; }
        }
        public double Vitesse
        {
            get { return vitesse; }
            set { vitesse = value; }
        }
    }
    public class Billes
    {
        public Image Texture { get; set; }
        public double[] Vitesse { get; set; }  

        public Billes(Image texture, double vitesseX, double vitesseY)
        {
            this.Texture = texture;
            this.Vitesse = new double[] { vitesseX, vitesseY };
        }
    }
    public partial class Plateformes
    {
        public Image Texture { get; set; }
        public int CoordonneeX { get; set; }
        public int CoordonneeY { get; set; }
        public System.Drawing.Rectangle BoiteCollision { get; set; }
        public Plateformes(Image texture, int coordonneeX, int coordonneeY, System.Drawing.Rectangle boiteCollision)
        {
            this.Texture = texture;
            this.CoordonneeX = coordonneeX;
            this.CoordonneeY = coordonneeY;
            this.BoiteCollision = boiteCollision;
        }
    }
}