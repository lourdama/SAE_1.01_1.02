using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.Security;
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
        private static readonly int TOLERANCE_COLISION = 5, GRAVITE = 8, TAUX_APPARITION_SAC = 300, NB_BILLES_DEPART = 3, TIMER_ANIMATION = 5, DEPLACEMENT_SOL = 1, GRAVITE_BILLE = 4, NB_ROCHE_BOSS = 5;
        private static readonly double DEPLACEMENT_AIR = 0.6;
        private static readonly int[,] NIVEAU_BILLE = new int[,]
            { {0,0,0}, {0,0,1}, {0,2,2}, {0,1,2} };
        private static readonly int[][,] NIVEAU_ENNEMIS = new int[][,]// définition des différents ennemis par type et coordonnées
        {
            new int[,] { {2, 800, 440}, {2, 500, 140}, {2, 900, 640} },
            new int[,] { { 1, 1000, 100 }, { 1, 1000, 700 }, { 1, 1400, 400 }, {2, 500, 340}, {2, 1400, 640}, {2, 1200, 140} },
            new int[,] { { 1, 100, 100 }, { 1, 1000, 100 }, { 1, 1000, 700 }, { 1, 1400, 400 } },
            new int[,] {{ 3, 450, 900} }

        };
        private static readonly double[,] TAILLE_SAUT = { { 61, 49 }, { 61, 49 }, { 61, 49 }, { 61, 49 }, };//taille des différentes images de l'animation

        private static readonly int[][,] PROPRIETES_PLATEFORMES = new int[][,] //définition des coordonnées des plateformes
        {
         new int[,] { { 425, 700 }, { 850, 700 }, { 1275, 700 },{ 600, 500 }, {300, 200 }, { 0, 700 }},
         new int[,] { { 0, 700 },{425 ,400 }, { 850, 400 }, { 1275, 700 }, { 850, 200 } },
         new int[,] { { 425, 700 }, { 850, 700 }, { 1275, 700 }},
         new int[,] { { 0, 800 }, { 425, 800 }, { 850, 800 } ,{ 1275,800}, { 75, 625 }, {1000, 625 }, { 538 , 450 } }
        };
        private static int[,] SAUT_TAILLE_ANIMATION = { { 61, 49 }, { 52, 64 }, { 50, 65 }, { 55, 57 }, { 60, 61 } }; //taille des différentes images de l'animation
        // Fin constantes, et début variables
        public DispatcherTimer minuterie, animationEntreeTimer;
        public int difficulte = 1, niveau = 0;
        public double volumeMusique = 1;
        public Key toucheGauche = Key.Q;
        public Key toucheDroite = Key.D;
        public Key toucheSaut = Key.Space;
        private static BitmapImage fond;
        private bool gauche, droite, saut, enSaut, billeBouge, pause, jouer, niveauGagne, animationEntreeBool, porteFerme, mort, phaseRoche = false, toucheG, toucheCtrl;
        private int vitesseJoueur = 8, gravite = 8, nbtouche = 0, choixBille;
        System.Drawing.Rectangle hitBoxJoueur, hitBoxBille, hitBoxEnnemi, hitBoxEnnemi2, hitBoxSac;
        private int animationJoueur = 1, animationSaut = 1, animationStatic = 1, timerAnimation, timerAnimationSaut, timerAnimationStatic, animationEntree = 1, timerAnimationEntree = 0, timerAnimationMort, animationMort = 1;
        private static Point clickPosition;
        private static double vitesseSaut, coefReductionDeplacementSaut;
        private static List<ProgressBar> barreDeVie = new List<ProgressBar>(); //liste des barre de vie des ennemis en jeu
        private static List<Ennemis> ennemisEnJeu = new List<Ennemis>(); //liste des ennemis en jeu
        private static List<Billes> billesEnJeu = new List<Billes>();// liste des billes en jeu
        private static List<Plateformes> plateformesEnJeu = new List<Plateformes>(); //liste des plateformes en jeu
        private static List<Sac> sacEnjeu = new List<Sac>(); //liste des sac en jeu
        private static int[] billeInventaire = new int[] { 3, 3, 3 }; //définition de l'inventaire l'index étant le type de bille et la valeur, le nombre de bille
        private static BitmapImage[] imageBilles;
        private static BitmapImage[] courseAnimationTab;
        private static BitmapImage[] sautAnimationTab;
        private static BitmapImage[] mortAnimationTab;
        private static BitmapImage[] marcheAnimationTab;
        private static BitmapImage[] inactifAnimationTab;
        private static BitmapImage[] ennemis;
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
            phaseRoche = false;
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
        private void InitFond() //définit le fond en fonction du niveau actuel
        {
            BitmapImage fondniveau = new BitmapImage
               (new Uri($"pack://application:,,,/img/fond_niveau/level{niveau}.jpeg"));
            canvasMainWindow.Background = new ImageBrush(fondniveau);
        }



        public void InitMusique(int indice) // lance une musique en fnction d'un indice demandé ( dans ce programme en fonction du niveau)
        {
            musique = new MediaPlayer();
            musique.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + $"/sons/musique{indice}.mp3"));
            musique.MediaEnded += RelanceMusique;
            musique.Volume = volumeMusique;
            musique.Play();
        }
        public void StopMusique() // stop la musique
        {
            musique.Stop();
        }
        public void ModifVolumeMusique(double volumeMusique) // permet de modifier le volume (UC paramètre) sans réinit le volume de la musqiue
        {
            musique.Volume = volumeMusique;
        }
        private void RelanceMusique(object? sender, EventArgs e) //recommence la musique quand elle est terminée
        {
            musique.Position = TimeSpan.Zero;
            musique.Play();
        }

        private void InitJeu()
        {
            canvasMainWindow.Focus();
            joueur.Visibility = Visibility.Visible; //rend visible certains éléments graphiques du jeu non généré
            ChoixBilleImg.Visibility = Visibility.Visible;
            ChoixBille.Visibility = Visibility.Visible;
            Canvas.SetLeft(joueur, -joueur.Width);
            Canvas.SetTop(joueur, PROPRIETES_PLATEFORMES[niveau - 1][0, 1] - joueur.Height); // positionnement du joueur en fonction du niveau (premiere plateforme)
            ChoixBille.Content = billeInventaire[choixBille]; //affiche le nombre de bille pour le type spécifié
            ChoixBilleImg.Source = imageBilles[choixBille]; // affiche le type de bille selectionné
        }

        private void InitRoche()
        {
            int nbRoche = NB_ROCHE_BOSS * difficulte;
            for (int i = 0; i < nbRoche; i++)
            {
                Ennemis ennemi = new Ennemis();
                ennemi.Texture = new Image();
                ennemi.Texture.Source = ennemis[3];
                ennemi.Texture.Width = 30;
                ennemi.Texture.Height = 22;
                ennemi.Vitesse = 15;
                ennemi.TypeDeplacement = 4;
                ennemisEnJeu.Insert(0, ennemi);
                canvasMainWindow.Children.Add(ennemisEnJeu[0].Texture);
                Canvas.SetTop(ennemisEnJeu[0].Texture, aleatoire.Next(-100, (int)(-2 * ennemi.Texture.Height)));
                Canvas.SetLeft(ennemisEnJeu[0].Texture, aleatoire.Next((int)ennemi.Texture.Width, (int)(this.Width - ennemi.Texture.Height)));
            }
        }
        private void InitImage()
        {
            imageBilles = new BitmapImage[3];
            for (int i = 0; i < imageBilles.Length; i++)
            {
                imageBilles[i] = new BitmapImage(new Uri($"pack://application:,,,/img/billes/bille{i + 1}.png"));
            }

            marcheAnimationTab = new BitmapImage[6];
            for (int i = 0; i < marcheAnimationTab.Length; i++)
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

            ennemis = new BitmapImage[4];
            for (int i = 0; i < ennemis.Length; i++)
            {
                ennemis[i] = new BitmapImage(new Uri($"pack://application:,,,/img/ennemi/ennemi{i + 1}.png"));
            }
            //definition de tableau d'images pour l'utiliser tout au long des animations et apparitions d'ennemis


        }
        private void InitEnnemis()
        {
            for (int i = 0; i < NIVEAU_ENNEMIS[niveau - 1].GetLength(0); i++)
            {
                // Initialisation ennemi , selon le tableau d'initialisation
                int multiplicationHP = 1;
                int multiplicationTailleX = 1;
                int multiplicationTailleY = 1;
                int multiplicationTailleLargeurBarre = 1;
                if (NIVEAU_ENNEMIS[niveau - 1][i, 0] == 3)
                {
                    multiplicationHP = 25;
                    multiplicationTailleX = 4;
                    multiplicationTailleY = 2;
                    multiplicationTailleLargeurBarre = 10;
                }
                Ennemis ennemi = new Ennemis();
                ennemi.Texture = new Image();
                ennemi.Texture.Source = ennemis[NIVEAU_ENNEMIS[niveau - 1][i, 0] - 1];
                ennemi.Texture.Width = 50 * multiplicationTailleX;
                ennemi.Texture.Height = 100 * multiplicationTailleY;
                ennemi.CoordonneeX = NIVEAU_ENNEMIS[niveau - 1][i, 1];
                ennemi.CoordonneeY = NIVEAU_ENNEMIS[niveau - 1][i, 2]; ;
                ennemi.Vitesse = 15;
                ennemi.TypeDeplacement = NIVEAU_ENNEMIS[niveau - 1][i, 0];
                ennemi.PointDeVie = 100 * multiplicationHP;
                ennemi.BarreDeVie = new ProgressBar();
                ennemi.BarreDeVie.Height = 10 * multiplicationTailleY;
                ennemi.BarreDeVie.Width = 75 * multiplicationTailleLargeurBarre;
                ennemi.BarreDeVie.Value = 100;
                ennemisEnJeu.Insert(0, ennemi);
                canvasMainWindow.Children.Add(ennemisEnJeu[0].Texture);
                canvasMainWindow.Children.Add(ennemisEnJeu[0].BarreDeVie);
                Canvas.SetTop(ennemisEnJeu[0].Texture, ennemisEnJeu[0].CoordonneeY);
                Canvas.SetLeft(ennemisEnJeu[0].Texture, ennemisEnJeu[0].CoordonneeX);
                if (NIVEAU_ENNEMIS[niveau - 1][i, 0] == 3)
                {
                    Canvas.SetTop(ennemisEnJeu[0].BarreDeVie, 30);
                    Canvas.SetLeft(ennemisEnJeu[0].BarreDeVie, this.Width - (ennemisEnJeu[0].BarreDeVie.Width * 1.5));
                    InitRoche();
                }
                else
                {
                    Canvas.SetTop(ennemisEnJeu[0].BarreDeVie, ennemisEnJeu[0].CoordonneeY);
                    Canvas.SetLeft(ennemisEnJeu[0].BarreDeVie, ennemisEnJeu[0].CoordonneeX);

                }
                Console.WriteLine("Ennemie apparu");
            }
        }


        private void InitTimer()
        {
            //timer pour faire fonctionner le jeu à 60 fps
            minuterie = new DispatcherTimer();
            minuterie.Interval = TimeSpan.FromMilliseconds(17);
            minuterie.Tick += Jeu;
            minuterie.Start();

        }

        private void InitPlateformes()
        {
            //initialise les plateformes en fonction du tableau d'initialisation
            for (int i = 0; i < PROPRIETES_PLATEFORMES[niveau - 1].GetLength(0); i++)
            {
                Plateformes nouvellePlateforme = new Plateformes(new Image(), new int(), new int(), new System.Drawing.Rectangle());
                nouvellePlateforme.Texture.Source = new BitmapImage(new Uri("pack://application:,,,/img/plateforme.png"));
                nouvellePlateforme.Texture.Width = 425;
                nouvellePlateforme.Texture.Height = 38;
                plateformesEnJeu.Insert(0, nouvellePlateforme);
                canvasMainWindow.Children.Add(plateformesEnJeu[0].Texture);

                Canvas.SetLeft(plateformesEnJeu[0].Texture, PROPRIETES_PLATEFORMES[niveau - 1][i, 0]);
                Canvas.SetTop(plateformesEnJeu[0].Texture, PROPRIETES_PLATEFORMES[niveau - 1][i, 1]);
                plateformesEnJeu[0].BoiteCollision = new System.Drawing.Rectangle((int)Canvas.GetLeft(plateformesEnJeu[0].Texture) - TOLERANCE_COLISION, (int)Canvas.GetTop(plateformesEnJeu[0].Texture) - TOLERANCE_COLISION, (int)plateformesEnJeu[0].Texture.Width + 2 * TOLERANCE_COLISION, (int)plateformesEnJeu[0].Texture.Height + 2 * TOLERANCE_COLISION);
            }
        }
        private void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //quand il y un roulement de la molette de la souris, la bille selectionnée change
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
            //vérification des inputs (touches appuyées)
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
            //vérification des inputs (touches relachées)
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
            //si le jeu est en cours et qu'un clic de souris est détecté alors appelle la méthode tir.
            if (jouer)
            {
                Tir(e);
            }
        }

        private void butQuitter_Click(object sender, RoutedEventArgs e)
        {
            //ferme le jeu si le bouton prévu à cet effet est appuyé
            this.Close();
        }

        public void Reprendre()
        {
            //reprend le jeu après la pause si appelé (Focus importan pour la redetection des touches)
            jouer = true;
            this.ControlContent.Content = null;
            canvasMainWindow.Focus();

        }
        private void Jeu(object? sender, EventArgs e)
        {
            // logique de jeu appel des différentes fonctions 
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
            //si colision avec la plateforme alors ne pas appliquer le verteur gravite 
            if (CollisionPlat() == 0)
            {
                gravite = 0;
                coefReductionDeplacementSaut = DEPLACEMENT_SOL;
                animationSaut = 1;
                ReinitialisationSaut();
            }
            //sinon appliquer le vecteur de gravité en mettant un coefficient de réduction de déplacement dans les airs
            else
            {

                gravite = 8;
                coefReductionDeplacementSaut = DEPLACEMENT_AIR;
                if (Canvas.GetTop(joueur) > this.Height)
                {
                    Canvas.SetTop(joueur, -joueur.Height);
                }

            }
            //on applique les différent déplacement si les touches sont enfoncées

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
            // évite les saut à l'infini, attend de ne plus être en saut pour l'être de nouveau
            hitBoxJoueur = new System.Drawing.Rectangle((int)Canvas.GetLeft(joueur), (int)Canvas.GetTop(joueur), (int)joueur.Width - 2, (int)joueur.Height - 2);
            if (saut && enSaut == false)
            {
                enSaut = true;
            }

        }




        private int CollisionPlat() //collision avec les plateformes par hitbox et calcul de dimensions pour couvrir tout les côtés et adapter les actions à réaliser
        {
            hitBoxJoueur = new System.Drawing.Rectangle((int)Canvas.GetLeft(joueur), (int)Canvas.GetTop(joueur), (int)joueur.Width, (int)joueur.Height);
            for (int i = 0; i < plateformesEnJeu.Count; i++)
            {
                if (hitBoxJoueur.IntersectsWith(plateformesEnJeu[i].BoiteCollision))
                {
                    Console.WriteLine($"Collision détectée avec la plateforme {i}");
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

        private int CollisionPlat(Sac sacDeBille) //surcharge simplifiée pour la collision des sac des billes qui tombent sur le sol
        {
            hitBoxSac = new System.Drawing.Rectangle((int)Canvas.GetLeft(sacDeBille.Texture), (int)Canvas.GetTop(sacDeBille.Texture), (int)sacDeBille.Texture.Width, (int)sacDeBille.Texture.Height);
            for (int i = 0; i < plateformesEnJeu.Count; i++)
            {
                if (hitBoxSac.IntersectsWith(plateformesEnJeu[i].BoiteCollision))
                {
                    Console.WriteLine("Au sol");
                    return 0;
                }
            }
            return -1;
        }
        private int CollisionPlat(Ennemis ennemi, double direction) // sucharge pour les ennemis slimes pour les empêcher de tombre de leur plateforme
        {
            hitBoxSac = new System.Drawing.Rectangle((int)(Canvas.GetLeft(ennemi.Texture) + (ennemi.Texture.Width * direction)), (int)Canvas.GetTop(ennemi.Texture), (int)(ennemi.Texture.Width + ennemi.Texture.Width), (int)ennemi.Texture.Height);
            for (int i = 0; i < plateformesEnJeu.Count; i++)
            {
                if (hitBoxSac.IntersectsWith(plateformesEnJeu[i].BoiteCollision))
                {
                    return 0;
                }
            }
            return -1;
        }
        private void SautEnCours() //permet de simuler un saut par translation exponentielle puis dégressive
        {
            if (vitesseSaut < 0)
            {
                AnimationSaut();
                Canvas.SetTop(joueur, Canvas.GetTop(joueur) + vitesseSaut);
                vitesseSaut = vitesseSaut + gravite / 6;
                Console.WriteLine($"Vitesse saut : {vitesseSaut}, Position : {Canvas.GetTop(joueur)}");
                if (Canvas.GetTop(joueur) - vitesseSaut < 0)
                {
                    vitesseSaut = 1;
                }
            }
            else
            {
                AnimationChute();
            }
        }

        private void Tir(MouseButtonEventArgs e) //méthode de tir, initialisant un bille en fonction d'un type , lui attribuant une vitesse en fonction de la distance de la souris du joueur
                                                 // en l'orientant dans la bonne direction, et en adaptant l'inventaire en fonction
        {
            if (billeInventaire[choixBille] > 0)
            {
                Billes nouvelleBille = new Billes(new Image(), 0, 0, 0, 0);
                nouvelleBille.Texture.Source = imageBilles[choixBille];
                nouvelleBille.Texture.Width = 16;
                nouvelleBille.Texture.Height = 16;
                nouvelleBille.TypeBille = choixBille;
                nouvelleBille.DegatBille = 25 / difficulte;
                clickPosition = e.GetPosition(this);
                double vitesseX = clickPosition.X - Canvas.GetLeft(joueur);
                double vitesseY = clickPosition.Y - Canvas.GetTop(joueur);
                nouvelleBille.Vitesse[0] = vitesseX;
                nouvelleBille.Vitesse[1] = vitesseY;
                billesEnJeu.Insert(0, nouvelleBille);
                canvasMainWindow.Children.Add(nouvelleBille.Texture);
                Canvas.SetTop(nouvelleBille.Texture, Canvas.GetTop(joueur));
                Canvas.SetLeft(nouvelleBille.Texture, Canvas.GetLeft(joueur));
                billeInventaire[choixBille]--;
                ChoixBille.Content = billeInventaire[choixBille];
                Console.WriteLine($"Nouvelle bille tirée : VitesseX = {vitesseX}, VitesseY = {vitesseY}");

            }
        }
        private void ApparitionSac() // permet de faire apparaitre des sacs contenant des billes en dans des proportions en fonction des niveau avec un peu d'aléatoire 
        {
            Sac nouveauSac = new Sac(new Image());
            nouveauSac.Texture = new Image();
            nouveauSac.Texture.Source = new BitmapImage(new Uri("pack://application:,,,/img/sacBille.png"));
            nouveauSac.Texture.Width = 32;
            nouveauSac.Texture.Height = 32;
            for (int i = 0; i < NIVEAU_BILLE.GetLength(1); i++)
            {
                int typeBilleContenu = NIVEAU_BILLE[niveau - 1, i];
                nouveauSac.Contenu[typeBilleContenu] = aleatoire.Next(3, 6);
            }
            sacEnjeu.Insert(0, nouveauSac);
            canvasMainWindow.Children.Add(sacEnjeu[0].Texture);
            Canvas.SetTop(nouveauSac.Texture, -nouveauSac.Texture.Height);
            Canvas.SetLeft(nouveauSac.Texture, aleatoire.Next(10, (int)this.Width - 10));
            Console.WriteLine($"Sac apparu à ({Canvas.GetLeft(nouveauSac.Texture)}, {Canvas.GetTop(nouveauSac.Texture)}) avec contenu {nouveauSac.Contenu}");

        }

        private void DeplacementSac() //fait déplacer les sac soumis au vecteur gravité jusqu'à une collision avec une plateforme
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
        private bool BilleLance(Billes bille) //simulation de la physique des trajectoires des billes et colisions
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

        private void DeplacementEnnemi() //déplacements des ennemis en fonctions du type d'ennemis
        {
            for (int i = 0; i < ennemisEnJeu.Count; i++)
            {
                if (ennemisEnJeu[i].TypeDeplacement == 1)
                {
                    //suit le joueur sur les deux axes
                    Canvas.SetLeft(ennemisEnJeu[i].Texture, Canvas.GetLeft(ennemisEnJeu[i].Texture) + Math.Sign(Canvas.GetLeft(joueur) - Canvas.GetLeft(ennemisEnJeu[i].Texture)) * 2 * Math.Sqrt((double)difficulte));
                    Canvas.SetTop(ennemisEnJeu[i].Texture, Canvas.GetTop(ennemisEnJeu[i].Texture) + Math.Sign(Canvas.GetTop(joueur) - Canvas.GetTop(ennemisEnJeu[i].Texture)));
                    Canvas.SetLeft(ennemisEnJeu[i].BarreDeVie, Canvas.GetLeft(ennemisEnJeu[i].Texture) + Math.Sign(Canvas.GetLeft(joueur) - Canvas.GetLeft(ennemisEnJeu[i].Texture)) * 2 * Math.Sqrt((double)difficulte));
                    Canvas.SetTop(ennemisEnJeu[i].BarreDeVie, Canvas.GetTop(ennemisEnJeu[i].Texture) + Math.Sign(Canvas.GetTop(joueur) - Canvas.GetTop(ennemisEnJeu[i].Texture)));
                }
                else if (ennemisEnJeu[i].TypeDeplacement == 2)
                {
                    if ((Canvas.GetLeft(joueur) > Canvas.GetLeft(ennemisEnJeu[i].Texture) && CollisionPlat(ennemisEnJeu[i], 1) == 0)
                        || Canvas.GetLeft(joueur) < Canvas.GetLeft(ennemisEnJeu[i].Texture) && CollisionPlat(ennemisEnJeu[i], -2) == 0)
                    {
                        // suit le joueur sur l'axe horizontal
                        Canvas.SetLeft(ennemisEnJeu[i].Texture, Canvas.GetLeft(ennemisEnJeu[i].Texture) + Math.Sign(Canvas.GetLeft(joueur) - Canvas.GetLeft(ennemisEnJeu[i].Texture)) * 2 * Math.Sqrt((double)difficulte));
                        Canvas.SetLeft(ennemisEnJeu[i].BarreDeVie, Canvas.GetLeft(ennemisEnJeu[i].Texture) + Math.Sign(Canvas.GetLeft(joueur) - Canvas.GetLeft(ennemisEnJeu[i].Texture)) * 2 * Math.Sqrt((double)difficulte));
                    }

                }
                hitBoxEnnemi = new System.Drawing.Rectangle((int)Canvas.GetLeft(ennemisEnJeu[i].Texture), (int)Canvas.GetTop(ennemisEnJeu[i].Texture), (int)ennemisEnJeu[i].Texture.Width, (int)ennemisEnJeu[i].Texture.Height);
                Console.WriteLine($"Ennemi {i} position : ({Canvas.GetLeft(ennemisEnJeu[i].Texture)}, {Canvas.GetTop(ennemisEnJeu[i].Texture)})");



                if (ennemisEnJeu[i].TypeDeplacement == 3)
                {

                    Console.WriteLine("X : " + Canvas.GetLeft(ennemisEnJeu[i].Texture));
                    Canvas.SetLeft(ennemisEnJeu[i].Texture, Canvas.GetLeft(ennemisEnJeu[i].Texture) + ennemisEnJeu[i].Vitesse);

                    if (Canvas.GetLeft(ennemisEnJeu[i].Texture) + ennemisEnJeu[i].Texture.Width <= 0)
                    {
                        ennemisEnJeu[i].Vitesse = 15;
                        Canvas.SetTop(ennemisEnJeu[i].Texture, aleatoire.Next(200, 700));
                        Console.WriteLine("Y : " + Canvas.GetTop(ennemisEnJeu[i].Texture));

                    }
                    if (Canvas.GetLeft(ennemisEnJeu[i].Texture) > this.Width + 100)
                    {

                        Canvas.SetTop(ennemisEnJeu[i].Texture, aleatoire.Next(300, 700));
                        ennemisEnJeu[i].Vitesse = -15;
                        Console.WriteLine("Y : " + Canvas.GetTop(ennemisEnJeu[i].Texture));
                    }
                    if (ennemisEnJeu[i].PointDeVie < 2500 / 2)
                    {
                        phaseRoche = true;
                    }
                }
                if (phaseRoche && ennemisEnJeu[i].TypeDeplacement == 4)
                {
                    Canvas.SetTop(ennemisEnJeu[i].Texture, Canvas.GetTop(ennemisEnJeu[i].Texture) + ennemisEnJeu[i].Vitesse);
                    if (Canvas.GetTop(ennemisEnJeu[i].Texture) > this.Height)
                    {
                        Canvas.SetTop(ennemisEnJeu[i].Texture, -ennemisEnJeu[i].Texture.Height);
                        Canvas.SetLeft(ennemisEnJeu[0].Texture, aleatoire.Next((int)ennemisEnJeu[i].Texture.Width, (int)(this.Width - ennemisEnJeu[i].Texture.Height)));
                    }
                }
                ennemisEnJeu[i].HitBox = new System.Drawing.Rectangle((int)Canvas.GetLeft(ennemisEnJeu[i].Texture), (int)(Canvas.GetTop(ennemisEnJeu[i].Texture)), (int)ennemisEnJeu[i].Texture.Width, (int)(ennemisEnJeu[i].Texture.Height));

            }



        }
        private void ColisionSac(Sac sacDeBille) // si le joueur rentre en collision avec un sac de bille et rempli son inventaire en fonction du contenu du sac 
        {

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
        private bool ColisionEnnemi(Billes bille) // si une bille rentre en collision avec un ennemi alors il applique les dégâts et fait disparaitre l'ennemi si necessaire
        {
            if (bille.TypeBille != 1)
            {
                for (int i = 0; i < ennemisEnJeu.Count; i++)
                {

                    if (ennemisEnJeu[i].TypeDeplacement != 4)
                    {
                        if (hitBoxBille.IntersectsWith(ennemisEnJeu[i].HitBox))
                        {
                            ennemisEnJeu[i].PointDeVie -= bille.DegatBille;
                            if (ennemisEnJeu[i].TypeDeplacement == 3)
                            {
                                ennemisEnJeu[i].BarreDeVie.Value -= (double)(bille.DegatBille) / 25;
                            }
                            else
                            {
                                ennemisEnJeu[i].BarreDeVie.Value -= bille.DegatBille;
                            }

                            if (ennemisEnJeu[i].PointDeVie <= 0)
                            {


                                if (ennemisEnJeu[i].TypeDeplacement == 3)
                                {
                                    canvasMainWindow.Children.Remove(ennemisEnJeu[i].Texture);
                                    canvasMainWindow.Children.Remove(ennemisEnJeu[i].BarreDeVie);
                                    ennemisEnJeu.Remove(ennemisEnJeu[i]);
                                    DestructionRoche();
                                }
                                else
                                {
                                    canvasMainWindow.Children.Remove(ennemisEnJeu[i].Texture);
                                    canvasMainWindow.Children.Remove(ennemisEnJeu[i].BarreDeVie);
                                    ennemisEnJeu.Remove(ennemisEnJeu[i]);
                                }

                                ReinitialisationSaut();

                            }
                            return true;
                        }
                    }


                }
            }
            return false;
        }

        private void DestructionRoche()
        {
            int nbRoche = ennemisEnJeu.Count;
            for (int i = 0; i < nbRoche; i++)
            {
                canvasMainWindow.Children.Remove(ennemisEnJeu[0].Texture);
                ennemisEnJeu.Remove(ennemisEnJeu[0]);
            }
        }
        private bool VerifTouche()//si un ennemi touche le joueur alors retourne vrai
        {
            for (int i = 0; i < ennemisEnJeu.Count; i++)
            {
                bool ennemiTouche = ennemisEnJeu[i].HitBox.IntersectsWith(hitBoxJoueur);
                if (ennemiTouche == true)
                    return ennemiTouche;
            }
            return false;
        }

        private void ReinitialisationSaut() // quand appelé réinitialise les variables de saut.
        {
            enSaut = false;
            vitesseSaut = -35;


        }
        private void FinJeu() // arrete la minuterie
        {
            minuterie.Stop();
        }
        private void Richesse() // code de triche donnant un nombre véridique de bille
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
        private void FinNiveau() // si le joueur va à l'endroit spécifié dans les règles quand il a tué tous les ennemis alors initie la procédure de destruction du niveau
        {

            if (Canvas.GetLeft(joueur) + joueur.Width > this.Width - 25 && Canvas.GetTop(joueur) > 500 && Canvas.GetTop(joueur) < 900)
            {
                Console.WriteLine("Fin de niveau atteinte");
                DestructionNiveau();
            }


        }

        private void DestructionNiveau() // destruction de tout les éléments relatif au niveau et renvoi vers un écran en fonction de l'issu
        {
            int plat = plateformesEnJeu.Count;
            int bille = billesEnJeu.Count;
            int sac = sacEnjeu.Count;
            for (int i = 0; i < plat; i++)
            {
                canvasMainWindow.Children.Remove(plateformesEnJeu[0].Texture);
                plateformesEnJeu.Remove(plateformesEnJeu[0]);
            }
            for (int i = 0; i < bille; i++)
            {
                canvasMainWindow.Children.Remove(billesEnJeu[0].Texture);
                billesEnJeu.Remove(billesEnJeu[0]);
            }
            for (int i = 0; i < sac; i++)
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
                fond = $"fond_niveau/level{niveau + 1}.jpeg";
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
        //ci-dessous différentes animations de déplacements du joueur
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
            regard.ScaleX = 1;
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
                if (timerAnimation == TIMER_ANIMATION)
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
            joueur.Width = SAUT_TAILLE_ANIMATION[animationSaut - 1, 0];
            joueur.Height = SAUT_TAILLE_ANIMATION[animationSaut - 1, 1];
            joueur.Source = sautAnimationTab[animationSaut - 1];
            timerAnimationSaut += 1;
            if (timerAnimationSaut == TIMER_ANIMATION && animationSaut < 4)
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
            joueur.Source = inactifAnimationTab[animationStatic - 1];
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

        //ci-dessous les différentes class permettant de donner des attributes à des objets créés comme ennemi, bille ou sac.



        public partial class Ennemis
        {
            private int coordonneeX, coordonneeY, typeDeplacement;
            private Image texture;
            private double vitesse, pointDeVie;
            private ProgressBar barreDeVie;
            private System.Drawing.Rectangle hitBox;

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
            public double PointDeVie
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
            public System.Drawing.Rectangle HitBox
            {
                get { return hitBox; }
                set { hitBox = value; }
            }
            public System.Drawing.Rectangle HitBox2
            {
                get { return hitBox; }
                set { hitBox = value; }
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
                this.Contenu = new int[] { 0, 0, 0 };

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
}