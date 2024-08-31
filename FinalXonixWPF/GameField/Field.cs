using FinalXonixWPF.Characters;
using FinalXonixWPF.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FinalXonixWPF.GameField
{
    internal class Field
    {
        /// <summary>
        /// ширина игрового поля
        /// </summary>
        private int _width = 40;
        /// <summary>
        /// высота игрового поля
        /// </summary>
        private int _height = 40;

        public int Width { get { return _width; } set { _width = value; } }
        public int Height { get { return _height; } set { _height = value; } }

        public Field()
        {
            CreateDefaultField();
        }

        public Field(int height, int width)
        {
            _height = height;
            _width = width;
            CreateDefaultField();
        }

        /// <summary>
        /// игровое поле, представляющее собой двумерный массив клеток Cell
        /// </summary>
        private Cell[,] GameField { get; set; }

        /// <summary>
        /// Сгенерировать стандартное игровое поле без игровых сущностей
        /// </summary>
        private void CreateDefaultField()
        {
            Cell[,] field = new Cell[Height, Width];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (i <= 1 || i >= Height - 2 || j <= 1 || j >= Width - 2)
                    {
                        field[i, j] = new Cell(i, j, CellType.Ground);
                    }
                    else
                        field[i, j] = new Cell(i, j, CellType.Sea);
                }
            }
            GameField = field;
        }

        public void PrintField(LevelGenerator levelGenerator, Canvas canvas)
        {
            int pixelSize = (int)canvas.Width / Width;
            for (int i = 0; i < levelGenerator.Field.Height; i++)
            {
                for (int j = 0; j < levelGenerator.Field.Width; j++)
                {
                    if (levelGenerator.Field[i, j].IsCaptured == true && levelGenerator.Field[i, j].Characters.Count == 0)
                    {
                        canvas.Children.Remove(levelGenerator.Field[i, j].rectangle);
                    }

                    //if (levelGenerator.Field[i, j].NeedRedraw == true || levelGenerator.Field[i, j].Characters.Count != 0)
                    //{
                    if (levelGenerator.Field[i, j].NeedRedraw == true || levelGenerator.Field[i, j].Characters.Count != 0)
                        DrawCell(i, j, levelGenerator, canvas, pixelSize);
                    //}
                    //}
                }
            }
            //levelGenerator.PrintCharacters(canvas);
        }

        private void DrawCell(int y, int x, LevelGenerator levelGenerator, Canvas canvas, int pixelSize)
        {
            Cell cell = levelGenerator.Field[y, x];
            if (levelGenerator.Field[y, x].rectangle != null)
            {
                canvas.Children.Remove(levelGenerator.Field[y, x].rectangle);
            }
            else 
            {
                levelGenerator.Field[y, x].rectangle = new Rectangle();
            }
                SolidColorBrush brush = SelectCellColor(y, x);
                cell.rectangle.Fill = brush;
                cell.rectangle.Stroke = brush;
                cell.rectangle.Width = pixelSize;
                cell.rectangle.Height = pixelSize;
                Canvas.SetLeft(cell.rectangle, x * pixelSize);
                Canvas.SetTop(cell.rectangle, y * pixelSize);
                canvas.Children.Add(cell.rectangle);
                cell.NeedRedraw = false;
        }

        private SolidColorBrush SelectCellColor(int y, int x)
        {
            SolidColorBrush scb = new SolidColorBrush() ;
            if (GameField[y, x].Characters.Count != 0)
            {
                if (GameField[y, x].Characters.FirstOrDefault().Type == CharacterType.Hero)
                    scb.Color = Colors.Blue;
                else if (GameField[y, x].Characters.FirstOrDefault().Type == CharacterType.SeaEnemy)
                    scb.Color = Colors.Red;
                else if (GameField[y, x].Characters.FirstOrDefault().Type == CharacterType.GroundEnemy)
                    scb.Color = Colors.Red;
            }
            else
            {
                switch (GameField[y, x].Type)
                {
                    case CellType.Sea:
                        scb.Color = Colors.MistyRose;

                        break;

                    case CellType.Ground:
                        scb.Color = Colors.Black;
                        break;

                    case CellType.Path:
                        scb.Color = Colors.Purple;
                        break;
                }
            }
            return scb;
        }

        /// <summary>
        /// функция заполнения региона при захвате игроком определенной территории
        /// </summary>
        public void RegionCapturedHandler()
        {
            for (int i = 0; i < GameField.GetLength(0); i++)
            {
                for (int j = 0; j < GameField.GetLength(1); j++)
                {
                    if (GameField[i, j].Type == CellType.Path)
                    {
                        int newX1 = GameField[i, j].X + Delta.ToDeltas(GameField[i, j].MemorizedDirection).DY;
                        int newY1 = GameField[i, j].Y - Delta.ToDeltas(GameField[i, j].MemorizedDirection).DX;
                        int newX2 = GameField[i, j].X - Delta.ToDeltas(GameField[i, j].MemorizedDirection).DY;
                        int newY2 = GameField[i, j].Y + Delta.ToDeltas(GameField[i, j].MemorizedDirection).DX;
                        SendTheWave(newX1, newY1, 1);
                        SendTheWave(newX2, newY2, 2);
                    }
                }
            }

            int filler = RegionSizeCounter();
            RegionCapturedFiller(filler);
            RegionNumberCleaner();

        }

        /// <summary>
        /// пускает цифровую волну для определения наименьшего из двух регионов. Для этого отмечает все морские клетки одного региона
        /// единицей, второго - двойкой. 
        /// </summary>
        private void SendTheWave(int x, int y, int RegionNumber)
        {
            if (x < 0 || y < 0 || x >= GameField.GetLength(1) || y >= GameField.GetLength(0))
            {
                return;
            }
            else if (GameField[y, x].Type != CellType.Sea || GameField[y, x].RegionNumber != 0)
            {
                return;
            }
            else
            {
                GameField[y, x].RegionNumber = RegionNumber;
                for (int i = -1; i < 2; i += 2)
                {
                    for (int j = -1; j < 2; j += 2)
                        SendTheWave(x + j, y + i, RegionNumber);
                }
            }
        }

        /// <summary>
        /// подсчитывает количество клеток для двух регионов при захвате игроком территории
        /// </summary>
        /// <returns>наименьший регион, который необходимо залить</returns>
        private int RegionSizeCounter()
        {
            int counter1 = 0;
            int counter2 = 0;
            for (int i = 1; i < GameField.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < GameField.GetLength(1) - 1; j++)
                {
                    if (GameField[i, j].RegionNumber == 1)
                        counter1++;
                    else if (GameField[i, j].RegionNumber == 2)
                        counter2++;
                }
            }
            return counter1 <= counter2 ? 1 : 2;
        }

        private void RegionCapturedFiller(int filler)
        {
            for (int i = 1; i < GameField.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < GameField.GetLength(1) - 1; j++)
                {
                    if (GameField[i, j].RegionNumber == filler || GameField[i, j].Type == CellType.Path)
                    {
                        GameField[i, j].Type = CellType.Ground;
                        GameField[i, j].IsCaptured = true;
                        //GameField[i, j].NeedRedraw = true;
                    }
                }
            }
        }

        private void RegionNumberCleaner()
        {
            for (int i = 0; i < GameField.GetLength(0); i++)
            {
                for (int j = 0; j < GameField.GetLength(1); j++)
                {
                    GameField[i, j].RegionNumber = 0;
                }
            }
        }

        public void PathCleaner()
        {
            for (int i = 0; i < GameField.GetLength(0); i++)
            {
                for (int j = 0; j < GameField.GetLength(1); j++)
                {
                    if (GameField[i, j].Type == CellType.Path)
                    {
                        GameField[i, j].Type = CellType.Sea;
                        GameField[i, j].NeedRedraw = true;
                    }
                }
            }
        }

        /// <summary>
        /// индексатор класса игрового поля
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public Cell this[int i, int j]
        {
            get { return GameField[i, j]; }
            set { GameField[i, j] = value; }
        }


    }
}
