using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace UndoRedoLib.Controlers
{
    public class UndoRedoToolStripSplitButtonControler : UndoRedoToolStripDropDownItemControler
    {
        public UndoRedoToolStripSplitButtonControler()
        {
            m_has_click_handler = false;
        }

        protected internal override Component Component
        {
            get
            {
                return base.Component;
            }
            set
            {
                ToolStripSplitButton button = value as ToolStripSplitButton;
                button.ButtonClick += (sender, e) => Action(GetSubList().First());

                base.Component = value;
            }
        }
    }
}
