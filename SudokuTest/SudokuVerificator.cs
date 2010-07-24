using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SudokuLib;

namespace SudokuTest
{
    public class SudokuVerificator
    {
        private SudokuBoard m_board;
        private List<SudokuSolutionType> m_simples;

        public SudokuVerificator(SudokuBoard a_board)
        {
            m_board = a_board;

            m_simples = new List<SudokuSolutionType>() 
            { 
                SudokuSolutionType.MarkImpossibles, 
                SudokuSolutionType.MarkSolved, 
                SudokuSolutionType.SinglesInUnit 
            };
        }

        public bool IsImpossible(int a_col, int a_row, int a_number_index)
        {
            SudokuBoard board = new SudokuBoard(m_board);
            board[a_col, a_row][a_number_index].State = SudokuNumberState.sudokucellstateSolved;
            SudokuSolutionNode node = SudokuSolutionNode.CreateRoot(board);

            return !IsSolvable(node);
        }

        private bool IsSolvable(SudokuSolutionNode a_node)
        {
            if (a_node.Board.IsSolved)
                return true;

            if (!a_node.Board.IsSolvable)
                return false;

            a_node.StepAll();

            SudokuSolutionNode node = a_node.Nodes.FirstOrDefault();

            if (node == null)
                return true;

            node.StepAll();

            return IsSolvable(node.Nodes.First());
        }
    }
}
