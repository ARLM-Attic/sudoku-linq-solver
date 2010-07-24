using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace UndoRedoLib.Controlers
{
    public class NavigationControler
    {
        private Component m_component;
        private dynamic m_dyn_component;
        protected string m_def_tool_tip_text;
        protected bool m_has_tool_tip_text = true;
        protected bool m_has_sub_menu = true;
        protected bool m_has_click_handler = true;

        protected internal virtual Component Component
        {
            get
            {
                return m_component;
            }
            set
            {
                m_component = value;
                m_dyn_component = value;

                m_def_tool_tip_text = ToolTipText;
            }
        }

        protected internal dynamic DynamicComponent
        {
            get
            {
                return m_dyn_component;
            }
        }

        protected internal virtual bool Enabled 
        {
            get
            {
                return m_dyn_component.Enabled;
            }
            set
            {
                m_dyn_component.Enabled = value;
            }
        }

        protected internal virtual string ToolTipText
        {
            get
            {
                if (m_has_tool_tip_text)
                {
                    return m_dyn_component.ToolTipText;
                }
                else
                    return String.Empty;
            }
            set
            {
                if (m_has_tool_tip_text)
                {
                    if (UndoRedoConfiguration.Navigation_ModifyTooltips)
                    {
                        if (value == String.Empty)
                            value = m_def_tool_tip_text;

                        m_dyn_component.ToolTipText = value;
                    }
                    else
                        m_dyn_component.ToolTipText = m_def_tool_tip_text;
                }    
            }
        }

        protected internal virtual void SetSubMenu()
        {
        }

        internal void Update()
        {
            Enabled = GetEnabled();
            SetSubMenu();
            ToolTipText = GetDescription(GetSubList().FirstOrDefault());
        }

        internal Func<bool> GetEnabled;
        internal Func<NavigationState, string> GetDescription;
        internal Func<IEnumerable<NavigationState>> GetSubList;
        internal Action<NavigationState> Action;
    }
}
