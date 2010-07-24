using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SudokuLib;
using System.Drawing;

namespace SudokuLINQSolver
{
    public partial class SolutionsTree : TreeView
    {
        public SolutionsTree()
        {
            InitializeComponent();

            ShowRootLines = false;
            DrawMode = TreeViewDrawMode.OwnerDrawText;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SudokuSolutionNode SelectedSolutionNode
        {
            get
            {
                return TreeNodeToSolutionNode(SelectedNode);
            }
            set
            {
                SelectedNode = SolutionNodeToTreeNode(value);
            }
        }

        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            base.OnDrawNode(e);

            if ((e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds);
                e.Graphics.DrawString(e.Node.Text, Font, new SolidBrush(SystemColors.HighlightText),
                    new PointF(e.Bounds.Left, e.Bounds.Top));
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(e.Node.BackColor), e.Bounds);
                e.Graphics.DrawString(e.Node.Text, Font, new SolidBrush(ForeColor),
                    new PointF(e.Bounds.Left, e.Bounds.Top));
            }
        }

        public void InitTree(SudokuBoard a_board)
        {
            UpdateTree(SudokuSolutionNode.CreateRoot(a_board));
        }

        public void UpdateTree(SudokuSolutionNode a_root, SudokuSolutionNode a_select = null)
        {
            Debug.Assert(a_root.Parent == null);

            BeginUpdate();

            Nodes.Clear();
            UpdateNode(a_root, null);

            EndUpdate();

            if ((a_select != null) && (SolutionNodeToTreeNode(a_select) != null))
                SelectedNode = SolutionNodeToTreeNode(a_select);
            else
                SelectedNode = Nodes.Cast<TreeNode>().Last();
        }

        private void UpdateNode(SudokuSolutionNode a_node, TreeNode a_treeNode)
        {
            if (a_node == null)
                return;

            a_treeNode = AddNextNode(a_treeNode, a_node);

            if (a_node.Nodes.Any(n => n.Nodes.Any()))
                a_node = a_node.Nodes.First(n => n.Nodes.Any());
            else
                a_node = a_node.Nodes.FirstOrDefault();

            UpdateNode(a_node, a_treeNode);
        }

        private TreeNode AddNextNode(TreeNode a_treeNode, SudokuSolutionNode a_solutionNode)
        {
            TreeNode node = null;
            node = GetParentCollection(a_treeNode).Insert(GetParentCollection(a_treeNode).IndexOf(a_treeNode) + 1, a_solutionNode.Info);
            node.Tag = a_solutionNode;
            ColorNode(node);
            return node;
        }

        private void ColorNode(TreeNode a_node)
        {
            SudokuSolutionNode node = TreeNodeToSolutionNode(a_node);

            if (node.State == SudokuSolutionNodeState.Solution)
            {
                if ((node.Solution.Type != SudokuSolutionType.MarkImpossibles) && 
                    (node.Solution.Type != SudokuSolutionType.MarkSolved) && 
                    (node.Solution.Type != SudokuSolutionType.SinglesInUnit))
                {
                    a_node.BackColor = Color.LightBlue;
                }
            }    
        }

        public void ResetBoard()
        {
            InitTree(SelectedSolutionNode.Root.Board);
        }

        public void Solve(SudokuSolution a_solution = null)
        {
            if (a_solution == null)
                a_solution = SelectedSolutionNode.Nodes.First().Solution;

            SudokuSolutionNode solution_node = TreeNodeToSolutionNode(SelectedNode);
            solution_node = solution_node.Nodes.FirstOrDefault(n => Object.ReferenceEquals(n.Solution, a_solution));

            if (solution_node != null)
                solution_node.Parent.RemoveNodes(solution_node);

            solution_node.Solve();

            UpdateTree(solution_node.Root);
        }

        private TreeNodeCollection GetParentCollection(TreeNode a_node)
        {
            if (a_node == null)
                return Nodes;
            if (a_node.Parent == null)
                return Nodes;
            else
                return a_node.Parent.Nodes;
        }

        public void Step(SudokuSolutionNode a_node)
        {
            if (a_node == null)
            {
                a_node = SelectedSolutionNode.Step();
                if (a_node == null)
                    return;
            }

            a_node.Parent.RemoveNodes(a_node);
            UpdateTree(a_node.Root, a_node);
        }

        public SudokuSolutionNode TreeNodeToSolutionNode(TreeNode a_node)
        {
            if (a_node == null)
                return null;

            return (SudokuSolutionNode)a_node.Tag;
        }

        public TreeNode SolutionNodeToTreeNode(SudokuSolutionNode a_node)
        {
            foreach (var node in Nodes.Cast<TreeNode>())
            {
                if (TreeNodeToSolutionNode(node).Equals(a_node))
                    return node;
            }

            return null;
        }

        public void RestoreTreeState(SolutionsTreeState a_state)
        {
            BeginUpdate();

            Nodes.Clear();

            TreeNode tree_node = AddNextNode(null, a_state.Nodes.First());

            foreach (var solution_node in a_state.Nodes.Skip(1))
                tree_node = AddNextNode(tree_node, solution_node);

            TreeNodeToSolutionNode(tree_node).RemoveNodes();

            EndUpdate();
        }
    }
}