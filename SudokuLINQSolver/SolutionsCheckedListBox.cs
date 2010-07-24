using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SudokuLib;

namespace SudokuLINQSolver
{
    public partial class SolutionsCheckedListBox : CheckedListBox
    {
        private SudokuSolutionNode m_node;
        private int m_selectedIndex = -1;
        private bool CheckReadOnly = true;

        public SolutionsCheckedListBox()
        {
            InitializeComponent();
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SudokuSolutionNode SelectedSolutionNode
        {
            get
            {
                if (m_node == null)
                    return null;
                else
                {
                    return m_node.Nodes.FirstOrDefault(node => (node.Solution != null) && node.Solution.Equals(SelectedItem as SudokuSolution));
                }
            }
            set
            {
                if (value == null)
                    SelectedItem = null;
                else
                    SelectedSolution = value.Solution;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SudokuSolution SelectedSolution
        {
            get
            {
                return (SudokuSolution)SelectedItem;
            }
            set
            {
                SelectedItem = Items.Cast<SudokuSolution>().FirstOrDefault(s => s.Equals(value));
            }
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            m_selectedIndex = SelectedIndex;

            base.OnSelectedIndexChanged(e);
        }

        protected override void OnItemCheck(ItemCheckEventArgs ice)
        {
            if (CheckReadOnly)
                ice.NewValue = ice.CurrentValue;

            base.OnItemCheck(ice);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (SelectionMode == SelectionMode.One)
            {
                int index = IndexFromPoint(e.Location);

                if (index == -1)
                {
                    if (SelectedIndex == Items.Count - 1)
                        SelectedIndex = ListBox.NoMatches;
                    else
                        SelectedIndex = Items.Count - 1;
                }
            }

            base.OnMouseClick(e);
        }

        public void Recalculate()
        {
            Node = Node;
        }

        private void OnBoardChanged(SudokuNumber a_number)
        {
            Node.RemoveNodes();
            Recalculate();
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SudokuSolutionNode Node
        {
            get
            {
                return m_node;
            }
            set
            {
                SudokuSolution selected_solution = SelectedSolution;

                if (m_node != null)
                    m_node.Board.BoardChanged -= OnBoardChanged;
                m_node = value;
                m_node.Board.BoardChanged += OnBoardChanged;

                CheckReadOnly = false;

                Items.Clear();
                m_selectedIndex = -1;

                Node.StepAll();
  
                if (Node.State == SudokuSolutionNodeState.State)
                {
                    foreach (SudokuSolutionNode node in m_node.Nodes)
                    {
                        bool check = false;
                        if (node.Nodes.Any())
                            if (node.Nodes.ElementAt(0).Nodes.Any())
                                check = true;

                        Items.Add(node.Solution, check);
                    }
                }

                CheckReadOnly = true;

                if (selected_solution != null)
                {
                    SelectedItem = Items.Cast<SudokuSolution>().FirstOrDefault(s => s.Equals(selected_solution));

                    if (SelectedItem == null)
                        OnSelectedIndexChanged(EventArgs.Empty);
                }
            }
        }
    }
}
