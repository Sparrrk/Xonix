using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace FinalXonixWPF.Characters
{
    public enum Direction
    {
        Up,

        Down,

        Left,

        Right,

        UpRight,

        DownRight,
        
        UpLeft,

        DownLeft,

        None
    }

    internal class Delta
    {
        /// <summary>
        /// смещение по оси X
        /// </summary>
        public int DX { get; set; }
        /// <summary>
        /// смещение по оси Y
        /// </summary>
        public int DY { get; set; }

        private Delta(int dY, int dX)
        {
            DX = dX;
            DY = dY;
        }

        static Delta DeltaUp = new Delta(-1, 0);
        static Delta DeltaDown = new Delta(1, 0);
        static Delta DeltaRight = new Delta(0, 1);
        static Delta DeltaLeft = new Delta(0, -1);
        static Delta DeltaUpLeft = new Delta(-1, -1);
        static Delta DeltaUpRight = new Delta(-1, 1);
        static Delta DeltaDownLeft = new Delta(1, -1);
        static Delta DeltaDownRight = new Delta(1, 1);
        static Delta DeltaNone = new Delta(0, 0);

        static public Dictionary<Delta, Direction> FromDeltasToDirections = new Dictionary<Delta, Direction>()
        {
            { DeltaUp, Direction.Up },
            { DeltaDown, Direction.Down },
            { DeltaRight, Direction.Right },
            { DeltaLeft, Direction.Left },
            { DeltaUpLeft, Direction.UpLeft },
            { DeltaUpRight, Direction.UpRight },
            { DeltaDownLeft, Direction.DownLeft },
            { DeltaDownRight, Direction.DownRight },
            { DeltaNone, Direction.None}
        };

        static public Dictionary<Direction, Delta> FromDirectionsToDeltas = new Dictionary<Direction, Delta>()
        {
            { Direction.Up, DeltaUp },
            { Direction.Down, DeltaDown },
            { Direction.Right, DeltaRight },
            { Direction.Left, DeltaLeft },
            { Direction.UpLeft, DeltaUpLeft },
            { Direction.UpRight, DeltaUpRight },
            { Direction.DownLeft, DeltaDownLeft },
            { Direction.DownRight, DeltaDownRight },
            { Direction.None, DeltaNone }
        };

        /// <summary>
        /// вернуть направление, соответствующее параметру-дельте
        /// </summary>
        /// <param name="delta">дельта направления</param>
        /// <returns></returns>
        static public Direction ToDirections(Delta delta)
        {
            return FromDeltasToDirections[delta];
        }

        /// <summary>
        /// вернуть дельту направления, соответствующую параметру-направлению
        /// </summary>
        /// <param name="direction">направление движения игровой сущности</param>
        /// <returns></returns>
        static public Delta ToDeltas(Direction direction)
        {
            return FromDirectionsToDeltas[direction];
        }

        /// <summary>
        /// возвращает true, если направления, соответствующие дельтам, противоположны
        /// </summary>
        /// <param name="delta1"></param>
        /// <param name="delta2"></param>
        /// <returns></returns>
        static public bool IsOpposite(Delta delta1, Delta delta2)
        {
            return delta1.DX == -delta2.DX && delta1.DY == -delta2.DY;
        }
    }
}
