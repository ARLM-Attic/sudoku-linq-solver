using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace UndoRedoLib.Controlers
{
    public class UndoRedoButtonControler : UndoRedoControler
    {
        public UndoRedoButtonControler()
        {
            m_has_tool_tip_text = false;
        }

        protected internal override Component Component
        {
            get
            {
                return base.Component;
            }
            set
            {
                Button button = value as Button;
                button.Click += (sender, e) => Action(GetSubList().First());

                base.Component = value;
            }
        }
    }
}
