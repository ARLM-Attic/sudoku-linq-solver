using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace SudokuLib
{
    internal abstract class SudokuSolverBase
    {
        public delegate List<SudokuSolution> SolvingFunction(SudokuBoard a_board, bool a_all);

        protected SolvingFunction[] m_simple_funcs;
        protected SolvingFunction[] m_complex_funcs;
        protected Dictionary<SudokuSolutionType, SolvingFunction> m_sudokuSolutionTypeToSolvingFunctionMap;

        public SudokuSolverBase()
        {
            m_simple_funcs = new SolvingFunction[]
            { 
                Solve_MarkImpossibles, 
                Solve_MarkSolved, 
                Solve_SinglesInUnit
            };

            m_complex_funcs = new SolvingFunction[]
            { 
                Solve_NakedPairs,
                Solve_NakedTriples, 
                Solve_XYZWing,
                Solve_NakedQuads, 
                Solve_BoxLineReduction, 
                Solve_PointingTriples, 
                Solve_MultivalueXWing, 
                Solve_WXYZWing, 
                Solve_HiddenPairs,
                Solve_XWing, 
                Solve_PointingPairs, 
                Solve_HiddenTriples,
                Solve_HiddenQuads, 
                Solve_YWing,
                Solve_JellyFish,
                Solve_SwordFish
            };

            m_sudokuSolutionTypeToSolvingFunctionMap = new Dictionary<SudokuSolutionType, SolvingFunction>();
            m_sudokuSolutionTypeToSolvingFunctionMap[SudokuSolutionType.BoxLineReduction] = Solve_BoxLineReduction;
            m_sudokuSolutionTypeToSolvingFunctionMap[SudokuSolutionType.HiddenPair] = Solve_HiddenPairs;
            m_sudokuSolutionTypeToSolvingFunctionMap[SudokuSolutionType.HiddenQuad] = Solve_HiddenQuads;
            m_sudokuSolutionTypeToSolvingFunctionMap[SudokuSolutionType.HiddenTriple] = Solve_HiddenTriples;
            m_sudokuSolutionTypeToSolvingFunctionMap[SudokuSolutionType.MarkImpossibles] = Solve_MarkImpossibles;
            m_sudokuSolutionTypeToSolvingFunctionMap[SudokuSolutionType.MarkSolved] = Solve_MarkSolved;
            m_sudokuSolutionTypeToSolvingFunctionMap[SudokuSolutionType.MultivalueXWing] = Solve_MultivalueXWing;
            m_sudokuSolutionTypeToSolvingFunctionMap[SudokuSolutionType.NakedPair] = Solve_NakedPairs;
            m_sudokuSolutionTypeToSolvingFunctionMap[SudokuSolutionType.NakedQuad] = Solve_NakedQuads;
            m_sudokuSolutionTypeToSolvingFunctionMap[SudokuSolutionType.NakedTriple] = Solve_NakedTriples;
            m_sudokuSolutionTypeToSolvingFunctionMap[SudokuSolutionType.PointingPair] = Solve_PointingPairs;
            m_sudokuSolutionTypeToSolvingFunctionMap[SudokuSolutionType.PointingTriple] = Solve_PointingTriples;
            m_sudokuSolutionTypeToSolvingFunctionMap[SudokuSolutionType.SinglesInUnit] = Solve_SinglesInUnit;
            m_sudokuSolutionTypeToSolvingFunctionMap[SudokuSolutionType.SwordFish] = Solve_SwordFish;
            m_sudokuSolutionTypeToSolvingFunctionMap[SudokuSolutionType.JellyFish] = Solve_JellyFish;
            m_sudokuSolutionTypeToSolvingFunctionMap[SudokuSolutionType.XWing] = Solve_XWing;
            m_sudokuSolutionTypeToSolvingFunctionMap[SudokuSolutionType.YWing] = Solve_YWing;
            m_sudokuSolutionTypeToSolvingFunctionMap[SudokuSolutionType.XYZWing] = Solve_XYZWing;
            m_sudokuSolutionTypeToSolvingFunctionMap[SudokuSolutionType.WXYZWing] = Solve_WXYZWing;
        }

        private static List<SudokuSolution> RemoveOverlapped(List<SudokuSolution> a_list)
        {
            var remove1 = (from sol1 in a_list
                           from sol2 in a_list
                           where (a_list.IndexOf(sol1) > a_list.IndexOf(sol2)) &&
                                 sol1.Stayed.Intersect(sol2.Removed).Any()
                           select sol1);

            var remove2 = (from sol1 in a_list
                           from sol2 in a_list
                           where (a_list.IndexOf(sol1) > a_list.IndexOf(sol2)) &&
                                 sol1.Removed.Intersect(sol2.Removed).Any()
                           select sol1);

            return a_list.Except(remove1.Concat(remove2)).ToList();
        }

        private SolvingFunction SudokuSolutionTypeToSolvingFunction(SudokuSolutionType a_type)
        {
            return m_sudokuSolutionTypeToSolvingFunctionMap[a_type];
        }

        internal SudokuSolutionNode Step(SudokuSolutionNode a_node)
        {
            if (a_node.Board.IsEmpty)
                return new SudokuSolutionNode(a_node.Board, SudokuSolutionNodeState.Unsolvable);

            if ((a_node.StepMode != SudokuSolutionNodeStepMode.StepFirstSolution) && 
                (a_node.StepMode != SudokuSolutionNodeStepMode.StepAllAlgorithms))
            {
                a_node.StepMode = SudokuSolutionNodeStepMode.StepFirstSolution;

                if (a_node.State == SudokuSolutionNodeState.State)
                {
                    List<SudokuSolution> solutions = null;

                    if (a_node.NextBoard.IsSolvable)
                    {
                        // PLINQ
                        solutions = (from func in m_simple_funcs.Concat(m_complex_funcs)
                                      select func(a_node.NextBoard, false)).FirstOrDefault(sols => sols.Count > 0);
                    }

                    if (solutions != null)
                    {
                        solutions = SudokuSolutionNode.Consolidate(RemoveOverlapped(solutions.Distinct().ToList()));

                        if (solutions.Count == 0)
                            solutions = null;
                    }

                    if (solutions != null)
                    {
                        foreach (SudokuSolution solution in solutions)
                        {
                            SudokuSolutionNode node = a_node.AddNode(a_node.NextBoard, SudokuSolutionNodeState.Solution, solution);
                            node = node.AddNode(node.NextBoard, SudokuSolutionNodeState.State);
                            UpdateState(node, false);
                        }

                        a_node = a_node.Nodes.First().Nodes.First();
                    }
                    else
                        UpdateState(a_node, true);
                }
                else
                {
                    a_node = a_node.AddNode(a_node.NextBoard, SudokuSolutionNodeState.State);
                    UpdateState(a_node, false);
                }
            }
            else
            {
                if (a_node.Nodes.Any())
                    a_node = a_node.Nodes.First();
                else
                    UpdateState(a_node, true);
            }

            return a_node;
        }

        private void UpdateState(SudokuSolutionNode a_node, bool a_bEnd)
        {
            if (!a_node.Board.IsSolvable)
                a_node.State = SudokuSolutionNodeState.Unsolvable;
            else if (a_node.Board.IsSolved)
                a_node.State = SudokuSolutionNodeState.Solved;
            else if (a_bEnd)
                a_node.State = SudokuSolutionNodeState.Unsolved;
        }

        internal SudokuSolutionNode Solve(SudokuSolutionNode a_node)
        {
            if (a_node.Board.IsEmpty)
                return new SudokuSolutionNode(a_node.Board, SudokuSolutionNodeState.Unsolvable);

            for (;;)
            {
                if ((a_node.State == SudokuSolutionNodeState.Solved) ||
                    (a_node.State == SudokuSolutionNodeState.Unsolvable) ||
                    (a_node.State == SudokuSolutionNodeState.Unsolved))
                {
                    return a_node;
                }

                a_node = Step(a_node);
            }
        }

        internal SudokuSolutionNode Step(SudokuSolutionNode a_node, SudokuSolutionType a_type)
        {
            if (a_node.Board.IsEmpty)
                return new SudokuSolutionNode(a_node.Board, SudokuSolutionNodeState.Unsolvable);

            if (a_node.StepMode != SudokuSolutionNodeStepMode.StepAllAlgorithms)
            {
                Debug.Assert(!a_node.Nodes.Any());

                a_node.StepMode = SudokuSolutionNodeStepMode.StepSelectedAlgorithm;

                List<SudokuSolution> solution_nodes = SudokuSolutionTypeToSolvingFunction(a_type)(a_node.NextBoard, true);

                solution_nodes = SudokuSolutionNode.Consolidate(solution_nodes.Distinct().ToList());

                foreach (SudokuSolution solution in solution_nodes)
                {
                    if (!a_node.Nodes.Any(n => n.Solution.Equals(solution)))
                        a_node.AddNode(a_node.NextBoard, SudokuSolutionNodeState.Solution, solution);
                }
            }

            return a_node.Nodes.FirstOrDefault(node => node.Solution.Type == a_type);
        }

        internal void StepAll(SudokuSolutionNode a_node)
        {
            if (a_node.StepMode == SudokuSolutionNodeStepMode.StepAllAlgorithms)
                return;

            a_node.StepMode = SudokuSolutionNodeStepMode.StepAllAlgorithms;

            if ((a_node.State == SudokuSolutionNodeState.Solved) ||
                (a_node.State == SudokuSolutionNodeState.Unsolvable) ||
                (a_node.State == SudokuSolutionNodeState.Unsolved))
            {
                return;
            }

            if (a_node.Board.IsEmpty)
                return;

            if (a_node.State == SudokuSolutionNodeState.Solution)
            {
                if (!a_node.Nodes.Any())
                {
                    a_node = a_node.AddNode(a_node.NextBoard, SudokuSolutionNodeState.State);
                    UpdateState(a_node, false);
                }
            }
            else
            {
                ConcurrentBag<SudokuSolution> solution_nodes = new ConcurrentBag<SudokuSolution>();

                Parallel.ForEach(m_simple_funcs, func =>
                {
                    List<SudokuSolution> list = func(a_node.NextBoard, true);
                    foreach (var sol in list)
                        solution_nodes.Add(sol);
                });

                if (solution_nodes.Count == 0)
                {
                    Parallel.ForEach(m_complex_funcs, func =>
                    {
                        List<SudokuSolution> list = func(a_node.NextBoard, true);
                        foreach (var sol in list)
                            solution_nodes.Add(sol);
                    });
                }

                List<SudokuSolution> solution_nodes_1 = SudokuSolutionNode.Consolidate(solution_nodes.Distinct().ToList());

                foreach (SudokuSolution solution in solution_nodes_1)
                {
                    if (!a_node.Nodes.Any(n => n.Solution.Equals(solution)))
                        a_node.AddNode(a_node.NextBoard, SudokuSolutionNodeState.Solution, solution);
                }
            }
        }

        protected abstract List<SudokuSolution> Solve_MarkImpossibles(SudokuBoard a_board, bool a_all);
        protected abstract List<SudokuSolution> Solve_MarkSolved(SudokuBoard a_board, bool a_all);
        protected abstract List<SudokuSolution> Solve_SinglesInUnit(SudokuBoard a_board, bool a_all);
        protected abstract List<SudokuSolution> Solve_NakedPairs(SudokuBoard a_board, bool a_all);
        protected abstract List<SudokuSolution> Solve_NakedTriples(SudokuBoard a_board, bool a_all);
        protected abstract List<SudokuSolution> Solve_HiddenPairs(SudokuBoard a_board, bool a_all);
        protected abstract List<SudokuSolution> Solve_HiddenTriples(SudokuBoard a_board, bool a_all);
        protected abstract List<SudokuSolution> Solve_HiddenQuads(SudokuBoard a_board, bool a_all);
        protected abstract List<SudokuSolution> Solve_NakedQuads(SudokuBoard a_board, bool a_all);
        protected abstract List<SudokuSolution> Solve_PointingPairs(SudokuBoard a_board, bool a_all);
        protected abstract List<SudokuSolution> Solve_PointingTriples(SudokuBoard a_board, bool a_all);
        protected abstract List<SudokuSolution> Solve_BoxLineReduction(SudokuBoard a_board, bool a_all);
        protected abstract List<SudokuSolution> Solve_XWing(SudokuBoard a_board, bool a_all);
        protected abstract List<SudokuSolution> Solve_YWing(SudokuBoard a_board, bool a_all);
        protected abstract List<SudokuSolution> Solve_SwordFish(SudokuBoard a_board, bool a_all);
        protected abstract List<SudokuSolution> Solve_MultivalueXWing(SudokuBoard a_board, bool a_all);
        protected abstract List<SudokuSolution> Solve_JellyFish(SudokuBoard a_board, bool a_all);
        protected abstract List<SudokuSolution> Solve_XYZWing(SudokuBoard a_board, bool a_all);
        protected abstract List<SudokuSolution> Solve_WXYZWing(SudokuBoard a_board, bool a_all);
    }
}
