using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndoRedoLib.Controlers
{
    public class UndoRedoToolStripMenuItemControler : UndoRedoToolStripDropDownItemControler
    {
        public UndoRedoToolStripMenuItemControler()
        {
            m_has_sub_menu = UndoRedoConfiguration.UndoRedo_SubMenu;
            m_has_click_handler = !m_has_sub_menu;
        }
    }
}
