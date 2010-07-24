using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using UndoRedoLib.Controlers;

namespace UndoRedoLib
{
    // TODO:
    // przyklad edytora tekstu na zakladkach, mozna dodawac i kasowac zakladki, + nawigacja w warunkach kasowania, dodawania linii.
    // dodac mozliwosc rejestracji wlasnych klas dla navigacji (przyciski back i forward i kontrolki nawigacyjne )i undoredo
    // dodac klase do rejestracji stanu focusu dla formy
    // dodac klase do rejestracji i przywracania stanu property dla navi i undo redo
    // zapis i odczyt, serializacja, xml
    // dodac kontrolki, ktore rejestruje sie tutaj jako te co wywoluja nawigacje
    // wykonanie undo, wykonanie back, gdzie cos juz nie istnieje, potencjalne rozwiaxzania: 
    // kazde undo reod czysci staty navi, pamietamy podczas undo redo staty navi i je tez przywracamy, cofamy staty nawi do 
    // tych w undo, nic nie rovimy, staty nawi powinny sobie z tym poradzic, zastosowanie tylko rozlaczne i taki design
    public class NavigationManager
    {
        private List<NavigationState> m_states = new List<NavigationState>();
        private int m_navi_index = -1;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool m_navigating = false;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<NavigationManagerPart> m_parts = new List<NavigationManagerPart>();

        private List<NavigationControler> m_back_controlers = new List<NavigationControler>();
        private List<NavigationControler> m_forward_controlers = new List<NavigationControler>();

        public event Action Changed;

        public static readonly NavigationManager Instance = new NavigationManager();

        protected NavigationManager()
        {
        }

        public void Register(Object a_owner, Action<Object> a_restore_func, Func<Object> a_save_func)
        {
            if (IsRegistered(a_owner))
                throw new InvalidOperationException();

            m_parts.Add(new NavigationManagerPart(a_restore_func, a_save_func, a_owner));
        }

        public void Unregister(Object a_owner)
        {
            m_parts.Remove(m_parts.FirstOrDefault(state => Object.ReferenceEquals(state.Owner, a_owner)));
        }

        internal List<NavigationManagerPart> Registered
        {
            get
            {
                return m_parts;
            }
        }

        internal bool IsRegistered(object a_owner)
        {
            return m_parts.Any(part => Object.ReferenceEquals(part.Owner, a_owner));
        }

        public bool CanBack
        {
            get
            {
                return !IsNavigating && (m_navi_index >= 1);
            }
        }

        public bool CanForward
        {
            get
            {
                return !IsNavigating && (m_navi_index < m_states.Count - 1);
            }
        }

        internal NavigationState CurrentState
        {
            get
            {
                if (m_navi_index == -1)
                    return null;
                else
                    return m_states[m_navi_index];
            }
        }

        public bool IsNavigating
        {
            get
            {
                return m_navigating;
            }
        }

        public void Clear()
        {
            if (IsNavigating)
                throw new InvalidOperationException();

            m_states.Clear();
            m_navi_index = -1;

            SaveNew(UndoRedoConfiguration.Navigation_InitialStateDescription, null, true);
            
            OnChanged();
        }

        public void Forward(NavigationState a_state = null)
        {
            if (!CanForward)
                throw new InvalidOperationException();
            if (IsNavigating)
                throw new InvalidOperationException();
            
            if (a_state == null)
                a_state = ForwardStates.First();
            if (!ForwardStates.Contains(a_state))
                throw new ArgumentException();

            m_navigating = true;

            try
            {
                CurrentState.Save();

                m_navi_index = m_states.IndexOf(a_state);

                if (UndoRedoConfiguration.Logging)
                    System.Console.WriteLine("forward: " + a_state.ForwardDescription);

                CurrentState.Restore();
            }
            finally
            {
                m_navigating = false;
            }

            OnChanged();
        }

        // TODO: przyklad, ze jesli nie da sie przejsc do stanu nawigacji to powinien byc wyjatek tutaj wychwycony i kasowanie stanu, i 
        // nastepny stan proba.
        public void Back(NavigationState a_state = null)
        {
            if (!CanBack)
                throw new InvalidOperationException();
            if (IsNavigating)
                throw new InvalidOperationException();

            if (a_state == null)
                a_state = BackStates.First();
            if (!BackStates.Contains(a_state))
                throw new ArgumentException();

            m_navigating = true;

            try
            {
                CurrentState.Save();

                m_navi_index = m_states.IndexOf(a_state);
                m_navi_index--;

                if (UndoRedoConfiguration.Logging)
                    System.Console.WriteLine("back: " + a_state.BackDescription);

                CurrentState.Restore();
            }
            finally
            {
                m_navigating = false;
            }

            OnChanged();
        }

        private void OnChanged()
        {
            if (Changed != null)
                Changed();

            foreach (var controler in m_back_controlers.Concat(m_forward_controlers))
                controler.Update();
        }

        public void SaveNew()
        {
            SaveNew("", "");
        }

        public void SaveNew(string a_back_description, string a_forward_description)
        {
            SaveNew(a_back_description, a_forward_description, false);
        }

        private void SaveNew(string a_back_description, string a_forward_description, bool a_init)
        {
            if (IsNavigating)
                return;
            if ((m_navi_index == -1) && !a_init)
                throw new InvalidOperationException();
            if (a_back_description == null)
                throw new ArgumentNullException();
            if ((a_forward_description == null) && !a_init)
                throw new ArgumentNullException();

            if (m_navi_index < m_states.Count - 1)
                m_states.RemoveRange(m_navi_index + 1, m_states.Count - (m_navi_index + 1));

            m_states.Add(new NavigationState(a_back_description, a_forward_description));
            m_navi_index++;

            if (m_states.Count > UndoRedoConfiguration.Navigation_MaxDeep)
            {
                m_states = m_states.Skip(m_states.Count - UndoRedoConfiguration.Navigation_MaxDeep).
                    Take(UndoRedoConfiguration.Navigation_MaxDeep).ToList();
            }

            CurrentState.Save();

            OnChanged();
        }

        public void Init()
        {
            SaveNew(UndoRedoConfiguration.Navigation_InitialStateDescription, null, true);

            OnChanged();
        }

        public IEnumerable<NavigationState> BackStates
        {
            get
            {
                return m_states.Take(m_navi_index + 1).Reverse().Take(UndoRedoConfiguration.Navigation_MaxDeep);
            }
        }

        public IEnumerable<NavigationState> ForwardStates
        {
            get
            {
                return m_states.Skip(m_navi_index + 1).Reverse().Take(UndoRedoConfiguration.Navigation_MaxDeep).Reverse();
            }
        }

        public void RegisterBackComponent(Component a_component)
        {
            if (m_back_controlers.Any(c => Object.ReferenceEquals(c.Component, a_component)))
                throw new InvalidOperationException();
            if (m_forward_controlers.Any(c => Object.ReferenceEquals(c.Component, a_component)))
                throw new InvalidOperationException();

            NavigationControler controler = NavigationControlers.Instance.GetControler(a_component);
            m_back_controlers.Add(controler);

            controler.GetDescription = (state) => (state == null) ? null : state.BackDescription;
            controler.GetEnabled = () => CanBack;
            controler.GetSubList = () => BackStates;
            controler.Action = (state) => Back(state);

            controler.Component.Disposed += (sender, e) => UnregisterBackComponent(a_component);

            controler.Update();
        }

        public void UnregisterBackComponent(Component a_component)
        {
            m_back_controlers.RemoveAll(c => Object.ReferenceEquals(c.Component, a_component));
        }

        public void RegisterForwardComponent(Component a_component)
        {
            if (m_back_controlers.Any(c => Object.ReferenceEquals(c.Component, a_component)))
                throw new InvalidOperationException();
            if (m_forward_controlers.Any(c => Object.ReferenceEquals(c.Component, a_component)))
                throw new InvalidOperationException();

            NavigationControler controler = NavigationControlers.Instance.GetControler(a_component);
            m_forward_controlers.Add(controler);

            controler.GetDescription = (state) => (state == null) ? null : state.ForwardDescription;
            controler.GetEnabled = () => CanForward;
            controler.GetSubList = () => ForwardStates;
            controler.Action = (state) => Forward(state);

            controler.Component.Disposed += (sender, e) => UnregisterForwardComponent(a_component);

            controler.Update();
        }

        public void UnregisterForwardComponent(Object a_component)
        {
            m_forward_controlers.RemoveAll(c => Object.ReferenceEquals(c.Component, a_component));
        }
    }
}
