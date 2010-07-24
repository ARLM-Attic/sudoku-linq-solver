using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndoRedoLib.Controlers
{
    public class NavigationToolStripMenuItemControler : NavigationToolStripDropDownItemControler
    {
        public NavigationToolStripMenuItemControler()
        {
            m_has_sub_menu = UndoRedoConfiguration.Navigation_SubMenu;
            m_has_click_handler = !m_has_sub_menu;
        }
    }
}
