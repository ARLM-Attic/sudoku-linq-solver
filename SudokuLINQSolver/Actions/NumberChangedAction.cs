using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndoRedoLib;
using SudokuLib;
using TomanuExtensions;

namespace SudokuLINQSolver.Actions
{
    public class NumberChangedAction : UndoRedoAction
    {
        private SudokuNumber m_number;
        private SudokuNumberState[] m_states;

        public NumberChangedAction(SudokuNumber a_number) 
        {
            m_number = a_number;
            m_states = (from num in a_number.Cell.Numbers()
                        select num.State).ToArray();
        }

        public override void Undo()
        {
            m_states.ForEachWithIndex((num, index) => m_number.Cell[index].State = m_states[index]);
        }

        public override void Redo()
        {
            if (m_number.State == SudokuNumberState.sudokucellstatePossible)
                m_number.State = SudokuNumberState.sudokucellstateImpossible;
            else if (m_number.State == SudokuNumberState.sudokucellstateImpossible)
                m_number.State = SudokuNumberState.sudokucellstatePossible;
        }

        public override string UndoDescription
        {
            get
            {
                if (m_states[m_number.Number - 1] != SudokuNumberState.sudokucellstateImpossible)
                    return String.Format("Mark number {0} in cell {1}", m_number.Number, m_number.Cell.Coords);
                else
                    return String.Format("Unmark number {0} in cell {1}", m_number.Number, m_number.Cell.Coords);
            }
        }

        public override string RedoDescription
        {
            get
            {
                if (m_states[m_number.Number - 1] == SudokuNumberState.sudokucellstateImpossible)
                    return String.Format("Mark number {0} in cell {1}", m_number.Number, m_number.Cell.Coords);
                else
                    return String.Format("Unmark number {0} in cell {1}", m_number.Number, m_number.Cell.Coords);
            }
        }
    }
}
