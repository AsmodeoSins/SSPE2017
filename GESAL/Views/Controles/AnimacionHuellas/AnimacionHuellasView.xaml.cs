using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GESAL.Views
{
    /// <summary>
    /// Interaction logic for AnimacionHuellas.xaml
    /// </summary>
    public partial class AnimacionHuellasView : UserControl
    {
        private int currentIndex;
        private List<ImageSource> images;
        private DispatcherTimer updateImageTimer;
        private DoubleAnimation myDoubleAnimation;
        private Storyboard myStoryboard;
        private Random randomizer;
        public AnimacionHuellasView()
        {
            InitializeComponent();
            randomizer = new Random();


            //inicializa el timer
            updateImageTimer = new DispatcherTimer(DispatcherPriority.Render);
            updateImageTimer.Interval = TimeSpan.FromMilliseconds(100.0);
            updateImageTimer.Tick += new EventHandler(updateImageTimer_Tick);

            //inicializa animacion
            myDoubleAnimation = new DoubleAnimation();
            myStoryboard=new Storyboard();
        }

        public void Load(List<ImageSource> _images)
        {
            updateImageTimer.Stop();
            images = _images;
            currentIndex = 0;
            LoadCurrentIndex();
        }

        public void FrozeenImage()
        {
            contenedor_sequencia.Source = images[currentIndex];
            Ln.Visibility = Visibility.Hidden;
        }

        private void LoadCurrentIndex()
        {
            if (((images != null) && (currentIndex < images.Count)) && (currentIndex >= 0))
            {
                contenedor_sequencia.Source = images[currentIndex];
            }
        }

        public void Play()
        {

            Ln.Visibility = Visibility.Visible;
            
            myDoubleAnimation.From = 0;
            myDoubleAnimation.To = 105;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.3));
            myDoubleAnimation.AutoReverse = true;
            myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            Storyboard.SetTargetName(myDoubleAnimation, "Ln");
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Canvas.TopProperty));
            myStoryboard.Children.Add(myDoubleAnimation);
            myStoryboard.Begin(Ln);

            currentIndex = 0;
            updateImageTimer.Start();

        }

        public void Stop()
        {
            updateImageTimer.Stop();
            myStoryboard.Stop(Ln);
            myStoryboard.Children.Clear();
            Ln.Visibility = Visibility.Hidden;
        }

        private void updateImageTimer_Tick(object sender, EventArgs e)
        {
            currentIndex = randomizer.Next(0, images.Count - 1);
            if (images != null)
            {
                LoadCurrentIndex();
            }
        }
    }
}
