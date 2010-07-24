using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Xml.Linq;
using System.Drawing;
using System.Globalization;

namespace SudokuLib
{
    public class SudokuSolution
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<SudokuNumber> m_removed;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<SudokuNumber> m_stayed;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<SudokuNumber> m_solved;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SudokuSolutionColorUnits m_colorUnits;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SudokuSolutionType m_type;

        public static IEnumerable<IEnumerable<SudokuSolutionType>> Includes = new []
        {
             new [] { SudokuSolutionType.XWing, SudokuSolutionType.BoxLineReduction }, 
             new [] { SudokuSolutionType.XWing, SudokuSolutionType.PointingPair }, 
             new [] { SudokuSolutionType.SwordFish, SudokuSolutionType.XWing }, 
             new [] { SudokuSolutionType.SwordFish, SudokuSolutionType.PointingPair },
             new [] { SudokuSolutionType.SwordFish, SudokuSolutionType.PointingTriple }, 
             new [] { SudokuSolutionType.SwordFish, SudokuSolutionType.BoxLineReduction },
             new [] { SudokuSolutionType.JellyFish, SudokuSolutionType.SwordFish },
             new [] { SudokuSolutionType.JellyFish, SudokuSolutionType.XWing },
             new [] { SudokuSolutionType.JellyFish, SudokuSolutionType.PointingPair },
             new [] { SudokuSolutionType.JellyFish, SudokuSolutionType.PointingTriple },
             new [] { SudokuSolutionType.JellyFish, SudokuSolutionType.BoxLineReduction },
             new [] { SudokuSolutionType.MultivalueXWing, SudokuSolutionType.PointingPair }, 
        };

        public SudokuSolution(SudokuSolutionType a_type, IEnumerable<SudokuNumber> a_removed, IEnumerable<SudokuNumber> a_stayed,
            IEnumerable<SudokuNumber> a_solved, IEnumerable<IEnumerable<SudokuCell>> a_cellColors)
        {
            m_type = a_type;

            if (a_removed != null)
                m_removed = a_removed.Distinct().OrderBy(num => num.Index).ToList();
            else
                m_removed = new List<SudokuNumber>();

            if (a_stayed != null)
                m_stayed = a_stayed.Distinct().OrderBy(num => num.Index).ToList();
            else
                m_stayed = new List<SudokuNumber>();

            if (a_solved != null)
                m_solved = a_solved.Distinct().OrderBy(num => num.Index).ToList();
            else
                m_solved = new List<SudokuNumber>();

            if (a_cellColors != null)
                m_colorUnits = new SudokuSolutionColorUnits(a_cellColors);
            else
                m_colorUnits = new SudokuSolutionColorUnits();
        }

        public SudokuSolutionType Type
        {
            get
            {
                return m_type;
            }
        }

        public IEnumerable<SudokuNumber> Removed
        {
            get
            {
                return m_removed;
            }
        }

        public SudokuSolutionColorUnits ColorUnits 
        {
            get
            {
                return m_colorUnits;
            }
        }

        public IEnumerable<SudokuNumber> Stayed
        {
            get
            {
                return m_stayed;
            }
        }

        public IEnumerable<SudokuNumber> Solved
        {
            get
            {
                return m_solved;
            }
        }

        public string Info
        {
            get
            {
                return Converters.SudokuSolutionTypeToString(m_type);
            }
        }

        internal static string MakeList(IEnumerable<SudokuNumber> a_numbers)
        {
            string res = a_numbers.Aggregate("", (str, num) => str += num.Coords + "." + num.Number + "  ");
            return res.Substring(0, res.Length - 2);
        }

        internal static string MakeList(IEnumerable<SudokuCell> a_cells)
        {
            string res = a_cells.Aggregate("", (str, num) => str += num.Coords + ", ");
            return res.Substring(0, res.Length - 2);
        }

        public override string ToString()
        {
            string res = Info;

            switch (m_type)
            {
                case SudokuSolutionType.MarkSolved:
                case SudokuSolutionType.SinglesInUnit: res += ": " + MakeList(Solved); break;

                case SudokuSolutionType.MarkImpossibles: res += ": " + MakeList(m_removed.Select(n => n.Cell).Distinct()); break;

                case SudokuSolutionType.BoxLineReduction:
                case SudokuSolutionType.HiddenPair:
                case SudokuSolutionType.HiddenQuad:
                case SudokuSolutionType.HiddenTriple:
                case SudokuSolutionType.NakedPair:
                case SudokuSolutionType.NakedQuad:
                case SudokuSolutionType.NakedTriple:
                case SudokuSolutionType.PointingPair:
                case SudokuSolutionType.PointingTriple:
                case SudokuSolutionType.SwordFish:
                case SudokuSolutionType.JellyFish:
                case SudokuSolutionType.XWing:
                case SudokuSolutionType.YWing:
                case SudokuSolutionType.XYZWing:
                case SudokuSolutionType.WXYZWing:
                case SudokuSolutionType.MultivalueXWing: res += ": " + MakeList(m_removed) + " <= " + MakeList(m_stayed); break;

                default: throw new Exception();
            }

            return res;
        }

        public override int GetHashCode()
        {
            return m_type.GetHashCode() + 
                   m_removed.Sum(num => num.GetHashCode()) + 
                   m_stayed.Sum(num => num.GetHashCode()) +
                   m_solved.Sum(num => num.GetHashCode()) + 
                   m_colorUnits.SelectMany().Sum(cellcolor => cellcolor.GetHashCode());
        }

        public override bool Equals(object a_obj)
        {
            if (a_obj == null)
                return false;
            if (ReferenceEquals(this, a_obj))
                return true;
            SudokuSolution solution = a_obj as SudokuSolution;
            if (solution == null)
                return false;

            return (m_type == solution.m_type) &&
                   m_removed.Exact(solution.m_removed) &&
                   m_solved.Exact(solution.m_solved) &&
                   m_stayed.Exact(solution.m_stayed) && 
                   m_colorUnits.SelectMany().Exact(solution.m_colorUnits.SelectMany());
        }

        internal static SudokuSolution LoadFromXML(XElement a_node, SudokuBoard a_board)
        {
            try
            {
                if (a_node.IsEmpty)
                    return null;

                return new SudokuSolution(

                    (SudokuSolutionType)Enum.Parse(typeof(SudokuSolutionType), a_node.Attribute("type").Value),

                    from num in a_node.Element("removed").Elements()
                    select a_board[Converters.CoordToPoint(num.Attribute("coord").Value).X,
                        Converters.CoordToPoint(num.Attribute("coord").Value).Y]
                            [Int32.Parse(num.Value) - 1],

                    from num in a_node.Element("stayed").Elements()
                    select a_board[Converters.CoordToPoint(num.Attribute("coord").Value).X,
                        Converters.CoordToPoint(num.Attribute("coord").Value).Y]
                            [Int32.Parse(num.Value) - 1],

                    from num in a_node.Element("solved").Elements()
                    select a_board[Converters.CoordToPoint(num.Attribute("coord").Value).X,
                        Converters.CoordToPoint(num.Attribute("coord").Value).Y]
                            [Int32.Parse(num.Value) - 1],

                    from unit in a_node.Element("cellcolors").Elements()
                    select (from cellcolor in unit.Elements()
                            select a_board[Converters.CoordToPoint(cellcolor.Attribute("coord").Value).X,
                                        Converters.CoordToPoint(cellcolor.Attribute("coord").Value).Y])
                );
            }
            catch
            {
                return null;
            }
        }

        public object[] GetAsXML()
        {
            return new object[] 
            {
                new XAttribute("type", Type), 
                new XElement("removed", from num in m_removed
                                        select new XElement("number", num.Number, new XAttribute("coord", num.Coords))), 
                new XElement("stayed", from num in m_stayed
                                       select new XElement("number", num.Number, new XAttribute("coord", num.Coords))), 
                new XElement("solved", from num in m_solved
                                       select new XElement("number", num.Number, new XAttribute("coord", num.Coords))), 
                new XElement("cellcolors", from unit in m_colorUnits
                                           select new XElement("unit", from cell in unit
                                                                       select new XElement("cell", new XAttribute("coord", cell.Coords))))
            };
        }
    }
}
