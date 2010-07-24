using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuLib
{
    public class SudokuOptions
    {
        public bool ShowAllSolutions = false;
        public bool IncludeBoxes = false;

        public static SudokuOptions Current = new SudokuOptions();

        public override string ToString()
        {
            return String.Format("Show all solutions: {0}, Include boxes: {1}", ShowAllSolutions, IncludeBoxes);
        }

    }
}
