using FinalXonixWPF.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace FinalXonixWPF.GameField
{
    internal class Cell
    {
        /// <summary>
        /// конструктор создания игровой ячейки
        /// </summary>
        /// <param name="x">координата x</param>
        /// <param name="y">координата y</param>
        /// <param name="type">тип игровой ячейки</param>
        public Cell(int y, int x, CellType type)
        {
            X = x;
            Y = y;
            _type = type;
        }
        /// <summary>
        /// коордиата x игровой ячейки
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// координата y игровой ячейки
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// тип игровой ячейки
        /// </summary>
        private CellType _type;
        /// <summary>
        /// тип игровой ячейки
        /// </summary>
        public CellType Type {
            get 
            {
                return _type; 
            }
            set
            {
                _type = value;
                CellTypeChanged?.Invoke();
            }
            }
        /// <summary>
        /// запоминает направление, в котором игрок покинул данную клетку
        /// </summary>
        public Direction MemorizedDirection { get; set; }
        /// <summary>
        /// булевая переменная, указывающая на необходимость перерисовки ячейки
        /// </summary>
        public bool NeedRedraw = true;
        public int RegionNumber = 0;
        public bool IsCaptured = false;

        public List<Character> Characters = new List<Character>();
        /// <summary>
        /// добавить выбранную игровую сущность в список персонажей, находящихся на этой клетке
        /// </summary>
        /// <param name="character"></param>
        public void AddCharacter(Character character)
        {
            Characters.Add(character);
            NeedRedraw = true;
        }

        /// <summary>
        /// удалить выбранную игровую сущность из списка персонажей, находящихся на этой клетке
        /// </summary>
        /// <param name="character"></param>
        public void RemoveCharacter(Character character)
        {
            Characters.Remove(character);
            NeedRedraw = true;
        }

        /// <summary>
        /// событие изменения списка игровых сущностей
        /// </summary>
        //private event Action CharacterLeave;

        /// <summary>
        /// событие изменения типа игровой ячейки
        /// </summary>
        private event Action CellTypeChanged;

        public Rectangle rectangle = new Rectangle();
    }
}
