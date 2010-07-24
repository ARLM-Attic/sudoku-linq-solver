using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndoRedoLib;
using SudokuLib;

namespace SudokuLINQSolver.Actions
{
    public class RotateAction : BaseSudokuAction
    {
        private SolutionsCheckedListBox m_list;

        public RotateAction(SolutionsTree a_tree, SolutionsCheckedListBox a_list, SudokuSolutionNode a_selected) 
            : base(a_tree, a_selected)
        {
            m_list = a_list;
        }

        public override void Redo()
        {
            base.Redo();

            SudokuSolution sol = null;

            if (m_list.SelectedSolutionNode != null)
                sol = m_list.SelectedSolutionNode.Solution.Rotate(m_list.SelectedSolutionNode.Board);

            m_tree.InitTree(new SudokuBoard(m_tree.SelectedSolutionNode.Board).Rotate());

            if (sol != null)
                m_list.SelectedItem = m_list.Items.Cast<SudokuSolution>().FirstOrDefault(s => EqualsWithoutStates(sol, s));
        }

        private bool EqualsWithoutStates(SudokuSolution a_solution1, SudokuSolution a_solution2)
        {
            if (a_solution1.Type != a_solution2.Type)
                return false;

            if (!a_solution1.Removed.Exact(a_solution2.Removed, Comparators.SudokuNumberRowColComparer.Instance))
                return false;
            if (!a_solution1.Stayed.Exact(a_solution2.Stayed, Comparators.SudokuNumberRowColComparer.Instance))
                return false;
            if (!a_solution1.Solved.Exact(a_solution2.Solved, Comparators.SudokuNumberRowColComparer.Instance))
                return false;
       
            return a_solution1.ColorUnits.Equals(a_solution2.ColorUnits);
        }

        public override string UndoDescription
        {
            get
            {
                return "Undo rotate";
            }
        }

        public override string RedoDescription
        {
            get
            {
                return "Redo rotate";
            }
        }
    }
}
