using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UndoRedoLib.Controlers
{
    public class NavigationToolStripDropDownItemControler : NavigationToolStripItemControler
    {
        public NavigationToolStripDropDownItemControler()
        {
            m_has_click_handler = false;
        }

        protected internal override void SetSubMenu()
        {
            if (!m_has_sub_menu)
                return;

            ToolStripDropDownItem item = Component as ToolStripDropDownItem;

            FillDropDownItem(item, GetSubList(), GetDescription, Action);
        }

        public static void FillDropDownItem(ToolStripDropDownItem a_item, IEnumerable<NavigationState> a_list,
                                            Func<NavigationState, string> a_description, Action<NavigationState> a_action)
        {
            ToolStripDropDownItem ddi = a_item as ToolStripDropDownItem;

            ddi.DropDown.Items.Clear();

            foreach (NavigationState state in a_list)
            {
                string text = a_description(state);
                if (text == null)
                    text = String.Empty;

                ToolStripItem tti = ddi.DropDown.Items.Add(text);

                tti.Tag = state;
                tti.Click += (s, e) =>
                {
                    ToolStripDropDownItem item = s as ToolStripDropDownItem;
                    NavigationState item_action = item.Tag as NavigationState;

                    a_action(item_action);
                };
            }
        }
    }
}
