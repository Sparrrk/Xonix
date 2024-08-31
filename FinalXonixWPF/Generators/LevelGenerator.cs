using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using FinalXonixWPF.Characters;
using FinalXonixWPF.GameField;

namespace FinalXonixWPF.Generators
{
    internal class LevelGenerator
    {
        /// <summary>
        /// игровое поле
        /// </summary>
        public Field Field { get; set; }
        /// <summary>
        /// игровой персонаж
        /// </summary>
        public Hero Hero { get; set; }
        /// <summary>
        /// список морских противников
        /// </summary>
        public List<SeaEnemy> SeaEnemies { get; set; } = new List<SeaEnemy>();
        /// <summary>
        /// список сухопутных противников
        /// </summary>
        public List<GroundEnemy> GroundEnemies { get; set; } = new List<GroundEnemy> ();
        /// <summary>
        /// класс-рандомайзер для случайной генерации начальной позиции врагов и бонусов
        /// </summary>
        private Random random = new Random();
        /// <summary>
        /// список разрешенных направлений движения для противников
        /// </summary>
        private Direction[] directions = { Direction.UpLeft, Direction.UpRight, Direction.DownRight, Direction.DownLeft };

        /// <summary>
        /// сгенерировать новый уровень
        /// </summary>
        public void SetupLevel(int level)
        {
            Field = new Field();

            Hero = new Hero(Field.Width / 2, 0);

            SetupEnemies(level);

            SetInteractions();
        }
        /// <summary>
        /// сгенерировать морских и сухопутных врагов в соотвествии с уровнем
        /// </summary>
        /// <param name="level"></param>
        public void SetupEnemies(int level)
        {
            for (int i = 0; i < level && i < 5; i++)
                SetupSeaEnemy();

            for (int i = 0; i < level / 2 && i < 3; i++)
                SetupGroundEnemy();
        }

        /// <summary>
        /// сгенерировать морского противника
        /// </summary>
        private void SetupSeaEnemy()
        {
            int x = random.Next(2, Field.Width - 2);
            int y = random.Next(2, Field.Height - 2);
            if (Field[y, x].Type == CellType.Sea && Field[y, x].Characters.Count == 0)
            {
                SeaEnemy seaEnemy = new SeaEnemy(x, y, directions[random.Next(0, 4)]);
                SeaEnemies.Add(seaEnemy);
                Field[y, x].Characters.Add(seaEnemy);
                seaEnemy.EnemyEaten += DeleteEnemy;
                seaEnemy.EnemyResurrect += EnemyResurrectAsync;
            }
            else
            {
                SetupSeaEnemy();
            }
        }

        /// <summary>
        /// сгенерировать сухопутного противника
        /// </summary>
        private void SetupGroundEnemy()
        {
            int x = random.Next(0, Field.Width - 1);
            int y = random.Next(0, Field.Height - 1);
            if (Field[y, x].Type == CellType.Ground && Field[y, x].Characters.Count == 0)
            {
                GroundEnemy groundEnemy = new GroundEnemy(x, y, directions[random.Next(0, 4)]);
                GroundEnemies.Add(groundEnemy);
                Field[y, x].Characters.Add(groundEnemy);
            }
            else
            {
                SetupGroundEnemy();
            }
        }

        private void SetInteractions()
        {
            Hero.RegionIsCaptured += Field.RegionCapturedHandler;
        }

        /// <summary>
        /// удалить всех морских врагов
        /// </summary>
        public void DeleteSeaEnemies()
        {
            foreach (SeaEnemy seaEnemy in SeaEnemies)
            {
                Field[seaEnemy.Y, seaEnemy.X].Characters.Remove(seaEnemy);
            }
            SeaEnemies.Clear();
        }

        /// <summary>
        /// удалить съеденного врага. Метод инициирует событие, запускающее таймер воскрешения съеденного противника
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DeleteEnemy(object sender, EventArgs e)
        {
            SeaEnemy enemy = (SeaEnemy)sender;
            SeaEnemies.Remove(enemy);
            Field[enemy.Y, enemy.X].Characters.Remove(enemy);
        }

        /// <summary>
        /// удалить всех наземных врагов
        /// </summary>
        public void DeleteGroundEnemies()
        {
            foreach (GroundEnemy GroundEnemy in GroundEnemies)
            {
                Field[GroundEnemy.Y, GroundEnemy.X].Characters.Remove(GroundEnemy);
            }
            GroundEnemies.Clear();
        }

        public void MakeOneStep()
        {
            Hero.Move(Field);
            for (int i = 0; i < SeaEnemies.Count; i++)
                SeaEnemies[i].Move(Field);
            for (int i = 0; i < GroundEnemies.Count; i++)
                GroundEnemies[i].Move(Field);

        }

        public void PrintCharacters(Canvas canvas)
        {
            Hero.PrintCharacter(canvas, Field);
            foreach (SeaEnemy enemy in SeaEnemies)
                enemy.PrintCharacter(canvas, Field);
            foreach (GroundEnemy groundEnemy in GroundEnemies)
                groundEnemy.PrintCharacter(canvas, Field);
        }

        /// <summary>
        /// Асинхронный метод возрождения противника
        /// </summary>
        /// <param name="sender">съеденный противник</param>
        /// <param name="e">пустой EventArgs</param>
        /// <returns></returns>
        private async Task EnemyResurrectAsync()
        {
            await Task.Run(() => EnemyResurrect());
        }

        /// <summary>
        /// возрождает съеденного противника через 6 секунд
        /// </summary>
        /// <param name="sender">съеденный противник</param>
        /// <param name="e">пустой EventArgs</param>
        private void EnemyResurrect()
        {
            Thread.Sleep(6000);
            SetupSeaEnemy();
        }
    }
}
