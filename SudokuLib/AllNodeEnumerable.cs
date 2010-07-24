using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuLib
{
    internal class AllNodesEnumerable : IEnumerable<SudokuSolutionNode>
    {
        private SudokuSolutionNode m_node;

        public AllNodesEnumerable(SudokuSolutionNode a_node)
        {
            m_node = a_node;
        }

        public IEnumerator<SudokuSolutionNode> GetEnumerator()
        {
            return new AllNodesEnumerator(m_node);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new AllNodesEnumerator(m_node);
        }
    }

}
