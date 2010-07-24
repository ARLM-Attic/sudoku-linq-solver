using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SudokuTest
{
    public partial class ProgressForm : Form
    {
        private string m_testName;

        public ProgressForm()
        {
            InitializeComponent();
        }

        public string TestName
        {
            get
            {
                return m_testName;
            }
            set
            {
                m_testName = value;
                UpdateCaption();
            }
        }

        public int Progress
        {
            get
            {
                return progressBar.Value;
            }
            set
            {
                progressBar.Value = value;
                UpdateCaption();
            }
        }

        private void UpdateCaption()
        {
            Text = m_testName + " - " + progressBar.Value + "%";
        }
    }
}
