using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using TomanuExtensions;

namespace SudokuLib
{
    public class SudokuNumber
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int m_number;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SudokuCell m_cell;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SudokuNumberState m_state;

        internal SudokuNumber(SudokuCell a_cell, int a_number)
        {
            m_cell = a_cell;
            m_number = a_number;
            m_state = SudokuNumberState.sudokucellstatePossible;
        }

        public  SudokuCell Cell
        {
            get
            {
                return m_cell;
            }
        }

        public override bool Equals(object a_obj)
        {
            if (a_obj == null)
                return false;
            if (ReferenceEquals(this, a_obj))
                return true;
            SudokuNumber num = a_obj as SudokuNumber;
            if (num == null)
                return false;

            return (m_state == num.State) && 
                   (num.Index == Index) && 
                   (m_number == num.Number);
        }

        public override int GetHashCode()
        {
            return m_number ^ (int)m_state ^ Index;
        }

        public int Number
        {
            get
            {
                return m_number;
            }
        }

        public string Coords
        {
            get
            {
                return Cell.Coords;
            }
        }

        public SudokuNumberState State
        {
            get
            {
                return m_state;
            }
            set
            {
                if ((value == SudokuNumberState.sudokucellstateManualEntered) || (value == SudokuNumberState.sudokucellstateSolved))
                {
                    (from num in m_cell.Numbers()
                     where !Object.ReferenceEquals(num, this)
                     where num.State != SudokuNumberState.sudokucellstateImpossible
                     select num).ForEach(num => num.m_state = SudokuNumberState.sudokucellstateImpossible);
                }

                m_state = value;

                m_cell.OnNumberChanged(this);
            }
        }

        public int Row
        {
            get
            {
                return m_cell.Row;
            }
        }

        public int Col
        {
            get
            {
                return m_cell.Col;
            }
        }

        internal int Index
        {
            get
            {
                return m_cell.Index;
            }
        }

        public Point Box
        {
            get
            {
                return m_cell.Box;
            }
        }

        internal IEnumerable<SudokuNumber> BoxNumbers()
        {
            return m_cell.BoxNumbers();
        }

        internal IEnumerable<SudokuCell> BoxCells()
        {
            return m_cell.BoxCells();
        }

        internal IEnumerable<SudokuCell> RowCells()
        {
            return m_cell.RowCells();
        }

        internal IEnumerable<SudokuNumber> RowNumbers()
        {
            return m_cell.RowNumbers();
        }

        internal IEnumerable<SudokuCell> ColCells()
        {
            return m_cell.ColCells();
        }

        internal IEnumerable<SudokuNumber> ColNumbers()
        {
            return m_cell.ColNumbers();
        }

        public override string ToString()
        {
            return String.Format("{0}, col: {1}, row: {2}, number: {3}, state: {4}", Coords,
                Col, Row, Number, m_state);
        }

        public bool IsSolved
        {
            get
            {
                return (m_state == SudokuNumberState.sudokucellstateManualEntered) || 
                    (m_state == SudokuNumberState.sudokucellstateSolved);
            }
        }

        internal void Clear()
        {
            m_state = SudokuNumberState.sudokucellstatePossible;

            m_cell.OnNumberChanged(this);
        }

        internal void CopyFrom(SudokuNumber a_number)
        {
            m_state = a_number.State;

            m_cell.OnNumberChanged(null);
        }
    }
}
