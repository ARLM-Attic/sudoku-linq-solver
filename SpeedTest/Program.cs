using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SudokuLib;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace SpeedTest
{
    class Program
    {
        private static Dictionary<SudokuSolutionType, double> s_algorithmToTime = new Dictionary<SudokuSolutionType, double>();

        static void Main(string[] args)
        {
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.AboveNormal;

            SudokuOptions.Current.IncludeBoxes = false;
            SudokuOptions.Current.ShowAllSolutions = false; 

            System.Console.WriteLine("");
            System.Console.WriteLine(SudokuOptions.Current);
            System.Console.WriteLine("");

            MeasuringAverageAlgorithmSolvingTime();
            System.Console.WriteLine("");
            MeasuringAvarageExampleSolvingTime();
            System.Console.WriteLine("");
            CalculateSuggestedAlgorithmsOrder();
            System.Console.ReadKey();
        }

        private static void MeasuringAverageAlgorithmSolvingTime()
        {
            const int REPEATS = 2;

            System.Console.WriteLine("Measuring average algorithm solving time:");
            System.Console.WriteLine("");

            FileInfo[] files = new DirectoryInfo(Directories.Solutions).GetFiles(FileExtensions.XmlZipMask);

            var solutions1 = from file in files.AsParallel()
                          select new
                          {
                              intermediate_solution = SudokuIntermediateSolution.LoadFromFile(file.FullName),
                              fileName = file.FullName
                          };

            var solutions2 = (from solution in solutions1
                             group solution by solution.intermediate_solution.Solution.Type into g
                             select (from obj in g
                                     orderby obj.fileName
                                     select obj).ToArray()).ToArray();

            foreach (var sols_gr in solutions2)
            {
                var sols = SudokuSolutionNode.FilterByOptions(sols_gr.Select(gr => gr.intermediate_solution.Solution).ToList());

                if (sols_gr.Length == 20)
                {
                    if (SudokuOptions.Current.IncludeBoxes)
                        sols = sols.Take(10).ToList();
                    else
                        sols = sols.Skip(10).Take(10).ToList();
                }

                var sols_gr2 = (from gr in sols_gr
                                where sols.Contains(gr.intermediate_solution.Solution)
                                select gr).ToList();

                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                foreach (var solution in sols_gr2)
                {
                    SudokuIntermediateSolution intermediate_solution = solution.intermediate_solution;

                    bool bError = false;

                    for (int i = 0; i < REPEATS; i++)
                    {
                        for (int j=0; j<4; j++)
                        {
                            if (!intermediate_solution.Test(false))
                                bError = true;
                                
                            intermediate_solution = intermediate_solution.Rotate();
                        }
                    }

                    if (bError)
                        System.Console.WriteLine("Can't solve: {0}", solution.fileName);
                }

                stopwatch.Stop();

                double time = stopwatch.ElapsedMilliseconds * 1.0 / sols_gr.Length / REPEATS / 4;

                s_algorithmToTime[sols_gr.First().intermediate_solution.Solution.Type] = time;

                System.Console.WriteLine("{0} ({1}): {2} [ms]", sols_gr.First().intermediate_solution.Solution.Type, sols_gr.Length, time.ToString("G4"));
            }
        }

        private static void MeasuringAvarageExampleSolvingTime()
        {
            System.Console.WriteLine("Measuring average example solving time:");
            System.Console.WriteLine("");

            FileInfo[] files = new DirectoryInfo(Directories.Examples).GetFiles(FileExtensions.XmlZipMask);

            SudokuBoard[] boards = new SudokuBoard[files.Length];

            for (int i = 0; i < files.Length; i++)
                boards[i] = SudokuBoard.LoadFromFile(files[i].FullName);

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < boards.Length; i++)
            {
                System.Console.WriteLine(String.Format("Solving {0} from {1}: {2}", i + 1, boards.Length, files[i].Name));

                SudokuSolutionNode node = SudokuSolutionNode.CreateRoot(boards[i]);
                node = node.Solve();

                if (!node.NextBoard.IsSolved)
                {
                    if (SudokuOptions.Current.IncludeBoxes)
                        System.Console.WriteLine(("ERROR: Can't solve: " + files[i]));
                    else
                        System.Console.WriteLine(("ERROR: Can't solve (recheck with boxes): " + files[i]));
                }
            }

            stopwatch.Stop();

            System.Console.WriteLine("");
            System.Console.WriteLine(("Average solving time per sudoku: " + stopwatch.ElapsedMilliseconds / files.Length + " [ms]"));
        }

        private static void CalculateSuggestedAlgorithmsOrder()
        {
            var times = from pair in s_algorithmToTime
                        orderby pair.Value
                        select new
                        {
                            type = pair.Key, 
                            time = pair.Value
                        };

            System.Console.WriteLine("Suggested algorithms order (SudokuSolverBase.m_complex_funcs):");
            System.Console.WriteLine("");

            foreach (var obj in times)
                System.Console.WriteLine("{0} - {1} [ms]", obj.type, obj.time.ToString("G4"));

            System.Console.WriteLine("");
        }
    }
}
