using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndoRedoLib;
using SudokuLib;
using TomanuExtensions;

namespace SudokuLINQSolver.Actions
{
    public class CellClearAction : UndoRedoAction
    {
        private SudokuCell m_cell;
        private SudokuNumberState[] m_states;

        public CellClearAction(SudokuCell a_cell) 
        {
            m_cell = a_cell;
            m_states = (from num in a_cell.Numbers()
                        select num.State).ToArray();
        }

        public override void Undo()
        {
            m_states.ForEachWithIndex((num, index) => m_cell[index].State = m_states[index]);
        }

        public override void Redo()
        {
            m_cell.Clear();
        }

        public override string UndoDescription
        {
            get
            {
                return String.Format("Undo cell {0} clear", m_cell.Coords);
            }
        }

        public override string RedoDescription
        {
            get
            {
                return String.Format("Redo cell {0} clear", m_cell.Coords);
            }
        }
    }
}
