using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UndoRedoLib.Controlers
{
    public class UndoRedoToolStripDropDownItemControler : UndoRedoToolStripItemControler
    {
        public UndoRedoToolStripDropDownItemControler()
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

        public static void FillDropDownItem(ToolStripDropDownItem a_item, IEnumerable<UndoRedoAction> a_list, 
                                            Func<UndoRedoAction, string> a_description, Action<UndoRedoAction> a_action)
        {
            ToolStripDropDownItem ddi = a_item as ToolStripDropDownItem;

            ddi.DropDown.Items.Clear();

            foreach (UndoRedoAction action in a_list)
            {
                string text = a_description(action);
                if (text == null)
                    text = string.Empty;

                ToolStripItem tti = ddi.DropDown.Items.Add(text);

                tti.Tag = action;
                tti.Click += (s, e) =>
                {
                    ToolStripDropDownItem item = s as ToolStripDropDownItem;
                    UndoRedoAction item_action = item.Tag as UndoRedoAction;

                    a_action(item_action);
                };
            }
        }
    }
}
