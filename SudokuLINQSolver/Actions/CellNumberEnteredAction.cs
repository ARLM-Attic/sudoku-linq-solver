using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndoRedoLib;
using SudokuLib;

namespace SudokuLINQSolver.Actions
{
    public class CellNumberEntered : UndoRedoAction
    {
        private SudokuCell m_cell;
        private int m_number_index;
        private SudokuNumberState[] m_states;

        public CellNumberEntered(SudokuCell a_cell, int a_number_index) 
        {
            m_cell = a_cell;
            m_number_index = a_number_index;
            m_states = (from num in a_cell.Numbers()
                        select num.State).ToArray();
        }

        public override void Undo()
        {
            m_states.ForEachWithIndex((num, index) => m_cell[index].State = m_states[index]);
        }

        public override void Redo()
        {
            m_cell[m_number_index].State = SudokuNumberState.sudokucellstateManualEntered;
        }

        public override string UndoDescription
        {
            get
            {
                return String.Format("Undo number {0} entered into {1}", m_number_index + 1, m_cell.Coords);
            }
        }

        public override string RedoDescription
        {
            get
            {
                return String.Format("Redo number {0} entered into {1}", m_number_index + 1, m_cell.Coords);
            }
        }
    }
}
