using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SudokuLib;

namespace SudokuLINQSolver
{
    public class SolutionsTreeState
    {
        private List<SudokuSolutionNode> m_nodes = new List<SudokuSolutionNode>();

        public SolutionsTreeState(SolutionsTree a_tree)
        {
            for (int i = 0; i < a_tree.Nodes.Count; i++)
                m_nodes.Add(a_tree.TreeNodeToSolutionNode(a_tree.Nodes[i]));
        }

        public IEnumerable<SudokuSolutionNode> Nodes
        {
            get
            {
                return m_nodes;
            }
        }
    }
}
