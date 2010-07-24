using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuLib
{
    internal class AllNodesEnumerator : IEnumerator<SudokuSolutionNode>
    {
        private SudokuSolutionNode m_node;
        private SudokuSolutionNode m_current;
        private bool m_ended;

        public AllNodesEnumerator(SudokuSolutionNode a_node)
        {
            m_node = a_node;
        }

        public SudokuSolutionNode Current
        {
            get
            {
                if (m_ended)
                    throw new InvalidOperationException();

                return m_current;
            }
        }

        public void Dispose()
        {
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                if (m_ended)
                    throw new InvalidOperationException();

                return m_current;
            }
        }

        public bool MoveNext()
        {
            if (m_ended)
                throw new InvalidOperationException();
            else if (m_current == null)
            {
                m_current = m_node;
                return true;
            }
            else
            {
                if (m_current.Nodes.Count() > 0)
                {
                    m_current = m_current.Nodes.First();
                    return true;
                }
                else
                {
                    SudokuSolutionNode node = m_current;
                    m_current = null;

                    while (node.Parent != null)
                    {
                        if (Object.ReferenceEquals(node.Parent.Nodes.Last(), node))
                            node = node.Parent;
                        else
                        {
                            m_current = node.Parent.Nodes.SkipWhile(n => !Object.ReferenceEquals(node, n)).Skip(1).FirstOrDefault();

                            if (m_current == null)
                                node = node.Parent;
                            else
                                break;
                        }
                    }

                    if (m_current == null)
                    {
                        m_ended = true;
                        return false;
                    }
                    else
                        return true;
                }
            }
        }

        public void Reset()
        {
            m_ended = false;
            m_current = null;
        }
    }

}
