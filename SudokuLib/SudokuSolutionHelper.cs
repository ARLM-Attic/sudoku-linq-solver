using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SudokuLib
{
    public abstract class SudokuSolutionHelper
    {
        public static Color REMOVED_COLOR = Color.OrangeRed;
        public static Color SOLVED_COLOR = Color.DeepSkyBlue;
        public static Color HELPER_COLOR = Color.LightGreen;
    }
}
