using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndoRedoLib
{
    internal class NavigationStatePart : NavigationManagerPart
    {
        private object m_state;
        private bool m_saved;

        public NavigationStatePart(NavigationManagerPart a_part)
            : base(a_part)
        {
            m_state = null;
            m_saved = false;
        }

        public void Save()
        {
            if (!NavigationManager.Instance.IsRegistered(Owner))
                throw new Exception();

            m_state = m_save_func();
            m_saved = true;
        }

        public void Restore()
        {
            if (!m_saved)
                throw new Exception();

            if (!NavigationManager.Instance.IsRegistered(Owner))
                throw new Exception();

            m_restore_func(m_state);
        }
    }
}
