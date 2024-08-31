using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace FinalXonixWPF
{
    internal class Images
    {
        public BitmapImage[] images = new BitmapImage[5];
        
        private Random random = new Random();

        public Images()
        {
            images[0] = new BitmapImage(new Uri("Assets/WatermelonFloppa.jpg", UriKind.Relative));
            images[1] = new BitmapImage(new Uri("Assets/FloppaWithSunglasses.jpg", UriKind.Relative));
            images[2] = new BitmapImage(new Uri("Assets/DefaultFloppa.jpg", UriKind.Relative));
            images[3] = new BitmapImage(new Uri("Assets/DefaultFloppa2.jpg", UriKind.Relative));
            images[4] = new BitmapImage(new Uri("Assets/FloppaSoundEngineer.jpg", UriKind.Relative));

            for (int i = images.Length - 1; i >= 1; i--)
            {
                int j = random.Next(i + 1);
                var temp = images[j];
                images[j] = images[i];
                images[i] = temp;
            }
        }
    }
}
