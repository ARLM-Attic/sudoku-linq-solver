using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndoRedoLib;
using SudokuLib;

namespace SudokuLINQSolver.Actions
{
    public class SolveAction : BaseSudokuAction
    {
        public SolveAction(SolutionsTree a_tree, SudokuSolutionNode a_selected) 
            : base(a_tree, a_selected)
        {
        }

        public override void Redo()
        {
            base.Redo();

            if (m_selected == null)
                m_tree.Solve();
            else
                m_tree.Solve(m_selected.Solution);
        }


        public override string UndoDescription
        {
            get 
            {
                return "Undo solving";
            }
        }

        public override string RedoDescription
        {
            get 
            {
                return "Redo solving";
            }
        }
    }
}
