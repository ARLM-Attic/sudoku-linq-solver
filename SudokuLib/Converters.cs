using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SudokuLib
{
    public static class Converters
    {
        public static string SudokuNumberStateToString(SudokuNumberState a_state)
        {
            if (a_state == SudokuNumberState.sudokucellstateImpossible)
                return "Impossible";
            else if (a_state == SudokuNumberState.sudokucellstateManualEntered)
                return "ManualEntered";
            else if (a_state == SudokuNumberState.sudokucellstatePossible)
                return "Possible";
            else if (a_state == SudokuNumberState.sudokucellstateSolved)
                return "Solved";
            else
                throw new Exception();
        }

        public static SudokuNumberState StringToSudokuNumberState(string a_state)
        {
            if (a_state.Equals("Impossible", StringComparison.CurrentCultureIgnoreCase))
                return SudokuNumberState.sudokucellstateImpossible;
            else if (a_state.Equals("ManualEntered", StringComparison.CurrentCultureIgnoreCase))
                return SudokuNumberState.sudokucellstateManualEntered;
            else if (a_state.Equals("Possible", StringComparison.CurrentCultureIgnoreCase))
                return SudokuNumberState.sudokucellstatePossible;
            else if (a_state.Equals("Solved", StringComparison.CurrentCultureIgnoreCase))
                return SudokuNumberState.sudokucellstateSolved;
            else
                throw new Exception();
        }

        public static string SudokuSolutionTypeToString(SudokuSolutionType a_type)
        {
            if (a_type == SudokuSolutionType.BoxLineReduction)
                return "Box-line reduction";
            else if (a_type == SudokuSolutionType.HiddenPair)
                return "Hidden pair";
            else if (a_type == SudokuSolutionType.HiddenQuad)
                return "Hidden quad";
            else if (a_type == SudokuSolutionType.HiddenTriple)
                return "Hidden triple";
            else if (a_type == SudokuSolutionType.MarkImpossibles)
                return "Mark impossibles";
            else if (a_type == SudokuSolutionType.MarkSolved)
                return "Mark solved";
            else if (a_type == SudokuSolutionType.NakedPair)
                return "Naked pair";
            else if (a_type == SudokuSolutionType.NakedQuad)
                return "Naked quad";
            else if (a_type == SudokuSolutionType.NakedTriple)
                return "Naked triple";
            else if (a_type == SudokuSolutionType.PointingPair)
                return "Pointing pair";
            else if (a_type == SudokuSolutionType.PointingTriple)
                return "Pointing triple";
            else if (a_type == SudokuSolutionType.SinglesInUnit)
                return "Singles in unit";
            else if (a_type == SudokuSolutionType.SwordFish)
                return "Sword-fish";
            else if (a_type == SudokuSolutionType.JellyFish)
                return "Jelly-fish";
            else if (a_type == SudokuSolutionType.XWing)
                return "X-Wing";
            else if (a_type == SudokuSolutionType.XYZWing)
                return "XYZ-Wing";
            else if (a_type == SudokuSolutionType.WXYZWing)
                return "WXYZ-Wing";
            else if (a_type == SudokuSolutionType.YWing)
                return "Y-Wing";
            else if (a_type == SudokuSolutionType.MultivalueXWing)
                return "Multivalue X-wing";
            else
                throw new NotImplementedException();
        }

        public static Point CoordToPoint(string a_coord)
        {
            char[] c = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I' };

            char row_char = a_coord.Substring(0, 1).ToUpper()[0];

            int row_index = -1;
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == row_char)
                {
                    row_index = i;
                    break;
                }
            }

            return new Point(Int32.Parse(a_coord.Substring(1, 1).ToLower()) - 1, row_index);
        }

        public static string PointToCoords(int a_col, int a_row)
        {
            char[] c = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I' };
            return c[a_row] + (a_col + 1).ToString();
        }
    }
}
