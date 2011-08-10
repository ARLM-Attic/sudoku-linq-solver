using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using TomanuExtensions;

namespace SudokuLib
{
    internal class SudokuSolverLINQ : SudokuSolverBase
    {
        protected override List<SudokuSolution> Solve_MarkImpossibles(SudokuBoard a_board, bool a_all)
        {
            var removed = (from unit in a_board.AllCellsUnits()
                           from cell1 in unit
                           where cell1.IsSolved
                           from num1 in cell1.Numbers()
                           where num1.IsSolved
                           from cell2 in unit
                           where cell2.Index != cell1.Index
                           where (cell2.Numbers().First(num2 => num2.Number == num1.Number).State == SudokuNumberState.sudokucellstatePossible)
                           select new
                           {
                               num = cell2.Numbers().First(num2 => num2.Number == num1.Number),
                               unit
                           }).Distinct().ToArray();

            if (removed.Length != 0)
                return new List<SudokuSolution>() { new SudokuSolution(SudokuSolutionType.MarkImpossibles, 
                                                                       removed.Select(obj => obj.num), null, null, 
                                                                       new [] { removed.Select(obj => obj.unit).SelectMany().Distinct() } ) };
            else
                return new List<SudokuSolution>();
        }

        protected override List<SudokuSolution> Solve_MarkSolved(SudokuBoard a_board, bool a_all)
        {
            var solved = (from cell in a_board.Cells()
                          where (cell.NumbersPossible().Count() == 1)
                          select cell.NumbersPossible().First()).Distinct().ToArray();

            if (solved.Length != 0)
                return new List<SudokuSolution>() { new SudokuSolution(SudokuSolutionType.MarkSolved, null, null, solved, null) };
            else
                return new List<SudokuSolution>();
        }

        protected override List<SudokuSolution> Solve_SinglesInUnit(SudokuBoard a_board, bool a_all)
        {
            var solved = (from unit in a_board.AllCellsUnits()
                          from cell in unit
                          from num in cell.NumbersPossible()
                          where unit.SelectMany(c => c.Numbers()).Where(n => n.Number == num.Number).Except(num).All(
                                    n => n.State == SudokuNumberState.sudokucellstateImpossible)
                          select new
                          {
                              num,
                              unit
                          }).ToArray();

            if (solved.Length > 0)
                return new List<SudokuSolution>() { new SudokuSolution(SudokuSolutionType.SinglesInUnit, null, null, solved.Select(obj => obj.num), 
                    new [] { solved.Select(obj => obj.unit).SelectMany().Distinct() } ) };

                return new List<SudokuSolution>();
        }

        protected override List<SudokuSolution> Solve_NakedPairs(SudokuBoard a_board, bool a_all)
        {
            var pairs = (from unit in a_board.AllCellsUnits()
                         from cell1 in unit
                         where (cell1.NumbersPossible().Count() == 2)
                         from cell2 in unit
                         where (cell1.Index < cell2.Index) &&
                               (cell2.NumbersPossible().Count() == 2) &&
                               (cell2.NumbersPossible().All(n2 => cell1.NumbersPossible().Any(n1 => n1.Number == n2.Number)))
                         select new
                         {
                             stayed = cell1.NumbersPossible().Concat(cell2.NumbersPossible()),
                             removed = (from cell3 in unit
                                        where (cell3.Index != cell1.Index) && (cell3.Index != cell2.Index)
                                        from num in cell3.NumbersPossible()
                                        where cell1.NumbersPossible().Any(n => num.Number == n.Number)
                                        select num).ToArray(),
                             unit = unit,
                         }).TakeAllOrOne(a_all, obj => obj.removed.Length > 0).ToArray();

            return new List<SudokuSolution>(from obj in pairs
                                            select new SudokuSolution(SudokuSolutionType.NakedPair, obj.removed, obj.stayed, null, new[] { obj.unit }));
        }

        protected override List<SudokuSolution> Solve_NakedTriples(SudokuBoard a_board, bool a_all)
        {
            var triples = (from unit in a_board.AllCellsUnits()
                           from cell1 in unit
                           where (cell1.NumbersPossible().Count() >= 1) &&
                                 (cell1.NumbersPossible().Count() <= 3)
                           from cell2 in unit
                           where (cell1.Index < cell2.Index) &&
                                 (cell2.NumbersPossible().Count() >= 1) &&
                                 (cell2.NumbersPossible().Count() <= 3)
                           from cell3 in unit
                           where (cell2.Index < cell3.Index) &&
                                 (cell3.NumbersPossible().Count() >= 1) &&
                                 (cell3.NumbersPossible().Count() <= 3) &&
                                 ((from num in cell1.NumbersPossible().Concat(cell2.NumbersPossible()).Concat(cell3.NumbersPossible())
                                  select num.Number).Distinct().Count() == 3)
                           select new
                           {
                               stayed = cell1.NumbersPossible().Concat(cell2.NumbersPossible()).Concat(cell3.NumbersPossible()),
                               removed = (from cell4 in unit
                                          where (cell4.Index != cell1.Index) && (cell4.Index != cell2.Index) && (cell4.Index != cell3.Index)
                                          from num in cell4.NumbersPossible()
                                          where cell1.NumbersPossible().Concat(cell2.NumbersPossible()).
                                                Concat(cell3.NumbersPossible()).Any(n => num.Number == n.Number)
                                          select num).ToArray(),
                               unit = unit
                           }).TakeAllOrOne(a_all, obj => obj.removed.Length > 0).ToArray();

            return new List<SudokuSolution>(from obj in triples
                                            select new SudokuSolution(SudokuSolutionType.NakedTriple, obj.removed, obj.stayed, null, new[] { obj.unit }));
        }

        protected override List<SudokuSolution> Solve_HiddenPairs(SudokuBoard a_board, bool a_all)
        {
            var cells_units = (from unit in a_board.AllCellsUnits()
                               select (from cell in unit
                                       where cell.NumbersPossible().Count() >= 2
                                       select new
                                       {
                                           cell,
                                           unit
                                       }).ToArray());

            var pairs1 = (from unit in cells_units
                          where (unit.Length >= 2) 
                          from cell_uint_1 in unit
                          from cell_uint_2 in unit
                          where (cell_uint_1.cell.Index < cell_uint_2.cell.Index)
                          select new
                          {
                              cells = new[] { cell_uint_1.cell, cell_uint_2.cell },
                              numbers = (from cell_unit in new[] { cell_uint_1, cell_uint_2 }
                                         from num in cell_unit.cell.NumbersPossible()
                                         select num.Number).Distinct().ToArray(),
                              restnumbers = (from cell in unit.Except(new[] { cell_uint_1, cell_uint_2 })
                                             from num in cell.cell.NumbersPossible()
                                             select num.Number).Distinct().ToArray(),
                              unit = cell_uint_1.unit
                          });

            var pairs2 = (from gr in pairs1
                          where (gr.numbers.Except(gr.restnumbers).Count() == 2)
                          select new
                          {
                              removed = (from cell in gr.cells
                                         from num in cell.NumbersPossible()
                                         where gr.numbers.Intersect(gr.restnumbers).Contains(num.Number)
                                         select num).ToArray(),
                              stayed = from cell in gr.cells
                                       from num in cell.NumbersPossible()
                                       where gr.numbers.Except(gr.restnumbers).Contains(num.Number)
                                       select num,
                              unit = gr.unit
                          }).TakeAllOrOne(a_all, obj => obj.removed.Length > 0).ToArray();

            return new List<SudokuSolution>(from obj in pairs2
                                            select new SudokuSolution(SudokuSolutionType.HiddenPair, obj.removed, obj.stayed, null, new[] { obj.unit }));
        }

        protected override List<SudokuSolution> Solve_HiddenTriples(SudokuBoard a_board, bool a_all)
        {
            var cells_units = (from unit in a_board.AllCellsUnits()
                               select (from cell in unit
                                       where cell.NumbersPossible().Any()
                                       select new
                                       {
                                           cell,
                                           unit
                                       }).ToArray());

            var triples1 = (from unit in cells_units
                            where (unit.Length >= 3)
                            from cell_unit_1 in unit
                            from cell_unit_2 in unit
                            where (cell_unit_1.cell.Index < cell_unit_2.cell.Index)
                            from cell_unit_3 in unit
                            where (cell_unit_2.cell.Index < cell_unit_3.cell.Index)
                            select new
                            {
                                cells = new[] { cell_unit_1.cell, cell_unit_2.cell, cell_unit_3.cell },
                                numbers = (from cell_unit in new[] { cell_unit_1, cell_unit_2, cell_unit_3 }
                                           from num in cell_unit.cell.NumbersPossible()
                                           select num.Number).Distinct().ToArray(),
                                restnumbers = (from cell_unit in unit.Except(new[] { cell_unit_1, cell_unit_2, cell_unit_3 })
                                               from num in cell_unit.cell.NumbersPossible()
                                               select num.Number).Distinct().ToArray(),
                                unit = cell_unit_1.unit
                            });

            var triples2 = (from gr in triples1
                            where (gr.numbers.Except(gr.restnumbers).Count() == 3)
                            select new
                            {
                                removed = (from cell in gr.cells
                                           from num in cell.NumbersPossible()
                                           where gr.numbers.Intersect(gr.restnumbers).Contains(num.Number)
                                           select num).ToArray(),
                                stayed = from cell in gr.cells
                                         from num in cell.NumbersPossible()
                                         where gr.numbers.Except(gr.restnumbers).Contains(num.Number)
                                         select num,
                                unit = gr.unit
                            }).TakeAllOrOne(a_all, obj => obj.removed.Length > 0).ToArray();

            return new List<SudokuSolution>(from obj in triples2
                                            select new SudokuSolution(SudokuSolutionType.HiddenTriple, obj.removed, obj.stayed, null, new[] { obj.unit }));
        }

        protected override List<SudokuSolution> Solve_HiddenQuads(SudokuBoard a_board, bool a_all)
        {
            var cells_units = (from unit in a_board.AllCellsUnits()
                               select (from cell in unit
                                       where cell.NumbersPossible().Any()
                                       select new 
                                       { 
                                           cell,
                                           unit
                                       }).ToArray());

            var quads1 = (from unit in cells_units
                          where (unit.Length >= 4)
                          from cell_unit_1 in unit
                          from cell_unit_2 in unit
                          where (cell_unit_1.cell.Index < cell_unit_2.cell.Index)
                          from cell_unit_3 in unit
                          where (cell_unit_2.cell.Index < cell_unit_3.cell.Index)
                          from cell_unit_4 in unit
                          where (cell_unit_3.cell.Index < cell_unit_4.cell.Index)
                          select new
                          {
                              cells = new[] { cell_unit_1.cell, cell_unit_2.cell, cell_unit_3.cell, cell_unit_4.cell },
                              numbers = (from cell_unit in new[] { cell_unit_1, cell_unit_2, cell_unit_3, cell_unit_4 }
                                         from num in cell_unit.cell.NumbersPossible()
                                         select num.Number).Distinct().ToArray(),
                              restnumbers = (from cell_unit in unit.Except(new[] { cell_unit_1, cell_unit_2, cell_unit_3, cell_unit_4 })
                                             from num in cell_unit.cell.NumbersPossible()
                                             select num.Number).Distinct().ToArray(),
                              unit = cell_unit_1.unit
                          });

            var quads2 = (from gr in quads1
                          where (gr.numbers.Except(gr.restnumbers).Count() == 4)
                          select new
                          {
                              removed = (from cell in gr.cells
                                         from num in cell.NumbersPossible()
                                         where gr.numbers.Intersect(gr.restnumbers).Contains(num.Number)
                                         select num).ToArray(),
                              stayed = from cell in gr.cells
                                       from num in cell.NumbersPossible()
                                       where gr.numbers.Except(gr.restnumbers).Contains(num.Number)
                                       select num,
                              unit = gr.unit
                          }).TakeAllOrOne(a_all, obj => obj.removed.Length > 0).ToArray();

            return new List<SudokuSolution>(from obj in quads2
                                            select new SudokuSolution(SudokuSolutionType.HiddenQuad, obj.removed, obj.stayed, null, new[] { obj.unit }));
        }

        protected override List<SudokuSolution> Solve_NakedQuads(SudokuBoard a_board, bool a_all)
        {
            var quads = (from unit in a_board.AllCellsUnits()
                         from cell1 in unit
                         where (cell1.NumbersPossible().Count() >= 1) &&
                               (cell1.NumbersPossible().Count() <= 4)
                         from cell2 in unit
                         where (cell1.Index < cell2.Index) &&
                               (cell2.NumbersPossible().Count() >= 1) &&
                               (cell2.NumbersPossible().Count() <= 4)
                         from cell3 in unit
                         where (cell2.Index < cell3.Index) &&
                               (cell3.NumbersPossible().Count() >= 1) &&
                               (cell3.NumbersPossible().Count() <= 4)
                         from cell4 in unit
                         where (cell3.Index < cell4.Index) &&
                               (cell4.NumbersPossible().Count() >= 1) &&
                               (cell4.NumbersPossible().Count() <= 4) && 
                               ((from num in cell1.NumbersPossible().Concat(cell2.NumbersPossible()).
                                            Concat(cell3.NumbersPossible()).Concat(cell4.NumbersPossible())
                                select num.Number).Distinct().Count() == 4)
                         select new
                         {
                             stayed = cell1.NumbersPossible().Concat(cell2.NumbersPossible()).
                                      Concat(cell3.NumbersPossible()).Concat(cell4.NumbersPossible()),
                             removed = (from cell5 in unit
                                        where (cell5.Index != cell1.Index) &&
                                              (cell5.Index != cell2.Index) &&
                                              (cell5.Index != cell3.Index) &&
                                              (cell5.Index != cell4.Index)
                                        from num in cell5.NumbersPossible()
                                        where cell1.NumbersPossible().Concat(cell2.NumbersPossible()).
                                              Concat(cell3.NumbersPossible()).Concat(cell4.NumbersPossible()).Any(n => num.Number == n.Number)
                                        select num).ToArray(),
                             unit = unit
                         }).TakeAllOrOne(a_all, obj => obj.removed.Length > 0).ToArray();

            return new List<SudokuSolution>(from obj in quads
                                            select new SudokuSolution(SudokuSolutionType.NakedQuad, obj.removed, obj.stayed, null, new[] { obj.unit }));
        }

        protected override List<SudokuSolution> Solve_PointingPairs(SudokuBoard a_board, bool a_all)
        {
            var pairs = (from box in a_board.BoxesNumbers()
                         from num1 in box
                         where num1.State == SudokuNumberState.sudokucellstatePossible
                         from num2 in box
                         where (num1.Row == num2.Row) || (num1.Col == num2.Col)
                         where (num2.State == SudokuNumberState.sudokucellstatePossible) &&
                               (num1.Index < num2.Index) &&
                               (num1.Number == num2.Number) &&
                               (from n in box
                                where (n.Index != num1.Index) &&
                                      (n.Index != num2.Index) &&
                                      (n.Number == num1.Number)
                                select n).All(n => n.State != SudokuNumberState.sudokucellstatePossible)
                         select new
                         {
                             removed = (num1.Row == num2.Row) ? (from num in num1.RowNumbers().Except(num1.BoxNumbers())
                                                                 where (num.State == SudokuNumberState.sudokucellstatePossible) &&
                                                                       (num.Number == num1.Number)
                                                                 select num).ToArray()
                                                              :
                                                                (from num in num1.ColNumbers().Except(num1.BoxNumbers())
                                                                 where (num.State == SudokuNumberState.sudokucellstatePossible) &&
                                                                       (num.Number == num1.Number)
                                                                 select num).ToArray(),

                             stayed = new[] { num1, num2 },
                             unit2 = num1.BoxCells(),
                             unit1 = (num1.Row == num2.Row) ? num1.RowCells() : num1.ColCells()
                         }).TakeAllOrOne(a_all, obj => obj.removed.Length > 0).ToArray();

            return new List<SudokuSolution>(from obj in pairs
                                            select new SudokuSolution(SudokuSolutionType.PointingPair, obj.removed, obj.stayed, null, new[] { obj.unit1, obj.unit2 }));
        }

        protected override List<SudokuSolution> Solve_PointingTriples(SudokuBoard a_board, bool a_all)
        {
            var triples = (from box in a_board.BoxesNumbers()
                           from num1 in box
                           where num1.State == SudokuNumberState.sudokucellstatePossible
                           from num2 in box
                           where (num2.State == SudokuNumberState.sudokucellstatePossible) &&
                                 (num1.Index < num2.Index) &&
                                 (num1.Number == num2.Number) && 
                                 ((num1.Row == num2.Row) || (num1.Col == num2.Col))
                           from num3 in box
                           where (num3.State == SudokuNumberState.sudokucellstatePossible) &&
                                 (num2.Index < num3.Index) &&
                                 (num2.Number == num3.Number) &&
                                 (((num1.Row == num2.Row) && (num2.Row == num3.Row)) || ((num1.Col == num2.Col) && (num2.Col == num3.Col))) && 
                                 (from n in box
                                  where (n.Index != num1.Index) &&
                                        (n.Index != num2.Index) &&
                                        (n.Index != num3.Index) &&
                                        (n.Number == num1.Number)
                                  select n).All(n => n.State != SudokuNumberState.sudokucellstatePossible) 
                           select new
                           {
                               removed = ((num1.Row == num2.Row) && (num2.Row == num3.Row)) ?
                                         (from num in num1.RowNumbers().Except(num1.BoxNumbers())
                                          where (num.State == SudokuNumberState.sudokucellstatePossible) &&
                                                (num.Number == num1.Number)
                                          select num).ToArray() :
                                          (from num in num1.ColNumbers().Except(num1.BoxNumbers())
                                           where (num.State == SudokuNumberState.sudokucellstatePossible) &&
                                                 (num.Number == num1.Number)
                                           select num).ToArray(),
                               stayed = new[] { num1, num2, num3 },
                               unit2 = num1.BoxCells(),
                               unit1 = (num1.Row == num2.Row) ? num1.RowCells() : num1.ColCells()
                           }).TakeAllOrOne(a_all, obj => obj.removed.Length > 0).ToArray();

            return new List<SudokuSolution>(from obj in triples
                                            select new SudokuSolution(SudokuSolutionType.PointingTriple, obj.removed, obj.stayed, null, 
                                                                      new[] { obj.unit1, obj.unit2 }));
        }

        protected override List<SudokuSolution> Solve_BoxLineReduction(SudokuBoard a_board, bool a_all)
        {
            var groups = (from unit in a_board.RowsNumbers().Concat(a_board.ColsNumbers())
                          select (from num in unit
                                  where num.State == SudokuNumberState.sudokucellstatePossible
                                  group num by num.Number into gr
                                  where (gr.Count() >= 2) &&
                                        (gr.Count() <= 3) && 
                                        (gr.Select(n => n.Box).Distinct().Count() == 1)
                                  select new
                                  {
                                      removed = (from cell in gr.First().BoxCells()
                                                 from num in cell.NumbersPossible().Where(n => n.Number == gr.First().Number).Except(gr)
                                                 select num).ToArray(),
                                      stayed = gr,
                                      unit1 = unit,
                                      unit2 = gr.First().BoxCells()
                                  })).SelectMany().TakeAllOrOne(a_all, obj => obj.removed.Length > 0).ToArray();

            return new List<SudokuSolution>(from obj in groups
                                            select new SudokuSolution(SudokuSolutionType.BoxLineReduction, obj.removed, obj.stayed, null, 
                                                                      new[] { obj.unit2, obj.unit1.Select(num => num.Cell).Distinct() }));
        }

        protected override List<SudokuSolution> Solve_XWing(SudokuBoard a_board, bool a_all)
        {
            if (SudokuOptions.Current.IncludeBoxes)
            {
                var units1 = (from unit in a_board.AllCellsUnits()
                              select new
                              {
                                  unit = unit.ToArray(),
                                  nums = (from cell in unit
                                          from n1 in cell.NumbersPossible()
                                          group n1 by n1.Number into gr
                                          select gr.ToArray())
                              });

                var units2 = (from obj in units1
                              from nums in obj.nums
                              where (nums.Length >= 2)
                              group new
                              {
                                  unit = obj.unit,
                                  nums = nums,
                              } by nums.First().Number into gr
                              select gr.ToArray()).ToArray();

                var units3 = (from nums_gr in units2
                              where (nums_gr.Length >= 4)
                              select (from obj in nums_gr
                                      select new
                                      {
                                          unit = obj.unit,
                                          nums = obj.nums,
                                          cross_units = (from obj2 in nums_gr
                                                         where !Object.ReferenceEquals(obj, obj2) &&
                                                               obj2.nums.ContainsAny(obj.nums)
                                                         select obj2).ToArray()
                                      }).ToArray()).ToArray();

                var units4 = (from nums_gr in units3
                              select (from obj in nums_gr
                                      where (obj.cross_units.Length >= 2)
                                      select new
                                      {
                                          unit = obj.unit,
                                          nums = obj.nums,
                                          cross_units = obj.cross_units,
                                          index = obj.unit.First().Index
                                      }).ToArray()).ToArray();

                var xwings = (from nums_gr in units4
                              where (nums_gr.Length >= 4)
                              from unit1 in nums_gr
                              from unit2 in nums_gr
                              where (unit1.index <= unit2.index) &&
                                    !Object.ReferenceEquals(unit1, unit2) &&
                                    !unit2.nums.ContainsAny(unit1.nums) &&
                                    (unit1.cross_units.Intersect(unit2.cross_units).Count() >= 2)
                              from cross_nums in
                                  new[] { (from u1 in unit1.cross_units.Intersect(unit2.cross_units)
                                       from u2 in nums_gr
                                       where Object.ReferenceEquals(u1.unit, u2.unit)
                                       select u2).ToArray() }
                              where (cross_nums.Length >= 2)
                              from unit3 in cross_nums
                              from unit4 in cross_nums
                              where unit3.index <= unit4.index &&
                                    !Object.ReferenceEquals(unit3, unit4) &&
                                    !unit4.nums.ContainsAny(unit3.nums)
                              where unit3.nums.Concat(unit4.nums).ContainsExact(unit1.nums.Concat(unit2.nums))
                              select new
                              {
                                  stayed = unit1.nums.Concat(unit2.nums),
                                  removed = unit3.nums.Concat(unit4.nums).Except(unit1.nums.Concat(unit2.nums)).ToArray(),
                                  unit1 = unit1.unit,
                                  unit2 = unit2.unit,
                                  unit3 = unit3.unit,
                                  unit4 = unit4.unit
                              }).TakeAllOrOne(a_all, obj => obj.removed.Length > 0).ToArray();

                return new List<SudokuSolution>(from obj in xwings
                                                select new SudokuSolution(SudokuSolutionType.XWing, obj.removed, obj.stayed, null,
                                                                          new[] { obj.unit1, obj.unit2, obj.unit3, obj.unit4 }));
            }
            else
            {
                var units1 = (from unit in a_board.RowsCells()
                              select new
                              {
                                  unit = unit.ToArray(),
                                  nums = (from cell in unit
                                          from n1 in cell.NumbersPossible()
                                          group n1 by n1.Number into gr
                                          select gr.ToArray()),
                                  row = true
                              }).Concat(from unit in a_board.ColsCells()
                              select new
                              {
                                  unit = unit.ToArray(),
                                  nums = (from cell in unit
                                          from n1 in cell.NumbersPossible()
                                          group n1 by n1.Number into gr
                                          select gr.ToArray()),
                                  row = false
                              });

                var units2 = (from obj in units1
                              from nums in obj.nums
                              where (nums.Length >= 2)
                              group new
                              {
                                  unit = obj.unit,
                                  nums = nums,
                                  row = obj.row
                              } by nums.First().Number into gr
                              select gr.ToArray()).ToArray();

                var units3 = (from nums_gr in units2
                              where (nums_gr.Length >= 4)
                              select (from obj in nums_gr
                                      select new
                                      {
                                          unit = obj.unit,
                                          nums = obj.nums,
                                          cross_units = (from obj2 in nums_gr
                                                         where !Object.ReferenceEquals(obj, obj2) &&
                                                               (obj2.row != obj.row) &&
                                                               obj2.nums.ContainsAny(obj.nums)
                                                         select obj2).ToArray(),
                                          row = obj.row,
                                      }).ToArray()).ToArray();

                var units4 = (from nums_gr in units3
                              select (from obj in nums_gr
                                      where (obj.cross_units.Length >= 2)
                                      select new
                                      {
                                          unit = obj.unit,
                                          nums = obj.nums,
                                          cross_units = obj.cross_units,
                                          index = obj.unit.First().Index,
                                          row = obj.row,
                                      }).ToArray()).ToArray();

                var xwings = (from nums_gr in units4
                              where (nums_gr.Length >= 4)
                              from unit1 in nums_gr
                              from unit2 in nums_gr
                              where (unit1.index < unit2.index) &&
                                    (unit1.row == unit2.row) && 
                                    (unit1.cross_units.Intersect(unit2.cross_units).Count() >= 2)
                              from cross_nums in
                                  new[] { (from u1 in unit1.cross_units.Intersect(unit2.cross_units)
                                       from u2 in nums_gr
                                       where Object.ReferenceEquals(u1.unit, u2.unit) 
                                       select u2).ToArray() }
                              from unit3 in cross_nums
                              from unit4 in cross_nums
                              where (unit3.index < unit4.index) &&
                                    unit3.nums.Concat(unit4.nums).ContainsExact(unit1.nums.Concat(unit2.nums))
                              select new
                              {
                                  stayed = unit1.nums.Concat(unit2.nums),
                                  removed = unit3.nums.Concat(unit4.nums).Except(unit1.nums.Concat(unit2.nums)).ToArray(),
                                  unit1 = unit1.unit,
                                  unit2 = unit2.unit,
                                  unit3 = unit3.unit,
                                  unit4 = unit4.unit
                              }).TakeAllOrOne(a_all, obj => obj.removed.Length > 0).ToArray();

                return new List<SudokuSolution>(from obj in xwings
                                                select new SudokuSolution(SudokuSolutionType.XWing, obj.removed, obj.stayed, null,
                                                                          new[] { obj.unit1, obj.unit2, obj.unit3, obj.unit4 }));
            }
        }

        protected override List<SudokuSolution> Solve_YWing(SudokuBoard a_board, bool a_all)
        {
            var ywings = (from cell1 in a_board.Cells()
                          where (cell1.NumbersPossible().Count() == 2)
                          from unit2 in new[] { cell1.ColCells(), cell1.RowCells() }
                          from cell2 in unit2
                          where (cell1.Box != cell2.Box) &&
                                (cell2.NumbersPossible().Count() == 2) &&
                                (cell1.NumbersPossible().Intersect(cell2.NumbersPossible(), Comparators.SudokuNumberComparer.Instance).Count() == 1)
                          from unit3 in new[] { cell2.ColCells(), cell2.RowCells(), cell2.BoxCells() }
                          from cell3 in unit3
                          where (cell3.Row != cell1.Row) &&
                                (cell3.Col != cell1.Col) &&
                                (cell3.NumbersPossible().Count() == 2) &&
                                (cell3.NumbersPossible().Intersect(cell1.NumbersPossible(), Comparators.SudokuNumberComparer.Instance).Count() == 1) &&
                                (cell3.NumbersPossible().Intersect(cell2.NumbersPossible(), Comparators.SudokuNumberComparer.Instance).Count() == 1) &&
                                !cell1.NumbersPossible().Intersect(cell2.NumbersPossible(), Comparators.SudokuNumberComparer.Instance).
                                     Intersect(cell3.NumbersPossible(), Comparators.SudokuNumberComparer.Instance).Any()
                          from unit1 in new[] { cell1.ColNumbers(), cell1.RowNumbers(), cell1.BoxNumbers() }
                          from unit4 in new[] { cell3.BoxNumbers(), cell3.ColNumbers(), cell3.RowNumbers() }
                          select new
                          {
                              stayed = cell1.NumbersPossible().Concat(cell2.NumbersPossible()).Concat(cell3.NumbersPossible()),
                              removed = (from num in unit1.Intersect(unit4)
                                         where num.State == SudokuNumberState.sudokucellstatePossible
                                         where (from n in cell1.NumbersPossible() select n.Number).
                                                   Intersect(from n in cell3.NumbersPossible() select n.Number).Contains(num.Number)
                                         select num).ToArray(),
                              unit1 = unit1,
                              unit2 = unit3,
                              unit3 = unit2,
                              unit4 = unit4,
                          }).TakeAllOrOne(a_all, obj => obj.removed.Length > 0).ToArray();

            return new List<SudokuSolution>(from obj in ywings
                                            select new SudokuSolution(SudokuSolutionType.YWing, obj.removed, obj.stayed, null, 
                                                                      new[] { obj.unit1.Select(num => num.Cell).Distinct(), 
                                                                              obj.unit2, obj.unit3, obj.unit4.Select(num => num.Cell).Distinct() }));
        }

        protected override List<SudokuSolution> Solve_SwordFish(SudokuBoard a_board, bool a_all)
        {
            if (SudokuOptions.Current.IncludeBoxes)
            {
                var units1 = (from unit in a_board.AllCellsUnits()
                              select new
                              {
                                  unit = unit.ToArray(),
                                  nums = (from cell in unit
                                          from n1 in cell.NumbersPossible()
                                          group n1 by n1.Number into gr
                                          select gr.ToArray())
                              });

                var units2 = (from obj in units1
                              from nums in obj.nums
                              where (nums.Length >= 2)
                              group new
                              {
                                  unit = obj.unit,
                                  nums = nums,
                              } by nums.First().Number into gr
                              select gr.ToArray()).ToArray();

                var units3 = (from nums_gr in units2
                              where (nums_gr.Length >= 6)
                              select (from obj in nums_gr
                                      select new
                                      {
                                          unit = obj.unit,
                                          nums = obj.nums,
                                          cross_units = (from obj2 in nums_gr
                                                         where !Object.ReferenceEquals(obj, obj2) &&
                                                               obj2.nums.ContainsAny(obj.nums)
                                                         select obj2).ToArray()
                                      }).ToArray()).ToArray();

                var units4 = (from nums_gr in units3
                              select (from obj in nums_gr
                                      where (obj.cross_units.Length >= 2)
                                      select new
                                      {
                                          unit = obj.unit,
                                          nums = obj.nums,
                                          cross_units = obj.cross_units,
                                          index = obj.unit.First().Index
                                      }).ToArray()).ToArray();

                var swordfishes = (from nums_gr in units4
                                   where (nums_gr.Length >= 6)
                                   from unit1 in nums_gr
                                   from unit2 in nums_gr
                                   where unit1.index <= unit2.index &&
                                         !Object.ReferenceEquals(unit1, unit2) &&
                                         !unit2.nums.ContainsAny(unit1.nums) &&
                                         unit1.cross_units.ContainsAny(unit2.cross_units)
                                   from unit3 in nums_gr
                                   where unit2.index <= unit3.index &&
                                         !Object.ReferenceEquals(unit2, unit3) &&
                                         !unit3.nums.ContainsAny(unit1.nums) &&
                                         !unit3.nums.ContainsAny(unit2.nums) &&
                                         unit3.cross_units.ContainsAny(unit1.cross_units) &&
                                         unit3.cross_units.ContainsAny(unit2.cross_units)
                                   from units_123 in new[] { new[] { unit1, unit2, unit3 } }
                                   where ((from u in units_123
                                           from cross_unit in u.cross_units
                                           group u by cross_unit into gr
                                           where gr.Count() >= 2
                                           from u in gr
                                           select u).Distinct().Count() == 3)
                                   from cross_nums in
                                       new[] { (from u1 in (from u in units_123
                                                        from cross_unit in u.cross_units
                                                        group u by cross_unit into gr
                                                        where gr.Count() >= 2
                                                        select gr.Key)
                                            from u2 in nums_gr
                                            where Object.ReferenceEquals(u1.unit, u2.unit)
                                            select u2).Except(units_123).ToArray() }
                                   where (cross_nums.Length >= 3)
                                   from cross_units_123 in new[] { units_123.SelectMany(u => u.cross_units).Select(u => u.unit).ToArray() }
                                   from unit4 in cross_nums
                                   where (cross_units_123.Count(u => Object.ReferenceEquals(u, unit4.unit)) >= 2)
                                   from unit5 in cross_nums
                                   where unit4.index <= unit5.index &&
                                         !Object.ReferenceEquals(unit4, unit5) &&
                                         !unit5.nums.ContainsAny(unit4.nums) &&
                                         (cross_units_123.Count(u => Object.ReferenceEquals(u, unit5.unit)) >= 2)
                                   from unit6 in cross_nums
                                   where unit5.index <= unit6.index &&
                                         !Object.ReferenceEquals(unit5, unit6) &&
                                         !unit6.nums.ContainsAny(unit4.nums) &&
                                         !unit6.nums.ContainsAny(unit5.nums) &&
                                         (cross_units_123.Count(u => Object.ReferenceEquals(u, unit6.unit)) >= 2)
                                   from units_456 in new[] { new[] { unit4, unit5, unit6 } }
                                   from cross_units_456 in new[] { units_456.SelectMany(u => u.cross_units).Select(u => u.unit).ToArray() }
                                   where units_123.All(u => (cross_units_456.Count(uu => Object.ReferenceEquals(uu, u.unit)) >= 2)) &&
                                         units_456.SelectMany(u => u.nums).ContainsExact(units_123.SelectMany(u => u.nums))
                                   select new
                                   {
                                       stayed = units_123.SelectMany(u => u.nums),
                                       removed = units_456.SelectMany(u => u.nums).Except(units_123.SelectMany(u => u.nums)).ToArray(),
                                       unit1 = unit4.unit,
                                       unit2 = unit5.unit,
                                       unit3 = unit6.unit,
                                       unit4 = unit1.unit,
                                       unit5 = unit2.unit,
                                       unit6 = unit3.unit,
                                   }).TakeAllOrOne(a_all, obj => obj.removed.Length > 0).ToArray();

                return new List<SudokuSolution>(from obj in swordfishes
                                                select new SudokuSolution(SudokuSolutionType.SwordFish, obj.removed, obj.stayed, null,
                                                                          new[] { obj.unit1, obj.unit2, obj.unit3, obj.unit4, obj.unit5, obj.unit6 }));
            }
            else
            {
                var units1 = (from unit in a_board.RowsCells()
                              select new
                              {
                                  unit = unit.ToArray(),
                                  nums = (from cell in unit
                                          from n1 in cell.NumbersPossible()
                                          group n1 by n1.Number into gr
                                          select gr.ToArray()),
                                  row = true
                              }).Concat(from unit in a_board.ColsCells()
                                        select new
                                        {
                                            unit = unit.ToArray(),
                                            nums = (from cell in unit
                                                    from n1 in cell.NumbersPossible()
                                                    group n1 by n1.Number into gr
                                                    select gr.ToArray()),
                                            row = false
                                        });

                var units2 = (from obj in units1
                              from nums in obj.nums
                              where (nums.Length >= 2)
                              group new
                              {
                                  unit = obj.unit,
                                  nums = nums,
                                  row = obj.row
                              } by nums.First().Number into gr
                              select gr.ToArray()).ToArray();

                var units3 = (from nums_gr in units2
                              where (nums_gr.Length >= 6)
                              select (from obj in nums_gr
                                      select new
                                      {
                                          unit = obj.unit,
                                          nums = obj.nums,
                                          cross_units = (from obj2 in nums_gr
                                                         where !Object.ReferenceEquals(obj, obj2) && 
                                                               (obj.row != obj2.row) &&
                                                               obj2.nums.ContainsAny(obj.nums)
                                                         select obj2).ToArray(),
                                          row = obj.row
                                      }).ToArray()).ToArray();

                var units4 = (from nums_gr in units3
                              select (from obj in nums_gr
                                      where (obj.cross_units.Length >= 2)
                                      select new
                                      {
                                          unit = obj.unit,
                                          nums = obj.nums,
                                          cross_units = obj.cross_units,
                                          index = obj.unit.First().Index,
                                          row = obj.row
                                      }).ToArray()).ToArray();

                var swordfishes = (from nums_gr in units4
                                   where (nums_gr.Length >= 6)
                                   from unit1 in nums_gr
                                   from unit2 in nums_gr
                                   where (unit1.index < unit2.index) && 
                                         (unit1.row == unit2.row) &&
                                         unit1.cross_units.ContainsAny(unit2.cross_units)
                                   from unit3 in nums_gr
                                   where (unit2.index < unit3.index) &&
                                         (unit2.row == unit3.row) &&
                                         unit3.cross_units.ContainsAny(unit1.cross_units) &&
                                         unit3.cross_units.ContainsAny(unit2.cross_units)
                                   from units_123 in new[] { new[] { unit1, unit2, unit3 } }
                                   where ((from u in units_123
                                           from cross_unit in u.cross_units
                                           group u by cross_unit into gr
                                           where gr.Count() >= 2
                                           from u in gr
                                           select u).Distinct().Count() == 3)
                                   from cross_nums in
                                       new[] { (from u1 in (from u in units_123
                                                            from cross_unit in u.cross_units
                                                            group u by cross_unit into gr
                                                            where gr.Count() >= 2
                                                            select gr.Key)
                                                from u2 in nums_gr
                                                where Object.ReferenceEquals(u1.unit, u2.unit)
                                                select u2).ToArray() }

                                   where (cross_nums.Length >= 3)
                                   from cross_units_123 in new[] { units_123.SelectMany(u => u.cross_units).Select(u => u.unit).ToArray() }
                                   from unit4 in cross_nums
                                   where (cross_units_123.Count(u => Object.ReferenceEquals(u, unit4.unit)) >= 2)
                                   from unit5 in cross_nums
                                   where unit4.index < unit5.index &&
                                         (cross_units_123.Count(u => Object.ReferenceEquals(u, unit5.unit)) >= 2)
                                   from unit6 in cross_nums
                                   where unit5.index < unit6.index &&
                                         (cross_units_123.Count(u => Object.ReferenceEquals(u, unit6.unit)) >= 2)
                                   from units_456 in new[] { new[] { unit4, unit5, unit6 } }
                                   from cross_units_456 in new[] { units_456.SelectMany(u => u.cross_units).Select(u => u.unit).ToArray() }
                                   where units_123.All(u => (cross_units_456.Count(uu => Object.ReferenceEquals(uu, u.unit)) >= 2)) &&
                                         units_456.SelectMany(u => u.nums).ContainsExact(units_123.SelectMany(u => u.nums))
                                   select new
                                   {
                                       stayed = units_123.SelectMany(u => u.nums),
                                       removed = units_456.SelectMany(u => u.nums).Except(units_123.SelectMany(u => u.nums)).ToArray(),
                                       unit1 = unit4.unit,
                                       unit2 = unit5.unit,
                                       unit3 = unit6.unit,
                                       unit4 = unit1.unit,
                                       unit5 = unit2.unit,
                                       unit6 = unit3.unit,
                                   }).TakeAllOrOne(a_all, obj => obj.removed.Length > 0).ToArray();

                return new List<SudokuSolution>(from obj in swordfishes
                                                select new SudokuSolution(SudokuSolutionType.SwordFish, obj.removed, obj.stayed, null,
                                                                          new[] { obj.unit1, obj.unit2, obj.unit3, obj.unit4, obj.unit5, obj.unit6 }));
            }
        }
         
        protected override List<SudokuSolution> Solve_MultivalueXWing(SudokuBoard a_board, bool a_all)
        {
            if (SudokuOptions.Current.IncludeBoxes)
            {
                var units1 = (from unit in a_board.AllCellsUnits()
                              select new
                              {
                                  unit = unit,
                                  pairs = (from cell1 in unit
                                           where (cell1.NumbersPossible().Count() == 2)
                                           from cell2 in unit
                                           where (cell1.Index < cell2.Index) &&
                                                 (cell2.NumbersPossible().Count() == 2) &&
                                                 (cell1.NumbersPossible().Intersect(cell2.NumbersPossible(), Comparators.SudokuNumberComparer.Instance).Count() == 1)
                                           select new
                                           {
                                               cell1 = cell1,
                                               cell2 = cell2,
                                               nums1 = (from num in cell1.NumbersPossible()
                                                        select num.Number).ToArray(),
                                               nums2 = (from num in cell2.NumbersPossible()
                                                        select num.Number).ToArray()
                                           }).ToArray()
                              }).ToArray();

                var units2 = (from obj in units1
                              where obj.pairs.Length > 0
                              select obj).ToArray();

                var mxwings = (from unit1 in units2
                               from unit4 in units2
                               where (unit1.unit.First().Index <= unit4.unit.First().Index) &&
                                     !Object.ReferenceEquals(unit1, unit4) &&
                                     (!unit1.unit.ContainsAny(unit4.unit))
                               from pair1 in unit1.pairs
                               from pair4 in unit4.pairs
                               where (pair1.nums1.Intersect(pair4.nums1.Concat(pair4.nums2)).Count() == 1) &&
                                     (pair1.nums2.Intersect(pair4.nums1.Concat(pair4.nums2)).Count() == 1) &&
                                     (pair4.nums1.Intersect(pair1.nums1.Concat(pair1.nums2)).Count() == 1) &&
                                     (pair4.nums2.Intersect(pair1.nums1.Concat(pair1.nums2)).Count() == 1) &&
                                     (pair1.nums1.Intersect(pair1.nums2).First() != pair4.nums1.Intersect(pair4.nums2).First())
                               from unit2 in units2
                               from pair2 in unit2.pairs
                               where ((pair2.cell1.Index == pair1.cell1.Index) || (pair2.cell1.Index == pair1.cell2.Index)) &&
                                     ((pair2.cell2.Index == pair4.cell1.Index) || (pair2.cell2.Index == pair4.cell2.Index))
                               from unit3 in units2
                               where (unit2.unit.First().Index <= unit3.unit.First().Index) &&
                                     !Object.ReferenceEquals(unit2, unit3) &&
                                     !unit3.unit.ContainsAny(unit2.unit)
                               from pair3 in unit3.pairs
                               where ((pair3.cell1.Index == pair1.cell1.Index) || (pair3.cell1.Index == pair1.cell2.Index)) &&
                                     ((pair3.cell2.Index == pair4.cell1.Index) || (pair3.cell2.Index == pair4.cell2.Index))
                               select new
                               {
                                   stayed = pair1.cell1.NumbersPossible().Concat(pair1.cell2.NumbersPossible()).Concat(
                                            pair4.cell1.NumbersPossible()).Concat(pair4.cell2.NumbersPossible()).ToArray(),
                                   removed = (from cell in unit1.unit
                                              where (cell.Index != pair1.cell1.Index) &&
                                                    (cell.Index != pair1.cell2.Index)
                                              from num in cell.NumbersPossible()
                                              from toremove in pair1.nums1.Intersect(pair1.nums2)
                                              where (num.Number == toremove)
                                              select num).Concat(from cell in unit2.unit
                                                                 where (cell.Index != pair2.cell1.Index) &&
                                                                       (cell.Index != pair2.cell2.Index)
                                                                 from num in cell.NumbersPossible()
                                                                 from toremove in pair2.nums1.Intersect(pair2.nums2)
                                                                 where (num.Number == toremove)
                                                                 select num).Concat(from cell in unit3.unit
                                                                                    where (cell.Index != pair3.cell1.Index) &&
                                                                                          (cell.Index != pair3.cell2.Index)
                                                                                    from num in cell.NumbersPossible()
                                                                                    from toremove in pair3.nums1.Intersect(pair3.nums2)
                                                                                    where (num.Number == toremove)
                                                                                    select num).Concat(from cell in unit4.unit
                                                                                                       where (cell.Index != pair4.cell1.Index) &&
                                                                                                             (cell.Index != pair4.cell2.Index)
                                                                                                       from num in cell.NumbersPossible()
                                                                                                       from toremove in pair4.nums1.Intersect(pair4.nums2)
                                                                                                       where (num.Number == toremove)
                                                                                                       select num).ToArray(),
                                   unit1 = unit2.unit,
                                   unit2 = unit3.unit,
                                   unit3 = unit1.unit,
                                   unit4 = unit4.unit
                               }).TakeAllOrOne(a_all, obj => obj.removed.Length > 0).ToArray();

                return new List<SudokuSolution>(from obj in mxwings
                                                select new SudokuSolution(SudokuSolutionType.MultivalueXWing, obj.removed, obj.stayed, null,
                                                                          new[] { obj.unit1, obj.unit2, obj.unit3, obj.unit4 }));
            }
            else
            {
                var units1 = (from unit in a_board.ColsCells()
                              select new
                              {
                                  unit = unit,
                                  pairs = (from cell1 in unit
                                           where (cell1.NumbersPossible().Count() == 2)
                                           from cell2 in unit
                                           where (cell1.Index < cell2.Index) &&
                                                 (cell2.NumbersPossible().Count() == 2) &&
                                                 (cell1.NumbersPossible().Intersect(cell2.NumbersPossible(), 
                                                        Comparators.SudokuNumberComparer.Instance).Count() == 1)
                                           select new
                                           {
                                               cell1 = cell1,
                                               cell2 = cell2,
                                               nums1 = (from num in cell1.NumbersPossible()
                                                        select num.Number).ToArray(),
                                               nums2 = (from num in cell2.NumbersPossible()
                                                        select num.Number).ToArray()
                                           }).ToArray(),
                                  index = unit.First().Index,
                                  row = true
                              }).Concat(from unit in a_board.RowsCells()
                                        select new
                                        {
                                            unit = unit,
                                            pairs = (from cell1 in unit
                                                     where (cell1.NumbersPossible().Count() == 2)
                                                     from cell2 in unit
                                                     where (cell1.Index < cell2.Index) &&
                                                           (cell2.NumbersPossible().Count() == 2) &&
                                                           (cell1.NumbersPossible().Intersect(cell2.NumbersPossible(), 
                                                                Comparators.SudokuNumberComparer.Instance).Count() == 1)
                                                     select new
                                                     {
                                                         cell1 = cell1,
                                                         cell2 = cell2,
                                                         nums1 = (from num in cell1.NumbersPossible()
                                                                  select num.Number).ToArray(),
                                                         nums2 = (from num in cell2.NumbersPossible()
                                                                  select num.Number).ToArray()
                                                     }).ToArray(),
                                            index = unit.First().Index,
                                            row = false
                                        }).ToArray();

                var units2 = (from obj in units1
                              where obj.pairs.Length > 0
                              select obj).ToArray();

                var mxwings = (from unit1 in units2
                               from unit4 in units2
                               where (unit1.index < unit4.index) &&
                                     (unit1.row == unit4.row)
                               from pair1 in unit1.pairs
                               from pair4 in unit4.pairs
                               where (pair1.nums1.Intersect(pair4.nums1.Concat(pair4.nums2)).Count() == 1) &&
                                     (pair1.nums2.Intersect(pair4.nums1.Concat(pair4.nums2)).Count() == 1) &&
                                     (pair4.nums1.Intersect(pair1.nums1.Concat(pair1.nums2)).Count() == 1) &&
                                     (pair4.nums2.Intersect(pair1.nums1.Concat(pair1.nums2)).Count() == 1) &&
                                     (pair1.nums1.Intersect(pair1.nums2).First() != pair4.nums1.Intersect(pair4.nums2).First())
                               from unit2 in units2
                               where (unit2.row != unit1.row)
                               from pair2 in unit2.pairs
                               where ((pair2.cell1.Index == pair1.cell1.Index) || (pair2.cell1.Index == pair1.cell2.Index)) &&
                                     ((pair2.cell2.Index == pair4.cell1.Index) || (pair2.cell2.Index == pair4.cell2.Index))
                               from unit3 in units2
                               where (unit2.index < unit3.index) &&
                                     (unit2.row != unit1.row)
                               from pair3 in unit3.pairs
                               where ((pair3.cell1.Index == pair1.cell1.Index) || (pair3.cell1.Index == pair1.cell2.Index)) &&
                                     ((pair3.cell2.Index == pair4.cell1.Index) || (pair3.cell2.Index == pair4.cell2.Index))
                               select new
                               {
                                   stayed = pair1.cell1.NumbersPossible().Concat(pair1.cell2.NumbersPossible()).Concat(
                                            pair4.cell1.NumbersPossible()).Concat(pair4.cell2.NumbersPossible()).ToArray(),
                                   removed = (from cell in unit1.unit
                                              where (cell.Index != pair1.cell1.Index) &&
                                                    (cell.Index != pair1.cell2.Index)
                                              from num in cell.NumbersPossible()
                                              from toremove in pair1.nums1.Intersect(pair1.nums2)
                                              where (num.Number == toremove)
                                              select num).Concat(from cell in unit2.unit
                                                                 where (cell.Index != pair2.cell1.Index) &&
                                                                       (cell.Index != pair2.cell2.Index)
                                                                 from num in cell.NumbersPossible()
                                                                 from toremove in pair2.nums1.Intersect(pair2.nums2)
                                                                 where (num.Number == toremove)
                                                                 select num).Concat(from cell in unit3.unit
                                                                                    where (cell.Index != pair3.cell1.Index) &&
                                                                                          (cell.Index != pair3.cell2.Index)
                                                                                    from num in cell.NumbersPossible()
                                                                                    from toremove in pair3.nums1.Intersect(pair3.nums2)
                                                                                    where (num.Number == toremove)
                                                                                    select num).Concat(from cell in unit4.unit
                                                                                                       where (cell.Index != pair4.cell1.Index) &&
                                                                                                             (cell.Index != pair4.cell2.Index)
                                                                                                       from num in cell.NumbersPossible()
                                                                                                       from toremove in pair4.nums1.Intersect(pair4.nums2)
                                                                                                       where (num.Number == toremove)
                                                                                                       select num).ToArray(),
                                   unit1 = unit2.unit,
                                   unit2 = unit3.unit,
                                   unit3 = unit1.unit,
                                   unit4 = unit4.unit,

                                   xx = new [] { unit1.row, unit2.row, unit3.row, unit4.row }

                               }).TakeAllOrOne(a_all, obj => obj.removed.Length > 0).ToArray();

                return new List<SudokuSolution>(from obj in mxwings
                                                select new SudokuSolution(SudokuSolutionType.MultivalueXWing, obj.removed, obj.stayed, null,
                                                                          new[] { obj.unit1, obj.unit2, obj.unit3, obj.unit4 }));
            }
        }

        protected override List<SudokuSolution> Solve_JellyFish(SudokuBoard a_board, bool a_all)
        {
            if (SudokuOptions.Current.IncludeBoxes)
            {
                var units1 = (from unit in a_board.AllCellsUnits()
                              select new
                              {
                                  unit = unit.ToArray(),
                                  nums = (from cell in unit
                                          from n1 in cell.NumbersPossible()
                                          group n1 by n1.Number into gr
                                          select gr.ToArray())
                              });

                var units2 = (from obj in units1
                              from nums in obj.nums
                              where (nums.Length >= 2)
                              group new
                              {
                                  unit = obj.unit,
                                  nums = nums,
                              } by nums.First().Number into gr
                              select gr.ToArray()).ToArray();

                var units3 = (from nums_gr in units2
                              where (nums_gr.Length >= 8)
                              select (from obj in nums_gr
                                      select new
                                      {
                                          unit = obj.unit,
                                          nums = obj.nums,
                                          cross_units = (from obj2 in nums_gr
                                                         where !Object.ReferenceEquals(obj, obj2) &&
                                                               obj2.nums.ContainsAny(obj.nums)
                                                         select obj2).ToArray()
                                      }).ToArray()).ToArray();

                var units4 = (from nums_gr in units3
                              select (from obj in nums_gr
                                      where (obj.cross_units.Length >= 2)
                                      select new
                                      {
                                          unit = obj.unit,
                                          nums = obj.nums,
                                          cross_units = obj.cross_units,
                                          index = obj.unit.First().Index
                                      }).ToArray()).ToArray();

                var jellyfishes = (from nums_gr in units4
                                   where (nums_gr.Length >= 8)
                                   from unit1 in nums_gr
                                   from unit2 in nums_gr
                                   where unit1.index <= unit2.index &&
                                        !Object.ReferenceEquals(unit1, unit2) &&
                                        !unit2.nums.ContainsAny(unit1.nums)
                                   from unit3 in nums_gr
                                   where unit2.index <= unit3.index &&
                                         !Object.ReferenceEquals(unit2, unit3) &&
                                         !unit3.nums.ContainsAny(unit1.nums) &&
                                         !unit3.nums.ContainsAny(unit2.nums)
                                   from unit4 in nums_gr
                                   where unit3.index <= unit4.index &&
                                         !Object.ReferenceEquals(unit3, unit4) &&
                                         !unit4.nums.ContainsAny(unit1.nums) &&
                                         !unit4.nums.ContainsAny(unit2.nums) &&
                                         !unit4.nums.ContainsAny(unit3.nums)
                                   from units_1234 in new[] { new[] { unit1, unit2, unit3, unit4 } }
                                   where ((from u in units_1234
                                           from cross_unit in u.cross_units
                                           group u by cross_unit into gr
                                           where gr.Count() >= 2
                                           from u in gr
                                           select u).Distinct().Count() == 4)
                                   from cross_nums in
                                       new[] { (from u1 in (from u in units_1234
                                                        from cross_unit in u.cross_units
                                                        group u by cross_unit into gr
                                                        where gr.Count() >= 2
                                                        select gr.Key)
                                            from u2 in nums_gr
                                            where Object.ReferenceEquals(u1.unit, u2.unit)
                                            select u2).Except(units_1234).ToArray() }
                                   where (cross_nums.Length >= 4)
                                   from cross_units_1234 in new[] { units_1234.SelectMany(u => u.cross_units).Select(u => u.unit).ToArray() }
                                   from unit5 in cross_nums
                                   where (cross_units_1234.Count(u => Object.ReferenceEquals(u, unit5.unit)) >= 2)
                                   from unit6 in cross_nums
                                   where unit5.index <= unit6.index &&
                                         !Object.ReferenceEquals(unit5, unit6) &&
                                         !unit6.nums.ContainsAny(unit5.nums) &&
                                         (cross_units_1234.Count(u => Object.ReferenceEquals(u, unit6.unit)) >= 2)
                                   from unit7 in cross_nums
                                   where unit6.index <= unit7.index &&
                                         !Object.ReferenceEquals(unit6, unit7) &&
                                         !unit7.nums.ContainsAny(unit5.nums) &&
                                         !unit7.nums.ContainsAny(unit6.nums) &&
                                         (cross_units_1234.Count(u => Object.ReferenceEquals(u, unit7.unit)) >= 2)
                                   from unit8 in cross_nums
                                   where unit7.index <= unit8.index &&
                                         !Object.ReferenceEquals(unit7, unit8) &&
                                         !unit8.nums.ContainsAny(unit5.nums) &&
                                         !unit8.nums.ContainsAny(unit6.nums) &&
                                         !unit8.nums.ContainsAny(unit7.nums) &&
                                         (cross_units_1234.Count(u => Object.ReferenceEquals(u, unit8.unit)) >= 2)
                                   from units_5678 in new[] { new[] { unit5, unit6, unit7, unit8 } }
                                   from cross_units_5678 in new[] { units_5678.SelectMany(u => u.cross_units).Select(u => u.unit).ToArray() }
                                   where units_1234.All(u => (cross_units_5678.Count(uu => Object.ReferenceEquals(uu, u.unit)) >= 2)) &&
                                         units_5678.SelectMany(u => u.nums).ContainsExact(units_1234.SelectMany(u => u.nums)) &&

                                         unit1.cross_units.Concat(unit2.cross_units).Intersect(unit3.cross_units.Concat(unit4.cross_units)).Any() &&
                                         unit1.cross_units.Concat(unit3.cross_units).Intersect(unit2.cross_units.Concat(unit4.cross_units)).Any() &&
                                         unit1.cross_units.Concat(unit4.cross_units).Intersect(unit2.cross_units.Concat(unit3.cross_units)).Any()
                                   select new
                                   {
                                       stayed = units_1234.SelectMany(u => u.nums),
                                       removed = units_5678.SelectMany(u => u.nums).Except(units_1234.SelectMany(u => u.nums)).ToArray(),
                                       unit1 = unit5.unit,
                                       unit2 = unit6.unit,
                                       unit3 = unit7.unit,
                                       unit4 = unit8.unit,
                                       unit5 = unit1.unit,
                                       unit6 = unit2.unit,
                                       unit7 = unit3.unit,
                                       unit8 = unit4.unit,
                                   }).TakeAllOrOne(a_all, obj => obj.removed.Length > 0).ToArray();

                return new List<SudokuSolution>(from obj in jellyfishes
                                                select new SudokuSolution(SudokuSolutionType.JellyFish, obj.removed, obj.stayed, null,
                                                                          new[] { obj.unit1, obj.unit2, obj.unit3, obj.unit4, obj.unit5, 
                                                                              obj.unit6, obj.unit7, obj.unit8 }));
            }
            else
            {
                var units1 = (from unit in a_board.RowsCells()
                              select new
                              {
                                  unit = unit.ToArray(),
                                  nums = (from cell in unit
                                          from n1 in cell.NumbersPossible()
                                          group n1 by n1.Number into gr
                                          select gr.ToArray()),
                                  row = true
                              }).Concat(from unit in a_board.ColsCells()
                                        select new
                                        {
                                            unit = unit.ToArray(),
                                            nums = (from cell in unit
                                                    from n1 in cell.NumbersPossible()
                                                    group n1 by n1.Number into gr
                                                    select gr.ToArray()),
                                            row = false
                                        });

                var units2 = (from obj in units1
                              from nums in obj.nums
                              where (nums.Length >= 2)
                              group new
                              {
                                  unit = obj.unit,
                                  nums = nums,
                                  row = obj.row
                              } by nums.First().Number into gr
                              select gr.ToArray()).ToArray();

                var units3 = (from nums_gr in units2
                              where (nums_gr.Length >= 8)
                              select (from obj in nums_gr
                                      select new
                                      {
                                          unit = obj.unit,
                                          nums = obj.nums,
                                          cross_units = (from obj2 in nums_gr
                                                         where !Object.ReferenceEquals(obj, obj2) &&
                                                               (obj.row != obj2.row) &&
                                                               obj2.nums.ContainsAny(obj.nums)
                                                         select obj2).ToArray(),
                                          row = obj.row
                                      }).ToArray()).ToArray();

                var units4 = (from nums_gr in units3
                              select (from obj in nums_gr
                                      where (obj.cross_units.Length >= 2)
                                      select new
                                      {
                                          unit = obj.unit,
                                          nums = obj.nums,
                                          cross_units = obj.cross_units,
                                          index = obj.unit.First().Index,
                                          row = obj.row
                                      }).ToArray()).ToArray();

                var jellyfishes = (from nums_gr in units4
                                   where (nums_gr.Length >= 8)
                                   from unit1 in nums_gr
                                   from unit2 in nums_gr
                                   where (unit1.index < unit2.index) &&
                                         (unit1.row == unit2.row)
                                   from unit3 in nums_gr
                                   where (unit2.index < unit3.index) &&
                                          (unit1.row == unit3.row)
                                   from unit4 in nums_gr
                                   where (unit3.index < unit4.index) &&
                                         (unit1.row == unit4.row)
                                   from units_1234 in new[] { new[] { unit1, unit2, unit3, unit4 } }
                                   where ((from u in units_1234
                                           from cross_unit in u.cross_units
                                           group u by cross_unit into gr
                                           where gr.Count() >= 2
                                           from u in gr
                                           select u).Distinct().Count() == 4)
                                   from cross_nums in
                                       new[] { (from u1 in (from u in units_1234
                                                        from cross_unit in u.cross_units
                                                        group u by cross_unit into gr
                                                        where gr.Count() >= 2
                                                        select gr.Key)
                                            from u2 in nums_gr
                                            where Object.ReferenceEquals(u1.unit, u2.unit)
                                            select u2).Except(units_1234).ToArray() }
                                   where (cross_nums.Length >= 4)
                                   from cross_units_1234 in new[] { units_1234.SelectMany(u => u.cross_units).Select(u => u.unit).ToArray() }
                                   from unit5 in cross_nums
                                   where (cross_units_1234.Count(u => Object.ReferenceEquals(u, unit5.unit)) >= 2)
                                   from unit6 in cross_nums
                                   where unit5.index < unit6.index &&
                                         (cross_units_1234.Count(u => Object.ReferenceEquals(u, unit6.unit)) >= 2)
                                   from unit7 in cross_nums
                                   where unit6.index < unit7.index &&
                                         (cross_units_1234.Count(u => Object.ReferenceEquals(u, unit7.unit)) >= 2)
                                   from unit8 in cross_nums
                                   where unit7.index < unit8.index &&
                                         (cross_units_1234.Count(u => Object.ReferenceEquals(u, unit8.unit)) >= 2)
                                   from units_5678 in new[] { new[] { unit5, unit6, unit7, unit8 } }
                                   from cross_units_5678 in new[] { units_5678.SelectMany(u => u.cross_units).Select(u => u.unit).ToArray() }
                                   where units_1234.All(u => (cross_units_5678.Count(uu => Object.ReferenceEquals(uu, u.unit)) >= 2)) &&
                                         units_5678.SelectMany(u => u.nums).ContainsExact(units_1234.SelectMany(u => u.nums)) &&
                                         unit1.cross_units.Concat(unit2.cross_units).Intersect(unit3.cross_units.Concat(unit4.cross_units)).Any() &&
                                         unit1.cross_units.Concat(unit3.cross_units).Intersect(unit2.cross_units.Concat(unit4.cross_units)).Any() &&
                                         unit1.cross_units.Concat(unit4.cross_units).Intersect(unit2.cross_units.Concat(unit3.cross_units)).Any()
                                   select new
                                   {
                                       stayed = units_1234.SelectMany(u => u.nums),
                                       removed = units_5678.SelectMany(u => u.nums).Except(units_1234.SelectMany(u => u.nums)).ToArray(),
                                       unit1 = unit5.unit,
                                       unit2 = unit6.unit,
                                       unit3 = unit7.unit,
                                       unit4 = unit8.unit,
                                       unit5 = unit1.unit,
                                       unit6 = unit2.unit,
                                       unit7 = unit3.unit,
                                       unit8 = unit4.unit,
                                   }).TakeAllOrOne(a_all, obj => obj.removed.Length > 0).ToArray();

                return new List<SudokuSolution>(from obj in jellyfishes
                                                select new SudokuSolution(SudokuSolutionType.JellyFish, obj.removed, obj.stayed, null,
                                                                          new[] { obj.unit1, obj.unit2, obj.unit3, obj.unit4, obj.unit5, 
                                                                              obj.unit6, obj.unit7, obj.unit8 }));
            }
        }

        protected override List<SudokuSolution> Solve_XYZWing(SudokuBoard a_board, bool a_all)
        {
            var xyz = (from unit1 in a_board.BoxesCells()
                       from cell1 in unit1
                       where (cell1.NumbersPossible().Count() == 3)
                       from cell2 in unit1
                       where (cell1.Index != cell2.Index) &&
                             (cell2.NumbersPossible().Count() == 2) &&
                             cell1.NumbersPossible().Contains(cell2.NumbersPossible(), Comparators.SudokuNumberComparer.Instance)
                       from cell3 in unit1
                       where (cell3.Row != cell2.Row) &&
                             (cell3.Col != cell2.Col) &&
                             (cell3.Index != cell1.Index) &&
                             (cell3.NumbersPossible().Intersect(cell1.NumbersPossible().Intersect(cell2.NumbersPossible(),
                                Comparators.SudokuNumberComparer.Instance), Comparators.SudokuNumberComparer.Instance).Count() > 0)
                       from unit2 in new[] { cell1.ColCells(), cell1.RowCells() }
                       where unit2.Contains(cell3)
                       from unit3 in new[] { cell2.ColCells(), cell2.RowCells(), cell2.BoxCells() }
                       where unit3.Contains(cell3)
                       from cell4 in unit2
                       where !unit1.Contains(cell4) &&
                             (cell4.NumbersPossible().Count() == 2) &&
                             (cell4.NumbersPossible().Intersect(cell1.NumbersPossible(), Comparators.SudokuNumberComparer.Instance).Count() == 2) &&
                             (cell4.NumbersPossible().Intersect(cell2.NumbersPossible(), Comparators.SudokuNumberComparer.Instance).Count() == 1)
                       select new
                       {
                           stayed = cell2.NumbersPossible().
                                    Concat(cell4.NumbersPossible()).
                                    Concat(cell1.NumbersPossible().Intersect(cell2.NumbersPossible().Concat(cell4.NumbersPossible()),
                                         Comparators.SudokuNumberComparer.Instance)),
                           removed = cell3.NumbersPossible().Intersect(cell2.NumbersPossible().Intersect(cell4.NumbersPossible(),
                                         Comparators.SudokuNumberComparer.Instance), Comparators.SudokuNumberComparer.Instance).ToArray(),
                           unit1 = unit1,
                           unit2 = unit2
                       }).TakeAllOrOne(a_all, obj => obj.removed.Length > 0).ToArray();

            return new List<SudokuSolution>(from obj in xyz
                                            select new SudokuSolution(SudokuSolutionType.XYZWing, obj.removed, obj.stayed, null, new [] { obj.unit1, obj.unit2 } ));
        }

        protected override List<SudokuSolution> Solve_WXYZWing(SudokuBoard a_board, bool a_all)
        {
            var wxyz = (from unit1 in a_board.BoxesCells()
                        from cell1 in unit1
                        where (cell1.NumbersPossible().Count() == 4)
                        from cell2 in unit1
                        where (cell1.Index != cell2.Index) &&
                              (cell2.NumbersPossible().Count() == 2) &&
                              cell1.NumbersPossible().Contains(cell2.NumbersPossible(), Comparators.SudokuNumberComparer.Instance)
                        from cell3 in unit1.Concat(cell1.RowCells()).Concat(cell1.ColCells())
                        where (cell3.Index != cell1.Index) &&
                              (cell3.Index != cell2.Index) &&
                              (cell3.NumbersPossible().Count() == 2) &&
                              cell1.NumbersPossible().Contains(cell3.NumbersPossible(), Comparators.SudokuNumberComparer.Instance) &&
                              (cell2.NumbersPossible().Intersect(cell3.NumbersPossible(), Comparators.SudokuNumberComparer.Instance).Count() == 1)
                        from unit2 in new[] { cell1.RowCells(), cell1.ColCells() }
                        where !unit2.Contains(cell2)
                        from cell4 in unit2.Except(unit1)
                        where (cell3.Index != cell4.Index) &&
                              (cell4.NumbersPossible().Count() == 2) &&
                              cell1.NumbersPossible().Contains(cell4.NumbersPossible(), Comparators.SudokuNumberComparer.Instance) &&
                              (cell4.NumbersPossible().Intersect(cell2.NumbersPossible(), Comparators.SudokuNumberComparer.Instance).Count() == 1) &&
                              (cell4.NumbersPossible().Intersect(cell3.NumbersPossible(), Comparators.SudokuNumberComparer.Instance).Count() == 1) &&
                              (cell4.NumbersPossible().Intersect(cell2.NumbersPossible(), Comparators.SudokuNumberComparer.Instance).
                                Intersect(cell3.NumbersPossible(), Comparators.SudokuNumberComparer.Instance).Count() == 1)
                        select new
                        {
                            stayed = cell1.NumbersPossible().Except(cell2.NumbersPossible().
                                     Intersect(cell3.NumbersPossible(), Comparators.SudokuNumberComparer.Instance)).Concat(cell2.NumbersPossible()).
                                     Concat(cell3.NumbersPossible()).Concat(cell4.NumbersPossible()),
                            removed = (from cell in unit1.Intersect(unit2).Intersect(cell3.BoxCells().Concat(cell3.RowCells()).Concat(cell3.ColCells())).
                                           Except(cell1).Except(cell3).Except(cell4)
                                       from num in cell.NumbersPossible()
                                       where (num.Number == cell2.NumbersPossible().Intersect(cell3.NumbersPossible(),
                                                Comparators.SudokuNumberComparer.Instance).First().Number)
                                       select num).ToArray(),
                            unit1 = unit1,
                            unit2 = unit2
                        }).TakeAllOrOne(a_all, obj => obj.removed.Length > 0).ToArray();

            return new List<SudokuSolution>(from obj in wxyz
                                            select new SudokuSolution(SudokuSolutionType.WXYZWing, obj.removed, obj.stayed, null, new[] { obj.unit1, obj.unit2 }));
        }
    }
}
