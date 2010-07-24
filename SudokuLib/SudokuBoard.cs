using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml.Linq;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.IO.Compression;
using System.Xml;

namespace SudokuLib
{
    // TODO: inne strategie rozwiazanywania
    // TODO: podjac probe analizy drzewa, z uwzglednieniem zapetlen, wyglada na to ze, po rozbiciu drzewa na wiele, one sie pozniej ladnie schodza, 
    //       dodac detekcje podwojnych, a na samym koncu detekcje mozliwych zlaczen galezi (jako permutacje w ramach linii)
    // TODO: dodac mpzliwosc alternatywnej prezentacji wynikow jako C1R1 ? R1C1 zamiast A1
    // TODO: tekstowa interpretacja wyniku - opis formatowany zgodnie z wynikiem, po najechaniu na A1 w opisie, jest ono pdswietlane
    // TODO: dodac obluge lancuchow, zrobic to bazie osobnego algorytmu cp, po znalezieniu wszystkich lancuchow sklasyfikowac je, 
    //       odrzucic rozwiazania znalezione innymi metodami, albo je tez tutaj klasyfikowac
    // TODO: multivalue-xwing, WXYZ-wing - za malo przykladow
    public class SudokuBoard
    {
        private SudokuCell[,] m_board;
       
        public const int SIZE = 9;
        public const int BOX_SIZE = 3;

        public event Action<SudokuNumber> BoardChanged;

        private bool? m_bSolvable;
        private bool? m_bSolved;

        public SudokuBoard()
        {
            m_board = new SudokuCell[SIZE, SIZE];

            for (int col = 0; col < SIZE; col++)
            {
                for (int row = 0; row < SIZE; row++)
                {
                    m_board[col, row] = new SudokuCell(this, col, row);
                }
            }
        }

        public SudokuBoard(SudokuBoard a_board)
            : this()
        {
            Cells().ForEach(cell => cell.CopyFrom(a_board[cell.Col, cell.Row]));
        }

        public SudokuBoard(string a_str)
            : this()
        {
            LoadFromText(a_str);
        }

        internal void OnCellChanged(SudokuNumber a_number)
        {
            m_bSolved = null;
            m_bSolvable = null;

            if (BoardChanged != null)
                BoardChanged(a_number);
        }

        public override bool Equals(object a_obj)
        {
            if (a_obj == null)
                return false;
            if (ReferenceEquals(this, a_obj))
                return true;
            SudokuBoard board = a_obj as SudokuBoard;
            if (board == null)
                return false;

            return Cells().SequenceEqual(board.Cells());
        }

        public override int GetHashCode()
        {
            return Cells().Sum(cell => cell.GetHashCode());
        }

        public override string ToString()
        {
            int x = Cells().Count(cell => cell.IsSolved) * 100 / (9 * 9);

            return String.Format("IsSolved={0}, PercentageSolved={1}%", IsSolved, x);
        }

        public SudokuCell this[int a_colIndex, int a_rowIndex]
        {
            get
            {
                return m_board[a_colIndex, a_rowIndex];
            }
        }

        public IEnumerable<SudokuCell> Cells()
        {
            return (from cell in m_board.Cast<SudokuCell>()
                    select cell);
        }

        public IEnumerable<SudokuNumber> Numbers()
        {
            return (from cell in m_board.Cast<SudokuCell>()
                    from num in cell.Numbers()
                    select num);
        }

        internal IEnumerable<SudokuCell> RowCells(int a_row)
        {
            return (from cell in Cells()
                    where cell.Row == a_row
                    select cell);
        }

        internal IEnumerable<SudokuNumber> RowNumbers(int a_row)
        {
            return (from num in Numbers()
                    where num.Row == a_row
                    select num);
        }

        internal IEnumerable<SudokuCell> ColCells(int a_col)
        {
            return (from cell in Cells()
                    where cell.Col == a_col
                    select cell);
        }

        internal IEnumerable<SudokuNumber> ColNumbers(int a_col)
        {
            return (from num in Numbers()
                    where num.Col == a_col
                    select num);
        }

        internal IEnumerable<SudokuCell> BoxCells(Point a_box)
        {
            return (from cell in Cells()
                    where cell.Box == a_box
                    select cell);
        }

        internal IEnumerable<SudokuNumber> BoxNumbers(Point a_box)
        {
            return (from num in Numbers()
                    where num.Box == a_box
                    select num);
        }

        internal IEnumerable<IEnumerable<SudokuCell>> BoxesCells()
        {
            return from cell in Cells()
                   group cell by cell.Box;
        }

        internal IEnumerable<IEnumerable<SudokuNumber>> BoxesNumbers()
        {
            return from num in Numbers()
                   group num by num.Box;
        }

        internal IEnumerable<IEnumerable<SudokuCell>> RowsCells()
        {
            return from cell in Cells()
                   group cell by cell.Row;
        }

        internal IEnumerable<IEnumerable<SudokuNumber>> RowsNumbers()
        {
            return from num in Numbers()
                   group num by num.Row;
        }

        internal IEnumerable<IEnumerable<SudokuCell>> ColsCells()
        {
            return from cell in Cells()
                   group cell by cell.Col;
        }

        internal IEnumerable<IEnumerable<SudokuNumber>> ColsNumbers()
        {
            return from num in Numbers()
                   group num by num.Col;
        }

        internal IEnumerable<IEnumerable<SudokuCell>> AllCellsUnits()
        {
            return BoxesCells().Concat(RowsCells()).Concat(ColsCells());
        }

        internal IEnumerable<IEnumerable<SudokuNumber>> AllNumbersUnits()
        {
            return BoxesNumbers().Concat(RowsNumbers()).Concat(ColsNumbers());
        }

        private static SudokuBoard LoadFromTextFile(string a_fileName)
        {
            try
            {
                string lines;
                using (StreamReader sr = new StreamReader(new FileStream(a_fileName, FileMode.Open, FileAccess.Read)))
                    lines = sr.ReadToEnd();

                return LoadFromText(lines);
            }
            catch
            {
                return null;
            }
        }

        internal static SudokuBoard LoadFromXML(XElement a_element)
        {
            try
            {
                SudokuBoard board = new SudokuBoard();

                a_element.Elements().ForEach(cell =>
                {
                    Point p = Converters.CoordToPoint(cell.Attribute("coord").Value);
                    int col = p.X;
                    int row = p.Y;

                    cell.Elements().ForEach(num =>
                    {
                        int num_index = Int32.Parse(num.Value) - 1;
                        board[p.X, p.Y][num_index].State = Converters.StringToSudokuNumberState(num.Attribute("state").Value);
                    });
                });

                if (!board.Check())
                    return null;

                return board;
            }
            catch
            {
                return null;
            }
        }

        public static SudokuBoard LoadFromFile(string a_fileName)
        {
            if (Path.GetExtension(a_fileName) == FileExtensions.ZipExt)
            {
                try
                {
                    using (FileStream file_stream = new FileStream(a_fileName, FileMode.Open, FileAccess.Read))
                    {
                        using (GZipStream gzip_stream = new GZipStream(file_stream, CompressionMode.Decompress))
                        {
                            using (StreamReader stream_reader = new StreamReader(gzip_stream, Encoding.ASCII))
                            {
                                return SudokuBoard.LoadFromXML(XElement.Load(stream_reader));
                            }
                        }
                    }
                }
                catch
                {
                    return null;
                }
            }
            else if (Path.GetExtension(a_fileName) == FileExtensions.XmlExt)
            {
                return SudokuBoard.LoadFromXML(XElement.Load(a_fileName));
            }
            else if (Path.GetExtension(a_fileName) == FileExtensions.TxtExt)
            {
                return SudokuBoard.LoadFromTextFile(a_fileName);
            }
            else
                return null;
        }

        public object[] GetAsXML()
        {
            return (from cell in Cells()
                    orderby cell.Row, cell.Col
                    select new XElement("cell", new XAttribute("coord", cell.Coords),
                        from num in cell.Numbers()
                        orderby num.Number
                        select new XElement("number", num.Number, new XAttribute("state", Converters.SudokuNumberStateToString(num.State)))
                    )).ToArray();
        }

        public void SaveToFile(string a_fileName)
        {
            if (Path.GetExtension(a_fileName) == ".txt")
            {
                using (FileStream file_stream = new FileStream(a_fileName, FileMode.Create, FileAccess.ReadWrite))
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(GetAsText());
                    file_stream.Write(bytes, 0, bytes.Length);
                }
            }
            else
            {
                using (FileStream file_stream = new FileStream(a_fileName, FileMode.Create, FileAccess.ReadWrite))
                {
                    using (GZipStream gzip_stream = new GZipStream(file_stream, CompressionMode.Compress))
                    {
                        using (XmlTextWriter xml_test_writer = new XmlTextWriter(gzip_stream, Encoding.ASCII))
                        {
                            new XElement("sudoku_board", GetAsXML()).WriteTo(xml_test_writer);
                        };
                    }
                }
            }
        }

        internal bool Check()
        {
            return AllCellsUnits().All(unit => unit.Where(cell => cell.IsSolved).All(cell => !cell.NumbersPossible().Any()));
        }

        internal void Apply(SudokuSolution a_solution)
        {
            a_solution.Removed.ForEach(num =>
                m_board[num.Col, num.Row][num.Number - 1].State = SudokuNumberState.sudokucellstateImpossible);
            a_solution.Solved.ForEach(num =>
                m_board[num.Col, num.Row][num.Number - 1].State = SudokuNumberState.sudokucellstateSolved);
        }

        public bool IsSolved
        {
            get
            {
                if (!m_bSolved.HasValue)
                {
                    if (Cells().Any(cell => !cell.IsSolved))
                        m_bSolved = false;
                    else
                    {
                        m_bSolved = AllCellsUnits().All(unit => (from cell in unit
                                                                 select cell.NumberSolved().Number).Distinct().Count() == SIZE);
                    }
                }

                return m_bSolved.Value;
            }
        }

        public bool IsSolvable
        {
            get
            {
                if (!m_bSolvable.HasValue)
                    m_bSolvable = !GetUnsolvableCells().Any();

                return m_bSolvable.Value;
            }
        }

        public IEnumerable<SudokuCell> GetUnsolvableCells()
        {
            var cells1 = from cell in Cells()
                         where cell.Numbers().All(num => num.State == SudokuNumberState.sudokucellstateImpossible)
                         select cell;

            var cells2 = from unit in AllCellsUnits()
                         from cell1 in unit
                         where cell1.IsSolved
                         from cell2 in unit
                         where (cell1.Index < cell2.Index) && 
                               cell2.IsSolved && 
                               (cell1.NumberSolved().Number == cell2.NumberSolved().Number)
                         select new[] { cell1, cell2 };

            var cells3 = (from unit in AllNumbersUnits()
                          select (from num in unit
                                  where num.State == SudokuNumberState.sudokucellstateImpossible
                                  group num by num.Number into gr
                                  where gr.Count() == 9
                                  from num in gr
                                  select num.Cell)).SelectMany();

            return cells2.SelectMany().Concat(cells1).Concat(cells3).Distinct();
        }

        public bool IsEmpty
        {
            get
            {
                return Numbers().All(num => num.State == SudokuNumberState.sudokucellstatePossible);
            }
        }

        public string GetAsText()
        {
            var board = (from row in RowsCells()
                        orderby row.First().Row 
                        select (from cell in row
                                orderby cell.Col 
                                select (cell.IsSolved ? cell.NumberSolved().Number.ToString() : ".")).Aggregate("", (r, s) => r += s)).ToArray();

            return board.Except(board.Last()).Aggregate("", (r, s) => r += s + System.Environment.NewLine) + board.Last();
        }

        private static SudokuBoard LoadFromText(string a_str)
        {
            a_str = a_str.Replace(System.Environment.NewLine, System.Environment.NewLine.Substring(0, 1));
            List<string> lines = a_str.Split(System.Environment.NewLine[0]).ToList();

            for (int i = lines.Count - 1; i >= 0; i--)
            {
                lines[i] = lines[i].Replace(" ", "");
                if (lines[i].IndexOfAny(new char[] { ']', '[', '-', '+', '*' }) != -1)
                    lines.RemoveAt(i);
                else if (lines[i].Length == 0)
                    lines.RemoveAt(i);
            }

            if (lines.Count == 1)
            {
                string str = lines[0];

                if (str.Length == 9 * 9)
                {
                    lines.Clear();

                    for (int i = 0; i < 9; i++)
                        lines.Add(str.Substring(i * 9, 9));
                }
            }

            for (int i = lines.Count - 1; i >= 0; i--)
            {
                lines[i] = lines[i].Replace(" |", "");
                lines[i] = lines[i].Replace("|", "");
                lines[i] = lines[i].Replace(" ", ".");
            }

            for (int i = lines.Count - 1; i >= 0; i--)
            {
                if (lines[i].Length != SIZE)
                    lines.RemoveAt(i);
            }

            if (lines.Count != SIZE)
                return null;

            SudokuBoard board = new SudokuBoard();

            (from cell in board.Cells()
             where lines[cell.Row][cell.Col] != '.'
             select cell).ForEach(cell =>
                 cell[Int32.Parse(new string(new char[] { lines[cell.Row][cell.Col] })) - 1].State =
                 SudokuNumberState.sudokucellstateManualEntered);

            if (!board.Check())
                return null;

            return board;
        }
    }
}
