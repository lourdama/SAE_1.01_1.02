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
    /// 
    
    public partial class MainWindow : Window
    {
        private static readonly int TOLERANCE_COLISION = 5, GRAVITE = 8, TAUX_APPARITION_SAC = 300, NB_BILLES_DEPART = 3, TIMER_ANIMATION = 5, DEPLACEMENT_SOL = 1, GRAVITE_BILLE = 4;
        private static readonly double DEPLACEMENT_AIR = 0.6;
        private static readonly int[,] NIVEAU_BILLE = new int[,]
            { {0,0,0}, {0,0,1}, {0,2,2}, {0,1,2} };
        private static readonly int[][,] NIVEAU_ENNEMIS = new int[][,]
        {
            new int[,] { {2, 800, 440}, {2, 500, 140}, {2, 900, 640} },
            new int[,] { { 1, 1000, 100 }, { 1, 1000, 700 }, { 1, 1400, 400 }, {2, 500, 340}, {2, 1400, 640}, {2, 1200, 140} },
            new int[,] { { 1, 100, 100 }, { 1, 1000, 100 }, { 1, 1000, 700 }, { 1, 1400, 400 } },
            new int[,] {{ 3, 450, 900} }

        };
        private static readonly double[,] TAILLE_SAUT = { { 61, 49 }, { 61, 49 }, { 61, 49 }, { 61, 49 }, };

        private static readonly int[][,] PROPRIETES_PLATEFORMES = new int[][,]
        {
         new int[,] { { 425, 700 }, { 850, 700 }, { 1275, 700 },{ 600, 500 }, {300, 200 }, { 0, 700 }},
         new int[,] { { 0, 700 },{425 ,400 }, { 850, 400 }, { 1275, 700 }, { 850, 200 } },
         new int[,] { { 425, 700 }, { 850, 700 }, { 1275, 700 }},
         new int[,] { { 425, 700 }, { 850, 700 }, { 1275, 700 } }
        };
        private static int[,] SAUT_TAILLE_ANIMATION = { { 61, 49 }, { 52, 64 }, { 50, 65 }, { 55, 57 }, { 60, 61 } };
        // Fin constantes, et début variables
        public DispatcherTimer minuterie, animationEntreeTimer;
        public int difficulte = 1, niveau = 0;
        public double volumeMusique = 1;
        public Key toucheGauche = Key.Q;
        public Key toucheDroite = Key.D;
        public Key toucheSaut = Key.Space;
        private static BitmapImage fond;
        private bool gauche, droite, saut, enSaut, billeBouge, pause,jouer, niveauGagne, animationEntreeBool, porteFerme, mort = false, toucheG, toucheCtrl ;
        private int vitesseJoueur = 8, gravite = 8, nbtouche = 0, choixBille;
        System.Drawing.Rectangle  hitBoxJoueur, hitBoxBille, hitBoxEnnemi, hitBoxSac;
        private int animationJoueur = 1, animationSaut = 1, animationStatic = 1, timerAnimation, timerAnimationSaut, timerAnimationStatic, animationEntree = 1, timerAnimationEntree = 0, timerAnimationMort, animationMort = 1;
        private static Point clickPosition;
        private static double vitesseSaut, coefReductionDeplacementSaut;
        private static List<ProgressBar> barreDeVie = new List<ProgressBar>();
        private static List<Ennemis> ennemisEnJeu = new List<Ennemis>();
        private static List<Billes> billesEnJeu = new List<Billes>();
        private static List<Plateformes> plateformesEnJeu = new List<Plateformes>();
        private static List<Sac> sacEnjeu = new List<Sac>();
        private static  int[] billeInventaire = new int[] { 3, 3, 3 };
        private static BitmapImage[] imageBilles;
        private static BitmapImage[] courseAnimationTab;
        private static BitmapImage[] sautAnimationTab;
        private static BitmapImage[] mortAnimationTab;
        private static BitmapImage[] marcheAnimationTab;
        private static BitmapImage[] inactifAnimationTab;
        private static BitmapImage[] ennemi;
        private static MediaPlayer musique = new MediaPlayer();

        private static Random aleatoire = new Random();


        public MainWindow()
        {
            InitializeComponent();
            InitMusique(niveau);
            InitImage();
        }


        private void butJouer_Click(object sender, RoutedEventArgs e)
        {
            Suivant();
        }
        public void Suivant()
        {
            InitJeu();
            InitEnnemis();
            InitPlateformes();
            StopMusique();
            InitMusique(niveau);
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



        public void InitMusique(int indice)
        {
            musique = new MediaPlayer();
            musique.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + $"/sons/musique{indice}.mp3"));
            musique.MediaEnded += RelanceMusique;
            musique.Volume = volumeMusique;
            musique.Play();
        }
        public void StopMusique()
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
            Canvas.SetTop(joueur, PROPRIETES_PLATEFORMES[niveau-1][0, 1] - joueur.Height);
            ChoixBille.Content = billeInventaire[choixBille];
            ChoixBilleImg.Source = imageBilles[choixBille];
        }

        private void InitImage()
        {
            imageBilles = new BitmapImage[3];
            for(int i = 0; i < imageBilles.Length; i++)
            {
                imageBilles[i] = new BitmapImage(new Uri($"pack://application:,,,/img/billes/bille{i+1}.png"));
            }
            
            marcheAnimationTab = new BitmapImage[6];
            for(int i = 0;i < marcheAnimationTab.Length; i++)
            {
                marcheAnimationTab[i] = new BitmapImage(new Uri($"pack://application:,,,/img/joueur/marche/marche{i + 1}.png"));
            }
            
            courseAnimationTab = new BitmapImage[8];
            for (int i = 0; i < courseAnimationTab.Length; i++)
            {
                courseAnimationTab[i] = new BitmapImage(new Uri($"pack://application:,,,/img/joueur/course/course{i + 1}.png"));
            }
            
            sautAnimationTab = new BitmapImage[5];
            for (int i = 0; i < sautAnimationTab.Length; i++)
            {
                sautAnimationTab[i] = new BitmapImage(new Uri($"pack://application:,,,/img/joueur/saut/saut{i + 1}.png"));
            }

            mortAnimationTab = new BitmapImage[6];
            for (int i = 0; i < mortAnimationTab.Length; i++)
            {
                mortAnimationTab[i] = new BitmapImage(new Uri($"pack://application:,,,/img/joueur/mort/mort{i + 1}.png"));
            }

            inactifAnimationTab = new BitmapImage[5];
            for (int i = 0; i < inactifAnimationTab.Length; i++)
            {
                inactifAnimationTab[i] = new BitmapImage(new Uri($"pack://application:,,,/img/joueur/inactif/inactif{i + 1}.png"));
            }

            ennemi = new BitmapImage[2];
            for (int i = 0;i < ennemi.Length; i++)
            {
                ennemi[i] = new BitmapImage(new Uri($"pack://application:,,,/img/ennemi/ennemi{i + 1}.png"));
            }



        }
        private void InitEnnemis()
        {
            for (int i = 0; i < NIVEAU_ENNEMIS[niveau-1].GetLength(0); i++)
            {
                // Initialisation ennemi 
                int multiplicateur
                Ennemis ennemie = new Ennemis();
                ennemie.Texture = new Image();
                ennemie.Texture.Source = ennemi[NIVEAU_ENNEMIS[niveau - 1][i, 0]-1];
                ennemie.Texture.Width = 50;
                ennemie.Texture.Height = 100;
                ennemie.CoordonneeX = NIVEAU_ENNEMIS[niveau-1][i,1];
                ennemie.CoordonneeY = NIVEAU_ENNEMIS[niveau-1][i,2]; ;
                ennemie.Vitesse = 2;
                ennemie.TypeDeplacement = NIVEAU_ENNEMIS[niveau-1][i,0];
                ennemie.PointDeVie = 100;
                ennemie.BarreDeVie = new ProgressBar();
                ennemie.BarreDeVie.Height = 10;
                ennemie.BarreDeVie.Width = 75;
                ennemie.BarreDeVie.Value = 100;
                ennemisEnJeu.Insert(0, ennemie);
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
            for (int i = 0; i < PROPRIETES_PLATEFORMES[niveau-1].GetLength(0); i++)
            {
                Plateformes nouvellePlateforme = new Plateformes(new Image(), new int(), new int(), new System.Drawing.Rectangle()); 
                nouvellePlateforme.Texture.Source = new BitmapImage(new Uri("pack://application:,,,/img/plateforme.png"));
                nouvellePlateforme.Texture.Width = 425;
                nouvellePlateforme.Texture.Height = 38;
                plateformesEnJeu.Insert(0, nouvellePlateforme);
                canvasMainWindow.Children.Add(plateformesEnJeu[0].Texture);

                Canvas.SetLeft(plateformesEnJeu[0].Texture, PROPRIETES_PLATEFORMES[niveau-1][i, 0]);
                Canvas.SetTop(plateformesEnJeu[0].Texture, PROPRIETES_PLATEFORMES[niveau-1][i, 1]);
                plateformesEnJeu[0].BoiteCollision = new System.Drawing.Rectangle((int)Canvas.GetLeft(plateformesEnJeu[0].Texture) - TOLERANCE_COLISION , (int)Canvas.GetTop(plateformesEnJeu[0].Texture) - TOLERANCE_COLISION, (int)plateformesEnJeu[0].Texture.Width + 2* TOLERANCE_COLISION, (int)plateformesEnJeu[0].Texture.Height + 2*TOLERANCE_COLISION);
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
                    if (jouer)
                    {
                        jouer = false;
                        this.ControlContent.Content = new Pause();
                    }

                }
                else if (e.Key == Key.G)
                {
                    toucheG = true;
                }
                else if (e.Key == Key.LeftCtrl)
                {
                    toucheCtrl = true;
                }
                else if (e.Key == Key.N)
                {
                    ApparitionSac();
                }
            }
            
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
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
            else if (e.Key == Key.G)
            {
                toucheG = false;
            }
            else if (e.Key == Key.LeftCtrl)
            {
                toucheCtrl = false;
            }
        }
        
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (jouer)
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
            jouer = true;
            this.ControlContent.Content = null;
            canvasMainWindow.Focus();

        }
        private void Jeu(object? sender, EventArgs e)
        {
            if (jouer)
            {
                Deplacement();
                DeplacementEnnemi();
                DeplacementSac();
                Richesse();
                if (VerifTouche())
                {
                    jouer = false;
                    niveauGagne = false;
                    mort = true;
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
                if (aleatoire.Next(0, TAUX_APPARITION_SAC) == 1)
                {
                    ApparitionSac();
                }
            }

            else if (animationEntreeBool)
            {
                AnimationEntree();
            }

            else if (mort)
            {
                AnimationMort();
            }
            


        }
        private void Deplacement()
        {

            if (CollisionPlat() == 0)
            {
                gravite = 0;
                coefReductionDeplacementSaut = DEPLACEMENT_SOL;
                animationSaut = 1;
                ReinitialisationSaut();
            }

            else
            {

                gravite = 8;
                coefReductionDeplacementSaut = DEPLACEMENT_AIR;
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

                    if (Canvas.GetTop(joueur) + joueur.Height < Canvas.GetTop(plateformesEnJeu[i].Texture) + TOLERANCE_COLISION)
                    {
                        timerAnimationSaut = 0;
                        if (!droite && !gauche)
                        {
                            AnimationStatic();
                        }
                        return 0;
                    }
                    else if (Canvas.GetLeft(joueur) + joueur.Width < Canvas.GetLeft(plateformesEnJeu[i].Texture) + TOLERANCE_COLISION)
                    {
                        return 1;
                    }
                    else if (Canvas.GetLeft(joueur) > Canvas.GetLeft(plateformesEnJeu[i].Texture) + plateformesEnJeu[i].Texture.Width - TOLERANCE_COLISION)
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
            hitBoxSac = new System.Drawing.Rectangle((int)Canvas.GetLeft(sacDeBille.Texture), (int)Canvas.GetTop(sacDeBille.Texture), (int)sacDeBille.Texture.Width, (int)sacDeBille.Texture.Height);
            for (int i = 0; i < plateformesEnJeu.Count; i++)
            {
                Console.WriteLine(Canvas.GetTop(sacDeBille.Texture));
                if (hitBoxSac.IntersectsWith(plateformesEnJeu[i].BoiteCollision))
                {
                    Console.WriteLine("Au sol");
                    return 0;
                }
            }
            return -1;
        }
        private int CollisionPlat(Ennemis ennemi, double direction)
        {
            hitBoxSac = new System.Drawing.Rectangle((int)(Canvas.GetLeft(ennemi.Texture) + (ennemi.Texture.Width * direction ) ), (int)Canvas.GetTop(ennemi.Texture), (int)(ennemi.Texture.Width + ennemi.Texture.Width), (int)ennemi.Texture.Height);
            for (int i = 0; i < plateformesEnJeu.Count; i++)
            {
                Console.WriteLine(Canvas.GetTop(ennemi.Texture));
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
                ChoixBille.Content = billeInventaire[choixBille];

            }
        }
        private void ApparitionSac()
        {
            Console.WriteLine("Apparition Sac");
            Sac nouveauSac = new Sac(new Image());
            nouveauSac.Texture = new Image();
            nouveauSac.Texture.Source = new BitmapImage(new Uri("pack://application:,,,/img/sacBille.png"));
            nouveauSac.Texture.Width = 32;
            nouveauSac.Texture.Height = 32;
            for (int i = 0; i < NIVEAU_BILLE.GetLength(1); i++)
            {
                int typeBilleContenu = NIVEAU_BILLE[niveau-1,i];
                nouveauSac.Contenu[typeBilleContenu] = aleatoire.Next(3,6);
            }
            sacEnjeu.Insert(0, nouveauSac);
            canvasMainWindow.Children.Add(sacEnjeu[0].Texture);
            Canvas.SetTop(nouveauSac.Texture, -nouveauSac.Texture.Height);
            Canvas.SetLeft(nouveauSac.Texture, aleatoire.Next(10, (int)this.Width - 10));
            Console.WriteLine("Sac ajouté");
        }

        private void DeplacementSac()
        {
            for (int i = 0; i < sacEnjeu.Count; i++)
            {
                
                if (CollisionPlat(sacEnjeu[i]) != 0)
                {
                    Canvas.SetTop(sacEnjeu[i].Texture, Canvas.GetTop(sacEnjeu[i].Texture) + 8);
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
                    bille.Vitesse[1] = bille.Vitesse[1] + GRAVITE_BILLE;
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
                if (ennemisEnJeu[i].TypeDeplacement == 1)
                {
                    Canvas.SetLeft(ennemisEnJeu[i].Texture, Canvas.GetLeft(ennemisEnJeu[i].Texture) + Math.Sign(Canvas.GetLeft(joueur) - Canvas.GetLeft(ennemisEnJeu[i].Texture)) * 2 * Math.Sqrt((double)difficulte));
                    Canvas.SetTop(ennemisEnJeu[i].Texture, Canvas.GetTop(ennemisEnJeu[i].Texture) + Math.Sign(Canvas.GetTop(joueur) - Canvas.GetTop(ennemisEnJeu[i].Texture)));
                    Canvas.SetLeft(ennemisEnJeu[i].BarreDeVie, Canvas.GetLeft(ennemisEnJeu[i].Texture) + Math.Sign(Canvas.GetLeft(joueur) - Canvas.GetLeft(ennemisEnJeu[i].Texture)) * 2 * Math.Sqrt((double)difficulte));
                    Canvas.SetTop(ennemisEnJeu[i].BarreDeVie, Canvas.GetTop(ennemisEnJeu[i].Texture) + Math.Sign(Canvas.GetTop(joueur) - Canvas.GetTop(ennemisEnJeu[i].Texture)));
                }
                else if (ennemisEnJeu[i].TypeDeplacement == 2)
                {
                    if ((Canvas.GetLeft(joueur) > Canvas.GetLeft(ennemisEnJeu[i].Texture) && CollisionPlat(ennemisEnJeu[i], 1) == 0 )
                        || Canvas.GetLeft(joueur) < Canvas.GetLeft(ennemisEnJeu[i].Texture) && CollisionPlat(ennemisEnJeu[i], -2) == 0 )
                    {
                        Canvas.SetLeft(ennemisEnJeu[i].Texture, Canvas.GetLeft(ennemisEnJeu[i].Texture) + Math.Sign(Canvas.GetLeft(joueur) - Canvas.GetLeft(ennemisEnJeu[i].Texture)) * 2 * Math.Sqrt((double)difficulte));
                        Canvas.SetLeft(ennemisEnJeu[i].BarreDeVie, Canvas.GetLeft(ennemisEnJeu[i].Texture) + Math.Sign(Canvas.GetLeft(joueur) - Canvas.GetLeft(ennemisEnJeu[i].Texture)) * 2 * Math.Sqrt((double)difficulte));
                    }
                    
                }
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
                ChoixBille.Content = billeInventaire[choixBille];
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
        private void Richesse()
        {
            if (toucheG && toucheCtrl)
            {
                for (int i = 0; i < billeInventaire.Length; i++)
                {
                    billeInventaire[i] = 4242;
                }
                ChoixBille.Content = billeInventaire[choixBille];
            }
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
            int sac = sacEnjeu.Count;
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
            for(int i = 0; i < sac; i++)
            {
                canvasMainWindow.Children.Remove(sacEnjeu[0].Texture);
                sacEnjeu.Remove(sacEnjeu[0]);
            }
            
            
            ChoixNiveau choixDuNiveau = new ChoixNiveau();
            Victoire victoire = new Victoire();
            EcranMort ecranMort = new EcranMort();

            for (int i = 0; i < billeInventaire.Length; i++)
            {
                billeInventaire[i] = NB_BILLES_DEPART;
            }
            jouer = false;
            joueur.Visibility = Visibility.Hidden;
            ChoixBilleImg.Visibility = Visibility.Hidden;
            ChoixBilleImg.Visibility = Visibility.Hidden;
            ChoixBille.Visibility = Visibility.Hidden;
            ChoixBilleImg.Visibility = Visibility.Hidden;
            choixDuNiveau.ChangerCouleurEllipseNiveau(niveau);
            String fond = String.Empty;
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
                this.ControlContent.Content = ecranMort;
                fond = $"fond_niveau/level{niveau+1}.jpeg";
            }
            else if (niveau == 4)
            {
                niveau = 0;
                this.ControlContent.Content = victoire;
                fond = "victoirefond.jpg";
            }
            else
            {
                this.ControlContent.Content = choixDuNiveau;
                fond = "choixduniveau.jpg";
            }
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/img/{fond}", UriKind.RelativeOrAbsolute));
            this.canvasMainWindow.Background = imageBrush;
            minuterie.Stop();
            StopMusique();
            InitMusique(0);



        }

        private void AnimationEntree()
        {
            Canvas.SetLeft(joueur, Canvas.GetLeft(joueur) + 5);
            joueur.Source = new BitmapImage(new Uri($"pack://application:,,,/img/joueur/marche/marche{animationEntree}.png"));
            timerAnimationEntree += 1;
            if (timerAnimationEntree == TIMER_ANIMATION)
            {
                animationEntree = animationEntree + 1;
                timerAnimationEntree = 0;
            }

            if (animationEntree > 6)
            {
                jouer = true;
                animationEntreeBool = false;
                animationEntree = 1;
            }
            
        }

        private void AnimationMort()
        {
            if (CollisionPlat() != 4)
            {
                Canvas.SetTop(joueur, Canvas.GetTop(joueur) + 5);
            }
            joueur.Source = mortAnimationTab[animationMort - 1];
            timerAnimationMort += 1;
            if (timerAnimationMort == TIMER_ANIMATION)
            {
                animationMort = animationMort + 1;
                timerAnimationMort = 0;
            }

            if (animationMort > mortAnimationTab.Length)
            {
                mort = false;
                animationMort = 1;
                timerAnimationMort = 0;
                DestructionNiveau();
            }

        }

        public void AnimationDeplacementJoueur(int direction)
        {
            joueur.Width = 41;
            joueur.Height = 55;
            regard.ScaleX = direction;
            if (gravite == 0)
            {
                joueur.Source = courseAnimationTab[animationJoueur - 1];
                timerAnimation += 1;
                if (timerAnimation == TIMER_ANIMATION )
                {
                    animationJoueur = animationJoueur + 1;
                    timerAnimation = 0;
                }
                    
                if (animationJoueur > courseAnimationTab.Length)
                {
                    animationJoueur = 1;
                }
            }
            
        }
        
        public void AnimationSaut()
        {
            joueur.Width = SAUT_TAILLE_ANIMATION[animationSaut - 1,0];
            joueur.Height = SAUT_TAILLE_ANIMATION[animationSaut - 1,1]; 
            joueur.Source = sautAnimationTab[animationSaut - 1];
            timerAnimationSaut += 1;
            if (timerAnimationSaut == TIMER_ANIMATION && animationSaut< 4)
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
            joueur.Source = inactifAnimationTab[animationStatic-1];
            timerAnimationStatic += 1;
            if (timerAnimationStatic == TIMER_ANIMATION)
            {
                animationStatic = animationStatic + 1;
                timerAnimationStatic = 0;
            }

            if (animationStatic > inactifAnimationTab.Length)
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