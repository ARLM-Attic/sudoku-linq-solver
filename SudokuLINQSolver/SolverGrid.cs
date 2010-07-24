using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using SudokuLib;
using SudokuLINQSolver.Actions;
using UndoRedoLib;

namespace SudokuLINQSolver
{
    public partial class SolverGrid : Control
    {
        private const float LINE_1 = 1;
        private const float LINE_2 = 2;
        private const float CELL_SIZE = 21;
        private const float CELL_OFFSET_Y = 1;
        private const float CELL_OFFSET_X = 1;
        private const float FONT_SIZE = 13;
        private const float SMALL_CELL_SIZE = 6;
        private const float SMALL_CELL_OFFSET_X = 0.5f;
        private const float SMALL_CELL_OFFSET_Y = 1;
        private const float SMALL_FONT_SIZE = 3.5f;
        private const float COORD_FONT = 10;
        private const float COORD_SIZE_X = 15;
        private const float COORD_SIZE_Y = 16;
        private const float COORD_OFFSET_X = 0;
        private const float COORD_OFFSET_Y = -5;
        private const float BOTTOM_RIGHT_GAP = LINE_2 * 1.5f;

        public static readonly Color NUMBER_COLOR = Color.Black;

        public event Action SolutionChanged;

        private class SudokuNumberHelper
        {
            public Color Color;
            public SudokuNumber Number;
        }

        public SudokuBoard m_board = new SudokuBoard();
        public SudokuSolution m_solution;
        private Dictionary<SudokuNumber, Rectangle> m_numberToRectMap = new Dictionary<SudokuNumber, Rectangle>();
        private float m_scale;
        
        public SolverGrid()
        {
            InitializeComponent();
            DoubleBuffered = true;
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
                m_board.BoardChanged += number => Invalidate();
               

                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SudokuSolution Solution
        {
            get
            {
                return m_solution;
            }
            set
            {
                m_solution = value;
                Invalidate();

                if (SolutionChanged != null)
                    SolutionChanged();
            }
        }

        private float GetPos(int a_cell)
        {
            int bold = a_cell / SudokuBoard.BOX_SIZE + 1;
            int thin = (a_cell + 1) - bold;

            return bold * LINE_2 + thin * LINE_1 + CELL_SIZE * a_cell;
        }

        protected override void OnResize(EventArgs a_e)
        {
            base.OnResize(a_e);
            Invalidate();
        }

        private float GetSmallPosX(int a_col, int a_number)
        {
            float sposx = GetPos(a_col) + (CELL_SIZE - SMALL_CELL_SIZE * SudokuBoard.BOX_SIZE) / SudokuBoard.BOX_SIZE;

            int xx = (a_number - 1) % SudokuBoard.BOX_SIZE;

            return sposx + xx * SMALL_CELL_SIZE;
        }

        private float GetSmallPosY(int a_row, int a_number)
        {
            float sposy = GetPos(a_row) + (CELL_SIZE - SMALL_CELL_SIZE * SudokuBoard.BOX_SIZE) / SudokuBoard.BOX_SIZE;

            int yy = (a_number - 1) / SudokuBoard.BOX_SIZE;

            return sposy + yy * SMALL_CELL_SIZE;
        }

        protected override void OnMouseDown(MouseEventArgs a_e)
        {
            base.OnMouseDown(a_e);

            if (!Enabled)
                return;

            SudokuNumber number = NumberFromPoint(a_e.Location);

            if (number == null)
                return;

            if (number.Cell.IsSolved)
                return;

            if ((number.State == SudokuNumberState.sudokucellstatePossible) || 
                (number.State == SudokuNumberState.sudokucellstateImpossible))
            {
                NumberChangedAction action = new NumberChangedAction(number);
                UndoRedoManager.Instance.SaveAndExecute(action);
            }
        }

        private SudokuNumber NumberFromPoint(Point a_point)
        {
            PointF p = new PointF(a_point.X / m_scale, a_point.Y / m_scale);
            p = new PointF(p.X - COORD_SIZE_X, p.Y - COORD_SIZE_Y);

            int col = -1;
            int col_num = -1;

            for (int c = 0; c < SudokuBoard.SIZE; c++)
            {
                for (int n = 0; n < SudokuBoard.SIZE; n++)
                {
                    float posx = GetSmallPosX(c, n + 1);

                    if ((p.X >= posx) && (p.X <= posx + SMALL_CELL_SIZE))
                    {
                        col = c;
                        col_num = n % SudokuBoard.BOX_SIZE;
                        break;
                    }
                }
            }

            int row = -1;
            int row_num = -1;

            for (int r = 0; r < SudokuBoard.SIZE; r++)
            {
                for (int n = 0; n < SudokuBoard.SIZE; n++)
                {
                    float posy = GetSmallPosY(r, n + 1);

                    if ((p.Y >= posy) && (p.Y <= posy + SMALL_CELL_SIZE))
                    {
                        row = r;
                        row_num = n / SudokuBoard.BOX_SIZE;
                        break;
                    }
                }
            }

            if ((row == -1) || (col == -1))
                return null;

            return m_board[col, row][row_num * SudokuBoard.BOX_SIZE + col_num];
        }

        protected override void OnPaint(PaintEventArgs a_pe)
        {
            base.OnPaint(a_pe);

            m_numberToRectMap.Clear();

            Font small_font = new Font("Microsoft Sans Serif", SMALL_FONT_SIZE, FontStyle.Regular);
            Font big_font = new Font("Microsoft Sans Serif", FONT_SIZE, FontStyle.Regular);
            Font coord_font = new Font("Microsoft Sans Serif", COORD_FONT, FontStyle.Regular);
      
            float s = Math.Min(ClientRectangle.Width, ClientRectangle.Height);
            float grid_size = SudokuBoard.SIZE * CELL_SIZE + (SudokuBoard.BOX_SIZE * LINE_2 + 1) + 
                              (SudokuBoard.SIZE - SudokuBoard.BOX_SIZE) * LINE_1;
            float size = grid_size + Math.Max(COORD_SIZE_X, COORD_SIZE_Y) + BOTTOM_RIGHT_GAP;

            m_scale = s / size;

            a_pe.Graphics.ScaleTransform(m_scale, m_scale);

            //
            // Coords.
            //

            a_pe.Graphics.TranslateTransform(0, COORD_SIZE_Y);
            
            for (int y = 0; y < SudokuBoard.SIZE; y++)
            {
                String str = ((char)('A' + y)).ToString();

                SizeF coord_size = a_pe.Graphics.MeasureString(str, coord_font);

                float posy = GetPos(y) + (CELL_SIZE - coord_size.Height) / 2;
                float posx = COORD_OFFSET_X + (COORD_SIZE_X - COORD_OFFSET_X - coord_size.Width) / 2;

                a_pe.Graphics.DrawString(str, coord_font, Brushes.Black, posx, posy);

            }

            a_pe.Graphics.TranslateTransform(COORD_SIZE_X, -COORD_SIZE_Y);

            for (int x = 0; x < SudokuBoard.SIZE; x++)
            {
                String str = (x + 1).ToString();

                SizeF coord_size = a_pe.Graphics.MeasureString(str, coord_font);

                float posx = GetPos(x) + (CELL_SIZE - coord_size.Width) / 2;
                float posy = COORD_OFFSET_Y + (COORD_SIZE_Y - COORD_OFFSET_Y - coord_size.Width) / 2;
                
                a_pe.Graphics.DrawString(str, coord_font, Brushes.Black, posx, posy);

            }

            a_pe.Graphics.TranslateTransform(0, COORD_SIZE_Y);

            //
            // Cells colors.
            //

            if (m_solution != null)
            {
                int index = 0;
                foreach (var unit in m_solution.ColorUnits)
                {
                    Brush brush;

                    if (index < m_solution.ColorUnits.Count() / 2)
                        brush = new SolidBrush(Colors.UNIT1);
                    else
                        brush = new SolidBrush(Colors.UNIT2);

                    foreach (var cell in unit)
                    {
                        float posx = GetPos(cell.Col);
                        float posy = GetPos(cell.Row);

                        a_pe.Graphics.FillRectangle(brush, posx, posy, CELL_SIZE + 1, CELL_SIZE + 1);

                        
                    }

                    index++;
                }
            }

            //
            // Color numbers.
            //

            List<SudokuNumberHelper> helpers = new List<SudokuNumberHelper>();

            if (m_solution != null)
            {
                helpers.AddRange((from num in m_solution.Stayed
                                  select new SudokuNumberHelper() { Number = num, Color = Colors.HELPER }));
                helpers.AddRange((from num in m_solution.Solved
                                  select new SudokuNumberHelper() { Number = num, Color = Colors.SOLVED }));
                helpers.AddRange((from num in m_solution.Removed
                                  select new SudokuNumberHelper() { Number = num, Color = Colors.REMOVED }));

                foreach (SudokuNumberHelper h in helpers)
                {
                    float posx = GetSmallPosX(h.Number.Col, h.Number.Number) + SMALL_CELL_OFFSET_X;
                    float posy = GetSmallPosY(h.Number.Row, h.Number.Number) + SMALL_CELL_OFFSET_Y;

                    a_pe.Graphics.FillRectangle(new SolidBrush(h.Color),
                        posx, posy, SMALL_CELL_SIZE, SMALL_CELL_SIZE);
                    a_pe.Graphics.DrawRectangle(Pens.Black,
                        posx, posy, SMALL_CELL_SIZE, SMALL_CELL_SIZE);
                };
            }

            //
            // Usolvable cells.
            //

            foreach (SudokuCell cell in m_board.GetUnsolvableCells())
            {
                Brush brush = new SolidBrush(Colors.REMOVED);

                float posx = GetPos(cell.Col);
                float posy = GetPos(cell.Row);

                a_pe.Graphics.FillRectangle(brush, posx, posy, CELL_SIZE + 1, CELL_SIZE + 1);
            }

            //
            // Numbers.
            //

            for (int x = 0; x < SudokuBoard.SIZE; x++)
            {
                for (int y = 0; y < SudokuBoard.SIZE; y++)
                {
                    SudokuCell cell = m_board[x, y];

                    Brush brush = Brushes.Black;

                    if (cell.IsSolved)
                    {
                        string str = cell.NumberSolved().Number.ToString();
                        brush = (cell.NumberSolved().State == SudokuNumberState.sudokucellstateManualEntered) ?
                            new SolidBrush(Colors.SOLVED_NUMBER) : new SolidBrush(NUMBER_COLOR);

                        SizeF ns = a_pe.Graphics.MeasureString(str, big_font);

                        float posx = GetPos(x) + (CELL_SIZE - ns.Width) / 2 + CELL_OFFSET_X;
                        float posy = GetPos(y) + CELL_OFFSET_Y;
    
                        a_pe.Graphics.DrawString(str, big_font, brush, posx, posy);
                    }
                    else
                    {
                        for (int num = 0; num < SudokuBoard.SIZE; num++)
                        {
                            SudokuNumberState state = cell[num].State;
                            string str = cell[num].Number.ToString();

                            if (state == SudokuNumberState.sudokucellstateImpossible)
                                continue;

                            SizeF ns = a_pe.Graphics.MeasureString(str, small_font);

                            float posx = GetSmallPosX(x, num + 1) + (SMALL_CELL_SIZE - ns.Width) / 2 + SMALL_CELL_OFFSET_X;
                            float posy = GetSmallPosY(y, num + 1) + SMALL_CELL_OFFSET_Y;

                            a_pe.Graphics.DrawString(str, small_font, Brushes.Black, posx, posy);
                        }
                    }
                }
            }

            //
            // Bold lines.
            //

            Pen bold_pen = new Pen(Brushes.Black, LINE_2);

            for (int xy = 0; xy <= SudokuBoard.SIZE; xy = xy + SudokuBoard.BOX_SIZE)
            {
                float posxy = GetPos(xy) - LINE_2 / 2;

                a_pe.Graphics.DrawLine(bold_pen, posxy, 0, posxy, grid_size + LINE_2 / 2);
                a_pe.Graphics.DrawLine(bold_pen, 0, posxy, grid_size + LINE_2 / 2, posxy);
            }

            //
            // Thin lines.
            //

            Pen thin_pen = new Pen(Brushes.Black, LINE_1);

            for (int xy = 1; xy < SudokuBoard.SIZE; xy++)
            {
                if (xy % SudokuBoard.BOX_SIZE == 0)
                    continue;

                float posxy = GetPos(xy) - LINE_1 / 2;

                a_pe.Graphics.DrawLine(thin_pen, posxy, 0, posxy, grid_size);
                a_pe.Graphics.DrawLine(thin_pen, 0, posxy, grid_size, posxy);
            }
        }
    }
}
