using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace UndoRedoLib
{
    public class UndoRedoDelegateAction : UndoRedoAction
    {
        public Action UndoAction;
        public Action RedoAction;
        public Func<string> UndoDescriptionAction;
        public Func<string> RedoDescriptionAction;

        public override void Undo()
        {
            if (UndoAction == null)
                throw new InvalidOperationException();

            UndoAction();
        }

        public override void Redo()
        {
            if (RedoAction == null)
                throw new InvalidOperationException();

            RedoAction();
        }

        public override string UndoDescription
        {
            get
            {
                return UndoDescriptionAction();
            }
        }

        public override string RedoDescription
        {
            get
            {
                return RedoDescriptionAction();
            }
        }
    }
}
