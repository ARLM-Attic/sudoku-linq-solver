using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace UndoRedoLib.Controlers
{
    public class UndoRedoToolStripItemControler : UndoRedoControler
    {
        protected internal override Component Component
        {
            get
            {
                return base.Component;
            }
            set
            {
                ToolStripItem button = value as ToolStripItem;

                if (m_has_click_handler)
                    button.Click += (sender, e) => Action(GetSubList().First());

                base.Component = value;
            }
        }
    }
}
