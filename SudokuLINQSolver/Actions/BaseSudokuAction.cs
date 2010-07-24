using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndoRedoLib;
using SudokuLib;

namespace SudokuLINQSolver.Actions
{
    public abstract class BaseSudokuAction : UndoRedoAction
    {
        protected SolutionsTree m_tree;
        protected SolutionsTreeState m_tree_state;
        protected SudokuSolutionNode m_selected;
        protected SudokuSolutionNode m_selected_tree;

        public BaseSudokuAction(SolutionsTree a_tree, SudokuSolutionNode a_selected)
        {
            m_selected = a_selected;
            m_tree_state = new SolutionsTreeState(a_tree);
            m_tree = a_tree;
            m_selected_tree = a_tree.SelectedSolutionNode;
        }

        public override void Undo()
        {
            m_tree.RestoreTreeState(m_tree_state);
        }

        public override void Redo()
        {
            m_tree.SelectedSolutionNode = m_selected_tree;
        }
    }
}
