using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuLib
{
    public enum SudokuSolutionType
    {
        MarkImpossibles, 
        MarkSolved, 
        SinglesInUnit, 
        NakedPair, 
        NakedTriple,
        NakedQuad,
        BoxLineReduction,
        PointingTriple,
        YWing,
        MultivalueXWing,
        PointingPair,
        HiddenTriple, 
        HiddenPair, 
        HiddenQuad, 
        XWing, 
        SwordFish,
        JellyFish,
        XYZWing,
        WXYZWing
    }
}
