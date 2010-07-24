using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuLib
{
    public static class Comparators
    {
        public class SudokuCellColRowComparer : IEqualityComparer<SudokuCell>
        {
            public static SudokuCellColRowComparer Instance = new SudokuCellColRowComparer();

            public bool Equals(SudokuCell a_cell1, SudokuCell a_cell2)
            {
                return (a_cell1.Col == a_cell2.Col) && (a_cell1.Row == a_cell2.Row);
            }

            public int GetHashCode(SudokuCell a_num)
            {
                return a_num.GetHashCode();
            }
        }

        public class SudokuNumberRowColComparer : IEqualityComparer<SudokuNumber>
        {
            public static SudokuNumberRowColComparer Instance = new SudokuNumberRowColComparer();

            public bool Equals(SudokuNumber a_num1, SudokuNumber a_num2)
            {
                return (a_num1.Number == a_num2.Number) && (a_num1.Col == a_num2.Col) && (a_num1.Row == a_num2.Row);
            }

            public int GetHashCode(SudokuNumber a_num)
            {
                return a_num.GetHashCode();
            }
        }

        public class SudokuNumberComparer : IEqualityComparer<SudokuNumber>
        {
            public static SudokuNumberComparer Instance = new SudokuNumberComparer();

            public bool Equals(SudokuNumber a_x, SudokuNumber a_y)
            {
                return a_x.Number == a_y.Number;
            }

            public int GetHashCode(SudokuNumber a_obj)
            {
                return a_obj.Number;
            }
        }
    }
}
