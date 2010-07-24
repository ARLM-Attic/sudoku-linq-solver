using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndoRedoLib
{
    public class NavigationState
    {
        private List<NavigationStatePart> m_parts = new List<NavigationStatePart>();
        private bool m_saved;
        private string m_back_description;
        private string m_forward_description;

        internal NavigationState(string a_back_description = null, string a_forward_description = null)
           
        {
            foreach (NavigationManagerPart manager_part in NavigationManager.Instance.Registered)
                m_parts.Add(new NavigationStatePart(manager_part));

            m_back_description = a_back_description;
            m_forward_description = a_forward_description;
        }

        internal void Restore()
        {
            if (!m_saved)
                throw new Exception();

            foreach (NavigationStatePart part in m_parts)
                part.Restore();
        }

        internal void Save()
        {
            m_saved = true;

            foreach (NavigationStatePart part in m_parts)
                part.Save();
        }

        public string BackDescription
        {
            get
            {
                return m_back_description;
            }
        }

        public string ForwardDescription
        {
            get
            {
                return m_forward_description;
            }
        }

        public override string ToString()
        {
            return BackDescription + " ; " + ForwardDescription;
        }
    }
}
