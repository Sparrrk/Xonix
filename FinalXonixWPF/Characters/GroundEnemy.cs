using FinalXonixWPF.GameField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalXonixWPF.Characters
{
    internal class GroundEnemy : Character
    {
        /// <summary>
        /// конструктор создания наземного противника
        /// </summary>
        /// <param name="x">координата x</param>
        /// <param name="y">координата y</param>
        /// <param name="direction">направление движения</param>
        public GroundEnemy(int x, int y, Direction direction) : base(x, y, CharacterType.GroundEnemy, direction)
        {
            X = x;
            Y = y;
            Direction = direction;
        }

        /// <summary>
        /// функция движения сухопутного противника
        /// </summary>
        /// <param name="field">игровое поле</param>
        public void Move(Field field)
        {
            Cell currentCell = field[Y, X];

            Delta delta = Delta.ToDeltas(Direction);

            CollisionHandler(field, delta);

            int newX = X + delta.DX;
            int newY = Y + delta.DY;

            Move(field, newX, newY);

            if (field[newY, newX].Characters.FirstOrDefault().Type == CharacterType.Hero)
                HeroLose?.Invoke();
        }

        /// <summary>
        /// меняет направление противника в результате столкновений
        /// </summary>
        /// <param name="field">игровое поле</param>
        /// <param name="delta">дельта, соответствующая направлению движения</param>
        private void CollisionHandler(Field field, Delta delta)
        {
            int newX = X + delta.DX;
            int newY = Y + delta.DY;
            int flag = 0;
            if (newX >= field.Width || newX < 0 || field[Y, newX].Type != CellType.Ground)
            {
                delta.DX *= -1;
                flag = 1;
            }
            if (newY >= field.Height || newY < 0 || field[newY, X].Type != CellType.Ground)
            {
                delta.DY *= -1;
                flag = 1;
            }
            Direction = Delta.ToDirections(delta);
            if (flag == 1)
                CollisionHandler(field, delta);
        }

        /// <summary>
        /// событие проигрыша игрока. Инициируется при столкновении противника с игроком
        /// </summary>
        public event Action HeroLose;
    }
}

