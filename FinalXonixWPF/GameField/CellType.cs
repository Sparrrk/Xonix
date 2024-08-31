using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalXonixWPF.GameField
{
    /// <summary>
    /// тип игровой ячейки
    /// </summary>
    public enum CellType
    {
        /// <summary>
        /// морская ячейка
        /// </summary>
        Sea,
        /// <summary>
        /// суша
        /// </summary>
        Ground,
        /// <summary>
        /// путь
        /// </summary>
        Path
    }
}
