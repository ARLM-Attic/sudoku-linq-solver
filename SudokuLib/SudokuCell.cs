using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace SudokuLib
{
    public class SudokuCell
    {
        private SudokuNumber[] m_numbers;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int m_index;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SudokuBoard m_board;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int m_row;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int m_col;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Point m_box;

        internal SudokuCell(SudokuBoard a_board, int a_col, int a_row)
        {
            m_board = a_board;

            m_numbers = new SudokuNumber[SudokuBoard.SIZE];

            m_row = a_row;
            m_col = a_col;
            m_index = a_row * SudokuBoard.SIZE + a_col;
            m_box = new Point(m_col / SudokuBoard.BOX_SIZE, m_row / SudokuBoard.BOX_SIZE);

            for (int i = 1; i <= SudokuBoard.SIZE; i++)
                m_numbers[i - 1] = new SudokuNumber(this, i);
        }

        public override bool Equals(object a_obj)
        {
            if (a_obj == null)
                return false;
            if (ReferenceEquals(this, a_obj))
                return true;
            SudokuCell cell = a_obj as SudokuCell;
            if (cell == null)
                return false;

            return m_numbers.SequenceEqual(cell.Numbers());
        }

        public SudokuBoard Board
        {
            get
            {
                return m_board;
            }
        }

        public override int GetHashCode()
        {
            return m_index ^ m_numbers.Sum(num => num.GetHashCode());
        }

        public int Row
        {
            get
            {
                return m_row;
            }
        }

        public int Col
        {
            get
            {
                return m_col;
            }
        }

        internal int Index
        {
            get
            {
                return m_index;
            }
        }

        public Point Box
        {
            get
            {
                return m_box;
            }
        }

        internal IEnumerable<SudokuNumber> BoxNumbers()
        {
            return m_board.BoxNumbers(m_box);
        }

        internal IEnumerable<SudokuCell> BoxCells()
        {
            return m_board.BoxCells(m_box);
        }

        internal IEnumerable<SudokuCell> RowCells()
        {
            return m_board.RowCells(m_row);
        }

        internal IEnumerable<SudokuNumber> RowNumbers()
        {
            return m_board.RowNumbers(m_row);
        }


        internal IEnumerable<SudokuCell> ColCells()
        {
            return m_board.ColCells(m_col);
        }

        internal IEnumerable<SudokuNumber> ColNumbers()
        {
            return m_board.ColNumbers(m_col);
        }

        public SudokuNumber this[int a_index]
        {
            get
            {
                return m_numbers[a_index];
            }
        }

        internal void CopyFrom(SudokuCell a_src)
        {
            for (int i = 0; i < SudokuBoard.SIZE; i++)
                this[i].CopyFrom(a_src[i]);
        }

        public IEnumerable<SudokuNumber> Numbers()
        {
            return m_numbers;
        }

        public bool IsSolved
        {
            get
            {
                return m_numbers.Any(n => n.IsSolved);
            }
        }

        public SudokuNumber NumberSolved()
        {
            return m_numbers.FirstOrDefault(n => n.IsSolved);
        }

        internal IEnumerable<SudokuNumber> NumbersPossible()
        {
            return (from num in m_numbers
                    where num.State == SudokuNumberState.sudokucellstatePossible
                    select num);
        }

        public string Coords
        {
            get
            {
                return Converters.PointToCoords(m_col, m_row);
            }
        }

        public override string ToString()
        {
            return String.Format("{0}, col: {1}, row: {2}", Coords, m_col, m_row);
        }

        public void Clear()
        {
            Numbers().ForEach((number) => number.Clear());
        }

        internal void OnNumberChanged(SudokuNumber a_number)
        {
            Board.OnCellChanged(a_number);
        }
    }
}
