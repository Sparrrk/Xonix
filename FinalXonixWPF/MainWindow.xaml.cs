using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FinalXonixWPF.Characters;
using FinalXonixWPF.GameField;
using FinalXonixWPF.Generators;

namespace FinalXonixWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameGenerator gameGenerator;
        Images images;

        public MainWindow()
        {
            InitializeComponent();
            images = new Images();
            gameGenerator = new GameGenerator();
            gameGenerator.HeroWin += ChangeImage;
            gameGenerator.GameInfoChange += GameInfoHandler;
            gameGenerator.GameLose += GameLoseHandler;
            gameGenerator.SetupGame();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HiddenImage.Source = images.images[0];
        }

        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Overlay.Visibility = Visibility.Hidden;
                gameGenerator.IsActive = !gameGenerator.IsActive;
                await RunGame(gameGenerator.levelGenerator);
            }
            else
            {
                Hero hero = gameGenerator.levelGenerator.Hero;
                hero.Direction = hero.ChangeDirection(hero.Direction, e.Key);
            }
        }


        private async void cCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await RunGame(gameGenerator.levelGenerator);
        }

        /// <summary>
        /// запустить игровой процесс
        /// </summary>
        /// <param name="levelGenerator"></param>
        /// <returns></returns>
        private async Task RunGame(LevelGenerator levelGenerator)
        {
            levelGenerator.Field.PrintField(levelGenerator, cCanvas);
            while (gameGenerator.IsActive)
            {
                await Task.Delay(150);
                levelGenerator.MakeOneStep();
                levelGenerator.Field.PrintField(levelGenerator, cCanvas);
            }
        }

        private void ChangeImage(int level)
        {
            HiddenImage.Source = images.images[level % 5];
            cCanvas.Children.Clear();
        }

        private void GameInfoHandler()
        {
            tbInfo.Text = $"Level: {gameGenerator.Level.ToString()}, Lifes: {gameGenerator.Lifes.ToString()}";
        }

        private void GameLoseHandler()
        {
            gameGenerator.IsActive = false;
            gameGenerator.Level = 0;
            gameGenerator.Lifes = 2;
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "Game over! Press esc to retry";
            gameGenerator.CreateNewLevel();
            GameInfoHandler();
        }


    }   
}
