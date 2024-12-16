using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
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
        public DispatcherTimer minuterie;
        private static BitmapImage imgBille, fond;
        private bool gauche, droite, saut, enSaut, billeBouge, pause = false;
        private int vitesseJoueur = 8, gravite = 8, toleranceColision = 12, nbtouche = 0, nbStockBille = 1000, niveau = 0, choixBille;
        System.Drawing.Rectangle hitBoxSol, hitBoxJoueur, hitBoxBille, hitBoxEnnemi;
        private int animationJoueur = 1, animationSaut = 1, animationStatic = 1, timerAnimation, timerAnimationSaut, timerAnimationStatic;
        private static Point clickPosition;
        private static double vitesseSaut, graviteBille = 4, coefReductionDeplacementSaut;
        private static Ennemis fantome = new Ennemis();
        private static List<ProgressBar> barreDeVie = new List<ProgressBar>();
        private static List<Ennemis> ennemisEnJeu = new List<Ennemis>();
        private static List<Billes> billesEnJeu = new List<Billes>();
        private static List<Plateformes> plateformesEnJeu = new List<Plateformes>();

        int[][,] proprietePlateformes = new int[][,]
        {
         new int[,] { { 425, 700 }, { 850, 700 }, { 1275, 700 },{ 600, 500 }, {300, 200 }, { 0, 700 }    },
         new int[,] { { 425, 700 }, { 850, 700 }, { 1275, 700 } },
         new int[,] { { 425, 700 }, { 850, 700 }, { 1275, 700 }},
         new int[,] { { 425, 700 }, { 850, 700 }, { 1275, 700 } },
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
        public void Suivant()
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


        }

        private void InitEnnemis()
        {
            // Initialisation ennemi classique
            fantome.Texture = new Image();
            fantome.Texture.Source = new BitmapImage(new Uri("pack://application:,,,/img/fantome.png"));
            fantome.Texture.Width = 50;
            fantome.Texture.Height = 100;
            fantome.CoordonneeX = 500;
            fantome.CoordonneeY = 500;
            fantome.Vitesse = 2;
            fantome.PointDeVie = 100;
            fantome.BarreDeVie = new ProgressBar();
            fantome.BarreDeVie.Height = 10;
            fantome.BarreDeVie.Width = 75;
            fantome.BarreDeVie.Value = 100;
            ennemisEnJeu.Insert(0, fantome);
            canvasMainWindow.Children.Add(ennemisEnJeu[0].Texture);
            canvasMainWindow.Children.Add(ennemisEnJeu[0].BarreDeVie);
            Canvas.SetTop(ennemisEnJeu[0].Texture, ennemisEnJeu[0].CoordonneeY);
            Canvas.SetLeft(ennemisEnJeu[0].Texture, ennemisEnJeu[0].CoordonneeX);
            Canvas.SetTop(ennemisEnJeu[0].BarreDeVie, ennemisEnJeu[0].CoordonneeY);
            Canvas.SetLeft(ennemisEnJeu[0].BarreDeVie, ennemisEnJeu[0].CoordonneeX);

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
            for (int i = 0; i < proprietePlateformes[niveau].GetLength(0); i++)
            {
                Plateformes nouvellePlateforme = new Plateformes(new Image(), new int(), new int(), new System.Drawing.Rectangle()); 
                nouvellePlateforme.Texture.Source = new BitmapImage(new Uri("pack://application:,,,/img/plateforme.png"));
                nouvellePlateforme.Texture.Width = 425;
                nouvellePlateforme.Texture.Height = 116;
                plateformesEnJeu.Insert(0, nouvellePlateforme);
                canvasMainWindow.Children.Add(plateformesEnJeu[0].Texture);

                Canvas.SetLeft(plateformesEnJeu[0].Texture, proprietePlateformes[niveau][i, 0]);
                Canvas.SetTop(plateformesEnJeu[0].Texture, proprietePlateformes[niveau][i, 1]);
                plateformesEnJeu[0].BoiteCollision = new System.Drawing.Rectangle((int)Canvas.GetLeft(plateformesEnJeu[0].Texture) - toleranceColision , (int)Canvas.GetTop(plateformesEnJeu[0].Texture), (int)plateformesEnJeu[0].Texture.Width + 2* toleranceColision, (int)plateformesEnJeu[0].Texture.Height);
            }
        }
        private void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0 && choixBille< 10)
            {
                choixBille = choixBille + 1;
            }
                

            else if (e.Delta < 0 && choixBille > 0 )
            {
                choixBille = choixBille -1;    
            }
            ChoixBille.Content = choixBille;

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
            else if(e.Key == Key.P)
            {
                if (!pause)
                {
                    pause = true;
                    minuterie.Stop();
                    this.ControlContent.Content = new Pause();
                }
                
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

        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!pause)
            {
                Tir(e);
            }
        }

        private void butQuitter_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void Reprendre()
        {
            

            minuterie.Start();
            pause = false;
            this.ControlContent.Content = null;
        }
        private void Jeu(object? sender, EventArgs e)
        {
            Deplacement();
            DeplacementEnnemi();
            if (VerifTouche())
            {
                FinJeu();
            }

            for (int i = 0; i < billesEnJeu.Count; i++)
            {
                if (BilleLance(billesEnJeu[i]))
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
        private void Deplacement()
        {

            if (CollisionPlat() == 0)
            {
                gravite = 0;
                coefReductionDeplacementSaut = 1;
                ReinitialisationSaut();
            }

            else
            {
                AnimationChute();
                gravite = 8;
            }


            Canvas.SetTop(joueur, Canvas.GetTop(joueur) + gravite);

            if (droite)
            {
                if ((Canvas.GetLeft(joueur) + vitesseJoueur * coefReductionDeplacementSaut) + joueur.Width < this.ActualWidth && CollisionPlat() != 1)
                {
                    Canvas.SetLeft(joueur, Canvas.GetLeft(joueur) + vitesseJoueur * coefReductionDeplacementSaut);
                    AnimationDeplacementJoueur(1);
                }

            }

            if (gauche)
            {
                if ((Canvas.GetLeft(joueur) - vitesseJoueur * coefReductionDeplacementSaut) > 0 && CollisionPlat() != 2)
                {
                    Canvas.SetLeft(joueur, Canvas.GetLeft(joueur) - vitesseJoueur * coefReductionDeplacementSaut);
                    AnimationDeplacementJoueur(-1);
                }


            }
            if (saut && enSaut == false)
            {
                enSaut = true;
                coefReductionDeplacementSaut = 0.5;
            }

        }

        


        private int CollisionPlat()
        {
            hitBoxJoueur = new System.Drawing.Rectangle((int)Canvas.GetLeft(joueur), (int)Canvas.GetTop(joueur), (int)joueur.Width, (int)joueur.Height);
            for (int i = 0; i < plateformesEnJeu.Count; i++)
            {
                if (hitBoxJoueur.IntersectsWith(plateformesEnJeu[i].BoiteCollision))
                {

                    if (Canvas.GetTop(joueur) + joueur.Height < Canvas.GetTop(plateformesEnJeu[i].Texture) + toleranceColision)
                    {
                        if (!droite && !gauche)
                        {
                            AnimationStatic();
                        }
                        return 0;
                    }
                    else if (Canvas.GetLeft(joueur) + joueur.Width < Canvas.GetLeft(plateformesEnJeu[i].Texture) + toleranceColision)
                    {
                        return 1;
                    }
                    else if (Canvas.GetLeft(joueur) > Canvas.GetLeft(plateformesEnJeu[i].Texture) + plateformesEnJeu[i].Texture.Width - toleranceColision)
                    {
                        return 2;
                    }
                    
                    
                    
                    
                    else
                    {
                        vitesseSaut = -1;
                        return 4;
                    }
                }
  
            }
            return -1;
            

        }
        private void SautEnCours()
        {
            if (vitesseSaut < 0)
            {
                AnimationSaut();
                Canvas.SetTop(joueur, Canvas.GetTop(joueur) + vitesseSaut);
                vitesseSaut = vitesseSaut + gravite / 6;
            }
            else
            {
                AnimationChute();
            }
        }

        private void Tir(MouseButtonEventArgs e)
        {
            if (nbStockBille > 0)
            {
                Billes nouvelleBille = new Billes(new Image(), 0, 0, 0, 0);
                nouvelleBille.Texture.Source = imgBille;
                nouvelleBille.Texture.Width = 15;
                nouvelleBille.Texture.Height = 15;
                nouvelleBille.TypeBille = choixBille;
                nouvelleBille.DegatBille = 25;
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
        private bool BilleLance(Billes bille)
        {

            if (Canvas.GetLeft(bille.Texture) < 0 || Canvas.GetLeft(bille.Texture) > this.ActualWidth || Canvas.GetTop(bille.Texture) > this.ActualHeight)
            {
                return true;
            }
            else
            {
                Canvas.SetLeft(bille.Texture, Canvas.GetLeft(bille.Texture) + (bille.Vitesse[0] / 25));
                Canvas.SetTop(bille.Texture, Canvas.GetTop(bille.Texture) + bille.Vitesse[1] / 25);
                if (bille.TypeBille != 2)
                {
                    bille.Vitesse[1] = bille.Vitesse[1] + graviteBille;
                    bille.Vitesse[0] = bille.Vitesse[0] * 0.985;
                }
                hitBoxBille = new System.Drawing.Rectangle((int)Canvas.GetLeft(bille.Texture), (int)Canvas.GetTop(bille.Texture), (int)bille.Texture.Width, (int)bille.Texture.Height);
                ColisionEnnemi(bille);
            }

            hitBoxBille = new System.Drawing.Rectangle((int)Canvas.GetLeft(bille.Texture), (int)Canvas.GetTop(bille.Texture), (int)bille.Texture.Width - 1, (int)bille.Texture.Height - 1);
            for (int i = 0; i < plateformesEnJeu.Count; i++)
            {
                if (plateformesEnJeu[i].BoiteCollision.IntersectsWith(hitBoxBille))
                {
                    if (bille.TypeBille == 1) 
                    {
                        Canvas.SetLeft(joueur, Canvas.GetLeft(bille.Texture));
                        Canvas.SetTop(joueur, Canvas.GetTop(bille.Texture) - joueur.Height);
                    }
                    
                    return true;
                }
            }
            if (hitBoxBille.IntersectsWith(hitBoxSol))
            {
                if (bille.TypeBille == 1)
                {
                    Canvas.SetLeft(joueur, Canvas.GetLeft(bille.Texture));
                    Canvas.SetTop(joueur, Canvas.GetTop(bille.Texture) - joueur.Height);
                }
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

        private void DeplacementEnnemi()
        {
            for (int i = 0;i < ennemisEnJeu.Count; i++)
            {
                Canvas.SetLeft(ennemisEnJeu[0].Texture, Canvas.GetLeft(ennemisEnJeu[0].Texture) + Math.Sign(Canvas.GetLeft(joueur) - Canvas.GetLeft(ennemisEnJeu[0].Texture)) * 2);
                Canvas.SetTop(ennemisEnJeu[0].Texture, Canvas.GetTop(ennemisEnJeu[0].Texture) + Math.Sign(Canvas.GetTop(joueur) - Canvas.GetTop(ennemisEnJeu[0].Texture)));
                Canvas.SetLeft(ennemisEnJeu[0].BarreDeVie, Canvas.GetLeft(ennemisEnJeu[0].Texture) + Math.Sign(Canvas.GetLeft(joueur) - Canvas.GetLeft(ennemisEnJeu[0].Texture)) * 2);
                Canvas.SetTop(ennemisEnJeu[0].BarreDeVie, Canvas.GetTop(ennemisEnJeu[0].Texture) + Math.Sign(Canvas.GetTop(joueur) - Canvas.GetTop(ennemisEnJeu[0].Texture)));
                hitBoxEnnemi = new System.Drawing.Rectangle((int)Canvas.GetLeft(ennemisEnJeu[0].Texture), (int)Canvas.GetTop(ennemisEnJeu[0].Texture), (int)ennemisEnJeu[0].Texture.Width, (int)ennemisEnJeu[0].Texture.Height);

            }


            /* Canvas.SetLeft(EnnemiVie, Canvas.GetLeft(ennemi) + 15);
             Canvas.SetLeft(EnnemiVie2, Canvas.GetLeft(ennemi) + 30);
             Canvas.SetTop(EnnemiVie, Canvas.GetTop(ennemi) - 10);
             Canvas.SetTop(EnnemiVie2, Canvas.GetTop(ennemi) - 10);*/
        }

        private bool ColisionEnnemi(Billes bille)
        {
            for (int i = 0; i< ennemisEnJeu.Count; i++)
            {
                if (hitBoxBille.IntersectsWith(hitBoxEnnemi))
                {
                    ennemisEnJeu[0].PointDeVie -= bille.DegatBille;
                    ennemisEnJeu[0].BarreDeVie.Value -= bille.DegatBille;
                    canvasMainWindow.Children.Remove(bille.Texture);
                    billesEnJeu.Remove(bille);
                    if (ennemisEnJeu[0].PointDeVie <= 0)
                    {
                        ennemisEnJeu[0].Texture.Visibility = Visibility.Hidden;
                        ennemisEnJeu[0].BarreDeVie.Visibility = Visibility.Hidden;
                        canvasMainWindow.Children.Remove(ennemisEnJeu[0].Texture);
                        ennemisEnJeu.Remove(ennemisEnJeu[0]);
                        //EnnemiVie.Visibility = Visibility.Hidden;
                        //EnnemiVie2.Visibility = Visibility.Hidden;
                        ReinitialisationSaut();
                    }
                    else
                    {
                        //nbtouche = nbtouche + 1;
                        //EnnemiVie2.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    return true;
                }
            
            }
            return false;
        }
        private bool VerifTouche()
        {
            for (int i = 0; i < ennemisEnJeu.Count; i++)
            {
                if (ennemisEnJeu[0].Texture.Visibility == Visibility.Visible)
                {
                    hitBoxJoueur = new System.Drawing.Rectangle((int)Canvas.GetLeft(joueur), (int)Canvas.GetTop(joueur), (int)joueur.Width - 2, (int)joueur.Height - 2);
                    hitBoxEnnemi = new System.Drawing.Rectangle((int)Canvas.GetLeft(ennemisEnJeu[0].Texture), (int)Canvas.GetTop(ennemisEnJeu[0].Texture), (int)ennemisEnJeu[0].Texture.Width - 2, (int)ennemisEnJeu[0].Texture.Height - 2);
                    bool ennemiTouche = hitBoxEnnemi.IntersectsWith(hitBoxJoueur);
                    return ennemiTouche;
                }
            }
            
            return false;
        }

        private void ReinitialisationSaut()
        {
            enSaut = false;
            vitesseSaut = -35;

        }
        private void FinJeu()
        {
            minuterie.Stop();
        }

        public void AnimationDeplacementJoueur(int direction)
        {
            regard.ScaleX = direction;
            if (gravite == 0)
            {
                joueur.Source = new BitmapImage(new Uri($"pack://application:,,,/img/joueur/course/course{animationJoueur}.png"));
                timerAnimation += 1;
                if (timerAnimation == 3 )
                {
                    animationJoueur = animationJoueur + 1;
                    timerAnimation = 0;
                }
                    
                if (animationJoueur > 8)
                {
                    animationJoueur = 1;
                }
            }
            
        }
        
        public void AnimationSaut()
        {
            joueur.Source = new BitmapImage(new Uri($"pack://application:,,,/img/joueur/saut/saut{animationSaut}.png"));
            timerAnimationSaut += 1;
            if (timerAnimationSaut == 10 && animationSaut< 4)
            {
                animationSaut = animationSaut + 1;
                timerAnimationSaut = 0;
            }
        }
        private void AnimationChute()
        {
            animationSaut = 6;
            AnimationSaut();
            animationSaut = 1;
        }

        private void AnimationStatic()
        {
           
            joueur.Source = new BitmapImage(new Uri($"pack://application:,,,/img/joueur/inactif/inactif{animationStatic}.png"));
            timerAnimationStatic += 1;
            if (timerAnimationStatic == 6)
            {
                animationStatic = animationStatic + 1;
                timerAnimationStatic = 0;
            }

            if (animationStatic > 5)
            {
                animationStatic = 1;
            }
            
        }
    }

    
    public partial class Ennemis
    {
        private int coordonneeX, coordonneeY, typeDeplacement, pointDeVie;
        private Image texture;
        private double vitesse;
        private ProgressBar barreDeVie;

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
        public int PointDeVie
        {
            get { return pointDeVie; }
            set { pointDeVie = value; }
        }
        public double Vitesse
        {
            get { return vitesse; }
            set { vitesse = value; }
        }
        public ProgressBar BarreDeVie
        {
            get { return barreDeVie; }
            set { barreDeVie = value; }
        }
    }
    public class Billes
    {
        public Image Texture { get; set; }
        public double[] Vitesse { get; set; }
        public int TypeBille { get; set; }
        public int DegatBille { get; set; } 

        public Billes(Image texture, double vitesseX, double vitesseY, int choix, int degatBille)
        {
            this.Texture = texture;
            this.Vitesse = new double[] { vitesseX, vitesseY };
            this.TypeBille = choix;
            this.DegatBille = degatBille;
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