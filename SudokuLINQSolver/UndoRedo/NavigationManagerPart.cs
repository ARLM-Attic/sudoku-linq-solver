using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace UndoRedoLib
{
    internal class NavigationManagerPart
    {
        protected Action<Object> m_restore_func;
        protected Func<Object> m_save_func;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private object m_owner;

        public NavigationManagerPart(NavigationManagerPart a_part)
            : this(a_part.m_restore_func, a_part.m_save_func, a_part.m_owner)
        {
        }

        public NavigationManagerPart(Action<Object> a_restore_func, Func<Object> a_save_func, object a_owner)
        {
            m_restore_func = a_restore_func;
            m_save_func = a_save_func;
            m_owner = a_owner;
        }

        public object Owner
        {
            get
            {
                return m_owner;
            }
        }
    }
}
