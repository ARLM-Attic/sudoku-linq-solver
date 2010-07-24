using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SudokuLib;
using System.Threading.Tasks;

namespace SudokuGenerator
{
    class Program
    {
        private enum GenerationMethod
        {
            All,
            OneType,
            OneSolution
        }

        static void Main(string[] args)
        {
            System.Console.CursorVisible = false;

            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.BelowNormal;

            SudokuOptions.Current.IncludeBoxes = true;
            SudokuOptions.Current.ShowAllSolutions = true;

            System.Console.WriteLine("");
            System.Console.WriteLine(SudokuOptions.Current);
            System.Console.WriteLine("");

            System.Console.WriteLine("Press:");
            System.Console.WriteLine("1 - Generate all solutions trees");
            System.Console.WriteLine("2 - Generate all solutions from solutions trees");
            System.Console.WriteLine("3 - Generate all solutions from solutions trees");
            System.Console.WriteLine("    (intermediate solution with only one solution type)");
            System.Console.WriteLine("4 - Generate all solutions from solutions trees");
            System.Console.WriteLine("    (intermediate solution with only one solution)");
            System.Console.WriteLine("5 - Exit");

            for (; ; )
            {
                switch (System.Console.ReadKey(true).KeyChar)
                {
                    case '1': GenerateAllSolutionsTrees(); return;
                    case '2': GenerateAllSolutions(GenerationMethod.All); return;
                    case '3': GenerateAllSolutions(GenerationMethod.OneType); return;
                    case '4': GenerateAllSolutions(GenerationMethod.OneSolution); return;
                    case '5': return;
                }
            }
        }

        private static void GenerateAllSolutions(GenerationMethod a_method)
        {
            System.Console.WriteLine();
            System.Console.WriteLine("Generating solutions...");
            System.Console.WriteLine();

            string dir = Directories.SolutionTrees + Path.DirectorySeparatorChar + "All";
            new DirectoryInfo(dir).CreateOrEmpty();

            FileInfo[] files1 = new DirectoryInfo(Directories.Examples).GetFiles(FileExtensions.XmlZipMask);
            FileInfo[] files2 = new DirectoryInfo(Directories.SolutionTrees).GetFiles(FileExtensions.XmlZipMask);

            var not_all = (from file1 in files1
                           where files2.Count(fi => Directories.ConvertName(fi.FullName) == file1.Name) < 4
                           select file1).ToArray();

            var too_much = (from file2 in files2
                            where files1.All(fi => fi.Name != Directories.ConvertName(file2.Name))
                            select file2).ToArray();


            if (not_all.Count() > 0)
            {
                System.Console.WriteLine("Is in 'Examples', but not in 'SolutionTrees' (run project 'SudokuGenerator'): " + not_all.First().FullName);
                System.Console.ReadKey(true);
                return;
            }

            if (too_much.Count() > 0)
            {
                System.Console.WriteLine("Is in 'SolutionTrees', but not in 'Examples': " + too_much.First().FullName);
                System.Console.ReadKey(true);
                return;
            }

            FileInfo[] files = (from file in new DirectoryInfo(Directories.SolutionTrees).GetFiles(FileExtensions.XmlZipMask)
                               group file by Directories.ConvertName(file.Name) into gr
                               select gr.First()).ToArray();

            ShowProgress(0, 70);

            ConcurrentCounter counter = new ConcurrentCounter();

            Parallel.ForEach(files, (file) =>
            {
                SudokuSolutionNode root = SudokuSolutionNode.LoadFromFile(file.FullName);

                string file_name = Path.GetFileNameWithoutExtension(Directories.ConvertName(file.Name));

                Dictionary<SudokuSolutionType, int> counters = new Dictionary<SudokuSolutionType, int>();
                Dictionary<SudokuSolutionType, List<SudokuSolutionNode>> lists = new Dictionary<SudokuSolutionType, List<SudokuSolutionNode>>();

                foreach (var type in Enum.GetValues(typeof(SudokuSolutionType)).Cast<SudokuSolutionType>())
                {
                    if (type == SudokuSolutionType.MarkImpossibles)
                        continue;
                    if (type == SudokuSolutionType.MarkSolved)
                        continue;
                    if (type == SudokuSolutionType.SinglesInUnit)
                        continue;

                    new DirectoryInfo(dir + Path.DirectorySeparatorChar + type).CreateOrEmpty();
                    lists[type] = new List<SudokuSolutionNode>();
                    counters[type] = 1;
                }

                foreach (var list in lists.Values)
                    list.Clear();

                foreach (SudokuSolutionNode node in root.GetAllNodesEnumerator())
                {
                    if (node.State != SudokuSolutionNodeState.State)
                        continue;

                    var nodes = (from n in node.Nodes
                                 where (n.Solution.Type != SudokuSolutionType.MarkImpossibles) &&
                                       (n.Solution.Type != SudokuSolutionType.MarkSolved) &&
                                       (n.Solution.Type != SudokuSolutionType.SinglesInUnit)
                                 select n).ToList();

                    var sols = SudokuSolutionNode.FilterByOptions(nodes.Select(n => n.Solution).ToList());

                    var nodes_gr = (from n in nodes
                                    where sols.Contains(n.Solution)
                                    group n by n.Solution.Type into gr
                                    select gr.ToList()).ToList();


                    if ((a_method == GenerationMethod.OneType) || (a_method == GenerationMethod.OneSolution))
                    {
                        bool one_type = nodes_gr.Count(gr => gr.Any()) == 1;

                        if (!one_type)
                            continue;
                    }

                    if (a_method == GenerationMethod.OneSolution)
                    {
                        bool one_solution = nodes_gr.Count(gr => gr.Count == 1) == 1;

                        if (!one_solution)
                            continue;
                    }

                    foreach (var n in nodes)
                        lists[n.Solution.Type].Add(n);
                }

                foreach (var list in lists.Values)
                {
                    var uniques = from n in list
                                  group n by n.Solution into gr
                                  select gr.First();

                    var exclude = from n1 in uniques
                                  from n2 in uniques
                                  where !Object.ReferenceEquals(n1, n2) &&
                                        n1.Solution.Removed.Contains(n2.Solution.Removed, Comparators.SudokuNumberRowColComparer.Instance) &&
                                        n1.Solution.Stayed.Contains(n2.Solution.Stayed, Comparators.SudokuNumberRowColComparer.Instance) &&
                                        n1.Solution.ColorUnits.Equals(n2.Solution.ColorUnits)
                                  select n2;

                    var sols = uniques.Except(exclude);

                    foreach (var n in sols)
                    {
                        SudokuIntermediateSolution intermediate_solution = new SudokuIntermediateSolution(n.Board, n.NextBoard, n.Solution);

                        intermediate_solution.SaveToFile(dir + Path.DirectorySeparatorChar + n.Solution.Type + Path.DirectorySeparatorChar +
                             file_name + " - " + counters[n.Solution.Type]++ + FileExtensions.XmlZipExt);
                    }
                }

                counter.Increment();
                lock (counter)
                {
                    ShowProgress(counter.Value * 70 / files.Length, 70);
                }
            });
        }

        private static void GenerateAllSolutionsTrees()
        {
            System.Console.WriteLine();
            System.Console.WriteLine("Generating all solutions trees...");
            System.Console.WriteLine();

            FileInfo[] files = new DirectoryInfo(Directories.Examples).GetFiles(FileExtensions.XmlZipMask);

            ShowProgress(0, 70);

            new DirectoryInfo(Directories.SolutionTrees).CreateOrEmpty();

            ConcurrentCounter counter = new ConcurrentCounter();

            Parallel.ForEach(files, (file) =>
            {
                SudokuBoard board = SudokuBoard.LoadFromFile(file.FullName);

                for (int i = 0; i < 4; i++)
                {
                    SudokuSolutionNode node = SudokuSolutionNode.CreateRoot(board);
                    node.SolveWithStepAll();
                    node.SaveToFile(Directories.SolutionTrees + Path.DirectorySeparatorChar +
                        Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file.FullName)) + "_rotate_" + i + FileExtensions.XmlZipExt);

                    board = board.Rotate();
                }

                counter.Increment();
                lock (counter)
                {
                    ShowProgress(counter.Value * 70 / files.Length, 70);
                }
            });
        }

        private static void ShowProgress(int a_progress, int a_totall)
        {
            StringBuilder str = new StringBuilder(1 + a_totall + 1);
            str.Length = str.Capacity;
            for (int j = 0; j < str.Length; j++)
                str[j] = ' ';
            for (int j = 1; j <= a_progress; j++)
                str[j] = '.';
            str[0] = '[';
            str[str.Length - 1] = ']';

            System.Console.SetCursorPosition(0, System.Console.CursorTop);
            System.Console.Write("{0}% ", a_progress * 100 / a_totall);
            System.Console.Write(str);
        }
    }
}
