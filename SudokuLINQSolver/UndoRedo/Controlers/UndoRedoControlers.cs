using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using TomanuExtensions;

namespace UndoRedoLib.Controlers
{
    public class UndoRedoControlers
    {
        private Dictionary<Type, Type> m_map = new Dictionary<Type, Type>();

        public static UndoRedoControlers Instance = new UndoRedoControlers();

        protected UndoRedoControlers()
        {
            RegisterControlType(typeof(ToolStripDropDownButton), typeof(UndoRedoToolStripDropDownItemControler));
            RegisterControlType(typeof(ToolStripButton), typeof(UndoRedoToolStripItemControler));
            RegisterControlType(typeof(Button), typeof(UndoRedoButtonControler));
            RegisterControlType(typeof(ToolStripMenuItem), typeof(UndoRedoToolStripMenuItemControler));
            RegisterControlType(typeof(ToolStripSplitButton), typeof(UndoRedoToolStripSplitButtonControler));
        }

        public void RegisterControlType(Type a_controlType, Type a_controlerType)
        {
            if (a_controlType == null)
                throw new ArgumentNullException();
            if (a_controlerType == null)
                throw new ArgumentNullException();
            if (!a_controlerType.IsDerivedFrom(typeof(UndoRedoControler)))
                throw new ArgumentException();

            m_map[a_controlType] = a_controlerType;
        }

        internal UndoRedoControler GetControler(Component a_component)
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

            UndoRedoControler controler = (UndoRedoControler)Activator.CreateInstance(controler_type);

            controler.Component = a_component;

            return controler;
        }
    }
}
