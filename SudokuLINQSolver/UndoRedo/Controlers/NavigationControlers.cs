using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace UndoRedoLib.Controlers
{
    public class NavigationControlers
    {
        private Dictionary<Type, Type> m_map = new Dictionary<Type, Type>();

        public static NavigationControlers Instance = new NavigationControlers();

        protected NavigationControlers()
        {
            RegisterControlType(typeof(ToolStripDropDownButton), typeof(NavigationToolStripDropDownItemControler));
            RegisterControlType(typeof(ToolStripButton), typeof(NavigationToolStripItemControler));
            RegisterControlType(typeof(Button), typeof(NavigationButtonControler));
            RegisterControlType(typeof(ToolStripMenuItem), typeof(NavigationToolStripMenuItemControler));
            RegisterControlType(typeof(ToolStripSplitButton), typeof(NavigationToolStripSplitButtonControler));
        }

        public void RegisterControlType(Type a_controlType, Type a_controlerType)
        {
            if (a_controlType == null)
                throw new ArgumentNullException();
            if (a_controlerType == null)
                throw new ArgumentNullException();
            if (!a_controlerType.IsDerivedFrom(typeof(NavigationControler)))
                throw new ArgumentException();

            m_map[a_controlType] = a_controlerType;
        }

        internal NavigationControler GetControler(Component a_component)
        {
            if (a_component == null)
                throw new ArgumentNullException();

            Type control_type = a_component.GetType();
            Type controler_type = m_map[a_component.GetType()];
            while ((controler_type == null) && (control_type != null))
            {
                control_type = control_type.BaseType;
                controler_type = m_map[a_component.GetType()];
            }

            if (controler_type == null)
                throw new NotSupportedException();

            NavigationControler controler = (NavigationControler)Activator.CreateInstance(controler_type);

            controler.Component = a_component;

            return controler;
        }
    }
}
