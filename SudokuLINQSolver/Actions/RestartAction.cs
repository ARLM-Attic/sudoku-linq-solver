using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndoRedoLib;
using SudokuLib;

namespace SudokuLINQSolver.Actions
{
    public class RestartAction : BaseSudokuAction
    {
        public RestartAction(SolutionsTree a_tree, SudokuSolutionNode a_selected) 
            : base(a_tree, a_selected)
        {
        }

        public override void Redo()
        {
            base.Redo();

            m_tree.ResetBoard();
        }

        public override string UndoDescription
        {
            get
            {
                return "Undo restart";
            }
        }

        public override string RedoDescription
        {
            get
            {
                return "Redo restart";
            }
        }
    }
}
