using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SudokuLib;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using SudokuLINQSolver.Configurations;

namespace SudokuTest
{
    [TestClass()]
    public class SudokuTests
    {
        private TestContext m_testContext;

        public TestContext TestContext
        {
            get
            {
                return m_testContext;
            }
            set
            {
                m_testContext = value;
            }
        }

        [TestInitialize]
        public void Initialize()
        {
            SudokuOptions.Current.ShowAllSolutions = true;
            SudokuOptions.Current.IncludeBoxes = true;
        }

        [TestMethod]
        public void Test_Solutions_Unique()
        {
            ProgressIndicator pi = new ProgressIndicator();
            pi.TestName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            FileInfo[] files = new DirectoryInfo(Directories.Solutions).GetFiles(FileExtensions.XmlZipMask);

            var solutions1 = (from file in files.AsParallel()
                              select new
                              {
                                  intermediate_solution = SudokuIntermediateSolution.LoadFromFile(file.FullName),
                                  fileName = file.FullName,
                              }).ToArray();

            solutions1.ForEach(example => Assert.IsNotNull(example.intermediate_solution, example.fileName));

            var solutions2 = (from sol in solutions1.AsParallel()
                              select new[] 
                            {
                                new 
                                {
                                    intermediate_solution = sol.intermediate_solution,
                                    fileName = sol.fileName,
                                    rotate = 0
                                },
                                new 
                                {
                                    intermediate_solution = sol.intermediate_solution.Rotate(),
                                    fileName = sol.fileName,
                                    rotate = 1
                                },
                                new 
                                {
                                    intermediate_solution = sol.intermediate_solution.Rotate().Rotate(),
                                    fileName = sol.fileName,
                                    rotate = 2
                                },
                                new 
                                {
                                    intermediate_solution = sol.intermediate_solution.Rotate().Rotate().Rotate(),
                                    fileName = sol.fileName,
                                    rotate = 3
                                }
                                
                            }).SelectMany().ToArray();

            var solutions_gr = (from obj in solutions2
                                group obj by obj.intermediate_solution.Solution.Type into gr
                                select (from obj in gr
                                        orderby obj.fileName
                                        select obj).ToArray()).ToArray();

            solutions_gr = (from gr in solutions_gr
                            select (gr.Length == 80) ? new[] { gr.Take(40).ToArray(), gr.Skip(40).Take(40).ToArray() } : 
                                new[] { gr }).SelectMany(gr => gr.ToArray()).ToArray();

            ConcurrentBag<string> not_uniques = new ConcurrentBag<string>();

            ConcurrentCounter counter = new ConcurrentCounter();
            int count = solutions_gr.Sum(gr => gr.Length);

            Parallel.ForEach(solutions_gr, (solution_gr) =>
            {
                var sols = solution_gr;

                foreach (var solution1 in sols)
                {
                    foreach (var solution2 in sols.Except(solution1))
                    {
                        if (sols.IndexOf(solution1) >= sols.IndexOf(solution2))
                            continue;
                        if ((solution1.fileName == solution2.fileName))
                            continue;

                        if (solution1.intermediate_solution.Equals(solution2.intermediate_solution))
                        {
                            not_uniques.Add("are the same");
                            not_uniques.Add("solution1; " + solution1.rotate + " - " + solution1.fileName);
                            not_uniques.Add("solution2; " + solution2.rotate + " - " + solution2.fileName);
                        }
                    }

                    counter.Increment();
                    pi.Progress = counter.Value * 100 / count;
                }
            });

            if (not_uniques.Count != 0)
            {
                foreach (var line in not_uniques)
                    TestContext.WriteLine(line);

                Assert.Fail();
            }
        }

        [TestMethod]
        public void Test_Solutions_Quantity()
        {
            SudokuOptions.Current.ShowAllSolutions = false;
            SudokuOptions.Current.IncludeBoxes = true;

            ProgressIndicator pi = new ProgressIndicator();
            pi.TestName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            FileInfo[] files = new DirectoryInfo(Directories.Solutions).GetFiles(FileExtensions.XmlZipMask);

            var solutions = (from file in files.AsParallel()
                             select new
                             {
                                 intermediate_solution = SudokuIntermediateSolution.LoadFromFile(file.FullName),
                                 fileName = file.FullName,
                             }).ToArray();

            solutions.ForEach(example => Assert.IsNotNull(example.intermediate_solution, example.fileName));

            var solutions_gr = (from obj in solutions
                                group obj by obj.intermediate_solution.Solution.Type into gr
                                select (from obj in gr
                                        orderby obj.fileName
                                        select obj).ToArray()).ToArray();

            ConcurrentBag<string> bad_quantities = new ConcurrentBag<string>();

            Parallel.ForEach(solutions_gr, (solution_gr) =>
            {
                pi.Progress = solutions_gr.IndexOf(solution_gr) * 100 / solutions_gr.Count();

                if (new[] { SudokuSolutionType.XWing, SudokuSolutionType.JellyFish, SudokuSolutionType.SwordFish }.Contains(
                    solution_gr.First().intermediate_solution.Solution.Type))
                {
                    if (solution_gr.Count() < 20)
                        bad_quantities.Add(solution_gr.First().intermediate_solution.Solution.Type + " - less then 20");
                    else if (solution_gr.Count() > 20)
                        bad_quantities.Add(solution_gr.First().intermediate_solution.Solution.Type + " - more then 20");
                    else
                    {
                        SudokuOptions.Current.IncludeBoxes = false;

                        var gr1 = (from obj in solution_gr.Take(10)
                                   select obj.intermediate_solution.Solution).ToList();
                        int c1 = SudokuSolutionNode.FilterByOptions(gr1).Count();
                        if (c1 == 0)
                            bad_quantities.Add(solution_gr.First().intermediate_solution.Solution.Type + " - solutions from 1 to 10 doesn't contains any boxes");
                        if (c1 == 10)
                            bad_quantities.Add(solution_gr.First().intermediate_solution.Solution.Type + " - solutions from 1 to 10 all contains boxes");

                        SudokuOptions.Current.IncludeBoxes = false;
                        var gr2 = (from obj in solution_gr.Skip(10).Take(10)
                                   select obj.intermediate_solution.Solution).ToList();
                        int c2 = SudokuSolutionNode.FilterByOptions(gr2).Count();
                        if (c2 != 10)
                            bad_quantities.Add(solution_gr.First().intermediate_solution.Solution.Type + " - solutions from 11 to 20 some contains boxes");
                    }
                }
                else
                {
                    if (solution_gr.Count() < 10)
                        bad_quantities.Add(solution_gr.First().intermediate_solution.Solution.Type + " - less then 10");
                    if (solution_gr.Count() > 10)
                        bad_quantities.Add(solution_gr.First().intermediate_solution.Solution.Type + " - more then 10");
                }
            });

            if (solutions_gr.Count() != Enum.GetValues(typeof(SudokuSolutionType)).Length)
                bad_quantities.Add("Not all algorithms was tested");

            if (bad_quantities.Count != 0)
            {
                foreach (var line in bad_quantities)
                    TestContext.WriteLine(line);
            }
        }

        [TestMethod]
        public void Test_Examples_Unique()
        {
            FileInfo[] files1 = new DirectoryInfo(Directories.Examples).GetFiles(FileExtensions.XmlZipMask);
            FileInfo[] files2 = new DirectoryInfo(Directories.Examples + Path.DirectorySeparatorChar + "Unsolvable").GetFiles(FileExtensions.XmlZipMask);

            ProgressIndicator pi = new ProgressIndicator();
            pi.TestName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            var examples1 = from file in files1.Concat(files2).AsParallel()
                            select new
                            {
                                board = SudokuBoard.LoadFromFile(file.FullName),
                                fileName = file.FullName
                            };

            var examples2 = (from example in examples1.AsParallel()
                             select new[] 
                            {
                                new 
                                {
                                    board = example.board,
                                    fileName = example.fileName,
                                    rotate = 0
                                },
                                new 
                                {
                                    board = example.board.Rotate(),
                                    fileName = example.fileName,
                                    rotate = 1
                                },
                                new 
                                {
                                    board = example.board.Rotate().Rotate(),
                                    fileName = example.fileName,
                                    rotate = 2
                                },
                                new 
                                {
                                    board = example.board.Rotate().Rotate().Rotate(),
                                    fileName = example.fileName,
                                    rotate = 3
                                }
                                
                            }).SelectMany().ToArray();

            ConcurrentBag<string> not_uniques = new ConcurrentBag<string>();

            ConcurrentCounter counter = new ConcurrentCounter();

            Parallel.ForEach(examples2, (example1) =>
            {
                counter.Increment();
                pi.Progress = counter.Value* 100 / examples2.Length;

                foreach (var example2 in examples2.Except(example1))
                {
                    if (example1.fileName == example2.fileName)
                        continue;

                    if ((from num1 in example1.board.Numbers()
                         where num1.IsSolved
                         select num1).All(num1 => example2.board[num1.Col, num1.Row][num1.Number - 1].IsSolved))
                    {
                        not_uniques.Add("not unique");
                        not_uniques.Add("example; rotate_" + example1.rotate + "; " + example1.fileName);
                        not_uniques.Add("include in rotate_" + example2.rotate + "; " + example2.fileName);
                    }
                }
            });

            if (not_uniques.Count != 0)
            {
                foreach (var line in not_uniques)
                    TestContext.WriteLine(line);

                Assert.Fail();
            }
        }

        [TestMethod()]
        public void Test_Board_Load_As_Text()
        {
            FileInfo[] files = new DirectoryInfo(Directories.TestExamples).GetFiles(FileExtensions.TxtExt);

            foreach (FileInfo file in files)
            {
                SudokuBoard board = SudokuBoard.LoadFromFile(file.FullName);

                Assert.IsNotNull(board, file.FullName);

                foreach (SudokuNumber num in board.Numbers())
                {
                    if (num.Row == num.Col)
                    {
                        if (num.Number == num.Row + 1)
                            Assert.AreEqual(SudokuNumberState.sudokucellstateManualEntered, num.State, file.FullName);
                        else
                            Assert.AreEqual(SudokuNumberState.sudokucellstateImpossible, num.State, file.FullName);
                    }
                    else
                        Assert.AreEqual(SudokuNumberState.sudokucellstatePossible, num.State, file.FullName);
                }
            }
        }

        [TestMethod()]
        public void Test_Board_Rotate()
        {
            SudokuBoard board = new SudokuBoard();

            Random r = new Random();
            board.Numbers().ForEach(num => num.State = (r.Next(2) == 1) ?
                SudokuNumberState.sudokucellstateImpossible : SudokuNumberState.sudokucellstatePossible);

            Assert.IsFalse(!board.Rotate().Equals(board));
            Assert.IsFalse(!board.Rotate().Rotate().Equals(board));
            Assert.IsTrue(board.Rotate().Rotate().Rotate().Equals(board));
        }

        [TestMethod()]
        public void Test_Solutions_Solvable()
        {
            SudokuOptions.Current.ShowAllSolutions = false;
            SudokuOptions.Current.IncludeBoxes = true;

            FileInfo[] files = new DirectoryInfo(Directories.Solutions).GetFiles(FileExtensions.XmlZipMask);

            ProgressIndicator pi = new ProgressIndicator();
            pi.TestName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            var solutions1 = (from file in files.AsParallel()
                              select new
                              {
                                  intermediate_solution = SudokuIntermediateSolution.LoadFromFile(file.FullName),
                                  fileName = file.FullName
                              }).ToArray();

            var solutions2 = (from sol in solutions1.AsParallel()
                              select new[] 
                            {
                                new 
                                {
                                    intermediate_solution = sol.intermediate_solution,
                                    fileName = sol.fileName,
                                    rotate = 0
                                },
                                new 
                                {
                                    intermediate_solution = sol.intermediate_solution.Rotate(),
                                    fileName = sol.fileName,
                                    rotate = 1
                                },
                                new 
                                {
                                    intermediate_solution = sol.intermediate_solution.Rotate().Rotate(),
                                    fileName = sol.fileName,
                                    rotate = 2
                                },
                                new 
                                {
                                    intermediate_solution = sol.intermediate_solution.Rotate().Rotate().Rotate(),
                                    fileName = sol.fileName,
                                    rotate = 3
                                }
                                
                            }).SelectMany().ToArray();

            var solutions3 = (from sol in solutions2
                              group sol by sol.intermediate_solution.Solution.Type into gr
                              select gr.ToArray()).ToArray();

            ConcurrentBag<string> unsolvable = new ConcurrentBag<string>();

            int count = solutions3.Sum(gr => gr.Count());
            ConcurrentCounter counter = new ConcurrentCounter();

            Parallel.ForEach(ChunkPartitioner.Create(solutions3.SelectMany(), 10), (solution) =>
            {
                if (!solution.intermediate_solution.Test(true))
                    unsolvable.Add("unsolvable; rotate_" + solution.rotate + "; " + solution.fileName);

                counter.Increment();

                pi.Progress = counter.Value * 100 / count;
            });

            if (unsolvable.Count > 0)
            {
                foreach (var file in unsolvable)
                    TestContext.WriteLine(file);

                Assert.Fail();
            }
        }

        [TestMethod()]
        public void Test_Solution_Tress_Verificator()
        {
            ProgressIndicator pi = new ProgressIndicator();
            pi.TestName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            FileInfo[] files = new DirectoryInfo(Directories.SolutionTrees).GetFiles(FileExtensions.XmlZipMask);

            string dir_diff = Directories.SolutionTrees + Path.DirectorySeparatorChar + "Verification_Failed";
            new DirectoryInfo(dir_diff).CreateOrEmpty();

            bool ok = true;

            ConcurrentCounter counter = new ConcurrentCounter();
            int index = 0;

            Parallel.ForEach(files, (file) =>
            {
                counter.Increment();
                pi.Progress = counter.Value * 100 / files.Length;

                SudokuSolutionNode node = SudokuSolutionNode.LoadFromFile(file.FullName);

                foreach (SudokuSolutionNode n in node.GetAllNodesEnumerator())
                {
                    if (n.State == SudokuSolutionNodeState.Solution)
                    {
                        SudokuVerificator verificator = new SudokuVerificator(n.Board);

                        foreach (SudokuNumber number in n.Solution.Removed)
                        {
                            bool b = verificator.IsImpossible(number.Col, number.Row, number.Number - 1);

                            if (!b)
                            {
                                ok = false;

                                string diff_file = String.Format("{0}{1}{2}_index_{3}_sol_type_{4}_coords_{5}_number_{6}{7}",
                                    dir_diff,
                                    Path.DirectorySeparatorChar,
                                    Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file.FullName)),
                                    index,
                                    n.Solution.Type,
                                    number.Coords,
                                    number.Number,
                                    FileExtensions.XmlZipExt);
                                new SudokuIntermediateSolution(n.Parent.Board, n.Board, n.Solution).SaveToFile(diff_file);

                                index++;
                            }
                        }
                    }
                }
            });

            Assert.IsTrue(ok);
        }

        [TestMethod()]
        public void Test_Unsolvable()
        {
            FileInfo[] files = new DirectoryInfo(Directories.TestExamples + Path.DirectorySeparatorChar + "Unsolvable").GetFiles(FileExtensions.XmlZipMask);

            foreach (FileInfo file in files)
            {
                SudokuBoard board = SudokuBoard.LoadFromFile(file.FullName);
                Assert.IsNotNull(board, file.FullName);

                SudokuSolutionNode root_node = SudokuSolutionNode.CreateRoot(board);
                SudokuSolutionNode node = root_node.Solve();

                Assert.IsTrue(!node.Board.IsSolvable, file.Name);
            }
        }

        [TestMethod()]
        public void Test_Examples_Unsolvable()
        {
            SudokuOptions.Current.ShowAllSolutions = false;
            SudokuOptions.Current.IncludeBoxes = true;

            FileInfo[] files = new DirectoryInfo(Directories.Examples + Path.DirectorySeparatorChar + "Unsolvable").GetFiles(FileExtensions.XmlZipMask);

            ProgressIndicator pi = new ProgressIndicator();
            pi.TestName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            ConcurrentBag<string> solvable = new ConcurrentBag<string>();

            ConcurrentCounter counter = new ConcurrentCounter();

            Parallel.ForEach(files, (file) =>
            {
                counter.Increment();
                pi.Progress = counter.Value * 100 / files.Length;

                SudokuBoard board = SudokuBoard.LoadFromFile(file.FullName);
                Assert.IsNotNull(board, file.FullName);

                SudokuSolutionNode root_node = SudokuSolutionNode.CreateRoot(board);
                SudokuSolutionNode node = root_node.Solve();

                if (node.Board.IsSolved)
                    solvable.Add("solvable; " + file.FullName);
            });

            if (solvable.Count != 0)
            {
                foreach (var line in solvable)
                    TestContext.WriteLine(line);

                Assert.Fail();
            }
        }

        [TestMethod()]
        public void Test_Solution_Trees_vs_Examples()
        {
            ProgressIndicator pi = new ProgressIndicator();
            pi.TestName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            FileInfo[] files1 = new DirectoryInfo(Directories.Examples).GetFiles(FileExtensions.XmlZipMask);
            FileInfo[] files2 = new DirectoryInfo(Directories.SolutionTrees).GetFiles(FileExtensions.XmlZipMask);

            var not_all = (from file1 in files1
                           where files2.Count(fi => Directories.ConvertName(fi.FullName) == file1.Name) < 4
                           select file1).ToArray();

            var too_much = (from file2 in files2
                            where files1.All(fi => fi.Name != Directories.ConvertName(file2.Name))
                            select file2).ToArray();

            if (not_all.Count() > 0)
                Assert.Fail("Is in 'Examples', but not in 'SolutionTrees' (run project 'SudokuGenerator'): " + not_all.First().FullName);

            if (too_much.Count() > 0)
                Assert.Fail("Is in 'SolutionTrees', but not in 'Examples': " + too_much.First().FullName);

            string dir3 = Directories.SolutionTrees + Path.DirectorySeparatorChar + "Diffrencies";
            new DirectoryInfo(dir3).CreateOrEmpty();

            string dir4 = Directories.SolutionTrees + Path.DirectorySeparatorChar + "Diffrencies" + Path.DirectorySeparatorChar + "Added";
            new DirectoryInfo(dir4).CreateOrEmpty();

            string dir5 = Directories.SolutionTrees + Path.DirectorySeparatorChar + "Diffrencies" + Path.DirectorySeparatorChar + "Removed";
            new DirectoryInfo(dir5).CreateOrEmpty();

            bool b = false;

            ConcurrentBag<string> list = new ConcurrentBag<string>();

            ConcurrentCounter counter = new ConcurrentCounter();

            Parallel.ForEach(files2, (file2, state) =>
            {
                counter.Increment();
                pi.Progress = counter.Value * 100 / files2.Length;

                SudokuSolutionNode node2 = SudokuSolutionNode.LoadFromFile(file2.FullName);

                SudokuBoard board = SudokuBoard.LoadFromFile(files1.First(f => f.Name == Directories.ConvertName(file2.FullName)).FullName);

                board = board.Rotate(ExtractRotateNumber(file2.Name));

                SudokuSolutionNode node1 = SudokuSolutionNode.CreateRoot(board);
                node1.SolveWithStepAll();

                Assert.AreEqual(node1.State, node2.State, file2.FullName);
                Assert.AreEqual(node1.Board, node2.Board, file2.FullName);
                Assert.AreEqual(node1.Solution, node2.Solution, file2.FullName);

                b |= CompareNodes(node1, node2, file2, list, 1, 1);
            });

            if (list.Count != 0)
            {
                string filename = dir3 + Path.DirectorySeparatorChar + "!Diffrencies.txt";

                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    StreamWriter sw = new StreamWriter(fs);
                    list.ForEach(s => sw.WriteLine(s));
                    sw.Flush();
                }

                TestContext.AddResultFile(filename);
            }

            Assert.IsFalse(b);
        }

        private int ExtractRotateNumber(string a_fileName)
        {
            string str = Path.GetFileNameWithoutExtension(a_fileName);
            str = Path.GetFileNameWithoutExtension(str);
            return Int32.Parse(str.Substring(str.Length - 1));
        }

        private bool CompareNodes(SudokuSolutionNode a_node, SudokuSolutionNode a_expected_node, FileInfo a_fileInfo,
                                  ConcurrentBag<string> a_list, int add_counter, int remove_counter)
        {
            bool b1 = false;

            IEnumerable<SudokuSolutionNode> solutions_added = null;
            IEnumerable<SudokuSolutionNode> solutions_removed = null;

            if (!a_node.Nodes.Exact(a_expected_node.Nodes))
            {
                solutions_added = a_node.Nodes.Substract(a_expected_node.Nodes);
                solutions_removed = a_expected_node.Nodes.Substract(a_node.Nodes);

                if (solutions_removed.Count() > 0)
                    solutions_removed = RemoveIfExistsElsewhere(solutions_removed, a_node.Root);
                else
                    solutions_removed = null;

                if (solutions_added.Count() > 0)
                    solutions_added = RemoveIfExistsElsewhere(solutions_added, a_node.Root);
                else
                    solutions_added = null;
            }

            if (solutions_added != null)
            {
                foreach (var added in solutions_added)
                {
                    if (added.Solution != null)
                    {
                        string dir = a_fileInfo.DirectoryName + Path.DirectorySeparatorChar + "Diffrencies" + Path.DirectorySeparatorChar + "Added";

                        SudokuIntermediateSolution intermediate_solution = new SudokuIntermediateSolution(added.Board, added.NextBoard, added.Solution);

                        intermediate_solution.SaveToFile(dir + Path.DirectorySeparatorChar +
                            Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(a_fileInfo.FullName)) +
                            "_added_" + add_counter + FileExtensions.XmlZipExt);

                        a_list.Add(Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(a_fileInfo.FullName)) +
                            "_added_" + add_counter + ": " + intermediate_solution.Solution.ToString());

                        add_counter++;

                        b1 = true;
                    }
                }
            }

            if (solutions_removed != null)
            {
                foreach (var removed in solutions_removed)
                {
                    if (removed.Solution != null)
                    {
                        string dir = a_fileInfo.DirectoryName + Path.DirectorySeparatorChar + "Diffrencies" + Path.DirectorySeparatorChar + "Removed";

                        SudokuIntermediateSolution intermediate_solution = new SudokuIntermediateSolution(removed.Board, removed.NextBoard, removed.Solution);

                        intermediate_solution.SaveToFile(dir + Path.DirectorySeparatorChar +
                            Path.GetFileNameWithoutExtension(Path.DirectorySeparatorChar +
                            Path.GetFileNameWithoutExtension(a_fileInfo.FullName)) + "_removed_" + remove_counter + FileExtensions.XmlZipExt);

                        a_list.Add(Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(a_fileInfo.FullName)) +
                            "_removed_" + remove_counter + ": " + intermediate_solution.Solution.ToString());

                        remove_counter++;

                        b1 = true;
                    }
                }
            }

            var node_next = ((a_node != null) && a_node.Nodes.Any()) ? a_node.Nodes.First() : null;
            var expected_next = ((a_expected_node != null) && a_expected_node.Nodes.Any()) ? a_expected_node.Nodes.First() : null;

            if ((node_next == null) || (expected_next == null))
                return b1;

            if (!node_next.Solution.Equals(expected_next.Solution))
            {             
                var next = a_node.Nodes.FirstOrDefault(n => n.Solution.Equals(expected_next.Solution));

                if (next == null)
                {
                    next = a_expected_node.Nodes.FirstOrDefault(n => n.Solution.Equals(node_next.Solution));

                    if (next != null)
                    {
                        expected_next = next;
                        expected_next.SolveWithStepAll();
                    }
                    else
                        return b1;
                }
                else
                {
                    node_next = next;
                    node_next.SolveWithStepAll();
                }
            }

            if (node_next.Nodes.Any())
                node_next = node_next.Nodes.First();
            else
                return b1;

            if (expected_next.Nodes.Any())
                expected_next = expected_next.Nodes.First();
            else
                return b1;

            bool b2 = CompareNodes(node_next, expected_next,a_fileInfo, a_list, add_counter, remove_counter);

            return b1 | b2;
        }

        private IEnumerable<SudokuSolutionNode> RemoveIfExistsElsewhere(IEnumerable<SudokuSolutionNode> a_enum, SudokuSolutionNode a_node)
        {
            List<SudokuSolutionNode> list = new List<SudokuSolutionNode>(a_enum);

            foreach (SudokuSolutionNode node in a_node.Root.GetAllNodesEnumerator())
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i].Equals(node))
                    {
                        if (!Object.ReferenceEquals(list[i], node))
                            list.RemoveAt(i);
                    }
                }

                if (list.Count == 0)
                    break;
            }

            return list;
        }

        [TestMethod()]
        public void Test_Examples_Solvable()
        {
            SudokuOptions.Current.ShowAllSolutions = false;
            SudokuOptions.Current.IncludeBoxes = true;

            ProgressIndicator pi = new ProgressIndicator();
            pi.TestName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            FileInfo[] files = new DirectoryInfo(Directories.Examples).GetFiles(FileExtensions.XmlZipMask);

            ConcurrentBag<string> unsolvable = new ConcurrentBag<string>();

            ConcurrentCounter counter = new ConcurrentCounter();

            Parallel.ForEach(files, (file) =>
            {
                SudokuBoard board = SudokuBoard.LoadFromFile(file.FullName);
                Assert.IsNotNull(board, file.FullName);

                counter.Increment();
                pi.Progress = counter.Value * 100 / files.Length;

                for (int i = 0; i < 4; i++)
                {
                    SudokuSolutionNode root_node = SudokuSolutionNode.CreateRoot(board);
                    SudokuSolutionNode node = root_node.Solve();

                    if (!node.Board.IsSolved)
                        unsolvable.Add("unsolvable; rotate_" + i + "; " + file.Name);

                    board = board.Rotate();
                }
            });

            if (unsolvable.Count > 0)
            {
                foreach (var file in unsolvable)
                    TestContext.WriteLine(file);

                Assert.Fail();
            }
        }

        [TestMethod()]
        public void Test_Solution_Trees_Consolidate()
        {
            SudokuOptions.Current.ShowAllSolutions = false;

            ProgressIndicator pi = new ProgressIndicator();
            pi.TestName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            FileInfo[] files = new DirectoryInfo(Directories.SolutionTrees).GetFiles(FileExtensions.XmlZipMask);

            string dir = Directories.SolutionTrees + Path.DirectorySeparatorChar + "Consolidate";
            new DirectoryInfo(dir).CreateOrEmpty();

            int index = 0;
            ConcurrentCounter counter = new ConcurrentCounter();

            Parallel.ForEach(files, (file) => 
            {
                counter.Increment();
                pi.Progress = counter.Value * 100 / files.Length;

                SudokuSolutionNode root = SudokuSolutionNode.LoadFromFile(file.FullName);

                foreach (SudokuSolutionNode node in root.GetAllNodesEnumerator())
                {
                    if (node.State != SudokuSolutionNodeState.State)
                        continue;

                    var filtered_sols = SudokuSolutionNode.FilterByOptions(node.Nodes.Select(n => n.Solution).ToList());

                    var filtered_nodes = (from n in node.Nodes
                                         where filtered_sols.Contains(n.Solution)
                                         select n).ToList();

                    var new_includes = (from n1 in filtered_nodes
                                        from n2 in filtered_nodes
                                        where SudokuSolutionNode.IsInclude(n1.Solution, n2.Solution, false)
                                        select new { n1, n2 }).ToList();

                    var ignore_pairs = new[] 
                    { 
                        new [] { SudokuSolutionType.NakedPair, SudokuSolutionType.BoxLineReduction }, 
                        new [] { SudokuSolutionType.NakedPair, SudokuSolutionType.PointingPair }, 
                        new [] { SudokuSolutionType.NakedPair, SudokuSolutionType.NakedTriple }, 
                        new [] { SudokuSolutionType.NakedPair, SudokuSolutionType.NakedQuad }, 
                        new [] { SudokuSolutionType.NakedTriple, SudokuSolutionType.BoxLineReduction }, 
                        new [] { SudokuSolutionType.NakedTriple, SudokuSolutionType.NakedQuad }, 
                        new [] { SudokuSolutionType.NakedTriple, SudokuSolutionType.PointingPair }, 
                        new [] { SudokuSolutionType.NakedTriple, SudokuSolutionType.PointingTriple }, 
                        new [] { SudokuSolutionType.NakedQuad, SudokuSolutionType.PointingPair }, 
                        new [] { SudokuSolutionType.HiddenPair, SudokuSolutionType.HiddenTriple }, 
                        new [] { SudokuSolutionType.HiddenPair, SudokuSolutionType.HiddenQuad }, 
                        new [] { SudokuSolutionType.HiddenTriple, SudokuSolutionType.HiddenQuad }, 
                    };

                    var ignore = from pair in new_includes
                                 from ignore_pair in ignore_pairs
                                 where (((pair.n1.Solution.Type == ignore_pair.First()) &&
                                        (pair.n2.Solution.Type == ignore_pair.Last())) ||
                                        ((pair.n1.Solution.Type == ignore_pair.Last()) &&
                                        (pair.n2.Solution.Type == ignore_pair.First())))
                                 select pair;

                    new_includes = new_includes.Except(ignore).ToList();

                    var doubles = from p1 in new_includes
                                  from p2 in new_includes
                                  where (p1.n1 == p2.n2) &&
                                        (p1.n2 == p2.n1)
                                  select p1;

                    new_includes = new_includes.Except(doubles).ToList();

                    foreach (var pair in new_includes)
                    {
                        SudokuIntermediateSolution intermediate_solution_1 = new SudokuIntermediateSolution(pair.n1.Board, pair.n1.NextBoard, pair.n1.Solution);

                        intermediate_solution_1.SaveToFile(dir + Path.DirectorySeparatorChar + index + "_" +
                            Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file.FullName)) + "_" +
                            pair.n1.Solution.Type + FileExtensions.XmlZipExt);

                        SudokuIntermediateSolution intermediate_solution_2 = new SudokuIntermediateSolution(pair.n2.Board, pair.n2.NextBoard, pair.n2.Solution);

                        intermediate_solution_2.SaveToFile(dir + Path.DirectorySeparatorChar + index + "_" +
                            Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file.FullName)) + "_" +
                            pair.n2.Solution.Type + FileExtensions.XmlZipExt);

                        index++;
                    }
                }
            });

            Assert.IsTrue(index == 0);
        }

        [TestMethod()]
        public void Test_Progress_Indicator_1()
        {
            ProgressIndicator pi = new ProgressIndicator();
            pi.TestName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            int counter = 0;

            for (; ; )
            {
                if (pi.IsDisposed)
                {
                    pi = new ProgressIndicator();
                    pi.TestName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                }

                pi.Progress = counter % 99;
                Thread.Sleep(10);
                counter++;
            }
        }

        [TestMethod()]
        public void Test_Progress_Indicator_2()
        {
            ProgressIndicator pi = new ProgressIndicator();
            pi.TestName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            int counter = 0;

            for (; ; )
            {
                if (pi.IsDisposed)
                {
                    pi = new ProgressIndicator();
                    pi.TestName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                }

                int c = pi.Progress;
                pi.Progress = counter % 99;
                Thread.Sleep(10);
                counter = pi.Progress + c;
                counter++;
            }
        }
    }
}
