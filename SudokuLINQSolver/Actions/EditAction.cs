using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndoRedoLib;
using SudokuLib;
using System.Windows.Forms;

namespace SudokuLINQSolver.Actions
{
    public class EditAction : BaseSudokuAction
    {
        private bool m_edit;
        private Action<bool> m_edit_func;

        public EditAction(SolutionsTree a_tree, SudokuSolutionNode a_selected, bool a_edit, Action<bool> a_edit_func) 
            : base(a_tree, a_selected)
        {
            m_edit = a_edit;
            m_edit_func = a_edit_func;
        }

        public override void Redo()
        {
            base.Redo();

            m_tree.InitTree(new SudokuBoard(m_tree.SelectedSolutionNode.Board));
            m_edit_func(!m_edit);
        }

        public override void Undo()
        {
            base.Undo();

            m_edit_func(m_edit);
        }


        public override string UndoDescription
        {
            get
            {
                if (!m_edit)
                    return "Exit edit mode";
                else
                    return "Enter edit mode";
            }
        }

        public override string RedoDescription
        {
            get
            {
                if (m_edit)
                    return "Exit edit mode";
                else
                    return "Enter edit mode";
            }
        }
    }
}
