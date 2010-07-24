using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace SudokuLib
{
    public class SudokuIntermediateSolution
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SudokuBoard m_before;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SudokuBoard m_after;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SudokuSolution m_solution;

        public SudokuIntermediateSolution(SudokuBoard a_before, SudokuBoard a_after, SudokuSolution a_solution)
        {
            Debug.Assert(a_before != null);

            m_before = a_before;
            m_after = a_after;
            m_solution = a_solution;
        }

        public SudokuBoard Before
        {
            get
            {
                return m_before;
            }
        }

        public SudokuBoard After
        {
            get
            {
                return m_after;
            }
        }

        public SudokuSolution Solution
        {
            get
            {
                return m_solution;
            }
        }

        public bool Test(bool a_all)
        {
            if (m_after == null)
                return false;
            if (m_solution == null)
                return false;

            SudokuSolutionNode node = SudokuSolutionNode.CreateRoot(m_before);

            if (a_all)
                node.StepAll();
            else
                node.Step(m_solution.Type);

            SudokuSolutionNode solved_node = node.Nodes.FirstOrDefault(n => n.Solution.Equals(m_solution));

            if (solved_node == null)
                return false;

            return m_after.Equals(solved_node.NextBoard);
        }

        private static SudokuIntermediateSolution LoadFromXML(XElement a_element)
        {
            try
            {
                SudokuBoard before = SudokuBoard.LoadFromXML(a_element.Element("board_before"));
                SudokuBoard after = SudokuBoard.LoadFromXML(a_element.Element("board_after"));
                SudokuSolution solution = SudokuSolution.LoadFromXML(a_element.Element("solution"), before);

                SudokuIntermediateSolution intermediate_solution = new SudokuIntermediateSolution(before, after, solution);
                   
                return intermediate_solution;
            }
            catch
            {
                return null;
            }   
        }

        public static SudokuIntermediateSolution LoadFromFile(string a_fileName)
        {
            if (Path.GetExtension(a_fileName) == FileExtensions.ZipExt)
            {
                try
                {
                    using (FileStream file_stream = new FileStream(a_fileName, FileMode.Open, FileAccess.Read))
                    {
                        using (GZipStream gzip_stream = new GZipStream(file_stream, CompressionMode.Decompress))
                        {
                            using (StreamReader stream_reader = new StreamReader(gzip_stream, Encoding.ASCII))
                            {
                                return SudokuIntermediateSolution.LoadFromXML(XElement.Load(stream_reader));
                            }
                        }
                    }
                }
                catch
                {
                    return null;
                }
            }
            else if (Path.GetExtension(a_fileName) == FileExtensions.XmlExt)
            {
                return SudokuIntermediateSolution.LoadFromXML(XElement.Load(a_fileName));
            }
            else
                return null;
        }

        public object[] GetAsXML()
        {
            List<XElement> list = new List<XElement>();

            list.Add(new XElement("board_before", m_before.GetAsXML()));
            
            if (m_after != null)
                list.Add(new XElement("board_after", m_after.GetAsXML()));

            if (m_solution != null)
                list.Add(new XElement("solution", m_solution.GetAsXML()));

            return list.ToArray(); 
        }

        public void SaveToFile(string a_fileName)
        {
            using (FileStream file_stream = new FileStream(a_fileName, FileMode.Create, FileAccess.ReadWrite))
            {
                using (GZipStream gzip_stream = new GZipStream(file_stream, CompressionMode.Compress))
                {
                    using (XmlTextWriter xml_test_writer = new XmlTextWriter(gzip_stream, Encoding.ASCII))
                    {
                        new XElement("intermediate_solution", GetAsXML()).WriteTo(xml_test_writer);
                    };
                }
            }
        }

        public override string ToString()
        {
            return String.Format("Solution: {{ {0} }}; Before: {{ {1} }}; After: {{ {2} }}", m_solution, m_before, m_after);
        }

        public override bool Equals(object a_obj)
        {
            if (a_obj == null)
                return false;
            if (ReferenceEquals(this, a_obj))
                return true;
            SudokuIntermediateSolution intermediate_solution = a_obj as SudokuIntermediateSolution;
            if (intermediate_solution == null)
                return false;

            return m_before.Equals(intermediate_solution.m_before) &&
                   m_after.Equals(intermediate_solution.m_after) &&
                   m_solution.Equals(intermediate_solution.m_solution);
        }

        public override int GetHashCode()
        {
            int hash = m_before.GetHashCode();

            if (m_after != null)
                hash ^= m_after.GetHashCode();
            
            if (m_solution != null)
                hash ^= m_solution.GetHashCode();

            return hash;
        }
    }
}
