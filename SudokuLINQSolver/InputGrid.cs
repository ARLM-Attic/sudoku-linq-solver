using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SudokuLib;
using SudokuLINQSolver.Actions;
using UndoRedoLib;

namespace SudokuLINQSolver
{
    public partial class InputGrid : Control
    {
        private SudokuBoard m_board;

        public const float CELL_SIZE = 20;
        private const float FONT_SIZE = 12;
        private const float LINE_1 = 1;
        private const float LINE_2 = 2;
        private const float OFFSET = 4;
        private const float CELL_OFFSET_X = 1;
        private const float CELL_OFFSET_Y = 1;

        private Point m_focusCell = new Point(0, 0);

        public InputGrid()
        {
            Board = new SudokuBoard();

            InitializeComponent();
            DoubleBuffered = true;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Point FocusCell
        {
            get
            {
                return m_focusCell;
            }
            set
            {
                m_focusCell = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SudokuBoard Board
        {
            get
            {
                return m_board;
            }
            set
            {
                m_board = value;

                Board.BoardChanged += (number) => Invalidate();

                Invalidate();
            }
        }

        protected override void OnResize(EventArgs a_e)
        {
            base.OnResize(a_e);
            Invalidate();
        }

        protected override bool IsInputKey(Keys a_keyData)
        {
            if ((a_keyData == Keys.Down) || (a_keyData == Keys.Up) || (a_keyData == Keys.Left) || (a_keyData == Keys.Right))
                return true;

            return base.IsInputKey(a_keyData);
        }

        private SudokuCell SelectedCell
        {
            get
            {
                return m_board[m_focusCell.X, m_focusCell.Y];
            }
        }

        protected override void OnKeyDown(KeyEventArgs a_e)
        {
            base.OnKeyDown(a_e);

            if (!Enabled)
                return;

            switch (a_e.KeyCode)
            {
                case Keys.Down: m_focusCell.Offset(0, 1); break;
                case Keys.Up: m_focusCell.Offset(0, -1); break;
                case Keys.Right: m_focusCell.Offset(1, 0); break;
                case Keys.Left: m_focusCell.Offset(-1, 0); break;
                case Keys.Delete:
                case Keys.Back:
                {
                    if (m_board[m_focusCell.X, m_focusCell.Y].Numbers().Any(num => num.State != SudokuNumberState.sudokucellstatePossible))
                    {
                        CellClearAction action = new CellClearAction(SelectedCell);
                        UndoRedoManager.Instance.SaveAndExecute(action);
                    }
                    break;
                }
            }

            if ((a_e.KeyCode >= Keys.D1) && (a_e.KeyCode <= Keys.D9))
            {
                int num = a_e.KeyCode - Keys.D0 - 1;

                SudokuBoard board = new SudokuBoard(m_board);
                board[m_focusCell.X, m_focusCell.Y][num].State = SudokuNumberState.sudokucellstateManualEntered;

                if (board.IsSolvable)
                {
                    CellNumberEntered action = new CellNumberEntered(SelectedCell, num);
                    UndoRedoManager.Instance.SaveAndExecute(action);
                }
                else
                {
                    System.Media.SystemSounds.Beep.Play();
                }
            }

            m_focusCell =  CorrectCellPos(m_focusCell);

            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs a_e)
        {
            base.OnMouseClick(a_e);

            if (!Enabled)
                return;

            int s;
            if (ClientRectangle.Width > ClientRectangle.Height)
                s = ClientRectangle.Height;
            else
                s = ClientRectangle.Width;

            int cell_size = s / SudokuBoard.SIZE;

            Point cell_pos = new Point(a_e.X / cell_size, a_e.Y / cell_size);

            if (CorrectCellPos(cell_pos) != cell_pos)
                return;

            m_focusCell = cell_pos;

            Focus();
            Invalidate();
        }

        private Point CorrectCellPos(Point a_cell_pos)
        {
            return new Point(
                Limit(a_cell_pos.X, 0, SudokuBoard.SIZE - 1),
                Limit(a_cell_pos.Y, 0, SudokuBoard.SIZE - 1));
        }

        private int Limit(int a_value, int a_min, int a_max)
        {
             return Math.Min(Math.Max(a_value, a_min), a_max);
        }

        protected override void OnGotFocus(EventArgs a_e)
        {
            base.OnGotFocus(a_e);

            Invalidate();
        }

        protected override void OnLostFocus(EventArgs a_e)
        {
            base.OnLostFocus(a_e);

            Invalidate();
        }

        public void PaintOn(Graphics a_graphics, bool a_printing)
        {
            float grid_size = SudokuBoard.SIZE * CELL_SIZE;

            if (Enabled  && !a_printing && !DesignMode)
            {
                a_graphics.FillRectangle(Brushes.LightGreen, CELL_SIZE * m_focusCell.X, CELL_SIZE * m_focusCell.Y,
                    CELL_SIZE, CELL_SIZE);
            }

            for (int i = 0; i <= SudokuBoard.SIZE; i++)
            {
                Pen line_pen;

                if (i % SudokuBoard.BOX_SIZE == 0)
                    line_pen = new Pen(Brushes.Black, LINE_2);
                else
                    line_pen = new Pen(Brushes.Black, LINE_1);

                a_graphics.DrawLine(line_pen, new PointF(-LINE_2 / 2, i * CELL_SIZE), new PointF(grid_size + LINE_2 / 2, i * CELL_SIZE));
                a_graphics.DrawLine(line_pen, new PointF(i * CELL_SIZE, -LINE_2 / 2), new PointF(i * CELL_SIZE, grid_size + LINE_2 / 2));
            }

            Font font = new Font("Microsoft Sans Serif", FONT_SIZE, FontStyle.Regular);

            (from cell in m_board.Cells()
             where cell.IsSolved
             from num in cell.Numbers()
             where num.IsSolved
             select num).ForEach(num =>
             {
                 Brush brush;
                 if (num.State == SudokuNumberState.sudokucellstateManualEntered)
                     brush = new SolidBrush(Colors.SOLVED_NUMBER);
                 else
                     brush = new SolidBrush(SolverGrid.NUMBER_COLOR);

                 string str = num.Number.ToString();

                 SizeF size = a_graphics.MeasureString(str, font);

                 a_graphics.DrawString(str, font, brush,
                     new PointF(CELL_SIZE * num.Col + CELL_OFFSET_X + (CELL_SIZE - size.Width) / 2, CELL_SIZE * num.Row + CELL_OFFSET_Y));
             });
        }

        protected override void OnPaint(PaintEventArgs a_pe)
        {
            base.OnPaint(a_pe);

            float s = Math.Min(ClientRectangle.Width, ClientRectangle.Height);
            float size = SudokuBoard.SIZE * CELL_SIZE + (LINE_2 / 2) * 2 + OFFSET * 2;
            float scale = s / size;

            a_pe.Graphics.ScaleTransform(scale, scale);
            a_pe.Graphics.TranslateTransform(OFFSET, OFFSET);

            PaintOn(a_pe.Graphics, false);
        }
    }
}
