namespace SudokuLINQSolver
{
    partial class SudokuSolverForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SudokuSolverForm));
            this.mainToolStrip = new System.Windows.Forms.ToolStrip();
            this.newToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.openToolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.printToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.optionsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.undoStripDropDownButton = new System.Windows.Forms.ToolStripSplitButton();
            this.redoStripDropDownButton = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.restartToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.solveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.stepToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.editToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.rotateToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.addSolutionToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveBoardFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.inputGrid = new SudokuLINQSolver.InputGrid();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.solverGrid = new SudokuLINQSolver.SolverGrid();
            this.panel3 = new System.Windows.Forms.Panel();
            this.solutionsCheckedListBox = new SudokuLINQSolver.SolutionsCheckedListBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.solutionsTree = new SudokuLINQSolver.SolutionsTree();
            this.printDocument = new System.Drawing.Printing.PrintDocument();
            this.printDialog = new System.Windows.Forms.PrintDialog();
            this.pageSetupDialog = new System.Windows.Forms.PageSetupDialog();
            this.saveContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            this.saveBoardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveIntermediateSolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveIntermediateSolutionFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.optionsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            this.showAllSolutionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.includeBoxesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainToolStrip.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.saveContextMenuStrip.SuspendLayout();
            this.optionsContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainToolStrip
            // 
            this.mainToolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripButton,
            this.openToolStripSplitButton,
            this.saveToolStripButton,
            this.toolStripSeparator1,
            this.printToolStripButton,
            this.optionsToolStripButton,
            this.toolStripSeparator2,
            this.undoStripDropDownButton,
            this.redoStripDropDownButton,
            this.toolStripSeparator4,
            this.restartToolStripButton,
            this.solveToolStripButton,
            this.stepToolStripButton,
            this.toolStripSeparator3,
            this.editToolStripButton,
            this.rotateToolStripButton,
            this.addSolutionToolStripButton});
            this.mainToolStrip.Location = new System.Drawing.Point(0, 0);
            this.mainToolStrip.Name = "mainToolStrip";
            this.mainToolStrip.Size = new System.Drawing.Size(1209, 39);
            this.mainToolStrip.TabIndex = 4;
            this.mainToolStrip.Text = "toolStrip";
            // 
            // newToolStripButton
            // 
            this.newToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripButton.Image")));
            this.newToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newToolStripButton.Name = "newToolStripButton";
            this.newToolStripButton.Size = new System.Drawing.Size(67, 36);
            this.newToolStripButton.Text = "New";
            this.newToolStripButton.Click += new System.EventHandler(this.newToolStripButton_Click);
            // 
            // openToolStripSplitButton
            // 
            this.openToolStripSplitButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripSplitButton.Image")));
            this.openToolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripSplitButton.Name = "openToolStripSplitButton";
            this.openToolStripSplitButton.Size = new System.Drawing.Size(84, 36);
            this.openToolStripSplitButton.Text = "Open";
            this.openToolStripSplitButton.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.openToolStripSplitButton.ToolTipText = "Open";
            this.openToolStripSplitButton.ButtonClick += new System.EventHandler(this.openToolStripSplitButton_ButtonClick);
            this.openToolStripSplitButton.DropDownOpening += new System.EventHandler(this.openToolStripSplitButton_DropDownOpening);
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.DropDown = this.saveContextMenuStrip;
            this.saveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripButton.Image")));
            this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Size = new System.Drawing.Size(76, 36);
            this.saveToolStripButton.Text = "Save";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // printToolStripButton
            // 
            this.printToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("printToolStripButton.Image")));
            this.printToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.printToolStripButton.Name = "printToolStripButton";
            this.printToolStripButton.Size = new System.Drawing.Size(68, 36);
            this.printToolStripButton.Text = "Print";
            this.printToolStripButton.Click += new System.EventHandler(this.printToolStripButton_Click);
            // 
            // optionsToolStripButton
            // 
            this.optionsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("optionsToolStripButton.Image")));
            this.optionsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.optionsToolStripButton.Name = "optionsToolStripButton";
            this.optionsToolStripButton.Size = new System.Drawing.Size(85, 36);
            this.optionsToolStripButton.Text = "Options";
            this.optionsToolStripButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.optionsToolStripButton_MouseUp);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 39);
            // 
            // undoStripDropDownButton
            // 
            this.undoStripDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("undoStripDropDownButton.Image")));
            this.undoStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.undoStripDropDownButton.Name = "undoStripDropDownButton";
            this.undoStripDropDownButton.Size = new System.Drawing.Size(84, 36);
            this.undoStripDropDownButton.Text = "Undo";
            this.undoStripDropDownButton.ButtonClick += new System.EventHandler(this.undoToolStripButton_ButtonClick);
            // 
            // redoStripDropDownButton
            // 
            this.redoStripDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("redoStripDropDownButton.Image")));
            this.redoStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.redoStripDropDownButton.Name = "redoStripDropDownButton";
            this.redoStripDropDownButton.Size = new System.Drawing.Size(82, 36);
            this.redoStripDropDownButton.Text = "Redo";
            this.redoStripDropDownButton.ButtonClick += new System.EventHandler(this.redoToolStripButton_ButtonClick);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 39);
            // 
            // restartToolStripButton
            // 
            this.restartToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("restartToolStripButton.Image")));
            this.restartToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.restartToolStripButton.Name = "restartToolStripButton";
            this.restartToolStripButton.Size = new System.Drawing.Size(79, 36);
            this.restartToolStripButton.Text = "Restart";
            this.restartToolStripButton.Click += new System.EventHandler(this.restartToolStripButton_Click);
            // 
            // solveToolStripButton
            // 
            this.solveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("solveToolStripButton.Image")));
            this.solveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.solveToolStripButton.Name = "solveToolStripButton";
            this.solveToolStripButton.Size = new System.Drawing.Size(71, 36);
            this.solveToolStripButton.Text = "Solve";
            this.solveToolStripButton.ToolTipText = "Solve";
            this.solveToolStripButton.Click += new System.EventHandler(this.solveToolStripButton_Click);
            // 
            // stepToolStripButton
            // 
            this.stepToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("stepToolStripButton.Image")));
            this.stepToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stepToolStripButton.Name = "stepToolStripButton";
            this.stepToolStripButton.Size = new System.Drawing.Size(66, 36);
            this.stepToolStripButton.Text = "Step";
            this.stepToolStripButton.ToolTipText = "Step";
            this.stepToolStripButton.Click += new System.EventHandler(this.stepToolStripButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 39);
            // 
            // editToolStripButton
            // 
            this.editToolStripButton.CheckOnClick = true;
            this.editToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("editToolStripButton.Image")));
            this.editToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editToolStripButton.Name = "editToolStripButton";
            this.editToolStripButton.Size = new System.Drawing.Size(63, 36);
            this.editToolStripButton.Text = "Edit";
            this.editToolStripButton.Click += new System.EventHandler(this.editToolStripButton_Click);
            // 
            // rotateToolStripButton
            // 
            this.rotateToolStripButton.BackColor = System.Drawing.SystemColors.Control;
            this.rotateToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("rotateToolStripButton.Image")));
            this.rotateToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rotateToolStripButton.Name = "rotateToolStripButton";
            this.rotateToolStripButton.Size = new System.Drawing.Size(77, 36);
            this.rotateToolStripButton.Text = "Rotate";
            this.rotateToolStripButton.Click += new System.EventHandler(this.rotateToolStripButton_Click);
            // 
            // addSolutionToolStripButton
            // 
            this.addSolutionToolStripButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.addSolutionToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.addSolutionToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addSolutionToolStripButton.Name = "addSolutionToolStripButton";
            this.addSolutionToolStripButton.Size = new System.Drawing.Size(150, 36);
            this.addSolutionToolStripButton.Text = "Add solution (debug only)";
            this.addSolutionToolStripButton.Click += new System.EventHandler(this.addSolutionToolStripButton_Click);
            // 
            // saveBoardFileDialog
            // 
            this.saveBoardFileDialog.Filter = "Board files (*.xml.zip)|*.xml.zip|Board files (*.txt)|*.txt|All files (*.*)|*.*";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            this.openFileDialog.Filter = "Board files (*.xml.zip)|*.xml.zip|Board files (*.txt)|*.txt|All files (*.*)|*.*";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 39);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            this.splitContainer1.Panel1MinSize = 200;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1209, 425);
            this.splitContainer1.SplitterDistance = 335;
            this.splitContainer1.TabIndex = 10;
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer_SplitterMoved);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.inputGrid);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(335, 425);
            this.panel1.TabIndex = 25;
            // 
            // inputGrid
            // 
            this.inputGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inputGrid.Location = new System.Drawing.Point(5, 5);
            this.inputGrid.Name = "inputGrid";
            this.inputGrid.Size = new System.Drawing.Size(323, 413);
            this.inputGrid.TabIndex = 25;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panel4);
            this.splitContainer2.Size = new System.Drawing.Size(870, 425);
            this.splitContainer2.SplitterDistance = 569;
            this.splitContainer2.TabIndex = 0;
            this.splitContainer2.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer_SplitterMoved);
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.panel2);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.panel3);
            this.splitContainer3.Size = new System.Drawing.Size(569, 425);
            this.splitContainer3.SplitterDistance = 290;
            this.splitContainer3.TabIndex = 0;
            this.splitContainer3.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer_SplitterMoved);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.solverGrid);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3, 3, 7, 7);
            this.panel2.Size = new System.Drawing.Size(569, 290);
            this.panel2.TabIndex = 0;
            // 
            // solverGrid
            // 
            this.solverGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.solverGrid.Location = new System.Drawing.Point(3, 3);
            this.solverGrid.Name = "solverGrid";
            this.solverGrid.Size = new System.Drawing.Size(557, 278);
            this.solverGrid.TabIndex = 2;
            this.solverGrid.Text = "sudokuGrid1";
            this.solverGrid.SolutionChanged += new System.Action(this.solverGrid_SolutionChanged);
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.solutionsCheckedListBox);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(569, 131);
            this.panel3.TabIndex = 0;
            // 
            // solutionsCheckedListBox
            // 
            this.solutionsCheckedListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.solutionsCheckedListBox.Location = new System.Drawing.Point(0, 0);
            this.solutionsCheckedListBox.Name = "solutionsCheckedListBox";
            this.solutionsCheckedListBox.Size = new System.Drawing.Size(567, 129);
            this.solutionsCheckedListBox.Sorted = true;
            this.solutionsCheckedListBox.TabIndex = 0;
            this.solutionsCheckedListBox.SelectedIndexChanged += new System.EventHandler(this.solutionsCheckedListBox_SelectedIndexChanged);
            this.solutionsCheckedListBox.DoubleClick += new System.EventHandler(this.solutionsCheckedListBox_DoubleClick);
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.solutionsTree);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(297, 425);
            this.panel4.TabIndex = 0;
            // 
            // solutionsTree
            // 
            this.solutionsTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.solutionsTree.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.solutionsTree.HideSelection = false;
            this.solutionsTree.Location = new System.Drawing.Point(0, 0);
            this.solutionsTree.Name = "solutionsTree";
            this.solutionsTree.ShowRootLines = false;
            this.solutionsTree.Size = new System.Drawing.Size(295, 423);
            this.solutionsTree.TabIndex = 11;
            this.solutionsTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.solutionsTree_AfterSelect);
            // 
            // printDocument
            // 
            this.printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage);
            // 
            // printDialog
            // 
            this.printDialog.UseEXDialog = true;
            // 
            // saveContextMenuStrip
            // 
            this.saveContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveBoardToolStripMenuItem,
            this.saveIntermediateSolutionToolStripMenuItem});
            this.saveContextMenuStrip.Name = "contextMenuStrip1";
            this.saveContextMenuStrip.Size = new System.Drawing.Size(188, 48);
            // 
            // saveBoardToolStripMenuItem
            // 
            this.saveBoardToolStripMenuItem.Name = "saveBoardToolStripMenuItem";
            this.saveBoardToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.saveBoardToolStripMenuItem.Text = "Board";
            this.saveBoardToolStripMenuItem.Click += new System.EventHandler(this.saveBoardToolStripMenuItem_Click);
            // 
            // saveIntermediateSolutionToolStripMenuItem
            // 
            this.saveIntermediateSolutionToolStripMenuItem.Name = "saveIntermediateSolutionToolStripMenuItem";
            this.saveIntermediateSolutionToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.saveIntermediateSolutionToolStripMenuItem.Text = "Intermediate solution";
            this.saveIntermediateSolutionToolStripMenuItem.Click += new System.EventHandler(this.saveIntermediateSolutionToolStripMenuItem_Click);
            // 
            // saveIntermediateSolutionFileDialog
            // 
            this.saveIntermediateSolutionFileDialog.Filter = "Board files (*.xml.zip)|*.xml.zip";
            // 
            // optionsContextMenuStrip
            // 
            this.optionsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showAllSolutionsToolStripMenuItem,
            this.includeBoxesToolStripMenuItem});
            this.optionsContextMenuStrip.Name = "contextMenuStrip1";
            this.optionsContextMenuStrip.Size = new System.Drawing.Size(404, 48);
            // 
            // showAllSolutionsToolStripMenuItem
            // 
            this.showAllSolutionsToolStripMenuItem.Name = "showAllSolutionsToolStripMenuItem";
            this.showAllSolutionsToolStripMenuItem.Size = new System.Drawing.Size(403, 22);
            this.showAllSolutionsToolStripMenuItem.Text = "Show all solutions";
            this.showAllSolutionsToolStripMenuItem.Click += new System.EventHandler(this.showAllSolutionsToolStripMenuItem_Click);
            // 
            // includeBoxesToolStripMenuItem
            // 
            this.includeBoxesToolStripMenuItem.Name = "includeBoxesToolStripMenuItem";
            this.includeBoxesToolStripMenuItem.Size = new System.Drawing.Size(403, 22);
            this.includeBoxesToolStripMenuItem.Text = "XWing, JellyFish, SwordFish, Multivalue-XWing - Include boxes";
            this.includeBoxesToolStripMenuItem.Click += new System.EventHandler(this.includeBoxesToolStripMenuItem_Click);
            // 
            // SudokuSolverForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1209, 464);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.mainToolStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "SudokuSolverForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Sudoku Solver";
            this.Load += new System.EventHandler(this.SudokuForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SudokuSolverForm_KeyDown);
            this.mainToolStrip.ResumeLayout(false);
            this.mainToolStrip.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.saveContextMenuStrip.ResumeLayout(false);
            this.optionsContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip mainToolStrip;
        private System.Windows.Forms.ToolStripSplitButton openToolStripSplitButton;
        private System.Windows.Forms.SaveFileDialog saveBoardFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStripButton stepToolStripButton;
        private System.Windows.Forms.ToolStripButton solveToolStripButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.ToolStripButton newToolStripButton;
        private System.Windows.Forms.ToolStripDropDownButton saveToolStripButton;
        private System.Windows.Forms.ToolStripButton restartToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Panel panel1;
        private InputGrid inputGrid;
        private System.Windows.Forms.Panel panel2;
        private SolverGrid solverGrid;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private SolutionsTree solutionsTree;
        private SolutionsCheckedListBox solutionsCheckedListBox;
        private System.Windows.Forms.ToolStripButton printToolStripButton;
        private System.Drawing.Printing.PrintDocument printDocument;
        private System.Windows.Forms.PrintDialog printDialog;
        private System.Windows.Forms.PageSetupDialog pageSetupDialog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton rotateToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton editToolStripButton;
        private System.Windows.Forms.ToolStripSplitButton undoStripDropDownButton;
        private System.Windows.Forms.ToolStripSplitButton redoStripDropDownButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton addSolutionToolStripButton;
        private System.Windows.Forms.ContextMenuStrip saveContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem saveBoardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveIntermediateSolutionToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveIntermediateSolutionFileDialog;
        private System.Windows.Forms.ToolStripButton optionsToolStripButton;
        private System.Windows.Forms.ContextMenuStrip optionsContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem showAllSolutionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem includeBoxesToolStripMenuItem;
    }
}

