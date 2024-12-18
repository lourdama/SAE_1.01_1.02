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
        public DispatcherTimer minuterie, animationEntreeTimer;
        public int difficulte = 1, niveau = 0;
        public double volumeMusique = 1;
        public Key toucheGauche = Key.Q;
        public Key toucheDroite = Key.D;
        public Key toucheSaut = Key.Space;
        private static BitmapImage fond;
        private bool gauche, droite, saut, enSaut, billeBouge, pause,jouer, niveauGagne, animationEntreeBool, porteFerme = false;
        private int vitesseJoueur = 8, gravite = 8, toleranceColision = 5, nbtouche = 0, nbStockBille = 100, choixBille ;
        System.Drawing.Rectangle  hitBoxJoueur, hitBoxBille, hitBoxEnnemi, hitBoxSac;
        private int animationJoueur = 1, animationSaut = 1, animationStatic = 1, timerAnimation, timerAnimationSaut, timerAnimationStatic, animationEntree = 1, timerAnimationEntree = 0;
        private static Point clickPosition;
        private static double vitesseSaut, graviteBille = 4, coefReductionDeplacementSaut;
        private static Ennemis fantome = new Ennemis();
        private static List<ProgressBar> barreDeVie = new List<ProgressBar>();
        private static List<Ennemis> ennemisEnJeu = new List<Ennemis>();
        private static List<Billes> billesEnJeu = new List<Billes>();
        private static List<Plateformes> plateformesEnJeu = new List<Plateformes>();
        private static List<Sac> sacEnjeu = new List<Sac>();
        private static int[,] sautTailleAnimation = { { 61, 49 }, { 52, 64 }, { 50, 65 }, { 55, 57 }, { 60, 61 } };
        private static BitmapImage[] imageBilles;
        BitmapImage[] marche;
        private static MediaPlayer musique;
        int[,] niveauBille = new int[,]
        { {0,0,0}, {0,0,1}, {0,2,2}, {0,1,2} };
        int[] billeInventaire = new int[] { 100, 100, 0 };
        int[][,] niveauEnnemis = new int[][,]
        {
            new int[,] { { 1, 100, 100 }, { 1, 200, 200 }, { 1, 300, 300 }, { 1, 400, 400 } },
            new int[,] { { 1, 100, 100 }, { 1, 200, 200 }, { 1, 300, 300 }, { 1, 400, 400 } }
        };
        private double[,] tailleSaut = { { 61, 49 }, { 61, 49 }, { 61, 49 }, { 61, 49 }, };

        int[][,] proprietePlateformes = new int[][,]
        {
         new int[,] { { 425, 700 }, { 850, 700 }, { 1275, 700 },{ 600, 500 }, {300, 200 }, { 0, 700 }    },
         new int[,] { { 0, 700 }, { 850, 700 }, { 1275, 700 }, { 100, 500 } },
         new int[,] { { 425, 700 }, { 850, 700 }, { 1275, 700 }},
         new int[,] { { 425, 700 }, { 850, 700 }, { 1275, 700 } },
        };
        private static Random aleatoire = new Random();


        public MainWindow()
        {
            InitializeComponent();
            InitMusique();
            joueur.Visibility = Visibility.Hidden;
            ChoixBilleImg.Visibility = Visibility.Hidden;
            ChoixBille.Visibility = Visibility.Hidden;
        }


        private void butJouer_Click(object sender, RoutedEventArgs e)
        {
            Suivant();
        }
        public void Suivant()
        {
            InitImage();
            InitJeu();
            InitEnnemis();
            InitPlateformes();
            StopMusique();
            InitMusique();
            InitFond();
            animationEntreeBool = true;
            if (niveau == 1)
            {
                InitTimer();
            }
            else
            {
                minuterie.Start();
            }
            


        }
        private void InitFond()
        {
            BitmapImage fondniveau = new BitmapImage
               (new Uri($"pack://application:,,,/img/fond_niveau/level{niveau}.jpeg"));
            canvasMainWindow.Background = new ImageBrush(fondniveau);
        }



        private void InitMusique()
        {
            musique = new MediaPlayer();
            musique.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + $"/sons/musique{niveau}.mp3"));
            musique.MediaEnded += RelanceMusique;
            musique.Volume = volumeMusique;
            musique.Play();
        }
        private void StopMusique()
        {
            musique.Stop();
        }
        private void RelanceMusique(object? sender, EventArgs e)
        {
            musique.Position = TimeSpan.Zero;
            musique.Play();
        }

        private void InitJeu()
        {
            canvasMainWindow.Focus();
            joueur.Visibility = Visibility.Visible;
            ChoixBilleImg.Visibility = Visibility.Visible;
            ChoixBille.Visibility = Visibility.Visible;
            Canvas.SetLeft(joueur, -joueur.Width);
            Canvas.SetTop(joueur, proprietePlateformes[niveau][0, 1] - joueur.Height);
            
        }

        private void InitImage()
        {
            imageBilles = new BitmapImage[3];
            for(int i = 0; i < imageBilles.Length; i++)
            {
                imageBilles[i] = new BitmapImage(new Uri($"pack://application:,,,/img/billes/bille{i+1}.png"));
            }
            marche = new BitmapImage[6];
            for(int i = 0;i < marche.Length; i++)
            {
                marche[i] = new BitmapImage(new Uri($"pack://application:,,,/img/joueur/marche/marche{i + 1}.png"));
            }

            
        }
        private void InitEnnemis()
        {
            for (int i = 0; i < niveauEnnemis[niveau-1].GetLength(0); i++)
            {
                // Initialisation ennemi classique
                Ennemis fantome = new Ennemis();
                fantome.Texture = new Image();
                fantome.Texture.Source = new BitmapImage(new Uri("pack://application:,,,/img/fantome.png"));
                fantome.Texture.Width = 50;
                fantome.Texture.Height = 100;
                fantome.CoordonneeX = niveauEnnemis[niveau-1][i,1];
                fantome.CoordonneeY = niveauEnnemis[niveau-1][i,2]; ;
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
            for (int i = 0; i < proprietePlateformes[niveau-1].GetLength(0); i++)
            {
                Plateformes nouvellePlateforme = new Plateformes(new Image(), new int(), new int(), new System.Drawing.Rectangle()); 
                nouvellePlateforme.Texture.Source = new BitmapImage(new Uri("pack://application:,,,/img/plateforme.png"));
                nouvellePlateforme.Texture.Width = 425;
                nouvellePlateforme.Texture.Height = 38;
                plateformesEnJeu.Insert(0, nouvellePlateforme);
                canvasMainWindow.Children.Add(plateformesEnJeu[0].Texture);

                Canvas.SetLeft(plateformesEnJeu[0].Texture, proprietePlateformes[niveau-1][i, 0]);
                Canvas.SetTop(plateformesEnJeu[0].Texture, proprietePlateformes[niveau-1][i, 1]);
                plateformesEnJeu[0].BoiteCollision = new System.Drawing.Rectangle((int)Canvas.GetLeft(plateformesEnJeu[0].Texture) - toleranceColision , (int)Canvas.GetTop(plateformesEnJeu[0].Texture) - toleranceColision, (int)plateformesEnJeu[0].Texture.Width + 2* toleranceColision, (int)plateformesEnJeu[0].Texture.Height + 2*toleranceColision);
            }
        }
        private void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (jouer)
            {
                if (e.Delta > 0 && choixBille < 2)
                {
                    choixBille = choixBille + 1;
                }


                else if (e.Delta < 0 && choixBille > 0)
                {
                    choixBille = choixBille - 1;
                }
                ChoixBille.Content = billeInventaire[choixBille];
            }

            ChoixBilleImg.Source = imageBilles[choixBille];

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (jouer)
            {
                if (e.Key == toucheGauche)
                {
                    gauche = true;
                }

                else if (e.Key == toucheDroite)
                {
                    droite = true;
                }

                else if (e.Key == toucheSaut)
                {
                    saut = true;
                }
                else if (e.Key == Key.P)
                {
                    if (!pause)
                    {
                        pause = true;
                        this.ControlContent.Content = new Pause();
                        minuterie.Start();
                    }

                }
            }
            
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if(jouer)
            {
                if (e.Key == toucheGauche)
                {
                    gauche = false;
                }
                else if (e.Key == toucheDroite)
                {
                    droite = false;
                }

                else if (e.Key == toucheSaut)
                {
                    saut = false;
                }
            }

            

        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!pause&&jouer)
            {
                Tir(e);
            }
        }

        private void butQuitter_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void Reprendre()
        {
            pause = false;
            this.ControlContent.Content = null;
            minuterie.Start();
        }
        private void Jeu(object? sender, EventArgs e)
        {
            if (jouer)
            {
                Deplacement();
                DeplacementEnnemi();
                if (VerifTouche())
                {
                    niveauGagne = false;
                    DestructionNiveau();
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
                if (ennemisEnJeu.Count == 0)
                {
                    niveauGagne = true;
                    FinNiveau();
                }
                if (aleatoire.Next(0, 900) == 1)
                {
                    // spawn sac de bille
                }
            }

            else if (!jouer)
            {
                AnimationEntree();
            }
            


        }
        private void Deplacement()
        {

            if (CollisionPlat() == 0)
            {
                gravite = 0;
                coefReductionDeplacementSaut = 1;
                animationSaut = 1;
                ReinitialisationSaut();
            }

            else
            {

                gravite = 8;
                coefReductionDeplacementSaut = 0.6;
                if (Canvas.GetTop(joueur) > this.Height)
                {
                    Canvas.SetTop(joueur, -joueur.Height);
                }

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
                        timerAnimationSaut = 0;
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
                        vitesseSaut = 1;
                        return 4;
                    }
                }
  
            }
            return -1;
            

        }
        private int CollisionPlat(Sac sacDeBille)
        {
            hitBoxSac = new System.Drawing.Rectangle((int)Canvas.GetLeft(sacDeBille.Texture), (int)Canvas.GetTop(sacDeBille.Texture), (int)joueur.Width, (int)joueur.Height);
            for (int i = 0; i < plateformesEnJeu.Count; i++)
            {
                if (hitBoxSac.IntersectsWith(plateformesEnJeu[i].BoiteCollision))
                {
                    return 0;
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
            if (billeInventaire[choixBille] > 0)
            {
                Billes nouvelleBille = new Billes(new Image(), 0, 0, 0, 0);
                nouvelleBille.Texture.Source = imageBilles[choixBille];
                nouvelleBille.Texture.Width = 16;
                nouvelleBille.Texture.Height = 16;
                nouvelleBille.TypeBille = choixBille;
                nouvelleBille.DegatBille = 25/difficulte;
                clickPosition = e.GetPosition(this);
                double vitesseX = clickPosition.X - Canvas.GetLeft(joueur);
                double vitesseY = clickPosition.Y - Canvas.GetTop(joueur);
                nouvelleBille.Vitesse[0] = vitesseX;
                nouvelleBille.Vitesse[1] = vitesseY;
                billesEnJeu.Insert(0, nouvelleBille);
                canvasMainWindow.Children.Add(nouvelleBille.Texture);
                Canvas.SetTop(nouvelleBille.Texture, Canvas.GetTop(joueur));
                Canvas.SetLeft(nouvelleBille.Texture, Canvas.GetLeft(joueur));
                billeInventaire[choixBille] --;
                StockBille.Content = "Stock De Billes : " + billeInventaire[choixBille];


            }
        }
        private void ApparitionSac()
        {
            Sac nouveauSac = new Sac(new Image());
            nouveauSac.Texture = new Image();
            nouveauSac.Texture.Source = new BitmapImage(new Uri("pack://application:,,,/img/billes/bille1.png"));
            nouveauSac.Texture.Width = 64;
            nouveauSac.Texture.Height = 64;
            for (int i = 0; i < niveauBille.GetLength(1); i++)
            {
                int typeBilleContenu = niveauBille[niveau,i];
                nouveauSac.Contenu[typeBilleContenu] = aleatoire.Next(3,6);
            }
            sacEnjeu.Insert(0, nouveauSac);
            canvasMainWindow.Children.Add(nouveauSac.Texture);
            Canvas.SetTop(nouveauSac.Texture, 0);
            Canvas.SetLeft(nouveauSac.Texture, aleatoire.Next(10, (int)canvasMainWindow.Width - 10));
        }
        private void DeplacementSac()
        {
            for (int i = 0; i < sacEnjeu.Count; i++)
            {
                if (CollisionPlat(sacEnjeu[i]) != 0)
                {
                    Canvas.SetTop(sacEnjeu[i].Texture, Canvas.GetTop(sacEnjeu[i].Texture) + gravite);
                }
                ColisionSac(sacEnjeu[i]);
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
                if (ColisionEnnemi(bille))
                {
                    return true;
                }
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
            return false;



        }

        private void DeplacementEnnemi()
        {
            for (int i = 0;i < ennemisEnJeu.Count; i++)
            {
                Canvas.SetLeft(ennemisEnJeu[i].Texture, Canvas.GetLeft(ennemisEnJeu[i].Texture) + Math.Sign(Canvas.GetLeft(joueur) - Canvas.GetLeft(ennemisEnJeu[i].Texture)) * 2 * Math.Sqrt((double)difficulte));
                Canvas.SetTop(ennemisEnJeu[i].Texture, Canvas.GetTop(ennemisEnJeu[i].Texture) + Math.Sign(Canvas.GetTop(joueur) - Canvas.GetTop(ennemisEnJeu[i].Texture)));
                Canvas.SetLeft(ennemisEnJeu[i].BarreDeVie, Canvas.GetLeft(ennemisEnJeu[i].Texture) + Math.Sign(Canvas.GetLeft(joueur) - Canvas.GetLeft(ennemisEnJeu[i].Texture)) * 2 * Math.Sqrt((double)difficulte));
                Canvas.SetTop(ennemisEnJeu[i].BarreDeVie, Canvas.GetTop(ennemisEnJeu[i].Texture) + Math.Sign(Canvas.GetTop(joueur) - Canvas.GetTop(ennemisEnJeu[i].Texture)));
                hitBoxEnnemi = new System.Drawing.Rectangle((int)Canvas.GetLeft(ennemisEnJeu[i].Texture), (int)Canvas.GetTop(ennemisEnJeu[i].Texture), (int)ennemisEnJeu[i].Texture.Width, (int)ennemisEnJeu[i].Texture.Height);

            }
        }
        private void ColisionSac(Sac sacDeBille)
        {
            hitBoxJoueur = new System.Drawing.Rectangle((int)Canvas.GetLeft(joueur), (int)Canvas.GetTop(joueur), (int)joueur.Width - 2, (int)joueur.Height - 2);
            hitBoxSac = new System.Drawing.Rectangle((int)Canvas.GetLeft(sacDeBille.Texture), (int)Canvas.GetTop(sacDeBille.Texture), (int)joueur.Width, (int)joueur.Height);
            if (hitBoxJoueur.IntersectsWith(hitBoxSac))
            {
                for (int i = 0; i < sacDeBille.Contenu.Length; i++)
                {
                    billeInventaire[i] += sacDeBille.Contenu[i];
                }
                canvasMainWindow.Children.Remove(sacDeBille.Texture);
                sacEnjeu.Remove(sacDeBille);
            }
        }
        private bool ColisionEnnemi(Billes bille)
        {
            if (bille.TypeBille != 1)
            {
                for (int i = 0; i < ennemisEnJeu.Count; i++)
                {
                    hitBoxEnnemi = new System.Drawing.Rectangle((int)Canvas.GetLeft(ennemisEnJeu[i].Texture), (int)Canvas.GetTop(ennemisEnJeu[i].Texture), (int)ennemisEnJeu[i].Texture.Width, (int)ennemisEnJeu[i].Texture.Height);
                    if (hitBoxBille.IntersectsWith(hitBoxEnnemi))
                    {
                        ennemisEnJeu[i].PointDeVie -= bille.DegatBille;
                        ennemisEnJeu[i].BarreDeVie.Value -= bille.DegatBille;
                        if (ennemisEnJeu[i].PointDeVie <= 0)
                        {
                            canvasMainWindow.Children.Remove(ennemisEnJeu[i].Texture);
                            canvasMainWindow.Children.Remove(ennemisEnJeu[i].BarreDeVie);
                            ennemisEnJeu.Remove(ennemisEnJeu[i]);
                            ReinitialisationSaut();
                        }
                        return true;
                    }

                }
            }
            return false;
        }
        private bool VerifTouche()
        {
            for (int i = 0; i < ennemisEnJeu.Count; i++)
            {
                    hitBoxJoueur = new System.Drawing.Rectangle((int)Canvas.GetLeft(joueur), (int)Canvas.GetTop(joueur), (int)joueur.Width - 2, (int)joueur.Height - 2);
                    hitBoxEnnemi = new System.Drawing.Rectangle((int)Canvas.GetLeft(ennemisEnJeu[i].Texture), (int)Canvas.GetTop(ennemisEnJeu[i].Texture), (int)ennemisEnJeu[i].Texture.Width - 2, (int)ennemisEnJeu[i].Texture.Height - 2);
                    bool ennemiTouche = hitBoxEnnemi.IntersectsWith(hitBoxJoueur);
                    if(ennemiTouche == true)    
                        return ennemiTouche;
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

        private void FinNiveau()
        {

            if (Canvas.GetLeft(joueur) + joueur.Width > this.Width -25 && Canvas.GetTop(joueur) > 500 && Canvas.GetTop(joueur) < 900)
            {
                DestructionNiveau();
            }
            

        }

        private void DestructionNiveau()
        {
            int plat = plateformesEnJeu.Count;
            int bille = billesEnJeu.Count;
            for(int i = 0; i < plat; i++)
            {
                canvasMainWindow.Children.Remove(plateformesEnJeu[0].Texture);
                plateformesEnJeu.Remove(plateformesEnJeu[0]);
            }
            for(int i = 0;i < bille; i++)
            {
                canvasMainWindow.Children.Remove(billesEnJeu[0].Texture);
                billesEnJeu.Remove(billesEnJeu[0]);
            }

            
            jouer = false;
            droite = false;
            gauche = false;
            saut = false;
            enSaut = true;
            vitesseSaut = 0;
            joueur.Visibility = Visibility.Hidden;
            ChoixBilleImg.Visibility = Visibility.Hidden;
            ChoixBille.Visibility = Visibility.Hidden;
            ChoixNiveau choixDuNiveau = new ChoixNiveau();
            if (niveauGagne == false)   
            {
                niveau--;
                int ennemi = ennemisEnJeu.Count;
                for (int i = 0; i < ennemi; i++)
                {
                    canvasMainWindow.Children.Remove(ennemisEnJeu[0].Texture);
                    canvasMainWindow.Children.Remove(ennemisEnJeu[0].BarreDeVie);
                    ennemisEnJeu.Remove(ennemisEnJeu[0]);
                }
            }
            choixDuNiveau.ChangerCouleurEllipseNiveau(niveau);
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/choixduniveau.jpg", UriKind.RelativeOrAbsolute));
            this.canvasMainWindow.Background = imageBrush;
            this.ControlContent.Content = choixDuNiveau;
            minuterie.Stop();



        }

        private void AnimationEntree()
        {
            Canvas.SetLeft(joueur, Canvas.GetLeft(joueur) + 5);
            joueur.Source = new BitmapImage(new Uri($"pack://application:,,,/img/joueur/marche/marche{animationEntree}.png"));
            timerAnimationEntree += 1;
            if (timerAnimationEntree == 5)
            {
                animationEntree = animationEntree + 1;
                timerAnimationEntree = 0;
            }

            if (animationEntree > 6)
            {
                jouer = true;
                animationEntree = 1;
            }
            
        }

        public void AnimationDeplacementJoueur(int direction)
        {
            joueur.Width = 41;
            joueur.Height = 55;
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
            joueur.Width = sautTailleAnimation[animationSaut - 1,0];
            joueur.Height = sautTailleAnimation[animationSaut - 1,1]; ;
            joueur.Source = new BitmapImage(new Uri($"pack://application:,,,/img/joueur/saut/saut{animationSaut}.png"));
            timerAnimationSaut += 1;
            if (timerAnimationSaut == 8 && animationSaut< 4)
            {
                animationSaut = animationSaut + 1;
                timerAnimationSaut = 0;
            }
        }
        private void AnimationChute()
        {
            animationSaut = 5;
            AnimationSaut();
            
        }

        private void AnimationStatic()
        {

            joueur.Width = 57;
            joueur.Height = 55;
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
    public class Sac
    {
        public Image Texture { get; set; }
        public int[] Contenu { get; set; }

        public Sac(Image texture)
        {
            this.Texture = texture;
            this.Contenu = new int[] {0,0,0};

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