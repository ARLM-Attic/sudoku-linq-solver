using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomanuExtensions;

namespace SudokuLib
{
    public class SudokuSolutionColorUnits : IEnumerable<IEnumerable<SudokuCell>>
    {
        private class SudokuCellsListComparer : IEqualityComparer<IEnumerable<SudokuCell>>
        {
            public static SudokuCellsListComparer Instance = new SudokuCellsListComparer();

            public bool Equals(IEnumerable<SudokuCell> a_units1, IEnumerable<SudokuCell> a_units2)
            {
                return a_units1.SequenceEqual(a_units2, Comparators.SudokuCellColRowComparer.Instance);
            }

            public int GetHashCode(IEnumerable<SudokuCell> a_unit)
            {
                return a_unit.First().Index + a_unit.Last().Index;
            }
        }

        private List<List<SudokuCell>> m_units = new List<List<SudokuCell>>();

        public SudokuSolutionColorUnits()
        {
        }

        public SudokuSolutionColorUnits(IEnumerable<IEnumerable<SudokuCell>> a_units)
        {
            m_units.AddRange(from unit in a_units
                             select unit.Distinct().OrderBy(c => c.Index).ToList());
        }

        public override bool Equals(object a_obj)
        {
            if (a_obj == null)
                return false;
            if (ReferenceEquals(this, a_obj))
                return true;
            SudokuSolutionColorUnits units = a_obj as SudokuSolutionColorUnits;
            if (units == null)
                return false;

            return m_units.ContainsExact(units.m_units, SudokuCellsListComparer.Instance);
        }

        public override int GetHashCode()
        {
            return m_units.Aggregate(0, (acc, unit) => acc += unit.First().Index + unit.Last().Index);
        }

        public override string ToString()
        {
            string str = "";

            foreach (var unit in m_units)
            {
                if (unit.First().Row == unit.Last().Row)
                    str += "Row " + unit.First().Coords;
                else if (unit.First().Col == unit.Last().Col)
                    str += "Col " + unit.First().Coords;
                else 
                    str += "Box " + unit.First().Coords;

                if (unit != m_units.Last())
                    str = str = "; ";
            }

            return str;
        }

        internal string GetCompareString()
        {
            StringBuilder str = new StringBuilder();

            var units = from unit in m_units
                        orderby unit.First().Index, unit.Last().Index
                        select unit;

            foreach (var unit in units)
            {
                if (unit.First().Row == unit.Last().Row)
                    str.Append("Row");
                else if (unit.First().Col == unit.Last().Col)
                    str.Append("Col");
                else
                    str.Append("Box");

                unit.ForEach(cell => str.Append(cell.Coords));
            }

            return str.ToString();
        }

        public IEnumerator<IEnumerable<SudokuCell>> GetEnumerator()
        {
            return m_units.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_units.GetEnumerator();
        }
    }
}
