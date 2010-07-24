using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace UndoRedoLib
{
    // TODO: dodac mozliwosc grupowania akcji w ramach jednej operacji undo redo, dodac opoznione grupowanie np. w edytorach tesktu liter -> wyraz -> zdanie. 
    // mozliwosc rozgrupowania
    // zapis do pliku xml, strumienia xml, strumienia serializacji i odczyt
    public abstract class UndoRedoAction
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NavigationState m_undo_state = new NavigationState();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NavigationState m_redo_state = new NavigationState();

        public abstract void Undo();
        public abstract void Redo();

        internal NavigationState UndoState
        {
            get
            {
                return m_undo_state;
            }
        }

        internal NavigationState RedoState
        {
            get
            {
                return m_redo_state;
            }
        }

        public override string ToString()
        {
            return UndoDescription + " ; " + RedoDescription;
        }

        public abstract string UndoDescription { get; }
        public abstract string RedoDescription { get; }
    }
}
