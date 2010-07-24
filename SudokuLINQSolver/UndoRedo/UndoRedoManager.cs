using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using UndoRedoLib.Controlers;

namespace UndoRedoLib
{
    // TODO: 
    // pokazac demo z tradycyjnym uzyciem, pokazac demo z edyotrem tekstu - konsolidacja, pokazac demo w 
    // edytorem graficznym - kontrola pamieci
    // wiele instancji tego jak i navigatora
    // weak pointers
    // maksymalny rozmiar pamieci 
    // przyklad z wykorzystaniem tego templata na undo redo elementy, takze na inne struktury
    // przyklad z wykorzystaniem porownywania struktury obiektow
    // wykorzystanie automatycznego reagowania na zmiany w gui, w jakis podstawowych kolekcjach
    public class UndoRedoManager
    {
        private List<UndoRedoAction> m_actions = new List<UndoRedoAction>();
        private int m_action_index = -1;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool m_undo_redo = false;

        private List<UndoRedoControler> m_undo_controlers = new List<UndoRedoControler>();
        private List<UndoRedoControler> m_redo_controlers = new List<UndoRedoControler>();

        public static readonly UndoRedoManager Instance = new UndoRedoManager();

        public event Action Changed;

        protected UndoRedoManager()
        {
        }

        public bool CanRedo
        {
            get
            {
                return !IsInUndoRedo && (m_action_index < m_actions.Count - 1);
            }
        }

        public bool CanUndo
        {
            get
            {
                return !IsInUndoRedo && (m_action_index >= 0);
            }
        }

        public UndoRedoAction CurrentAction
        {
            get
            {
                if (m_action_index == -1)
                    return null;
                else
                    return m_actions[m_action_index];
            }
        }

        public bool IsInUndoRedo
        {
            get
            {
                return m_undo_redo;
            }
        }

        public void Clear()
        {
            if (IsInUndoRedo)
                throw new InvalidOperationException();

            m_actions.Clear();
            m_action_index = -1;

            OnChanged();
        }

        public void Redo(UndoRedoAction a_action = null)
        {
            if (!CanRedo)
                throw new InvalidOperationException();
            if (IsInUndoRedo)
                throw new InvalidOperationException();

            if (a_action == null)
                a_action = RedoActions.First();
            if (!RedoActions.Contains(a_action))
                throw new ArgumentException();

            m_undo_redo = true;

            try
            {
                var todo = RedoActions.Reverse().SkipWhile(a => a != a_action).Reverse();

                todo.First().UndoState.Save();

                foreach (var action in todo)
                {
                    if (UndoRedoConfiguration.Logging)
                        System.Console.WriteLine("redo: " + action.RedoDescription);

                    action.Redo();
                    m_action_index++;
                }

                todo.Last().RedoState.Restore();

            }
            finally
            {
                m_undo_redo = false;
            }

            OnChanged();
        }

        // TODO: dodac opjce ze lista redo nawigacji zawiera kompletna liste nawigacyjna, tak jak w firexie - example zrobic
        public void Undo(UndoRedoAction a_action = null)
        {
            if (!CanUndo)
                throw new InvalidOperationException();
            if (IsInUndoRedo)
                throw new InvalidOperationException();

            if (a_action == null)
                a_action = UndoActions.First();
            if (!UndoActions.Contains(a_action))
                throw new ArgumentException();

            m_undo_redo = true;

            try
            {
                var todo = UndoActions.Reverse().SkipWhile(a => a != a_action).Reverse();
                
                todo.First().RedoState.Save();

                foreach (var action in todo)
                {
                    if (UndoRedoConfiguration.Logging)
                        System.Console.WriteLine("undo: " + action.UndoDescription);

                    action.Undo();
                    m_action_index--;
                }

                todo.Last().UndoState.Restore();
                
            }
            finally
            {
                m_undo_redo = false;
            }

            OnChanged();
        }

        private void OnChanged()
        {
            if (Changed != null)
                Changed();

            foreach (var controler in m_undo_controlers.Concat(m_redo_controlers))
                controler.Update();
        }

        public void SaveAndExecute(UndoRedoAction a_action)
        {
            if (IsInUndoRedo)
                throw new Exception();

            if (m_action_index < m_actions.Count - 1)
                m_actions.RemoveRange(m_action_index + 1, m_actions.Count - (m_action_index + 1));

            a_action.UndoState.Save();
            m_actions.Add(a_action);
            m_action_index++;

            if (m_actions.Count > UndoRedoConfiguration.UndoRedo_MaxDeep)
            {
                m_actions = m_actions.Skip(m_actions.Count - UndoRedoConfiguration.UndoRedo_MaxDeep).
                    Take(UndoRedoConfiguration.UndoRedo_MaxDeep).ToList();
            }

            OnChanged();

            CurrentAction.Redo();
        }

        public void Init()
        {
            OnChanged();
        }

        public IEnumerable<UndoRedoAction> UndoActions
        {
            get
            {
                return m_actions.Take(m_action_index + 1).Reverse().Take(UndoRedoConfiguration.UndoRedo_MaxSubMenuItems);
            }
        }

        public IEnumerable<UndoRedoAction> RedoActions
        {
            get
            {
                return m_actions.Skip(m_action_index + 1).Reverse().Take(UndoRedoConfiguration.UndoRedo_MaxSubMenuItems).Reverse();
            }
        }

        public void RegisterUndoComponent(Component a_component)
        {
            if (m_undo_controlers.Any(c => Object.ReferenceEquals(c.Component, a_component)))
                throw new InvalidOperationException();
            if (m_redo_controlers.Any(c => Object.ReferenceEquals(c.Component, a_component)))
                throw new InvalidOperationException();

            UndoRedoControler controler = UndoRedoControlers.Instance.GetControler(a_component);
            m_undo_controlers.Add(controler);

            controler.GetDescription = (action) => (action == null) ? null : action.UndoDescription;
            controler.GetEnabled = () => CanUndo;
            controler.GetSubList = () => UndoActions;
            controler.Action = (action) => Undo(action);
                
            controler.Component.Disposed += (sender, e) => UnregisterUndoComponent(a_component);

            controler.Update();
        }

        public void UnregisterUndoComponent(Component a_component)
        {
            m_undo_controlers.RemoveAll(c => Object.ReferenceEquals(c.Component, a_component));
        }

        public void RegisterRedoComponent(Component a_component)
        {
            if (m_undo_controlers.Any(c => Object.ReferenceEquals(c.Component, a_component)))
                throw new InvalidOperationException();
            if (m_redo_controlers.Any(c => Object.ReferenceEquals(c.Component, a_component)))
                throw new InvalidOperationException();

            UndoRedoControler controler = UndoRedoControlers.Instance.GetControler(a_component);
            m_redo_controlers.Add(controler);

            controler.GetDescription = (action) => (action == null) ? null : action.UndoDescription;
            controler.GetEnabled = () => CanRedo;
            controler.GetSubList = () => RedoActions;
            controler.Action = (action) => Redo(action);

            controler.Component.Disposed += (sender, e) => UnregisterRedoComponent(a_component);

            controler.Update();
        }

        public void UnregisterRedoComponent(Object a_component)
        {
            m_redo_controlers.RemoveAll(c => Object.ReferenceEquals(c.Component, a_component));
        }
    }
}
