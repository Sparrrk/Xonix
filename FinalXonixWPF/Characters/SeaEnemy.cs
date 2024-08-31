using FinalXonixWPF.GameField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalXonixWPF.Characters
{
    internal class SeaEnemy : Character
    {
        public SeaEnemy(int x, int y, Direction direction) : base(x, y, CharacterType.SeaEnemy, direction)
        {
            X = x;
            Y = y;
            Direction = direction;
        }

        /// <summary>
        /// меняет положение противника в соответствии с направлением
        /// </summary>
        public void Move(Field field)
        {

            Cell currentCell = field[Y, X];

            if (currentCell.Type == CellType.Ground)
            {
                EnemyEaten?.Invoke(this, EventArgs.Empty);
                EnemyResurrect?.Invoke();
                return;
            }

            Delta delta = Delta.ToDeltas(Direction);

            CollisionHandler(field, delta);

            int newX = X + delta.DX;
            int newY = Y + delta.DY;

            Move(field, newX, newY);

            if (field[newY, newX].Type == CellType.Path || field[newY, newX].Characters.FirstOrDefault().Type == CharacterType.Hero)
            {
                HeroLose?.Invoke();
            }
        }

        /// <summary>
        /// меняет вектор движения противника вследствие отскоков
        /// </summary>
        /// <param name="field">игровое поле</param>
        /// <param name="delta">приращение по координатам</param>
        private void CollisionHandler(Field field, Delta delta)
        {
            Cell currentCell = field[Y, X];

            CornerCollisionHandler(field, currentCell, delta);
            BorderCollisionHandler(field, currentCell, delta);
        }

        /// <summary>
        /// меняет направление движения противника вследствие отскока от угла
        /// </summary>
        /// <param name="field">игровое поле</param>
        /// <param name="currentCell">клетка, на которой располагается враг в текущий момент</param>
        /// <param name="delta">приращение по координатам</param>
        /// <returns></returns>
        private void CornerCollisionHandler(Field field, Cell currentCell, Delta delta)
        {
            Cell nextCell = field[Y + delta.DY, X + delta.DX];
            Cell NeighbourCell1 = field[Y, X + delta.DX];
            Cell NeighbourCell2 = field[Y + delta.DY, X];

            if (nextCell.Type == CellType.Ground && NeighbourCell1.Type == NeighbourCell2.Type)
            {
                Delta newDelta = Delta.ToDeltas(Direction);
                newDelta.DX *= -1;
                newDelta.DY *= -1;
                Direction = Delta.ToDirections(newDelta);
                CollisionHandler(field, newDelta);
            }
        }

        /// <summary>
        /// меняет направление движения противника вследствие отскока от стенок поля
        /// </summary>
        /// <param name="field">игровое поле</param>
        /// <param name="currentCell">клетка, на которой располагается враг в текущий момент</param>
        /// <param name="delta">приращение по координатам</param>
        /// <returns></returns>
        private void BorderCollisionHandler(Field field, Cell currentCell, Delta delta)
        {
            Cell NeighbourCellY = field[Y + delta.DY, X];
            Cell NeighbourCellX = field[Y, X + delta.DX];

            if (NeighbourCellY.Type == CellType.Ground && NeighbourCellX.Type != CellType.Ground)
            {
                Delta newDelta = Delta.ToDeltas(Direction);
                newDelta.DY *= -1;
                Direction = Delta.ToDirections(newDelta);
                CollisionHandler(field, newDelta);
            }
            if (NeighbourCellX.Type == CellType.Ground && NeighbourCellY.Type != CellType.Ground)
            {
                Delta newDelta = Delta.ToDeltas(Direction);
                newDelta.DX *= -1;
                Direction = Delta.ToDirections(newDelta);
                CollisionHandler(field, newDelta);
            }
        }

        /// <summary>
        /// событие, сигнализирующее о потере игроком одной жизни
        /// </summary>
        public event Action HeroLose;

        public event EventHandler EnemyEaten;

        public event onEnemyResurrect EnemyResurrect;

        public delegate Task onEnemyResurrect();
    }
}
