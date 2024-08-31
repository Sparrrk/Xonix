using FinalXonixWPF.Characters;
using FinalXonixWPF.GameField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace FinalXonixWPF.Generators
{
    internal class GameGenerator
    {
        public LevelGenerator levelGenerator = new LevelGenerator();

        public GameGenerator()
        {

        }

        /// <summary>
        /// текущий уровень
        /// </summary>
        public int Level = 1;

        /// <summary>
        /// указывает, активен ли игровой процесс
        /// </summary>
        public bool IsActive { get; set; } = false;

        /// <summary>
        /// процент захваченных игроком территорий
        /// </summary>
        public double CapturedPercent { get; set; }

        /// <summary>
        /// текущее количество жизней
        /// </summary>
        public int Lifes = 3;

        public void SetupGame()
        {
            levelGenerator.SetupLevel(Level);
            CapturedPercent = 0;
            SetInteractions();
        }

        private void SetInteractions()
        {
            levelGenerator.Hero.RegionIsCaptured += RecalculateCapturedPercent;
            levelGenerator.Hero.HeroLose += HeroLoseHandler;

            foreach (SeaEnemy seaEnemy in levelGenerator.SeaEnemies)
            {
                seaEnemy.HeroLose += HeroLoseHandler;
            }
            foreach (GroundEnemy groundEnemy in levelGenerator.GroundEnemies)
            {
                groundEnemy.HeroLose += HeroLoseHandler;
            }
        }


        public void CreateNewLevel()
        {

            if (Level % 3 == 0 && Lifes < 5)
                Lifes++;
            Level++;
            levelGenerator.DeleteSeaEnemies();
            levelGenerator.DeleteGroundEnemies();
            HeroWin?.Invoke(Level);

            SetupGame();
        }

        /// <summary>
        /// Подсчитывает, какую часть игрового поля захватил игрок. При достижении значения 80% инициирует событие победы игрока 
        /// </summary>
        private void RecalculateCapturedPercent()
        {
            int counterSea = 0;
            int counterGround = 0;
            for (int i = 2; i < levelGenerator.Field.Height - 2; i++)
            {
                for (int j = 2; j < levelGenerator.Field.Width - 2; j++)
                {
                    if (levelGenerator.Field[i, j].Type == CellType.Sea)
                        counterSea++;
                    else if (levelGenerator.Field[i, j].Type == CellType.Ground)
                        counterGround++;
                }
            }
            CapturedPercent = (double)counterGround / (double)(counterGround + counterSea);
            if (CapturedPercent > 0.80)
            {
                CreateNewLevel();
                GameInfoChange?.Invoke();
            }
        }

        private void HeroLoseHandler()
        {
            levelGenerator.Field[levelGenerator.Hero.Y, levelGenerator.Hero.X].NeedRedraw = true;
            Lifes--;
            levelGenerator.Field[levelGenerator.Hero.Y, levelGenerator.Hero.X].Characters.Remove(levelGenerator.Hero);
            levelGenerator.Hero.Y = 0;
            levelGenerator.Hero.X = levelGenerator.Field.Width / 2;
            levelGenerator.Hero.Direction = Direction.None;
            levelGenerator.Field.PathCleaner();
            GameInfoChange?.Invoke();
            if (Lifes == 0)
                GameLose?.Invoke();
        }

        public delegate void OnHeroWin(int level);

        public event OnHeroWin HeroWin;

        public event Action GameInfoChange;

        public event Action GameLose;
    }
}
