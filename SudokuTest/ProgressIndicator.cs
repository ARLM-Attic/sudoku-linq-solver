using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TomanuExtensions.TestUtils;

namespace SudokuTest
{
    public class ProgressIndicator1
    {
        private Thread m_thread;
        private ProgressForm1 m_form = new ProgressForm1();

        public ProgressIndicator1()
        {
            AutoResetEvent are = new AutoResetEvent(false);

            m_thread = new Thread(() =>
            {
                Application.EnableVisualStyles();
                are.Set();
                Application.Run(m_form);
                m_thread = null;
            });

            m_thread.SetApartmentState(ApartmentState.MTA);
            m_thread.Start();
            are.WaitOne();
            are.Close();
        }

        public bool IsDisposed
        {
            get
            {
                return m_form.IsDisposed;
            }
        }

        private T Invoke<T>(Func<T> a_delegate)
        {
            if (m_form.InvokeRequired)
            {
                try
                {
                    return (T)m_form.Invoke(a_delegate);
                }
                catch (ObjectDisposedException)
                {
                    return default(T);
                }
            }
            else if (m_form.IsHandleCreated)
                return a_delegate();
            else
                return default(T);
        }

        private void Invoke(Action a_delegate)
        {
            if (m_form.InvokeRequired)
                m_form.BeginInvoke(a_delegate);
            else if (m_form.IsHandleCreated)
                a_delegate();
        }

        public int Progress
        {
            get
            {
                return Invoke(() => { return m_form.Progress; });
            }
            set
            {
                Invoke(() => { m_form.Progress = value; });
            }
        }

        public string TestName
        {
            get
            {
                return Invoke(() => { return m_form.TestName; });
            }
            set
            {
                Invoke(() => { m_form.TestName = value; });
            }
        }
    }
}
