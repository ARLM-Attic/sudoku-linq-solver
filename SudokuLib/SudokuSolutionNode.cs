using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Xml.Linq;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace SudokuLib
{
    public class SudokuSolutionNode
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SudokuBoard m_board;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SudokuSolution m_solution;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<SudokuSolutionNode> m_nodes = new List<SudokuSolutionNode>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SudokuSolutionNode m_parent;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SudokuBoard m_nextBoard;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SudokuSolutionNodeStepMode m_step_mode = SudokuSolutionNodeStepMode.StepNone;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SudokuSolutionNodeState m_state;

        private static SudokuSolverBase s_solver;

        static SudokuSolutionNode()
        {
            s_solver = new SudokuSolverLINQ();
        }

        internal SudokuSolutionNode(SudokuBoard a_board, SudokuSolutionNodeState a_state, SudokuSolution a_solution = null)
        {
            m_state = a_state;
            m_board = a_board;
            m_board.BoardChanged += num => m_nextBoard = null;
            m_solution = a_solution;
        }

        internal SudokuSolutionNodeStepMode StepMode
        {
            get
            {
                return m_step_mode;
            }
            set
            {
                m_step_mode = value;
            }
        }

        public SudokuSolutionNode Root
        {
            get
            {
                if (Parent != null)
                    return Parent.Root;
                else
                    return this;
            }
        }

        public IEnumerable<SudokuSolutionNode> GetAllNodesEnumerator()
        {
            return new AllNodesEnumerable(this);
        }

        public SudokuSolutionNodeState State
        {
            get
            {
                return m_state;
            }
            internal set
            {
                m_state = value;
            }
        }

        public override bool Equals(object a_obj)
        {
            if (a_obj == null)
                return false;
            if (ReferenceEquals(this, a_obj))
                return true;
            SudokuSolutionNode node = a_obj as SudokuSolutionNode;
            if (node == null)
                return false;

            if (m_state != node.m_state)
                return false;

            if (!m_board.Equals(node.m_board))
                return false;

            if ((m_solution == null) && (node.m_solution == null))
                return true;

            if ((m_solution == null) ^ (node.m_solution == null))
                return false;

            if (m_solution.Equals(node.m_solution))
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return m_board.GetHashCode() ^ m_state.GetHashCode() ^ ((m_solution != null) ? m_solution.GetHashCode() : 0);
        }

        public string Info
        {
            get
            {
                if (m_state == SudokuSolutionNodeState.State)
                {
                    if (m_parent == null)
                        return "Start";
                    else
                        return "State";
                }
                else if (m_state == SudokuSolutionNodeState.Solution)
                    return m_solution.Info;
                else if (m_state == SudokuSolutionNodeState.Solved)
                    return "Solved";
                else if (m_state == SudokuSolutionNodeState.Unsolved)
                    return "Can't find solution";
                else if (m_state == SudokuSolutionNodeState.Unsolvable)
                    return "Unsolvable";
                else
                    throw new Exception();
            }
        }

        public override string ToString()
        {
            if (m_state == SudokuSolutionNodeState.Solution)
                return String.Format("Board={{ {0} }}; Info={1}; Solution={{ {2} }}; Count={3}", m_board, Info, m_solution, m_nodes.Count);
            else
                return String.Format("Board={{ {0} }}; Info={1}; Count={2}", m_board, Info, m_nodes.Count);
        }

        public SudokuSolution Solution
        {
            get 
            {
                return m_solution;
            }
        }

        internal bool CanSolve
        {
            get
            {
                return (m_state != SudokuSolutionNodeState.Solved) &&
                       (m_state != SudokuSolutionNodeState.Unsolved) &&
                       (m_state != SudokuSolutionNodeState.Unsolvable);
            }
        }

        public SudokuBoard NextBoard
        {
            get
            {
                if (m_nextBoard == null)
                {
                    m_nextBoard = new SudokuBoard(m_board);
                    if (m_solution != null)
                        m_nextBoard.Apply(m_solution);
                }

                return m_nextBoard;
            }
        }

        public SudokuBoard Board
        {
            get
            {
                return m_board;
            }
        }

        public SudokuSolutionNode Parent
        {
            get
            {
                return m_parent;
            }
        }

        public IEnumerable<SudokuSolutionNode> Nodes
        {
            get
            {
                return m_nodes;
            }
        }

        internal SudokuSolutionNode AddNode(SudokuBoard a_board, SudokuSolutionNodeState a_state)
        {
            return AddNode(a_board, a_state, null);
        }

        internal SudokuSolutionNode AddNode(SudokuBoard a_board, SudokuSolutionNodeState a_state, SudokuSolution a_solution)
        {
            return AddNode(new SudokuSolutionNode(a_board, a_state, a_solution));
        }

        internal SudokuSolutionNode AddNode(SudokuSolutionNode a_node)
        {
            a_node.m_parent = this;
            m_nodes.Add(a_node);
            return a_node;
        }

        public SudokuSolutionNode Solve()
        {
            return s_solver.Solve(this);
        }

        public void StepAll()
        {
            s_solver.StepAll(this);
        }

        public static SudokuSolutionNode CreateRoot(SudokuBoard a_board)
        {
            return new SudokuSolutionNode(a_board, SudokuSolutionNodeState.State);
        }

        public SudokuSolutionNode Step()
        {
            return s_solver.Step(this);
        }

        public SudokuSolutionNode Step(SudokuSolutionType a_type)
        {
            return s_solver.Step(this, a_type);
        }

        public object[] GetAsXML()
        {
            return new object[] 
            {
                new XAttribute("state", m_state), 
                new XElement("board", m_board.GetAsXML()), 
                new XElement("solution", (m_solution != null) ? m_solution.GetAsXML() : null), 
                new XElement("solution_nodes", from node in m_nodes
                                               select new XElement("solution_node", node.GetAsXML()))
            };
        }

        private static void LoadNodesFromXML(XElement a_element, SudokuSolutionNode a_parent)
        {
            a_element.Element("solution_nodes").Elements().ForEach(child_element =>
            { 
                SudokuBoard board = SudokuBoard.LoadFromXML(child_element.Element("board"));
                SudokuSolutionNode child_node = a_parent.AddNode(
                    board,
                    (SudokuSolutionNodeState)Enum.Parse(typeof(SudokuSolutionNodeState), child_element.Attribute("state").Value),
                    SudokuSolution.LoadFromXML(child_element.Element("solution"), board)
                );

                LoadNodesFromXML(child_element, child_node);
            });
        }

        private static SudokuSolutionNode LoadFromXML(XElement a_element)
        {
            try
            {
                SudokuSolutionNode node = SudokuSolutionNode.CreateRoot(SudokuBoard.LoadFromXML(a_element.Element("board")));

                Debug.Assert(a_element.Element("solution").IsEmpty);

                LoadNodesFromXML(a_element, node);

                return node;
            }
            catch
            {
                return null;
            }
        }

        public static SudokuSolutionNode LoadFromFile(string a_fileName)
        {
            try
            {
                using (FileStream file_stream = new FileStream(a_fileName, FileMode.Open, FileAccess.Read))
                {
                    using (GZipStream gzip_stream = new GZipStream(file_stream, CompressionMode.Decompress))
                    {
                        using (StreamReader stream_reader = new StreamReader(gzip_stream, Encoding.ASCII))
                        {
                            return SudokuSolutionNode.LoadFromXML(XElement.Load(stream_reader));
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public void SaveToFile(string a_fileName)
        {
            using (FileStream file_stream = new FileStream(a_fileName, FileMode.Create, FileAccess.ReadWrite))
            {
                using (GZipStream gzip_stream = new GZipStream(file_stream, CompressionMode.Compress))
                {
                    using (XmlTextWriter xml_test_writer = new XmlTextWriter(gzip_stream, Encoding.ASCII)) 
                    {
                        new XElement("solution_node", GetAsXML()).WriteTo(xml_test_writer);
                    };
                }
            }
        }

        public SudokuSolutionNode SolveWithStepAll()
        {
            StepAll();

            if (m_nodes.Count > 0)
                return m_nodes[0].SolveWithStepAll();
            else
                return this;
        }

        public void RemoveNodes()
        {
            m_nodes.Clear();
            m_step_mode = SudokuSolutionNodeStepMode.StepNone;
        }

        public void RemoveNodes(SudokuSolutionNode a_except)
        {
            if (!m_nodes.Contains(a_except))
                a_except = null;
            
            m_nodes.Clear();

            if (a_except != null)
                m_nodes.Add(a_except);

            m_step_mode = SudokuSolutionNodeStepMode.StepSelectedAlgorithm;
        }

        public static SudokuSolutionNode CreateTree(SudokuSolutionNode a_root)
        {
            SudokuSolutionNode root = CreateRoot(a_root.Board);

            root.RecreateTree(a_root);

            return root;
        }

        private void RecreateTree(SudokuSolutionNode a_node)
        {
            if (a_node.State == SudokuSolutionNodeState.Solution)
            {
                Step();
                if (a_node.Nodes.Any())
                    Nodes.First().RecreateTree(a_node.Nodes.First());
            }
            else if ((a_node.State == SudokuSolutionNodeState.Solved) || 
                     (a_node.State == SudokuSolutionNodeState.Unsolvable) ||
                     (a_node.State == SudokuSolutionNodeState.Unsolved))
            {            }
            else if (a_node.State == SudokuSolutionNodeState.State)
            {
                if (a_node.StepMode == SudokuSolutionNodeStepMode.StepAllAlgorithms)
                    StepAll();
                else if (a_node.StepMode == SudokuSolutionNodeStepMode.StepFirstSolution)
                    Step();
                else if (a_node.StepMode == SudokuSolutionNodeStepMode.StepSelectedAlgorithm)
                    Step(a_node.Nodes.First().Solution.Type);
                else if (a_node.StepMode == SudokuSolutionNodeStepMode.StepNone)
                    return;

                SudokuSolutionNode sol1 = a_node.Nodes.FirstOrDefault(n => n.Nodes.Any());

                SudokuSolutionNode sol2 = null;
                if (sol1 != null)
                    sol2 = Nodes.FirstOrDefault(n => n.Solution.Equals(sol1.Solution));

                if ((sol1 != null) && (sol2 != null))
                    sol2.RecreateTree(sol1);
            }
        }

        public static bool IsInclude(SudokuSolution a_stayed, SudokuSolution a_toremoved, bool a_use_includes_list)
        { 
            if (Object.ReferenceEquals(a_stayed, a_toremoved))
                return false;

            if (a_use_includes_list)
            {
                if (!SudokuSolution.Includes.Any(pair => (a_toremoved.Type == pair.First()) && (a_stayed.Type == pair.Last())))
                    return false;  
            }

            if (a_stayed.Type == a_toremoved.Type)
            {
                if (!a_stayed.ColorUnits.Equals(a_toremoved.ColorUnits))
                    return false;
            }

            return a_stayed.Removed.Contains(a_toremoved.Removed) &&
                   a_toremoved.Stayed.Contains(a_stayed.Stayed) &&
                   !a_stayed.Removed.ContainsAny(a_toremoved.Stayed) &&
                   !a_stayed.Stayed.ContainsAny(a_toremoved.Removed);
        }

        public static List<SudokuSolution> FilterByOptions(List<SudokuSolution> a_list)
        {
            var list = new List<SudokuSolution>(a_list);

            if (!SudokuOptions.Current.ShowAllSolutions)
            {
                var includes = from stayed in list
                               from toremoved in list
                               where IsInclude(stayed, toremoved, true)
                               select toremoved;

                list = list.Except(includes).ToList();
            }

            if (!SudokuOptions.Current.IncludeBoxes)
            {
                var exludes = from sol1 in list
                              where (sol1.Type == SudokuSolutionType.XWing) ||
                                    (sol1.Type == SudokuSolutionType.SwordFish) ||
                                    (sol1.Type == SudokuSolutionType.JellyFish) ||
                                    (sol1.Type == SudokuSolutionType.MultivalueXWing)
                              where sol1.ColorUnits.Any(u => (u.First().Col != u.Last().Col) && (u.First().Row != u.Last().Row))
                              select sol1;

                list = list.Except(exludes).ToList();
            }

            return list;
        }

        internal static List<SudokuSolution> Consolidate(List<SudokuSolution> a_list)
        {
            var gr1 = from sol in a_list
                      where !sol.Solved.Any()
                      group sol by new
                      {
                          sol.Type,
                          stayed = sol.Stayed.OrderBy(num => num.Index).Aggregate("", (acc, num) => acc += num.Coords + num.Number),
                          cellcolors = sol.ColorUnits.GetCompareString()
                      } into gr
                      select new SudokuSolution(gr.Key.Type, (from obj in gr
                                                              from num in obj.Removed
                                                              select num).Distinct(),
                                                              gr.First().Stayed,
                                                              null,
                                                              gr.First().ColorUnits);

            var gr2 = from sol in a_list
                      where (sol.Solved.Count() > 0)
                      select sol;

            return FilterByOptions(gr1.Concat(gr2).ToList());
        }
    }
}
