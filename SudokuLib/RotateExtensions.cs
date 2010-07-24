using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SudokuLib;

namespace SudokuLib
{
    public static class RotateExtensions
    {
        public static SudokuBoard Rotate(this SudokuBoard a_board, int a_count)
        {
            SudokuBoard board = a_board;
            for (int i = 0; i < a_count; i++)
                board = Rotate(board);

            return board;
        }

        public static SudokuBoard Rotate(this SudokuBoard a_board)
        {
            SudokuBoard board = new SudokuBoard();

            for (int row = 0; row < SudokuBoard.SIZE; row++)
            {
                for (int col = 0; col < SudokuBoard.SIZE; col++)
                {
                    int col_r = col;
                    int row_r = row;
                    Rotate(ref col_r, ref row_r);

                    for (int i = 0; i < SudokuBoard.SIZE; i++)
                    {
                        board[col_r, row_r][i].State = a_board[col, row][i].State;
                    }
                }
            }

            return board;
        }

        public static SudokuIntermediateSolution Rotate(this SudokuIntermediateSolution a_inter_sol)
        {
            SudokuBoard before = Rotate(a_inter_sol.Before);
            return new SudokuIntermediateSolution(before, Rotate(a_inter_sol.After), Rotate(a_inter_sol.Solution, before));
        }

        private static void Rotate(ref int a_col, ref int a_row)
        {
            int col = a_col;
            a_col = SudokuBoard.SIZE - a_row - 1;
            a_row = col;
        }

        public static SudokuSolution Rotate(this SudokuSolution a_sol, SudokuBoard a_board)
        {
            return new SudokuSolution(a_sol.Type, Rotate(a_sol.Removed, a_board), Rotate(a_sol.Stayed, a_board),
                Rotate(a_sol.Solved, a_board), from unit in a_sol.ColorUnits
                                           select Rotate(unit, a_board));
        }

        private static IEnumerable<SudokuCell> Rotate(IEnumerable<SudokuCell> a_list, SudokuBoard a_board)
        {
            var list = from cell in a_list
                       select GetRotatedCell(a_board, cell);
            return from cell in list
                   orderby cell.Row, cell.Col
                   select cell;
        }

        private static IEnumerable<SudokuNumber> Rotate(IEnumerable<SudokuNumber> a_list, SudokuBoard a_board)
        {
            var list = from num in a_list
                       select Rotate(num, a_board);
            return from num in list
                   orderby num.Row, num.Col
                   select num;
        }

        private static SudokuNumber Rotate(SudokuNumber a_number, SudokuBoard a_board)
        {
            int col_r = a_number.Col;
            int row_r = a_number.Row;
            Rotate(ref col_r, ref row_r);

            return a_board[col_r, row_r][a_number.Number - 1];
        }

        private static SudokuCell GetRotatedCell(SudokuBoard a_board, SudokuCell a_cell)
        {
            int col = a_cell.Col;
            int row = a_cell.Row;
            Rotate(ref col, ref row);
            return a_board[col, row];
        }
    }
}
