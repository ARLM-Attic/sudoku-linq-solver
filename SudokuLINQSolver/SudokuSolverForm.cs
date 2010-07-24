using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SudokuLINQSolver.Configurations;
using System.Xml.Linq;
using System.Media;
using System.IO;
using SudokuLib;
using System.Drawing.Printing;
using System.Diagnostics;
using UndoRedoLib;
using System.Reflection;
using SudokuLINQSolver.Actions;

namespace SudokuLINQSolver
{
    public partial class SudokuSolverForm : Form
    {
        private bool m_bLoading = false;
        private bool m_edit = false;

        public SudokuSolverForm()
        {
            InitializeComponent();

            FormState.Instance.Init(this);
        }

        private void SudokuForm_Load(object a_sender, EventArgs a_e)
        {
            m_bLoading = true;

            UndoRedoManager.Instance.RegisterUndoComponent(undoStripDropDownButton);
            UndoRedoManager.Instance.RegisterRedoComponent(redoStripDropDownButton);

            NavigationManager.Instance.Register(
                editToolStripButton,
                edit =>
                {
                    Edit((bool)edit);  
                },
                () => (object)m_edit);
            NavigationManager.Instance.Register(
                solutionsTree,
                node =>
                {
                    solutionsTree.SelectedSolutionNode = null;
                    solutionsTree.SelectedSolutionNode = (SudokuSolutionNode)node;
                },
                () => solutionsTree.SelectedSolutionNode);
            NavigationManager.Instance.Register(
                solutionsCheckedListBox,
                index => solutionsCheckedListBox.SelectedIndex = (int)index, 
                () => solutionsCheckedListBox.SelectedIndex);
            NavigationManager.Instance.Register(
                inputGrid,
                cell => inputGrid.FocusCell = (Point)cell,
                () => inputGrid.FocusCell);

            for (int i = 0; i < mainToolStrip.Items.Count; i++)
            {
                if (mainToolStrip.Items[i].Image != null)
                {
                    mainToolStrip.Items[i].Image = mainToolStrip.Items[i].Image.GetThumbnailImage(
                        mainToolStrip.ImageScalingSize.Width, mainToolStrip.ImageScalingSize.Height, null, IntPtr.Zero);
                }
            }

            FormState.Instance.LoadState();
            
            #if (!DEBUG)
            addSolutionToolStripButton.Visible = false;
            #endif

            inputGrid.Enabled = false;
            solverGrid.Enabled = false;
            rotateToolStripButton.Enabled = false;
            
            if (Splitters.Instance[splitContainer1.Name].Distance != 0)
                splitContainer1.SplitterDistance = Splitters.Instance[splitContainer1.Name].Distance;
            if (Splitters.Instance[splitContainer2.Name].Distance != 0)
                splitContainer2.SplitterDistance = Splitters.Instance[splitContainer2.Name].Distance;
            if (Splitters.Instance[splitContainer3.Name].Distance != 0)
                splitContainer3.SplitterDistance = Splitters.Instance[splitContainer3.Name].Distance;

            m_bLoading = false;

            Options.Instance.Changed += OnOptionsChanged;
            SudokuOptions.Current = Options.Instance.SudokuOptions;

            LastOpenFiles.Instance.Check();

            if (!LoadFile(LastOpenFiles.Instance.LastOpenFile))
            {
                solutionsTree.InitTree(new SudokuBoard());

                LastOpenFiles.Instance.Opened("");
                openFileDialog.FileName = "";
                saveBoardFileDialog.FileName = "";
                saveIntermediateSolutionFileDialog.FileName = "";
            }
        }

        private void OnLastOpenItemClick(object sender, EventArgs e)
        {
            LoadFile((sender as ToolStripDropDownItem).Text);
        }

        private void SaveBoard(string a_fileName)
        {
            solverGrid.Board.SaveToFile(a_fileName);
           
            Text = "Sudoku - " + a_fileName;

            LastOpenFiles.Instance.Opened(a_fileName);
            openFileDialog.FileName = new FileInfo(a_fileName).Name;
            saveBoardFileDialog.FileName = new FileInfo(a_fileName).Name;
            saveIntermediateSolutionFileDialog.FileName = new FileInfo(a_fileName).Name;
        }

        private bool LoadFile(string a_fileName)
        {
            if (a_fileName.Equals(""))
                return false;

            solutionsCheckedListBox.SelectedSolution = null;

            SudokuBoard board = SudokuBoard.LoadFromFile(a_fileName);
            if (board != null)
            {
                Cursor.Current = Cursors.WaitCursor;

                try
                {
                    solutionsTree.InitTree(board);
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
            else
            {
                SudokuIntermediateSolution intermediate_solution = SudokuIntermediateSolution.LoadFromFile(a_fileName);

                if (intermediate_solution == null)
                {
                    MessageBox.Show("Loaded file is invalid", "Sudoku", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                else
                {
                    Cursor.Current = Cursors.WaitCursor;

                    try
                    {
                        solutionsTree.InitTree(intermediate_solution.Before);
                        solutionsCheckedListBox.SelectedSolution =
                            solutionsCheckedListBox.Node.Nodes.Select(n => n.Solution).FirstOrDefault(s => s.Equals(intermediate_solution.Solution));      
                    }
                    finally
                    {
                        Cursor.Current = Cursors.Default;
                    }
                }
            }

            LastOpenFiles.Instance.Opened(a_fileName);
            openFileDialog.FileName = new FileInfo(a_fileName).Name;
            saveBoardFileDialog.FileName = new FileInfo(a_fileName).Name;
            saveIntermediateSolutionFileDialog.FileName = new FileInfo(a_fileName).Name;

            Text = "Sudoku - " + a_fileName;

            UndoRedoManager.Instance.Clear();

            return true;
        }

        private void openToolStripSplitButton_DropDownOpening(object sender, EventArgs e)
        {
            LastOpenFiles.Instance.Check();
       
            ToolStripDropDownItem ddi = sender as ToolStripDropDownItem;
            ddi.DropDown.Items.Clear();

            foreach (string s in LastOpenFiles.Instance.RecentFiles.FileNames)
            {
                ToolStripItem tti = ddi.DropDown.Items.Add(s);
                tti.Click += new EventHandler(OnLastOpenItemClick);
            }
        }

        private void openToolStripSplitButton_ButtonClick(object sender, EventArgs e)
        {
            Open();
        }

        private void Open()
        {
            LastOpenFiles.Instance.Check();

            openFileDialog.InitialDirectory = LastOpenFiles.Instance.LastOpenDir;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
                LoadFile(openFileDialog.FileName);
        }

        private void solveToolStripButton_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                SolveAction action = new SolveAction(solutionsTree, solutionsCheckedListBox.SelectedSolutionNode);
                UndoRedoManager.Instance.SaveAndExecute(action);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void Restart()
        {
            RestartAction action = new RestartAction(solutionsTree, solutionsCheckedListBox.SelectedSolutionNode);
            UndoRedoManager.Instance.SaveAndExecute(action);
        }

        private void restartToolStripButton_Click(object sender, EventArgs e)
        {
            Restart();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            New();
        }

        private void New()
        {
            UndoRedoManager.Instance.Clear();
            solutionsTree.InitTree(new SudokuBoard());
            Text = "Sudoku Solver";
            LastOpenFiles.Instance.LastOpenFile = "";
        }

        private void SaveBoard()
        {
            LastOpenFiles.Instance.Check();

            saveBoardFileDialog.InitialDirectory = LastOpenFiles.Instance.LastOpenDir;

            if (saveBoardFileDialog.ShowDialog() == DialogResult.OK)
                SaveBoard(saveBoardFileDialog.FileName);
        }

        private void stepToolStripButton_Click(object sender, EventArgs e)
        {
            Step();
        }

        private void Step()
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                StepAction action = new StepAction(solutionsTree, solutionsCheckedListBox.SelectedSolutionNode);
                UndoRedoManager.Instance.SaveAndExecute(action);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void splitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (!m_bLoading)
            {
                SplitContainer sc = sender as SplitContainer;
                Splitters.Instance[sc.Name].Distance = sc.SplitterDistance;
            }
        }

        private void solutionsCheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (solutionsCheckedListBox.SelectedSolutionNode != null)
                solverGrid.Solution = solutionsCheckedListBox.SelectedSolutionNode.Solution;
            else if (solutionsTree.SelectedSolutionNode.State == SudokuSolutionNodeState.Solution)
                solverGrid.Solution = solutionsTree.SelectedSolutionNode.Solution;
            else
                solverGrid.Solution = null;
        }

        private void printToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                FileInfo opened_file = new FileInfo(LastOpenFiles.Instance.LastOpenFile);
                string docName = opened_file.Name.Substring(0, opened_file.Name.Length - opened_file.Extension.Length);
                printDocument.DocumentName = docName;
            }
            catch 
            {
                printDocument.DocumentName = "";
            }

            pageSetupDialog.Document = printDocument;
            pageSetupDialog.PrinterSettings.PrintFileName = printDocument.DocumentName;

            SudokuLINQSolver.Configurations.PageSettings.Instance.Restore(pageSetupDialog.PageSettings);

            if (pageSetupDialog.ShowDialog() == DialogResult.OK)
            {

                SudokuLINQSolver.Configurations.PageSettings.Instance.Save(pageSetupDialog.PageSettings);

                printDialog.Document = pageSetupDialog.Document;

                if (printDialog.ShowDialog() == DialogResult.OK)
                    printDialog.Document.Print();
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            int printHeight = printDocument.DefaultPageSettings.PaperSize.Height - printDocument.DefaultPageSettings.Margins.Top -
                printDocument.DefaultPageSettings.Margins.Bottom;
            int printWidth = printDocument.DefaultPageSettings.PaperSize.Width - printDocument.DefaultPageSettings.Margins.Left -
                printDocument.DefaultPageSettings.Margins.Right;
            int printLeft = printDocument.DefaultPageSettings.Margins.Left;
            int printTop = printDocument.DefaultPageSettings.Margins.Top;

            int printSize = Math.Min(printWidth, printHeight);

            float size = SudokuBoard.SIZE * InputGrid.CELL_SIZE;
            float scale = printSize / size;

            e.Graphics.TranslateTransform(printLeft, printTop);
            e.Graphics.ScaleTransform(scale, scale);

            inputGrid.PaintOn(e.Graphics, true);

            e.HasMorePages = false;
        }

        private void rotateToolStripButton_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                RotateAction action = new RotateAction(solutionsTree, solutionsCheckedListBox, solutionsCheckedListBox.SelectedSolutionNode);
                UndoRedoManager.Instance.SaveAndExecute(action);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void solutionsTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Cursor old = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                solverGrid.Solution = solutionsTree.SelectedSolutionNode.Solution;
                solverGrid.Board = solutionsTree.SelectedSolutionNode.Board;
                inputGrid.Board = solutionsTree.SelectedSolutionNode.Board;
                solutionsCheckedListBox.Node = solutionsTree.SelectedSolutionNode;

                if (solutionsCheckedListBox.Items.Count > 0)
                {
                    if (solutionsCheckedListBox.SelectedSolution == null)
                    {
                        solutionsCheckedListBox.SelectedSolution = solutionsTree.SelectedSolutionNode.Nodes.First().Solution;
                    }
                }

                stepToolStripButton.Enabled =
                    (solutionsTree.SelectedSolutionNode.State == SudokuSolutionNodeState.Solution) ||
                    (solutionsTree.SelectedSolutionNode.State == SudokuSolutionNodeState.State);
                stepToolStripButton.Enabled = stepToolStripButton.Enabled && !editToolStripButton.Checked;
                solveToolStripButton.Enabled = stepToolStripButton.Enabled;
            }
            finally
            {
                Cursor.Current = old;
            }
        }

        private void Edit(bool a_edit)
        {
            m_edit = a_edit;

            editToolStripButton.Checked = a_edit;

            inputGrid.Enabled = a_edit;
            solverGrid.Enabled = a_edit;
            rotateToolStripButton.Enabled = a_edit;

            solveToolStripButton.Enabled = !a_edit;
            stepToolStripButton.Enabled = !a_edit;
            restartToolStripButton.Enabled = !a_edit;    
        }

        private void Undo()
        {
            UndoRedoManager.Instance.Undo();
        }

        private void Redo()
        {
            UndoRedoManager.Instance.Redo();
        }

        private void editToolStripButton_Click(object sender, EventArgs e)
        {
            Edit();
        }

        private void Edit()
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                EditAction action = new EditAction(solutionsTree, solutionsCheckedListBox.SelectedSolutionNode, m_edit, Edit);
                UndoRedoManager.Instance.SaveAndExecute(action);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private SudokuIntermediateSolution GetIntermediateSolution()
        {
            if (solutionsTree.SelectedSolutionNode.State == SudokuSolutionNodeState.Solution)
            {
                return new SudokuIntermediateSolution(solutionsTree.SelectedSolutionNode.Board,
                    solutionsTree.SelectedSolutionNode.NextBoard, solutionsTree.SelectedSolutionNode.Solution);
            }
            else if (solutionsCheckedListBox.SelectedSolutionNode != null)
            {
                return new SudokuIntermediateSolution(solutionsTree.SelectedSolutionNode.Board, solutionsCheckedListBox.SelectedSolutionNode.NextBoard,
                    solutionsCheckedListBox.SelectedSolutionNode.Solution);
            }
            else
            {
                return new SudokuIntermediateSolution(solutionsTree.SelectedSolutionNode.Board, null, null);
            }
            
        }

        private void addSolutionToolStripButton_Click(object sender, EventArgs e)
        {
            FileInfo[] files = new DirectoryInfo(Directories.Solutions).GetFiles(FileExtensions.XmlZipMask);

            int index = 1;
            string fileName;
            do
            {
                fileName = Directories.Solutions + Path.DirectorySeparatorChar + solverGrid.Solution.Type + (index <10 ? "0" : "")  + 
                    index + FileExtensions.XmlZipExt;
                index++;
            }
            while (new FileInfo(fileName).Exists);

            GetIntermediateSolution().SaveToFile(fileName);
         
        }

        private void SudokuSolverForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.O)
            {
                Open();
                e.SuppressKeyPress = true;
            }

            if (e.Control && e.KeyCode == Keys.S)
            {
                SaveBoard();
                e.SuppressKeyPress = true;
            }

            if (e.Control && e.KeyCode == Keys.C)
            {
                CopyToClipboard();
                e.SuppressKeyPress = true;
            }

            if (e.Control && e.KeyCode == Keys.V)
            {
                PasteFromClipboard();
                e.SuppressKeyPress = true;
            }

            if (e.Control && e.KeyCode == Keys.Z)
            {
                if (undoStripDropDownButton.Enabled)
                    Undo();
                e.SuppressKeyPress = true;
            }

            if (e.Control && e.KeyCode == Keys.Y)
            {
                if (redoStripDropDownButton.Enabled)
                    Undo();
                e.SuppressKeyPress = true;
            }

            if (e.Control && e.KeyCode == Keys.E)
            {
                Edit();
                e.SuppressKeyPress = true;
            }

            if (e.Control && e.KeyCode == Keys.R)
            {
                Restart();
                e.SuppressKeyPress = true;
            }

            if (e.Control && e.KeyCode == Keys.N)
            {
                New();
                e.SuppressKeyPress = true;
            }
        }

        private void CopyToClipboard()
        {
            Clipboard.SetText(solverGrid.Board.GetAsText());
        }

        private void PasteFromClipboard()
        {
            if (!Clipboard.ContainsText())
                return;

            SudokuBoard board = new SudokuBoard(Clipboard.GetText());
            if (board != null)
            {
                Cursor.Current = Cursors.WaitCursor;

                try
                {
                    solutionsTree.InitTree(board);
                    UndoRedoManager.Instance.Clear();
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            } 
        }

        private void solverGrid_SolutionChanged()
        {
            addSolutionToolStripButton.Enabled = (solverGrid.Solution != null);
        }

        private void saveBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveBoard();
        }

        private void saveIntermediateSolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveIntermediateSolution();
        }

        private void SaveIntermediateSolution()
        {
            LastOpenFiles.Instance.Check();

            saveIntermediateSolutionFileDialog.InitialDirectory = LastOpenFiles.Instance.LastOpenDir;

            if (saveIntermediateSolutionFileDialog.ShowDialog() == DialogResult.OK)
                SaveIntermediateSolution(saveIntermediateSolutionFileDialog.FileName);
        }

        private void SaveIntermediateSolution(string a_fileName)
        {
            GetIntermediateSolution().SaveToFile(a_fileName);

            Text = "Sudoku - " + a_fileName;

            LastOpenFiles.Instance.Opened(a_fileName);
            openFileDialog.FileName = new FileInfo(a_fileName).Name;
            saveIntermediateSolutionFileDialog.FileName = new FileInfo(a_fileName).Name;
        }

        private void optionsToolStripButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Clicks > 0)
            {
                if (e.Button == MouseButtons.Left)
                {
                    showAllSolutionsToolStripMenuItem.Checked = Options.Instance.ShowAllSolutions;
                    includeBoxesToolStripMenuItem.Checked = Options.Instance.IncludeBoxes;

                    Point p = mainToolStrip.PointToScreen(e.Location);
                    p.Offset(optionsToolStripButton.Bounds.Left, 0);
                    optionsContextMenuStrip.Show(p);
                }
            }
        }

        private void showAllSolutionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options.Instance.ShowAllSolutions = !Options.Instance.ShowAllSolutions;
            RefreshBoard();
        }

        private void includeBoxesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options.Instance.IncludeBoxes = !Options.Instance.IncludeBoxes;
            RefreshBoard();
        }

        private void OnOptionsChanged()
        {
            SudokuOptions.Current = Options.Instance.SudokuOptions;
        }

        private void RefreshBoard()
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                UndoRedoManager.Instance.Clear();

                SudokuSolutionNode selected_node = solutionsTree.SelectedSolutionNode;
                SudokuSolution selected_sol =  solutionsCheckedListBox.SelectedSolution;

                foreach (TreeNode node in solutionsTree.Nodes)
                {
                    SudokuSolutionNode sol_node = solutionsTree.TreeNodeToSolutionNode(node);
                    if (sol_node.State == SudokuSolutionNodeState.Solution)
                        sol_node.Parent.RemoveNodes(sol_node);
                }

                SudokuSolutionNode last = solutionsTree.TreeNodeToSolutionNode(solutionsTree.Nodes.Cast<TreeNode>().Last());
                last.RemoveNodes();
                SudokuSolutionNode root = SudokuSolutionNode.CreateTree(solutionsTree.SelectedSolutionNode.Root);

                solutionsTree.UpdateTree(root, solutionsTree.SelectedSolutionNode);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void solutionsCheckedListBox_DoubleClick(object sender, EventArgs e)
        {
            Step();
        }

        private void undoToolStripButton_ButtonClick(object sender, EventArgs e)
        {
            Undo();
        }

        private void redoToolStripButton_ButtonClick(object sender, EventArgs e)
        {
            Redo();
        }
    }
}