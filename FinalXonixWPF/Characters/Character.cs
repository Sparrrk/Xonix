using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FinalXonixWPF.GameField;
using FinalXonixWPF.Generators;

namespace FinalXonixWPF.Characters
{
    internal class Character
    {
        /// <summary>
        /// конструктор создания игровой сущности
        /// </summary>
        /// <param name="x">координата x</param>
        /// <param name="y">координата y</param>
        /// <param name="type">тип игровой сущности</param>
        /// <param name="direction">направление движения</param>
        public Character(int x, int y, CharacterType type, Direction direction)
        {
            X = x;
            Y = y;
            Type = type;
            Direction = direction;
        }


        /// <summary>
        /// координата X игровой сущности
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// координата Y игровой сущности
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// тип игровой сущности
        /// </summary>
        public CharacterType Type { get; set; }
        /// <summary>
        /// направление движения
        /// </summary>
        public Direction Direction { get; set; }

        public Rectangle rectangle;
        /// <summary>
        /// переместить игровую сущность в соответсвии с направлением
        /// </summary>
        /// <param name="field"></param>
        public void Move(Field field, int x, int y)
        {
            field[Y, X].RemoveCharacter(this);
            field[y, x].AddCharacter(this);

            if (this.Type == CharacterType.Hero && field[Y, X].Type == CellType.Sea)
            {
                field[Y, X].Type = CellType.Path;
                field[Y, X].MemorizedDirection = Direction;
            }

            if (this.Type == CharacterType.Hero && field[Y, X].Type == CellType.Path && field[y, x].Type == CellType.Ground)
            {
                RegionIsCaptured?.Invoke();
                Direction = Direction.None;
            }

            X = x;
            Y = y;
        }

        public void PrintCharacter(Canvas canvas, Field field)
        {
            int pixelSize = (int)(canvas.Width / field.Width);
            if (rectangle != null)
                canvas.Children.Remove(rectangle);
            if (Type == CharacterType.Hero)
            {
                if (field[Y, X].rectangle != null)
                {
                    canvas.Children.Remove(field[Y, X].rectangle);
                }
                rectangle = new Rectangle { Fill = Brushes.Red, Stroke = Brushes.Red };
                
            }
            else if (Type == CharacterType.SeaEnemy || Type == CharacterType.GroundEnemy)
            {
                if (field[Y, X].rectangle != null)
                {
                    canvas.Children.Remove(field[Y, X].rectangle);
                }
                rectangle = new Rectangle { Fill = Brushes.Olive, Stroke = Brushes.Olive };
            }
            Canvas.SetTop(rectangle, Y * pixelSize);
            Canvas.SetLeft(rectangle, X * pixelSize);
            canvas.Children.Add(rectangle);
        }

        /// <summary>
        /// событие, сигнализирующее о захвате игроком определенной территории
        /// </summary>
        public event Action RegionIsCaptured;
    }
}
