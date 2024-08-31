using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FinalXonixWPF.GameField;

namespace FinalXonixWPF.Characters
{
    internal class Hero : Character
    {
        /// <summary>
        /// конструктор создания класса игрового персонажа
        /// </summary>
        /// <param name="x">координата х</param>
        /// <param name="y">координата у</param>
        public Hero(int x, int y) : base(x, y, CharacterType.Hero, Direction.None)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// изменить координату персонажа в соответствии с заданным направлением
        /// </summary>
        /// <param name="field">игровое поле</param>
        public void Move(Field field)
        {
            Cell curentCell = field[Y, X];
            Delta delta = Delta.ToDeltas(Direction);

            int newX = X + delta.DX;
            int newY = Y + delta.DY;

            if (newX < 0 || newX >= field.Width || newY < 0 || newY >= field.Height)
            {
                Direction = Direction.None;
            }
            else
            {
                Move(field, newX, newY);
                //if (field[newY, newX].Characters.Find(x => x.Type == CharacterType.Bonus) != null)
                    //GetBonus?.Invoke();
                if (field[newY, newX].Type == CellType.Path)
                    HeroLose?.Invoke();
            }
        }

        /// <summary>
        /// изменить позицию игрока в соответсвии с нажатыми пользователем клавишами
        /// </summary>
        /// <param name="currentDirection">текущее направление движения</param>
        /// <returns>Новое направление движения</returns>
        public Direction ChangeDirection(Direction currentDirection, Key key)
        {
            switch (key)
            {
                case Key.Up:
                    if (currentDirection != Direction.Down)
                        currentDirection = Direction.Up;
                    break;

                case Key.Down:
                    if (currentDirection != Direction.Up)
                        currentDirection = Direction.Down;
                    break;

                case Key.Right:
                    if (currentDirection != Direction.Left)
                        currentDirection = Direction.Right;
                    break;

                case Key.Left:
                    if (currentDirection != Direction.Right)
                        currentDirection = Direction.Left;
                    break;

                default:
                    break;
            }
            return currentDirection;
        }

        /// <summary>
        /// событие, сигнализирующее о потере игроком одной жизни
        /// </summary>
        public event Action HeroLose;

        /// <summary>
        /// событие, сигнализирующее о поднятии игроком бонуса
        /// </summary>
        public event Action GetBonus;
    }
}
