using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows.Forms;

namespace SudokuLINQSolver.Configurations
{
    public class FormState : ConfigurationSection
    {
        private static String SECTION_NAME = "formState";
        private static FormState s_instance;

        private Form m_form;

        static FormState()
        {
            s_instance = Config.Instance.GetSection(SECTION_NAME) as FormState;
   
            if (s_instance == null)
            {
                s_instance = new FormState();
                s_instance.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
                Config.Instance.Sections.Add(SECTION_NAME, s_instance);
            }  
        }

        public static FormState Instance 
        {
            get
            {
                return s_instance;
            }
        }

        public void Init(Form a_form)
        {
            m_form = a_form;
            a_form.FormClosing += OnFormClosing;
        }

        public void LoadState()
        {
            m_form.WindowState = WindowState;
            m_form.Left = Left;
            m_form.Top = Top;
            m_form.Width = Width;
            m_form.Height = Height;
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            Form form = sender as Form;
            WindowState = form.WindowState;

            if (form.WindowState == FormWindowState.Normal)
            {
                Left = form.Left;
                Top = form.Top;
                Width = form.Width;
                Height = form.Height;
            }

            Config.Instance.Save();
        }

        [ConfigurationProperty("windowState", DefaultValue = FormWindowState.Normal)]
        public FormWindowState WindowState
        {
            get
            {
                return (FormWindowState)base["windowState"];
            }
            set
            {
                base["windowState"] = value;
            }
        }

        [ConfigurationProperty("left", DefaultValue = 100)]
        public int Left
        {
            get
            {
                return (int)base["left"];
            }
            set
            {
                base["left"] = value;
            }
        }

        [ConfigurationProperty("top", DefaultValue = 100)]
        public int Top
        {
            get
            {
                return (int)base["top"];
            }
            set
            {
                base["top"] = value;
            }
        }

        [ConfigurationProperty("width", DefaultValue = 500)]
        public int Width
        {
            get
            {
                return (int)base["width"];
            }
            set
            {
                base["width"] = value;
            }
        }

        [ConfigurationProperty("height", DefaultValue = 300)]
        public int Height
        {
            get
            {
                return (int)base["height"];
            }
            set
            {
                base["height"] = value;
            }
        }
    }
}
