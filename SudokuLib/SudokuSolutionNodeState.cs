using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuLib
{
    public enum SudokuSolutionNodeState
    {
        State,
        Solution,
        Solved,
        Unsolved,
        Unsolvable
    }
}
