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
        private bool gauche, droite, saut = false, enSaut;
        private int vitesseJoueur = 4, sautJoueur = 8, gravite = 0;
        System.Drawing.Rectangle hitBoxSol, hitBoxJoueur;
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
            if (e.Key == Key.Left)
            {
                gauche = true;
            }

            else if (e.Key == Key.Right)
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
            if (e.Key == Key.Left)
            {
                gauche = false;
            }
            else if (e.Key == Key.Right) 
            {
                droite = false;
            }
                
            else if (e.Key == Key.Space)
            {
                saut = false;
            }
        }
        private void Jeu(object? sender, EventArgs e)
        {
            deplacement();

        }
        private void deplacement()
        {

             if (auSol())
            {
                gravite = 0;
                if (Canvas.GetTop(joueur) > hitBoxSol.Top - joueur.Height)
                {
                    Canvas.SetTop(joueur, hitBoxSol.Top - joueur.Height);
                    enSaut = false;
                }
            }
            else
            {
                gravite = 3;
            }
            Canvas.SetTop(joueur, Canvas.GetTop(joueur) + gravite);
            
            if (droite)
            {
                if ((Canvas.GetLeft(joueur) + sautJoueur) + joueur.Width < this.ActualWidth)
                {
                    Canvas.SetLeft(joueur, Canvas.GetLeft(joueur) + vitesseJoueur);
                }
                
            }

            if (gauche)
            {
                if ((Canvas.GetLeft(joueur) - sautJoueur) > 0)
                {
                    Canvas.SetLeft(joueur, Canvas.GetLeft(joueur) - vitesseJoueur);
                }
                    
            }
            if (saut)
            {
                enSaut = true;
                if ((Canvas.GetTop(joueur) - sautJoueur) > 0)
                {
                    Canvas.SetTop(joueur, Canvas.GetTop(joueur) - sautJoueur);
                }
            }

        }
        private bool auSol()
        {

            hitBoxJoueur = new System.Drawing.Rectangle((int)Canvas.GetLeft(joueur), (int)Canvas.GetTop(joueur), (int)joueur.Width, (int)joueur.Height);
            bool estAuSol = hitBoxSol.IntersectsWith(hitBoxJoueur);
            return estAuSol;
        }
        

    }
}